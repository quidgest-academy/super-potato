using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// List of criteria operators
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public enum CriteriaOperator
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Lesser,
        LesserOrEqual,
        Like,
        NotLike,
        In,
        NotIn,
        Exists,
        NotExists
    }
}
