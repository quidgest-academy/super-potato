namespace CSGenio.framework
{
	/// <summary>
	/// Options and Fields involved in defining a tree table
	/// </summary>
	public class TreeTable
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public TreeTable()
		{
        }

        /// <summary>
        /// Field holding the tree level
        /// </summary>
        public string RecordLevelField { get; set; }

        /// <summary>
        /// Fields holding the parent
        /// </summary>
        public string ParentTableField { get; set; }

        /// <summary>
        /// Field holding the leaf status
        /// </summary>
        public string MoveableField { get; set; }

        /// <summary>
        /// Field holding the tree position code
        /// </summary>
        public string DesignationField { get; set; }
    }
}
