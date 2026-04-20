
 
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
	/// Async process
	/// </summary>
	public class CSGenioAs_apr : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAs_apr(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR S_APR]/
		}

		public CSGenioAs_apr(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codascpr", FieldType.KEY_GUID);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  36;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "type", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Process type";
			Qfield.FieldSize =  12;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PROCESS_TYPE25967";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCs_tpproc";
            Qfield.ArrayClassName = "S_tpproc";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "daterequ", FieldType.DATE);
			Qfield.FieldDescription = "Request date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "REQUEST_DATE25771";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "initprc", FieldType.DATETIME);
			Qfield.FieldDescription = "Start time";
			Qfield.FieldSize =  16;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "START_TIME30037";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "endprc", FieldType.DATETIME);
			Qfield.FieldDescription = "End time";
			Qfield.FieldSize =  16;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "END_TIME53495";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "duration", FieldType.TIME_HOURS);
			Qfield.FieldDescription = "Duration";
			Qfield.FieldSize =  5;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "DURATION40426";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "status", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Status";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "STATUS62033";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCs_prstat";
            Qfield.ArrayClassName = "S_prstat";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "rsltmsg", FieldType.TEXT);
			Qfield.FieldDescription = "Result message";
			Qfield.FieldSize =  250;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "RESULT_MESSAGE40830";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "finished", FieldType.LOGIC);
			Qfield.FieldDescription = "Finished";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "FINISHED26993";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"status","status","status"}, new int[] {0,1,2}, "s_apr", "codascpr"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 3, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((((string)args[0])=="T"||((string)args[1])=="AB"||((string)args[2])=="C")?(1):(0));
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "lastupdt", FieldType.DATETIMESECONDS);
			Qfield.FieldDescription = "Last update";
			Qfield.FieldSize =  19;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "LAST_UPDATE11909";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "result", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Result";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "RESULT40974";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCs_resul";
            Qfield.ArrayClassName = "S_resul";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "info", FieldType.TEXT);
			Qfield.FieldDescription = "Process info";
			Qfield.FieldSize =  500;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PROCESS_INFO62044";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "percenta", FieldType.NUMERIC);
			Qfield.FieldDescription = "Percentage";
			Qfield.FieldSize =  3;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 3;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PERCENTAGE57728";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "modoproc", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Process mode";
			Qfield.FieldSize =  9;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PROCESS_MODE22419";

			Qfield.Dupmsg = "";
            Qfield.ArrayName = "dbo.GetValArrayCs_modpro";
            Qfield.ArrayClassName = "S_modpro";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "external", FieldType.LOGIC);
			Qfield.FieldDescription = "Executed by external app";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "EXECUTED_BY_EXTERNAL36156";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "id", FieldType.NUMERIC);
			Qfield.FieldDescription = "Process ID";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 8;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PROCESS_ID11161";

			Qfield.Dupmsg = "";
			Qfield.DefaultValue = new DefaultValue(DefaultValue.getGreaterPlus1_int, "id");
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codentit", FieldType.KEY_INT);
			Qfield.FieldDescription = "Entid key";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "motivo", FieldType.TEXT);
			Qfield.FieldDescription = "Motive";
			Qfield.FieldSize =  200;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "MOTIVE13407";

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
			Qfield = new Field(info.Alias, "opershut", FieldType.TEXT);
			Qfield.FieldDescription = "Canceled by";
			Qfield.FieldSize =  128;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CANCELED_BY01167";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "rtstatus", FieldType.ARRAY_TEXT);
			Qfield.FieldDescription = "Real time status";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "REAL_TIME_STATUS00476";

			Qfield.IsVirtual = true;
			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"status","status","status","status","lastupdt","status","lastupdt","status","status","status"}, new int[] {0,1,2,3,4,5,6,7,8,9}, "s_apr", "codascpr"));
			Qfield.Formula = new InternalOperationFormula(argumentsListByArea, 10, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return ((((string)args[0])=="EE"||((string)args[1])=="D"||((string)args[2])=="AC"||((string)args[3])=="AG")?((((GenFunctions.DateDiffPart(((DateTime)args[4]),DateTime.Now,"S")>10&&((string)args[5])!="AG")||(GenFunctions.DateDiffPart(((DateTime)args[6]),DateTime.Now,"S")>45&&((string)args[7])=="AG"))?("NR"):(((string)args[8])))):(((string)args[9])));
			});
            Qfield.ArrayName = "dbo.GetValArrayCs_prstat";
            Qfield.ArrayClassName = "S_prstat";
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
			info.ChildTable = new ChildRelation[2];
			info.ChildTable[0]= new ChildRelation("s_pax", new String[] {"cods_apr"}, DeleteProc.NA);
			info.ChildTable[1]= new ChildRelation("s_arg", new String[] {"cods_apr"}, DeleteProc.NA);

			// Mother Relations
			//------------------------------
			info.ParentTables = new Dictionary<string, Relation>();
		}

		/// <summary>
		/// Initializes metadata for indirect paths to other areas
		/// </summary>
		private static void InicializaCaminhos(AreaInfo info)
		{
			// Pathways
			//------------------------------
			info.Pathways = new Dictionary<string, string>(0);
		}

		/// <summary>
		/// Initializes metadata for triggers and formula arguments
		/// </summary>
		private static void InicializaFormulas(AreaInfo info)
		{
			// Formulas
			//------------------------------



			info.InternalOperationFields = new string[] {
			 "finished","rtstatus"
			};

			info.SequentialDefaultValues = new string[] {
			 "id"
			};





			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAs_apr()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="asyncprocess";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codascpr";
			info.HumanKeyName="id,".TrimEnd(',');
			info.Alias="s_apr";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Async process";
			info.AreaPluralDesignation="Async process";
			info.DescriptionCav="ASYNC_PROCESS56674";

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
		public static FieldRef FldCodascpr { get { return m_fldCodascpr; } }
		private static FieldRef m_fldCodascpr = new FieldRef("s_apr", "codascpr");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodascpr
		{
			get { return (string)returnValueField(FldCodascpr); }
			set { insertNameValueField(FldCodascpr, value); }
		}

		/// <summary>Field : "Process type" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldType { get { return m_fldType; } }
		private static FieldRef m_fldType = new FieldRef("s_apr", "type");

		/// <summary>Field : "Process type" Tipo: "AC" Formula:  ""</summary>
		public string ValType
		{
			get { return (string)returnValueField(FldType); }
			set { insertNameValueField(FldType, value); }
		}

		/// <summary>Field : "Request date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDaterequ { get { return m_fldDaterequ; } }
		private static FieldRef m_fldDaterequ = new FieldRef("s_apr", "daterequ");

		/// <summary>Field : "Request date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDaterequ
		{
			get { return (DateTime)returnValueField(FldDaterequ); }
			set { insertNameValueField(FldDaterequ, value); }
		}

		/// <summary>Field : "Start time" Tipo: "DT" Formula:  ""</summary>
		public static FieldRef FldInitprc { get { return m_fldInitprc; } }
		private static FieldRef m_fldInitprc = new FieldRef("s_apr", "initprc");

		/// <summary>Field : "Start time" Tipo: "DT" Formula:  ""</summary>
		public DateTime ValInitprc
		{
			get { return (DateTime)returnValueField(FldInitprc); }
			set { insertNameValueField(FldInitprc, value); }
		}

		/// <summary>Field : "End time" Tipo: "DT" Formula:  ""</summary>
		public static FieldRef FldEndprc { get { return m_fldEndprc; } }
		private static FieldRef m_fldEndprc = new FieldRef("s_apr", "endprc");

		/// <summary>Field : "End time" Tipo: "DT" Formula:  ""</summary>
		public DateTime ValEndprc
		{
			get { return (DateTime)returnValueField(FldEndprc); }
			set { insertNameValueField(FldEndprc, value); }
		}

		/// <summary>Field : "Duration" Tipo: "T" Formula:  ""</summary>
		public static FieldRef FldDuration { get { return m_fldDuration; } }
		private static FieldRef m_fldDuration = new FieldRef("s_apr", "duration");

		/// <summary>Field : "Duration" Tipo: "T" Formula:  ""</summary>
		public string ValDuration
		{
			get { return (string)returnValueField(FldDuration); }
			set { insertNameValueField(FldDuration, value); }
		}

		/// <summary>Field : "Status" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldStatus { get { return m_fldStatus; } }
		private static FieldRef m_fldStatus = new FieldRef("s_apr", "status");

		/// <summary>Field : "Status" Tipo: "AC" Formula:  ""</summary>
		public string ValStatus
		{
			get { return (string)returnValueField(FldStatus); }
			set { insertNameValueField(FldStatus, value); }
		}

		/// <summary>Field : "Result message" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldRsltmsg { get { return m_fldRsltmsg; } }
		private static FieldRef m_fldRsltmsg = new FieldRef("s_apr", "rsltmsg");

		/// <summary>Field : "Result message" Tipo: "C" Formula:  ""</summary>
		public string ValRsltmsg
		{
			get { return (string)returnValueField(FldRsltmsg); }
			set { insertNameValueField(FldRsltmsg, value); }
		}

		/// <summary>Field : "Finished" Tipo: "L" Formula: + "iif([S_APR->STATUS]=="T" || [S_APR->STATUS]=="AB" || [S_APR->STATUS]=="C" ,1, 0)"</summary>
		public static FieldRef FldFinished { get { return m_fldFinished; } }
		private static FieldRef m_fldFinished = new FieldRef("s_apr", "finished");

		/// <summary>Field : "Finished" Tipo: "L" Formula: + "iif([S_APR->STATUS]=="T" || [S_APR->STATUS]=="AB" || [S_APR->STATUS]=="C" ,1, 0)"</summary>
		public int ValFinished
		{
			get { return (int)returnValueField(FldFinished); }
			set { insertNameValueField(FldFinished, value); }
		}

		/// <summary>Field : "Last update" Tipo: "DS" Formula:  ""</summary>
		public static FieldRef FldLastupdt { get { return m_fldLastupdt; } }
		private static FieldRef m_fldLastupdt = new FieldRef("s_apr", "lastupdt");

		/// <summary>Field : "Last update" Tipo: "DS" Formula:  ""</summary>
		public DateTime ValLastupdt
		{
			get { return (DateTime)returnValueField(FldLastupdt); }
			set { insertNameValueField(FldLastupdt, value); }
		}

		/// <summary>Field : "Result" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldResult { get { return m_fldResult; } }
		private static FieldRef m_fldResult = new FieldRef("s_apr", "result");

		/// <summary>Field : "Result" Tipo: "AC" Formula:  ""</summary>
		public string ValResult
		{
			get { return (string)returnValueField(FldResult); }
			set { insertNameValueField(FldResult, value); }
		}

		/// <summary>Field : "Process info" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldInfo { get { return m_fldInfo; } }
		private static FieldRef m_fldInfo = new FieldRef("s_apr", "info");

		/// <summary>Field : "Process info" Tipo: "C" Formula:  ""</summary>
		public string ValInfo
		{
			get { return (string)returnValueField(FldInfo); }
			set { insertNameValueField(FldInfo, value); }
		}

		/// <summary>Field : "Percentage" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldPercenta { get { return m_fldPercenta; } }
		private static FieldRef m_fldPercenta = new FieldRef("s_apr", "percenta");

		/// <summary>Field : "Percentage" Tipo: "N" Formula:  ""</summary>
		public decimal ValPercenta
		{
			get { return (decimal)returnValueField(FldPercenta); }
			set { insertNameValueField(FldPercenta, value); }
		}

		/// <summary>Field : "Process mode" Tipo: "AC" Formula:  ""</summary>
		public static FieldRef FldModoproc { get { return m_fldModoproc; } }
		private static FieldRef m_fldModoproc = new FieldRef("s_apr", "modoproc");

		/// <summary>Field : "Process mode" Tipo: "AC" Formula:  ""</summary>
		public string ValModoproc
		{
			get { return (string)returnValueField(FldModoproc); }
			set { insertNameValueField(FldModoproc, value); }
		}

		/// <summary>Field : "Executed by external app" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldExternal { get { return m_fldExternal; } }
		private static FieldRef m_fldExternal = new FieldRef("s_apr", "external");

		/// <summary>Field : "Executed by external app" Tipo: "L" Formula:  ""</summary>
		public int ValExternal
		{
			get { return (int)returnValueField(FldExternal); }
			set { insertNameValueField(FldExternal, value); }
		}

		/// <summary>Field : "Process ID" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldId { get { return m_fldId; } }
		private static FieldRef m_fldId = new FieldRef("s_apr", "id");

		/// <summary>Field : "Process ID" Tipo: "N" Formula:  ""</summary>
		public decimal ValId
		{
			get { return (decimal)returnValueField(FldId); }
			set { insertNameValueField(FldId, value); }
		}

		/// <summary>Field : "Entid key" Tipo: "CF" Formula:  ""</summary>
		public static FieldRef FldCodentit { get { return m_fldCodentit; } }
		private static FieldRef m_fldCodentit = new FieldRef("s_apr", "codentit");

		/// <summary>Field : "Entid key" Tipo: "CF" Formula:  ""</summary>
		public string ValCodentit
		{
			get { return (string)returnValueField(FldCodentit); }
			set { insertNameValueField(FldCodentit, value); }
		}

		/// <summary>Field : "Motive" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldMotivo { get { return m_fldMotivo; } }
		private static FieldRef m_fldMotivo = new FieldRef("s_apr", "motivo");

		/// <summary>Field : "Motive" Tipo: "C" Formula:  ""</summary>
		public string ValMotivo
		{
			get { return (string)returnValueField(FldMotivo); }
			set { insertNameValueField(FldMotivo, value); }
		}

		/// <summary>Field : "" Tipo: "CF" Formula:  ""</summary>
		public static FieldRef FldCodpsw { get { return m_fldCodpsw; } }
		private static FieldRef m_fldCodpsw = new FieldRef("s_apr", "codpsw");

		/// <summary>Field : "" Tipo: "CF" Formula:  ""</summary>
		public string ValCodpsw
		{
			get { return (string)returnValueField(FldCodpsw); }
			set { insertNameValueField(FldCodpsw, value); }
		}

		/// <summary>Field : "Canceled by" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldOpershut { get { return m_fldOpershut; } }
		private static FieldRef m_fldOpershut = new FieldRef("s_apr", "opershut");

		/// <summary>Field : "Canceled by" Tipo: "C" Formula:  ""</summary>
		public string ValOpershut
		{
			get { return (string)returnValueField(FldOpershut); }
			set { insertNameValueField(FldOpershut, value); }
		}

		/// <summary>Field : "Real time status" Tipo: "AC" Formula: + "iif([S_APR->STATUS] == "EE" || [S_APR->STATUS] == "D" || [S_APR->STATUS] == "AC" || [S_APR->STATUS] == "AG", iif((Diferenca_entre_Datas([S_APR->LASTUPDT], [Now], "S") > 10 && [S_APR->STATUS] != "AG") || (Diferenca_entre_Datas([S_APR->LASTUPDT], [Now], "S") > 45 && [S_APR->STATUS] == "AG"), "NR", [S_APR->STATUS]), [S_APR->STATUS])"</summary>
		public static FieldRef FldRtstatus { get { return m_fldRtstatus; } }
		private static FieldRef m_fldRtstatus = new FieldRef("s_apr", "rtstatus");

		/// <summary>Field : "Real time status" Tipo: "AC" Formula: + "iif([S_APR->STATUS] == "EE" || [S_APR->STATUS] == "D" || [S_APR->STATUS] == "AC" || [S_APR->STATUS] == "AG", iif((Diferenca_entre_Datas([S_APR->LASTUPDT], [Now], "S") > 10 && [S_APR->STATUS] != "AG") || (Diferenca_entre_Datas([S_APR->LASTUPDT], [Now], "S") > 45 && [S_APR->STATUS] == "AG"), "NR", [S_APR->STATUS]), [S_APR->STATUS])"</summary>
		public string ValRtstatus
		{
			get { return (string)returnValueField(FldRtstatus); }
			set { insertNameValueField(FldRtstatus, value); }
		}

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("s_apr", "opercria");

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public static FieldRef FldDatacria { get { return m_fldDatacria; } }
		private static FieldRef m_fldDatacria = new FieldRef("s_apr", "datacria");

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public DateTime ValDatacria
		{
			get { return (DateTime)returnValueField(FldDatacria); }
			set { insertNameValueField(FldDatacria, value); }
		}

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
		private static FieldRef m_fldOpermuda = new FieldRef("s_apr", "opermuda");

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public string ValOpermuda
		{
			get { return (string)returnValueField(FldOpermuda); }
			set { insertNameValueField(FldOpermuda, value); }
		}

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
		private static FieldRef m_fldDatamuda = new FieldRef("s_apr", "datamuda");

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public DateTime ValDatamuda
		{
			get { return (DateTime)returnValueField(FldDatamuda); }
			set { insertNameValueField(FldDatamuda, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("s_apr", "zzstate");



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
        public static CSGenioAs_apr search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAs_apr area = new CSGenioAs_apr(user, user.CurrentModule);

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
        public static List<CSGenioAs_apr> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAs_apr>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAs_apr> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAs_apr>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX S_APR]/

 
                          

	}
}
