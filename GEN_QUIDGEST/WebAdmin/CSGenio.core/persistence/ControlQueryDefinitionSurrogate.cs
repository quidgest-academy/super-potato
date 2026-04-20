using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Quidgest.Persistence.GenericQuery;
using System.Runtime.Serialization;

namespace CSGenio.persistence
{
    [Serializable]
    public class ControlQueryDefinitionSurrogate
    {
        public class FunctionSurrogate
        {
            private SqlFunction m_object;
            [XmlIgnore]
            public SqlFunction Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }
                    return m_object;
                }
            }

            public List<object> Arguments
            {
                get;
                set;
            }

            public SqlFunctionType Type
            {
                get;
                set;
            }

            public FunctionSurrogate()
            {
                Arguments = new List<object>();
            }

            public FunctionSurrogate(SqlFunction obj) : this()
            {
                m_object = obj;
                Type = obj.Function;
                foreach (object arg in obj.Arguments)
                {
                    if (arg is ColumnReference)
                    {
                        Arguments.Add(new ColumnReferenceSurrogate((ColumnReference)arg));
                    }
                    else
                    {
                        Arguments.Add(arg);
                    }
                }
            }

            private void OnDeserialized()
            {
                List<object> args = new List<object>();
                foreach (object arg in Arguments)
	            {
                    if (arg is ColumnReferenceSurrogate)
                    {
                        args.Add(((ColumnReferenceSurrogate)arg).Object);
                    }
                    else
                    {
                        args.Add(arg);
                    }
	            }
                m_object = new SqlFunction(Type, args.ToArray());
            }
        }

        public class SelectFieldSurrogate
        {
            private SelectField m_object;
            [XmlIgnore]
            public SelectField Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }
                    return m_object;
                }
            }

            public ColumnReferenceSurrogate Column
            {
                get;
                set;
            }

            public FunctionSurrogate Function
            {
                get;
                set;
            }

            public string Alias
            {
                get;
                set;
            }

            public SelectFieldSurrogate()
            {
            }

            public SelectFieldSurrogate(SelectField obj)
            {
                m_object = obj;
                if (obj.Expression is ColumnReference)
                {
                    Column = new ColumnReferenceSurrogate((ColumnReference)obj.Expression);
                }
                else
                {
                    Function = new FunctionSurrogate((SqlFunction)obj.Expression);
                }
                Alias = obj.Alias;
            }

            private void OnDeserialized()
            {
                m_object = new SelectField(Column == null ? (ISqlExpression)Function.Object : Column.Object , Alias);
            }
        }

        public class ColumnReferenceSurrogate
        {
            private ColumnReference m_object;
            [XmlIgnore]
            public ColumnReference Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }
                    return m_object;
                }
            }

            public string TableAlias
            {
                get;
                set;
            }

            public string ColumnName
            {
                get;
                set;
            }

            public ColumnReferenceSurrogate()
            {
            }

            public ColumnReferenceSurrogate(ColumnReference obj)
            {
                m_object = obj;
                TableAlias = obj.TableAlias;
                ColumnName = obj.ColumnName;
            }

            private void OnDeserialized()
            {
                m_object = new ColumnReference(TableAlias, ColumnName);
            }
        }

        public class TableReferenceSurrogate
        {
            private TableReference m_object;
            [XmlIgnore]
            public TableReference Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }
                    return m_object;
                }
            }

            public string TableName
            {
                get;
                set;
            }

            public string TableAlias
            {
                get;
                set;
            }

            public string SchemaName
            {
                get;
                set;
            }

            public TableReferenceSurrogate()
            {
            }

            public TableReferenceSurrogate(TableReference obj)
            {
                m_object = obj;
                TableName = obj.TableName;
                TableAlias = obj.TableAlias;
                SchemaName = obj.SchemaName;
            }

            public void OnDeserialized()
            {
                m_object = new TableReference(SchemaName, TableName, TableAlias);
            }
        }

        public class TableJoinSurrogate
        {
            private TableJoin m_object;
            [XmlIgnore]
            public TableJoin Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }
                    return m_object;
                }
            }

            public TableReferenceSurrogate Table
            {
                get;
                set;
            }

            public TableJoinType JoinType
            {
                get;
                set;
            }

            public CriteriaSetSurrogate OnCondition
            {
                get;
                set;
            }

            public TableJoinSurrogate()
            {
            }

            public TableJoinSurrogate(TableJoin obj)
            {
                m_object = obj;
                Table = new TableReferenceSurrogate((TableReference)obj.Table);
                JoinType = obj.JoinType;
                OnCondition = new CriteriaSetSurrogate(obj.OnCondition);
            }

            private void OnDeserialized()
            {
                m_object = new TableJoin(Table.Object, OnCondition.Object, JoinType);
            }
        }

        public class CriteriaSetSurrogate
        {
            private CriteriaSet m_object;
            [XmlIgnore]
            public CriteriaSet Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }

                    return m_object;
                }
            }

            public CriteriaSetOperator Operation
            {
                get;
                set;
            }

            public List<CriteriaSurrogate> Criterias
            {
                get;
                set;
            }

            public List<CriteriaSetSurrogate> SubSets
            {
                get;
                set;
            }

            public CriteriaSetSurrogate()
            {
                Criterias = new List<CriteriaSurrogate>();
                SubSets = new List<CriteriaSetSurrogate>();
            }

            public CriteriaSetSurrogate(CriteriaSet obj)
                : this()
            {
                m_object = obj;
                Operation = obj.Operation;
                foreach (Criteria criteria in obj.Criterias)
                {
                    Criterias.Add(new CriteriaSurrogate(criteria));
                }
                foreach (CriteriaSet subSet in obj.SubSets)
                {
                    SubSets.Add(new CriteriaSetSurrogate(subSet));
                }
            }

            private void OnDeserialized()
            {
                m_object = new CriteriaSet(Operation);
                foreach (CriteriaSurrogate criteria in Criterias)
                {
                    m_object.Criterias.Add(criteria.Object);
                }
                foreach (CriteriaSetSurrogate subSet in SubSets)
                {
                    m_object.SubSets.Add(subSet.Object);
                }
            }
        }

        public class CriteriaSurrogate
        {
            private Criteria m_object;
            [XmlIgnore]
            public Criteria Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }
                    return m_object;
                }
            }

            public CriteriaOperator Operation
            {
                get;
                set;
            }

            public ColumnReferenceSurrogate LeftColumn
            {
                get;
                set;
            }

            public object LeftValue
            {
                get;
                set;
            }

            public ColumnReferenceSurrogate RightColumn
            {
                get;
                set;
            }

            public object RightValue
            {
                get;
                set;
            }

            public ControlQueryDefinitionSurrogate RightQuery
            {
                get;
                set;
            }

            public CriteriaSurrogate()
            {
            }

            public CriteriaSurrogate(Criteria obj)
            {
                m_object = obj;
                Operation = obj.Operation;
                
                if (obj.LeftTerm is ColumnReference)
                {
                    LeftColumn = new ColumnReferenceSurrogate((ColumnReference)obj.LeftTerm);
                }
                else
                {
                    LeftValue = obj.LeftTerm;
                }
                if (obj.RightTerm is ColumnReference)
                {
                    RightColumn = new ColumnReferenceSurrogate((ColumnReference)obj.RightTerm);
                }
                else if (obj.RightTerm is SelectQuery)
                {
                    RightQuery = new ControlQueryDefinitionSurrogate((SelectQuery)obj.RightTerm);
                }
                else
                {
                    RightValue = obj.RightTerm;
                }
            }

            private void OnDeserialized()
            {
                object rightTerm = null;

                if (RightColumn != null)
                    rightTerm = RightColumn.Object;
                else if (RightValue != null)
                    rightTerm = RightValue;
                else if (RightQuery != null)
                    rightTerm = RightQuery.Object.ToSelectQuery();

                m_object = new Criteria(
                    LeftColumn == null ? LeftValue : LeftColumn.Object,
                    Operation,
                    rightTerm);
            }
        }

        public class ColumnSortSurrogate
        {
            private ColumnSort m_object;
            [XmlIgnore]
            public ColumnSort Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }

                    return m_object;
                }
            }

            public int? ColumnIndex
            {
                get;
                set;
            }

            public ColumnReferenceSurrogate Column
            {
                get;
                set;
            }

            public SortOrder Order
            {
                get;
                set;
            }

            public ColumnSortSurrogate()
            {
            }

            public ColumnSortSurrogate(ColumnSort obj)
            {
                m_object = obj;
                ColumnIndex = obj.ColumnIndex;
                Column = new ColumnReferenceSurrogate((ColumnReference)obj.Expression);
                Order = obj.Order;
            }

            private void OnDeserialized()
            {
                if (ColumnIndex == null)
                {
                    m_object = new ColumnSort(Column.Object, Order);
                }
                else
                {
                    m_object = new ColumnSort(ColumnIndex.Value, Order);
                }
            }
        }

        public class QueryReferenceSurrogate
        {
            private QueryReference m_object;
            [XmlIgnore]
            public QueryReference Object
            {
                get
                {
                    if (m_object == null)
                    {
                        OnDeserialized();
                    }

                    return m_object;
                }
            }

            public ControlQueryDefinitionSurrogate Query
            {
                get;
                set;
            }

            public string TableAlias
            {
                get;
                set;
            }

            private void OnDeserialized()
            {
                m_object = new QueryReference(Query.Object.ToSelectQuery(), TableAlias);
            }
        }

        private CSGenio.persistence.PersistentSupport.ControlQueryDefinition m_object;
        [XmlIgnore]
        public CSGenio.persistence.PersistentSupport.ControlQueryDefinition Object
        {
            get
            {
                if (m_object == null)
                {
                    OnDeserialized();
                }

                return m_object;
            }
        }

        public bool Distinct
        {
            get;
            set;
        }

        public List<SelectFieldSurrogate> SelectFields
        {
            get;
            set;
        }

        public TableReferenceSurrogate FromTable
        {
            get;
            set;
        }

        public QueryReferenceSurrogate FromQuery
        {
            get;
            set;
        }

        public List<TableJoinSurrogate> Joins
        {
            get;
            set;
        }

        public CriteriaSetSurrogate WhereConditions
        {
            get;
            set;
        }

        public ControlQueryDefinitionSurrogate()
        {
            SelectFields = new List<SelectFieldSurrogate>();
            Joins = new List<TableJoinSurrogate>();
        }

        public ControlQueryDefinitionSurrogate(CSGenio.persistence.PersistentSupport.ControlQueryDefinition obj)
            : this()
        {
            m_object = obj;
            Distinct = obj.Distinct;
            foreach (SelectField field in obj.SelectFields)
            {
                SelectFields.Add(new SelectFieldSurrogate(field));
            }
            FromTable = new TableReferenceSurrogate((TableReference)obj.FromTable);
            foreach (TableJoin join in obj.Joins)
            {
                Joins.Add(new TableJoinSurrogate(join));
            }
            WhereConditions = new CriteriaSetSurrogate(obj.WhereConditions);
        }

        private static CSGenio.persistence.PersistentSupport.ControlQueryDefinition SelectQueryToControlQueryDefinition(Quidgest.Persistence.GenericQuery.SelectQuery obj)
        {
            return new PersistentSupport.ControlQueryDefinition(obj.SelectFields, obj.FromTable, obj.Joins, obj.WhereCondition, obj.DistinctProp);
        }

        public ControlQueryDefinitionSurrogate(Quidgest.Persistence.GenericQuery.SelectQuery obj)
            : this(SelectQueryToControlQueryDefinition(obj))
        {
        }

        private void OnDeserialized()
        {
            ITableSource from = null;

            if (FromTable != null)
                from = FromTable.Object;
            else if (FromQuery != null)
                from = FromQuery.Object;

            m_object = new CSGenio.persistence.PersistentSupport.ControlQueryDefinition(new List<SelectField>(), from, new List<TableJoin>(), WhereConditions.Object, Distinct);
            foreach (SelectFieldSurrogate field in SelectFields)
            {
                m_object.SelectFields.Add(field.Object);
            }
            foreach (TableJoinSurrogate join in Joins)
            {
                m_object.Joins.Add(join.Object);
            }
        }
    }
}
