
 
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
	/// Async process attachment
	/// </summary>
	public class CSGenioAs_pax : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAs_pax(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR S_PAX]/
		}

		public CSGenioAs_pax(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codpranx", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "cods_apr", FieldType.KEY_GUID);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  36;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "document", FieldType.DOCUMENT);
			Qfield.FieldDescription = "Document";
			Qfield.FieldSize =  200;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "DOCUMENT00695";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);
			Qfield = new Field(info.Alias, "documentfk", FieldType.KEY_INT);
			Qfield.FieldSize = 8;
			Qfield.FieldDescription = "Chave estrangeira para o documento";
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
			info.ParentTables.Add("s_apr", new Relation("FOR", "asyncprocessattachments", "s_pax", "codpranx", "cods_apr", "FOR", "asyncprocess", "s_apr", "codascpr", "codascpr"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(1);
			info.Pathways.Add("s_apr","s_apr");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------








			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAs_pax()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="asyncprocessattachments";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codpranx";
			info.HumanKeyName="document,".TrimEnd(',');
			info.Alias="s_pax";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Async process attachment";
			info.AreaPluralDesignation="Async process attachments";
			info.DescriptionCav="ASYNC_PROCESS_ATTACH34417";

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

            // Documents in DB
            //------------------------------
			info.DocumsForeignKeys = new List<String> {
			 "documentfk"
			};
			info.HasVersionManagment = true; //a true por omissão, quando o Qfield no genio tiver criado preencher por esse Qvalue

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
		public static FieldRef FldCodpranx { get { return m_fldCodpranx; } }
		private static FieldRef m_fldCodpranx = new FieldRef("s_pax", "codpranx");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodpranx
		{
			get { return (string)returnValueField(FldCodpranx); }
			set { insertNameValueField(FldCodpranx, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCods_apr { get { return m_fldCods_apr; } }
		private static FieldRef m_fldCods_apr = new FieldRef("s_pax", "cods_apr");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCods_apr
		{
			get { return (string)returnValueField(FldCods_apr); }
			set { insertNameValueField(FldCods_apr, value); }
		}

		/// <summary>Field : "Document" Tipo: "IB" Formula:  ""</summary>
		public static FieldRef FldDocument { get { return m_fldDocument; } }
		private static FieldRef m_fldDocument = new FieldRef("s_pax", "document");

		/// <summary>Field : "Document" Tipo: "IB" Formula:  ""</summary>
		public string ValDocument
		{
			get { return (string)returnValueField(FldDocument); }
			set { insertNameValueField(FldDocument, value); }
		}

		/// <summary>Field : "Document FK" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldDocumentfk { get { return m_fldDocumentfk; } }
		private static FieldRef m_fldDocumentfk = new FieldRef("s_pax", "documentfk");

		/// <summary>Field : "Document FK" Tipo: "CE" Formula:  ""</summary>
		public string ValDocumentfk
		{
			get { return (string)returnValueField(FldDocumentfk); }
			set { insertNameValueField(FldDocumentfk, value); }
		}

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("s_pax", "opercria");

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public static FieldRef FldDatacria { get { return m_fldDatacria; } }
		private static FieldRef m_fldDatacria = new FieldRef("s_pax", "datacria");

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public DateTime ValDatacria
		{
			get { return (DateTime)returnValueField(FldDatacria); }
			set { insertNameValueField(FldDatacria, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("s_pax", "zzstate");



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
        public static CSGenioAs_pax search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAs_pax area = new CSGenioAs_pax(user, user.CurrentModule);

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
        public static List<CSGenioAs_pax> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAs_pax>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAs_pax> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAs_pax>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX S_PAX]/

 
      

	}
}
