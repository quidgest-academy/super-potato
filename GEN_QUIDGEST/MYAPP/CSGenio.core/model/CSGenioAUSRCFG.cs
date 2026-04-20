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
	public class CSGenioAusrcfg : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAusrcfg(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAusrcfg(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "forusrcfg";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codusrcfg";
            info.HumanKeyName = "codusrcfg";
			info.ShadowTabKeyName = "";
			info.Alias = "usrcfg";
			info.IsDomain =  true;
			info.AreaDesignation = "User configuration";
			info.AreaPluralDesignation = "User configurations";
			info.DescriptionCav = "User configuration";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
      info.RegisterFieldDB(new Field(info.Alias, "codusrcfg", FieldType.KEY_INT));
      info.RegisterFieldDB(new Field(info.Alias, "codpsw", FieldType.KEY_INT));  
	  info.RegisterFieldDB(new Field(info.Alias, "modulo", FieldType.TEXT));
      info.RegisterFieldDB(new Field(info.Alias, "tipo", FieldType.TEXT));
      info.RegisterFieldDB(new Field(info.Alias, "id", FieldType.TEXT));
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

		// USE /[MANUAL FOR TABAUX USRCFG]/

		        public static FieldRef FldCodusrcfg { get { return m_FldCodusrcfg; } }
        private static FieldRef m_FldCodusrcfg = new FieldRef("usrcfg", "codusrcfg");

        public string ValCodusrcfg
        {
            get { return (string)returnValueField(FldCodusrcfg); }
            set { insertNameValueField(FldCodusrcfg, value); }
        }

        public static FieldRef FldCodpsw { get { return m_FldCodpsw; } }
        private static FieldRef m_FldCodpsw = new FieldRef("usrcfg", "codpsw");

        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        public static FieldRef FldModulo { get { return m_FldModulo; } }
        private static FieldRef m_FldModulo = new FieldRef("usrcfg", "modulo");

        public string ValModulo
        {
            get { return (string)returnValueField(FldModulo); }
            set { insertNameValueField(FldModulo, value); }
        }

        public static FieldRef FldTipo { get { return m_FldTipo; } }
        private static FieldRef m_FldTipo = new FieldRef("usrcfg", "tipo");

        public string ValTipo
        {
            get { return (string)returnValueField(FldTipo); }
            set { insertNameValueField(FldTipo, value); }
        }

        public static FieldRef FldId { get { return m_FldId; } }
        private static FieldRef m_FldId = new FieldRef("usrcfg", "id");

        public string ValId
        {
            get { return (string)returnValueField(FldId); }
            set { insertNameValueField(FldId, value); }
        }
		
        public static FieldRef FldZzstate { get { return m_FldZzstate; } }
        private static FieldRef m_FldZzstate = new FieldRef("usrcfg", "zzstate");

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
        public static CSGenioAusrcfg search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioAusrcfg area = new CSGenioAusrcfg(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="module">Current module (null if all modules)</param>
        /// <returns>Uma lista de registos da areas do tipo "Favoritos" com todos os fields preenchidos</returns>
        public static List<CSGenioAusrcfg> searchListBookmarks(PersistentSupport sp, User user, string module = null)
        {
            return searchList(sp, user, "FV", module);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="tipo">Configuration type</param>
        /// <param name="module">Current module (null if all modules)</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAusrcfg> searchList(PersistentSupport sp, User user, string tipo, string module = null)
        {
            var where = CriteriaSet.And()
                    .Equal(CSGenioAusrcfg.FldCodpsw, user.Codpsw)
                    .Equal(CSGenioAusrcfg.FldTipo, tipo);
            if(module != null)
                where.Equal(CSGenioAusrcfg.FldModulo, module);

            return searchList(sp, user, where, null);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAusrcfg> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return searchList(sp, user, where, null);
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
        public static List<CSGenioAusrcfg> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields)
        {
            return sp.searchListWhere<CSGenioAusrcfg>(where, user, fields);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condiÃ§Ã£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAusrcfg> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields, bool distinct)
        {
            return sp.searchListWhere<CSGenioAusrcfg>(where, user, fields, distinct);			
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condição de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAusrcfg> searchList(PersistentSupport sp, User user, CriteriaSet where, bool distinct)
        {
            return searchList(sp, user, where, null, distinct);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAusrcfg> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAusrcfg>(where, listing);
        }

	}
}
