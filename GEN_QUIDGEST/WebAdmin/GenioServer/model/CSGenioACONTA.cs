
 
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
	/// Contact
	/// </summary>
	public class CSGenioAconta : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAconta(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR CONTA]/
		}

		public CSGenioAconta(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codconta", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "date", FieldType.DATE);
			Qfield.FieldDescription = "Date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "DATE18475";

			Qfield.Dupmsg = "";
			Qfield.DefaultValue = new DefaultValue(DefaultValue.getToday);
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codprope", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "client", FieldType.TEXT);
			Qfield.FieldDescription = "Client name";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "CLIENT_NAME39245";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "email", FieldType.TEXT);
			Qfield.FieldDescription = "Email do cliente";
			Qfield.FieldSize =  80;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "EMAIL_DO_CLIENTE30111";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "phone", FieldType.TEXT);
			Qfield.FieldDescription = "Phone number";
			Qfield.FieldSize =  14;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "PHONE_NUMBER20774";

			Qfield.Dupmsg = "";
			Qfield.FillingRule = (rule) =>
			{
				string mask = "+000 000000000";
				string validation = "+000 000000000";
				return Validation.validateMP(rule, mask, validation);
			};
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "descript", FieldType.MEMO);
			Qfield.FieldDescription = "Description";
			Qfield.FieldSize =  80;
			Qfield.MQueue = false;
			Qfield.Decimals = 5;
			Qfield.CavDesignation = "DESCRIPTION07383";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"phone"}, new int[] {0}, "conta", "codconta"));
			Qfield.BlockWhen = new ConditionFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return (((string)args[0]) == "");
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "pricepro", FieldType.CURRENCY);
			Qfield.FieldDescription = "Preço";
			Qfield.FieldSize =  12;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 7;
			Qfield.Decimals = 4;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			Qfield.Formula = new ReplicaFormula("_replicRel_codprope", "price");
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "visit_date", FieldType.DATE);
			Qfield.FieldDescription = "Visit Date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "VISIT_DATE27188";

            Qfield.NotNull = true;
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
			info.ParentTables.Add("prope", new Relation("FOR", "forcontact", "conta", "codconta", "codprope", "FOR", "forproperty", "prope", "codprope", "codprope"));
			info.ParentTables.Add("_replicRel_codprope", new Relation("FOR", "forcontact", "conta", "codconta", "codprope", "FOR", "forproperty", "prope", "codprope", "codprope"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(6);
			info.Pathways.Add("prope","prope");
			info.Pathways.Add("city","prope");
			info.Pathways.Add("agent","prope");
			info.Pathways.Add("count","prope");
			info.Pathways.Add("caddr","prope");
			info.Pathways.Add("cborn","prope");
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------
			//Actualiza as seguintes somas relacionadas:
			info.RelatedSumArgs = new List<RelatedSumArgument>();
			info.RelatedSumArgs.Add( new RelatedSumArgument("conta", "prope", "numbercontacts", "visit_date", '+', true));



			info.ReplicaFields = new string[] {
			 "pricepro"
			};

			info.DefaultValues = new string[] {
			 "date"
			};






			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();

			// isEmptyC([CONTA->CLIENT])
			{
			List<ByAreaArguments> argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea= new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"client"},new int[] {0},"conta","codconta"));
			ConditionFormula writeCondition = new ConditionFormula(argumentsListByArea, 1, delegate(object []args,User user,string module,PersistentSupport sp) {
				return (((string)args[0]) == "");
			});
			writeCondition.ErrorWarning = "";
            writeCondition.Type =  ConditionType.MANDATORY;
            writeCondition.Validate = true;
			writeCondition.Field = info.DBFields["descript"];
			conditions.Add(writeCondition);
			}

			// isEmptyC([CONTA->DESCRIPT])
			{
			List<ByAreaArguments> argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea= new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"descript"},new int[] {0},"conta","codconta"));
			ConditionFormula writeCondition = new ConditionFormula(argumentsListByArea, 1, delegate(object []args,User user,string module,PersistentSupport sp) {
				return (((string)args[0]) == "");
			});
			writeCondition.ErrorWarning = "Por favor diga-nos o motivo do contato!";
            writeCondition.Type =  ConditionType.ERROR;
            writeCondition.Validate = true;
			conditions.Add(writeCondition);
			}
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAconta()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="forcontact";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codconta";
			info.HumanKeyName="client,".TrimEnd(',');
			info.Alias="conta";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Contact";
			info.AreaPluralDesignation="Contacts";
			info.DescriptionCav="CONTACT59247";

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
		public static FieldRef FldCodconta { get { return m_fldCodconta; } }
		private static FieldRef m_fldCodconta = new FieldRef("conta", "codconta");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodconta
		{
			get { return (string)returnValueField(FldCodconta); }
			set { insertNameValueField(FldCodconta, value); }
		}

		/// <summary>Field : "Date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDate { get { return m_fldDate; } }
		private static FieldRef m_fldDate = new FieldRef("conta", "date");

		/// <summary>Field : "Date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDate
		{
			get { return (DateTime)returnValueField(FldDate); }
			set { insertNameValueField(FldDate, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodprope { get { return m_fldCodprope; } }
		private static FieldRef m_fldCodprope = new FieldRef("conta", "codprope");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodprope
		{
			get { return (string)returnValueField(FldCodprope); }
			set { insertNameValueField(FldCodprope, value); }
		}

		/// <summary>Field : "Client name" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldClient { get { return m_fldClient; } }
		private static FieldRef m_fldClient = new FieldRef("conta", "client");

		/// <summary>Field : "Client name" Tipo: "C" Formula:  ""</summary>
		public string ValClient
		{
			get { return (string)returnValueField(FldClient); }
			set { insertNameValueField(FldClient, value); }
		}

		/// <summary>Field : "Email do cliente" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldEmail { get { return m_fldEmail; } }
		private static FieldRef m_fldEmail = new FieldRef("conta", "email");

		/// <summary>Field : "Email do cliente" Tipo: "C" Formula:  ""</summary>
		public string ValEmail
		{
			get { return (string)returnValueField(FldEmail); }
			set { insertNameValueField(FldEmail, value); }
		}

		/// <summary>Field : "Phone number" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldPhone { get { return m_fldPhone; } }
		private static FieldRef m_fldPhone = new FieldRef("conta", "phone");

		/// <summary>Field : "Phone number" Tipo: "C" Formula:  ""</summary>
		public string ValPhone
		{
			get { return (string)returnValueField(FldPhone); }
			set { insertNameValueField(FldPhone, value); }
		}

		/// <summary>Field : "Description" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldDescript { get { return m_fldDescript; } }
		private static FieldRef m_fldDescript = new FieldRef("conta", "descript");

		/// <summary>Field : "Description" Tipo: "MO" Formula:  ""</summary>
		public string ValDescript
		{
			get { return (string)returnValueField(FldDescript); }
			set { insertNameValueField(FldDescript, value); }
		}

		/// <summary>Field : "Preço" Tipo: "$" Formula: ++ "[PROPE->PRICE]"</summary>
		public static FieldRef FldPricepro { get { return m_fldPricepro; } }
		private static FieldRef m_fldPricepro = new FieldRef("conta", "pricepro");

		/// <summary>Field : "Preço" Tipo: "$" Formula: ++ "[PROPE->PRICE]"</summary>
		public decimal ValPricepro
		{
			get { return (decimal)returnValueField(FldPricepro); }
			set { insertNameValueField(FldPricepro, value); }
		}

		/// <summary>Field : "Visit Date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldVisit_date { get { return m_fldVisit_date; } }
		private static FieldRef m_fldVisit_date = new FieldRef("conta", "visit_date");

		/// <summary>Field : "Visit Date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValVisit_date
		{
			get { return (DateTime)returnValueField(FldVisit_date); }
			set { insertNameValueField(FldVisit_date, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("conta", "zzstate");



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
        public static CSGenioAconta search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAconta area = new CSGenioAconta(user, user.CurrentModule);

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
        public static List<CSGenioAconta> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAconta>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAconta> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAconta>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX CONTA]/

 
          

	}
}
