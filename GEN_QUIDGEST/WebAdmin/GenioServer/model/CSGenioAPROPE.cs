
 
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
	/// Property
	/// </summary>
	public class CSGenioAprope : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAprope(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR PROPE]/
		}

		public CSGenioAprope(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codprope", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "photo", FieldType.IMAGE);
			Qfield.FieldDescription = "Main photo";
			Qfield.FieldSize =  3;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "title", FieldType.TEXT);
			Qfield.FieldDescription = "Title";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "TITLE21885";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "price", FieldType.CURRENCY);
			Qfield.FieldDescription = "Price";
			Qfield.FieldSize =  12;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 9;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "PRICE06900";

            Qfield.NotNull = true;
			Qfield.Dupmsg = "";
//Actualiza as seguintes réplicas:
			Qfield.ReplicaDestinationList = new List<ReplicaDestination>();
			Qfield.ReplicaDestinationList.Add( new ReplicaDestination("FOR", "forcontact", "codprope", "pricepro"));
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codagent", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "descript", FieldType.MEMO);
			Qfield.FieldDescription = "Description";
			Qfield.FieldSize =  80;
			Qfield.MQueue = false;
			Qfield.Decimals = 5;
			Qfield.CavDesignation = "DESCRIPTION07383";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codcity", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "size", FieldType.NUMERIC);
			Qfield.FieldDescription = "Size (m2)";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 8;
			Qfield.CavDesignation = "SIZE__M2_57059";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "bathnr", FieldType.NUMERIC);
			Qfield.FieldDescription = "Bathrooms number";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 2;
			Qfield.CavDesignation = "BATHROOMS_NUMBER52698";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "dtconst", FieldType.DATE);
			Qfield.FieldDescription = "Construction date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "DATA_DE_CONTRUCAO03489";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "buildtyp", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Building type";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "BUILDING_TYPE57152";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCbuildtyp";
            Qfield.ArrayClassName = "Buildtyp";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "typology", FieldType.ARRAY_NUMERIC);
			Qfield.FieldDescription = "Building typology";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "BUILDING_TYPOLOGY54011";

			Qfield.Dupmsg = "";
			Qfield.ArrayName = "dbo.GetValArrayNtypology";
            Qfield.ArrayClassName = "Typology";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "id", FieldType.NUMERIC);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  5;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 5;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			Qfield.DefaultValue = new DefaultValue(DefaultValue.getGreaterPlus1_int, "id");
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "buildage", FieldType.NUMERIC);
			Qfield.FieldDescription = "Building age";
			Qfield.FieldSize =  4;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 4;
			Qfield.CavDesignation = "BUILDING_AGE27311";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"dtconst"}, new int[] {0}, "prope", "codprope"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return GenFunctions.Year(DateTime.Today)-GenFunctions.Year(((DateTime)args[0]));
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "grdsize", FieldType.NUMERIC);
			Qfield.FieldDescription = "Ground size";
			Qfield.FieldSize =  9;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 9;
			Qfield.CavDesignation = "GROUND_57164";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"buildtyp"}, new int[] {0}, "prope", "codprope"));
			Qfield.ShowWhen = new ConditionFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((string)args[0])=="house";
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "floornr", FieldType.NUMERIC);
			Qfield.FieldDescription = "Floor";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 2;
			Qfield.CavDesignation = "FLOOR19993";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"buildtyp"}, new int[] {0}, "prope", "codprope"));
			Qfield.ShowWhen = new ConditionFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((string)args[0])=="apartment";
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "sold", FieldType.LOGIC);
			Qfield.FieldDescription = "Sold";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "SOLD59824";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "profit", FieldType.CURRENCY);
			Qfield.FieldDescription = "Profit";
			Qfield.FieldSize =  12;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 9;
			Qfield.Decimals = 2;
			Qfield.CavDesignation = "PROFIT55910";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"sold","price"}, new int[] {0,1}, "prope", "codprope"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 2, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((((int)args[0])==1)?(((decimal)args[1])):(0));
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "dtsold", FieldType.DATE);
			Qfield.FieldDescription = "Sold date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.CavDesignation = "SOLD_DATE37976";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"sold"}, new int[] {0}, "prope", "codprope"));
			Qfield.FillWhen = new ConditionFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((int)args[0])==1;
			});
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"sold"}, new int[] {0}, "prope", "codprope"));
			Qfield.ShowWhen = new ConditionFormula(argumentsListByArea, 1, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((int)args[0])==1;
			});
			Qfield.DefaultValue = new DefaultValue(DefaultValue.getToday);
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "average", FieldType.NUMERIC);
			Qfield.FieldDescription = "AveragePrice";
			Qfield.FieldSize =  12;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 12;
			Qfield.CavDesignation = "AVERAGEPRICE13700";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 0, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return new GlobalFunctions(user,module,sp).Average();
			});
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
			info.ChildTable = new ChildRelation[2];
			info.ChildTable[0]= new ChildRelation("conta", new String[] {"codprope"}, DeleteProc.NA);
			info.ChildTable[1]= new ChildRelation("photo", new String[] {"codprope"}, DeleteProc.NA);

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
			info.ParentTables.Add("agent", new Relation("FOR", "forproperty", "prope", "codprope", "codagent", "FOR", "foragent", "agent", "codagent", "codagent"));
			info.ParentTables.Add("city", new Relation("FOR", "forproperty", "prope", "codprope", "codcity", "FOR", "forcity", "city", "codcity", "codcity"));
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(5);
			info.Pathways.Add("city","city");
			info.Pathways.Add("agent","agent");
			info.Pathways.Add("count","city");
			info.Pathways.Add("caddr","agent");
			info.Pathways.Add("cborn","agent");
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
			info.RelatedSumArgs.Add( new RelatedSumArgument("prope", "agent", "nrprops", "1", '+', false));
			info.RelatedSumArgs.Add( new RelatedSumArgument("prope", "agent", "profit", "profit", '+', true));



			//Actualiza as seguintes rotinas de ultimo Qvalue:
			info.LastValueArgs = new List<LastValueArgument>();
			info.LastValueArgs.Add( new LastValueArgument("agent",
				new string [] {"lastprop"},
				new string [] {"price"},
				"dtsold",
				null,

				null, false));



			info.InternalOperationFields = new string[] {
			 "buildage","profit","average"
			};

			info.DefaultValues = new string[] {
			 "dtsold"
			};

			info.SequentialDefaultValues = new string[] {
			 "id"
			};




			info.FieldsParametersReplicas = new string[] {
			 "price"
			};

			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();

			// [PROPE->PRICE]>0
			{
			List<ByAreaArguments> argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea= new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"price"},new int[] {0},"prope","codprope"));
			ConditionFormula writeCondition = new ConditionFormula(argumentsListByArea, 1, delegate(object []args,User user,string module,PersistentSupport sp) {
				return ((decimal)args[0])>0;
			});
			writeCondition.ErrorWarning = "You are attempting to save a property without any price!";
            writeCondition.Type =  ConditionType.ERROR;
            writeCondition.Validate = true;
			conditions.Add(writeCondition);
			}
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAprope()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="forproperty";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codprope";
			info.HumanKeyName="title,price,".TrimEnd(',');
			info.Alias="prope";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Property";
			info.AreaPluralDesignation="Properties";
			info.DescriptionCav="PROPERTY43977";

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
		public static FieldRef FldCodprope { get { return m_fldCodprope; } }
		private static FieldRef m_fldCodprope = new FieldRef("prope", "codprope");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodprope
		{
			get { return (string)returnValueField(FldCodprope); }
			set { insertNameValueField(FldCodprope, value); }
		}

		/// <summary>Field : "Main photo" Tipo: "IJ" Formula:  ""</summary>
		public static FieldRef FldPhoto { get { return m_fldPhoto; } }
		private static FieldRef m_fldPhoto = new FieldRef("prope", "photo");

		/// <summary>Field : "Main photo" Tipo: "IJ" Formula:  ""</summary>
		public byte[] ValPhoto
		{
			get { return (byte[])returnValueField(FldPhoto); }
			set { insertNameValueField(FldPhoto, value); }
		}

		/// <summary>Field : "Title" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldTitle { get { return m_fldTitle; } }
		private static FieldRef m_fldTitle = new FieldRef("prope", "title");

		/// <summary>Field : "Title" Tipo: "C" Formula:  ""</summary>
		public string ValTitle
		{
			get { return (string)returnValueField(FldTitle); }
			set { insertNameValueField(FldTitle, value); }
		}

		/// <summary>Field : "Price" Tipo: "$" Formula:  ""</summary>
		public static FieldRef FldPrice { get { return m_fldPrice; } }
		private static FieldRef m_fldPrice = new FieldRef("prope", "price");

		/// <summary>Field : "Price" Tipo: "$" Formula:  ""</summary>
		public decimal ValPrice
		{
			get { return (decimal)returnValueField(FldPrice); }
			set { insertNameValueField(FldPrice, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodagent { get { return m_fldCodagent; } }
		private static FieldRef m_fldCodagent = new FieldRef("prope", "codagent");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodagent
		{
			get { return (string)returnValueField(FldCodagent); }
			set { insertNameValueField(FldCodagent, value); }
		}

		/// <summary>Field : "Description" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldDescript { get { return m_fldDescript; } }
		private static FieldRef m_fldDescript = new FieldRef("prope", "descript");

		/// <summary>Field : "Description" Tipo: "MO" Formula:  ""</summary>
		public string ValDescript
		{
			get { return (string)returnValueField(FldDescript); }
			set { insertNameValueField(FldDescript, value); }
		}

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public static FieldRef FldCodcity { get { return m_fldCodcity; } }
		private static FieldRef m_fldCodcity = new FieldRef("prope", "codcity");

		/// <summary>Field : "" Tipo: "CE" Formula:  ""</summary>
		public string ValCodcity
		{
			get { return (string)returnValueField(FldCodcity); }
			set { insertNameValueField(FldCodcity, value); }
		}

		/// <summary>Field : "Size (m2)" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldSize { get { return m_fldSize; } }
		private static FieldRef m_fldSize = new FieldRef("prope", "size");

		/// <summary>Field : "Size (m2)" Tipo: "N" Formula:  ""</summary>
		public decimal ValSize
		{
			get { return (decimal)returnValueField(FldSize); }
			set { insertNameValueField(FldSize, value); }
		}

		/// <summary>Field : "Bathrooms number" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldBathnr { get { return m_fldBathnr; } }
		private static FieldRef m_fldBathnr = new FieldRef("prope", "bathnr");

		/// <summary>Field : "Bathrooms number" Tipo: "N" Formula:  ""</summary>
		public decimal ValBathnr
		{
			get { return (decimal)returnValueField(FldBathnr); }
			set { insertNameValueField(FldBathnr, value); }
		}

		/// <summary>Field : "Construction date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDtconst { get { return m_fldDtconst; } }
		private static FieldRef m_fldDtconst = new FieldRef("prope", "dtconst");

		/// <summary>Field : "Construction date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDtconst
		{
			get { return (DateTime)returnValueField(FldDtconst); }
			set { insertNameValueField(FldDtconst, value); }
		}

		/// <summary>Field : "Building type" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldBuildtyp { get { return m_fldBuildtyp; } }
		private static FieldRef m_fldBuildtyp = new FieldRef("prope", "buildtyp");

		/// <summary>Field : "Building type" Tipo: "AC" Formula:  ""</summary>
		public string ValBuildtyp
		{
			get { return (string)returnValueField(FldBuildtyp); }
			set { insertNameValueField(FldBuildtyp, value); }
		}

		/// <summary>Field : "Building typology" Tipo: "AN" Formula:  ""</summary>
		public static FieldRef FldTypology { get { return m_fldTypology; } }
		private static FieldRef m_fldTypology = new FieldRef("prope", "typology");

		/// <summary>Field : "Building typology" Tipo: "AN" Formula:  ""</summary>
		public decimal ValTypology
		{
			get { return (decimal)returnValueField(FldTypology); }
			set { insertNameValueField(FldTypology, value); }
		}

		/// <summary>Field : "" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldId { get { return m_fldId; } }
		private static FieldRef m_fldId = new FieldRef("prope", "id");

		/// <summary>Field : "" Tipo: "N" Formula:  ""</summary>
		public decimal ValId
		{
			get { return (decimal)returnValueField(FldId); }
			set { insertNameValueField(FldId, value); }
		}

		/// <summary>Field : "Building age" Tipo: "N" Formula: + "Year([Today])- Year([PROPE->DTCONST])"</summary>
		public static FieldRef FldBuildage { get { return m_fldBuildage; } }
		private static FieldRef m_fldBuildage = new FieldRef("prope", "buildage");

		/// <summary>Field : "Building age" Tipo: "N" Formula: + "Year([Today])- Year([PROPE->DTCONST])"</summary>
		public decimal ValBuildage
		{
			get { return (decimal)returnValueField(FldBuildage); }
			set { insertNameValueField(FldBuildage, value); }
		}

		/// <summary>Field : "Ground size" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldGrdsize { get { return m_fldGrdsize; } }
		private static FieldRef m_fldGrdsize = new FieldRef("prope", "grdsize");

		/// <summary>Field : "Ground size" Tipo: "N" Formula:  ""</summary>
		public decimal ValGrdsize
		{
			get { return (decimal)returnValueField(FldGrdsize); }
			set { insertNameValueField(FldGrdsize, value); }
		}

		/// <summary>Field : "Floor" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldFloornr { get { return m_fldFloornr; } }
		private static FieldRef m_fldFloornr = new FieldRef("prope", "floornr");

		/// <summary>Field : "Floor" Tipo: "N" Formula:  ""</summary>
		public decimal ValFloornr
		{
			get { return (decimal)returnValueField(FldFloornr); }
			set { insertNameValueField(FldFloornr, value); }
		}

		/// <summary>Field : "Sold" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldSold { get { return m_fldSold; } }
		private static FieldRef m_fldSold = new FieldRef("prope", "sold");

		/// <summary>Field : "Sold" Tipo: "L" Formula:  ""</summary>
		public int ValSold
		{
			get { return (int)returnValueField(FldSold); }
			set { insertNameValueField(FldSold, value); }
		}

		/// <summary>Field : "Profit" Tipo: "$" Formula: + "iif([PROPE->SOLD]==1,[PROPE->PRICE],0)"</summary>
		public static FieldRef FldProfit { get { return m_fldProfit; } }
		private static FieldRef m_fldProfit = new FieldRef("prope", "profit");

		/// <summary>Field : "Profit" Tipo: "$" Formula: + "iif([PROPE->SOLD]==1,[PROPE->PRICE],0)"</summary>
		public decimal ValProfit
		{
			get { return (decimal)returnValueField(FldProfit); }
			set { insertNameValueField(FldProfit, value); }
		}

		/// <summary>Field : "Sold date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDtsold { get { return m_fldDtsold; } }
		private static FieldRef m_fldDtsold = new FieldRef("prope", "dtsold");

		/// <summary>Field : "Sold date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDtsold
		{
			get { return (DateTime)returnValueField(FldDtsold); }
			set { insertNameValueField(FldDtsold, value); }
		}

		/// <summary>Field : "AveragePrice" Tipo: "N" Formula: + "Average()"</summary>
		public static FieldRef FldAverage { get { return m_fldAverage; } }
		private static FieldRef m_fldAverage = new FieldRef("prope", "average");

		/// <summary>Field : "AveragePrice" Tipo: "N" Formula: + "Average()"</summary>
		public decimal ValAverage
		{
			get { return (decimal)returnValueField(FldAverage); }
			set { insertNameValueField(FldAverage, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("prope", "zzstate");



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
        public static CSGenioAprope search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAprope area = new CSGenioAprope(user, user.CurrentModule);

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
        public static List<CSGenioAprope> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAprope>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAprope> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAprope>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX PROPE]/

 
                     

	}
}
