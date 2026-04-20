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
	public class CSGenioAusrset : DbArea
	{
	    /// <summary>
		/// Meta-informação sobre esta àrea
		/// </summary>
		protected static AreaInfo informacao = InicializaAreaInfo();

		public CSGenioAusrset(User user,string module)
		{
            this.user = user;
            this.module = module;
		}
	
		public CSGenioAusrset(User user) : this(user, user.CurrentModule)
		{
		}
	
		private static AreaInfo InicializaAreaInfo()
		{
			AreaInfo info = new AreaInfo();
			
			/*Information das areas*/
			info.TableName = "forusrset";
			info.ShadowTabName = "";
			info.PrimaryKeyName = "codusrset";
            info.HumanKeyName = "codusrset";
			info.ShadowTabKeyName = "";
			info.Alias = "usrset";
			info.IsDomain =  true;
			info.AreaDesignation = "Configuração de user";
			info.AreaPluralDesignation = "Configurações de user";
			info.DescriptionCav = "Configuração de user";
			
			//sincronização
			info.SyncIncrementalDateStart = TimeSpan.FromHours(9.0);
			info.SyncIncrementalDateEnd = TimeSpan.FromHours(23.0);
			info.SyncCompleteHour = TimeSpan.FromHours(1.0);
			info.SyncIncrementalPeriod = TimeSpan.FromHours(1);
			info.BatchSync = 100;
			info.SyncType = SyncType.Central;
					
        info.RegisterFieldDB(new Field(info.Alias, "codusrset", FieldType.KEY_INT));
		info.RegisterFieldDB(new Field(info.Alias, "modulo", FieldType.TEXT));
		info.RegisterFieldDB(new Field(info.Alias, "codpsw", FieldType.KEY_INT));

        info.RegisterFieldDB(new Field(info.Alias, "chave", FieldType.TEXT));
		info.RegisterFieldDB(new Field(info.Alias, "valor", FieldType.TEXT));
		info.RegisterFieldDB(new Field(info.Alias, "zzstate", FieldType.INTEGER));


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

		// USE /[MANUAL FOR TABAUX USRSET]/

		        public static FieldRef FldCodusrset { get { return m_FldCodusrset; } }
        private static FieldRef m_FldCodusrset = new FieldRef("usrset", "codusrset");

        public string ValCodusrset
        {
            get { return (string)returnValueField(FldCodusrset); }
            set { insertNameValueField(FldCodusrset, value); }
        }

        public static FieldRef FldModulo { get { return m_FldModulo; } }
        private static FieldRef m_FldModulo = new FieldRef("usrset", "modulo");

        public string ValModulo
        {
            get { return (string)returnValueField(FldModulo); }
            set { insertNameValueField(FldModulo, value); }
        }

        public static FieldRef FldCodpsw { get { return m_FldCodpsw; } }
        private static FieldRef m_FldCodpsw = new FieldRef("usrset", "codpsw");

        public string ValCodpsw
        {
            get { return (string)returnValueField(FldCodpsw); }
            set { insertNameValueField(FldCodpsw, value); }
        }

        public static FieldRef FldChave { get { return m_FldChave; } }
        private static FieldRef m_FldChave = new FieldRef("usrset", "chave");

        public string ValChave
        {
            get { return (string)returnValueField(FldChave); }
            set { insertNameValueField(FldChave, value); }
        }

        public static FieldRef FldValor { get { return m_FldValor; } }
        private static FieldRef m_FldValor = new FieldRef("usrset", "valor");

        public string ValValor
        {
            get { return (string)returnValueField(FldValor); }
            set { insertNameValueField(FldValor, value); }
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
        public static CSGenioAusrset search(PersistentSupport sp, string key, User user, string[] fields = null)
        {
            CSGenioAusrset area = new CSGenioAusrset(user, user.CurrentModule);
            if (sp.getRecord(area, key, fields))
                return area;
            return null;
        }


	}
}
