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
	public class CSGenioAusrwid : DbArea
	{
	    /// <summary>
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAusrwid(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAusrwid(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "forusrwid";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codusrwid";
            info.HumanKeyName = "widget";
			info.ShadowTabKeyName = "";
			info.Alias = "usrwid";
			info.IsDomain =  false;
			info.AreaDesignation = "Widget do utilizador";
			info.AreaPluralDesignation = "Widgets do utilizador";
			info.DescriptionCav = "Widget do utilizador";
			
			//sincronizaÁ„o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field(info.Alias, "codusrwid", FieldType.KEY_INT));
			info.RegisterFieldDB(new Field(info.Alias, "codlstusr", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "widget", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "rowkey", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "visible", FieldType.LOGIC));
            info.RegisterFieldDB(new Field(info.Alias, "hposition", FieldType.INTEGER));
            info.RegisterFieldDB(new Field(info.Alias, "vposition", FieldType.INTEGER));
            info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));


			// RelaÁıes Filhas
			//------------------------------

			// RelaÁıes M„e
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
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL FOR TABAUX usrwid]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        public static FieldRef FldCodusrwid { get { return m_fldCodusrwid; } }
        private static FieldRef m_fldCodusrwid = new FieldRef("usrwid", "codusrwid");

        public string ValCodusrwid
        {
            get { return (string)returnValueField(FldCodusrwid); }
            set { insertNameValueField(FldCodusrwid, value); }
        }


        public static FieldRef FldCodlstusr { get { return m_fldCodlstusr; } }
        private static FieldRef m_fldCodlstusr = new FieldRef("usrwid", "codlstusr");

        public string ValCodlstusr
        {
            get { return (string)returnValueField(FldCodlstusr); }
            set { insertNameValueField(FldCodlstusr, value); }
        }


        public static FieldRef FldWidget { get { return m_fldWidget; } }
        private static FieldRef m_fldWidget = new FieldRef("usrwid", "widget");

        public string ValWidget
        {
            get { return (string)returnValueField(FldWidget); }
            set { insertNameValueField(FldWidget, value); }
        }
        
        
        public static FieldRef FldRowkey { get { return m_fldRowkey; } }
        private static FieldRef m_fldRowkey = new FieldRef("usrwid", "rowkey");

        public string ValRowkey
        {
            get { return (string)returnValueField(FldRowkey); }
            set { insertNameValueField(FldRowkey, value); }
        }


        public static FieldRef FldVisible { get { return m_fldVisible; } }
        private static FieldRef m_fldVisible = new FieldRef("usrwid", "visible");

        public int ValVisible
        {
            get { return (int)returnValueField(FldVisible); }
            set { insertNameValueField(FldVisible, value); }
        }


        public static FieldRef FldHposition { get { return m_fldHposition; } }
        private static FieldRef m_fldHposition = new FieldRef("usrwid", "hposition");

        public int ValHposition
        {
            get { return (int)returnValueField(FldHposition); }
            set { insertNameValueField(FldHposition, value); }
        }


        public static FieldRef FldVposition { get { return m_fldVposition; } }
        private static FieldRef m_fldVposition = new FieldRef("usrwid", "vposition");

        public int ValVposition
        {
            get { return (int)returnValueField(FldVposition); }
            set { insertNameValueField(FldVposition, value); }
        }


        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("usrwid", "zzstate");

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
        public static CSGenioAusrwid search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //to proteger chamadas "cegas"
                return null;
            CSGenioAusrwid area = new CSGenioAusrwid(user, user.CurrentModule);
            if(sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAusrwid> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return searchList(sp, user, where, null);
        }
        
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N√£o devem ser utilizadas opera√ß√µes de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAusrwid> searchList(PersistentSupport sp, User user, CriteriaSet where, string []fields)
        {
            return sp.searchListWhere<CSGenioAusrwid>(where, user, fields);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N√£o devem ser utilizadas opera√ß√µes de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAusrwid> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAusrwid>(where, user, fields, distinct, noLock);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N√£o devem ser utilizadas opera√ß√µes de persistence sobre um registo parcialmente posicionado</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAusrwid> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAusrwid>(where, listing);			
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAusrwid> searchList(PersistentSupport sp, User user, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, user, where, null, distinct, noLock);
        }	

	}
}
