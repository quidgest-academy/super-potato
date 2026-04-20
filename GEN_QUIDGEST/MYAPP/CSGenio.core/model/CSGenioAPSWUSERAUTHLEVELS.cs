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
	public class CSGenioApswuserauthlevels : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioApswuserauthlevels(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioApswuserauthlevels(User user) : this(user, user.CurrentModule)
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
			info.Alias = "pswuserauthlevels";
			info.IsDomain =  false;
			info.AreaDesignation = "AutorizaÃ§Ã£o";
			info.AreaPluralDesignation = "AutorizaÃ§Ãµes";
			info.DescriptionCav = "AutorizaÃ§Ã£o";
			
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
			info.RegisterFieldDB(new Field(info.Alias, "nivel", FieldType.NUMERIC));
            info.RegisterFieldDB(new Field(info.Alias, "role", FieldType.TEXT));            
			info.RegisterFieldDB(new Field(info.Alias, "opercria", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATE));
			info.RegisterFieldDB(new Field(info.Alias, "opermuda", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "datamuda", FieldType.DATE));
			info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

			// Relações Filhas
			//------------------------------

			// Relações Mãe
			//------------------------------

			// Pathways
			//------------------------------

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

		// USE /[MANUAL FOR TABAUX pswuserauthlevels]/

		
        public static FieldRef FldCodua { get { return m_FldCodua; } }
        private static FieldRef m_FldCodua = new FieldRef("pswuserauthlevels", "codua");

        public string ValCodua
        {
            get { return (string)returnValueField(FldCodua); }
            set { insertNameValueField(FldCodua, value); }
        }

        public static FieldRef FldCodpsw { get { return m_FldCodpsw; } }
        private static FieldRef m_FldCodpsw = new FieldRef("pswuserauthlevels", "codpsw");

        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        public static FieldRef FldSistema { get { return m_FldSistema; } }
        private static FieldRef m_FldSistema = new FieldRef("pswuserauthlevels", "sistema");

        public string ValSistema
        {
            get { return (string)returnValueField(FldSistema); }
            set { insertNameValueField(FldSistema, value); }
        }

        public static FieldRef FldModulo { get { return m_FldModulo; } }
        private static FieldRef m_FldModulo = new FieldRef("pswuserauthlevels", "modulo");

        public string ValModulo
        {
            get { return (string)returnValueField(FldModulo); }
            set { insertNameValueField(FldModulo, value); }
        }
        
        public static FieldRef FldNivel { get { return m_FldNivel; } }
        private static FieldRef m_FldNivel = new FieldRef("pswuserauthlevels", "nivel");

        public decimal ValNivel
        {
            get { return (decimal)returnValueField(FldNivel); }
            set { insertNameValueField(FldNivel, value); }
        }

        public static FieldRef FldRole { get { return m_FldRole; } }
        private static FieldRef m_FldRole = new FieldRef("pswuserauthlevels", "role");

        public string ValRole
        {
            get { return (string)returnValueField(FldRole); }
            set { insertNameValueField(FldRole, value); }
        }
        
        public static FieldRef FldZzstate { get { return m_FldZzstate; } }
        private static FieldRef m_FldZzstate = new FieldRef("pswuserauthlevels", "zzstate");

        public int ValZzstate
        {
            get { return (int)returnValueField(FldZzstate); }
            set { insertNameValueField(FldZzstate, value); }
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
        public static CSGenioApswuserauthlevels search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioApswuserauthlevels area = new CSGenioApswuserauthlevels(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condiÃ§Ã£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condiÃ§Ã£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioApswuserauthlevels> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return sp.searchListWhere<CSGenioApswuserauthlevels>(where, user, null);
        }
    
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condiÃ§Ã£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condiÃ§Ã£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>NÃ£o devem ser utilizadas operaÃ§Ãµes de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioApswuserauthlevels> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields)
        {
            return sp.searchListWhere<CSGenioApswuserauthlevels>(where, user, fields);
        }

	}
}
