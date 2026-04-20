namespace CSGenio.framework
{
    public class SumsCreatesRecords
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetTable">Name of the target table</param>
        /// <param name="aliasTargetTab">Alias (area) of the target table</param>
        /// <param name="targetIntKey">Primary key name for the target table</param>
        /// <param name="targetRelField">Foreigh key name that connects to the primary key</param>
        /// <param name="sourceFields">Source grouping field names</param>
        /// <param name="targetFields">Target grouping field names</param>
        public SumsCreatesRecords(string targetTable, string aliasTargetTab, string targetIntKey, string targetRelField, string[] sourceFields, string[] targetFields)
		{
			TargetTable = targetTable;
			AliasTargetTab = aliasTargetTab;
			TargetIntKey = targetIntKey;
            TargetRelField = targetRelField;
            STSourceFields = sourceFields;
            STTargetFields = targetFields;
		}

        /// <summary>
        /// Name of the target table
        /// </summary>
        public string TargetTable { get; set; }

        /// <summary>
        /// Alias (area) of the target table
        /// </summary>
        public string AliasTargetTab { get; set; }

        /// <summary>
        /// Primary key name for the target table
        /// </summary>
        public string TargetIntKey { get; set; }

        /// <summary>
        /// Foreigh key name that connects to the primary key
        /// </summary>
        public string TargetRelField { get; set; }

        /// <summary>
        /// Source grouping field names
        /// </summary>
        public string[] STSourceFields { get; set; }

        /// <summary>
        /// Target grouping field names
        /// </summary>
        public string[] STTargetFields { get; set; }
    }
}
