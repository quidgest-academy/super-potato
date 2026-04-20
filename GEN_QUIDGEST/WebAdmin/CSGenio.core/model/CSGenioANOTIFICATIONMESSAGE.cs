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
	public class CSGenioAnotificationmessage : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAnotificationmessage(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAnotificationmessage(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "notificationmessage";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codmesgs";
            info.HumanKeyName = "assunto";
			info.ShadowTabKeyName = "";
			info.Alias = "notificationmessage";
			info.IsDomain =  true;
			info.AreaDesignation = "Mensagem da notificação";
			info.AreaPluralDesignation = "Mensagens da notificação";
			info.DescriptionCav = "Mensagem da notificação";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
            info.RegisterFieldDB(new Field(info.Alias, "codmesgs", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "codsigna", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "codpmail", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "codtpnot", FieldType.KEY_INT));
            info.RegisterFieldDB(new Field(info.Alias, "coddestn", FieldType.KEY_INT));

			info.RegisterFieldDB(new Field(info.Alias, "from", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "to", FieldType.TEXT));
	        info.RegisterFieldDB(new Field(info.Alias, "destnman", FieldType.LOGIC));
            info.RegisterFieldDB(new Field(info.Alias, "tomanual", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "cc", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "bcc", FieldType.TEXT));

			info.RegisterFieldDB(new Field(info.Alias, "idnotif", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "notifica", FieldType.LOGIC));
            info.RegisterFieldDB(new Field(info.Alias, "email", FieldType.LOGIC));
			info.RegisterFieldDB(new Field(info.Alias, "assunto", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "agregado", FieldType.LOGIC));
			info.RegisterFieldDB(new Field(info.Alias, "anexo", FieldType.LOGIC));
			
			info.RegisterFieldDB(new Field(info.Alias, "ativo", FieldType.LOGIC));
			info.RegisterFieldDB(new Field(info.Alias, "designac", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "mensagem", FieldType.MEMO));
			info.RegisterFieldDB(new Field(info.Alias, "gravabd", FieldType.LOGIC));
			info.RegisterFieldDB(new Field(info.Alias, "html", FieldType.LOGIC));
			
			info.RegisterFieldDB(new Field(info.Alias, "opercria", FieldType.TEXT));
			info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS));
            info.RegisterFieldDB(new Field(info.Alias, "opermuda", FieldType.TEXT));
            info.RegisterFieldDB(new Field(info.Alias, "datamuda", FieldType.DATETIMESECONDS));
			info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

            // Carimbos automáticos na BD
            //------------------------------
   			info.StampFieldsIns = new string[] {
			 "opercria","datacria"
			};
  			info.StampFieldsIns = new string[] {
			 "opermuda","datamuda"
			};

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

		// USE /[MANUAL FOR TABAUX notificationmessage]/

		
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }
		
		public class FinalMsg
        {
            /// <summary>
            /// Message identifier
            /// </summary>
            public String ID { get; set; }
            /// <summary>
            /// Subject 
            /// </summary>
            public String Subject { get; set; }
            /// <summary>
            /// Original Message 
            /// </summary>
            public String Message { get; set; }
            /// <summary>
            /// Primary key of the message Destination 
            /// </summary>
            public String Destination_PK { get; set; }
            /// <summary>
            /// Email of the message Destination 
            /// </summary>
            public String Destination_EMAIL { get; set; }
            /// <summary>
            /// True if the message was sent
            /// </summary>
            public bool Sent { get; set; }
            /// <summary>
            /// Error message received while trying to send
            /// </summary>
            public String ErrorMessage { get; set; }
            /// <summary>
            /// Querydata rowID responsable for this message 
            /// </summary>
            public String QueryDataID { get; set; }
        }
        private List<FinalMsg> _FinalMsgs = new List<FinalMsg>();
        public List<FinalMsg> FinalMsgs { get { return _FinalMsgs; } set { _FinalMsgs = value; } }
		
         /// <summary>Campo : "PK da tabela MESGS" Tipo: "+" Formula:  ""</summary>
        public static FieldRef FldCodmesgs { get { return m_fldCodmesgs; } }
        private static FieldRef m_fldCodmesgs = new FieldRef("notificationmessage", "codmesgs");

        /// <summary>Campo : "PK da tabela MESGS" Tipo: "+" Formula:  ""</summary>
        public string ValCodmesgs
        {
            get { return (string)returnValueField(FldCodmesgs); }
            set { insertNameValueField(FldCodmesgs, value); }
        }

		/// <summary>Campo : "Chave da tabela 'Assinatura'" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCodsigna { get { return m_fldCodsigna; } }
        private static FieldRef m_fldCodsigna = new FieldRef("notificationmessage", "codsigna");

        /// <summary>Campo : "Chave da tabela 'Assinatura'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodsigna
        {
            get { return (string)returnValueField(FldCodsigna); }
            set { insertNameValueField(FldCodsigna, value); }
        }

        /// <summary>Campo : "Chave da tabela 'Propriedades de envio de emails'" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCodpmail { get { return m_fldCodpmail; } }
        private static FieldRef m_fldCodpmail = new FieldRef("notificationmessage", "codpmail");

        /// <summary>Campo : "Chave da tabela 'Propriedades de envio de emails'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodpmail
        {
            get { return (string)returnValueField(FldCodpmail); }
            set { insertNameValueField(FldCodpmail, value); }
        }


		/// <summary>Campo : "Remetente"</summary>
        public static FieldRef FldFrom { get { return m_fldFrom; } }
        private static FieldRef m_fldFrom = new FieldRef("notificationmessage", "from");

        /// <summary>Campo : "From (formula + das propriedades de envio)"</summary>
        public string ValFrom
        {
            get { return (string)returnValueField(FldFrom); }
            set { insertNameValueField(FldFrom, value); }
        }


        /// <summary>Campo : "Chave para a tabela 'destinatários possíveis'" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCoddestn { get { return m_fldCoddestn; } }
        private static FieldRef m_fldCoddestn = new FieldRef("notificationmessage", "coddestn");

        /// <summary>Campo : "Chave para a tabela 'destinatários possíveis'" Tipo: "CE" Formula:  ""</summary>
        public string ValCoddestn
        {
            get { return (string)returnValueField(FldCoddestn); }
            set { insertNameValueField(FldCoddestn, value); }
        }

        /// <summary>Campo : "Destinatário"</summary>
        public static FieldRef FldTo { get { return m_fldTo; } }
        private static FieldRef m_fldTo = new FieldRef("notificationmessage", "to");

        /// <summary>Campo : "Destinatário (formula + dos destinatários)"</summary>
        public string ValTo
        {
            get { return (string)returnValueField(FldTo); }
            set { insertNameValueField(FldTo, value); }
        }

		/// <summary>Campo : "Destinatário manual?" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldDestnman { get { return m_fldDestnman; } }
        private static FieldRef m_fldDestnman = new FieldRef("notificationmessage", "destnman");

        /// <summary>Campo : "Destinatário manual?" Tipo: "L" Formula:  ""</summary>
        public int ValDestnman
        {
            get { return (int)returnValueField(FldDestnman); }
            set { insertNameValueField(FldDestnman, value); }
        }

        /// <summary>Campo : "Destinatário manual"</summary>
        public static FieldRef FldTomanual { get { return m_fldTomanual; } }
        private static FieldRef m_fldTomanual = new FieldRef("notificationmessage", "tomanual");

        /// <summary>Campo : "Destinatário manual"</summary>
        public string ValTomanual
        {
            get { return (string)returnValueField(FldTomanual); }
            set { insertNameValueField(FldTomanual, value); }
        }

        /// <summary>Campo : "Cc"</summary>
        public static FieldRef FldCc { get { return m_fldCc; } }
        private static FieldRef m_fldCc = new FieldRef("notificationmessage", "cc");

        /// <summary>Campo : "Cc"</summary>
        public string ValCc
        {
            get { return (string)returnValueField(FldCc); }
            set { insertNameValueField(FldCc, value); }
        }
        /// <summary>Campo : "Bcc"</summary>
        public static FieldRef FldBcc { get { return m_fldBcc; } }
        private static FieldRef m_fldBcc = new FieldRef("notificationmessage", "bcc");

        /// <summary>Campo : "Bcc"</summary>
        public string ValBcc
        {
            get { return (string)returnValueField(FldBcc); }
            set { insertNameValueField(FldBcc, value); }
        }

		
        /// <summary>Campo : "Chave para a tabela 'Tipos de notificações'" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCodtpnot { get { return m_fldCodtpnot; } }
        private static FieldRef m_fldCodtpnot = new FieldRef("notificationmessage", "codtpnot");

        /// <summary>Campo : "Chave para a tabela 'Tipos de notificações'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodtpnot
        {
            get { return (string)returnValueField(FldCodtpnot); }
            set { insertNameValueField(FldCodtpnot, value); }
        }

        /// <summary>Campo : "Chave para a tabela 'Tipos de notificações'" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldIdnotif { get { return m_FldIdnotif; } }
        private static FieldRef m_FldIdnotif = new FieldRef("notificationmessage", "idnotif");

        /// <summary>Campo : "Chave para a tabela 'Tipos de notificações'" Tipo: "CE" Formula:  ""</summary>
        public string ValIdnotif
        {
            get { return (string)returnValueField(FldIdnotif); }
            set { insertNameValueField(FldIdnotif, value); }
        }
		
        /// <summary>Campo : "Disponibiliza notificação no portal" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldNotifica { get { return m_fldNotifica; } }
        private static FieldRef m_fldNotifica = new FieldRef("notificationmessage", "notifica");

        /// <summary>Campo : "Disponibiliza notificação no portal" Tipo: "L" Formula:  ""</summary>
        public int ValNotifica
        {
            get { return (int)returnValueField(FldNotifica); }
            set { insertNameValueField(FldNotifica, value); }
        }


        /// <summary>Campo : "Envia e-mail" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldEmail { get { return m_fldEmail; } }
        private static FieldRef m_fldEmail = new FieldRef("notificationmessage", "email");

        /// <summary>Campo : "Envia e-mail" Tipo: "L" Formula:  ""</summary>
        public int ValEmail
        {
            get { return (int)returnValueField(FldEmail); }
            set { insertNameValueField(FldEmail, value); }
        }


        /// <summary>Campo : "Assunto" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldAssunto { get { return m_fldAssunto; } }
        private static FieldRef m_fldAssunto = new FieldRef("notificationmessage", "assunto");

        /// <summary>Campo : "Assunto" Tipo: "C" Formula:  ""</summary>
        public string ValAssunto
        {
            get { return (string)returnValueField(FldAssunto); }
            set { insertNameValueField(FldAssunto, value); }
        }


        /// <summary>Campo : "Agregado" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldAgregado { get { return m_fldAgregado; } }
        private static FieldRef m_fldAgregado = new FieldRef("notificationmessage", "agregado");

        /// <summary>Campo : "Agregado" Tipo: "L" Formula:  ""</summary>
        public int ValAgregado
        {
            get { return (int)returnValueField(FldAgregado); }
            set { insertNameValueField(FldAgregado, value); }
        }


        /// <summary>Campo : "Envia anexo?" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldAnexo { get { return m_fldAnexo; } }
        private static FieldRef m_fldAnexo = new FieldRef("notificationmessage", "anexo");

        /// <summary>Campo : "Envia anexo?" Tipo: "L" Formula:  ""</summary>
        public int ValAnexo
        {
            get { return (int)returnValueField(FldAnexo); }
            set { insertNameValueField(FldAnexo, value); }
        }


        /// <summary>Campo : "Chave da tabela 'Periodicidades'" Tipo: "CE" Formula:  ""</summary>
        public static FieldRef FldCodpperi { get { return m_fldCodpperi; } }
        private static FieldRef m_fldCodpperi = new FieldRef("notificationmessage", "codpperi");

        /// <summary>Campo : "Chave da tabela 'Periodicidades'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodpperi
        {
            get { return (string)returnValueField(FldCodpperi); }
            set { insertNameValueField(FldCodpperi, value); }
        }


        /// <summary>Campo : "Formato HTML" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldHtml { get { return m_fldHtml; } }
        private static FieldRef m_fldHtml = new FieldRef("notificationmessage", "html");

        /// <summary>Campo : "Formato HTML" Tipo: "L" Formula:  ""</summary>
        public int ValHtml
        {
            get { return (int)returnValueField(FldHtml); }
            set { insertNameValueField(FldHtml, value); }
        }


		/// <summary>Campo : "Ativo" Tipo: "AC" Formula:  ""</summary>
        public static FieldRef FldAtivo { get { return m_fldAtivo; } }
        private static FieldRef m_fldAtivo = new FieldRef("notificationmessage", "ativo");

        /// <summary>Campo : "Ativo" Tipo: "L" Formula:  ""</summary>
        public int ValAtivo
        {
            get { return (int)returnValueField(FldAtivo); }
            set { insertNameValueField(FldAtivo, value); }
        }


        /// <summary>Campo : "Configuração da mensagem" Tipo: "C" Formula:  ""</summary>
        public static FieldRef FldDesignac { get { return m_fldDesignac; } }
        private static FieldRef m_fldDesignac = new FieldRef("notificationmessage", "designac");

        /// <summary>Campo : "Configuração da mensagem" Tipo: "C" Formula:  ""</summary>
        public string ValDesignac
        {
            get { return (string)returnValueField(FldDesignac); }
            set { insertNameValueField(FldDesignac, value); }
        }


        /// <summary>Campo : "Mensagem" Tipo: "MO" Formula:  ""</summary>
        public static FieldRef FldMensagem { get { return m_fldMensagem; } }
        private static FieldRef m_fldMensagem = new FieldRef("notificationmessage", "mensagem");

        /// <summary>Campo : "Mensagem" Tipo: "MO" Formula:  ""</summary>
        public string ValMensagem
        {
            get { return (string)returnValueField(FldMensagem); }
            set { insertNameValueField(FldMensagem, value); }
        }

		/// <summary>Campo : "Grava na BD" Tipo: "L" Formula:  ""</summary>
        public static FieldRef FldGravabd { get { return m_fldGravabd; } }
        private static FieldRef m_fldGravabd = new FieldRef("notificationmessage", "gravabd");

        /// <summary>Campo : "Grava na BD" Tipo: "L" Formula:  ""</summary>
        public int ValGravabd
        {
            get { return (int)returnValueField(FldGravabd); }
            set { insertNameValueField(FldGravabd, value); }
        }

        /// <summary>Campo : "Alteração: Data" Tipo: "ED" Formula:  ""</summary>
        public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
        private static FieldRef m_fldDatamuda = new FieldRef("notificationmessage", "datamuda");

        /// <summary>Campo : "Alteração: Data" Tipo: "ED" Formula:  ""</summary>
        public DateTime ValDatamuda
        {
            get { return (DateTime)returnValueField(FldDatamuda); }
            set { insertNameValueField(FldDatamuda, value); }
        }


        /// <summary>Campo : "Criação: Data" Tipo: "OD" Formula:  ""</summary>
        public static FieldRef FldDatacria { get { return m_fldDatacria; } }
        private static FieldRef m_fldDatacria = new FieldRef("notificationmessage", "datacria");

        /// <summary>Campo : "Criação: Data" Tipo: "OD" Formula:  ""</summary>
        public DateTime ValDatacria
        {
            get { return (DateTime)returnValueField(FldDatacria); }
            set { insertNameValueField(FldDatacria, value); }
        }


        /// <summary>Campo : "Criação: Operador" Tipo: "ON" Formula:  ""</summary>
        public static FieldRef FldOpercria { get { return m_fldOpercria; } }
        private static FieldRef m_fldOpercria = new FieldRef("notificationmessage", "opercria");

        /// <summary>Campo : "Criação: Operador" Tipo: "ON" Formula:  ""</summary>
        public string ValOpercria
        {
            get { return (string)returnValueField(FldOpercria); }
            set { insertNameValueField(FldOpercria, value); }
        }


        /// <summary>Campo : "Alteração: Operador" Tipo: "EN" Formula:  ""</summary>
        public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
        private static FieldRef m_fldOpermuda = new FieldRef("notificationmessage", "opermuda");

        /// <summary>Campo : "Alteração: Operador" Tipo: "EN" Formula:  ""</summary>
        public string ValOpermuda
        {
            get { return (string)returnValueField(FldOpermuda); }
            set { insertNameValueField(FldOpermuda, value); }
        }


        /// <summary>Campo : "ZZSTATE" Tipo: "INT" Formula:  ""</summary>
        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("notificationmessage", "zzstate");



        /// <summary>Campo : "ZZSTATE" Tipo: "INT"</summary>
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
        public static CSGenioAnotificationmessage search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            if (string.IsNullOrEmpty(key)) //para proteger chamadas "cegas"
                return null;
            CSGenioAnotificationmessage area = new CSGenioAnotificationmessage(user, user.CurrentModule);
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
        public static List<CSGenioAnotificationmessage> searchList(PersistentSupport sp, User User, CriteriaSet where)
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
        public static List<CSGenioAnotificationmessage> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos)
        {
            return sp.searchListWhere<CSGenioAnotificationmessage>(where, User, campos);
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
        public static List<CSGenioAnotificationmessage> searchList(PersistentSupport sp, User User, CriteriaSet where, string[] campos, bool distinct, bool noLock = false)
        {
            return sp.searchListWhere<CSGenioAnotificationmessage>(where, User, campos, distinct, noLock);
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User User, CriteriaSet where, ListingMVC<CSGenioAnotificationmessage> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAnotificationmessage>(where, listing);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condição
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="User">O contexto do User</param>
        /// <param name="where">A condição de procura dos registos. Usar null para obter todos os registos</param>
        /// <param name="distinct">Obter distinct de campos</param>
        /// <param name="noLock">NOLOCK</param>
        /// <returns>Uma lista de registos da areas com todos os campos preenchidos</returns>
        public static List<CSGenioAnotificationmessage> searchList(PersistentSupport sp, User User, CriteriaSet where, bool distinct, bool noLock = false)
        {
            return searchList(sp, User, where, null, distinct, noLock);
        }

	

	}
}
