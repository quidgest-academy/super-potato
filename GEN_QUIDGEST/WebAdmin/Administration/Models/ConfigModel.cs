using Administration.AuxClass;
using CSGenio;
using CSGenio.framework;
using GenioServer.security;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CSGenio.config;

namespace Administration.Models
{
    public class ConfigModel : ModelBase
    {
		public ConfigModel()
        {
            AdvancedProperties = new List<MorePropertyCfg>();
        }

        [Display(Name = "NOME_DO_SERVIDOR_DE_38232", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string Server { get; set; }

        [Display(Name = "NOME_DO_SERVIDOR_DE_38232", ResourceType = typeof(Resources.Resources))]
        public string Log_Server { get; set; }

        [Display(Name = "PORTA55707", ResourceType = typeof(Resources.Resources))]
        public string Port { get; set; }

        [Display(Name = "PORTA55707", ResourceType = typeof(Resources.Resources))]
        public string Log_Port { get; set; }

        [Display(Name = "IDENTIFICADOR_DO_SER22713", ResourceType = typeof(Resources.Resources))]
        public string Service { get; set; }

        [Display(Name = "IDENTIFICADOR_DO_SER22713", ResourceType = typeof(Resources.Resources))]
        public string Log_Service { get; set; }
		
		[Display(Name = "NOME_DO_SERVICO32188", ResourceType = typeof(Resources.Resources))]
        public string ServiceName { get; set; }

        [Display(Name = "NOME_DO_SERVICO32188", ResourceType = typeof(Resources.Resources))]
        public string Log_ServiceName { get; set; }

        [Display(Name = "TIPO_DE_SERVIDOR_DE_25581", ResourceType = typeof(Resources.Resources))]
        public HardCodedLists.DBMS ServerType { get; set; }

        [Display(Name = "TIPO_DE_SERVIDOR_DE_25581", ResourceType = typeof(Resources.Resources))]
        public HardCodedLists.DBMS Log_ServerType { get; set; }

        public bool DatabaseSidePk { get; set; }

        [Display(Name = "DB Schema")]
        [Required]
        public string Schema { get; set; }

        [Display(Name = "DB Schema")]
        public string Log_Schema { get; set; }

		[Display(Name = "ENCRIPTAR_LIGACAO12834", ResourceType = typeof(Resources.Resources))]
        public bool ConnEncrypt { get; set; }

        [Display(Name = "ENCRIPTAR_LIGACAO12834", ResourceType = typeof(Resources.Resources))]
        public bool Log_ConnEncrypt { get; set; }

		[Display(Name = "UTILIZADOR_DE_DOMINI41043", ResourceType = typeof(Resources.Resources))]
        public bool ConnWithDomainUser { get; set; }

        [Display(Name = "UTILIZADOR_DE_DOMINI41043", ResourceType = typeof(Resources.Resources))]
        public bool Log_ConnWithDomainUser { get; set; }



        [Display(Name = "O_ANO_DEFAULT_NAO_ES52509", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DefaultYear { get; set; }

        [Display(Name = "OCULTAR_ANOS03755", ResourceType = typeof(Resources.Resources))]
        public bool HideYears { get; set; }

        [Display(Name = "NOME_DO_ADMINISTRADO41779", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbUser { get; set; }

        [Display(Name = "NOME_DO_ADMINISTRADO41779", ResourceType = typeof(Resources.Resources))]
        public string Log_DbUser { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbPsw { get; set; }
        public bool HasDbPsw { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        public string Log_DbPsw { get; set; }
        public bool Log_HasDbPsw { get; set; }

        [Display(Name = "CONFIRMAR_NOVA_PALAV02846", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbCheckPsw { get; set; }

        [Display(Name = "CONFIRMAR_NOVA_PALAV02846", ResourceType = typeof(Resources.Resources))]
        public string Log_DbCheckPsw { get; set; }

        [Display(Name = "ESTADO_DA_OPERACAO38065", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        public string AlertType { get; set; }

        [Display(Name = "MESSAGE_QUEUEING34227", ResourceType = typeof(Resources.Resources))]
        public MessageQueue MQueues { get; set; }

        [Display(Name = "CAMINHO_PARA_RELATOR05547", ResourceType = typeof(Resources.Resources))]
        public string pathReports { get; set; }

        [Display(Name = "URL05719", ResourceType = typeof(Resources.Resources))]
        public string ssrsServer { get; set; }

		[Display(Name = "CAMINHO18436", ResourceType = typeof(Resources.Resources))]
        public string ssrsServerPath { get; set; }

        [Display(Name = "SAO_OS_RELATORIOS_LO04230", ResourceType = typeof(Resources.Resources))]
        public bool isLocalReports { get; set; }

        [Display(Name = "DOMINIO33043", ResourceType = typeof(Resources.Resources))]
        public string ssrsServerDomain { get; set; }

        [Display(Name = "NOME_DE_UTILIZADOR58858", ResourceType = typeof(Resources.Resources))]
        public string ssrsServerUsername { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        public string ssrsServerPassword { get; set; }
        
        public bool hasSsrsServerPassword { get; set; }

        [Display(Name = "FORMATO_DAS_DATAS11781", ResourceType = typeof(Resources.Resources))]
        public DateFormatCfg DateFormat { get; set; }

        [Display(Name = "SEPARADOR_DECIMAL14173", ResourceType = typeof(Resources.Resources))]
        public HardCodedLists.DisplayNumberFormatDecimal DecimalSeparator { get; set; }

		[Display(Name = "SEPARADOR_DE_GRUPO26735", ResourceType = typeof(Resources.Resources))]
        public HardCodedLists.DisplayNumberFormatGroup GroupSeparator { get; set; }

		[Display(Name = "FORMATO_DE_NUMERO_NE41581", ResourceType = typeof(Resources.Resources))]
        public HardCodedLists.DisplayNumberFormatNegative NegativeFormat { get; set; }

        [Display(Name = "MOTOR_DE_PESQUISA__E50766", ResourceType = typeof(Resources.Resources))]
        public List<CoreCfg> Cores { get; set; }

		[Display(Name = "AUDITORIA_DE_ACOES_D42106", ResourceType = typeof(Resources.Resources))]
        public bool RegistActions { get; set; }

        [Display(Name = "AUDITORIA_DE_LOGIN_D00905", ResourceType = typeof(Resources.Resources))]
        public bool RegistLoginOut { get; set; }

        [Display(Name = "AUDITORIA_DO_SISTEMA08460", ResourceType = typeof(Resources.Resources))]
        public bool AuditInterface { get; set; }

		[Display(Name = "FORNECEDORES_DE_IDEN35608", ResourceType = typeof(Resources.Resources))]
        public List<MorePropertyCfg> AdvancedProperties { get; set; }

        [BindNever]
        public List<ClientApplication> Applications { get; set; }

        public Dictionary<String, SecurityCfg> Security { get; set; }

        public Dictionary<string, PathCfg> Paths { get; set; }

        public string UrlAPIBackend { get; set; }

        public string UrlMCP { get; set; }

        public MCPSecurityMode MCPSecurityMode { get; set; }

        public string JWTEncryptionKey { get; set; }

		[Display(Name = "AMBIENTE_DE_QA_09940", ResourceType = typeof(Resources.Resources))]
        public bool QAEnvironment { get; set; }

        /// <summary>
        /// Event tracing feature
        /// </summary>
        [Display(Name = "REGISTO_DE_EVENTOS65341", ResourceType = typeof(Resources.Resources))]
        public bool EventTracking { get; set; }

        public MessagingXml Messaging { get; set; }

        public SchedulerXml Scheduler { get; set; }

        public CSGenio.core.messaging.MessageMetadata MessagingMetadata {get; set;}

        [BindNever]
        public object SelectLists
        {
            get
            {
                return new
                {
                    DBMS = AuxFunctions.ToSelectList<HardCodedLists.DBMS>(),
                    AuthenticationMode = AuxFunctions.ToSelectList<DisplayAuthenticationMode>(),
                    MultisessionMode = AuxFunctions.ToSelectList<DisplayMultisessionMode>(),
                    PasswordStrength = AuxFunctions.ToSelectList<DisplayPasswordStrength>(),
                    PasswordAlgorithms = AuxFunctions.ToSelectList<DisplayPasswordAlgorithms>(),
                    DecimalSeparator = AuxFunctions.ToSelectList<HardCodedLists.DisplayNumberFormatDecimal>(),
                    GroupSeparator = AuxFunctions.ToSelectList<HardCodedLists.DisplayNumberFormatGroup>(),
                    NegativeFormat = AuxFunctions.ToSelectList<HardCodedLists.DisplayNumberFormatNegative>(),
                    DisplayUserType = AuxFunctions.ToSelectList<DisplayUserType>(),
                    IdentityProviderTypeList = IdentityProviderCfg.TypeList(),
                    RoleProviderTypeList = RoleProviderCfg.TypeList(),
                    PropertyList = MorePropertyCfg.PropertyList,
                    SchedulerTaskList = ScheduleTaskFactory.GetTaskOptions(),
                };
            }
        }

        public DataSystemXml GetDataSystemXml()
        {
            return new DataSystemXml
            {
                Server = this.Server,
                Password = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(this.DbPsw)),
                Login = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(this.DbUser)),
                Name = this.Schema,
                Port = this.Port,
                Type = this.ServerType.ToString(),
                Schemas = new List<DataXml>
                {
                    new DataXml { Schema = this.Schema }
                }
            };
        }
    }

	public enum DisplayPasswordStrength
    {
        [Display(Name = "POBRE46544", ResourceType = typeof(Resources.Resources))]
        Pobre,
        [Display(Name = "FRACO22195", ResourceType = typeof(Resources.Resources))]
        Fraco,
        [Display(Name = "BOM29058", ResourceType = typeof(Resources.Resources))]
        Bom,
        [Display(Name = "FORTE13835", ResourceType = typeof(Resources.Resources))]
        Forte
    }

    public enum DisplayPasswordAlgorithms
    {
        [Display(Name = "Quidgest")]
        QUI,
        [Display(Name = "Argon2")]
        ARG
    }

    public enum DisplayAuthenticationMode
    {        
        [Display(Name = "ACEITAR_AO_PRIMEIRO_40557", ResourceType = typeof(Resources.Resources))]
        AcceptOnFirstSucess,
        [Display(Name = "UM_BOTAO_POR_PROVIDE48545", ResourceType = typeof(Resources.Resources))]
        OneButtonPerProvider
    }

    public enum DisplayMultisessionMode
    {
        [Display(Name = "PERMISSIVA04157", ResourceType = typeof(Resources.Resources))]
        Loose,
        [Display(Name = "POR_IP48549", ResourceType = typeof(Resources.Resources))]
        PerIp,
        [Display(Name = "SESSAO_UNICA28803", ResourceType = typeof(Resources.Resources))]
        Strict
    }

    public static class TypeLoaderExtensions
    {
        //Solves the problem of some assemblies that may not have loaded yet
        //http://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface/
        public static IEnumerable<Type> GetLoadableTypes(this AppDomain domain)
        {
            List<Type> res = new List<Type>();

            foreach (var assembly in domain.GetAssemblies())
                if(!assembly.FullName.StartsWith("System"))
	            {
	                try
	                {
	                    res.AddRange(assembly.GetTypes());
	                }
	                catch (System.Reflection.ReflectionTypeLoadException e)
	                {
	                    res.AddRange(e.Types.Where(t => t != null));
	                }
	            }

            return res;
        }
    }


    public class SecurityOptionMetainfo
    {
        public string Type { get; set; }
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Optional { get; set; }
        public string Parent { get; set; }
    }

    public class SecurityProviderMetainfo
    {
        [JsonIgnore]
        public Type Type { get; set; }
        public string TypeFullName => Type.Assembly.GetName().Name == "CSGenio.core"
            ? Type.FullName
            : Type.FullName + ", " + Type.Assembly.GetName().Name;
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<SecurityOptionMetainfo> Options { get; set; } = new List<SecurityOptionMetainfo>();

        public SecurityProviderMetainfo(Type providerType)
        {
            Type = providerType;

            //read the class attributes
            var a = providerType.GetCustomAttributes(true);
            Description = (a.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute)?.Description ?? "";
            DisplayName = (a.FirstOrDefault(x => x is DisplayNameAttribute) as DisplayNameAttribute)?.DisplayName ?? providerType.Name;

            //Read the options
            CollectOptions(providerType, "");

            //Recursive function to collect all the security options of a type
            void CollectOptions(Type pt, string parent)
            {
                foreach (var prop in pt.GetProperties())
                {
                    SecurityOptionMetainfo mi = new SecurityOptionMetainfo();
                    mi.Type = prop.PropertyType.Name;
                    mi.PropertyName = prop.Name;
                    mi.DisplayName = prop.Name;
                    mi.Description = "";
                    mi.Parent = parent;
                    bool isOption = false;
                    bool isJson = false;
                    foreach (var pa in prop.GetCustomAttributes(true))
                    {
                        if (pa is SecurityProviderOptionAttribute spoa)
                        {
                            isOption = true;
                            isJson = spoa.IsJson;
                            mi.Optional = spoa.Optional;
                        }
                        if (pa is DisplayNameAttribute dna)
                            mi.DisplayName = dna.DisplayName;
                        if (pa is DescriptionAttribute da)
                            mi.Description = da.Description;
                    }
                    if (isJson)
                        CollectOptions(prop.PropertyType, prop.Name);
                    else if (isOption)
                        Options.Add(mi);
                }
            }

        }
    }

    public class IdentityProviderCfg
    {
        private static IEnumerable<SecurityProviderMetainfo> idProviderList = AppDomain.CurrentDomain.GetLoadableTypes()
            .Where(p => p.GetInterfaces().Contains(typeof(GenioServer.security.IIdentityProvider)) && !p.IsAbstract)
            .Select(x => new SecurityProviderMetainfo(x));
        public static IEnumerable<SecurityProviderMetainfo> TypeList() => idProviderList;

        public IdentityProviderCfg()
        {
            obj = new CSGenio.IdentityProviderCfgEl();
        }

        public IdentityProviderCfg(CSGenio.IdentityProviderCfgEl o)
        {
            obj = o;
        }

        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string Name
        {
            get { return obj.Name; }
            set { obj.Name = value; }
        }

        [Display(Name = "DESCRICAO07528", ResourceType = typeof(Resources.Resources))]
        public string Description
        {
            get { return obj.Description; }
            set { obj.Description = value; }
        }

        [Display(Name = "TIPO55111", ResourceType = typeof(Resources.Resources))]
        public string Type
        {
            get { return obj.Type; }
            set { obj.Type = value; }
        }

        [Display(Name = "2FA")]
        public bool Is2FA
        {
            get { return obj.Is2FA; }
            set { obj.Is2FA = value; }
        }

        [Display(Name = "CONFIGURACAO10928", ResourceType = typeof(Resources.Resources))]
        public CSGenio.SerializableDictionary<string, string> Options
        {
            get { return obj.Options; }
            set { obj.Options = value; }
        }

        [JsonIgnore]
        public IdentityProviderCfgEl obj { get; set; }

        public string FormMode { get; set; }
        public int Rownum { get; set; }

    }

    public class RoleProviderCfg
    {
        private static IEnumerable<SecurityProviderMetainfo> roleProviderList = AppDomain.CurrentDomain.GetLoadableTypes()
            .Where(p => p.GetInterfaces().Contains(typeof(GenioServer.security.IRoleProvider)) && !p.IsAbstract)
            .Select(x => new SecurityProviderMetainfo(x));
        public static IEnumerable<SecurityProviderMetainfo> TypeList() => roleProviderList;

        public RoleProviderCfg()
        {
            obj = new CSGenio.RoleProviderCfgEl();
        }

        public RoleProviderCfg(CSGenio.RoleProviderCfgEl o)
        {
            obj = o;
        }
        
        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string Name
        {
            get { return obj.Name; }
            set { obj.Name = value; }
        }

        [Display(Name = "TIPO55111", ResourceType = typeof(Resources.Resources))]
        public string Type
        {
            get { return obj.Type; }
            set { obj.Type = value; }
        }

        [Display(Name = "CONFIGURACAO10928", ResourceType = typeof(Resources.Resources))]
        public CSGenio.SerializableDictionary<string, string> Options
        {
            get { return obj.Options; }
            set { obj.Options = value; }
        }

        [Display(Name = "PRECONDICAO44917", ResourceType = typeof(Resources.Resources))]
        public string Precond
        {
            get { return obj.Precond; }
            set { obj.Precond = value; }
        }

        [JsonIgnore]
        public CSGenio.RoleProviderCfgEl obj { get; set; }
        public string FormMode { get; set; }
        public int Rownum { get; set; }

    }

    public enum DisplayUserType
    {
        [Display(Name = "NORMAL32684", ResourceType = typeof(Resources.Resources))]
        Regular,
        [Display(Name = "CONVIDADO07696", ResourceType = typeof(Resources.Resources))]
        Guest,
        [Display(Name = "ADMINISTRADOR57294", ResourceType = typeof(Resources.Resources))]
        Admin
    }

    public class UserCfg
    {
        public UserCfg()
        {
            obj = new UserCfgEl();
        }
        public UserCfg(UserCfgEl o)
        {
            obj = o;
        }
        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string Name
        {
            get { return obj.Name; }
            set { obj.Name = value; }
        }
        [Display(Name = "PASSWORD09467", ResourceType = typeof(Resources.Resources))]
        public string Password
        {
            get { return obj.Password; }
            set { obj.Password = value; }
        }
        [Display(Name = "TIPO55111", ResourceType = typeof(Resources.Resources))]
        [JsonConverter(typeof(JsonStringEnumConverter<UserType>))]
        public UserType Type
        {
            get { return obj.Type; }
            set { obj.Type = value; }
        }
        [Display(Name = "LOGIN_AUTOMATICO22707", ResourceType = typeof(Resources.Resources))]
        public bool AutoLogin
        {
            get { return obj.AutoLogin; }
            set { obj.AutoLogin = value; }
        }
        [JsonIgnore]
        public UserCfgEl obj { get; set; }
        public string FormMode { get; set; }
        public int Rownum { get; set; }
    }

	public class MorePropertyCfg
    {
        public MorePropertyCfg()
        {
            obj = new MorePropertyCfgEl();
        }
        public MorePropertyCfg(MorePropertyCfgEl o)
        {
            obj = o;
        }
        public MorePropertyCfg(string key, string val)
        {
            obj = new MorePropertyCfgEl();
            obj.Key = key;
            obj.Val = val;
        }

        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string Key
        {
            get { return obj.Key; }
            set { obj.Key = value; }
        }

        [Display(Name = "PASSWORD09467", ResourceType = typeof(Resources.Resources))]
        public string Val
        {
            get { return obj.Val; }
            set { obj.Val = value; }
        }

        [JsonIgnore]
        public MorePropertyCfgEl obj { get; set; }
        public string FormMode { get; set; }
        public int Rownum { get; set; }

        public static IEnumerable<AdvancedPropertyItem> PropertyList
        {
            get
            {
                List<AdvancedPropertyItem> res = new List<AdvancedPropertyItem>();
                foreach (string t in ExtraProperties.GetInitialKeys())
                {
                    res.Add(new AdvancedPropertyItem() { Value = t, Text = t });
                }
                
                foreach (var t in ExtraProperties.GetAdvancedProperties())
                {
                    res.Add(
                        new AdvancedPropertyItem() { 
                            Value = t.Id,
                            Text = t.Label,
                            Type= t.Type,
                            TextResourceId = t.ResourceId,
                            TextHelpResourceId = t.HelpResourceId,
                            TextHelpResourceVerboseId = t.HelpResourceVerboseId
                        }
                    );
                }
                return res;
            }
        }
    }

    public class AdvancedPropertyItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public string TextResourceId { get; set; }
        public string TextHelpResourceId { get; set; }
        public string TextHelpResourceVerboseId { get; set; }
    }

    public class DateFormatCfg
    {
        public DateFormatCfg()
        {
            obj = new DateFormatXml();
        }
        public DateFormatCfg(CSGenio.DateFormatXml d)
        {
            obj = d;
        }
        [Display(Name = "DATA18071", ResourceType = typeof(Resources.Resources))]
        public string date
        {
            get { return obj.Date; }
            set { obj.Date = value; }
        }
        [Display(Name = "DATA_E_HORA33196", ResourceType = typeof(Resources.Resources))]
        public string dateTime
        {
            get { return obj.DateTime; }
            set { obj.DateTime = value; }
        }
        [Display(Name = "DATA__HORAS_E_SEGUND03637", ResourceType = typeof(Resources.Resources))]
        public string dateTimeSeconds
        {
            get { return obj.DateTimeSeconds; }
            set { obj.DateTimeSeconds = value; }
        }
        [Display(Name = "HORAS01448", ResourceType = typeof(Resources.Resources))]
        public string time
        {
            get { return obj.Time; }
            set { obj.Time = value; }
        }
        [JsonIgnore]
        public DateFormatXml obj { get; set; }
    }

	public class MessageQueue
    {
		public MessageQueue()
        {
            obj = new messagequeueing();
        }

		[Display(Name = "LISTA_DE_MENSAGENS31887", ResourceType = typeof(Resources.Resources))]
        public List<QueueCfg> Queues { get; set; }

        [Display(Name = "CONFIGURACAO_DE_ACKS49550", ResourceType = typeof(Resources.Resources))]
        public List<QueueACK> Acks { get; set; }

		[Display(Name = "JOURNAL_TIMEOUT__MIN38634", ResourceType = typeof(Resources.Resources))]
        public string Journaltimeout { get; set; }

		[Display(Name = "NUMERO_MAXIMO_DE_TEN51201", ResourceType = typeof(Resources.Resources))]
        public string Maxsendnumber { get; set; }

        [JsonIgnore]
        public CSGenio.messagequeueing obj { get; set; }
    }

    public class QueueCfg
    {
        public QueueCfg()
        {
            obj = new Queue();
        }
        public QueueCfg(CSGenio.Queue u)
        {
            obj = u;
        }
        [Display(Name = "NOME_DA_QUEUE56594", ResourceType = typeof(Resources.Resources))]
        public string queue
        {
            get { return obj.queue; }
            set { obj.queue = value; }
        }
        [Display(Name = "TRAJETO_DA_QUEUE07185", ResourceType = typeof(Resources.Resources))]
        public string path
        {
            get { return obj.path; }
            set { obj.path = value; }
        }
        [Display(Name = "CANAL_DA_QUEUE34934", ResourceType = typeof(Resources.Resources))]
        public string queueChannel
        {
            get { return obj.channelId; }
            set { obj.channelId = value; }
        }
        [Display(Name = "ANO33022", ResourceType = typeof(Resources.Resources))]
        public string Qyear
        {
            get { return obj.Qyear; }
            set { obj.Qyear = value; }
        }
        [Display(Name = "UNICODE63246", ResourceType = typeof(Resources.Resources))]
        public bool Unicode
        {
            get { return obj.Unicode; }
            set { obj.Unicode = value; }
        }
        [Display(Name = "USA_MSMQ18528", ResourceType = typeof(Resources.Resources))]
        public bool UsesMsmq
        {
            get { return obj.UsesMsmq; }
            set { obj.UsesMsmq = value; }
        }
		[Display(Name = "JOURNAL20931", ResourceType = typeof(Resources.Resources))]
        public bool Journal
        {
            get { return obj.Journal; }
            set { obj.Journal = value; }
        }

        [Display(Name = "TAMANHO_DO_BLOCO42316", ResourceType = typeof(Resources.Resources))]
        public int Blocksize
        {
            get { return obj.BlockSize; }
            set { obj.BlockSize = value; }
        }

        [JsonIgnore]
        public Queue obj { get; set; }
        public string FormMode { get; set; }
        public int Rownum { get; set; }
    }

	public class QueueACK
    {
        public QueueACK()
        {
            obj = new ACK();
        }

		public QueueACK(CSGenio.ACK u)
        {
            obj = u;
        }

		[Display(Name = "QUEUE_ORIGEM31278", ResourceType = typeof(Resources.Resources))]
        public string source
        {
            get { return obj.Source; }
            set { obj.Source = value; }
        }

		[Display(Name = "QUEUE_ACK30680", ResourceType = typeof(Resources.Resources))]
        public string ackQueue
        {
            get { return obj.ACKqueue; }
            set { obj.ACKqueue = value; }
        }

        [Display(Name = "TAMANHO_DO_BLOCO42316", ResourceType = typeof(Resources.Resources))]
        public int Blocksize
        {
            get { return obj.BlockSize; }
            set { obj.BlockSize = value; }
        }

        [JsonIgnore]
        public ACK obj { get; set; }
        public string FormMode { get; set; }
        public int Rownum { get; set; }
    }

    public class CoreCfg
    {
        public CoreCfg()
        {
            Obj = new CoreXml();
        }
        public CoreCfg(CSGenio.CoreXml c)
        {
            Obj = c;
        }
        [Display(Name = "INDEX00140", ResourceType = typeof(Resources.Resources))]
        public string Index
        {
            get { return Obj.Index; }
            set { Obj.Index = value; }
        }
        [Display(Name = "ID36840", ResourceType = typeof(Resources.Resources))]
        public string Id
        {
            get { return Obj.Name; }
            set { Obj.Name = value; }
        }
        [Display(Name = "AREA19058", ResourceType = typeof(Resources.Resources))]
        public string Area
        {
            get { return Obj.Area; }
            set { Obj.Area = value; }
        }
        [Display(Name = "URL05719", ResourceType = typeof(Resources.Resources))]
        public string Url
        {
            get { return Obj.Url; }
            set { Obj.Url = value; }
        }
        [Display(Name = "FSCRAWLER01982", ResourceType = typeof(Resources.Resources))]
        public string Urlfscrawler
        {
            get { return Obj.Urlfscrawler; }
            set { Obj.Urlfscrawler = value; }
        }
        [Display(Name = "NOME_DE_UTILIZADOR58858", ResourceType = typeof(Resources.Resources))]
        public string ElasticUser
        {
            get { return Obj.Username; }
            set { Obj.Username = value; }
        }
        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        public string ElasticPsw
        {
            get { return Obj.Password; }
            set { Obj.Password = value; }
        }
        [JsonIgnore]
        public CoreXml Obj { get; set; }
        public string FormMode { get; set; }
        public int Rownum { get; set; }
    }

    public class SecurityCfg
    {
        public SecurityCfg()
        {
            IdentityProviders = new List<IdentityProviderCfg>();
            RoleProviders = new List<RoleProviderCfg>();
            Users = new List<UserCfg>();
        }

        [Display(Name = "MODO_DE_AUTENTICACAO19339", ResourceType = typeof(Resources.Resources))]
        public GenioServer.security.AuthenticationMode AuthenticationMode { get; set; }
        [Display(Name = "POLITICA_DE_SESSOES_19368", ResourceType = typeof(Resources.Resources))]
        public GenioServer.security.MultiSessionMode AllowMultiSessionPerUser { get; set; }
        [Display(Name = "PERMITE_RECUPERACAO_41959", ResourceType = typeof(Resources.Resources))]
        public bool AllowAuthenticationRecovery { get; set; }
		[Display(Name = "OBRIGATORIO_A_UTILIZ32451", ResourceType = typeof(Resources.Resources))]
        public bool Mandatory2FA { get; set; }

        [Display(Name = "FORNECEDORES_DE_IDEN35608", ResourceType = typeof(Resources.Resources))]
        public List<IdentityProviderCfg> IdentityProviders { get; set; }

        [Display(Name = "FORNECEDORES_DE_AUTO29899", ResourceType = typeof(Resources.Resources))]
        public List<RoleProviderCfg> RoleProviders { get; set; }

        [Display(Name = "UTILIZADORES_FIXOS00716", ResourceType = typeof(Resources.Resources))]
        public List<UserCfg> Users { get; set; }

        [Display(Name = "MINIMO_DE_CARACTERES10869", ResourceType = typeof(Resources.Resources))]
        public int MinCharacters { get; set; }

        [Display(Name = "EXPIRACAO_DA_PASSWOR46052", ResourceType = typeof(Resources.Resources))]
        public bool ExpirationDateBool { get; set; }

        [Display(Name = "EXPIRACAO43455", ResourceType = typeof(Resources.Resources))]
        public string ExpirationDate { get; set; }

        [Display(Name = "FORCA_DA_PASSWORD58535", ResourceType = typeof(Resources.Resources))]
        public GenioServer.security.PasswordStrength PasswordStrength { get; set; }

        [Display(Name = "ALGORITMO_DE_ENCRIPT09649", ResourceType = typeof(Resources.Resources))]
        public GenioServer.security.PasswordAlgorithms PasswordAlgorithms { get; set; }

		[Display(Name = "NUMERO_MAXIMO_TENTAT34521", ResourceType = typeof(Resources.Resources))]
        public int MaxAttempts { get; set; }

        [Display(Name = "TIME_OUT_DA_SESSAO36825", ResourceType = typeof(Resources.Resources))]
        public int SessionTimeOut { get; set; }

        public bool UsePasswordBlacklist { get; set; }
    }

    public class PathCfg
    {
        [Display(Name = "CAMINHO_PARA_A_APLIC44450", ResourceType = typeof(Resources.Resources))]
        public string pathApp { get; set; }

        [Display(Name = "CAMINHO_PARA_DOCUMEN18456", ResourceType = typeof(Resources.Resources))]
        public string pathDocuments { get; set; }
      }

}
