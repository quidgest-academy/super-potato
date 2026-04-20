using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// List of criteria set operators
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
	/// Modified: CX 2012.03.29
    /// Reviewed:
    /// -->
    /// </remarks>
    public enum CriteriaSetOperator
    {
        And,
        Or,
		NotAnd,
		NotOr
    }
}
