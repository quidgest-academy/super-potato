using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administration.Models
{
    public class JQueryDataTableParamModel : ModelBase
    {
        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string global_search { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int per_page { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// Columns in table
        /// </summary>
        public IEnumerable<DTColumn> columns { get; set; }

        /// <summary>
        /// Columns to which ordering should be applied
        /// </summary>
        public IEnumerable<DTOrder> sort { get; set; }

    }

    public class DTColumn
    {
        /// <summary>
        /// Column's data source (if defined)
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// Column's name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Flag to indicate if this column is orderable
        /// </summary>
        public bool orderable { get; set; }

        /// <summary>
        /// Flag to indicate if this column is searchable
        /// </summary>
        public bool searchable { get; set; }

        /// <summary>
        /// Search to apply to this specific column
        /// </summary>
        public string search { get; set; }
    }

    public class DTOrder
    {
        /// <summary>
        /// Column index to which ordering should be applied
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Ordering direction for this column
        /// </summary>
        public string order { get; set; }
        public bool caseSensitive { get; set; }
    }
}