namespace CSGenio.framework
{
	/// <summary>
	/// Represents a database relation. Relations are viewed as pointer where the
	/// source is a many to one relation with the target.
	/// For example, in the relation 'Invoice'<-'Details' the 'Details' is the 
	/// source table and 'Invoice' is the target table.
	/// </summary>
	public class Relation
	{

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceSystem">Source table system</param>
        /// <param name="sourceTable">Source table name</param>
        /// <param name="aliasSourceTab">Source table alias (area)</param>
        /// <param name="sourceIntKey">Source table primary key</param>
        /// <param name="sourceRelField">Source table foreign key</param>
        /// <param name="destinationSystem">Target table system</param>
        /// <param name="targetTable">Target table name</param>
        /// <param name="aliasTargetTab">Target table alias (area)</param>
        /// <param name="targetIntKey">Target table primary key</param>
        /// <param name="targetRelField">Target table relation key field</param>
        public Relation(string sourceSystem, string sourceTable, string aliasSourceTab, string sourceIntKey, string sourceRelField, string destinationSystem, string targetTable, string aliasTargetTab, string targetIntKey, string targetRelField)
		{
			SourceSystem = sourceSystem;
			SourceTable = sourceTable;
			AliasSourceTab = aliasSourceTab;
			SourceIntKey = sourceIntKey;
            SourceRelField = sourceRelField;

			DestinationSystem = destinationSystem;
			TargetTable = targetTable;
			AliasTargetTab = aliasTargetTab;
			TargetIntKey = targetIntKey;
            TargetRelField = targetRelField;
		}

        /// <summary>
        /// Target table alias (area)
        /// </summary>
        public string AliasTargetTab { get; set; }

        /// <summary>
        /// Source table system
        /// </summary>
        public string SourceSystem { get; set; }

        /// <summary>
        /// Source table name
        /// </summary>
        public string SourceTable { get; set; }

        /// <summary>
        /// Target table system
        /// </summary>
        public string DestinationSystem { get; set; }

        /// <summary>
        /// Target table name
        /// </summary>
        public string TargetTable { get; set; }

        /// <summary>
        /// Source table alias (area)
        /// </summary>
        public string AliasSourceTab { get; set; }

        /// <summary>
        /// Target table primary key
        /// </summary>
        public string TargetIntKey { get; set; }

        /// <summary>
        /// Source table primary key
        /// </summary>
        public string SourceIntKey { get; set; }

        /// <summary>
        /// Target table relation key field
        /// </summary>
        public string TargetRelField { get; set; }

        /// <summary>
        /// Source table foreign key
        /// </summary>
        public string SourceRelField { get; set; }
	}
}
