
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Linq;

namespace CSGenio.business
{
	/// <summary>
	/// User Authorization
	/// </summary>
	public class CSGenioAs_ua : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAs_ua(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR S_UA]/
		}

		public CSGenioAs_ua(User user) : this(user, user.CurrentModule)
		{
		}

		/// <summary>
		/// Initializes the metadata relative to the fields of this area
		/// </summary>
		private static void InicializaCampos(AreaInfo info)
		{
			Field Qfield = null;
#pragma warning disable CS0168, S1481 // Variable is declared but never used
			List<ByAreaArguments> argumentsListByArea;
#pragma warning restore CS0168, S1481 // Variable is declared but never used
			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codua", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codpsw", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "sistema", FieldType.TEXT);
			Qfield.FieldDescription = "System";
			Qfield.FieldSize =  20;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SYSTEM02957";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "modulo", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Module";
			Qfield.FieldSize =  3;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "MODULE42049";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCs_module";
            Qfield.ArrayClassName = "S_module";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "naodupli", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  39;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"codpsw","modulo"}, new int[] {0,1}, "s_ua", "codua"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 2, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return GenFunctions.KeyToString(((string)args[0]))+((string)args[1]);
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "role", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Role";
			Qfield.FieldSize =  16;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "ROLE60946";

			Qfield.Dupmsg = "";
            Qfield.NotDup = true;
            Qfield.PrefNDup = "naodupli";
            Qfield.ArrayName = "dbo.GetValArrayCs_roles";
            Qfield.ArrayClassName = "S_roles";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "nivel", FieldType.NUMERIC);
			Qfield.FieldDescription = "Level";
			Qfield.FieldSize =  15;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 15;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "LEVEL06184";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"nivel","role"}, new int[] {0,1}, "s_ua", "codua"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 2, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return GlobalFunctions.GetLevelFromRole(((decimal)args[0]),((string)args[1]));
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "opercria", FieldType.TEXT);
			Qfield.FieldDescription = "Created by";
			Qfield.FieldSize =  128;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CREATED_BY12292";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS);
			Qfield.FieldDescription = "Created on";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CREATED_ON00051";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "opermuda", FieldType.TEXT);
			Qfield.FieldDescription = "Changed by";
			Qfield.FieldSize =  128;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CHANGED_BY08967";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "datamuda", FieldType.DATETIMESECONDS);
			Qfield.FieldDescription = "Changed on";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CHANGED_ON19727";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "zzstate", FieldType.INTEGER);
			Qfield.FieldDescription = "Estado da ficha";
			info.RegisterFieldDB(Qfield);

		}

		/// <summary>
		/// Initializes metadata for paths direct to other areas
		/// </summary>
		private static void InicializaRelacoes(AreaInfo info)
		{
			// Daughters Relations
			//------------------------------

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("psw", new Relation("FOR", "userauthorization", "s_ua", "codua", "codpsw", "FOR", "userlogin", "psw", "codpsw", "codpsw"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(1);
			info.Pathways.Add("psw","psw");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------



			info.InternalOperationFields = new string[] {
			 "naodupli","nivel"
			};






			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAs_ua()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="userauthorization";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codua";
			info.HumanKeyName="sistema,".TrimEnd(',');
			info.Alias="s_ua";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="User Authorization";
			info.AreaPluralDesignation="User Authorization";
			info.DescriptionCav="USER_AUTHORIZATION53599";

			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(0);
			info.SyncCompleteHour = TimeSpan.FromHours(0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(0);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
            info.SolrList = new List<string>();
        	info.QueuesList = new List<GenioServer.business.QueueGenio>();





			//RS 22.03.2011 I separated in submetodos due to performance problems with the JIT in 64bits
			// that in very large projects took 2 minutes on the first call.
			// After a Microsoft analysis of the JIT algortimo it was revealed that it has a
			// complexity O(n*m) where n are the lines of code and m the number of variables of a function.
			// Tests have revealed that splitting into subfunctions cuts the JIT time by more than half by 64-bit.
			//------------------------------
			InicializaCampos(info);

			//------------------------------
			InicializaRelacoes(info);

			//------------------------------
			InicializaCaminhos(info);

			//------------------------------
			InicializaFormulas(info);

			// Automatic audit stamps in BD
            //------------------------------
			info.StampFieldsIns = new string[] {
                "opercria","datacria"
			};

			info.StampFieldsAlt = new string[] {
                "opermuda","datamuda"
			};
            // Documents in DB
            //------------------------------

            // Historics
            //------------------------------

			// Duplication
			//------------------------------

			// Ephs
			//------------------------------
			info.Ephs=new Hashtable();

			// Table minimum roles and access levels
			//------------------------------
            info.QLevel = new QLevel();
            info.QLevel.Query = Role.AUTHORIZED;
            info.QLevel.Create = Role.AUTHORIZED;
            info.QLevel.AlterAlways = Role.AUTHORIZED;
            info.QLevel.RemoveAlways = Role.AUTHORIZED;

      		return info;
		}

		/// <summary>
		/// Meta-information about this area
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-information about this area
		/// </summary>
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public static FieldRef FldCodua { get { return m_fldCodua; } }
		private static FieldRef m_fldCodua = new FieldRef("s_ua", "codua");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodua
		{
			get { return (string)returnValueField(FldCodua); }
			set { insertNameValueField(FldCodua, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodpsw { get { return m_fldCodpsw; } }
		private static FieldRef m_fldCodpsw = new FieldRef("s_ua", "codpsw");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodpsw
		{
			get { return (string)returnValueField(FldCodpsw); }
			set { insertNameValueField(FldCodpsw, value); }
		}

		/// <summary>Field : "System" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldSistema { get { return m_fldSistema; } }
		private static FieldRef m_fldSistema = new FieldRef("s_ua", "sistema");

		/// <summary>Field : "System" Tipo: "C" Formula:  ""</summary>
		public string ValSistema
		{
			get { return (string)returnValueField(FldSistema); }
			set { insertNameValueField(FldSistema, value); }
		}

		/// <summary>Field : "Module" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldModulo { get { return m_fldModulo; } }
		private static FieldRef m_fldModulo = new FieldRef("s_ua", "modulo");

		/// <summary>Field : "Module" Tipo: "AC" Formula:  ""</summary>
		public string ValModulo
		{
			get { return (string)returnValueField(FldModulo); }
			set { insertNameValueField(FldModulo, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula: + "KeyToString([S_UA->CODPSW]) + [S_UA->MODULO]"</summary>
		public static FieldRef FldNaodupli { get { return m_fldNaodupli; } }
		private static FieldRef m_fldNaodupli = new FieldRef("s_ua", "naodupli");

		/// <summary>Field : "" Tipo: "C" Formula: + "KeyToString([S_UA->CODPSW]) + [S_UA->MODULO]"</summary>
		public string ValNaodupli
		{
			get { return (string)returnValueField(FldNaodupli); }
			set { insertNameValueField(FldNaodupli, value); }
		}

		/// <summary>Field : "Role" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldRole { get { return m_fldRole; } }
		private static FieldRef m_fldRole = new FieldRef("s_ua", "role");

		/// <summary>Field : "Role" Tipo: "AC" Formula:  ""</summary>
		public string ValRole
		{
			get { return (string)returnValueField(FldRole); }
			set { insertNameValueField(FldRole, value); }
		}

		/// <summary>Field : "Level" Tipo: "N" Formula: + "GetLevelFromRole([S_UA->NIVEL], [S_UA->ROLE])"</summary>
		public static FieldRef FldNivel { get { return m_fldNivel; } }
		private static FieldRef m_fldNivel = new FieldRef("s_ua", "nivel");

		/// <summary>Field : "Level" Tipo: "N" Formula: + "GetLevelFromRole([S_UA->NIVEL], [S_UA->ROLE])"</summary>
		public decimal ValNivel
		{
			get { return (decimal)returnValueField(FldNivel); }
			set { insertNameValueField(FldNivel, value); }
		}

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("s_ua", "opercria");

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public static FieldRef FldDatacria { get { return m_fldDatacria; } }
		private static FieldRef m_fldDatacria = new FieldRef("s_ua", "datacria");

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public DateTime ValDatacria
		{
			get { return (DateTime)returnValueField(FldDatacria); }
			set { insertNameValueField(FldDatacria, value); }
		}

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
		private static FieldRef m_fldOpermuda = new FieldRef("s_ua", "opermuda");

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public string ValOpermuda
		{
			get { return (string)returnValueField(FldOpermuda); }
			set { insertNameValueField(FldOpermuda, value); }
		}

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
		private static FieldRef m_fldDatamuda = new FieldRef("s_ua", "datamuda");

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public DateTime ValDatamuda
		{
			get { return (DateTime)returnValueField(FldDatamuda); }
			set { insertNameValueField(FldDatamuda, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("s_ua", "zzstate");



		/// <summary>Field : "ZZSTATE" Type: "INT"</summary>
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
		/// <param name="forUpdate">True if you are preparing to update this record, false otherwise</param>
        /// <returns>An area with the fields requests of the record read or null if the key does not exist</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static CSGenioAs_ua search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAs_ua area = new CSGenioAs_ua(user, user.CurrentModule);

            if (sp.getRecord(area, key, fields, forUpdate))
                return area;
			return null;
        }


		public static string GetkeyFromControlledRecord(PersistentSupport sp, string ID, User user)
		{
			if (informacao.ControlledRecords != null)
				return informacao.ControlledRecords.GetPrimaryKeyFromControlledRecord(sp, user, ID);
			return String.Empty;
		}


        /// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="fields">The fields to be filled in the area</param>
        /// <param name="distinct">Get distinct from fields</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static List<CSGenioAs_ua> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAs_ua>(where, user, fields, distinct, noLock);
        }



       	/// <summary>
        /// Search for all records of this area that comply with a condition
        /// </summary>
        /// <param name="sp">Persistent support from where to get the list</param>
        /// <param name="user">The context of the user</param>
        /// <param name="where">The search condition for the records. Use null to get all records</param>
        /// <param name="listing">List configuration</param>
        /// <returns>A list of area records with all fields populated</returns>
        /// <remarks>Persistence operations should not be used on a partially positioned register</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAs_ua> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAs_ua>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX S_UA]/

 
            

	}
}
