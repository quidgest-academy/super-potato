namespace CSGenio.reporting
{
    /// <summary>
    /// MH (31/07/2017) - Esta class foi implementada to diminuir quantidade de parametros
    /// recebidos por constructo do ReportCrystal e ReportSSRS
    /// </summary>
    public class ReportLimitParameter
    {
        public enum LimitSource { SE, SU, DB, DM, AC };

        public LimitSource Source { get; set; }

        /// <summary>
        /// Full field name
        /// </summary>
        public string FullFieldName { get; set; }

        /// <summary>
        /// D - Date | N - Numeric | C - String
        /// </summary>
        public string FieldType { get; set; }

        public ReportLimitParameter(LimitSource lim)
        {
            this.Source = lim;
        }
    }

    /// <summary>
    /// Limit of the type: Selection between limits
    /// </summary>
    public class ReportLimitParameter_SE : ReportLimitParameter
    {
        // Min value
        public string MinFieldName { get; set; }
        public object MinFieldValue { get; set; }

        // Max value
        public string MaxFieldName { get; set; }
        public object MaxFieldValue { get; set; }

        public ReportLimitParameter_SE() : base(LimitSource.SE) { }
    }

    /// <summary>
    /// Limit of the type: Selection between limits
    /// </summary>
    public class ReportLimitParameter_SU : ReportLimitParameter
    {
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public string LimitType { get; set; }

        public ReportLimitParameter_SU() : base(LimitSource.SU) { }
    }

    /// <summary>
    /// Limit of the type: List with tree selection
    /// </summary>
    public class ReportLimitParameter_DB : ReportLimitParameter
    {
        /// <summary>
        /// Tree table - Design
        /// </summary>
        public string FieldValue { get; set; }

        public ReportLimitParameter_DB() : base(LimitSource.DB) { }
    }

    /// <summary>
    /// Limit of the type: List with multi selection
    /// </summary>
    public class ReportLimitParameter_DM : ReportLimitParameter
    {
        /// <summary>
        /// Primary or Foreign key values
        /// </summary>
        public string[] FieldValue { get; set; }

        public ReportLimitParameter_DM() : base(LimitSource.DM) { }
    }
    public class ReportLimitParameter_AC : ReportLimitParameter
    {
        /// <summary>
        /// Field value for auto complete
        /// </summary>
        public string FieldValue { get; set; }
        public ReportLimitParameter_AC() : base(LimitSource.AC) { }
    }
}
