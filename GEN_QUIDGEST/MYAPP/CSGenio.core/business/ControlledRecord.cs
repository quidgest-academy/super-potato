using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace CSGenio.business
{
    /// <summary>
    /// Modos de criação de registos controlados
    /// </summary>
    public enum ControlledRecordMode
    {
        Automatico,
        Form,
        Erro
    }
	
	
	public class ControlledRecords
    {
        /// <summary>
        /// Table name
        /// </summary>
        public string l_table;

        /// <summary>
        /// Name of the primary Key field
        /// </summary>
        public string l_primaryKeyField;

        /// <summary>
        /// "Semaphore" to manage dictionary access
        /// </summary>
        private ReaderWriterLockSlim l_lockControlledRecords = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// List of controlled records
        /// </summary>
        private Dictionary<string, ControlledRecord> m_registosControlados = new Dictionary<string, ControlledRecord>();


        public ControlledRecords(string table, string primaryKeyField)
        {
            l_table = table;
            l_primaryKeyField = primaryKeyField;
        }

        // Add a controlled record to the dictionary
        /// <summary>
        /// Add a controlled record to the dictionary
        /// </summary>
        public void addControlledRecord(string key, ControlledRecord record)
        {

            l_lockControlledRecords.EnterWriteLock();
            if (m_registosControlados.ContainsKey(key))
                m_registosControlados[key] = record;
            else
                m_registosControlados.Add(key, record);
            l_lockControlledRecords.ExitWriteLock();

        }

        // Find the controlled record with the key and return it
        /// <summary>
        /// Find the controlled record with the key and return it.
        /// </summary>
        /// <returns>
        /// Controlled record founded.
        /// </returns>
        public ControlledRecord getControlledRecord(string key)
        {
            ControlledRecord record = null;

            l_lockControlledRecords.EnterReadLock();
            m_registosControlados.TryGetValue(key, out record);
            l_lockControlledRecords.ExitReadLock();
            return record;
        }

        // Search for the controlled record with the ID and returns the primary key
        /// <summary>
        /// Search for the controlled record with the ID and returns the primary key.
        /// </summary>
        /// <returns>
        /// Controlled record founded.
        /// </returns>
        public string GetPrimaryKeyFromControlledRecord(PersistentSupport sp, User user, string ID)
        {
            string primaryKey = "";

            ControlledRecord l_record;

            l_record = getControlledRecord(ID);

            if (l_record != null)
            {
                if (string.IsNullOrWhiteSpace(l_record.PrimaryKey))
                {
                    if (l_record.ControlledFields.Count > 0)
                    {
                        CriteriaSet selectionCrit = CriteriaSet.And();
                        for (int i = 0; i < l_record.ControlledFields.Count; i++)
                        {
                            FieldRef l_field = new FieldRef(l_table, l_record.ControlledFields[i].Qfield);
                            selectionCrit.Equal(l_field, l_record.ControlledFields[i].Qvalue);
                        }

                        // Search for the record in the BD
                        SelectQuery controlledRecordQuery = new SelectQuery()
                            .Select(l_table, l_primaryKeyField)
                            .From(l_table, l_table)
                            .PageSize(1);

                        controlledRecordQuery.Where(selectionCrit);
                        object recordExists = sp.ExecuteScalar(controlledRecordQuery);
                        // If the record was found let's save the primary key
                        if (recordExists != null)
                        {
                            l_record.PrimaryKey = primaryKey = recordExists.ToString();
                            addControlledRecord(ID, l_record);
                        }
                    }
                }
                else
                    primaryKey = l_record.PrimaryKey;
            }
            return primaryKey;
        }
    }

    /// <summary>
    /// Classe to representar um registo controlado pela aplicação
    /// </summary>
    public class ControlledRecord
    {
        /// <summary>
        /// Representa que Qvalue determinado Qfield deve ter
        /// </summary>
        public class ControlledField
        {
            /// <summary>
            /// O Qfield controlado
            /// </summary>
            public string Qfield;
            /// <summary>
            /// O Qvalue que esse Qfield deve ter
            /// </summary>
            public object Qvalue;
        }

        /// <summary>
        /// Identificador
        /// </summary>
        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }
        private string m_id;

        /// <summary>
        /// Key to o registo controlado
        /// </summary>
        public SortedList<int, string> Key
        {
            get { return m_chave; }
            set { m_chave = value; }
        }
        private SortedList<int, string> m_chave = new SortedList<int, string>();
		
		
		/// <summary>
        /// Primary key of the controlled record in the DB
        /// </summary>
        public string PrimaryKey
        {
            get { return m_primaryKey; }
            set { m_primaryKey = value; }
        }
        private string m_primaryKey;

        /// <summary>
        /// Mode de criação automática
        /// </summary>
        public ControlledRecordMode Mode
        {
            get { return m_modo; }
            set { m_modo = value; }
        }
        private ControlledRecordMode m_modo;

        /// <summary>
        /// Lista de fields controlados por este registo
        /// </summary>
        public List<ControlledField> ControlledFields
        {
            get { return m_camposControlados; }
            set { m_camposControlados = value; }
        }
        private List<ControlledField> m_camposControlados = new List<ControlledField>();
    }
}
