using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CSGenio.framework
{
    /// <summary>
    /// Classe base dos recursos
    /// </summary>
    /// <remarks>Todas as classes que implementam os recursos devem ser serializáveis</remarks>
    public abstract class Resource
    {
        /// <summary>
        /// Name do resource
        /// </summary>
        public string Name { get; protected set; }

        public Resource(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Função abstrata que deve ser implementada nas sub-classes com a lógica to obter byte array correspondente ao resource
        /// </summary>
        /// <param name="sp">Suporte persistente</param>
        /// <returns>byte array com o Qvalue a obter</returns>
        public abstract byte[] GetContent(PersistentSupport sp);

        public Resource(BinaryReader reader)
        {
            FromBinaryStream(reader);
        }

        public abstract void ToBinaryStream(BinaryWriter writer);

        public abstract void FromBinaryStream(BinaryReader reader);
    }

    public class ResourceUser : Resource
    {
        /// <summary>
        /// Ticket date
        /// </summary>
        private string Data { get; set; }
        /// <summary>
        /// ID do utilizador
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Ticket creation date
        /// </summary>
        public DateTime CreationDate => DateTime.Parse(Data);

        private ResourceUser(string name, string data, string id)
            : base(name)
        {
            this.Data = data;
            this.ID = id;
        }

        public ResourceUser(string name, string id)
            : this(name, DateTime.UtcNow.ToString(), id)
        {
        }

        public override byte[] GetContent(PersistentSupport sp)
        {
            return null;
        }

        public ResourceUser(BinaryReader reader) : base(reader) { }

        public override void ToBinaryStream(BinaryWriter writer)
        {
            writer.Write(Name ?? "");
            writer.Write(Data ?? "");
            writer.Write(ID ?? "");
        }

        public override void FromBinaryStream(BinaryReader reader)
        {
            Name = reader.ReadString();
            Data = reader.ReadString();
            ID = reader.ReadString();
        }
    }

    /// <summary>
    /// ResourceQuery has been usurped by Docums table
    /// We need a resource type to represent a binary field directly in a row
    /// </summary>
    public class ResourceBinary : Resource
    {
        public string Table { get; private set; }

        public string Field { get; private set; }

        public string Pk { get; private set; }

        public ResourceBinary(string name, string table, string dataField, string keyValue) : base(name)
        {
            Table = table;
            Field = dataField;
            Pk = keyValue;
        }

        public override byte[] GetContent(PersistentSupport sp)
        {
            /*
            DbArea area = Area.createArea(Table, user, "") as DbArea ?? throw new Exception("Invalid area");
            area.insertNamesFields(new string[]
            {
                area.Alias + "." + area.PrimaryKeyName,
                area.Alias + "." + Field ,
            });

            CriteriaSet condition = CriteriaSet.And().Equal(area.Alias, area.PrimaryKeyName, Field);
            area.selectOne(condition, new List<ColumnSort>(), "", sp);

            if (!area.AccessRightsToConsult(user))
                throw new Exception("User does not have sufficient access rights");

            return (byte[])area.returnValueField(Field);
            */

            throw new NotImplementedException("The signature of this method induces too much data having to be serialized");
            //user would need to be received in the parameters so we can use createArea, otherwise we would need to serialize the table, schema, etc...
        }

        public ResourceBinary(BinaryReader reader) : base(reader) { }

        public override void ToBinaryStream(BinaryWriter writer)
        {
            writer.Write(Name ?? "");
            writer.Write(Table ?? "");
            writer.Write(Field ?? "");
            writer.Write(Pk ?? "");
        }

        public override void FromBinaryStream(BinaryReader reader)
        {
            Name = reader.ReadString();
            Table = reader.ReadString();
            Field = reader.ReadString();
            Pk = reader.ReadString();
        }
    }

    /// <summary>
    /// Resource por query, permite obter um byte[] da db
    /// </summary>
    public class ResourceQuery : Resource
    {
		/// <summary>
		/// Name do schema to a query
		/// </summary>
		public string Schema { get; private set; }
		/// <summary>
        /// Name da table to a query
        /// </summary>
        public string Table { get; private set; }
        /// <summary>
        /// Name do Qfield que contem os dados
        /// </summary>
        public string KeyData { get; private set; }
        /// <summary>
        /// Name do Qfield key
        /// </summary>
        public string KeyField { get; private set; }
        /// <summary>
        /// Value do Qfield key do registo do qual se quer obter os dados
        /// </summary>
        public string KeyValue { get; private set; }

        public ResourceQuery(string name, IArea area, string campoDados, string keyValue)
            : this(name, area.QSystem, area.TableName, campoDados, area.PrimaryKeyName, keyValue)
        {
        }

        public ResourceQuery(IArea area, string campoDados, string keyValue)
            : this(GenerateResourceName(area.Alias, campoDados), area.QSystem, area.TableName, campoDados, area.PrimaryKeyName, keyValue)
        {
        }

        public ResourceQuery(string table, string campoDados, string campoChave, string keyValue)
            : this(GenerateResourceName(table, campoDados), null, table, campoDados, campoChave, keyValue)
        {
        }

        public ResourceQuery(string name, string table, string campoDados, string campoChave, string keyValue)
            : this(name, null, table, campoDados, campoChave, keyValue)
        {
        }

        private ResourceQuery(string name, string schema, string table, string campoDados, string campoChave, string keyValue)
            : base(name)
        {
            this.Schema = schema;
            this.Table = table;
            this.KeyData = campoDados;
            this.KeyField = campoChave;
            this.KeyValue = keyValue;
        }

        /// <summary>
        /// Create a resource name with a random sufix to avoid overriding the same files
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Qfield"></param>
        /// <returns></returns>
        public static string GenerateResourceName(string table, string Qfield)
        {
            string suf = Guid.NewGuid().ToString();
            // a forma de nomear os ficheiros pode ser alterada, visto que já não são gravados na pasta temp
            string file = "imagem" + "_" + table + "_" + Qfield + suf + ".jpg";
            return file;
        }

        /// <summary>
        /// Executa a query sobre a table e devolve o byte array com o Qvalue do Qfield
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns>byte array com o Qvalue a obter da db</returns>
        public override byte[] GetContent(PersistentSupport sp)
        {
            if (Configuration.Files2Disk)
            {
                //The GetContent methods need to be phased out and deleted since new methods in DbArea are the ones being maintained
                SelectQuery qs = new SelectQuery()
                    .Select("docums", "docpath")
                    .Select("docums", "document")
                    .From(Schema, "docums", "docums")
                    .Where(CriteriaSet.And()
                        .Equal("docums", "coddocums", KeyValue))
                    .PageSize(1);

                var doc = sp.executeReaderOneRow(qs);

                if (doc == null || doc.Count != 2)
                    return null;

                //try to read from the file
                var file = Convert.ToString(doc[0]);
                if(!string.IsNullOrEmpty(file))
                    return PersistentSupport.getFileFromDisk(file);

                //if it fails then try to read from the binary
                return (byte [])doc[1];
            }
            else //same query for compatibility purpuses
            {
                SelectQuery qs = new SelectQuery()
                    .Select(Table, KeyData)
                    .From(Schema, Table, Table)
                    .Where(CriteriaSet.And()
                        .Equal(Table, KeyField, KeyValue))
                    .PageSize(1);

                return (byte[])sp.ExecuteScalar(qs);
            }
        }

        public ResourceQuery(BinaryReader reader) : base(reader) { }

        public override void ToBinaryStream(BinaryWriter writer)
        {
            writer.Write(Name ?? "");
            writer.Write(Table ?? "");
            writer.Write(KeyData ?? "");
            writer.Write(KeyField ?? "");
            writer.Write(KeyValue ?? "");
        }

        public override void FromBinaryStream(BinaryReader reader)
        {
            Name = reader.ReadString();
            Table = reader.ReadString();
            KeyData = reader.ReadString();
            KeyField = reader.ReadString();
            KeyValue = reader.ReadString();
        }
    }

    /// <summary>
    /// Resource por file, permite obter um byte[] com o conteúdo do mesmo
    /// </summary>
    public class ResourceFile : Resource
    {
        /// <summary>
        /// QPath do file no servidor. Pode ser um path de rede
        /// </summary>
        public string FilePath { get; private set; }

        public ResourceFile(string name, string caminhoFicheiro) : base(name)
        {
            this.FilePath = caminhoFicheiro;
        }

        /// <summary>
        /// Lê um file e devolve um byte array com o seu conteúdo
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns>byte array com o conteúdo do file</returns>
        public override byte[] GetContent(PersistentSupport sp)
        {
            return File.ReadAllBytes(FilePath);
        }

        public ResourceFile(BinaryReader reader) : base(reader) { }

        public override void ToBinaryStream(BinaryWriter writer)
        {
            writer.Write(Name ?? "");
            writer.Write(FilePath ?? "");
        }

        public override void FromBinaryStream(BinaryReader reader)
        {
            Name = reader.ReadString();
            FilePath = reader.ReadString();
        }
    }

    /// <summary>
    /// Temporary Resource file, permite obter um byte[] com o conteúdo do mesmo apagando o documento no final
    /// </summary>
    public class TemporaryResourceFile : ResourceFile
    {
        public TemporaryResourceFile(string name, string caminhoFicheiro) : base(name, caminhoFicheiro) { }

        /// <summary>
        /// Lê um file, apaga o ficheiro e devolve um byte array com o seu conteúdo
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns>byte array com o conteúdo do file</returns>
        public override byte[] GetContent(PersistentSupport sp)
        {
            byte[] fbytes = File.ReadAllBytes(this.FilePath);

            FileInfo file = new FileInfo(this.FilePath);
            if (file.Exists)
                file.Delete();

            return fbytes;
        }

        public TemporaryResourceFile(BinaryReader reader) : base(reader) { }
    }

    /// <summary>
    /// Classe auxiliar to ajudar a leitura e criação de tickets de recursos serializados e cifrados
    /// </summary>
    public static class QResources
    {
        private static ConcurrentDictionary<string, Type> ResourceTypes { get; set; } = new ConcurrentDictionary<string, Type>();

        // a key e o vector de inicialização que são re-gerados sempre que o application pool arranca
        // o que implica que os tickets gerados anteriormente ficam inválidos to leitura

        // key de cifra simétrica utilizada to cifrar e decifrar os recursos
        private static byte[] m_key;

        private static byte[] Key
        {
            get { InitializeValues(); return m_key; }
        }

        // cria uma nova key e vector de inicialização, caso algum deles não esteja definido
        private static void InitializeValues()
        {
            if (m_key == null)
            {
                // derivar as passwords de forma deterministica a partir de uma password
                byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };
                var passphrase = Configuration.GetProperty("RESTSECRET", "default_secret_please_reconfigure_with_jwtkey_property");
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, SALT);
                m_key = pdb.GetBytes(32);
            }
        }

        /// <summary>
        /// Serializa, cifra e gera uma string em base 64 com a location e o resource.
        /// A localização é um parametro de validação, na web é utilizado o endereço IP do cliente.
        /// </summary>
        /// <param name="localizacao">Localização, serve to validação</param>
        /// <param name="recurso">Resource em si</param>
        /// <param name="allowWrite">Whether write operations to the DB are allowed when using this ticket</param>
        /// <returns>string em base 64 com a representação dos objectos serializados e cifrados</returns>
        public static string CreateTicketEncryptedBase64(string username, string location, Resource resource, bool allowWrite = true)
        {
            byte[] objsByteArray;
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(username);
                writer.Write(location);
                Type type = resource.GetType();
                writer.Write(type.FullName);
                writer.Write(type.Assembly.GetName().Name);
                writer.Write(allowWrite);
                resource.ToBinaryStream(writer);
                objsByteArray = stream.ToArray();
            }

            // a key e o vector de inicialização são acedidos através das
            // propriedades da classe to garantir que foram inicializados
            byte[] objsCrypt = CryptographicFunctions.EncryptData(Key, objsByteArray);
            return Convert.ToBase64String(objsCrypt).Replace('+', '.').Replace('=', '-').Replace('/', '_');
        }

        /// <summary>
        /// Converte a representação de um ticket em base 64 to os objectos que foram utilizados na sua criação,
        /// decifrando e desserializando o seu conteúdo.
        /// Por agora, to criar o ticket são utilizados dois objectos, o primeiro é o endereço ip do cliente,
        /// to validação e o second é o resource em si.
        /// </summary>
        /// <param name="ticket64">Representação do ticket em base 64</param>
        /// <returns>Array de objectos utilizados na criação do ticket</returns>
        public static object[] DecryptTicketBase64(string ticket64)
        {
            byte[] ticket = Convert.FromBase64String(ticket64.Replace('.', '+').Replace('-', '=').Replace('_', '/'));
            // a key e o vector de inicialização são acedidos através das
            // propriedades da classe to garantir que foram inicializados
            byte[] ticketClean = CryptographicFunctions.DecryptData(Key, ticket);

            object[] objs = new object[4];
            using (var stream = new MemoryStream(ticketClean))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                objs[0] = reader.ReadString(); //username
                objs[1] = reader.ReadString(); //location
                string typeName = reader.ReadString();
                string assemblyName = reader.ReadString();
                string searchTypeName = $"{typeName}, {assemblyName}";

                if (!ResourceTypes.TryGetValue(searchTypeName, out Type type))
                {
                    ResourceTypes.GetOrAdd(searchTypeName, key =>
                    {
                        type = Type.GetType(searchTypeName, false, true);
                        if (type != null && type.IsSubclassOf(typeof(Resource)))
                            return type;
                        else
                            throw new Exception("Unknown type for Resource deserialization");
                    });
                }

                objs[3] = reader.ReadBoolean(); // Are writes allowed?
                // Invoke the constructor using the BinaryReader parameter
                objs[2] = type.GetConstructor([typeof(BinaryReader)]).Invoke([reader]);
            }

            return objs;
        }
        
        /// <summary>
        /// Serializa, cifra e gera uma string em base 64.
        /// </summary>
        /// <param name="recurso">Resource em si</param>
        /// <returns>string em base 64 com a representação dos objectos serializados e cifrados</returns>
        public static string CreatePayloadEncryptedBase64(string payload)
        {
            // a key e o vector de inicialização são acedidos através das
            // propriedades da classe to garantir que foram inicializados
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            byte[] objsCrypt = CryptographicFunctions.EncryptData(Key, payloadBytes);
            return Convert.ToBase64String(objsCrypt).Replace('+', '.').Replace('=', '-').Replace('/', '_');
        }

        /// <summary>
        /// Converte a representação de um ticket em base 64 to os objectos que foram utilizados na sua criação,
        /// decifrando e desserializando o seu conteúdo.
        /// </summary>
        /// <param name="ticket64">Representação do ticket em base 64</param>
        /// <returns>A string original</returns>
        public static string DecryptPayloadBase64(string ticket64)
        {
            byte[] ticket = Convert.FromBase64String(ticket64.Replace('.', '+').Replace('-', '=').Replace('_', '/'));
            // a key e o vector de inicialização são acedidos através das
            // propriedades da classe to garantir que foram inicializados
            byte[] ticketClean = CryptographicFunctions.DecryptData(Key, ticket);
            return Encoding.UTF8.GetString(ticketClean);
        }
    }

	public static class QResourcesSign
    {
        // a key e o vector de inicialização que são re-gerados sempre que o application pool arranca
        // o que implica que os tickets gerados anteriormente ficam inválidos to leitura

        // key de cifra simétrica utilizada to cifrar e decifrar os recursos
        private static byte[] m_key;

        //public static Dictionary<string, Type> ResourceTypes { get; set; } = new Dictionary<string, Type>();

        private static byte[] Key
        {
            get { InitializeValues(); return m_key; }
        }

        // cria uma nova key e vector de inicialização, caso algum deles não esteja definido
        private static void InitializeValues()
        {
            if (m_key == null)
            {
                // instancia-se um objecto da classe apenas to gerar as chaves
                RijndaelManaged dummyRijndael = new RijndaelManaged();
                m_key = dummyRijndael.Key;
            }
        }

        /// <summary>
        /// Serializa, cifra e gera uma string em base 64 com a location e o resource.
        /// A localização é um parametro de validação, na web é utilizado o endereço IP do cliente.
        /// </summary>
        /// <param name="localizacao">Localização, serve to validação</param>
        /// <param name="recurso">Resource em si</param>
        /// <returns>string em base 64 com a representação dos objectos serializados e cifrados</returns>
        public static string CreateTicketEncryptedBase64(string username, string location, ResourceSign resource)
        {
            byte[] objsByteArray;
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(username);
                writer.Write(location);
                Type type = resource.GetType();
                writer.Write(type.FullName);
                writer.Write(type.Assembly.GetName().Name);

                resource.ToBinaryStream(writer);
                objsByteArray = stream.ToArray();
            }

            // a key e o vector de inicialização são acedidos através das
            // propriedades da classe to garantir que foram inicializados
            byte[] objsCrypt = CryptographicFunctions.EncryptData(Key, objsByteArray);
            return Convert.ToBase64String(objsCrypt).Replace('+', '.').Replace('=', '-').Replace('/', '_');
        }

        /// <summary>
        /// Converte a representação de um ticket em base 64 to os objectos que foram utilizados na sua criação,
        /// decifrando e desserializando o seu conteúdo.
        /// Por agora, to criar o ticket são utilizados dois objectos, o primeiro é o endereço ip do cliente,
        /// to validação e o second é o resource em si.
        /// </summary>
        /// <param name="ticket64">Representação do ticket em base 64</param>
        /// <returns>Array de objectos utilizados na criação do ticket</returns>
        public static object[] DecryptTicketBase64(string ticket64)
        {
            byte[] ticket = Convert.FromBase64String(ticket64.Replace('.', '+').Replace('-', '=').Replace('_', '/'));
            // a key e o vector de inicialização são acedidos através das
            // propriedades da classe to garantir que foram inicializados
            byte[] ticketClean = CryptographicFunctions.DecryptData(Key, ticket);

            object[] objs = new object[3];
            using (var stream = new MemoryStream(ticketClean))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                objs[0] = reader.ReadString(); //username
                objs[1] = reader.ReadString(); //location
                string typename = reader.ReadString();
                string assemblyName = reader.ReadString();

                Type type = Type.GetType($"{typename}, {assemblyName}", true, true);

                if (type != null && type.IsSubclassOf(typeof(ResourceSign)))
                    objs[2] = (ResourceSign)Activator.CreateInstance(type, reader);
                else
                    throw new Exception("Unknown type for ResourceSign deserialization");
            }

            return objs;
        }

    }

    public abstract class ResourceSign
    {
        public ResourceSign() { }

        public ResourceSign(BinaryReader reader)
        {
            FromBinaryStream(reader);
        }

        public abstract void ToBinaryStream(BinaryWriter writer);
        public abstract void FromBinaryStream(BinaryReader reader);
        public abstract void Sign(PersistentSupport sp, User user, byte[] file);
		public abstract string GetDefaultErrorMessage(PersistentSupport sp, User user);
    }
}
