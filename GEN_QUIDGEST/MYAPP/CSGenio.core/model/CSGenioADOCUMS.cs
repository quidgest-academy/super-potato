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
	public class CSGenioAdocums : DbArea
	{
	    /// <summary>
		/// Meta-informaÁ„o sobre esta ‡rea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAdocums(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAdocums(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "docums";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "coddocums";
            info.HumanKeyName = "coddocums";
			info.ShadowTabKeyName = "";
			info.Alias = "docums";
			info.IsDomain =  true;
			info.AreaDesignation = "Documento na BD";
			info.AreaPluralDesignation = "Documentos na BD";
			info.DescriptionCav = "Documento na BD";
			
			//sincronizaÁ„o
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
            info.RegisterFieldDB(new Field(info.Alias, "coddocums", FieldType.KEY_INT));
    		info.RegisterFieldDB(new Field(info.Alias, "documid", FieldType.KEY_INT));
    		info.RegisterFieldDB(new Field(info.Alias, "document", FieldType.BINARY));
            info.RegisterFieldDB(new Field(info.Alias, "docpath", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "nome", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "tabela", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "campo", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "chave", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "versao", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "tamanho", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "extensao", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "opercria", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "datacria", FieldType.DATETIMESECONDS));
    		info.RegisterFieldDB(new Field(info.Alias, "opermuda", FieldType.TEXT));
    		info.RegisterFieldDB(new Field(info.Alias, "datamuda", FieldType.DATETIMESECONDS));
    		info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));

    		info.DBFields["docpath"].FieldSize = 260;
    		info.DBFields["nome"].FieldSize = 255;
    		info.DBFields["extensao"].FieldSize = 5;
		    info.DBFields["coddocums"].FieldSize = 8;

    		info.StampFieldsIns = new string[] {
                "opercria","datacria"
			};

   			info.StampFieldsAlt = new string[] {
                "opermuda","datamuda"
			};

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

		// USE /[MANUAL FOR TABAUX DOCUMS]/

		        public static FieldRef FldCoddocums { get { return m_fldCoddocums; } }
        private static FieldRef m_fldCoddocums = new FieldRef("docums", "coddocums");

        public string ValCoddocums
        {
            get { return (string)returnValueField(FldCoddocums); }
            set { insertNameValueField(FldCoddocums, value); }
        }

        public static FieldRef FldDocumid { get { return m_fldDocumid; } }
        private static FieldRef m_fldDocumid = new FieldRef("docums", "documid");

        public string ValDocumid
        {
            get { return (string)returnValueField(FldDocumid); }
            set { insertNameValueField(FldDocumid, value); }
        }

        public static FieldRef FldDocument { get { return m_fldDocument; } }
        private static FieldRef m_fldDocument = new FieldRef("docums", "document");

        public byte[] ValDocument
        {
            get { return (byte[])returnValueField(FldDocument); }
            set { insertNameValueField(FldDocument, value); }
        }

        public static FieldRef FldDocpath { get { return m_fldDocpath; } }
        private static FieldRef m_fldDocpath = new FieldRef("docums", "docpath");

        public string ValDocpath
        {
            get { return (string)returnValueField(FldDocpath); }
            set { insertNameValueField(FldDocpath, value); }
        }

        public static FieldRef FldNome { get { return m_fldNome; } }
        private static FieldRef m_fldNome = new FieldRef("docums", "nome");

        public string ValNome
        {
            get { return (string)returnValueField(FldNome); }
            set { insertNameValueField(FldNome, value); }
        }

        public static FieldRef FldTabela { get { return m_fldTabela; } }
        private static FieldRef m_fldTabela = new FieldRef("docums", "tabela");

        public string ValTabela
        {
            get { return (string)returnValueField(FldTabela); }
            set { insertNameValueField(FldTabela, value); }
        }

        public static FieldRef FldCampo { get { return m_fldCampo; } }
        private static FieldRef m_fldCampo = new FieldRef("docums", "campo");

        public string ValCampo
        {
            get { return (string)returnValueField(FldCampo); }
            set { insertNameValueField(FldCampo, value); }
        }
		
        public static FieldRef FldChave { get { return m_fldChave; } }
        private static FieldRef m_fldChave = new FieldRef("docums", "chave");

        public string ValChave
        {
            get { return (string)returnValueField(FldChave); }
            set { insertNameValueField(FldChave, value); }
        }
        
        public static FieldRef FldVersao { get { return m_fldVersao; } }
        private static FieldRef m_fldVersao = new FieldRef("docums", "versao");

        public string ValVersao
        {
            get { return (string)returnValueField(FldVersao); }
            set { insertNameValueField(FldVersao, value); }
        }

        public static FieldRef FldTamanho { get { return m_fldTamanho; } }
        private static FieldRef m_fldTamanho = new FieldRef("docums", "tamanho");

        public string ValTamanho
        {
            get { return (string)returnValueField(FldTamanho); }
            set { insertNameValueField(FldTamanho, value); }
        }
        public static FieldRef FldExtensao { get { return m_fldExtensao; } }
        private static FieldRef m_fldExtensao = new FieldRef("docums", "extensao");

        public string ValExtensao
        {
            get { return (string)returnValueField(FldExtensao); }
            set { insertNameValueField(FldExtensao, value); }
        }

        public static FieldRef FldOpercria { get { return m_fldOpercria; } }
        private static FieldRef m_fldOpercria = new FieldRef("docums", "opercria");

        public string ValOpercria
        {
            get { return (string)returnValueField(FldOpercria); }
            set { insertNameValueField(FldOpercria, value); }
        }

        public static FieldRef FldDatacria { get { return m_fldDatacria; } }
        private static FieldRef m_fldDatacria = new FieldRef("docums", "datacria");

        public DateTime ValDatacria
        {
            get { return (DateTime)returnValueField(FldDatacria); }
            set { insertNameValueField(FldDatacria, value); }
        }

        public static FieldRef FldOpermuda { get { return m_fldOpermuda; } }
        private static FieldRef m_fldOpermuda = new FieldRef("docums", "opermuda");

        public string ValOpermuda
        {
            get { return (string)returnValueField(FldOpermuda); }
            set { insertNameValueField(FldOpermuda, value); }
        }

        public static FieldRef FldDatamuda { get { return m_fldDatamuda; } }
        private static FieldRef m_fldDatamuda = new FieldRef("docums", "datamuda");

        public DateTime ValDatamuda
        {
            get { return (DateTime)returnValueField(FldDatamuda); }
            set { insertNameValueField(FldDatamuda, value); }
        }

        public static FieldRef FldZzstate { get { return m_fldZzstate; } }
        private static FieldRef m_fldZzstate = new FieldRef("docums", "zzstate");

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
        public static CSGenioAdocums search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioAdocums area = new CSGenioAdocums(user, user.CurrentModule);
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
        public static List<CSGenioAdocums> searchList(PersistentSupport sp, User user, CriteriaSet where)
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
        public static List<CSGenioAdocums> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields)
        {
            return sp.searchListWhere<CSGenioAdocums>(where, user, fields);
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
        public static List<CSGenioAdocums> searchList(PersistentSupport sp, User user, CriteriaSet where, string[] fields, bool distinct)
        {
            return sp.searchListWhere<CSGenioAdocums>(where, user, fields, distinct);			
        }

        /// <summary>
        /// Procura todos os registos desta area que obedecem a uma condi√ß√£o
        /// </summary>
        /// <param name="sp">O suporte persistente de onde obter a lista</param>
        /// <param name="utilizador">O contexto do user</param>
        /// <param name="where">A condi√ß√£o de procura dos registos. Usar null to obter todos os registos</param>
        /// <param name="distinct">Obter distinct de fields</param>
        /// <returns>Uma lista de registos da areas com todos os fields preenchidos</returns>
        public static List<CSGenioAdocums> searchList(PersistentSupport sp, User user, CriteriaSet where, bool distinct)
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
        public static void searchListAdvancedWhere(PersistentSupport sp, User user, CriteriaSet where, ListingMVC<CSGenioAdocums> listing)
        {
            sp.searchListAdvancedWhere<CSGenioAdocums>(where, listing);
        }

	}
}
