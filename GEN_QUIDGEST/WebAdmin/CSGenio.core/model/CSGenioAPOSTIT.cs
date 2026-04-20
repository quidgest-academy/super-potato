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
	public class CSGenioApostit : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioApostit(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioApostit(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "forpostit";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codpostit";
            info.HumanKeyName = "name";
			info.ShadowTabKeyName = "";
			info.Alias = "postit";
			info.IsDomain =  false;
			info.AreaDesignation = "Post-It";
			info.AreaPluralDesignation = "Post-Its";
			info.DescriptionCav = "Post-It";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
            info.RegisterFieldDB(new Field(info.Alias, "codpostit", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "codpost1", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "codtabel", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "tabela", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "codpsw", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "postit", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "tpostit", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "opercria", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "lido", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "apagado", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "validade", FieldType.DATETIMESECONDS));
            //info.RegisterFieldDB(new Field(info.Alias, "nivel", FieldType.INTEGER));
            info.RegisterFieldDB(new Field(info.Alias, "recipient", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

            //------------------------------
            info.StampFieldsIns = ["opercria", "datacria"];

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

		// USE /[MANUAL FOR TABAUX postit]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        /// <summary>Campo: "PK" Tipo: "+"</summary>
        public static FieldRef FldCodpostit => m_fldCodpostit;
        private static readonly FieldRef m_fldCodpostit = new("postit", "codpostit");

        /// <summary>Campo: "PK" Tipo: "+"</summary>
        public string ValCodpostit
        {
            get { return (string)returnValueField(FldCodpostit); }
            set { insertNameValueField(FldCodpostit, value); }
        }

        /// <summary>Campo: "FK psw" Tipo: "CE"</summary>
        public static FieldRef FldCodpsw => m_fldCodpsw;
        private static readonly FieldRef m_fldCodpsw = new("postit", "codpsw");

        /// <summary>Campo: "FK psw" Tipo: "CE"</summary>
        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        /// <summary>Campo: "FK tabel" Tipo: "CE"</summary>
        public static FieldRef FldCodtabel => m_fldCodtabel;
        private static readonly FieldRef m_fldCodtabel = new("postit", "codtabel");

        /// <summary>Campo: "FK tabel" Tipo: "CE"</summary>
        public string ValCodtabel
        {
            get { return (string)returnValueField(FldCodtabel); }
            set { insertNameValueField(FldCodtabel, value); }
        }

        /// <summary>Campo: "FK postit" Tipo: "CE"</summary>
        public static FieldRef FldCodpost1 => m_fldCodpost1;
        private static readonly FieldRef m_fldCodpost1 = new("postit", "codpost1");

        /// <summary>Campo: "FK postit" Tipo: "CE"</summary>
        public string ValCodpost1
        {
            get { return (string)returnValueField(FldCodpost1); }
            set { insertNameValueField(FldCodpost1, value); }
        }

        /// <summary>Campo: "Tabela" Tipo: "C"</summary>
        public static FieldRef FldTabela => m_fldTabela;
        private static readonly FieldRef m_fldTabela = new("postit", "tabela");

        /// <summary>Campo: "Tabela" Tipo: "C"</summary>
        public string ValTabela
        {
            get { return (string)returnValueField(FldTabela); }
            set { insertNameValueField(FldTabela, value); }
        }

        /// <summary>Campo: "Postit - text" Tipo: "C"</summary>
        public static FieldRef FldPostit => m_fldPostit;
        private static readonly FieldRef m_fldPostit = new("postit", "postit");

        /// <summary>Campo: "Postit - text" Tipo: "C"</summary>
        public string ValPostit
        {
            get { return (string)returnValueField(FldPostit); }
            set { insertNameValueField(FldPostit, value); }
        }

        /// <summary>Campo: "Postit - tipo" Tipo: "C"</summary>
        public static FieldRef FldTpostit => m_fldTpostit;
        private static readonly FieldRef m_fldTpostit = new("postit", "tpostit");

        /// <summary>Campo: "Postit - tipo" Tipo: "C"</summary>
        public string ValTpostit
        {
            get { return (string)returnValueField(FldTpostit); }
            set { insertNameValueField(FldTpostit, value); }
        }

        /// <summary>Campo: "Criação: Date" Tipo: "OD"</summary>
        public static FieldRef FldDatacria => m_fldDatacria;
        private static readonly FieldRef m_fldDatacria = new("postit", "datacria");

        /// <summary>Campo: "Criação: Date" Tipo: "OD"</summary>
        public DateTime ValDatacria
        {
            get { return (DateTime)returnValueField(FldDatacria); }
            set { insertNameValueField(FldDatacria, value); }
        }

		/// <summary>Field : "Criado por" Tipo: "ON" </summary>
		public static FieldRef FldOpercria { get { return m_fldOpercria; } }
		private static FieldRef m_fldOpercria = new FieldRef("postit", "opercria");

		/// <summary>Field : "Criado por" Tipo: "ON"</summary>
		public string ValOpercria
		{
			get { return (string)returnValueField(FldOpercria); }
			set { insertNameValueField(FldOpercria, value); }
		}

        /// <summary>Campo: "Lido" Tipo: "DT"</summary>
        public static FieldRef FldLido => m_fldLido;
        private static readonly FieldRef m_fldLido = new("postit", "lido");

        /// <summary>Campo: "Lido" Tipo: "DT"</summary>
        public DateTime ValLido
        {
            get { return (DateTime)returnValueField(FldLido); }
            set { insertNameValueField(FldLido, value); }
        }

        /// <summary>Campo: "Apagado" Tipo: "DT"</summary>
        public static FieldRef FldApagado => m_fldApagado;
        private static readonly FieldRef m_fldApagado = new("postit", "apagado");

        /// <summary>Campo: "Apagado" Tipo: "DT"</summary>
        public DateTime ValApagado
        {
            get { return (DateTime)returnValueField(FldApagado); }
            set { insertNameValueField(FldApagado, value); }
        }

        /// <summary>Campo: "Validade" Tipo: "DT"</summary>
        public static FieldRef FldValidade => m_fldValidade;
        private static readonly FieldRef m_fldValidade = new("postit", "validade");

        /// <summary>Campo: "Validade" Tipo: "DT"</summary>
        public DateTime ValValidade
        {
            get { return (DateTime)returnValueField(FldValidade); }
            set { insertNameValueField(FldValidade, value); }
        }

		/// <summary>Field : "Recipient" Tipo: "C" </summary>
		public static FieldRef FldRecipient { get { return m_fldRecipient; } }
		private static FieldRef m_fldRecipient = new FieldRef("postit", "recipient");

		/// <summary>Field : "Recipient" Tipo: "C"</summary>
		public string ValRecipient
		{
			get { return (string)returnValueField(FldRecipient); }
			set { insertNameValueField(FldRecipient, value); }
		}

        /// <summary>Campo: "ZZSTATE" Tipo: "N"</summary>
        public static FieldRef FldZzstate => m_fldZzstate;
        private static readonly FieldRef m_fldZzstate = new("postit", "zzstate");

        /// <summary>Campo: "ZZSTATE" Tipo: "N"</summary>
        public int ValZzstate
        {
            get { return (int)returnValueField(FldZzstate); }
            set { insertNameValueField(FldZzstate, value); }
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
        public static List<CSGenioApostit> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields = null, bool distinct = false, bool noLock = false)
        {
				return sp.searchListWhere<CSGenioApostit>(where, user, fields, distinct, noLock);
        }

	}
}
