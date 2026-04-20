
 
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
	/// Notification Message
	/// </summary>
	public class CSGenioAs_nm : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAs_nm(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR S_NM]/
		}

		public CSGenioAs_nm(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codmesgs", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codsigna", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codpmail", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "from", FieldType.TEXT);
			Qfield.FieldDescription = "Sender";
			Qfield.FieldSize =  254;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SENDER07671";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "codtpnot", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "coddestn", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  50;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "to", FieldType.TEXT);
			Qfield.FieldDescription = "To";
			Qfield.FieldSize =  254;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "TO55217";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "destnman", FieldType.LOGIC);
			Qfield.FieldDescription = "Manual destination";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "MANUAL_DESTINATION21892";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "tomanual", FieldType.MEMO);
			Qfield.FieldDescription = "Manual destination";
			Qfield.FieldSize =  8000;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "MANUAL_DESTINATION21892";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "cc", FieldType.MEMO);
			Qfield.FieldDescription = "Cc";
			Qfield.FieldSize =  8000;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CC35482";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "bcc", FieldType.MEMO);
			Qfield.FieldDescription = "Bcc";
			Qfield.FieldSize =  8000;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "BCC22049";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "idnotif", FieldType.TEXT);
			Qfield.FieldDescription = "Notification ID";
			Qfield.FieldSize =  100;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "NOTIFICATION_ID25507";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "notifica", FieldType.LOGIC);
			Qfield.FieldDescription = "Create a website alert";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CREATE_A_WEBSITE_ALE02013";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "email", FieldType.LOGIC);
			Qfield.FieldDescription = "Sends email?";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SENDS_EMAIL_12942";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "assunto", FieldType.TEXT);
			Qfield.FieldDescription = "Subject";
			Qfield.FieldSize =  100;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SUBJECT33942";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "agregado", FieldType.LOGIC);
			Qfield.FieldDescription = "Aggregate";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "AGGREGATE05721";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "anexo", FieldType.LOGIC);
			Qfield.FieldDescription = "Sends attachment?";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SENDS_ATTACHMENT_64661";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "html", FieldType.LOGIC);
			Qfield.FieldDescription = "HTML format?";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "HTML_FORMAT_60293";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "ativo", FieldType.LOGIC);
			Qfield.FieldDescription = "Enabled?";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "ENABLED_33995";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "designac", FieldType.TEXT);
			Qfield.FieldDescription = "Name";
			Qfield.FieldSize =  100;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "NAME31974";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "mensagem", FieldType.MEMO);
			Qfield.FieldDescription = "Message";
			Qfield.FieldSize =  8000;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "MESSAGE30602";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "gravabd", FieldType.LOGIC);
			Qfield.FieldDescription = "Saves on DB?";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SAVES_ON_DB_61384";

			Qfield.Dupmsg = "";
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








			//Write conditions
			List<ConditionFormula> conditions = new List<ConditionFormula>();
			info.WriteConditions = conditions.Where(c=> c.IsWriteCondition()).ToList();
			info.CrudConditions = conditions.Where(c=> c.IsCrudCondition()).ToList();

		}

		/// <summary>
		/// static CSGenioAs_nm()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="notificationmessage";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codmesgs";
			info.HumanKeyName="mensagem,".TrimEnd(',');
			info.Alias="s_nm";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Notification Message";
			info.AreaPluralDesignation="Notification Messages";
			info.DescriptionCav="NOTIFICATION_MESSAGE31108";

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
		public static FieldRef FldCodmesgs { get { return m_fldCodmesgs; } }
		private static FieldRef m_fldCodmesgs = new FieldRef("s_nm", "codmesgs");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodmesgs
		{
			get { return (string)returnValueField(FldCodmesgs); }
			set { insertNameValueField(FldCodmesgs, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCodsigna { get { return m_fldCodsigna; } }
		private static FieldRef m_fldCodsigna = new FieldRef("s_nm", "codsigna");

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public string ValCodsigna
		{
			get { return (string)returnValueField(FldCodsigna); }
			set { insertNameValueField(FldCodsigna, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCodpmail { get { return m_fldCodpmail; } }
		private static FieldRef m_fldCodpmail = new FieldRef("s_nm", "codpmail");

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public string ValCodpmail
		{
			get { return (string)returnValueField(FldCodpmail); }
			set { insertNameValueField(FldCodpmail, value); }
		}

		/// <summary>Field : "Sender" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldFrom { get { return m_fldFrom; } }
		private static FieldRef m_fldFrom = new FieldRef("s_nm", "from");

		/// <summary>Field : "Sender" Tipo: "C" Formula:  ""</summary>
		public string ValFrom
		{
			get { return (string)returnValueField(FldFrom); }
			set { insertNameValueField(FldFrom, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCodtpnot { get { return m_fldCodtpnot; } }
		private static FieldRef m_fldCodtpnot = new FieldRef("s_nm", "codtpnot");

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public string ValCodtpnot
		{
			get { return (string)returnValueField(FldCodtpnot); }
			set { insertNameValueField(FldCodtpnot, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCoddestn { get { return m_fldCoddestn; } }
		private static FieldRef m_fldCoddestn = new FieldRef("s_nm", "coddestn");

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public string ValCoddestn
		{
			get { return (string)returnValueField(FldCoddestn); }
			set { insertNameValueField(FldCoddestn, value); }
		}

		/// <summary>Field : "To" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldTo { get { return m_fldTo; } }
		private static FieldRef m_fldTo = new FieldRef("s_nm", "to");

		/// <summary>Field : "To" Tipo: "C" Formula:  ""</summary>
		public string ValTo
		{
			get { return (string)returnValueField(FldTo); }
			set { insertNameValueField(FldTo, value); }
		}

		/// <summary>Field : "Manual destination" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldDestnman { get { return m_fldDestnman; } }
		private static FieldRef m_fldDestnman = new FieldRef("s_nm", "destnman");

		/// <summary>Field : "Manual destination" Tipo: "L" Formula:  ""</summary>
		public int ValDestnman
		{
			get { return (int)returnValueField(FldDestnman); }
			set { insertNameValueField(FldDestnman, value); }
		}

		/// <summary>Field : "Manual destination" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldTomanual { get { return m_fldTomanual; } }
		private static FieldRef m_fldTomanual = new FieldRef("s_nm", "tomanual");

		/// <summary>Field : "Manual destination" Tipo: "MO" Formula:  ""</summary>
		public string ValTomanual
		{
			get { return (string)returnValueField(FldTomanual); }
			set { insertNameValueField(FldTomanual, value); }
		}

		/// <summary>Field : "Cc" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldCc { get { return m_fldCc; } }
		private static FieldRef m_fldCc = new FieldRef("s_nm", "cc");

		/// <summary>Field : "Cc" Tipo: "MO" Formula:  ""</summary>
		public string ValCc
		{
			get { return (string)returnValueField(FldCc); }
			set { insertNameValueField(FldCc, value); }
		}

		/// <summary>Field : "Bcc" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldBcc { get { return m_fldBcc; } }
		private static FieldRef m_fldBcc = new FieldRef("s_nm", "bcc");

		/// <summary>Field : "Bcc" Tipo: "MO" Formula:  ""</summary>
		public string ValBcc
		{
			get { return (string)returnValueField(FldBcc); }
			set { insertNameValueField(FldBcc, value); }
		}

		/// <summary>Field : "Notification ID" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldIdnotif { get { return m_fldIdnotif; } }
		private static FieldRef m_fldIdnotif = new FieldRef("s_nm", "idnotif");

		/// <summary>Field : "Notification ID" Tipo: "C" Formula:  ""</summary>
		public string ValIdnotif
		{
			get { return (string)returnValueField(FldIdnotif); }
			set { insertNameValueField(FldIdnotif, value); }
		}

		/// <summary>Field : "Create a website alert" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldNotifica { get { return m_fldNotifica; } }
		private static FieldRef m_fldNotifica = new FieldRef("s_nm", "notifica");

		/// <summary>Field : "Create a website alert" Tipo: "L" Formula:  ""</summary>
		public int ValNotifica
		{
			get { return (int)returnValueField(FldNotifica); }
			set { insertNameValueField(FldNotifica, value); }
		}

		/// <summary>Field : "Sends email?" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldEmail { get { return m_fldEmail; } }
		private static FieldRef m_fldEmail = new FieldRef("s_nm", "email");

		/// <summary>Field : "Sends email?" Tipo: "L" Formula:  ""</summary>
		public int ValEmail
		{
			get { return (int)returnValueField(FldEmail); }
			set { insertNameValueField(FldEmail, value); }
		}

		/// <summary>Field : "Subject" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldAssunto { get { return m_fldAssunto; } }
		private static FieldRef m_fldAssunto = new FieldRef("s_nm", "assunto");

		/// <summary>Field : "Subject" Tipo: "C" Formula:  ""</summary>
		public string ValAssunto
		{
			get { return (string)returnValueField(FldAssunto); }
			set { insertNameValueField(FldAssunto, value); }
		}

		/// <summary>Field : "Aggregate" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldAgregado { get { return m_fldAgregado; } }
		private static FieldRef m_fldAgregado = new FieldRef("s_nm", "agregado");

		/// <summary>Field : "Aggregate" Tipo: "L" Formula:  ""</summary>
		public int ValAgregado
		{
			get { return (int)returnValueField(FldAgregado); }
			set { insertNameValueField(FldAgregado, value); }
		}

		/// <summary>Field : "Sends attachment?" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldAnexo { get { return m_fldAnexo; } }
		private static FieldRef m_fldAnexo = new FieldRef("s_nm", "anexo");

		/// <summary>Field : "Sends attachment?" Tipo: "L" Formula:  ""</summary>
		public int ValAnexo
		{
			get { return (int)returnValueField(FldAnexo); }
			set { insertNameValueField(FldAnexo, value); }
		}

		/// <summary>Field : "HTML format?" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldHtml { get { return m_fldHtml; } }
		private static FieldRef m_fldHtml = new FieldRef("s_nm", "html");

		/// <summary>Field : "HTML format?" Tipo: "L" Formula:  ""</summary>
		public int ValHtml
		{
			get { return (int)returnValueField(FldHtml); }
			set { insertNameValueField(FldHtml, value); }
		}

		/// <summary>Field : "Enabled?" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldAtivo { get { return m_fldAtivo; } }
		private static FieldRef m_fldAtivo = new FieldRef("s_nm", "ativo");

		/// <summary>Field : "Enabled?" Tipo: "L" Formula:  ""</summary>
		public int ValAtivo
		{
			get { return (int)returnValueField(FldAtivo); }
			set { insertNameValueField(FldAtivo, value); }
		}

		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldDesignac { get { return m_fldDesignac; } }
		private static FieldRef m_fldDesignac = new FieldRef("s_nm", "designac");

		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		public string ValDesignac
		{
			get { return (string)returnValueField(FldDesignac); }
			set { insertNameValueField(FldDesignac, value); }
		}

		/// <summary>Field : "Message" Tipo: "MO" Formula:  ""</summary>
		public static FieldRef FldMensagem { get { return m_fldMensagem; } }
		private static FieldRef m_fldMensagem = new FieldRef("s_nm", "mensagem");

		/// <summary>Field : "Message" Tipo: "MO" Formula:  ""</summary>
		public string ValMensagem
		{
			get { return (string)returnValueField(FldMensagem); }
			set { insertNameValueField(FldMensagem, value); }
		}

		/// <summary>Field : "Saves on DB?" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldGravabd { get { return m_fldGravabd; } }
		private static FieldRef m_fldGravabd = new FieldRef("s_nm", "gravabd");

		/// <summary>Field : "Saves on DB?" Tipo: "L" Formula:  ""</summary>
		public int ValGravabd
		{
			get { return (int)returnValueField(FldGravabd); }
			set { insertNameValueField(FldGravabd, value); }
		}

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("s_nm", "opercria");

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public static FieldRef FldDatacria { get { return m_fldDatacria; } }
		private static FieldRef m_fldDatacria = new FieldRef("s_nm", "datacria");

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public DateTime ValDatacria
		{
			get { return (DateTime)returnValueField(FldDatacria); }
			set { insertNameValueField(FldDatacria, value); }
		}

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
		private static FieldRef m_fldOpermuda = new FieldRef("s_nm", "opermuda");

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public string ValOpermuda
		{
			get { return (string)returnValueField(FldOpermuda); }
			set { insertNameValueField(FldOpermuda, value); }
		}

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
		private static FieldRef m_fldDatamuda = new FieldRef("s_nm", "datamuda");

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public DateTime ValDatamuda
		{
			get { return (DateTime)returnValueField(FldDatamuda); }
			set { insertNameValueField(FldDatamuda, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("s_nm", "zzstate");



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
        public static CSGenioAs_nm search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioAs_nm area = new CSGenioAs_nm(user, user.CurrentModule);

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
        public static List<CSGenioAs_nm> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioAs_nm>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAs_nm> listing)
        {
			sp.searchListAdvancedWhere<CSGenioAs_nm>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX S_NM]/

 
                           

	}
}
