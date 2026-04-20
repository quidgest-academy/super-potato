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
	public class CSGenioAtblcfg : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAtblcfg(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAtblcfg(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "fortblcfg";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codtblcfg";
            info.HumanKeyName = "name";
			info.ShadowTabKeyName = "";
			info.Alias = "tblcfg";
			info.IsDomain =  false;
			info.AreaDesignation = "User table configuration";
			info.AreaPluralDesignation = "User table configurations";
			info.DescriptionCav = "User table configuration";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
            info.RegisterFieldDB(new Field(info.Alias, "codtblcfg", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "codpsw", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "uuid", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "name", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "config", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "usrsetv", FieldType.INTEGER));
            info.RegisterFieldDB(new Field(info.Alias, "date", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "isdefault", FieldType.LOGIC));
            info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

            // Carimbos automáticos na BD
            //------------------------------
            info.StampFieldsIns = ["date"];

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

		// USE /[MANUAL FOR TABAUX tblcfg]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        /// <summary>Campo: "PK da tabela tblcfg" Tipo: "+"</summary>
        public static FieldRef FldCodtblcfg => m_fldCodtblcfg;
        private static readonly FieldRef m_fldCodtblcfg = new("tblcfg", "codtblcfg");

        /// <summary>Campo: "PK da tabela tblcfg" Tipo: "+"</summary>
        public string ValCodtblcfg
        {
            get { return (string)returnValueField(FldCodtblcfg); }
            set { insertNameValueField(FldCodtblcfg, value); }
        }

        /// <summary>Campo: "PK da tabela psw" Tipo: "+"</summary>
        public static FieldRef FldCodpsw => m_fldCodpsw;
        private static readonly FieldRef m_fldCodpsw = new("tblcfg", "codpsw");

        /// <summary>Campo: "PK da tabela psw" Tipo: "+"</summary>
        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        /// <summary>Campo: "uuid" Tipo: "C"</summary>
        public static FieldRef FldUuid => m_fldUuid;
        private static readonly FieldRef m_fldUuid = new("tblcfg", "uuid");

        /// <summary>Campo: "uuid" Tipo: "C"</summary>
        public string ValUuid
        {
            get { return (string)returnValueField(FldUuid); }
            set { insertNameValueField(FldUuid, value); }
        }

        /// <summary>Campo: "Nome da lista" Tipo: "C"</summary>
        public static FieldRef FldName => m_fldName;
        private static readonly FieldRef m_fldName = new("tblcfg", "name");

        /// <summary>Campo: "Nome da lista" Tipo: "C"</summary>
        public string ValName
        {
            get { return (string)returnValueField(FldName); }
            set { insertNameValueField(FldName, value); }
        }

        /// <summary>Campo: "Config" Tipo: "C"</summary>
        public static FieldRef FldConfig => m_fldConfig;
        private static readonly FieldRef m_fldConfig = new("tblcfg", "config");

        /// <summary>Campo: "Config" Tipo: "C"</summary>
        public string ValConfig
        {
            get { return (string)returnValueField(FldConfig); }
            set { insertNameValueField(FldConfig, value); }
        }

        /// <summary>Campo: "User settings version" Tipo: "N"</summary>
        public static FieldRef FldUsrsetv => m_fldUsrsetv;
        private static readonly FieldRef m_fldUsrsetv = new("tblcfg", "usrsetv");

        /// <summary>Campo: "User settings version" Tipo: "N"</summary>
        public int ValUsrsetv
        {
            get { return (int)returnValueField(FldUsrsetv); }
            set { insertNameValueField(FldUsrsetv, value); }
        }

        /// <summary>Campo: "Criação: Date" Tipo: "OD"</summary>
        public static FieldRef FldDate => m_fldDate;
        private static readonly FieldRef m_fldDate = new("tblcfg", "date");

        /// <summary>Campo: "Criação: Date" Tipo: "OD"</summary>
        public DateTime ValDate
        {
            get { return (DateTime)returnValueField(FldDate); }
            set { insertNameValueField(FldDate, value); }
        }

        /// <summary>Campo: "ISDEFAULT" Tipo: "L"</summary>
        public static FieldRef FldIsdefault => m_fldIsdefault;
        private static readonly FieldRef m_fldIsdefault = new("tblcfg", "isdefault");

        /// <summary>Campo: "ISDEFAULT" Tipo: "L"</summary>
        public int ValIsdefault
        {
            get { return (int)returnValueField(FldIsdefault); }
            set { insertNameValueField(FldIsdefault, value); }
        }

        /// <summary>Campo: "ZZSTATE" Tipo: "N"</summary>
        public static FieldRef FldZzstate => m_fldZzstate;
        private static readonly FieldRef m_fldZzstate = new("tblcfg", "zzstate");

        /// <summary>Campo: "ZZSTATE" Tipo: "N"</summary>
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
        public static CSGenioAtblcfg search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //para proteger chamadas "cegas"
                return null;
            CSGenioAtblcfg area = new(user, user.CurrentModule);
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
        public static List<CSGenioAtblcfg> searchList(PersistentSupport sp, User User, CriteriaSet where)
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
        public static List<CSGenioAtblcfg> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos)
        {
            return sp.searchListWhere<CSGenioAtblcfg>(where, User, campos);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User User, CriteriaSet where, ListingMVC<CSGenioAtblcfg> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAtblcfg>(where, listing);
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
        public static List<CSGenioAtblcfg> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAtblcfg>(where, User, campos, distinct, noLock);
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
        public static List<CSGenioAtblcfg> searchList(PersistentSupport sp, User User, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, User, where, null, distinct, noLock);
        }

	}
}
