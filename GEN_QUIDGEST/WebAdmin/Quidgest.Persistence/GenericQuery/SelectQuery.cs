using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a select query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified: CX 2011.11.03
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SelectQuery : ISqlExpression
    {
        /// <summary>
        /// The select field list
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<SelectField> SelectFields
        {
            get;
            private set;
        }

        private ITableSource m_fromTable;
        /// <summary>
        /// The table for the from clause
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public ITableSource FromTable
        {
            get
            {
                return m_fromTable;
            }
            set
            {
                m_fromTable = value;
                if (m_fromTable != null)
                {
                    foreach (SelectField field in SelectFields)
                    {
                        ColumnReference c = field.Expression as ColumnReference;
                        if (c != null && c.TableAlias == null)
                        {
                            c.TableAlias = m_fromTable.TableAlias;
                            if (field.Alias == null)
                            {
                                field.Alias = c.TableAlias + "." + c.ColumnName;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The list of the tables to be joinned in this query an their join conditions
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<TableJoin> Joins
        {
            get;
            private set;
        }

        /// <summary>
        /// The where condition
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaSet WhereCondition
        {
            get;
            set;
        }

        /// <summary>
        /// The list of fields to group by
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<ISqlExpression> GroupByFields
        {
            get;
            private set;
        }

        /// <summary>
        /// The having condition
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaSet HavingCondition
        {
            get;
            set;
        }

        /// <summary>
        /// The sortings to be applied to the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<ColumnSort> OrderByFields
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the query should return only the distinct results, otherwise false
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool DistinctProp
        {
            get;
            set;
        }

        private int m_page = 1;

        /// <summary>
        /// The result page we want. Must be greater than 0.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int PageProp
        {
            get
            {
                return m_page;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Must be greater than 0");
                }

                m_page = value;
            }
        }

        private int? m_pageSize;

        /// <summary>
        /// The page size. Must be null, 0 or greater.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int? PageSizeProp
        {
            get
            {
                return m_pageSize;
            }
            set
            {
                if (value != null && value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Must be null, 0 or greater");
                }

                m_pageSize = value;
            }
        }

        private int m_offset;

        /// <summary>
        /// The result list offset. Must be 0 or greater.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int OffsetProp
        {
            get
            {
                return m_offset;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Must be 0 or greater");
                }

                m_offset = value;
            }
        }

        public bool NeedsPaging
        {
            get
            {
                return OffsetProp != 0 || PageProp != 1 || PageSizeProp != null;
            }
        }

        /// <summary>
        /// The queries to union with
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.02
        /// Modified: CX 2011.11.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<QueryUnion> UnionQueries
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the query should apply the UpdLockCommand
        /// </summary>
        /// <remarks>
        /// <!--
        /// This is a quick "dirty" solution to add update locks to queries
        /// In the future this should be implemented in a different way, since we can have
        /// only one type of lock in a query. There should only be a single lock type attribute
        /// that could get its value from an enum, for instance.
        /// Author: RC 2017.05.24
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool updateLock
        {
            get;
            set;
        }

        /// <summary>
        /// True if the query should apply the NoLockCommand
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: NH 2016.07.21
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool noLock
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="persistentSupport">The object with the persistent support information</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery()
        {
            DistinctProp = false;
            PageProp = 1;
            PageSizeProp = null;
            OffsetProp = 0;
            SelectFields = new List<SelectField>();
            Joins = new List<TableJoin>();
            GroupByFields = new List<ISqlExpression>();
            OrderByFields = new List<ColumnSort>();
            UnionQueries = new List<QueryUnion>();
			noLock = false;
            updateLock = false;
        }

        /// <summary>
        /// Adds a select field to the query
        /// </summary>
        /// <param name="tableAlias">The alias of the table of the column</param>
        /// <param name="column">The column</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Select(string tableAlias, string column)
        {
            if (tableAlias == null)
            {
                SelectFields.Add(new SelectField(new ColumnReference(FromTable == null ? null : FromTable.TableAlias, column), FromTable == null ? null : FromTable.TableAlias + "." + column));
            }
            else
            {
                SelectFields.Add(new SelectField(new ColumnReference(tableAlias, column), tableAlias + "." + column));
            }

            return this;
        }

        /// <summary>
        /// Adds a select field to the query
        /// </summary>
        /// <param name="tableAlias">The alias of the table of the column</param>
        /// <param name="column">The column</param>
        /// <param name="alias">The field alias</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Select(string tableAlias, string column, string alias)
        {
            SelectFields.Add(new SelectField(new ColumnReference(tableAlias, column), alias));

            return this;
        }

        /// <summary>
        /// Adds a select field to the query
        /// </summary>
        /// <param name="expression">The expression to the field value</param>
        /// <param name="alias">The field alias</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Select(ISqlExpression expression, string alias)
        {
            SelectFields.Add(new SelectField(expression, alias));

            return this;
        }

        /// <summary>
        /// Adds a select field to the query
        /// </summary>
        /// <param name="field">The area field</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Select(FieldRef field)
        {
            SelectFields.Add(new SelectField(new ColumnReference(field.Area, field.Field), field.FullName));

            return this;
        }

        /// <summary>
        /// Adds a select field to the query
        /// </summary>
        /// <param name="field">The area field</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Select(FieldRef field, string alias)
        {
            SelectFields.Add(new SelectField(new ColumnReference(field.Area, field.Field), alias));

            return this;
        }

        /// <summary>
        /// Adds a select field collection to the query
        /// </summary>
        /// <param name="fields">The fields to add</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.19
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Select(IEnumerable<SelectField> fields)
        {
            if (fields != null)
            {
                foreach (SelectField field in fields)
                {
                    this.SelectFields.Add(field);
                }
            }

            return this;
        }
        
        /// <summary>
        /// Specifies the table for the from clause
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery From(string tableName)
        {
            return From(tableName, tableName);
        }

        /// <summary>
        /// Specifies the table for the from clause
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery From(string tableName, string tableAlias)
        {
            FromTable = new TableReference(tableName, tableAlias);

            return this;
        }

        /// <summary>
        /// Specifies the table for the from clause
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery From(string schemaName, string tableName, string tableAlias)
        {
            FromTable = new TableReference(schemaName, tableName, tableAlias);

            return this;
        }

        /// <summary>
        /// Specifies a function for the from clause
        /// </summary>
        /// <param name="function">The function to be used as the from table</param>
        /// <param name="alias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2012.03.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery From(SqlFunction function, string alias)
        {
			FromTable = new FunctionReference(function, alias);

            return this;
        }

		/// <summary>
		/// Specifies a query for the from clause
		/// </summary>
		/// <param name="query">The query to be used as the from table</param>
		/// <param name="alias">The alias for the table</param>
		/// <returns>The query</returns>
		/// <remarks>
		/// <!--
		/// Author: CX 2011.06.28
		/// Modified:
		/// Reviewed:
		/// -->
		/// </remarks>
		public SelectQuery From(SelectQuery query, string alias)
		{
			FromTable = new QueryReference(query, alias);

			return this;
		}

        /// <summary>
        /// Specifies the table for the from clause
        /// </summary>
        /// <param name="area">The area we want the data from</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery From(AreaRef area)
        {
            return From(area.Schema, area.Table, area.Alias);
        }

        /// <summary>
        /// Specifies the table for the from clause
        /// </summary>
        /// <param name="source">The source of the data</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.19
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery From(ITableSource source)
        {
            FromTable = source;

            return this;
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(string tableName)
        {
            return Join(tableName, tableName, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(string tableName, string tableAlias)
        {
            return Join(tableName, tableAlias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.10.03
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(string schemaName, string tableName, string tableAlias)
        {
            return Join(schemaName, tableName, tableAlias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(string tableName, TableJoinType type)
        {
            return Join(tableName, tableName, type);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(string tableName, string tableAlias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new TableReference(tableName, tableAlias), type));

            return this;
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.10.03
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(string schemaName, string tableName, string tableAlias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new TableReference(schemaName, tableName, tableAlias), type));

            return this;
        }

        /// <summary>
        /// Adds a join of a select query
        /// </summary>
        /// <param name="query">The query to join with</param>
        /// <param name="alias">The alias for the query</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(SelectQuery query, string alias)
        {
            return Join(query, alias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a select query
        /// </summary>
        /// <param name="query">The query to join with</param>
        /// <param name="alias">The alias for the query</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(SelectQuery query, string alias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new QueryReference(query, alias), type));

            return this;
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="area">The area we want the data from</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(AreaRef area)
        {
            return Join(area.Schema, area.Table, area.Alias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="area">The area we want the data from</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(AreaRef area, TableJoinType type)
        {
            return Join(area.Schema, area.Table, area.Alias, type);
        }

        /// <summary>
        /// Adds a collectin of joins to the query
        /// </summary>
        /// <param name="joins">The joins to add</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.19
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(IEnumerable<TableJoin> joins)
        {
            if (joins != null)
            {
                foreach (TableJoin join in joins)
                {
                    this.Joins.Add(join);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds a join of a select query
        /// </summary>
        /// <param name="function">The table valued function</param>
        /// <param name="alias">The alias for the query</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: TAP 2018.09.13
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Join(SqlFunction function, string alias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new FunctionReference(function, alias), type));

            return this;
        }

        /// <summary>
        /// Specifies the on clause for the last added join
        /// </summary>
        /// <param name="conditions">The set of conditions for the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery On(CriteriaSet conditions)
        {
            if (Joins.Count == 0)
            {
                throw new Exception("There is no JOIN specified in the query");
            }

            if (Joins[Joins.Count - 1].OnCondition != null)
            {
                throw new Exception("Current JOIN already has it's ON clause specified");
            }

            Joins[Joins.Count - 1].OnCondition = conditions;

            return this;
        }

        /// <summary>
        /// Specifies the where clause for the query
        /// </summary>
        /// <param name="conditions">The set of conditions for the query</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Where(CriteriaSet conditions) 
        {
            WhereCondition = conditions;

            return this;
        }

        /// <summary>
        /// Adds a field to the group by clause
        /// </summary>
        /// <param name="tableAlias">The alias of the table</param>
        /// <param name="column">The column name</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery GroupBy(string tableAlias, string column)
        {
            GroupByFields.Add(new ColumnReference(tableAlias, column));

            return this;
        }

        /// <summary>
        /// Adds an expression to the group clause
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery GroupBy(ISqlExpression expression)
        {
            GroupByFields.Add(expression);

            return this;
        }

        /// <summary>
        /// Adds a field to the group by clause
        /// </summary>
        /// <param name="field">The area field</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery GroupBy(FieldRef field)
        {
            GroupByFields.Add(new ColumnReference(field.Area, field.Field));

            return this;
        }

        /// <summary>
        /// Specifies the having clause for the query
        /// </summary>
        /// <param name="conditions">The set of conditions for the having clause</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Having(CriteriaSet conditions)
        {
            HavingCondition = conditions;

            return this;
        }

        /// <summary>
        /// Adds a column to the order by clause
        /// </summary>
        /// <param name="tableAlias">The alias of the table</param>
        /// <param name="column">The column name</param>
        /// <param name="sort">The sort direction</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery OrderBy(string tableAlias, string column, SortOrder sort)
        {
            OrderByFields.Add(new ColumnSort(new ColumnReference(tableAlias, column), sort));

            return this;
        }

        /// <summary>
        /// Adds an expression to the order by clause
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="sort">The sort direction</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery OrderBy(ISqlExpression expression, SortOrder sort)
        {
            OrderByFields.Add(new ColumnSort(expression, sort));

            return this;
        }

        /// <summary>
        /// Adds a column to the order by clause
        /// </summary>
        /// <param name="field">The area field</param>
        /// <param name="sort">The sort direction</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery OrderBy(FieldRef field, SortOrder sort)
        {
            OrderByFields.Add(new ColumnSort(new ColumnReference(field.Area, field.Field), sort));

            return this;
        }

        /// <summary>
        /// Adds a column by index to the order by clause
        /// </summary>
        /// <param name="index">The index of the column to sort</param>
        /// <param name="sort">The sort direction</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery OrderBy(int index, SortOrder sort)
        {
            OrderByFields.Add(new ColumnSort(index, sort));

            return this;
        }

        /// <summary>
        /// Adds a column to the order by clause
        /// </summary>
        /// <param name="columns">The object List of type ColumnSort containing the columns to use in the order by clause</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: JMT 2015.07.27
        /// Modified: 
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery OrderBy(List<ColumnSort> columns)
        {
        	foreach(ColumnSort column in columns)
        		OrderByFields.Add(column);

        	return this;
        }
		
        /// <summary>
        /// Sets if the query should return only the distinct values
        /// </summary>
        /// <param name="value">True if the query should return only the distinct values, otherwise false</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.13
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
		public SelectQuery Distinct(bool value)
		{
            DistinctProp = value;

			return this;
		}
		
        /// <summary>
        /// Sets the page of results the query should return
        /// </summary>
        /// <param name="value">The page number</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.13
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
		public SelectQuery Page(int value)
		{
            PageProp = value;

			return this;
		}
		
        /// <summary>
        /// Sets the size of a page of results of the query
        /// </summary>
        /// <param name="value">The page size</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.13
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
		public SelectQuery PageSize(int? value)
		{
            PageSizeProp = value;

			return this;
		}
		
        /// <summary>
        /// Sets the offset at which the pagination should start.
		/// If no page size is defined, sets the number of results that should be skipped.
        /// </summary>
        /// <param name="value">The offset value</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.13
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
		public SelectQuery Offset(int value)
		{
            OffsetProp = value;

			return this;
		}

        /// <summary>
        /// Adds a query to the union list
        /// </summary>
        /// <param name="query">The query to union with</param>
        /// <returns>The main query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Union(SelectQuery query)
        {
            UnionQueries.Add(new QueryUnion(query, false));

            return this;
        }

        public SelectQuery Union(SelectQuery query, bool all)
        {
            UnionQueries.Add(new QueryUnion(query, all));

            return this;
        }

        public object Clone()
        {
            SelectQuery result = new SelectQuery();
            foreach (SelectField field in SelectFields)
            {
                result.SelectFields.Add((SelectField)field.Clone());
            }
            if (FromTable != null)
            {
                result.FromTable = (ITableSource)FromTable.Clone();
            }
            foreach (TableJoin join in Joins)
            {
                result.Joins.Add((TableJoin)join.Clone());
            }
            if (WhereCondition != null)
            {
                result.WhereCondition = (CriteriaSet)WhereCondition.Clone();
            }
            foreach (ISqlExpression groupBy in GroupByFields)
            {
                result.GroupByFields.Add((ISqlExpression)groupBy.Clone());
            }
            foreach (ColumnSort sort in OrderByFields)
            {
                result.OrderByFields.Add((ColumnSort)sort.Clone());
            }
            result.DistinctProp = DistinctProp;
            result.PageProp = PageProp;
            result.PageSizeProp = PageSizeProp;
            result.OffsetProp = OffsetProp;
			result.noLock = noLock;
            result.updateLock = updateLock;
            foreach (QueryUnion query in UnionQueries)
            {
                result.UnionQueries.Add((QueryUnion)query.Clone());
            }

            return result;
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is SelectQuery))
			{
				return false;
			}

			SelectQuery other = (SelectQuery)obj;

			bool isEqual = this.DistinctProp == other.DistinctProp
				&& Object.Equals(this.FromTable, other.FromTable)
				&& Object.Equals(this.HavingCondition, other.HavingCondition)
				&& this.OffsetProp == other.OffsetProp
				&& this.PageProp == other.PageProp
				&& this.PageSizeProp == other.PageSizeProp
				&& Object.Equals(this.WhereCondition, other.WhereCondition)
				&& this.GroupByFields.Count == other.GroupByFields.Count
				&& this.Joins.Count == other.Joins.Count
				&& this.OrderByFields.Count == other.OrderByFields.Count
				&& this.SelectFields.Count == other.SelectFields.Count
				&& this.UnionQueries.Count == other.UnionQueries.Count;

			for (int i = 0; i < this.GroupByFields.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.GroupByFields[i], other.GroupByFields[i]);
			}

			for (int i = 0; i < this.Joins.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.Joins[i], other.Joins[i]);
			}

			for (int i = 0; i < this.OrderByFields.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.OrderByFields[i], other.OrderByFields[i]);
			}

			for (int i = 0; i < this.SelectFields.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.SelectFields[i], other.SelectFields[i]);
			}

			for (int i = 0; i < this.UnionQueries.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.UnionQueries[i], other.UnionQueries[i]);
			}

			return isEqual;
		}

		public override int GetHashCode()
		{
			int hash = this.DistinctProp.GetHashCode()
				^ (this.FromTable == null ? 0 : this.FromTable.GetHashCode())
				^ (this.HavingCondition == null ? 0 : this.HavingCondition.GetHashCode())
				^ this.OffsetProp.GetHashCode()
				^ this.PageProp.GetHashCode()
				^ (this.PageSizeProp == null ? 0 : this.PageSizeProp.GetHashCode())
				^ (this.WhereCondition == null ? 0 : this.WhereCondition.GetHashCode());

			for (int i = 0; i < this.GroupByFields.Count; i++)
			{
				hash ^= this.GroupByFields[i] == null ? 0 : this.GroupByFields[i].GetHashCode();
			}

			for (int i = 0; i < this.Joins.Count; i++)
			{
				hash ^= this.Joins[i] == null ? 0 : this.Joins[i].GetHashCode();
			}

			for (int i = 0; i < this.OrderByFields.Count; i++)
			{
				hash ^= this.OrderByFields[i] == null ? 0 : this.OrderByFields[i].GetHashCode();
			}

			for (int i = 0; i < this.SelectFields.Count; i++)
			{
				hash ^= this.SelectFields[i] == null ? 0 : this.SelectFields[i].GetHashCode();
			}

			for (int i = 0; i < this.UnionQueries.Count; i++)
			{
				hash ^= this.UnionQueries[i] == null ? 0 : this.UnionQueries[i].GetHashCode();
			}

			return hash;
		}
	}
}
