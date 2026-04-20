using System;
using CSGenio.framework;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using CSGenio.persistence;
using System.Text;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
	/// <summary>
	/// Summary description for CSArea.
	/// </summary>
	public class CSGenioAuserauthorization : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAuserauthorization(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAuserauthorization(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "userauthorization";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codua";
            info.HumanKeyName = "codua";
			info.ShadowTabKeyName = "";
			info.Alias = "userauthorization";
			info.IsDomain =  true;
			info.AreaDesignation = "Autorização";
			info.AreaPluralDesignation = "Autorizações";
			info.DescriptionCav = "Autorização";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field(info.Alias, "codua", FieldType.KEY_INT));
			info.RegisterFieldDB(new Field(info.Alias, "codpsw", FieldType.KEY_INT));
			info.RegisterFieldDB(new Field(info.Alias, "sistema", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "modulo", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "naodupli", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "role", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "nivel", FieldType.NUMERIC));
            info.RegisterFieldDB(new Field(info.Alias, "opercria", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "opermuda", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "datamuda", FieldType.DATETIMESECONDS));
			info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

            // Carimbos automáticos na BD
            //------------------------------
   			info.StampFieldsIns = new string[] {
			 "opercria","datacria"
			};
  			info.StampFieldsAlt = new string[] {
			 "opermuda","datamuda"
			};

			// Relações Filhas
			//------------------------------

			// Relações Mãe
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("psw", new Relation("FOR", "userauthorization", "userauthorization", "codua", "codpsw", "FOR", "userlogin", "psw", "codpsw", "codpsw"));

			// Pathways
			//------------------------------
            info.Pathways = new Dictionary<string, string>(1);
            info.Pathways.Add("psw", "psw");

			// Levels de acesso
			//------------------------------
			info.QLevel = new QLevel();
			info.QLevel.Query = Role.UNAUTHORIZED;
			info.QLevel.Create = Role.UNAUTHORIZED;
			info.QLevel.AlterAlways = Role.UNAUTHORIZED;
			info.QLevel.RemoveAlways = Role.UNAUTHORIZED;

			// Automatic audit stamps in BD
            //------------------------------

			return info;
		}
		
		/// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL FOR TABAUX UserAuthorization]/

		
        public static FieldRef FldOperMuda { get { return m_FldOperMuda; } }
        private static FieldRef m_FldOperMuda = new FieldRef("userauthorization", "opermuda");        

		public string ValOperMuda
		{
			get { return (string)returnValueField(FldOperMuda); }
			set { insertNameValueField(FldOperMuda, value); }
		}

        public static FieldRef FldOperCria { get { return m_FldOperCria; } }
        private static FieldRef m_FldOperCria = new FieldRef("userauthorization", "opercria");

		public string ValOperCria
		{
			get { return (string)returnValueField(FldOperCria); }
			set { insertNameValueField(FldOperCria, value); }
		}
		
        public static FieldRef FldDataMuda { get { return m_FldDataMuda; } }
        private static FieldRef m_FldDataMuda = new FieldRef("userauthorization", "datamuda");

		public DateTime ValDataMuda
		{
			get { return (DateTime)returnValueField(FldDataMuda); }
			set { insertNameValueField(FldDataMuda, value); }
		}

        public static FieldRef FldDataCria { get { return m_FldDataCria; } }
        private static FieldRef m_FldDataCria = new FieldRef("userauthorization", "datacria");

		public DateTime ValDataCria
		{
			get { return (DateTime)returnValueField(FldDataCria); }
			set { insertNameValueField(FldDataCria, value); }
		}
		
        public static FieldRef FldCodua { get { return m_FldCodua; } }
        private static FieldRef m_FldCodua = new FieldRef("userauthorization", "codua");

        public string ValCodua
        {
            get { return (string)returnValueField(FldCodua); }
            set { insertNameValueField(FldCodua, value); }
        }

        public static FieldRef FldCodpsw { get { return m_FldCodpsw; } }
        private static FieldRef m_FldCodpsw = new FieldRef("userauthorization", "codpsw");

        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        public static FieldRef FldSistema { get { return m_FldSistema; } }
        private static FieldRef m_FldSistema = new FieldRef("userauthorization", "sistema");

        public string ValSistema
        {
            get { return (string)returnValueField(FldSistema); }
            set { insertNameValueField(FldSistema, value); }
        }

        public static FieldRef FldModulo { get { return m_FldModulo; } }
        private static FieldRef m_FldModulo = new FieldRef("userauthorization", "modulo");

        public string ValModulo
        {
            get { return (string)returnValueField(FldModulo); }
            set { insertNameValueField(FldModulo, value); }
        }

        public static FieldRef FldNaodupli { get { return m_FldNaodupli; } }
        private static FieldRef m_FldNaodupli = new FieldRef("userauthorization", "naodupli");

        public string ValNaodupli
        {
            get { return (string)returnValueField(FldNaodupli); }
            set { insertNameValueField(FldNaodupli, value); }
        }

        public static FieldRef FldNivel { get { return m_FldNivel; } }
        private static FieldRef m_FldNivel = new FieldRef("userauthorization", "nivel");

        /// <summary>
        /// Returns the level that was saved.
        /// For unknown reasons, levels were sometimes 0 while the role was marked correctly, hence this indirection
        /// </summary>
        public decimal ValNivel
        {
            get {
                decimal level = (decimal)returnValueField(FldNivel);
                decimal role;
                if (level == 0 && decimal.TryParse(ValRole, out role))
                {
                    return role;
                }
                return level;
            }
            set { insertNameValueField(FldNivel, value); }
        }
        
        public static FieldRef FldRole { get { return m_FldRole; } }
        private static FieldRef m_FldRole = new FieldRef("userauthorization", "role");

        /// <summary>
        /// Returns the role id. Because backoffice only writes the Nivel field we do this indirection when the db field is empty.
        /// </summary>
        public string ValRole
        {
            get {
                string role = (string)returnValueField(FldRole);
                if (String.IsNullOrEmpty(role))
                    return returnValueField(FldNivel).ToString();
                else
                    return role;
            }
            set {
                insertNameValueField(FldRole, value);
            }
        }
        
        
        public static FieldRef FldZzstate { get { return m_FldZzstate; } }
        private static FieldRef m_FldZzstate = new FieldRef("userauthorization", "zzstate");

        public int ValZzstate
        {
            get { return (int)returnValueField(FldZzstate); }
            set { insertNameValueField(FldZzstate, value); }
        }

        public override StatusMessage change(PersistentSupport sp, CriteriaSet condition)
        {
            //Override this method to ensure every call to this also fills the role value (QWeb for example)
            if (!Fields.ContainsKey(FldRole.FullName) && Fields.ContainsKey(FldNivel.FullName))
            {
                var field = new RequestedField(FldRole.FullName, Alias);
                field.Value = Fields[FldNivel.FullName].Value.ToString();
                Fields[FldRole.FullName] = field;
            }
            return base.change(sp, condition);
        }

        /// <summary>
        /// Obtains a partially populated area with the record corresponding to a primary key
        /// </summary>
        /// <param name="sp">Persistent support from where to get the registration</param>
        /// <param name="key">The value of the primary key</param>
        /// <param name="user">The context of the user</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <returns>An area with the fields requests of the record read or null if the key does not exist</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static CSGenioAuserauthorization search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioAuserauthorization area = new CSGenioAuserauthorization(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAuserauthorization> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return sp.searchListWhere<CSGenioAuserauthorization>(where, user, null);
        }
    
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAuserauthorization> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields)
        {
            return sp.searchListWhere<CSGenioAuserauthorization>(where, user, fields);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAuserauthorization> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAuserauthorization>(where, listing);
        }

        public bool IsRole(string module, Role role)
        {
            if(role.Type == RoleType.LEVEL)
            {
                return ValModulo == module && ValNivel == role.GetLevelInt();
            }
            else
            {
                return ValModulo == module && role.Id == ValRole;
            }
        }

        public static void InsertRole(PersistentSupport sp, User user, string codpsw, string module, Role role)
        {
            //Inserir nível
            CSGenioAuserauthorization userauth = new CSGenioAuserauthorization(user, module);
            userauth.ValSistema = "FOR";
            userauth.ValModulo = module;
            userauth.ValNaodupli = codpsw.Replace("{", "").Replace("}", "").Replace("-", "").ToUpper() + module;
            userauth.ValRole = role.Id;
            if (role.Type != RoleType.ROLE)
                userauth.ValNivel = role.GetLevelInt();
            userauth.ValCodpsw = codpsw;
            userauth.insert(sp);
        }

	}
}
