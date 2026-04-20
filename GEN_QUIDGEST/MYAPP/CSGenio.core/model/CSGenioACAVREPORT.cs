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
	public class CSGenioAcavreport : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAcavreport(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAcavreport(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "cavreport";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codreport";
            info.HumanKeyName = "title";
			info.ShadowTabKeyName = "";
			info.Alias = "cavreport";
			info.IsDomain =  true;
			info.AreaDesignation = "Lista de relatorios cav";
			info.AreaPluralDesignation = "Lista de relatorios cav";
			info.DescriptionCav = "Lista de relatorios cav";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
            info.RegisterFieldDB(new Field(info.Alias, "codreport", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "title", FieldType.TEXT));
	        info.RegisterFieldDB(new Field(info.Alias, "acesso", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "dataxml", FieldType.MEMO));
			info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS));	  
			info.RegisterFieldDB(new Field(info.Alias, "opercria", FieldType.TEXT)); 
			info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));			

            // Carimbos automáticos na BD
            //------------------------------
   			info.StampFieldsIns = new string[] {
			 "opercria","datacria"
			};

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

		// USE /[MANUAL FOR TABAUX cavreport]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        ////////////////////////////////////////////////////////////////////////////BD fields CONVERTED TO Areainfo style
        /// <summary>Campo : "PK da tabela cavreport" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodreport { get { return m_fldCodreport; } }
        private static FieldRef m_fldCodreport = new FieldRef("cavreport", "codreport");

        /// <summary>Campo : "PK da tabela cavreport" Tipo: "+" Formula:  ""</summary>
        public string ValCodreport
        {
            get { return (string)returnValueField(FldCodreport); }
            set { insertNameValueField(FldCodreport, value); }
        }
		
		/// <summary>Campo : "Título do report" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldTitle { get { return m_fldTitle; } }
        private static FieldRef m_fldTitle = new FieldRef("cavreport", "title");

        /// <summary>Campo : "Título do report" Tipo: "C" Formula:  ""</summary>
        public string ValTitle
        {
            get { return (string)returnValueField(FldTitle); }
            set { insertNameValueField(FldTitle, value); }
        }				

        /// <summary>Campo : "Tipo de acesso" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldAcesso { get { return m_fldAcesso; } }
        private static FieldRef m_fldAcesso = new FieldRef("cavreport", "acesso");

        /// <summary>Campo : "Tipo de acesso" Tipo: "C" Formula:  ""</summary>
        public string ValAcesso
        {
            get { return (string)returnValueField(FldAcesso); }
            set { insertNameValueField(FldAcesso, value); }
        }

		/// <summary>Campo : "XML do report" Tipo: "MO" Formula:  ""</summary>
        public static FieldRef FldDataxml { get { return m_fldDataxml; } }
        private static FieldRef m_fldDataxml = new FieldRef("cavreport", "dataxml");

        /// <summary>Campo : "XML do report" Tipo: "MO" Formula:  ""</summary>
        public string ValDataxml
        {
            get { return (string)returnValueField(FldDataxml); }
            set { insertNameValueField(FldDataxml, value); }
        }  
       
	    /// <summary>Field : "Criado em" Tipo: "CD" Formula:  ""</summary>
	    public static FieldRef FldDatacria { get { return m_FldDatacria; } }
        private static FieldRef m_FldDatacria = new FieldRef("cavreport", "datacria");

		/// <summary>Field : "Criado em" Tipo: "CD" Formula:  ""</summary>
        public DateTime ValDatacria
        {
            get { return (DateTime)returnValueField(FldDatacria); }
            set { insertNameValueField(FldDatacria, value); }
        }
		
		/// <summary>Field : "Criado por" Tipo: "ON" Formula:  ""</summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("cavreport", "opercria");

		/// <summary>Field : "Criado por" Tipo: "ON" Formula:  ""</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}
	   
        /// <summary>Campo : "ZZSTATE" Tipo: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("cavreport", "zzstate");

        /// <summary>Campo : "ZZSTATE" Tipo: "INT"</summary>
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
        public static CSGenioAcavreport search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //para proteger chamadas "cegas"
                return null;
            CSGenioAcavreport area = new CSGenioAcavreport(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        public static List<CSGenioAcavreport> searchList(PersistentSupport sp, User User, CriteriaSet where)
        {
            return searchList(sp, User, where, null);
        }
   
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAcavreport> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos)
        {
            return sp.searchListWhere<CSGenioAcavreport>(where, User, campos);
        }
		
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
		public static void searchListAdvancedWhere(PersistentSupport sp, User User, CriteriaSet where, ListingMVC<CSGenioAcavreport> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAcavreport>(where, listing);
        }
		
        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAcavreport> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAcavreport>(where, User, campos, distinct, noLock);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="campos">Os campos a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        /// <remarks>Não devem ser utilizadas operações de persistence sobre um registo parcialmente posicionado</remarks>
		public static List<CSGenioAcavreport> searchList(PersistentSupport sp, User User, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, User, where, null, distinct, noLock);
        }

	}
}
