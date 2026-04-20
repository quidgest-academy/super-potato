
 
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
	/// Password
	/// </summary>
	public class CSGenioApsw : DbArea
	{
		/// <summary>
		/// Meta-information on this area
		/// </summary>
		protected readonly static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioApsw(User user, string module)
		{
            this.user = user;
            this.module = module;
			// USE /[MANUAL FOR CONSTRUTOR PSW]/
		}

		public CSGenioApsw(User user) : this(user, user.CurrentModule)
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
			Qfield = new Field(info.Alias, "codpsw", FieldType.KEY_INT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "nome", FieldType.TEXT);
			Qfield.FieldDescription = "Name";
			Qfield.FieldSize =  100;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "NAME31974";

			Qfield.Dupmsg = "";
            Qfield.NotDup = true;
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "password", FieldType.ENCRYPTED);
			Qfield.FieldDescription = "Password";
			Qfield.FieldSize =  150;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PASSWORD09467";

			Qfield.Dupmsg = "";
			argumentsListByArea = new List<ByAreaArguments>();
			argumentsListByArea.Add(new ByAreaArguments(new string[] {"password","pswtype"}, new int[] {0,1}, "psw", "codpsw"));
			Qfield.EncryptFieldValueFormula = new InternalOperationFormula(argumentsListByArea, 2, delegate(object[] args, User user, string module, PersistentSupport sp) {
				return GenioServer.security.PasswordFactory.EncryptPasswordField((string)(args[0] as EncryptedDataType)?.DecryptedValue, (string)args[1]);
			});
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "certsn", FieldType.TEXT);
			Qfield.FieldDescription = "Certified Series Number";
			Qfield.FieldSize =  32;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "CERTIFIED_SERIES_NUM14349";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "email", FieldType.TEXT);
			Qfield.FieldDescription = "Email";
			Qfield.FieldSize =  254;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "EMAIL25170";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "pswtype", FieldType.TEXT);
			Qfield.FieldDescription = "Password type";
			Qfield.FieldSize =  3;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PASSWORD_TYPE03035";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "salt", FieldType.TEXT);
			Qfield.FieldDescription = "Salt";
			Qfield.FieldSize =  32;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "SALT05277";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "datapsw", FieldType.DATE);
			Qfield.FieldDescription = "Password date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PASSWORD_DATE16593";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "userid", FieldType.TEXT);
			Qfield.FieldDescription = "User ID";
			Qfield.FieldSize =  250;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "USER_ID13914";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "psw2favl", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  1000;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "psw2fatp", FieldType.TEXT);
			Qfield.FieldDescription = "";
			Qfield.FieldSize =  16;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "datexp", FieldType.DATE);
			Qfield.FieldDescription = "Expiration date";
			Qfield.FieldSize =  8;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "EXPIRATION_DATE34293";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "attempts", FieldType.NUMERIC);
			Qfield.FieldDescription = "Login attempts";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 2;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "LOGIN_ATTEMPTS62337";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "phone", FieldType.TEXT);
			Qfield.FieldDescription = "Phone number";
			Qfield.FieldSize =  16;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "PHONE_NUMBER20774";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "status", FieldType.NUMERIC);
			Qfield.FieldDescription = "Status";
			Qfield.FieldSize =  2;
			Qfield.MQueue = false;
			Qfield.IntegerDigits = 2;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "STATUS62033";

			Qfield.Dupmsg = "";
			info.RegisterFieldDB(Qfield);

			//- - - - - - - - - - - - - - - - - - -
			Qfield = new Field(info.Alias, "associa", FieldType.LOGIC);
			Qfield.FieldDescription = "Has login?";
			Qfield.FieldSize =  1;
			Qfield.MQueue = false;
			Qfield.VisivelCav = CavVisibilityType.Nunca;
			Qfield.CavDesignation = "HAS_LOGIN_58044";

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
			info.ChildTable = new ChildRelation[1];
			info.ChildTable[0]= new ChildRelation("s_ua", new String[] {"codpsw"}, DeleteProc.NA);

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

			info.PasswordFields = new string[] {
			 "password"
			};
		}

		/// <summary>
		/// static CSGenioApsw()
		/// </summary>
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();

			// Area meta-information
			info.QSystem="FOR";
			info.TableName="userlogin";
			info.ShadowTabName="";
			info.ShadowTabKeyName="";

			info.PrimaryKeyName="codpsw";
			info.HumanKeyName="";
			info.Alias="psw";
			info.IsDomain = true;
			info.PersistenceType = PersistenceType.Database;
			info.AreaDesignation="Password";
			info.AreaPluralDesignation="Passwords";
			info.DescriptionCav="PASSWORD09467";

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
		public static FieldRef FldCodpsw { get { return m_fldCodpsw; } }
		private static FieldRef m_fldCodpsw = new FieldRef("psw", "codpsw");

		/// <summary>Field : "" Tipo: "+" Formula:  ""</summary>
		public string ValCodpsw
		{
			get { return (string)returnValueField(FldCodpsw); }
			set { insertNameValueField(FldCodpsw, value); }
		}

		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldNome { get { return m_fldNome; } }
		private static FieldRef m_fldNome = new FieldRef("psw", "nome");

		/// <summary>Field : "Name" Tipo: "C" Formula:  ""</summary>
		public string ValNome
		{
			get { return (string)returnValueField(FldNome); }
			set { insertNameValueField(FldNome, value); }
		}

		/// <summary>Field : "Password" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldPassword { get { return m_fldPassword; } }
		private static FieldRef m_fldPassword = new FieldRef("psw", "password");

		/// <summary>Field : "Password" Tipo: "C" Formula:  ""</summary>
		public string ValPassword
		{
			get { return (string)((EncryptedDataType)returnValueField(FldPassword)).EncryptedValue; }
			set { insertNameValueField(FldPassword, value); }
		}

        /// <summary>Field : "Password | Decrypted value" Type: "C"</summary>
        public string ValPasswordDecrypted
        {
            get { return (string)ReturnDecryptedValueField(FldPassword); }
            set { InsertNameDecryptedValueField(FldPassword, value); }
        }

		/// <summary>Field : "Certified Series Number" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldCertsn { get { return m_fldCertsn; } }
		private static FieldRef m_fldCertsn = new FieldRef("psw", "certsn");

		/// <summary>Field : "Certified Series Number" Tipo: "C" Formula:  ""</summary>
		public string ValCertsn
		{
			get { return (string)returnValueField(FldCertsn); }
			set { insertNameValueField(FldCertsn, value); }
		}

		/// <summary>Field : "Email" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldEmail { get { return m_fldEmail; } }
		private static FieldRef m_fldEmail = new FieldRef("psw", "email");

		/// <summary>Field : "Email" Tipo: "C" Formula:  ""</summary>
		public string ValEmail
		{
			get { return (string)returnValueField(FldEmail); }
			set { insertNameValueField(FldEmail, value); }
		}

		/// <summary>Field : "Password type" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldPswtype { get { return m_fldPswtype; } }
		private static FieldRef m_fldPswtype = new FieldRef("psw", "pswtype");

		/// <summary>Field : "Password type" Tipo: "C" Formula:  ""</summary>
		public string ValPswtype
		{
			get { return (string)returnValueField(FldPswtype); }
			set { insertNameValueField(FldPswtype, value); }
		}

		/// <summary>Field : "Salt" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldSalt { get { return m_fldSalt; } }
		private static FieldRef m_fldSalt = new FieldRef("psw", "salt");

		/// <summary>Field : "Salt" Tipo: "C" Formula:  ""</summary>
		public string ValSalt
		{
			get { return (string)returnValueField(FldSalt); }
			set { insertNameValueField(FldSalt, value); }
		}

		/// <summary>Field : "Password date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDatapsw { get { return m_fldDatapsw; } }
		private static FieldRef m_fldDatapsw = new FieldRef("psw", "datapsw");

		/// <summary>Field : "Password date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDatapsw
		{
			get { return (DateTime)returnValueField(FldDatapsw); }
			set { insertNameValueField(FldDatapsw, value); }
		}

		/// <summary>Field : "User ID" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldUserid { get { return m_fldUserid; } }
		private static FieldRef m_fldUserid = new FieldRef("psw", "userid");

		/// <summary>Field : "User ID" Tipo: "C" Formula:  ""</summary>
		public string ValUserid
		{
			get { return (string)returnValueField(FldUserid); }
			set { insertNameValueField(FldUserid, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldPsw2favl { get { return m_fldPsw2favl; } }
		private static FieldRef m_fldPsw2favl = new FieldRef("psw", "psw2favl");

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public string ValPsw2favl
		{
			get { return (string)returnValueField(FldPsw2favl); }
			set { insertNameValueField(FldPsw2favl, value); }
		}

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldPsw2fatp { get { return m_fldPsw2fatp; } }
		private static FieldRef m_fldPsw2fatp = new FieldRef("psw", "psw2fatp");

		/// <summary>Field : "" Tipo: "C" Formula:  ""</summary>
		public string ValPsw2fatp
		{
			get { return (string)returnValueField(FldPsw2fatp); }
			set { insertNameValueField(FldPsw2fatp, value); }
		}

		/// <summary>Field : "Expiration date" Tipo: "D" Formula:  ""</summary>
		public static FieldRef FldDatexp { get { return m_fldDatexp; } }
		private static FieldRef m_fldDatexp = new FieldRef("psw", "datexp");

		/// <summary>Field : "Expiration date" Tipo: "D" Formula:  ""</summary>
		public DateTime ValDatexp
		{
			get { return (DateTime)returnValueField(FldDatexp); }
			set { insertNameValueField(FldDatexp, value); }
		}

		/// <summary>Field : "Login attempts" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldAttempts { get { return m_fldAttempts; } }
		private static FieldRef m_fldAttempts = new FieldRef("psw", "attempts");

		/// <summary>Field : "Login attempts" Tipo: "N" Formula:  ""</summary>
		public decimal ValAttempts
		{
			get { return (decimal)returnValueField(FldAttempts); }
			set { insertNameValueField(FldAttempts, value); }
		}

		/// <summary>Field : "Phone number" Tipo: "C" Formula:  ""</summary>
		public static FieldRef FldPhone { get { return m_fldPhone; } }
		private static FieldRef m_fldPhone = new FieldRef("psw", "phone");

		/// <summary>Field : "Phone number" Tipo: "C" Formula:  ""</summary>
		public string ValPhone
		{
			get { return (string)returnValueField(FldPhone); }
			set { insertNameValueField(FldPhone, value); }
		}

		/// <summary>Field : "Status" Tipo: "N" Formula:  ""</summary>
		public static FieldRef FldStatus { get { return m_fldStatus; } }
		private static FieldRef m_fldStatus = new FieldRef("psw", "status");

		/// <summary>Field : "Status" Tipo: "N" Formula:  ""</summary>
		public decimal ValStatus
		{
			get { return (decimal)returnValueField(FldStatus); }
			set { insertNameValueField(FldStatus, value); }
		}

		/// <summary>Field : "Has login?" Tipo: "L" Formula:  ""</summary>
		public static FieldRef FldAssocia { get { return m_fldAssocia; } }
		private static FieldRef m_fldAssocia = new FieldRef("psw", "associa");

		/// <summary>Field : "Has login?" Tipo: "L" Formula:  ""</summary>
		public int ValAssocia
		{
			get { return (int)returnValueField(FldAssocia); }
			set { insertNameValueField(FldAssocia, value); }
		}

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("psw", "opercria");

		/// <summary>Field : "Created by" Tipo: "ON" Formula:  ""</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public static FieldRef FldDatacria { get { return m_fldDatacria; } }
		private static FieldRef m_fldDatacria = new FieldRef("psw", "datacria");

		/// <summary>Field : "Created on" Tipo: "OD" Formula:  ""</summary>
		public DateTime ValDatacria
		{
			get { return (DateTime)returnValueField(FldDatacria); }
			set { insertNameValueField(FldDatacria, value); }
		}

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
		private static FieldRef m_fldOpermuda = new FieldRef("psw", "opermuda");

		/// <summary>Field : "Changed by" Tipo: "EN" Formula:  ""</summary>
		public string ValOpermuda
		{
			get { return (string)returnValueField(FldOpermuda); }
			set { insertNameValueField(FldOpermuda, value); }
		}

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
		private static FieldRef m_fldDatamuda = new FieldRef("psw", "datamuda");

		/// <summary>Field : "Changed on" Tipo: "ED" Formula:  ""</summary>
		public DateTime ValDatamuda
		{
			get { return (DateTime)returnValueField(FldDatamuda); }
			set { insertNameValueField(FldDatamuda, value); }
		}

		/// <summary>Field : "ZZSTATE" Type: "INT" Formula:  ""</summary>
		public static FieldRef FldZzstate { get { return m_fldZzstate; } }
		private static FieldRef m_fldZzstate = new FieldRef("psw", "zzstate");



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
        public static CSGenioApsw search(PersistentSupport sp, string key, User user, string[] fields = null, bool forUpdate = false)
        {
			if (string.IsNullOrEmpty(key))
				return null;

		    CSGenioApsw area = new CSGenioApsw(user, user.CurrentModule);

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
        public static List<CSGenioApsw> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioApsw>(where, user, fields, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioApsw> listing)
        {
			sp.searchListAdvancedWhere<CSGenioApsw>(where, listing);
        }




		/// <summary>
		/// Check if a record exist
		/// </summary>
		/// <param name="key">Record key</param>
		/// <param name="sp">DB conecntion</param>
		/// <returns>True if the record exist</returns>
		public static bool RecordExist(string key, PersistentSupport sp) => DbArea.RecordExist(key, informacao, sp);








		// USE /[MANUAL FOR TABAUX PSW]/

 
                               /// <summary>
        /// Set decrypted value to encrypted field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldValue">Decrypted value</param>
        public override void InsertNameDecryptedValueField(string fieldName, object fieldValue)
        {
            // Auto-completion of the cipher type to be used when will creating the Update Query.
            // It also includes other fields needed for the specific case of the PSW Password field.
            if(fieldName == FldPassword)
            {
                if(!string.IsNullOrWhiteSpace((string)fieldValue))
                {
                    insertNameValueField(fieldName, new EncryptedDataType(null, fieldValue));
                    insertNameValueField(FldSalt, string.Empty);
                    insertNameValueField(FldPswtype, Configuration.Security.PasswordAlgorithms.ToString());
                    insertNameValueField(FldDatexp, GenioServer.security.UserFactory.CalculateExpirationDate());
                }
            }
            else
                base.InsertNameDecryptedValueField(fieldName, fieldValue);
        }



	}
}
