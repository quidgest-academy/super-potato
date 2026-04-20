using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents an object that can be treated as a table with data in a query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public interface ITableSource : ICloneable
    {
        /// <summary>
        /// The alias for the table
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        string TableAlias { get; }
    }
}
