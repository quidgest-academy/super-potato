using CSGenio.business;
using CSGenio.framework;
using CSGenio.framework.Geography;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CSGenio.persistence
{
    public static class QueryUtils
    {
		private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static T[] ToArray<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
				throw new PersistenceException(null, "QueryUtils.ToArray<T>", "Enumerable is null.");
            }

            IList<T> aux = new List<T>(enumerable);
            T[] result = new T[aux.Count];
            aux.CopyTo(result, 0);

            return result;
        }

        public static void setFromTabDirect(SelectQuery query, IList<Relation> relations, IArea area)
        {
            setFromTabDirect(query, relations, area.Information);
        }

        public static void setFromTabDirect(SelectQuery query, IList<Relation> relations, AreaInfo area)
        {
            query.From(area.QSystem, area.TableName, area.Alias);

            foreach (Relation r in relations)
            {
                query.Join(r.TargetTable, r.AliasTargetTab, TableJoinType.Left)
                    .On(CriteriaSet.And()
                        .Equal(r.AliasSourceTab, r.SourceRelField, r.AliasTargetTab, r.TargetRelField));
            }
        }

        /// <summary>
        /// Adds fields of an area to be fetched from the database by the select query.
        /// Client side fields are removed.
        /// </summary>
        /// <param name="qs">The SelectQuery to be modified</param>
        /// <param name="area">The area to fetch fields from</param>
        /// <param name="fields">Optionally a list of specific fields you want. Default fetches all available fields</param>
        /// <returns>The modified SelectQuery. Its the same object it was passed in, just for chaining purposes.</returns>
        public static SelectQuery SelectDatabaseFields(this SelectQuery qs, IArea area, string[] fields = null)
        {
            if (fields != null && fields.Length > 0)
            {
                foreach (string field in fields)
                {
                    int ix = field.IndexOf('.');
                    if (ix == -1)
                    {
                        if (!area.DBFields[field].IsClientSide)
                            qs.Select(area.Alias, field);
                    }
                    else
                    {
                        string fieldarea = field.Substring(0, ix);
                        string fieldname = field.Substring(ix + 1);
                        Field cp;
                        if (fieldarea != area.Alias)
                            cp = Area.GetInfoArea(fieldarea).DBFields[fieldname];
                        else
                            cp = area.DBFields[fieldname];
                        if (!cp.IsClientSide)
                            qs.Select(cp.Alias, cp.Name);
                    }
                }
            }
            else
            {
                foreach (Field field in area.DBFields.Values)
                    if (!field.IsClientSide)
                    {
                        qs.Select(area.Alias, field.Name);
                    }
            }
            return qs;
        }

        /// <summary>
        /// Adds fields of an list to be fetched from the database by the select query.
        /// Client side fields are removed.
        /// This method supports requesting fields of different base areas.
        /// </summary>
        /// <param name="qs">The SelectQuery to be modified</param>
        /// <param name="area">The base area to fetch fields from</param>
        /// <param name="fields">A list of specific fields you want. Passing null fetched all fields of the base area.</param>
        /// <returns>The modified SelectQuery. Its the same object it was passed in, just for chaining purposes.</returns>
        public static SelectQuery SelectDatabaseFields(this SelectQuery qs, IArea area, FieldRef[] fields)
        {
            if (fields != null && fields.Length > 0)
            {
                foreach (var field in fields)
                {
                    Field cp;
                    if (field.Area != area.Alias)
                        cp = Area.GetInfoArea(field.Area).DBFields[field.Field];
                    else
                        cp = area.DBFields[field.Field];
                    if (!cp.IsClientSide)
                        qs.Select(cp.Alias, cp.Name);
                }
            }
            else
            {
                SelectDatabaseFields(qs, area);
            }
            return qs;
        }

        public static SelectQuery buildQueryCount(SelectQuery query)
        {
            SelectQuery newQuery = (SelectQuery)query.Clone();
            newQuery.SelectFields.Clear();
            newQuery.OrderByFields.Clear();

            // Base count query
            newQuery.Select(SqlFunctions.Count(1), "count")
                    .Offset(0)
                    .Page(1)
                    .PageSize(null);

            newQuery.noLock = query.noLock;

            return newQuery;
        }

        public static SelectQuery buildQueryTotalizers(SelectQuery query, List<FieldRef> fieldsWithTotalizer = null, List<string> selectedRecords = null, IArea areaBase = null)
        {
            // In order to improve efficiency, the totalizer query is integrated within the record count query.
            // This way, when loading a list(multiple selection) with totalizers and record count, only 2(3) queries are executed, instead of 3(4)
            // 1 - Record query
            // 2 - Count + totalizers
            // 3 - Selected records totalizers (multiple selection)
            SelectQuery newQuery = buildQueryCount(query);

            // Totalizer query
            if (fieldsWithTotalizer?.Any() == true)
            {
                newQuery.Select(new SqlValue("Total"), "RowId");
                foreach (FieldRef field in fieldsWithTotalizer)
                {
                    newQuery.Select(SqlFunctions.Sum(field), $"{field.FullName}");
                }
            }

            // Selected records totalizer query
            if (fieldsWithTotalizer?.Any() == true && selectedRecords?.Any() == true)
            {
                SelectQuery selectedQuery = (SelectQuery)query.Clone();
                selectedQuery.SelectFields.Clear();
                selectedQuery.OrderByFields.Clear();

                selectedQuery
                    .Select(new SqlValue(-1), "count") // Union needs the rows to have the same columns
                    .Select(new SqlValue("Selected"), "RowId")
                    .Offset(0)
                    .Page(1)
                    .PageSize(null)
                    .Where(CriteriaSet.And().In(areaBase.Alias, areaBase.PrimaryKeyName, selectedRecords));

                foreach (FieldRef field in fieldsWithTotalizer)
                {
                    selectedQuery.Select(SqlFunctions.Sum(field), $"{field.FullName}");
                }

                newQuery.Union(selectedQuery);
            }

            newQuery.noLock = query.noLock;

            return newQuery;
        }

        public static void buildQueryInsert(InsertQuery query, IArea area, bool ouputsPk = false)
        {
            query.Into(area.QSystem, area.TableName);

            foreach(var campoPedido in area.Fields.Values)
                // Foreign fields (EPHs) can be present in the requested fields list
                if(area.DBFields.TryGetValue(campoPedido.Name, out var campoBD) && !campoBD.IsVirtual)
                {
                    if(ouputsPk && campoBD.Name == area.Information.PrimaryKeyName)
                        query.Output(campoPedido.Name);
                    else
                        query.Value(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));
                }
        }

        public static object getRandomValue(Field Qfield)
        {
			// When you create a Random object, it's seeded with a value from the system clock.
            // If you create Random instances too close in time, they will all be seeded with the same random sequence,
            // which means you get the same value lots of times.
            // So, we should create a single Random object for the class, instead of creating one Random object each time the function is called.
            // See: http://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number
            //Random nr = new Random();
            //int Qvalue = nr.Next(-99999,0);
            //SJ (26.04.2007) Limitou-se o number random á largura do Qfield -1 (por causa do sign '-') por causa dos fields do tipo caracter

            int minValue = int.MinValue;
            if(Qfield.FieldSize < 11)
                // For higher FieldSize we have to assume minValue = int.MinValue, because otherwise minValue can become positive due to overflow.
                minValue = (int)(Math.Pow(10, Qfield.FieldSize - 1) * -1) + 1;

            //int Qvalue = nr.Next(minValue, 0);
			// Because Random is not thread safe, we need to do some synchronization
            int Qvalue;
            lock (syncLock)
            {
                Qvalue = random.Next(minValue, 0);
            }
            //a conversaoBd assume (e bem) que o Qfield ja vem no tipo certo, no entanto aqui queremos transformar o inteiro
            //em diferentes tipos consoante o tipo de Qfield.
            return convertIntegerToInterno(Qvalue, Qfield.FieldType.GetFormatting());
        }

        private static object convertIntegerToInterno(int Qvalue, FieldFormatting tipo)
        {
            switch (tipo)
            {
                case FieldFormatting.CARACTERES:
                    return Qvalue.ToString();
                case FieldFormatting.INTEIRO:
                    return Qvalue;
                case FieldFormatting.FLOAT:
                    return (decimal)Qvalue;
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    return DateTime.MinValue;
                default:
					throw new PersistenceException(null, "QueryUtils.convertIntegerToInterno", "Type not accepted: " + tipo.ToString());
            }
        }

        public static void buildQueryInsertShadow(InsertQuery query, IArea area, FunctionType functionType, string user)
        {
            query.Into(area.QSystem, area.ShadowTabName);

            foreach (var campoPedido in area.Fields.Values)
                // Foreign fields (EPHs) can be present in the requested fields list
                if (area.DBFields.TryGetValue(campoPedido.Name, out var campoBD) && !campoBD.IsVirtual)
                    query.Value(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));

            //20051207 nao esquecer de preencher o operdel e de criar operation
            //The operation is going to be hardcoded for now.
            query.Value("operacao", "D")
                .Value("operdel", user)
                .Value("datadel", DateTime.Now);
        }

        public static void fillQueryUpdate(UpdateQuery query, IArea area)
        {
            string QtableName = area.TableName.Trim();
            object pkVal = ToValidDbValue(area.QPrimaryKey, area.DBFields[area.PrimaryKeyName]);
            query.Update(area.QSystem, QtableName);
            query.Where(CriteriaSet.And()
                .Equal(QtableName, area.PrimaryKeyName, pkVal));

            foreach (var campoPedido in area.Fields.Values)
            {
                //only save fields that belong to this area
                if (!area.DBFields.TryGetValue(campoPedido.Name, out var campoBD))
                    continue;
                //we don't need to update the primary key
                if (campoPedido.Name.Equals(area.PrimaryKeyName))
                    continue;
                //virtual vields do not support updates
				if (campoBD.IsVirtual)
                    continue;
                //skip empty binary fields
                if ((campoBD.FieldType.Equals(FieldType.IMAGE) || campoBD.FieldType.Equals(FieldType.PATH)
                    || campoBD.FieldType.Equals(FieldType.MEMO_COMP_RTF))
                    && (campoBD.isEmptyValue(campoPedido.Value) || campoPedido.Value.ToString().StartsWith("*")))
                    continue;

                //skip non-dirty fields (the value stayed the same from the last know db read)
                if (!campoPedido.IsDirty())
                    continue;

                query.Set(campoPedido.Name, ToValidDbValue(campoPedido.Value, campoBD));
            }
        }

        public static void increaseQuery(SelectQuery query, IList<SelectField> select, ITableSource from, IList<TableJoin> joins, CriteriaSet where, int numRecords, CriteriaSet conditions, IList<ColumnSort> orderRequest, bool selectDistinct)
        {
            query.SelectFields.Clear();
            foreach (SelectField field in select)
            {
                query.SelectFields.Add(field);
            }
            query.FromTable = from;
            if (joins != null)
            {
                foreach (TableJoin join in joins)
                {
                    query.Joins.Add(join);
                }
            }
            query.WhereCondition = CriteriaSet.And()
                .SubSet(where)
                .SubSet(conditions);
            query.OrderByFields.Clear();
            if (orderRequest != null)
            {
                foreach (ColumnSort sort in orderRequest)
                {
                    query.OrderByFields.Add(sort);
                }
            }
            query.Distinct(selectDistinct);
            if (numRecords > 0)
            {
                query.PageSize(numRecords);
            }
        }

        public static void setWhereGetPos(SelectQuery query, IList<ColumnSort> sorting, IArea area, string primaryKeyValue)
        {
            if (query.WhereCondition == null)
            {
                query.Where(CriteriaSet.And());
            }

            if (sorting.Count == 0)
                throw new PersistenceException(null, "QuerySelect.setWhereGetPos", "No fields to order by.");

            SelectQuery innerQuery = new SelectQuery()
                .Select(sorting[0].Expression, "order")
                .From(area.QSystem, area.TableName, area.Alias)
                .Where(CriteriaSet.And()
                    .Equal(area.Alias, area.PrimaryKeyName, primaryKeyValue));
			if (innerQuery.Joins != null)
            {
                foreach (TableJoin join in query.Joins)
                {
                    innerQuery.Joins.Add(join);
                }
            }

            if (sorting[0].Order == SortOrder.Ascending)
            {
                query.WhereCondition.LesserOrEqual(sorting[0].Expression, innerQuery);
            }
            else
            {
                query.WhereCondition.GreaterOrEqual(sorting[0].Expression, innerQuery);
            }
        }

        public static void addWhere(SelectQuery query, CriteriaSet condition, CriteriaSetOperator conector)
        {
            if (condition != null)
            {
                if (query.WhereCondition == null)
                {
                    query.Where(new CriteriaSet(conector));
                }

                query.WhereCondition.SubSet(condition);
            }
        }

        public static void increaseQueryBetweenLines(SelectQuery query, IList<SelectField> select, ITableSource from, CriteriaSet where, CriteriaSet conditions, IList<ColumnSort> sorting, int firstLine, int lastLine, bool selectDistinct)
        {
            query.SelectFields.Clear();
            foreach (SelectField field in select)
            {
                query.SelectFields.Add(field);
            }
            query.FromTable = from;
            query.WhereCondition = CriteriaSet.And()
                .SubSet(where)
                .SubSet(conditions);
            query.OrderByFields.Clear();
            foreach (ColumnSort sort in sorting)
            {
                query.OrderByFields.Add(sort);
            }
            query.Distinct(selectDistinct).PageSize(lastLine - firstLine).Offset(firstLine);
        }

        public static CriteriaOperator ParseEphOperator(string ephOperator)
        {
            switch (ephOperator)
            {
                case "=":
                    return CriteriaOperator.Equal;
                case "<":
                    return CriteriaOperator.Lesser;
                case ">":
                    return CriteriaOperator.Greater;
                case "<=":
                    return CriteriaOperator.LesserOrEqual;
                case ">=":
                    return CriteriaOperator.GreaterOrEqual;
                case "!=":
                    return CriteriaOperator.NotEqual;
                default:
					throw new PersistenceException(null, "QueryUtils.ParseEphOperator", "Unknown operator: " + ephOperator);
            }
        }

        public static CriteriaOperator ParseSqlOperator(string sqlOperator)
        {
            switch (sqlOperator)
            {
                case "=":
                    return CriteriaOperator.Equal;
                case "<>":
                    return CriteriaOperator.NotEqual;
                case ">":
                    return CriteriaOperator.Greater;
                case ">=":
                    return CriteriaOperator.GreaterOrEqual;
                case "<":
                    return CriteriaOperator.Lesser;
                case "<=":
                    return CriteriaOperator.LesserOrEqual;
                case "LIKE":
                    return CriteriaOperator.Like;
                case "IN":
                    return CriteriaOperator.In;
                case "NOT IN":
                    return CriteriaOperator.NotIn;
                case "EXISTS":
                    return CriteriaOperator.Exists;
                case "NOT EXISTS":
                    return CriteriaOperator.NotExists;
                default:
					throw new PersistenceException(null, "QueryUtils.ParseSqlOperator", "Unknown operator: " + sqlOperator);
            }
        }

        public static void addSelect(SelectQuery query, IList<SelectField> fields)
        {
            foreach (SelectField field in fields)
            {
                query.SelectFields.Add(field);
            }
        }

        public static void increaseQuery(SelectQuery query, CSGenio.persistence.PersistentSupport.ControlQueryDefinition definition)
        {
            if (definition.SelectFields != null)
            {
                foreach (SelectField field in definition.SelectFields)
                {
                    query.SelectFields.Add(field);
                }
            }

            if (definition.FromTable != null)
            {
                query.FromTable = definition.FromTable;
            }

            if (definition.Joins != null)
            {
                foreach (TableJoin join in definition.Joins)
                {
                    query.Joins.Add(join);
                }
            }

            if (definition.WhereConditions != null)
            {
                if (query.WhereCondition == null)
                {
                    query.Where(CriteriaSet.And());
                }
                query.WhereCondition.SubSet(definition.WhereConditions);
            }

            query.Distinct(definition.Distinct);
        }

        public static object ToValidDbValue(object value, FieldType type)
        {
            //Empty keys and dates are represented as null in the database
            if ((type.IsKey() || value is DateTime) && Field.isEmptyValue(value, type.GetFormatting()))
                return DBNull.Value;

            //Convert keys into their correct database type
            if (type.IsKey())
            {
                if (type == FieldType.KEY_GUID)
                    return new Guid(value.ToString());
                else if (type == FieldType.KEY_INT)
                    return int.Parse(value.ToString());
                else if (type == FieldType.KEY_VARCHAR)
                    return value;
            }

            //Encrypt secure data before sending to database
            if (type == FieldType.ENCRYPTED)
                return (value as EncryptedDataType)?.EncryptedValue;

            //Truncate time of days of date-only fields
            if (type == FieldType.DATE)
                return ((DateTime)value).Date;

            //Convert custom type fields
            if (type == FieldType.MEMO_COMP_RTF)
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)))
                    return System.Text.Encoding.UTF8.GetBytes("");
                return System.Text.Encoding.UTF8.GetBytes(value.ToString());
            }
            if (type == FieldType.GEOGRAPHY_POINT)
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)))
                    return DBNull.Value;
                return Convert.ToString(value);
            }
            if (type == FieldType.GEOGRAPHY_SHAPE || type == FieldType.GEOMETRY_SHAPE)
            {
                if (value == null)
                    return DBNull.Value;
                return value.ToString();
            }

            return value;
        }

        public static object ToValidDbValue(object value, Field field)
        {
            return ToValidDbValue(value, field.FieldType);
        }

        // TODO: As seguintes funções devem ser revistos to poder reaproveitar...
        // tablesRelationships <= Tornar a função privada que irá receber SelectQuery aumentava query com Joins e devolvia SelectQuery
        // SetInnerJoins <=

        /// <summary>
        /// Método que permite obter a clausula from to fields que não estão em tables directamente referenciadas
        /// Este método devolve uma lista com os id's dos caminhos necessários to construir os queries
        /// </summary>
        /// <param name="tabIndAux">tables indirectamente relacionadas com a area</param>
        /// <param name="area">area do pedido</param>
        /// <returns>A lista de relações to chegar às areas pedidas</returns>
        public static List<Relation> tablesRelationships(List<string> tabIndAux, IArea area)
        {
            List<Relation> relations = new List<Relation>();
            foreach (string outra in tabIndAux)
            {
                //se já chegámos a esta table não precisamos dos caminhos
                if (relations.Exists(x => x.AliasTargetTab == outra))
                    continue;

                List<Relation> areasRelationships = area.Information.GetRelations(outra);
                if(areasRelationships == null)
                    continue;

                //combinar as relações to a nova area com as existentes
                foreach (Relation rel in areasRelationships)
                    if (!relations.Contains(rel))
                        relations.Add(rel);
            }
            return relations;
        }

        /// <summary>
        /// Checks fields used on the selectquery for different tables from the base area.
        /// Adds the different areas to the otherTables list
        /// </summary>
        /// <param name="fields">Array of used fields</param>
        /// <param name="area">The base area</param>
        /// <param name="otherTables">List of other tables</param>
        public static void checkFieldsForForeignTables(string[] fields, Area area, List<string> otherTables)
        {
            foreach (string field in fields)
            {
                string fieldArea = field.Split(new char[] { '.' })[0];
                if (!fieldArea.Equals(area.Alias) && !otherTables.Contains(fieldArea))
                    otherTables.Add(fieldArea);
            }
        }

        /// <summary>
        /// Checks the conditions used on the selectQuery for different tables from the base area.
        /// Adds the different areas to the otherTables list
        /// </summary>
        /// <param name="condicao">The condition used on the selectQuery</param>
        /// <param name="area">The base area</param>
        /// <param name="otherTables"></param>
        public static void checkConditionsForForeignTables(CriteriaSet condition, Area area, List<string> otherTables)
        {
            if (condition == null)
                return;
            foreach (Criteria c in condition.Criterias)
            {
                if (c.LeftTerm is ColumnReference)
                {
                    ColumnReference f = c.LeftTerm as ColumnReference;
                    if (!f.TableAlias.Equals(area.Alias) && !otherTables.Contains(f.TableAlias))
                        otherTables.Add(f.TableAlias);
                }
                else if (c.LeftTerm is Quidgest.Persistence.FieldRef)
                {
                    var f = c.LeftTerm as Quidgest.Persistence.FieldRef;
                    if (!f.Area.Equals(area.Alias) && !otherTables.Contains(f.Area))
                        otherTables.Add(f.Area);
                }
                else if (c.LeftTerm is SqlFunction)
                {
                    _checkConditionsSqlFunctionForForeignTables(c.LeftTerm as SqlFunction, area, otherTables);
                }
                else if(c.LeftTerm is SelectQuery)
                {
					//RMR(2019-07-02) - Exists operator does not imply a direct relation, as it can refere to a foreign key, or any other field
                    CriteriaOperator[] excludedOperators = new CriteriaOperator[] { CriteriaOperator.Exists, CriteriaOperator.NotExists };
                    if (excludedOperators.Contains(c.Operation))
                        continue;

                    var sq = c.LeftTerm as SelectQuery;
                    if (sq.FromTable != null && !sq.FromTable.TableAlias.Equals(area.Alias) && !otherTables.Contains(sq.FromTable.TableAlias))
                        otherTables.Add(sq.FromTable.TableAlias);
                    foreach (var f in sq.SelectFields)
                    {
                        if (!f.Alias.Equals(area.Alias) && !otherTables.Contains(f.Alias))
                            otherTables.Add(f.Alias);
                    }
                    checkConditionsForForeignTables(sq.WhereCondition, area, otherTables);
                }
                else if (c.LeftTerm is CriteriaSet)
                {
                    checkConditionsForForeignTables((CriteriaSet)c.LeftTerm, area, otherTables);
                }
            }

            // Need to apply recursion for subsets
            foreach (CriteriaSet subset in condition.SubSets)
                checkConditionsForForeignTables(subset, area, otherTables);
        }

        private static void _checkConditionsSqlFunctionForForeignTables(SqlFunction condition, Area area, List<string> otherTables)
        {
            foreach (var arg in condition.Arguments)
            {
                if (arg is ColumnReference)
                {
                    ColumnReference f = arg as ColumnReference;
                    if (!f.TableAlias.Equals(area.Alias) && !otherTables.Contains(f.TableAlias))
                        otherTables.Add(f.TableAlias);
                }
                else if (arg is Quidgest.Persistence.FieldRef)
                {
                    var f = arg as Quidgest.Persistence.FieldRef;
                    if (!f.Area.Equals(area.Alias) && !otherTables.Contains(f.Area))
                        otherTables.Add(f.Area);
                }
                else if (arg is SqlFunction)
                {
                    _checkConditionsSqlFunctionForForeignTables(arg as SqlFunction, area, otherTables);
                }
            }
        }

        public static void SetInnerJoins(string[] requestedFields, CriteriaSet conditions, Area area, SelectQuery querySelect)
        {
            // Look for inner joins
            List<string> tabelasAcima = new List<string>();
            if (querySelect.SelectFields != null)
                checkFieldsForForeignTables(requestedFields, area, tabelasAcima);
            if (conditions != null)
                checkConditionsForForeignTables(conditions, area, tabelasAcima);

            List<Relation> relations = tablesRelationships(tabelasAcima, area);
            setFromTabDirect(querySelect, relations, area);
        }

        /// <summary>
        /// Devolve os fields de uma área em format de text separados por vírgulas
        /// de mode a serem usados no SQL.
        /// Deve ser preterido sobre o "*", porque podem ter sido apagadas colunas
        /// nas definições, mas ainda permanecem no repositório SQL - a consequência é uma
        /// excepção no cliente, uma vez que são obtidas essas colunas no retorno da query;
        /// depois o cliente tenta aceder à sua estrutura correspondente à table nessas
        /// colunas e as mesmas não existem!
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        /// <remarks>eu queria usar o LINQ e recorrer à função acima, mas
        /// o GenioServer está feito sobre a plataforma .NET 2.0
        /// </remarks>
        public static StringBuilder fieldsToSQL(IArea area)
        {
            StringBuilder res = new StringBuilder(area.DBFields.Count * 8);

            foreach (Field Qfield in area.DBFields.Values)
                res.Append(Qfield.Name + ",");

            if (res.Length > 0)
                res.Remove(res.Length - 1, 1);

            return res;
        }

        /// <summary>
        /// Creates a KeyList compatible DataTable from a collection of keys
        /// </summary>
        /// <param name="keys">A collection of keys</param>
        /// <returns>The configured Datatable to use in Queries</returns>
        public static DataTable CreateKeyListType(IEnumerable<string> keys)
        {
            var tvp = new DataTable();
            tvp.TableName = "KeyListType";
            tvp.Columns.Add("item", typeof(string));
            foreach (var pk in keys)
                tvp.Rows.Add(pk);
            return tvp;
        }
    }
}
