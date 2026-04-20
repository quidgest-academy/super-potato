using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence.GenericQuery;
using System.Data;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Compatibility Sql Server dialect for older versions (2008 minimum)
    /// </summary>
    /// <remarks>
    /// Override here only the methods or registrations that are different
    /// </remarks>
    public class SqlServerCompatDialect : SqlServerDialect
    {
        public override void AddLimitString(StringBuilder sql, int offset, int? limit)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Value cannot be lesser than 0");
            if (limit != null && limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit), limit, "Value cannot be lesser than 0");

            int? max = null;
            if (limit != null)
                max = UseMaxForLimit ? limit.Value : offset + limit.Value;

            string s = sql.ToString();
            int selectInsertPoint = s.StartsWith("SELECT DISTINCT") ? 15 : 6;

            if (offset == 0)
            {
                //first page is trivial top query
                if (max != null)
                    sql.Insert(selectInsertPoint, " TOP " + max.Value + " ");
                return;
            }

            // find the alias of the select fields
            IList<string> alias = new List<string>();
            int parentisisCount = 0;
            int lastSpace = selectInsertPoint;
            string lastWord = null;
            for (int i = selectInsertPoint + 1; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    lastWord = s.Substring(lastSpace + 1, i - lastSpace - 1);
                    lastSpace = i;
                }
                else if (s[i] == '(')
                {
                    parentisisCount++;
                }
                else if (s[i] == ')')
                {
                    parentisisCount--;
                }
                else if (parentisisCount == 0)
                {
                    if (s[i] == ',')
                    {
                        if (i > 0 && s[i - 1] != ' ')
                        {
                            lastWord = s.Substring(lastSpace + 1, i - lastSpace - 1);
                        }
                        lastSpace = i;
                        alias.Add(lastWord);
                    }
                    else if ((s[i] == 'F' || s[i] == 'f')
                        && s.IndexOf("FROM ", i, StringComparison.InvariantCultureIgnoreCase) == i)
                    {
                        alias.Add(lastWord);
                        break;
                    }
                }
            }

            // find the order by clause
            int orderByPoint = s.LastIndexOf("ORDER BY", StringComparison.InvariantCultureIgnoreCase);
            string orderClause = null;
            if (orderByPoint < 0)
            {
                string orderField = alias[0].Replace(".", "].[");
                orderClause = "ORDER BY " + orderField;
            }
            else
            {
                orderClause = s.Substring(orderByPoint);
                // insert new sql from bottom to top to keep the insert points correct
                // remove the order by clause, it will go to the ROW_NUMBER() OVER(...)
                sql.Length = orderByPoint;
            }
            // add the outter query to satisfy the limit
            string[] aliasArr = new string[alias.Count];
            alias.CopyTo(aliasArr, 0);
            sql.Insert(selectInsertPoint,
                " " + String.Join(",", aliasArr)
                + " FROM (SELECT " + (max != null ? "TOP " + max : "") + " ROW_NUMBER() OVER(" + orderClause + ") __genio_sort_row, ");
            sql.Append(") AS query WHERE __genio_sort_row > " + offset + " ORDER BY __genio_sort_row");
        }
    }
}