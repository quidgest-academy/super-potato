using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.business
{
    /// <summary>
    /// Aggregates the sources of formulas for each area in order to minimize the number of querys needed
    /// to calculate those formulas
    /// </summary>
    public class FormulaDbContext
    {

        /// <summary>
        /// Creates a new FormulaDbContext
        /// </summary>
        /// <param name="area">The base area for the context</param>
        public FormulaDbContext(Area area)
        {
            areaBase = area;
        }

        private readonly Area areaBase;
        private readonly Dictionary<string, Dictionary<string, Area>> m_readContext = new Dictionary<string, Dictionary<string, Area>>();
        private readonly Dictionary<string, Dictionary<string, Area>> m_updateContext = new Dictionary<string, Dictionary<string, Area>>();
        private readonly Dictionary<string, HashSet<string>> m_metaContext = new Dictionary<string, HashSet<string>>();
        private readonly HashSet<string> m_updlockTables = new HashSet<string>();


        private Dictionary<string, Area> GetReadTable(string target)
        {
            if (!m_readContext.TryGetValue(target, out var result))
            {
                result = new Dictionary<string, Area>();
                m_readContext.Add(target, result);
            }
            return result;
        }

        /// <summary>
        /// Ensures the information for a record is available to the formulas
        /// from the database in case it hasn't been read or from memory if it was already read.
        /// It will only fetch the columns that were added as formula sources.
        /// </summary>
        /// <param name="area">The area to fetch</param>
        /// <param name="pk">The primary key of the record to fetch</param>
        /// <param name="sp">Persisistent support where to fetch data from</param>
        /// <returns>The requested record</returns>
        public Area ReadRecord(string area, string pk, PersistentSupport sp)
        {
            if(string.IsNullOrEmpty(pk))
            {
                var user = areaBase.User;
                return Area.createArea(area, user, user.CurrentModule);
            }

            var rowContext = GetReadTable(area);
            if (!rowContext.TryGetValue(pk, out var result))
            {
                var user = areaBase.User;
                result = Area.createArea(area, user, user.CurrentModule);
                m_metaContext.TryGetValue(area, out var fields);
                sp.getRecord(result, pk, fields.ToArray(), m_updlockTables.Contains(area));
                result.QPrimaryKey = pk;
                rowContext.Add(pk, result);
            }
            return result;
        }




        private Dictionary<string, Area> GetUpdateTable(string target)
        {
            if (!m_updateContext.TryGetValue(target, out var result))
            {
                result = new Dictionary<string, Area>();
                m_updateContext.Add(target, result);
            }
            return result;
        }

        /// <summary>
        /// Creates a record proxy to be updated.
        /// The record should only set the columns that need to be updated.
        /// Multiple update to the same row will be aggregated into a single update.
        /// The updates are collected and will only be sent to the database with the RunPropagations method.
        /// </summary>
        /// <param name="area">The area to update</param>
        /// <param name="pk">The primary key of the record to update</param>
        /// <returns>A record ready to have its columns modified</returns>
        public Area UpdateRecord(string area, string pk)
        {
            var rowContext = GetUpdateTable(area);
            if (!rowContext.TryGetValue(pk, out var result))
            {
                var user = areaBase.User;
                //if there is an already positioned read row with bookmarks reuse those values
                var readContext = GetReadTable(area);
                if (readContext.TryGetValue(pk, out var read) && read.IsBookmarkLocked)
                {
                    result = Area.createFromBookmark(read);
                }
                //otherwise allocate a new empty area with no assumptions to force a fresh bookmark
                else
                {
                    result = Area.createArea(area, user, user.CurrentModule);
                    result.QPrimaryKey = pk;
                }
                result.UserRecord = false;
                rowContext.Add(pk, result);
            }
            return result;
        }

        /// <summary>
        /// Sends to the database all the pending updates
        /// </summary>
        /// <param name="sp">Persisistent support where to update data</param>
        public void RunPropagations(PersistentSupport sp)
        {
            foreach (var area in m_updateContext.Values)
                foreach (var row in area.Values)
                    row.change(sp, null);
        }

        private HashSet<string> GetMetaFields(string area)
        {
            if (!m_metaContext.TryGetValue(area, out var fields))
            {
                fields = new HashSet<string>();
                m_metaContext.Add(area, fields);
            }
            return fields;
        }

        /// <summary>
        /// Adds a single ad-hoc formula to the field sources
        /// </summary>
        /// <param name="f">The formula to register</param>
        public void AddFormulaSources(InternalOperationFormula f)
        {
            AddFormulaSources(f.ByAreaArguments);
        }

        /// <summary>
        /// Adds the fields in the specified arguments list to the field sources
        /// </summary>
        /// <param name="args">The list of arguments</param>
        public void AddFormulaSources(List<ByAreaArguments> args)
        {
            foreach (ByAreaArguments arg in args)
            {
                HashSet<string> fields = GetMetaFields(arg.AliasName);
                foreach (string f in arg.FieldNames)
                    fields.Add(f);
            }
        }

        /// <summary>
        /// Makes the context aware of all the default formula sources
        /// </summary>
        public void AddDefaults()
        {
            if (areaBase.DefaultValues != null)
            {
                string[] valoresDefault = areaBase.DefaultValues;
                for (int i = 0; i < valoresDefault.Length; i++)
                {
                    var info = areaBase.DBFields[valoresDefault[i]];
                    if (info.DefaultValue.tpDefault == DefaultValue.DefaultType.OP_INT)
                    {
                        var f = info.DefaultValue.DefaultFormula;
                        AddFormulaSources(f);
                    }
                }
            }
        }

        /// <summary>
        /// Makes the coxtext aware of all the arithmetic formula sources
        /// </summary>
        public void AddInternalOperations()
        {
            foreach (Field Qfield in areaBase.Information.DBFieldsList)
            {
                if (Qfield.Formula is InternalOperationFormula)
                {
                    var f = Qfield.Formula as InternalOperationFormula;
                    AddFormulaSources(f);
                }
            }

            //There is no reason to add only internal formulas when they need all these 3 things
            AddReplicas();
            AddDefaults();
        }


        /// <summary>
        /// Makes the context aware of all the replica formula sources
        /// </summary>
        private void AddReplicas()
        {
            foreach (Field Qfield in areaBase.Information.DBFieldsList)
            {
                if (Qfield.Formula is ReplicaFormula)
                {
                    var f = Qfield.Formula as ReplicaFormula;
                    //convert the replica physical relation target into a logic relation target
                    var alias = areaBase.ParentTables[f.Alias].AliasTargetTab;
                    var fields = GetMetaFields(alias);
                    fields.Add(f.Field);
                }
            }
        }

        /// <summary>
        /// Makes the context aware of all the propagation target fields
        /// This is often needed so that the current target value can be compared with the calculated target value.
        /// </summary>
        public void AddPropagations()
        {
            AddUV();
            AddSR();
            AddLG();
        }

        public void AddWholeRow(HashSet<string> fields, string area)
        {
            var info = Area.GetInfoArea(area);
            foreach (Field fieldInfo in info.DBFields.Values)
                fields.Add(fieldInfo.Name);
        }

        private void AddUV()
        {
            if (areaBase.LastValueArgs == null)
                return;
            foreach (var arg in areaBase.LastValueArgs)
            {
                var fields = GetMetaFields(arg.AliasRUV);
                foreach (string lv in arg.LVRFields)
                    fields.Add(lv);
            }
        }
        private void AddSR()
        {
            if (areaBase.RelatedSumArgs == null)
                return;
            foreach (var arg in areaBase.RelatedSumArgs)
            {
                var fields = GetMetaFields(arg.AliasSR);
                //A SR will need to update the target row, so we might as well pre-prosition it completely
                //This allows getBookmark to save a query as long as we update lock it.
                //fields.Add(arg.SRField);
                AddWholeRow(fields, arg.AliasSR);
                //these areas need to be marked for update
                //SR are diferencial so they are very sensitive to concurrent updates
                //making sure their reads are done with UPDLOCK is the only way
                //to atomize the select operation that reads the current value with
                //the future update that will actually update the database.
                m_updlockTables.Add(arg.AliasSR);
            }
        }
        private void AddLG()
        {
            if (areaBase.ArgsListAggregate == null)
                return;
            foreach (var arg in areaBase.ArgsListAggregate)
            {
                var fields = GetMetaFields(arg.AliasLG);
                fields.Add(arg.LGField);
            }
        }


        /// <summary>
        /// Caches the data from an alread read area overriding any existing values.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <remarks>Can be usefull if you need formulas to use memory values instead of database values</remarks>
        public void SetArea(Area area)
        {
            var rowContext = GetReadTable(area.Alias);
            if (rowContext.ContainsKey(area.QPrimaryKey))
                rowContext[area.QPrimaryKey] = area;
            else
                rowContext.Add(area.QPrimaryKey, area);
        }

		// Cache the Glob internal code (it should never change)
		private static string m_codglob = null;
        private static object m_codglobLock = new object();

        /// <summary>
        /// Gets the value of the foreign key to an area.
        /// The main objective of this method is to support GLOB table.
        /// </summary>
        /// <param name="area">The target area of the relation</param>
        /// <param name="nomeChave">The name of the foreign key</param>
        /// <param name="sp">A persistent support in case we need to fetch the value from the database</param>
        /// <returns>The value of the foreign key</returns>
        public string GetForeignKeyValue(string area, string nomeChave, PersistentSupport sp)
        {
            //Go fetch the primary key of the table to which the fields belong
            //Verify that the areaField contains the foreign key that corresponds to the primary key
            //field we're looking for, if it doesn't count then you have to go read the database
            string valorChaveEst;

            if (area == "glob")
            {
                lock(m_codglobLock)
                {
                    if (m_codglob == null)
                    {
                        //Go fetch the value of the primary key from the GLOB table
                        var a = Area.GetInfoArea(area);
                        SelectQuery qs = new SelectQuery()
                            .Select(a.TableName, a.PrimaryKeyName)
                            .From(a.QSystem, a.TableName, a.TableName)
                            .PageSize(1);

                        object Qresult = sp.ExecuteScalar(qs);
                        if (Qresult != null)
                            m_codglob = DBConversion.ToKey(Qresult);
                        else
                            m_codglob = "";
                    }
					valorChaveEst = m_codglob;
                }
            }
            else
            {
                // AV (2010/06/04) The fields that were erased in the forms were to be overlapped with the value in the DB so we have to test whether the field is in memory
                if (!areaBase.Fields.ContainsKey(areaBase.Alias + "." + nomeChave))//If the value is not in memory go read to BD
                {
                    // TODO: The code should never have to come through here, if it passes we may have a problem of efficiency. You Must always read all the fields at once to the head. Review this if block.
                    string codIntValue = areaBase.QPrimaryKey;
                    if (codIntValue == "")
                        codIntValue = null;
                    valorChaveEst = DBConversion.ToKey(sp.returnField(areaBase, nomeChave, codIntValue));
                    areaBase.insertNameValueField(areaBase.Alias + "." + nomeChave, valorChaveEst);
                }
                else
                    valorChaveEst = (string)areaBase.returnValueField(areaBase.Alias + "." + nomeChave);
            }

            return valorChaveEst;
        }
    }
}
