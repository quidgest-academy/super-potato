
 
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
	/// Agent
	/// </summary>
	public class CSGenioAagent : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAagent(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR AGENT]/
		}

		public CSGenioAagent(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codagent", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "photography", FieldType.IMAGE);
			Qfield.FieldDescription = "Photography";
			Qfield.FieldSize =  3;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "name", FieldType.TEXT);
			Qfield.FieldDescription = "Agent's name";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "AGENT_S_NAME42642";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "birthdat", FieldType.DATE);
			Qfield.FieldDescription = "Birthdate";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "BIRTHDATE22743";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "email", FieldType.TEXT);
			Qfield.FieldDescription = "E-mail";
			Qfield.FieldSize =  80;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "E_MAIL42251";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
            Qfield.NotDup = true;
			Qfield.DefaultValue = new DefaultValue("@agency.com");
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "telephon", FieldType.TEXT);
			Qfield.FieldDescription = "Telephone";
			Qfield.FieldSize =  14;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "TELEPHONE28697";

			Qfield.Dupmsg = "";
			Qfield.FillingRule = (rule) =>
			{
				string mask = "+000 000000000";
				string validation = "+000 000000000";
				return Validation.validateMP(rule, mask, validation);
			};
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "cborn", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codcaddr", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "nrprops", FieldType.NUMERIC);
			Qfield.FieldDescription = "Number of properties";
			Qfield.FieldSize =  5;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 5;
			Qfield.CavDesignation = "NUMBER_OF_PROPERTIES01169";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "profit", FieldType.CURRENCY);
			Qfield.FieldDescription = "Profit";
			Qfield.FieldSize =  14;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 11;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "PROFIT55910";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "lastprop", FieldType.CURRENCY);
			Qfield.FieldDescription = "Last property sold (price)";
			Qfield.FieldSize =  12;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 9;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "LAST_PROPERTY_SOLD61882";

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
			info.ChildTable = new ChildRelation[1];
			info.ChildTable[0]= new ChildRelation("prope", new String[] {"codagent"}, DeleteProc.NA);

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("caddr", new Relation("FOR", "foragent", "agent", "codagent", "codcaddr", "FOR", "forcount", "caddr", "codcount", "codcount"));
			info.ParentTables.Add("cborn", new Relation("FOR", "foragent", "agent", "codagent", "cborn", "FOR", "forcount", "cborn", "codcount", "codcount"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(2);
			info.Pathways.Add("caddr","caddr");
			info.Pathways.Add("cborn","cborn");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------



			info.DefaultValues = new string[] {
			 "email"
			};


			info.RelatedSumFields = new string[] {
			 "nrprops","profit"
			};


			info.LastValueFields = new string[] {
			 "lastprop"
			};



			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAagent()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="foragent";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codagent";
			info.HumanKeyName="name,email,".TrimEnd(',');
			info.Alias="agent";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Agent";
			info.AreaPluralDesignation="Agents";
			info.DescriptionCav="AGENT00994";

			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(8);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23);
			info.SyncCompleteHour = TimeSpan.FromHours(0.5);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
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
		public static FieldRef FldCodagent { get { return m_fldCodagent; } }
		private static FieldRef m_fldCodagent = new FieldRef("agent", "codagent");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodagent
		{
			get { return (string)returnValueField(FldCodagent); }
			set { insertNameValueField(FldCodagent, value); }
		}

		/// <summary>Field : "Photography" Tipo: "IJ" Formula:  ""</summary>
		public static FieldRef FldPhotography { get { return m_fldPhotography; } }
		private static FieldRef m_fldPhotography = new FieldRef("agent", "photography");

		/// <summary>Field : "Photography" Tipo: "IJ" Formula:  ""</summary>
		public byte[] ValPhotography
		{
			get { return (byte[])returnValueField(FldPhotography); }
			set { insertNameValueField(FldPhotography, value); }
		}

		/// <summary>Field : "Agent's name" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldName { get { return m_fldName; } }
		private static FieldRef m_fldName = new FieldRef("agent", "name");

		/// <summary>Field : "Agent's name" Tipo: "C" Formula:  ""</summary>
		public string ValName
		{
			get { return (string)returnValueField(FldName); }
			set { insertNameValueField(FldName, value); }
		}

		/// <summary>Field : "Birthdate" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldBirthdat { get { return m_fldBirthdat; } }
		private static FieldRef m_fldBirthdat = new FieldRef("agent", "birthdat");

		/// <summary>Field : "Birthdate" Tipo: "D" Formula:  ""</summary>
		public DateTime ValBirthdat
		{
			get { return (DateTime)returnValueField(FldBirthdat); }
			set { insertNameValueField(FldBirthdat, value); }
		}

		/// <summary>Field : "E-mail" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldEmail { get { return m_fldEmail; } }
		private static FieldRef m_fldEmail = new FieldRef("agent", "email");

		/// <summary>Field : "E-mail" Tipo: "C" Formula:  ""</summary>
		public string ValEmail
		{
			get { return (string)returnValueField(FldEmail); }
			set { insertNameValueField(FldEmail, value); }
		}

		/// <summary>Field : "Telephone" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldTelephon { get { return m_fldTelephon; } }
		private static FieldRef m_fldTelephon = new FieldRef("agent", "telephon");

		/// <summary>Field : "Telephone" Tipo: "C" Formula:  ""</summary>
		public string ValTelephon
		{
			get { return (string)returnValueField(FldTelephon); }
			set { insertNameValueField(FldTelephon, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCborn { get { return m_fldCborn; } }
		private static FieldRef m_fldCborn = new FieldRef("agent", "cborn");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCborn
		{
			get { return (string)returnValueField(FldCborn); }
			set { insertNameValueField(FldCborn, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodcaddr { get { return m_fldCodcaddr; } }
		private static FieldRef m_fldCodcaddr = new FieldRef("agent", "codcaddr");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodcaddr
		{
			get { return (string)returnValueField(FldCodcaddr); }
			set { insertNameValueField(FldCodcaddr, value); }
		}

		/// <summary>Field : "Number of properties" Tipo: "N" Formula: SR "[PROPE->1]"</summary>
		public static FieldRef FldNrprops { get { return m_fldNrprops; } }
		private static FieldRef m_fldNrprops = new FieldRef("agent", "nrprops");

		/// <summary>Field : "Number of properties" Tipo: "N" Formula: SR "[PROPE->1]"</summary>
		public decimal ValNrprops
		{
			get { return (decimal)returnValueField(FldNrprops); }
			set { insertNameValueField(FldNrprops, value); }
		}

		/// <summary>Field : "Profit" Tipo: "$" Formula: SR "[PROPE->PROFIT]"</summary>
		public static FieldRef FldProfit { get { return m_fldProfit; } }
		private static FieldRef m_fldProfit = new FieldRef("agent", "profit");

		/// <summary>Field : "Profit" Tipo: "$" Formula: SR "[PROPE->PROFIT]"</summary>
		public decimal ValProfit
		{
			get { return (decimal)returnValueField(FldProfit); }
			set { insertNameValueField(FldProfit, value); }
		}

		/// <summary>Field : "Last property sold (price)" Tipo: "$" Formula: U1 "PROPE[PROPE->DTSOLD][PROPE->PRICE]"</summary>
		public static FieldRef FldLastprop { get { return m_fldLastprop; } }
		private static FieldRef m_fldLastprop = new FieldRef("agent", "lastprop");

		/// <summary>Field : "Last property sold (price)" Tipo: "$" Formula: U1 "PROPE[PROPE->DTSOLD][PROPE->PRICE]"</summary>
		public decimal ValLastprop
		{
			get { return (decimal)returnValueField(FldLastprop); }
			set { insertNameValueField(FldLastprop, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("agent", "zzstate");



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
        public static CSGenioAagent search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAagent area = new CSGenioAagent(user, user.CurrentModule);

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
        public static List<CSGenioAagent> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAagent>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAagent> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAagent>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX AGENT]/

 
            

	}
}
