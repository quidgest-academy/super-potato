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
	public class CSGenioAlstusr : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAlstusr(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAlstusr(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "forlstusr";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codlstusr";
            info.HumanKeyName = "descric";
			info.ShadowTabKeyName = "";
			info.Alias = "lstusr";
			info.IsDomain =  false;
			info.AreaDesignation = "Lista de utilizador";
			info.AreaPluralDesignation = "Listas de utilizadores";
			info.DescriptionCav = "Lista de utilizador";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
			info.RegisterFieldDB(new Field(info.Alias, "codlstusr", FieldType.KEY_INT));
			info.RegisterFieldDB(new Field(info.Alias, "codpsw", FieldType.KEY_INT));
			info.RegisterFieldDB(new Field(info.Alias, "idlist", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "descric", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "modulo", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "sistema", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "ordercol", FieldType.INTEGER));
            info.RegisterFieldDB(new Field(info.Alias, "ordertype", FieldType.INTEGER));
            info.RegisterFieldDB(new Field(info.Alias, "data", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

            // Carimbos automáticos na BD
            //------------------------------
   			info.StampFieldsIns = new string[] {
             "data"
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

		// USE /[MANUAL FOR TABAUX lstusr]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        ////////////////////////////////////////////////////////////////////////////BD fields CONVERTED TO Areainfo style
        /// <summary>Campo : "PK da tabela lstusr" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodlstusr { get { return m_fldCodlstusr; } }
        private static FieldRef m_fldCodlstusr = new FieldRef("lstusr", "codlstusr");

        /// <summary>Campo : "PK da tabela lstusr" Tipo: "+" Formula:  ""</summary>
        public string ValCodlstusr
        {
            get { return (string)returnValueField(FldCodlstusr); }
            set { insertNameValueField(FldCodlstusr, value); }
        }

        /// <summary>Campo : "PK da tabela psw" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodpsw { get { return m_fldCodpsw; } }
        private static FieldRef m_fldCodpsw = new FieldRef("lstusr", "codpsw");

        /// <summary>Campo : "PK da tabela psw" Tipo: "+" Formula:  ""</summary>
        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        /// <summary>Campo : "Nome da lista" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldDescric { get { return m_fldDescric; } }
        private static FieldRef m_fldDescric = new FieldRef("lstusr", "descric");

        /// <summary>Campo : "Nome da lista" Tipo: "C" Formula:  ""</summary>
        public string ValDescric
        {
            get { return (string)returnValueField(FldDescric); }
            set { insertNameValueField(FldDescric, value); }
        }

        /// <summary>Campo : "ID da lista" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldIdlist { get { return m_fldIdlist; } }
        private static FieldRef m_fldIdlist = new FieldRef("lstusr", "idlist");

        /// <summary>Campo : "ID da lista" Tipo: "C" Formula:  ""</summary>
        public string ValIdlist
        {
            get { return (string)returnValueField(FldIdlist); }
            set { insertNameValueField(FldIdlist, value); }
        }

        /// <summary>Campo : "Módulo" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldModulo { get { return m_fldModulo; } }
        private static FieldRef m_fldModulo = new FieldRef("lstusr", "modulo");

        /// <summary>Campo : "Módulo" Tipo: "C" Formula:  ""</summary>
        public string ValModulo
        {
            get { return (string)returnValueField(FldModulo); }
            set { insertNameValueField(FldModulo, value); }
        }

        /// <summary>Campo : "Sistema" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldSistema { get { return m_fldSistema; } }
        private static FieldRef m_fldSistema = new FieldRef("lstusr", "sistema");

        /// <summary>Campo : "Sistema" Tipo: "C" Formula:  ""</summary>
        public string ValSistema
        {
            get { return (string)returnValueField(FldSistema); }
            set { insertNameValueField(FldSistema, value); }
        }

        /// <summary>Campo : "Coluna de ordenação" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldOrdercol { get { return m_fldOrdercol; } }
        private static FieldRef m_fldOrdercol = new FieldRef("lstusr", "ordercol");

        /// <summary>Campo : "Coluna de ordenação" Tipo: "N" Formula:  ""</summary>
        public int ValOrdercol
        {
            get { return (int)returnValueField(FldOrdercol); }
            set { insertNameValueField(FldOrdercol, value); }
        }

        /// <summary>Campo : "Tipo de ordenação" Tipo: "N" Formula:  ""</summary>
        public static FieldRef FldOrdertype { get { return m_fldOrdertype; } }
        private static FieldRef m_fldOrdertype = new FieldRef("lstusr", "ordertype");

        /// <summary>Campo : "Tipo de ordenação" Tipo: "N" Formula:  ""</summary>
        public int ValOrdertype
        {
            get { return (int)returnValueField(FldOrdertype); }
            set { insertNameValueField(FldOrdertype, value); }
        }


        /// <summary>Campo : "Criação: Data" Tipo: "OD" Formula:  ""</summary>
        public static FieldRef FldData { get { return m_fldData; } }
        private static FieldRef m_fldData = new FieldRef("lstusr", "data");

        /// <summary>Campo : "Criação: Data" Tipo: "OD" Formula:  ""</summary>
        public DateTime ValData
        {
            get { return (DateTime)returnValueField(FldData); }
            set { insertNameValueField(FldData, value); }
        }

        /// <summary>Campo : "ZZSTATE" Tipo: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("lstusr", "zzstate");

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
		public static CSGenioAlstusr search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //para proteger chamadas "cegas"
                return null;
            CSGenioAlstusr area = new CSGenioAlstusr(user, user.CurrentModule);
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
        public static List<CSGenioAlstusr> searchList(PersistentSupport sp, User User, CriteriaSet where)
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
        public static List<CSGenioAlstusr> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos)
        {
            return sp.searchListWhere<CSGenioAlstusr>(where, User, campos);
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
		public static void searchListAdvancedWhere(PersistentSupport sp, User User, CriteriaSet where, ListingMVC<CSGenioAlstusr> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAlstusr>(where, listing);
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
        public static List<CSGenioAlstusr> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAlstusr>(where, User, campos, distinct, noLock);
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
		public static List<CSGenioAlstusr> searchList(PersistentSupport sp, User User, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, User, where, null, distinct, noLock);
        }

	}
}
