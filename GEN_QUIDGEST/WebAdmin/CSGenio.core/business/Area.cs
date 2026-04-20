using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Concurrent;

namespace CSGenio.business
{
    /// <summary>
    /// Classe genérica que representa uma área, todas as áreas criadas deverão extender esta
    /// classe e implementar os métodos abstractos
    /// Os atributos:
    /// - fields é uma hash cujo identifier é (alias+fieldName), cada entrada aponta to um Field
    /// todasAreas hashtable que permite obter todas as áreas
    /// </summary>
    public abstract class Area : IArea
    {
        public static AreaRef AreaCTAX { get { return m_AreaCTAX; } }
        private static AreaRef m_AreaCTAX = new AreaRef("FOR", "forctax", "ctax");
        public static AreaRef AreaCONTA { get { return m_AreaCONTA; } }
        private static AreaRef m_AreaCONTA = new AreaRef("FOR", "forcontact", "conta");
        public static AreaRef AreaCITY { get { return m_AreaCITY; } }
        private static AreaRef m_AreaCITY = new AreaRef("FOR", "forcity", "city");
        public static AreaRef AreaS_UA { get { return m_AreaS_UA; } }
        private static AreaRef m_AreaS_UA = new AreaRef("FOR", "userauthorization", "s_ua");
        public static AreaRef AreaPHOTO { get { return m_AreaPHOTO; } }
        private static AreaRef m_AreaPHOTO = new AreaRef("FOR", "forphoto", "photo");
        public static AreaRef AreaMEM { get { return m_AreaMEM; } }
        private static AreaRef m_AreaMEM = new AreaRef("FOR", "formem", "mem");
        public static AreaRef AreaS_NES { get { return m_AreaS_NES; } }
        private static AreaRef m_AreaS_NES = new AreaRef("FOR", "notificationemailsignature", "s_nes");
        public static AreaRef AreaS_NM { get { return m_AreaS_NM; } }
        private static AreaRef m_AreaS_NM = new AreaRef("FOR", "notificationmessage", "s_nm");
        public static AreaRef AreaS_PAX { get { return m_AreaS_PAX; } }
        private static AreaRef m_AreaS_PAX = new AreaRef("FOR", "asyncprocessattachments", "s_pax");
        public static AreaRef AreaPROPE { get { return m_AreaPROPE; } }
        private static AreaRef m_AreaPROPE = new AreaRef("FOR", "forproperty", "prope");
        public static AreaRef AreaS_ARG { get { return m_AreaS_ARG; } }
        private static AreaRef m_AreaS_ARG = new AreaRef("FOR", "asyncprocessargument", "s_arg");
        public static AreaRef AreaPSW { get { return m_AreaPSW; } }
        private static AreaRef m_AreaPSW = new AreaRef("FOR", "userlogin", "psw");
        public static AreaRef AreaS_APR { get { return m_AreaS_APR; } }
        private static AreaRef m_AreaS_APR = new AreaRef("FOR", "asyncprocess", "s_apr");
        public static AreaRef AreaCOUNT { get { return m_AreaCOUNT; } }
        private static AreaRef m_AreaCOUNT = new AreaRef("FOR", "forcount", "count");
        public static AreaRef AreaCBORN { get { return m_AreaCBORN; } }
        private static AreaRef m_AreaCBORN = new AreaRef("FOR", "forcount", "cborn");
        public static AreaRef AreaCADDR { get { return m_AreaCADDR; } }
        private static AreaRef m_AreaCADDR = new AreaRef("FOR", "forcount", "caddr");
        public static AreaRef AreaAGENT { get { return m_AreaAGENT; } }
        private static AreaRef m_AreaAGENT = new AreaRef("FOR", "foragent", "agent");
        //areas hardcoded
        public static AreaRef AreaDELEGA { get { return m_AreaDELEGA; } }
        private static AreaRef m_AreaDELEGA = new AreaRef("FORdelega", "delega");
        public static AreaRef AreaPSWUP { get { return m_AreaPSWUP; } }
        private static AreaRef m_AreaPSWUP = new AreaRef("UserLogin", "pswup");
        public static AreaRef AreaMQQUEUES { get { return m_AreaMQQUEUES; } }
        private static AreaRef m_AreaMQQUEUES = new AreaRef("FORmqqueues", "mqqueues");
        public static AreaRef AreaUSERAUTHORIZATION { get { return m_AreaUSERAUTHORIZATION; } }
        private static AreaRef m_AreaUSERAUTHORIZATION = new AreaRef("userauthorization", "userauthorization");
        public static AreaRef AreaNOTIFICATIONEMAILSIGNATURE { get { return m_AreaNOTIFICATIONEMAILSIGNATURE; } }
        private static AreaRef m_AreaNOTIFICATIONEMAILSIGNATURE = new AreaRef("notificationemailsignature", "notificationemailsignature");
        public static AreaRef AreaNOTIFICATIONMESSAGE { get { return m_AreaNOTIFICATIONMESSAGE; } }
        private static AreaRef m_AreaNOTIFICATIONMESSAGE = new AreaRef("notificationmessage", "notificationmessage");
        public static AreaRef AreaTBLCFG { get { return m_AreaTBLCFG; } }
        private static AreaRef m_AreaTBLCFG = new AreaRef("FOR", "FORtblcfg", "tblcfg");
        public static AreaRef AreaLSTUSR { get { return m_AreaLSTUSR; } }
        private static AreaRef m_AreaLSTUSR = new AreaRef("FORlstusr", "lstusr");
        public static AreaRef AreaLSTCOL { get { return m_AreaLSTCOL; } }
        private static AreaRef m_AreaLSTCOL = new AreaRef("FORlstcol", "lstcol");
        public static AreaRef AreaUSRCFG { get { return m_AreaUSRCFG; } }
        private static AreaRef m_AreaUSRCFG = new AreaRef("FORusrcfg", "usrcfg");
        public static AreaRef AreaLSTREN { get { return m_AreaLSTREN; } }
        private static AreaRef m_AreaLSTREN = new AreaRef("FORlstren", "lstren");
        public static AreaRef AreaUSRWID { get { return m_AreaUSRWID; } }
        private static AreaRef m_AreaUSRWID = new AreaRef("FORusrwid", "usrwid");

        /// <summary>
        /// Lista de areas
        /// </summary>
        public static readonly System.Collections.ObjectModel.ReadOnlyCollection<string> ListaAreas = new System.Collections.ObjectModel.ReadOnlyCollection<string>(
            new List<string>() {
            "ctax",
            "conta",
            "city",
            "s_ua",
            "photo",
            "mem",
            "s_nes",
            "s_nm",
            "s_pax",
            "prope",
            "s_arg",
            "psw",
            "s_apr",
            "count",
            "cborn",
            "caddr",
            "agent",
        });

        /// <summary>
        /// User
        /// </summary>
        protected User user;
        /// <summary>
        /// Module
        /// </summary>
        protected string module;
        /// <summary>
        /// fields da table
        /// </summary>
        protected Hashtable fields = new Hashtable();


        //Static class accessed a lot during startup, must have concurrency concerns
        private static ConcurrentDictionary<string, Type> m_areaRegistry = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Função que dado o identifier da area devolve um objecto da mesma
        /// </summary>
        /// <param name="nome">name da Area</param>
        /// <param name="utilizador">O contexto de user</param>
        /// <returns>Area correspondente</returns>
        public static Area createArea(string name, User user, string module)
        {
            if (name == null)
                throw new BusinessException(null, "Area.criarArea", "Argument [nome] is null.");

            //RS 21.03.2011 Apaguei as listas de areas e passei a usar reflection to obter a informação de uma area
            // Isto permite reduzir o time de inicialização na primeira chamada ao servidor de forma substancial
            // uma vez que existiam problemas em 64 bits (podia demorar 2 minutes)

            Type areaType = GetTypeArea(name);

            return createArea(areaType, user, module);
        }

        /// <summary>
        /// Creates a new record with its current values set to the bookmarked values of another record
        /// </summary>
        /// <param name="other">The area to copy values from</param>
        /// <returns>A new record</returns>
        public static Area createFromBookmark(Area other)
        {
            Area area = createArea(other.GetType(), other.User, other.Module);
            foreach(var field in other.Fields)
                area.insertNameValueField(field.Key, field.Value.OldValue, true);
            area.IsBookmarkLocked = other.IsBookmarkLocked;
            area.UserRecord = other.UserRecord;
            return area;
        }

        /// <summary>
        /// Returns the type of the area from the area name. Also caches the type in memory
        /// </summary>
        /// <param name="name">Area Id</param>
        /// <returns></returns>
        public static Type GetTypeArea(string name)
        {
            if (name == null)
                throw new BusinessException(null, "Area.criarArea", "Argument [nome] is null.");

            if(!m_areaRegistry.ContainsKey(name)) {
                m_areaRegistry[name] = LoadType(name);
            }
            return  m_areaRegistry[name];
        }


        private static Type LoadType(string name) {
            const string classPrefix = "CSGenio.business.CSGenioA";
            //We need to pass an hint for the assembly, or it will only search in CSGenio.core
            string areaName = classPrefix + name + ", GenioServer";
            var type = Type.GetType(areaName);
            //Since there are much more assemblies in GenioServer, we search for CSGenio.core only after not finding in GenioServer.
            if(type == null)
            {
                areaName = classPrefix + name + ", CSGenio.core";
                type = Type.GetType(areaName);
            }
            return type;
        }

        /// <summary>
        /// Função que dado o tipo da area devolve um objecto da mesma
        /// </summary>
        /// <typeparam name="TArea">O tipo da area</typeparam>
        /// <param name="utilizador">O contexto de user</param>
        /// <returns>Area correspondente</returns>
        public static TArea createArea<TArea>(User user, string module) where TArea : Area
        {
            return (TArea)createArea(typeof(TArea), user, module);
        }

        /// <summary>
        /// Função que dado o identifier da area devolve um objecto da mesma
        /// </summary>
        /// <param name="nome">name da Area</param>
        /// <param name="utilizador">O contexto de user</param>
        /// <returns>Area correspondente</returns>
        private static Area createArea(Type areaType, User user, string module)
        {
            if (areaType == null)
                throw new BusinessException(null, "Area.criarArea", "Argument [areaType] is null.");

            Area result = System.Activator.CreateInstance(areaType, user, module) as Area;
            if (result == null)
                throw new BusinessException(null, "Area.criarArea", "CreateInstance returned null.");

            return result;
        }

        private bool m_isFichaUtilizador = true;
        /// <summary>
        /// True se a ficha deve autenticar e carimbar o user, false caso seja o negócio
        /// </summary>
        /// <remarks>
        /// Os métodos desta classe vão considerar a ficha como sendo gravada pelo user
        ///  e vão autorizar e carimbar-la como tal. Sempre que esse comportamente não é desejado
        ///  esta propriadade deve ser explicitamente colocada a false antes dessas operações.
        /// Tipicamente isto é necessário em operações em cascata ou outras rotinas manuais
        ///  automáticas e deve ser feito imediatamente após instanciar a área. É esperado que
        ///  durante a vida dessa instancia este Qvalue não mude mas o servidor não garante nem
        ///  assume que tal possa acontecer.
        /// </remarks>
        public bool UserRecord
        {
            get { return m_isFichaUtilizador; }
            set { m_isFichaUtilizador = value; }
        }

        /// <summary>
        /// True if the record is to be validated, false if not
        /// </summary>
        /// <remarks>
        /// By default, this field will be set as true, to prevent the storage of invalid records.
        /// However, certain situations require the validations to be delayed or even not occur - in these cases,
        /// it is preferable to alter this property instead of the UserRecord flag, since that is used for several
        /// other cases outside of the validation scope.
        /// </remarks>
        public bool NeedsValidation { get; set; } = true;

        /// <summary>
        /// True se as validações de valores dos campos se devem forçar a validação se o campo é null ou não
        /// </summary>
        private bool m_validateIfIsNull = false;
        public bool ValidateIfIsNull
        {
            get { return m_validateIfIsNull; }
            set { m_validateIfIsNull = value; }
        }

        /// <summary>
        /// Finds the meta information about a field reference
        /// </summary>
        /// <param name="fieldRef"></param>
        /// <returns>The Field infomation related to the field reference</returns>
        public static Field GetFieldInfo(FieldRef fieldRef)
        {
            return GetInfoArea(fieldRef.Area).DBFields[fieldRef.Field];
        }

        /// <summary>
        /// Obtem a informação sobre uma area dado o seu name
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public static AreaInfo GetInfoArea(string name)
        {
            //RS 21.03.2011 Apaguei as listas de areas e passei a usar reflection to obter a informação de uma area
            // Isto permite reduzir o time de inicialização na primeira chamada ao servidor de forma substancial
            // uma vez que existiam problemas em 64 bits (podia demorar 2 minutes)

            Type areaType = GetTypeArea(name);
            if (areaType == null)
                throw new BusinessException(null, "Area.GetInfoArea", "Argument [areaType] is null.");

            return GetInfoArea(areaType);
        }

		/// <summary>
        /// Obtem a informação sobre uma area dado o seu tipo
        /// </summary>
        /// <returns></returns>
		public static AreaInfo GetInfoArea<A>() where A : Area
        {
            return GetInfoArea(typeof(A));
        }

        private static AreaInfo GetInfoArea(Type t)
        {
            return t.InvokeMember("GetInformation"
                , System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod
                , null
                , null
                , null) as AreaInfo;
        }


        /// <summary>
        /// Função que devolve a lista de todas as queues
        /// </summary>
        /// <returns>Lista de queues</returns>
        public static List<GenioServer.business.QueueGenio>  GetAllQueues()
        {
            List<GenioServer.business.QueueGenio> listaQueues = new List<GenioServer.business.QueueGenio>();
            return listaQueues;
        }

        /// <summary>
        /// Clones the properties of another IArea instance into this instance.
        /// </summary>
        /// <param name="other">The IArea instance to clone from.</param>
        /// <exception cref="InvalidOperationException">Thrown when the alias of the provided Area instance does not match the current instance's alias.</exception>
        public void CloneFrom(IArea other)
        {
            if (Alias != other.Alias)
                throw new InvalidOperationException($"Alias mismatch: Unable to clone from the provided Area instance. Expected alias '{Alias}', but received '{other.Alias}'.");

            foreach(var kvp in other.Fields)
                insertNameValueField(kvp.Key, kvp.Value.Value);
        }

        /// <summary>
        /// Initializes the requested fields with a new list
        /// </summary>
        /// <param name="fieldNames">The fields being requested</param>
        public void insertNamesFields(string[] fieldNames)
        {
            Fields.Clear();
            foreach (string name in fieldNames)
                insertNameValueField(name, null);
        }

        /// <summary>
        /// Metodo auxiliar que Insere ou Altera os nomes e Qvalues dos fields na área
        /// </summary>
        /// <param name="nomeCampos">nomes dos fields</param>
        /// <param name="valoresCampos">Qvalues dos fields que estão em string</param>
        /// <param name="inserir">Bool to indicar se é chamada to introduce</param>
        /// <param name="acrescentar">Bool to indicar se é chamada to acrescentar</param>
        private void AuxNomesValoresCampos(string[] fieldNames, string[] fieldsvalues, bool introduce, bool acrescentar)
        {
            string function;
            if (introduce)
                function = "Area.addNamesValuesFields";
            else if (acrescentar)
                function = "Area.insertNamesValuesFields";
            else
                throw new BusinessException(null, "Area.AuxNomesValoresCampos", "Arguments [inserir] and [acrescentar] are both false.");
            try
            {
                if ((introduce && (fieldNames.Length == 0 || fieldsvalues.Length == 0)) || fieldNames.Length != fieldsvalues.Length)
                    throw new BusinessException(null, "Area.AuxNomesValoresCampos", "Lengths of nomeCampos and valoresCampos don't match or one of these parameters has length 0.");

                if (introduce)
                    Fields.Clear();

                for (int i = 0; i < fieldNames.Length; i++)
                {
                    string fieldName = fieldNames[i];

                    //support for non-fully-qualified names
                    Field Qfield;
                    if (!fieldName.Contains("."))
                    {
                        Qfield = DBFields[fieldName];
                        fieldName = Qfield.FullName;
                    }

                    RequestedField campoPedido = new RequestedField(fieldName, Alias);

                    if (campoPedido.BelongsArea && acrescentar && PrimaryKeyName.Equals(campoPedido.Name))
                        continue;

                    if (!campoPedido.WithoutArea)
                    {
                        if(campoPedido.BelongsArea)
                            Qfield = DBFields[campoPedido.Name];
                        else
                            Qfield = Area.GetInfoArea(campoPedido.Area).DBFields[campoPedido.Name];

                        //SO 20061207 validação dos fields não nulos
                        if (Qfield.NotNull && Qfield.DefaultValue == null)
                        {
                            if (fieldsvalues[i].Equals(""))
                                throw new BusinessException("O campo " + Qfield.FieldDescription + " (" + Qfield.Alias + "." + Qfield.Name + ")  é obrigatório mas não está preenchido.", function, "The field " + Qfield.FieldDescription + " (" + Qfield.Alias + "." + Qfield.Name + ")  is mandatory but is not filled.");
                        }

                        if (Qfield.FieldSize < 0 && Qfield.FieldType == FieldType.TEXT && Qfield.FieldSize < fieldsvalues[i].ToString().Length)
                            throw new BusinessException("O campo " + Qfield.FieldDescription + " excede a dimensão máxima permitida.", "Area.AuxNomesValoresCampos", "The field " + Qfield.FieldDescription + " exceeds the maximum length allowed.");

                        campoPedido.FieldType = Qfield.FieldType;
                        if (campoPedido.FieldType.Equals(FieldType.IMAGE))
                            throw new BusinessException("Erro ao gravar a imagem.", "Area.AuxNomesValoresCampos", "The field type JPEG image is not supported by function " + function);

                        // RR 01-04-2011 - os fields tipo path e file db não são geridos a este nível, mas sim a um nível superior
                        campoPedido.Value = Conversion.string2TypeInternal(fieldsvalues[i], Qfield.FieldType.GetFormatting());
                    }

                    if (!Fields.ContainsKey(fieldName))
                        Fields.Add(fieldName, campoPedido);
                    else if (acrescentar)
                        Fields[fieldName] = campoPedido;
                }
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "Area.AuxNomesValoresCampos")
                    throw;
                throw new BusinessException(ex.UserMessage, "Area.AuxNomesValoresCampos", "Error inserting or changing fields names and values in Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.AuxNomesValoresCampos", "Error inserting or changing fields names and values in Area: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Insere os nomes e Qvalues dos fields na área
        /// </summary>
        /// <param name="nomeCampos">nomes dos fields</param>
        /// <param name="valoresCampos">Qvalues dos fields que estão em string</param>
        public void insertNamesValuesFields(string[] fieldNames, string[] fieldsvalues)
        {
            AuxNomesValoresCampos(fieldNames, fieldsvalues, true, false);
        }

        /// <summary>
        /// Acrescenta os nomes e Qvalues dos fields na área caso não existam. Actualiza os Qvalues caso contrário
        /// </summary>
        /// <param name="nomeCampos">nomes dos fields</param>
        /// <param name="valoresCampos">Qvalues dos fields que estão em string</param>
        public void addNamesValuesFields(string[] fieldNames, string[] fieldsvalues)
        {
            AuxNomesValoresCampos(fieldNames, fieldsvalues, false, true);
        }

        /// <summary>
        /// Initializes all the non-requested fields with empty value
        /// </summary>
        public void createEmptyFields()
        {
            foreach(Field fieldInfo in DBFields.Values)
            {
                if (!Fields.ContainsKey(fieldInfo.FullName))
                {
                    var campoPedido = new RequestedField(fieldInfo.FullName, Alias);
                    campoPedido.FieldType = fieldInfo.FieldType;
                    campoPedido.Value = fieldInfo.GetValorEmpty();
                    Fields.Add(campoPedido.FullName, campoPedido);
                }
            }
        }

        /// <summary>
        /// Sets the value of a field
        /// </summary>
        /// <param name="fieldName">
        /// The name of the field to update. Supports both "area.field" (fully qualified) and "field" formats.
        /// If the field has a database name, it is expected to be provided instead of the field id.
        /// </param>
        /// <param name="fieldValue">The value to assign to the field.</param>
        /// <param name="fromDatabase">True if the value is being read directly from the database, so it can be used as a bookmarked value</param>
        /// <exception cref="BusinessException">
        /// Thrown if the specified field does not exist or if an error occurs during the update.
        /// </exception>
        public void insertNameValueField(string fieldName, object fieldValue, bool fromDatabase = false)
        {
            try
            {
                FieldType fieldType;
                Field fieldInfo = null;

                //support for non-fully-qualified names
                if(!fieldName.Contains("."))
                {
                    fieldInfo = DBFields[fieldName];
                    fieldName = fieldInfo.FullName;
                }

                //if we haven't requested the field yet, then request it
                if(!Fields.TryGetValue(fieldName, out RequestedField campoPedido))
                {
                    campoPedido = new RequestedField(fieldName, Alias);
                    Fields.Add(fieldName, campoPedido);
                }

                //field belongs to this area
                if (campoPedido.BelongsArea)
                {
                    fieldInfo ??= DBFields[campoPedido.Name];
                    fieldType = fieldInfo.FieldType;
                    campoPedido.FieldType = fieldType;
                    campoPedido.Value = Conversion.internal2InternalValid(fieldValue, fieldType.GetFormatting());
                    trimPrecision(campoPedido);
                    //set the bookmark if the caller indicated the value is coming from the database
                    if (fromDatabase)
                        campoPedido.OldValue = campoPedido.Value;
                }
                //field belongs to another area
                else
                {
                    //----------------------------------------------------------------
                    //Este código só é executado na sequancia de um pedido GET1.
                    //TODO: Convinha tentar fazer de outra forma. Não faz muito sentido
                    //  uma area guardar fields de outra area
                    //System.Diagnostics.Debug.Assert(false);

                    if (!campoPedido.WithoutArea)
                    {
                        //SO 2007.05.29
                        fieldInfo = Area.GetInfoArea(campoPedido.Area).DBFields[campoPedido.Name];
                        fieldType = fieldInfo.FieldType;
                        campoPedido.FieldType = fieldType;
                        campoPedido.Value = Conversion.internal2InternalValid(fieldValue, fieldType.GetFormatting());
                    }
                    //----------------------------------------------------------------
                }

            }
            catch (GenioException ex)
            {
                string message = $"Error inserting value in field {fieldName} in area {this.Alias}: {ex.Message}";
                throw new BusinessException(ex.UserMessage, "Area.insertNameValueField", message, ex);

            }
            catch (Exception ex)
            {
                string message = $"Error inserting value in field {fieldName} in area {this.Alias}: {ex.Message}";
                throw new BusinessException(message, "Area.insertNameValueField", message, ex);
            }
        }

        /// <summary>
        /// Trims a field to its declared maximum precision to prevent discrepancies with the database values
        /// </summary>
        /// <param name="field">The requested field to trim</param>
        /// <remarks>
        /// This method assumes the field value was previously normalized to its valid internal type.
        /// This trim is necessary because if you allow temporary values to retain more precision during calculations
        ///  then that extra precision can add up to differences to a calculation done in the SQL fields.
        /// This can happen anywhere but will most commonly happen in SR, for example:
        /// A = [+] (B + C + D) / 3
        /// S = [SR] A
        /// </remarks>
        private void trimPrecision(RequestedField field)
        {
            if(field.FieldType.GetFormatting() == FieldFormatting.FLOAT)
            {
                var dec = DBFields[field.Name].Decimals;
                field.Value = Math.Round((decimal)field.Value, dec, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Obtains the internal value set for the requested field
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Value of the field</returns>
        public object returnValueField(string fieldName)
        {
            try
            {
                string area;
                string name;
                //support for non-fully-qualified names
                var ix = fieldName.IndexOf('.');
                if(ix == -1)
                {
                    area = Alias;
                    name = fieldName;
                    fieldName = DBFields[name].FullName;
                }
                else
                {
                    area = fieldName.Substring(0, ix);
                    name = fieldName.Substring(ix + 1);
                }

                if (Alias == area)
                {
                    var fieldInfo = DBFields[name];
                    var primaryKeyField = DBFields[this.PrimaryKeyName];

                    if (
                        fieldInfo.IsClientSide &&
                        !primaryKeyField.isEmptyValue(this.QPrimaryKey)
                    )
                    {
                        //queries inside these calculations are not supported
                        //using fields from other tables inside these calculations is not supported
                        var formula = fieldInfo.Formula as InternalOperationFormula;
                        return formula.calculateInternalFormula(this, null, null, FunctionType.ALT);
                    }
                }

                if(Fields.TryGetValue(fieldName, out RequestedField requested))
                    return requested.Value;
                else
                    return DBFields[name].GetValorEmpty();
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.returnValueField", "Error returning the field's value: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.returnValueField", "Error returning the field's value: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Returns the decrypted value of the field by name.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Field decrypted value</returns>
        public object ReturnDecryptedValueField(string fieldName)
        {
            try
            {
                //support for non-fully-qualified names
                var ix = fieldName.IndexOf('.');
                var dbField = ix == -1
                    ? DBFields[fieldName]
                    : DBFields[fieldName.Substring(ix+1)];

                if(!Fields.TryGetValue(dbField.FullName, out var reqField))
                    return dbField.GetValorEmpty();
                else
                {
                    var encData = (EncryptedDataType)reqField.Value;
                    if(encData?.IsEmpty() ?? true)
                        return dbField.GetValorEmpty();

                    // TODO: Decrypt the value if necessary.

                    return encData.DecryptedValue;
                }

            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.ReturnDecryptedValueField", "Error returning the field's value: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.ReturnDecryptedValueField", "Error returning the field's value: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Função que permite remover um Qfield da área
        /// </summary>
        /// <param name="nomeCampo">name do Qfield a remover</param>
        /// <returns>True se foi removido false caso contrário</returns>
        public bool removeFieldValue(string fieldName)
        {
            //support for non-fully-qualified names
            var ix = fieldName.IndexOf('.');
            if (ix == -1)
                fieldName = DBFields[fieldName]?.FullName ?? fieldName;

            return Fields.Remove(fieldName);
        }

        /// <summary>
        /// Função que permite remover da área os fields que são doutras áreas
        /// </summary>
        public void removeFieldsOtherAreas()
        {
            foreach (var elem in Fields.Values.Where(x => !x.BelongsArea).ToList())
                Fields.Remove(elem.FullName);
        }

        /// <summary>
        /// Set decrypted value to encrypted field.
        /// TODO: In the future it will have to automatically include the type of encryption to use when writing the value to the database.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldValue">Decrypted value of the field</param>
        public virtual void InsertNameDecryptedValueField(string fieldName, object fieldValue)
        {
            try
            {
                //support for non-fully-qualified names
                if (!fieldName.Contains("."))
                {
                    var fieldInfo = DBFields[fieldName];
                    fieldName = fieldInfo.FullName;
                }

                RequestedField campoPedido;
                // Check if the field exists in the current table
                if (Fields.ContainsKey(fieldName))
                    campoPedido = Fields[fieldName];
                else
                    campoPedido = new RequestedField(fieldName, Alias);

                // Currently only supported for fields in the table itself
                if (campoPedido.BelongsArea)
                {
                    // The DB fields only take the field name, not the alias.
                    // Empty value will be ignored as the decrypted value is not always filled in.
                    // When read from the database, we will have only the encrypted value.
                    if (!DBFields[campoPedido.Name].isEmptyValue(fieldValue))
                    {
                        // If we have a new decrypted value, this will override the existing encrypted one to apply the update in if it happened
                        insertNameValueField(fieldName, new EncryptedDataType(null, fieldValue));
                    }
                }
                else
                {
                    throw new NotSupportedException($"Field {fieldName} does not belong to area {this.Alias}");
                }
            }
            catch (GenioException ex)
            {
                string message = $"Error inserting value in field {fieldName} in area {this.Alias}: {ex.Message}";
                throw new BusinessException(ex.UserMessage, "Area.InsertNameDecryptedValueField", message, ex);

            }
            catch (Exception ex)
            {
                string message = $"Error inserting value in field {fieldName} in area {this.Alias}: {ex.Message}";
                throw new BusinessException(message, "Area.InsertNameDecryptedValueField", message, ex);
            }
        }

        /// <summary>
        /// Function that allows removing Password-type fields from the area.
        /// </summary>
        /// <param name="removeOnlyEmpty">Remove only those taht have an empty value</param>
        public void RemovePasswordFields(bool removeOnlyEmpty = false)
        {
            foreach (var passwordField in this.PasswordFields ?? Enumerable.Empty<string>())
            {
                var fullFieldName = Alias + "." + passwordField;
                if (removeOnlyEmpty)
                {
                    var currentValue = this.returnValueField(fullFieldName);
                    var field = this.DBFields[passwordField];
                    var isEmpty = field.isEmptyValue(currentValue);

                    if(!isEmpty)
                        continue;
                }
                this.removeFieldValue(fullFieldName);
            }
        }

        /// <summary>
        /// Função que permite remover da área os fields que são calculados automaticamente
        /// Sempre que não confiamos nos Qvalues que estão a ser dados do exterior devemos
        ///  invocar este método depois de preencher os fields de uma área e antes de invocar
        ///  funções de recálculo.
        /// </summary>
        public void removeCalculatedFields()
        {
            string[] camposSR = this.RelatedSumFields;
            if (camposSR != null)
            {
                for (int i = 0; i < camposSR.Length; i++)
                {
                    this.removeFieldValue(Alias + "." + camposSR[i]);
                }
            }

            string[] camposU1 = this.LastValueFields;
            if (camposU1 != null)
            {
                for (int i = 0; i < camposU1.Length; i++)
                {
                    this.removeFieldValue(Alias + "." + camposU1[i]);
                }
            }

            // Fields with Concatenate rows formulas should not be overwritten by external inputs.
            // This type of formula is propagated from bottom to top.
            if (AggregateListFields != null)
            {
                foreach (var field in AggregateListFields)
                {
                    removeFieldValue($"{Alias}.{field}");
                }
            }

            // MH (23/06/2017) - Apagar os fields do carimbo da ficha.
            // O metodo do Carimbar a ficha está e deve estar só depois deste metodo.
            if (this.StampFieldsIns != null)
            {
                foreach (string campoCarimbo in this.StampFieldsIns)
                    this.removeFieldValue(Alias + "." + campoCarimbo);
            }
            if (this.StampFieldsAlt != null)
            {
                foreach (string campoCarimbo in this.StampFieldsAlt)
                    this.removeFieldValue(Alias + "." + campoCarimbo);
            }

            // MH (23/06/2017) - Apagar os fields com formulas + e +H
            if (this.InternalOperationFields != null)
            {
                foreach (string Qfield in this.InternalOperationFields)
                    this.removeFieldValue(Alias + "." + Qfield);
            }

            // MH (23/06/2017) - Apagar os fields com formula do tipo Replica
            if (this.ReplicaFields != null)
            {
                foreach (string Qfield in this.ReplicaFields)
                    this.removeFieldValue(Alias + "." + Qfield);
            }

            // MH (23/06/2017) - Apagar os fields com formula do tipo Fim Periodo
            if (this.EndofPeriodFields != null)
            {
                foreach (string Qfield in this.EndofPeriodFields)
                    this.removeFieldValue(Alias + "." + Qfield);
            }
        }

        /// <summary>
        /// Função que permite obter os Qvalues da lista de fields
        /// </summary>
        ///
        public void getFields(string[] fieldsList, PersistentSupport sp)
        {
            if (fieldsList != null && fieldsList.Length!=0)
            {
                string[] listaValores = new string[fieldsList.Length];
                string[] listaCamposCompletos = new string[fieldsList.Length];

                SelectField[] listaCamposQuery = new SelectField[fieldsList.Length];
                for (int i = 0; i < fieldsList.Length; i++)
                    listaCamposQuery[i] = new SelectField(new ColumnReference(Alias, fieldsList[i]), fieldsList[i]);

                ArrayList Qvalues = sp.returnFields(this, listaCamposQuery, PrimaryKeyName, this.QPrimaryKey);

                for (int i = 0; i < fieldsList.Length; i++)
                {
                    if (Qvalues != null)
                    {
                            listaValores[i] = Conversion.internal2String(Qvalues[i],this.DBFields[fieldsList[i]].FieldType);
                            listaCamposCompletos[i] = Alias + "." + fieldsList[i];
                    }
                }
                this.addNamesValuesFields(listaCamposCompletos, listaValores);
            }
        }

        /// <summary>
        /// Método to devolver a formatação de um Qfield da Base de dados
        /// </summary>
        /// <param name="nomeCampoBD">name do Qfield</param>
        /// <returns>a formatação do Qfield se existir na BD, null caso contrário</returns>
        public FieldFormatting returnFormattingDBField(string dbFieldName)
        {
            return DBFields[dbFieldName].FieldFormat;
            //RS(2008.06.09) Os enumerados não são nullable, o Qfield devia existir sempre senão é barracada da aplicação e deve
            // ser corrigida (pelo que tem de crashar to se perceber de onde vem o erro).
            //if (DBFields.ContainsKey(dbFieldName))
            //    return ((Field)DBFields[dbFieldName]).FieldFormat;
            //else
            //    return null;
        }

        /****************************CARIMBAR REGISTOS***************************************/

        /// <summary>
        /// Método to preencher os fields relativos aos dados da pessoa que inseriu o registo e à altura em que o fez
        /// </summary>
        public void fillStampInsert()
        {
            try
            {
                if (StampFieldsIns == null)
                    return;

                DateTime now = DateTime.Now;
                foreach(string stamp in StampFieldsIns)
                {
                    Field info = DBFields[stamp];
                    if(info.FieldType == FieldType.DATETIMESECONDS)
                        insertNameValueField(info.FullName, now);
                    else if (info.FieldType == FieldType.TEXT)
                        insertNameValueField(info.FullName, user.Name);
                    else if (info.FieldType == FieldType.TIME_HOURS)
                        insertNameValueField(info.FullName, string.Format("{0:00}:{1:00}", now.Hour, now.Minute));
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.preencherCarimboIns", "Error filling stamp fields for insertion: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.preencherCarimboIns", "Error filling stamp fields for insertion: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to preencher os fields relativos aos dados da pessoa que alterou o registo e à altura em que o fez
        /// </summary>
        public void fillStampChange()
        {
            try
            {
                if (StampFieldsAlt == null)
                    return;

                DateTime now = DateTime.Now;
                foreach (string stamp in StampFieldsAlt)
                {
                    Field info = DBFields[stamp];
                    if (info.FieldType == FieldType.DATETIMESECONDS)
                        insertNameValueField(info.FullName, now);
                    else if (info.FieldType == FieldType.TEXT)
                        insertNameValueField(info.FullName, user.Name);
                    else if (info.FieldType == FieldType.TIME_HOURS)
                        insertNameValueField(info.FullName, string.Format("{0:00}:{1:00}", now.Hour, now.Minute));
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.preencherCarimboAlt", "Error filling stamp fields for update: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.preencherCarimboAlt", "Error filling stamp fields for update: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// A partir de uma lista de historial filtra quais são os Qvalues aplicáveis a uma area.
        /// Tipicamente estes Qvalues vão ser usados to construir condições
        /// </summary>
        /// <param name="valoresEph">Um dicionario de condições a filtrar. A key é o name do eph, o Qvalue é um array de object.</param>
        /// <param name="identificador">Um identifier de override to a filtragem. Null caso não se queira considerar</param>
        /// <param name="unico">True caso o filtro só deva ser aplicado quando existir um e só um Qvalue to o eph</param>
        /// <returns>Uma lista de condicões em que cada uma é representada por um EPHOfArea</returns>
        /// <remarks>
        /// RS(2010.10.27) Refactorizei o codigo dos EPH em toda a frameword de forma a evitar duplicações
        /// Retirado de Listing (unico=false)
        /// *Bate certo com o da Area (unico=true, identifier=null)
        /// *Bate quase certo com o do Cart (unico=true, identifier=null) -> o unico é aplicado apenas a relations acima
        /// *Bate certo com o da Comunicacao (unico=false)
        /// *Bate certo com o do ReportCrystal (unico=false, identifier=null)
        /// </remarks>
        public List<EPHOfArea> CalculateAreaEphs(Hashtable ephValues, string identifier, bool unico)
        {
            List<EPHOfArea> res = new List<EPHOfArea>();

            //AV 20091229 Refiz condições to permitir EPH em árvore, com múltiplos Qvalues e
            //aplicadas a fields diferentes de chaves
            if (ephValues == null)
                return res;
            if (Ephs == null)
                return res;

            Hashtable condEph = new Hashtable(ephValues);
            // obtém as EPHs definidas to a área, module e nível de user

            //JGF 2020.09.09 Changed to get EPHs from a list of roles.
            var roleList = User.GetModuleRoles(module);
            List<EPHField> ephsArea = new List<EPHField>();
            foreach (var role in roleList)
            {
                var roleEphs = (EPHField[]) Ephs[new Par(Module, role.Id)];
                if(roleEphs != null)
                {
                    ephsArea.AddRange(roleEphs);
                }
            }

            foreach(var ephArea in ephsArea)
            {
                //considerar os identificadores nao sujeitos a EPH
                EPH eph = EPH.getEPH(module);//FFS (05-05-2011) a variável identifier vinha a null em alguns casos
                // RR 29-08-2012 - é preciso verificar também pela a EPH, faltava a última parte da condição
                if (eph.MenusNotSubjectEPH != null && identifier != null && eph.MenusNotSubjectEPH.ContainsKey(identifier) && eph.MenusNotSubjectEPH[identifier].Contains(ephArea.Name))
                    continue;

                //esta lista se exists eph nunca está vazia
                string[] listaValores = (string[])condEph[module + "_" + ephArea.Name];
                if (listaValores == null)
                    continue;

                //Se for unico entao a lista de Qvalues so pode ter um Qvalue
                if (unico && listaValores.Length != 1)
                {
                    condEph.Remove(module + "_" + ephArea);
                    continue;
                }

                EPHOfArea ephRes = new EPHOfArea(ephArea, listaValores);

                // Check if EPH is related to parent areas.
                // If found, these relations will be used to create query conditions.
                // Note: current area can be the same as the EPH area.
                // No need to add a relation if the current area is the EPH area itself.
                if (ParentTables != null)
                {
                    AreaInfo tabelaAtual = Area.GetInfoArea(Alias);

                    if (!String.IsNullOrEmpty(ephArea.Table) && Alias != ephArea.Table)
                    {
                        Relation myrelacao = null;
                        if (ParentTables.TryGetValue(ephArea.Table.ToLower(), out myrelacao))
                        {
                            ephRes.Relation = myrelacao;
                        }
                        else if (ephArea.Propagate)
                        {
                            List<Relation> relations = tabelaAtual.GetRelations(ephArea.Table);

                            if (relations != null && relations.Count > 0)
                                ephRes.Relation = relations.Last();
                        }
                    }

                    // Repeat the process if the EPH has a second condition (EPH2)
                    if (!String.IsNullOrEmpty(ephArea.Table2) && Alias != ephArea.Table2)
                    {
                        Relation myrelacao = null;

                        if (ParentTables.TryGetValue(ephArea.Table2.ToLower(), out myrelacao))
                        {
                            ephRes.Relation2 = myrelacao;
                        }
                        else if (ephArea.Propagate)
                        {
                            List<Relation> relations = tabelaAtual.GetRelations(ephArea.Table2);

                            if (relations != null && relations.Count > 0)
                                ephRes.Relation2 = relations.Last();
                        }
                    }
                }
                res.Add(ephRes);

                condEph.Remove(module + "_" + ephArea);
                if (condEph.Count == 0)
                    break;
            }

            return res;
        }

        /// <summary>
        /// Checks if the current user has access rights to query a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToConsult()
        {
            return AccessRightsToConsult(User);
        }

        /// <summary>
        /// Checks if a user has access rights to query a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToConsult(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanConsult(role));
        }


        /// <summary>
        /// Testa se o user tem direitos de Acesso to apagar
        /// </summary>
        /// <returns>true se o user pode apagar, false caso contrário</returns>
        public bool AccessRightsToDelete(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanDelete(role));
        }

        /// <summary>
        /// Função que verifica se um registo pode ser alterado
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns>true se pode change, false caso contrário</returns>
        public bool AccessRightsToChange(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanChange(role));
        }

        /// <summary>
        /// Checks if a user has access rights to create a record in this area.
        /// </summary>
        /// <returns></returns>
        public bool AccessRightsToCreate(User user)
        {
            return user.GetModuleRoles(module).Any(role => QLevel.CanCreate(role));
        }

        /// <summary>
        /// função que preenche as eph quando exists um único Qvalue
        /// </summary>
        /// <param name="User">user em sessão</param>
        public void fillEPH(User user, PersistentSupport sp, string identifier)
        {
            List<EPHOfArea> ephsDaArea = CalculateAreaEphs(user.Ephs, identifier, true);
            foreach (EPHOfArea v in ephsDaArea)
            {
                if (v.Relation == null && v.Relation2 == null) {
                    AuxAdicionaCondicaoMesmaArea(v.Eph, v.ValuesList);
                    continue;
                }

                // Add field from EPH first condition
                if (v.Relation != null)
                {
                    AuxAdicionaCondicaoOutraArea(sp, v.Eph, v.ValuesList, v.Relation);
                }
                // Add field from EPH second condition
                // Caveat: EPHs with multiple conditions will only add one field to the requested fields
                else if (v.Relation2 != null)
                {
                    // In order to reuse code, we create a second EPH field from data in area's EPH
                    EPHField EPH2 = new EPHField(v.Eph.Name, v.Eph.Table2, v.Eph.Field2, v.Eph.Operator2, v.Eph.Propagate);
                    AuxAdicionaCondicaoOutraArea(sp, EPH2, v.ValuesList, v.Relation2);
                }
            }
        }

        private RequestedField AuxAdicionaCondicaoOutraArea(PersistentSupport sp, EPHField ephArea, string[] listaValores, Relation myrelacao)
        {
            AreaInfo tabelaEPH = Area.GetInfoArea(ephArea.Table);

            var crorigem = ephArea.Propagate
                ? tabelaEPH.DBFields[myrelacao.TargetRelField]
                : DBFields[myrelacao.SourceRelField];

            Field QPrimaryKeyField = tabelaEPH.DBFields[tabelaEPH.PrimaryKeyName];
            RequestedField campoPedido = new RequestedField(crorigem.FullName, Alias);
            campoPedido.FieldType = QPrimaryKeyField.FieldType;

            object Qvalue = null;

            // If the EPH is set on a primary-key column, we can directly use the supplied value
            // Otherwise, we retrieve the primary-key value of the record that validates the EPH condition
            if (ephArea.Field.ToLower().Equals(myrelacao.TargetRelField))
            {
                Qvalue = listaValores[0];
            }
            else
            {
                CriteriaSet where = CriteriaSet.And();

                switch (ephArea.Operator)
                {
                    case "=" :
                        where.Equal(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "<" :
                        where.Lesser(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case ">" :
                        where.Greater(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "<=" :
                        where.LesserOrEqual(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case ">=" :
                        where.GreaterOrEqual(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "!=" :
                        where.NotEqual(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        break;
                    case "L" :
                        where.Like(tabelaEPH.TableName, ephArea.Field, listaValores[0] + "%"); // TODO: Use LEFT. BackOffice: (LEFT(%s,%d)=
                        break;
                    case "LN" :
                        // MH - Eph em árvore ou NULL
                        CriteriaSet auxWhere = CriteriaSet.Or();
                        Field campoLN = tabelaEPH.DBFields[ephArea.Field];
                        if (campoLN.isKey())
                        {
                            auxWhere.Equal(new ColumnReference(tabelaEPH.TableName, ephArea.Field), null);
                        }
                        else
                        {
                            string funcaoSQL = campoLN.FieldType.GetEPHFunction();
                            auxWhere.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(tabelaEPH.TableName, ephArea.Field)), 1);
                        }
                        auxWhere.Like(tabelaEPH.TableName, ephArea.Field, listaValores[0] + "%"); // TODO: Use LEFT. BackOffice: (LEFT(%s,%d)=
                        where.SubSet(auxWhere);
                        break;
                    case "NULL" :
                        where.Equal(tabelaEPH.TableName, ephArea.Field, null);
                        break;
                    case "EN":
						Field Qfield = tabelaEPH.DBFields[ephArea.Field];
                        CriteriaSet lim = new CriteriaSet(CriteriaSetOperator.Or);
                        if(Qfield.isKey())
                        {
                            lim.Equal(new ColumnReference(tabelaEPH.TableName, ephArea.Field), null);
                        }
                        else
                        {
                            string funcaoSQL = Qfield.FieldType.GetEPHFunction();
                            lim.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(tabelaEPH.TableName, ephArea.Field)), 1);
                        }
                        lim.Equal(tabelaEPH.TableName, ephArea.Field, listaValores[0]);
                        where.SubSet(lim);
                        break;
                    default:
                        throw new BusinessException(null, "Area.AuxAdicionaCondicaoOutraArea", "The eph operator '" + ephArea.Operator + "' is not known.");
                }

                SelectQuery query = new SelectQuery()
                    .Select(tabelaEPH.TableName, tabelaEPH.PrimaryKeyName)
                    .From(tabelaEPH.QSystem, tabelaEPH.TableName, tabelaEPH.TableName)
                    .Where(where);

                var result = sp.Execute(query);
                if (result.NumRows == 1)
                    Qvalue = DBConversion.ToInternal(result.GetDirect(0, 0), QPrimaryKeyField.FieldFormat);
            }

            if (QPrimaryKeyField.isEmptyValue(Qvalue))
                Qvalue = QPrimaryKeyField.GetValorEmpty();

            campoPedido.Value = Qvalue;

            // Do not add a repeated field to the query
            // The request already includes the requested field needed for the EPH
            if (ephArea.Propagate && Fields.ContainsKey(Alias + "." + crorigem.Name))
                return campoPedido;

            Fields[crorigem.FullName] = campoPedido;
            return campoPedido;
        }


        private RequestedField AuxAdicionaCondicaoMesmaArea(EPHField ephArea, string[] listaValores)
        {
            Field Qfield = DBFields[ephArea.Field];
            RequestedField campoPedido = new RequestedField(Qfield.FullName, Alias);
            campoPedido.FieldType = Qfield.FieldType;
            campoPedido.Value = listaValores[0];

            Fields[Qfield.FullName] = campoPedido;
            return campoPedido;
        }

        public int Zzstate
        {
            get { return (int)returnValueField("zzstate"); }
            set { insertNameValueField("zzstate", value); }
        }

        public string QPrimaryKey
        {
            get { return (string)returnValueField(PrimaryKeyName); }
            set { insertNameValueField(PrimaryKeyName, value); }
        }

        /// <summary>
        /// Selecciona o Qresult da query quando é único
        /// </summary>
        /// <param name="condicao">Condição WHERE</param>
        /// <param name="identificador">Identificado do controlo</param>
        /// <param name="sp">Suporte persistente</param>
        /// <returns></returns>
        public void selectSingle(CriteriaSet condition, string identifier, PersistentSupport sp)
        {
            try
            {
                sp.selectSingle(condition, this, identifier);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUnico", "Error selecting a single query result: " + ex.Message, ex);
            }
        }


        public void selectOne(CriteriaSet condition, IList<ColumnSort> sorting, string identifier, PersistentSupport sp, int pageSize = 1)
        {
            try
            {
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                MethodInfo funcOver = funcObj.GetMethod(identifier);
                if (funcOver != null)
                {
                    //void (CriteriaSet condition, string tokenAux, PersistentSupport sp, IList<ColumnSort> sorting)

                    object[] parameters = new object[5];
                    parameters[0] = condition;
                    parameters[1] = user;
                    parameters[2] = sp;
                    parameters[3] = sorting;
                    parameters[4] = this;

                    funcOver.Invoke(this, parameters); //TODO : os paramentros adicionais e tratamentos dos mesmo
                }
                else
                {
                    if (Information.PersistenceType == PersistenceType.Database || Information.PersistenceType == PersistenceType.View)
                    {
                        sp.selectOne(condition, sorting, this, identifier, pageSize);
                    }
                    else
                    {
                        string[] fieldsRequested = new string[Fields.Keys.Count];
                        Fields.Keys.CopyTo(fieldsRequested, 0);

                        Type areaType = this.GetType();
                        MethodInfo listMethod = areaType.GetMethod("search",
                            BindingFlags.Static | BindingFlags.Public,
                            null,
                            new Type[] { typeof(PersistentSupport), typeof(string), typeof(User), typeof(string[]) },
                            null
                        );
                        IArea res = listMethod.Invoke(null, new object[] {
                            sp,
                            condition.FindCriteria(this.Alias, this.PrimaryKeyName, CriteriaOperator.Equal, CriteriaSet.FindVariable.Any).RightTerm as string,
                            user,
                            fieldsRequested
                        }) as IArea;
                        CloneFrom(res);
                    }
                }
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new BusinessException(null, "Area.seleccionarUm", "Error selecting first query result: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="sp">The sp.</param>
        /// <param name="user">The user.</param>
        /// <param name="where">The where.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <param name="noLock">if set to <c>true</c> [no lock].</param>
        /// <returns></returns>
        public static List<Area> searchList(string area, PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
            // Find the generic method in Persistent Support (searchListWhere<T>)
            var mInfo = typeof(PersistentSupport).GetMethod("genericSearchListWhere",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                CallingConventions.Any,
                new Type[] { typeof(CriteriaSet), typeof(User), typeof(string[]), typeof(bool), typeof(bool) },
                null);

            // Apply concrete type to method type parameter (searchListWhere<CSGenioA_____>)
            Type type = GetTypeArea(area);
            MethodInfo generic = mInfo.MakeGenericMethod(type);

            // Invoke
            object[] args = { where, user, fields, distinct, noLock };
            return ((List<Area>)generic.Invoke(sp, args));
        }

        /// <summary>
        /// Delete a record from the database. Requires an opened connection
        /// </summary>
        /// <param name="sp">PersistentSupport</param>
        /// <returns></returns>
        public virtual StatusMessage eliminate(PersistentSupport sp)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.eliminar [area] {0}", Alias));
                object valorCodigoObj = QPrimaryKey;
                if (valorCodigoObj == null)
                    throw new BusinessException(null, "Area.eliminar", "ChavePrimaria is null.");

                sp.deleteRecord(this, valorCodigoObj.ToString());
                return StatusMessage.OK("Eliminação bem sucedida.");
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "Area.eliminar")
                    throw;
                throw new BusinessException(ex.UserMessage, "Area.eliminar", "Error deleting record from Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.eliminar", "Error deleting record from Area: " + ex.Message, ex);
            }

        }


        /// <summary>
        /// Eliminate a record due to the deletion of another record. The root record must be known for various checks
        /// </summary>
        /// <param name="rootRecord">Root record which originated this deletion</param>
        /// <returns></returns>
        public virtual StatusMessage eliminateDependent(PersistentSupport sp, Area rootRecord)
        {
            return eliminate(sp);
        }

        public virtual StatusMessage change(PersistentSupport sp, CriteriaSet condition)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.alterar [area] {0}", Alias));
                sp.change(this);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.alterar", "Error changing record from Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.alterar", "Error changing record from Area: " + ex.Message, ex);
            }

            return StatusMessage.OK("Alteração bem sucedida.");
        }

        public virtual void apply(PersistentSupport sp)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.apply [area] {0}", Alias));
                sp.change(this);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.apply", "Error changing record from Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.apply", "Error changing record from Area: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to introduce um registo que fica imediatamente disponivel (zztate=0)
        /// </summary>
        /// <param name="modulo">módulo</param>
        /// <returns>o status e a mensagem resposta da inserção</returns>
        public virtual StatusMessage inserir_WS(PersistentSupport sp)
        {
            try
            {
                insertPseud(sp);

                //Aqui queremos sempre garantir que o zzstate passa a 0
                Zzstate = 0;

                return change(sp, (CriteriaSet)null);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.inserir_WS", "Error inserting record in Area: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método que permite introduce um registo na base de dados,
        /// pressupoe uma ligação aberta
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns></returns>
        public virtual Area insertPseud(PersistentSupport sp)
        {
            return insertPseud(sp,  new string[] { }, new string[] { });
        }

        /// <summary>
        /// Método que permite introduce um registo na base de dados como uma ficha pseudo-nova (zzstate = 1)
        /// </summary>
        /// <param name="sp">Suporte Persistente</param>
        /// <param name="condicao">Condição de seleção dos registos</param>
        /// <returns></returns>
        public virtual Area insertPseud(PersistentSupport sp, string[] fieldNames, string[] fieldsvalues)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Area.inserir [area] {0}", Alias));

                sp.insertPseud(this);
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Area.inserir", "Error inserting record in Area: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "Area.inserir", "Error inserting record in Area: " + ex.Message, ex);
            }

            return this;
        }

        public virtual Area duplicate(PersistentSupport sp, CriteriaSet condition)
        {
            throw new BusinessException(null, "Area.duplicar", "Function not implemented.");
        }

        public virtual StatusMessage beforeUpdate(PersistentSupport sp, Area oldvalues)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterUpdate(PersistentSupport sp, Area oldvalues)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage beforeInsert(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterInsert(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage beforeEliminate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterEliminate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage beforeDuplicate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        public virtual StatusMessage afterDuplicate(PersistentSupport sp)
        {
            return StatusMessage.OK();
        }

        /// <summary>
        /// Gets the primary key and all the requested fields of a given related area.
        /// Returns an area filled with these informations.
        /// </summary>
        /// <param name="relatedAreaName">The related area</param>
        /// <param name="requestedFields">The requested fields</param>
        /// <returns>Area field with the requested fields and primary key</returns>
        public virtual Area fillRelatedArea(string relatedAreaName, string[] requestedFields) {
            Area relatedArea = Area.createArea(relatedAreaName, user, Module);
            // adds the restriction, that will limit the upper areas
            CriteriaSet cs = CriteriaSet.And();
            cs.Equal(new FieldRef(Alias,PrimaryKeyName), QPrimaryKey);

            List<string> fieldsList = new List<string>(requestedFields);
            string pkFullFieldName = relatedArea.Alias + "." + relatedArea.PrimaryKeyName;
            if(!fieldsList.Contains(pkFullFieldName))
                fieldsList.Add(pkFullFieldName);

            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year);
            sp.fillInfoForForeignKey(relatedArea, this, cs, fieldsList);

            return relatedArea;
        }

        /// <summary>
        /// Fields that have been requested to be read or written
        /// </summary>
        public Dictionary<string, RequestedField> Fields { get; set; } = new Dictionary<string, RequestedField>();

        public abstract AreaInfo Information
        {
            get;
        }

        public string QSystem
        {
            get { return Information.QSystem; }
        }

        public string Alias
        {
            get { return Information.Alias; }
        }

        public Dictionary<string, Relation> ParentTables
        {
            get { return Information.ParentTables; }
        }

        public Relation[] DuplicationRelations
        {
            get { return Information.DuplicationRelations; }
        }

        public string PrimaryKeyName
        {
            get { return Information.PrimaryKeyName; }
        }

        public string ShadowTabKeyName
        {
            get { return Information.ShadowTabKeyName; }
        }

        public string TableName
        {
            get { return Information.TableName; }
        }

        public string ShadowTabName
        {
            get { return Information.ShadowTabName; }
        }

        public Dictionary<string, Field> DBFields
        {
            get { return Information.DBFields; }
        }

        public ChildRelation[] ChildTable
        {
            get { return Information.ChildTable; }
        }

        public Hashtable Ephs
        {
            get { return Information.Ephs; }
        }

        public ArrayList ShadowTabLevels
        {
            get { return Information.ShadowTabLevels; }
        }

        public string[] DefaultValues
        {
            get { return Information.DefaultValues; }
        }

        public string[] SequentialDefaultValues
        {
            get { return Information.SequentialDefaultValues; }
        }

        public string[] ReplicaFields
        {
            get { return Information.ReplicaFields; }
        }

        public string[] CheckTableFields
        {
            get { return Information.CheckTableFields; }
        }

        //SO 20060616
        public string[] EndofPeriodFields
        {
            get { return Information.EndofPeriodFields; }
        }

        public string[] InternalOperationFields
        {
            get { return Information.InternalOperationFields; }
        }

        public string[] InternalOperationSequentialFields
        {
            get { return Information.InternalOperationSequentialFields; }
        }

        public string[] RelatedSumFields
        {
            get { return Information.RelatedSumFields; }
        }

        public List<RelatedSumArgument> RelatedSumArgs
        {
            get { return Information.RelatedSumArgs; }
        }

        //AJAGENIO
        public string[] AggregateListFields
        {
            get { return Information.AggregateListFields; }
        }
        public List<ListAggregateArgument> ArgsListAggregate
        {
            get { return Information.ArgsListAggregate; }
        }

        public List<History> HistoryList
        {
            get { return Information.HistoryList; }
        }
        //AV 20090206
        public string[] LastValueFields
        {
            get { return Information.LastValueFields; }
        }

        public string[] StampFieldsIns
        {
            get { return Information.StampFieldsIns; }
        }

        public string[] StampFieldsAlt
        {
            get { return Information.StampFieldsAlt; }
        }

        //SO 20060810
        public List<LastValueArgument> LastValueArgs
        {
            get { return Information.LastValueArgs; }
        }

        //SO 20060818 propriedade to retirar no QLevel
        public QLevel QLevel
        {
            get { return Information.QLevel; }
        }

        public string AreaPluralDesignation
        {
            get { return Information.AreaPluralDesignation; }
        }

        public string AreaDesignation
        {
            get { return Information.AreaDesignation; }
        }

        public User User
        {
            get { return user; }
        }
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// Write conditions for this area
        /// </summary>
        private List<ConditionFormula> WriteConditions
        {
            get => Information.WriteConditions;
        }

        /// <summary>
        /// CRUD conditions for this area
        /// </summary>
        private List<ConditionFormula> CrudConditions
        {
            get => Information.CrudConditions;
        }

        public string[] PasswordFields
        {
            get => Information.PasswordFields;
        }

        /// <summary>
        /// True when the last information read was locked so that we can assume the old values wont change,
        /// false otherwise.
        /// </summary>
        public bool IsBookmarkLocked { get; set; } = false;

        /// <summary>
        /// Validate all area level conditions
        /// </summary>
        /// <param name="sp">The persistent support to get values from</param>
        /// <param name="isApply">If this is an apply operation. Some conditions can execute in apply and other not</param>
        /// <returns>A status message with the aggregated result of all conditions evaluation</returns>
        public StatusMessage ValidateConditions(PersistentSupport sp, bool isApply)
        {
            StatusMessage result = StatusMessage.OK();
            //Area conditions
            foreach (ConditionFormula condition in WriteConditions)
            {
                if (isApply && !condition.Validate)
                    continue;

                bool conditionResult = condition.ExecuteCondition(this, sp, FunctionType.ALT);
                StatusMessage status = StatusMessage.OK();
                if (!conditionResult)
                {
                    if (condition.Type == ConditionType.ERROR)
                    {
                        status = StatusMessage.Error(Translations.Get(condition.ErrorWarning, user.Language));
                    }
                    else if (condition.Type == ConditionType.WARNING)
                    {
                        status = StatusMessage.Warning(Translations.Get(condition.ErrorWarning, user.Language));
                    }
                    else if (condition.Type == ConditionType.SAVE) {
                        status = StatusMessage.OK(Translations.Get(condition.ErrorWarning, user.Language));
                        result.MergeStatusMessage(status); //If this is the right condition result for SAVE, merge the message
                    }
                }
                else if (condition.Type == ConditionType.MANDATORY)
                {
                    var value = returnValueField(condition.Field.FullName);
                    if (condition.Field.isEmptyValue(value))
                    {
                        status = StatusMessage.Error(Translations.Get(condition.ErrorWarning, user.Language));
                    }
                }

                if(status.Status != Status.OK)
                    result.MergeStatusMessage(status);
            }
            return result;
        }

        /// <summary>
        /// Executes the encryption formulas associated with the fields before saving the value to the database.
        /// </summary>
        /// <param name="sp">Persistent Support instance</param>
        public void ExecuteFieldValueEncryption(PersistentSupport sp)
        {
            FormulaDbContext fdc = new FormulaDbContext(this);

            foreach (RequestedField requestedField in Fields.Values)
            {
                if (requestedField.Name.Equals(PrimaryKeyName) || !DBFields.TryGetValue(requestedField.Name, out Field dbField))
                    continue;
                if (dbField.IsVirtual)
                    continue;

                // Encrypt the fields before save in the database
                if (dbField.FieldType == FieldType.ENCRYPTED && dbField.EncryptFieldValueFormula != null)
                {
                    // The encrypted field, if it does not have the value, will not change what is in the database.
                    if (!dbField.isEmptyValue(requestedField.Value))
                    {
                        if (requestedField.Value is EncryptedDataType encryptedData)
                        {
                            var encryptedValueStr = encryptedData.EncryptedValue as string;
                            //If we only have the decrypted value, we'll try use the encryption associated with the field.
                            if (string.IsNullOrWhiteSpace(encryptedValueStr))
                                encryptedData.EncryptedValue = dbField.EncryptFieldValueFormula.calculateInternalFormula(this, sp, fdc, FunctionType.EXW);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Adds a message to the queue corresponding to the current row
        /// Uses the old Quidserver based mechanism
        /// </summary>
        /// <param name="sp">Persistent support</param>
        /// <param name="operation">Type of crud operation, C - Create, U - Update, D - Delete</param>
        /// <param name="oldValues">Previous values of this row, send null for insertions</param>
        /// <param name="queueId">Only send a specific queue, or null to send all queues associated with the row</param>
        public abstract void insertQueue(PersistentSupport sp, string operation, Area oldValues, string queueId);

        /// <summary>
        /// Adds a message to the queue corresponding to the current row
        /// Uses the new RabbitMq based mechanism
        /// </summary>
        /// <param name="sp">Persistent support</param>
        /// <param name="operation">Type of crud operation, C - Create, U - Update, D - Delete</param>
        /// <param name="oldValues">Previous values of this row, send null for insertions</param>
        /// <param name="queueId">Only send a specific queue, or null to send all queues associated with the row</param>
        public abstract void MessageQueue(PersistentSupport sp, string operation, Area oldValues);

    }
}
