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
	public class CSGenioAmqqueues : DbArea
	{
	    /// <summary>
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAmqqueues(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAmqqueues(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "formqqueues";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codmqqueues";
            info.HumanKeyName = "codmqqueues";
			info.ShadowTabKeyName = "";
			info.Alias = "mqqueues";
			info.IsDomain =  true;
			info.AreaDesignation = "Queue";
			info.AreaPluralDesignation = "Queues";
			info.DescriptionCav = "Queue";
			
			//sincronizaÁ„o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
      info.RegisterFieldDB(new Field(info.Alias, "codmqqueues", FieldType.KEY_INT));
	  info.RegisterFieldDB(new Field(info.Alias, "queueid", FieldType.TEXT));
      info.RegisterFieldDB(new Field(info.Alias, "channelid", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "ano", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "username", FieldType.TEXT));
 	  info.RegisterFieldDB(new Field(info.Alias, "tabela", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "tabelacod", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "queuekey", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "queue", FieldType.IMAGE));
	  info.RegisterFieldDB(new Field(info.Alias, "mqstatus", FieldType.TEXT));	
	  info.RegisterFieldDB(new Field(info.Alias, "datastatus", FieldType.DATETIME));
	  info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS));
	  info.RegisterFieldDB(new Field(info.Alias, "operacao", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "resposta", FieldType.TEXT));
	  info.RegisterFieldDB(new Field(info.Alias, "sendnumber", FieldType.INTEGER));
	  info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

			// RelaÁıes Filhas
			//------------------------------

			// RelaÁıes M„e
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
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>
		public override AreaInfo Information
		{
			get { return informacao; }
		}
		/// <summary>
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>		
		public static AreaInfo GetInformation()
		{
			return informacao;
		}

		// USE /[MANUAL FOR TABAUX MQQUEUES]/

		        public static FieldRef FldCodmqqueues { get { return m_FldCodmqqueues; } }
        private static FieldRef m_FldCodmqqueues = new FieldRef("mqqueues", "codmqqueues");

        public string ValCodmqqueues
        {
            get { return (string)returnValueField(FldCodmqqueues); }
            set { insertNameValueField(FldCodmqqueues, value); }
        }

        public static FieldRef FldQueueID { get { return m_FldQueueID; } }
        private static FieldRef m_FldQueueID = new FieldRef("mqqueues", "queueid");

        public string ValQueueID
        {
            get { return (string)returnValueField(FldQueueID); }
            set { insertNameValueField(FldQueueID, value); }
        }

        public static FieldRef FldChannelID { get { return m_FldChannelID; } }
        private static FieldRef m_FldChannelID = new FieldRef("mqqueues", "channelid");

        public string ValChannelID
        {
            get { return (string)returnValueField(FldChannelID); }
            set { insertNameValueField(FldChannelID, value); }
        }

        public static FieldRef FldAno { get { return m_FldAno; } }
        private static FieldRef m_FldAno = new FieldRef("mqqueues", "ano");

        public string ValAno
        {
            get { return (string)returnValueField(FldAno); }
            set { insertNameValueField(FldAno, value); }
        }

        public static FieldRef FldUsername { get { return m_FldUsername; } }
        private static FieldRef m_FldUsername = new FieldRef("mqqueues", "username");

        public string ValUsername
        {
            get { return (string)returnValueField(FldUsername); }
            set { insertNameValueField(FldUsername, value); }
        }

        public static FieldRef FldTabela { get { return m_FldTabela; } }
        private static FieldRef m_FldTabela = new FieldRef("mqqueues", "tabela");

        public string ValTabela
        {
            get { return (string)returnValueField(FldTabela); }
            set { insertNameValueField(FldTabela, value); }
        }

        public static FieldRef FldTabelaCod { get { return m_FldTabelaCod; } }
        private static FieldRef m_FldTabelaCod = new FieldRef("mqqueues", "tabelacod");

        public string ValTabelaCod
        {
            get { return (string)returnValueField(FldTabelaCod); }
            set { insertNameValueField(FldTabelaCod, value); }
        }

        public static FieldRef FldQueueKey { get { return m_FldQueueKey; } }
        private static FieldRef m_FldQueueKey = new FieldRef("mqqueues", "queuekey");

        public string ValQueueKey
        {
            get { return (string)returnValueField(FldQueueKey); }
            set { insertNameValueField(FldQueueKey, value); }
        }

        public static FieldRef FldQueue { get { return m_FldQueue; } }
        private static FieldRef m_FldQueue = new FieldRef("mqqueues", "queue");

        public byte[] ValQueue
        {
            get { return (byte[])returnValueField(FldQueue); }
            set { insertNameValueField(FldQueue, value); }
        }

        public static FieldRef FldMQStatus { get { return m_FldMQStatus; } }
        private static FieldRef m_FldMQStatus = new FieldRef("mqqueues", "mqstatus");

        public string ValMQStatus
        {
            get { return (string)returnValueField(FldMQStatus); }
            set { insertNameValueField(FldMQStatus, value); }
        }

        public static FieldRef FldDataStatus { get { return m_FldDataStatus; } }
        private static FieldRef m_FldDataStatus = new FieldRef("mqqueues", "datastatus");

        public DateTime ValDataStatus
        {
            get { return (DateTime)returnValueField(FldDataStatus); }
            set { insertNameValueField(FldDataStatus, value); }
        }

        public static FieldRef FldDatacria { get { return m_FldDatacria; } }
        private static FieldRef m_FldDatacria = new FieldRef("mqqueues", "datacria");

        public DateTime ValDatacria
        {
            get { return (DateTime)returnValueField(FldDatacria); }
            set { insertNameValueField(FldDatacria, value); }
        }
		
        public static FieldRef FldOperacao { get { return m_FldOperacao; } }
        private static FieldRef m_FldOperacao = new FieldRef("mqqueues", "operacao");

        public string ValOperacao
        {
            get { return (string)returnValueField(FldOperacao); }
            set { insertNameValueField(FldOperacao, value); }
        }

        public static FieldRef FldResposta { get { return m_FldResposta; } }
        private static FieldRef m_FldResposta = new FieldRef("mqqueues", "resposta");

        public string ValResposta
        {
            get { return (string)returnValueField(FldResposta); }
            set { insertNameValueField(FldResposta, value); }
        }
		
        public static FieldRef FldSendnumber { get { return m_FldSendnumber; } }
        private static FieldRef m_FldSendnumber = new FieldRef("mqqueues", "sendnumber");

        public int ValSendnumber
        {
            get { return (int)returnValueField(FldSendnumber); }
            set { insertNameValueField(FldSendnumber, value); }
        }
		
        public static FieldRef FldZzstate { get { return m_FldZzstate; } }
        private static FieldRef m_FldZzstate = new FieldRef("mqqueues", "zzstate");

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
        public static CSGenioAmqqueues search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioAmqqueues area = new CSGenioAmqqueues(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAmqqueues> searchList(PersistentSupport sp, User user, CriteriaSet where)
        {
            return searchList(sp, user, where, null);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N√£o devem ser utilizadas opera√ß√µes de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAmqqueues> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields)
        {
            return sp.searchListWhere<CSGenioAmqqueues>(where, user, fields);
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√É¬ß√É¬£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N√£o devem ser utilizadas opera√ß√µes de persistence sobre um registo parcialmente posicionado</remarks>
        public static List<CSGenioAmqqueues> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields, bool distinct)
        {
            return sp.searchListWhere<CSGenioAmqqueues>(where, user, fields, distinct);			
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAmqqueues> searchList(PersistentSupport sp, User user, CriteriaSet where, bool distinct)
        {
            return searchList(sp, user, where, null, distinct);
        }
		
		/// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="campos">Os fields a serem preenchidos na area</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        /// <remarks>N√£o devem ser utilizadas opera√ß√µes de persistence sobre um registo parcialmente posicionado</remarks>
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAmqqueues> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAmqqueues>(where, listing);
        }

	}
}
