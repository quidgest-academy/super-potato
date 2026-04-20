using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a condition
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class Criteria : ICloneable
    {
        /// <summary>
        /// The left term of the operation
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public object LeftTerm
        {
            get;
            set;
        }

        /// <summary>
        /// The right term of the operation
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public object RightTerm
        {
            get;
            set;
        }

        /// <summary>
        /// The operation to apply to the terms
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaOperator Operation
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="leftTerm">The left term of the operation</param>
        /// <param name="operation">The operation to apply to the terms</param>
        /// <param name="rightTerm">The right term of the operation</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public Criteria(object leftTerm, CriteriaOperator operation, object rightTerm)
        {
            LeftTerm = leftTerm;
            RightTerm = rightTerm;
            Operation = operation;
        }

        public object Clone()
        {
            return new Criteria(
                LeftTerm != null && LeftTerm is ICloneable ? ((ICloneable)LeftTerm).Clone() : LeftTerm,
                Operation,
                RightTerm != null && RightTerm is ICloneable ? ((ICloneable)RightTerm).Clone() : RightTerm);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Criteria))
			{
				return false;
			}

			Criteria other = (Criteria)obj;

			return Object.Equals(this.LeftTerm, other.LeftTerm)
				&& this.Operation == other.Operation
				&& Object.Equals(this.RightTerm, other.RightTerm);
		}

		public override int GetHashCode()
		{
			return (this.LeftTerm == null ? 0 : this.LeftTerm.GetHashCode())
				^ this.Operation.GetHashCode()
				^ (this.RightTerm == null ? 0 : this.RightTerm.GetHashCode());
		}
    }
}
