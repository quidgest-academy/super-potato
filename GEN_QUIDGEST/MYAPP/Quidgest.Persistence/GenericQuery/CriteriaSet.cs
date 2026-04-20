using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a set of criterias
    /// </summary>
    public class CriteriaSet : ICloneable
    {
		public enum FindVariable
		{
			Any, // accepts any term
			AnyValue, // accepts any value (number, string, date, etc...)
			AnyNonValue, // accepts any non-value (column, function, etc...)
			AnyColumn // accepts any column
		}

        private IList<Criteria> m_criterias;
        /// <summary>
        /// The list of conditions to check
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<Criteria> Criterias
        {
            get
            {
                if (m_criterias == null)
                {
                    m_criterias = new List<Criteria>();
                }

                return m_criterias;
            }
        }

        private IList<CriteriaSet> m_subSets;
        /// <summary>
        /// The list of condition subsets to check
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<CriteriaSet> SubSets
        {
            get
            {
                if (m_subSets == null)
                {
                    m_subSets = new List<CriteriaSet>();
                }

                return m_subSets;
            }
        }

        /// <summary>
        /// The operation to aggregate the condition values
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaSetOperator Operation
        {
            get;
            set; //not being able to set this directly involves a lot of workarounds with clones to make it happen.
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation">The operation to aggregate the condition values</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaSet(CriteriaSetOperator operation)
        {
            Operation = operation;
        }

        /// <summary>
        /// Checks if the supplied tables satisfy all references in the conditions
        /// </summary>
        /// <param name="availableTables">The list of tables that might satisfy the references</param>
        /// <returns>True the supplied list satisfy the references, otherwise false</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool CanSatisfyReferences(IEnumerable<ITableSource> availableTables)
        {
            return CanSatisfyReferences(this, availableTables);
        }

        /// <summary>
        /// Checks if the supplied tables satisfy all references in the conditions of the specified criteria set recursively
        /// </summary>
        /// <param name="criteriaSet">The criteria set with the conditions</param>
        /// <param name="availableTables">The list of tables that might satisfy the references</param>
        /// <returns>True the supplied list satisfy the references, otherwise false</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        private bool CanSatisfyReferences(CriteriaSet criteriaSet, IEnumerable<ITableSource> availableTables)
        {
            IList<ColumnReference> references = new List<ColumnReference>();
            foreach (var criteria in criteriaSet.Criterias)
            {
                references.Clear();

                // a column references a table
                if (criteria.LeftTerm is ColumnReference)
                {
                    references.Add(criteria.LeftTerm as ColumnReference);
                }
                if (criteria.RightTerm is ColumnReference)
                {
                    references.Add(criteria.RightTerm as ColumnReference);
                }

                // the arguments of a function might reference a table
                var func = criteria.LeftTerm as SqlFunction;
                {
                    if (func != null && func.Arguments != null)
                    {
                        foreach (var arg in func.Arguments)
                        {
                            if (arg is ColumnReference)
                            {
                                references.Add(arg as ColumnReference);
                            }
                        }
                    }
                }
                func = criteria.RightTerm as SqlFunction;
                {
                    if (func != null && func.Arguments != null)
                    {
                        foreach (var arg in func.Arguments)
                        {
                            if (arg is ColumnReference)
                            {
                                references.Add(arg as ColumnReference);
                            }
                        }
                    }
                }

                foreach (var reference in references)
                {
                    // if at least one reference is not satisfied, the criteria set is not satisfied
                    if (!IsTableInList(reference.TableAlias, availableTables))
                    {
                        return false;
                    }
                }
            }

            foreach (var subSet in criteriaSet.SubSets)
            {
                // if at least one subset is not satisfied, the criteria set is not satisfied
                if (!CanSatisfyReferences(subSet, availableTables))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsTableInList(string tableAlias, IEnumerable<ITableSource> tables)
        {
            foreach (ITableSource table in tables)
            {
                if (table.TableAlias == tableAlias)
                {
                    return true;
                }
            }

            return false;
        }

		/// <summary>
		/// Merges the subsets with the same operator within this set and removes the empty subsets
		/// </summary>
		/// <returns>this set normalized</returns>
		public CriteriaSet Normalize()
		{
			IList<CriteriaSet> subSets = new List<CriteriaSet>(SubSets);

			foreach (CriteriaSet subSet in subSets)
			{
				if (subSet == null)
				{
					this.SubSets.Remove(subSet);
					continue;
				}

				subSet.Normalize();

				if ((subSet.Operation == this.Operation) || (subSet.Criterias.Count == 0 && subSet.SubSets.Count == 0))
				{
					foreach (Criteria subCrit in subSet.Criterias)
					{
						this.Criterias.Add(subCrit);
					}
					foreach (CriteriaSet subSetSet in subSet.SubSets)
					{
						this.SubSets.Add(subSetSet);
					}

					this.SubSets.Remove(subSet);
				}
			}

			if (this.Criterias.Count == 0 && this.SubSets.Count == 1)
			{
				CriteriaSet other = this.SubSets[0];
				this.SubSets.Clear();
				this.Operation = other.Operation;
				foreach (Criteria subCrit in other.Criterias)
				{
					this.Criterias.Add(subCrit);
				}
				foreach (CriteriaSet subSetSet in other.SubSets)
				{
					this.SubSets.Add(subSetSet);
				}
			}

			return this;
		}

        public CriteriaSet Equal(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return Equal(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet Equal(string tableAlias, string column, object rightTerm)
        {
            return Equal(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet Equal(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.Equal, rightTerm));

            return this;
        }

        public CriteriaSet Equal(FieldRef leftField, FieldRef rightField)
        {
            if (rightField == null)
            {
                return Equal(leftField, (object)rightField);
            }
            else
            {
                return Equal(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
            }
        }

        public CriteriaSet Equal(FieldRef leftField, object rightTerm)
        {
            return Equal(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet NotEqual(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return NotEqual(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet NotEqual(string tableAlias, string column, object rightTerm)
        {
            return NotEqual(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet NotEqual(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.NotEqual, rightTerm));

            return this;
        }

        public CriteriaSet NotEqual(FieldRef leftField, FieldRef rightField)
        {
            if (rightField == null)
            {
                return NotEqual(leftField, (object)rightField);
            }
            else
            {
                return NotEqual(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
            }
        }

        public CriteriaSet NotEqual(FieldRef leftField, object rightTerm)
        {
            return NotEqual(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet Greater(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return Greater(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet Greater(string tableAlias, string column, object rightTerm)
        {
            return Greater(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet Greater(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.Greater, rightTerm));

            return this;
        }

        public CriteriaSet Greater(FieldRef leftField, FieldRef rightField)
        {
            return Greater(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
        }

        public CriteriaSet Greater(FieldRef leftField, object rightTerm)
        {
            return Greater(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet GreaterOrEqual(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return GreaterOrEqual(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet GreaterOrEqual(string tableAlias, string column, object rightTerm)
        {
            return GreaterOrEqual(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet GreaterOrEqual(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.GreaterOrEqual, rightTerm));

            return this;
        }

        public CriteriaSet GreaterOrEqual(FieldRef leftField, FieldRef rightField)
        {
            return GreaterOrEqual(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
        }

        public CriteriaSet GreaterOrEqual(FieldRef leftField, object rightTerm)
        {
            return GreaterOrEqual(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet Lesser(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return Lesser(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet Lesser(string tableAlias, string column, object rightTerm)
        {
            return Lesser(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet Lesser(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.Lesser, rightTerm));

            return this;
        }

        public CriteriaSet Lesser(FieldRef leftField, FieldRef rightField)
        {
            return Lesser(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
        }

        public CriteriaSet Lesser(FieldRef leftField, object rightTerm)
        {
            return Lesser(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet LesserOrEqual(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return LesserOrEqual(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet LesserOrEqual(string tableAlias, string column, object rightTerm)
        {
            return LesserOrEqual(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet LesserOrEqual(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.LesserOrEqual, rightTerm));

            return this;
        }

        public CriteriaSet LesserOrEqual(FieldRef leftField, FieldRef rightField)
        {
            return LesserOrEqual(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
        }

        public CriteriaSet LesserOrEqual(FieldRef leftField, object rightTerm)
        {
            return LesserOrEqual(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet Like(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return Like(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet Like(string tableAlias, string column, object rightTerm)
        {
            return Like(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet Like(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.Like, rightTerm));

            return this;
        }

        public CriteriaSet Like(FieldRef leftField, FieldRef rightField)
        {
            return Like(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
        }

        public CriteriaSet Like(FieldRef leftField, object rightTerm)
        {
            return Like(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet NotLike(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return NotLike(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public CriteriaSet NotLike(string tableAlias, string column, object rightTerm)
        {
            return NotLike(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet NotLike(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.NotLike, rightTerm));

            return this;
        }

        public CriteriaSet NotLike(FieldRef leftField, FieldRef rightField)
        {
            return NotLike(new ColumnReference(leftField.Area, leftField.Field), new ColumnReference(rightField.Area, rightField.Field));
        }

        public CriteriaSet NotLike(FieldRef leftField, object rightTerm)
        {
            return NotLike(new ColumnReference(leftField.Area, leftField.Field), rightTerm);
        }

        public CriteriaSet In(string tableAlias, string column, IEnumerable rightTerm)
        {
            return InCore(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet In(string tableAlias, string column, SelectQuery rightTerm)
        {
            return InCore(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet In(FieldRef field, IEnumerable rightTerm)
        {
            return InCore(new ColumnReference(field.Area, field.Field), rightTerm);
        }

        public CriteriaSet In(FieldRef field, SelectQuery rightTerm)
        {
            return InCore(new ColumnReference(field.Area, field.Field), rightTerm);
        }

        public CriteriaSet In(string tableAlias, string column, DataTable rightTerm)
        {
            return InCore(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet In(FieldRef field, DataTable rightTerm)
        {
            return InCore(new ColumnReference(field.Area, field.Field), rightTerm);
        }        

        private CriteriaSet InCore(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.In, rightTerm));

            return this;
        }

        public CriteriaSet NotIn(string tableAlias, string column, IEnumerable rightTerm)
        {
            return NotInCore(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet NotIn(string tableAlias, string column, SelectQuery rightTerm)
        {
            return NotInCore(new ColumnReference(tableAlias, column), rightTerm);
        }

        public CriteriaSet NotIn(FieldRef field, IEnumerable rightTerm)
        {
            return NotInCore(new ColumnReference(field.Area, field.Field), rightTerm);
        }

        public CriteriaSet NotIn(FieldRef field, SelectQuery rightTerm)
        {
            return NotInCore(new ColumnReference(field.Area, field.Field), rightTerm);
        }

        private CriteriaSet NotInCore(object leftTerm, object rightTerm)
        {
            Criterias.Add(new Criteria(leftTerm, CriteriaOperator.NotIn, rightTerm));

            return this;
        }

        public CriteriaSet Exists(SelectQuery term)
        {
            Criterias.Add(new Criteria(term, CriteriaOperator.Exists, null));

            return this;
        }

        public CriteriaSet NotExists(SelectQuery term)
        {
            Criterias.Add(new Criteria(term, CriteriaOperator.NotExists, null));

            return this;
        }

        public CriteriaSet SubSet(CriteriaSet subSet)
        {
			if (subSet != null)
			{
				SubSets.Add(subSet);
			}

            return this;
        }

		/// <summary>
		/// Creates a set of criterias connected by the AND operator
		/// </summary>
		/// <returns>The criteria set with the AND operator</returns>
		/// <remarks>
		/// (crit1 AND crit2 AND crit3)
		/// <!--
		/// Author: CX 2011.06.28
		/// Modified: CX 2012.03.29
		/// Reviewed:
		/// -->
		/// </remarks>
		public static CriteriaSet And()
        {
            return new CriteriaSet(CriteriaSetOperator.And);
        }

		/// <summary>
		/// Creates a set of criterias connected by the OR operator
		/// </summary>
		/// <returns>The criteria set with the OR operator</returns>
		/// <remarks>
		/// (crit1 OR crit2 OR crit3)
		/// <!--
		/// Author: CX 2011.06.28
		/// Modified: CX 2012.03.29
		/// Reviewed:
		/// -->
		/// </remarks>
		public static CriteriaSet Or()
        {
            return new CriteriaSet(CriteriaSetOperator.Or);
        }

		/// <summary>
		/// Creates a set of criterias connected by the AND operator and negated
		/// </summary>
		/// <returns>The negated criteria set with the AND operator</returns>
		/// <remarks>
		/// NOT(crit1 AND crit2 AND crit3)
		/// <!--
		/// Author: CX 2012.03.29
		/// Modified:
		/// Reviewed:
		/// -->
		/// </remarks>
		public static CriteriaSet NotAnd()
		{
			return new CriteriaSet(CriteriaSetOperator.NotAnd);
		}

		/// <summary>
		/// Creates a set of criterias connected by the OR operator and negated
		/// </summary>
		/// <returns>The negated criteria set with the OR operator</returns>
		/// <remarks>
		/// NOT(crit1 OR crit2 OR crit3)
		/// <!--
		/// Author: CX 2012.03.29
		/// Modified:
		/// Reviewed:
		/// -->
		/// </remarks>
		public static CriteriaSet NotOr()
		{
			return new CriteriaSet(CriteriaSetOperator.NotOr);
		}

        public object Clone()
        {
            CriteriaSet result = new CriteriaSet(Operation);

            foreach (Criteria criteria in Criterias)
            {
                result.Criterias.Add((Criteria)criteria.Clone());
            }
            foreach (CriteriaSet subSet in SubSets)
            {
                result.SubSets.Add((CriteriaSet)subSet.Clone());
            }

            return result;
        }

		public Criteria FindCriteria(FieldRef leftTerm, CriteriaOperator op, FieldRef rightTerm)
		{
			return FindCriteriaCore(new ColumnReference(leftTerm), op, new ColumnReference(rightTerm));
		}

		public Criteria FindCriteria(FieldRef leftTerm, CriteriaOperator op, object rightTerm)
		{
			return FindCriteriaCore(new ColumnReference(leftTerm), op, rightTerm);
		}

		public Criteria FindCriteria(string alias, string column, CriteriaOperator op, object rightTerm)
		{
			return FindCriteriaCore(new ColumnReference(alias, column), op, rightTerm);
		}

		public Criteria FindCriteria(FieldRef leftTerm, FindVariable op, FieldRef rightTerm)
		{
			return FindCriteriaCore(new ColumnReference(leftTerm), op, new ColumnReference(rightTerm));
		}

		public Criteria FindCriteria(FieldRef leftTerm, CriteriaOperator op, FindVariable rightTerm)
		{
			return FindCriteriaCore(new ColumnReference(leftTerm), op, rightTerm);
		}

		public Criteria FindCriteria(string alias, string column, CriteriaOperator op, FindVariable rightTerm)
		{
			return FindCriteriaCore(new ColumnReference(alias, column), op, rightTerm);
		}

		private Criteria FindCriteriaCore(object leftTerm, object op, object rightTerm)
		{

			foreach (Criteria crit in this.Criterias)
			{
				if (crit == null)
				{
					continue;
				}

				if (IsFindMatch(crit, leftTerm, op, rightTerm))
				{
					return crit;
				}
			}

			foreach (CriteriaSet subSet in this.SubSets)
			{
				if (subSet == null)
				{
					continue;
				}

				Criteria result = subSet.FindCriteriaCore(leftTerm, op, rightTerm);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		private bool IsFindMatch(Criteria crit, object leftTerm, object op, object rightTerm)
		{
			return IsTermMatch(crit.LeftTerm, leftTerm)
				&& IsOperatorMatch(crit.Operation, op)
				&& IsTermMatch(crit.RightTerm, rightTerm);
		}

		private bool IsTermMatch(object criteriaTerm, object findTerm)
		{
			if (findTerm is FindVariable)
			{
				FindVariable findVar = (FindVariable)findTerm;
				switch (findVar)
				{
					case FindVariable.Any:
						return true;
					case FindVariable.AnyValue:
						return !(criteriaTerm is ColumnReference || criteriaTerm is SqlFunction || criteriaTerm is SelectQuery);
					case FindVariable.AnyNonValue:
						return (criteriaTerm is ColumnReference || criteriaTerm is SqlFunction || criteriaTerm is SelectQuery);
					case FindVariable.AnyColumn:
						return criteriaTerm is ColumnReference;
					default:
						return false;
				}
			}
			else
			{
				return Object.Equals(criteriaTerm, findTerm);
			}
		}

		private bool IsOperatorMatch(CriteriaOperator criteriaOperator, object findOperator)
		{
			if (findOperator is FindVariable)
			{
				return ((FindVariable)findOperator) == FindVariable.Any;
			}

			if (findOperator is CriteriaOperator)
			{
				return ((CriteriaOperator)findOperator) == criteriaOperator;
			}

			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is CriteriaSet))
			{
				return false;
			}

			CriteriaSet other = (CriteriaSet)obj;

			bool isEqual = this.Criterias.Count == other.Criterias.Count
				&& this.SubSets.Count == other.SubSets.Count;

			for (int i = 0; i < this.Criterias.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.Criterias[i], other.Criterias[i]);
			}

			for (int i = 0; i < this.SubSets.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.SubSets[i], other.SubSets[i]);
			}

			return isEqual;
		}

		public override int GetHashCode()
		{
			int hash = 0;

			for (int i = 0; i < this.Criterias.Count; i++)
			{
				hash ^= (this.Criterias[i] == null ? 0 : this.Criterias[i].GetHashCode());
			}

			for (int i = 0; i < this.SubSets.Count; i++)
			{
				hash ^= (this.SubSets[i] == null ? 0 : this.SubSets[i].GetHashCode());
			}

			return hash;
		}
    }
}
