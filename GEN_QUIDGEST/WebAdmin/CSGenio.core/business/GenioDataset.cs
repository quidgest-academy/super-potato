using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CSGenio.core.business;

/// <summary>
/// Support class for block crud operations that need to ensure business rules
/// </summary>
public class GenioDataset
{
    private readonly User _user;
    private readonly PersistentSupport _sp;
    private readonly Dictionary<string, Dictionary<string, Area>> m_tables = new Dictionary<string, Dictionary<string, Area>>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sp">Persistent support</param>
    /// <param name="user">User context</param>
    public GenioDataset(PersistentSupport sp, User user)
    {
        _user = user;
        _sp = sp;
    }


    /// <summary>
    /// Positions records by primary key and keeps a cache of records already positioned before
    /// </summary>
    /// <typeparam name="A">The specific class of Area to search</typeparam>
    /// <param name="key">The primary key to position</param>
    /// <param name="fields">The field we need from that record</param>
    /// <returns>The record found or an exception if it failed to find it</returns>
    public A Search<A>(string key, string[] fields = null) where A : Area
    {
        //check the cache for existing information
        //TODO: check if the fields requested are the same as the ones in cache
        string areaname = Area.GetInfoArea<A>().Alias;
        if (m_tables.TryGetValue(areaname, out var tableRows))
            if (tableRows.TryGetValue(key, out var row))
                return row as A;

        //otherwise go to the database
        A area = Area.createArea<A>(_user, _user.CurrentModule);
        _sp.getRecord(area, key, fields);

        //ensure the dataset has the table
        if (!m_tables.ContainsKey(areaname))
            m_tables.Add(areaname, new Dictionary<string, Area>());

        //cache the row in the dataset
        m_tables[areaname].Add(key, area);

        return area;
    }


    /// <summary>
    /// Insert multiple rows in bulk to the database and run all the business rules
    /// </summary>
    /// <param name="rows">The rows to insert</param>
    /// <remarks>Primary keys will be set in the same input rows when this function returns</remarks>
    public void InsertBlock(IEnumerable<Area> rows)
    {
        if (!rows.Any())
            return;
        var info = rows.First().Information;

        //allocate primary keys for every record
        //guids are ok, but ints and strings should be allocated in block
        {
            Field chaveinfo = info.DBFields[info.PrimaryKeyName];
            var prealloc_pks = _sp.generatePrimaryKey(info.TableName, info.PrimaryKeyName, chaveinfo.FieldSize, info.KeyType, rows.Count());
            int i = 0;
            foreach (var row in rows)
            {
                row.QPrimaryKey = prealloc_pks[i++];
                row.Zzstate = 0; //should it start at 1 instead of zero?
                                 //calc block wont change it after so it would require an additional update
                row.fillStampInsert();
            }
        }

        //rows should be sorted by pk to facilitate matching
        var sortedRows = rows.ToList();
        sortedRows.Sort((x, y) => x.QPrimaryKey.CompareTo(y.QPrimaryKey));

        //before insert, setup any columns that use sequential numbers
        CalculateSequencialNumbers(sortedRows, null, info);

        //ST aggregations need to insert the group records before the propagations occur
        //and that will fill a FK field that must be updated in the row
        CreateStRecords(sortedRows, info);

        //insert all of them into the database
        _sp.bulkInsert(sortedRows);

        //run the calcblock procedure over the list of inserted records
        var newvalues = CalculateBlock(sortedRows.Select(x => x.QPrimaryKey), info);

        //give back the results to the application can chain this call
        for (int i = 0; i < sortedRows.Count; i++)
            sortedRows[i].CloneFrom(newvalues[i]);
    }



    /// <summary>
    /// Update multiple rows in bulk to the database and run all the business rules
    /// </summary>
    /// <param name="rows">The rows to update</param>
    /// <param name="oldrows">The oldvalues of the rows in case you already have them, or null to fetch them from the database</param>
    public void UpdateBlock(IEnumerable<Area> rows, IEnumerable<Area> oldrows = null)
    {
        if (!rows.Any())
            return;

        AreaInfo info = rows.First().Information;

        //force the zzstate to 0 if it isn't already
        foreach (var row in rows)
        {
            row.Zzstate = 0;
            row.fillStampChange();
        }

        //rows should be sorted by pk to facilitate matching
        var sortedrows = rows.ToList();
        sortedrows.Sort((x, y) => x.QPrimaryKey.CompareTo(y.QPrimaryKey));

        //fetch oldvalues (we assume the ones given to us in rows are new values)
        var keys = rows.Select(x => x.QPrimaryKey);
        List<Area> sortedoldrows;
        if (oldrows != null)
        {
            sortedoldrows = oldrows.ToList();
            sortedoldrows.Sort((x, y) => x.QPrimaryKey.CompareTo(y.QPrimaryKey));
        }
        else
        {
            sortedoldrows = GetCurrentSnapshot(keys, info);
        }

        //recalculate any sequential numbers that might have changed
        CalculateSequencialNumbers(sortedrows, sortedoldrows, info);

        //ST aggregations need to insert the group records before the propagations occur
        //and that will fill a FK field that must be updated in the row
        CreateStRecords(sortedrows, info);

        //update user defined fields
        _sp.bulkUpdate(sortedrows);

        //calculate the formulas
        var newvalues = CalculateBlock(keys, info, sortedoldrows);

        //give back the results to the application so it can chain this call
        for (int i = 0; i < sortedrows.Count; i++)
            sortedrows[i].CloneFrom(newvalues[i]);
    }


    /// <summary>
    /// Delete multiple rows in bulk to the database and run all the business rules
    /// </summary>
    /// <param name="rows">The rows to delete</param>
    public void DeleteBlock(IEnumerable<Area> rows)
    {
        if (!rows.Any())
            return;
        var info = rows.First().Information;
        var keys = rows.Select(x => x.QPrimaryKey);

        //check for child records
        DeleteDependencies(keys, info);

        //delete does not need to recalculate the self table
        _sp.bulkDelete(rows);

        //analize the values to determine what other rows in other tables should be recalculated
        Propagations propagations = new Propagations();

        foreach (var oldrow in rows)
        {
            DetermineClPropagation(propagations, null, oldrow);
            DetermineSrPropagation(propagations, null, oldrow);
            DetermineUvPropagation(propagations, null, oldrow);
            DetermineFpPropagation(propagations, null, oldrow);

            //delete does not propagate replicas, at most it could clear all the dependent fields
            //however, most of the time those child rows are also deleted in cascade.

            //message queues need to be sent out for each deleted record
            oldrow.insertQueue(_sp, "D", null, null);
            oldrow.MessageQueue(_sp, "D", null);
        }

        RecursePropagations(propagations);
    }


    /// <summary>
    /// Invokes the Calc Block stored procedure and handles the necessary propagations
    /// </summary>
    /// <param name="keys">The primary keys of the rows</param>
    /// <param name="info">The area information</param>
    /// <param name="oldvalues">The current row values</param>
    /// <returns>A list with the new row values</returns>
    private List<Area> CalculateBlock(IEnumerable<string> keys, AreaInfo info, ICollection<Area> oldvalues = null)
    {
        //update in block the rows
        InvokeCalcBlock(keys, info);

        //fetch newvalues
        List<Area> newvalues = GetCurrentSnapshot(keys, info);

        //validate
        ValidateRows(newvalues);

        //analize the values to determine what other rows in other tables should be recalculated
        Propagations propagations = new();

        int ixMatch = 0;
        foreach (var newrow in newvalues)
        {
            //match up the new row with the old row
            var oldrow = oldvalues?.ToList()[ixMatch++];

            DetermineClPropagation(propagations, newrow, oldrow);
            DetermineSrPropagation(propagations, newrow, oldrow);
            DetermineUvPropagation(propagations, newrow, oldrow);
            DetermineFpPropagation(propagations, newrow, oldrow);

            //propagate replicas. The business code only does the update, it does not recalculate the target (to avoid loops)
            PropagateReplicas(newrow, oldrow);

            //TODO:
            //ST aggregate formulas need to insert rows when they don't exist
            //createHistory also needs to insert rows

            //message queues need to be sent out for each changed record
            if (newrow.Zzstate == 0)
            {
                var op = oldvalues is null || oldrow.Zzstate != 0 ? "C" : "U";
                newrow.insertQueue(_sp, op, oldrow, null);
                newrow.MessageQueue(_sp, op, oldrow);
            }
        }

        //continue the propagation chain to the affected rows
        RecursePropagations(propagations);

        //create audit trail rows
        CreateHist(newvalues, oldvalues, info);

        return newvalues;
    }

    private void ValidateRows(List<Area> newvalues)
    {
        foreach (var row in newvalues)
        {
            if (row.UserRecord)
            {
                var validationResults = Validation.validateFieldsChange(row, _sp, _user);
                if (validationResults.HasError)
                    throw new Exception(validationResults.Message);
            }
        }
    }

    private void RecursePropagations(Propagations propagations)
    {
        //iterate recursively. The assumption is that the rows will eventually stabilize
        // without any field value changes, and the propagations will then be empty.
        foreach (var target in propagations.GetTables())
        {
            AreaInfo targetInfo = Area.GetInfoArea(target);
            var pks = propagations.GetPks(target);
            if (pks.Any())
            {
                //fetch oldvalues
                List<Area> targetoldvalues = GetCurrentSnapshot(pks, targetInfo);
                //propagate formulas
                CalculateBlock(pks, targetInfo, targetoldvalues);
            }
        }
    }


    private void CalculateSequencialNumbers(IEnumerable<Area> newrows, IEnumerable<Area> oldrows, AreaInfo info)
    {
        if (info.SequentialDefaultValues == null)
            return;

        foreach (var seqfield in info.SequentialDefaultValues)
        {
            var fieldInfo = info.DBFields[seqfield];
            var prefndupname = fieldInfo.PrefNDup == null ? "" : info.Alias + "." + fieldInfo.PrefNDup;
            var fieldname = info.Alias + "." + fieldInfo.Name;

            //filter the rows to the ones that actually need a new sequencial value:
            // - an insert operation
            // - an update changing to zzstate 0
            // - an update that changes the ndup prefix
            // - an update that still holds an invalid number
            var changedrows = newrows;
            if (oldrows != null)
            {
                changedrows = newrows.Zip(oldrows, (newrow, oldrow) => {
                    var needsUpdate = (oldrow.Zzstate != 0)
                    || (fieldInfo.PrefNDup != null && !oldrow.returnValueField(prefndupname).Equals(newrow.returnValueField(prefndupname)))
                    || (Convert.ToInt32(newrow.returnValueField(fieldname)) < 0);
                    return needsUpdate ? newrow : null;
                }).Where(x => x != null).ToList();
            }

            //if no rows need change do nothing
            if (!changedrows.Any())
                continue;

            //base query for fetching the current largest number
            SelectQuery qs = new SelectQuery()
                .From(info.QSystem, info.TableName, info.Alias);
            qs.updateLock = true;

            if (fieldInfo.FieldFormat == FieldFormatting.FLOAT)
            {
                //If the field is already numeric the query is much simpler and more efficient
                qs.Select(SqlFunctions.Max(new ColumnReference(info.Alias, fieldInfo.Name)), "max")
                .Where(CriteriaSet.And()
                    .Equal(info.Alias, "zzstate", 0));
            }
            else
            {
                //Ignore non-numeric values
                qs.Select(SqlFunctions.Max(SqlFunctions.Cast(new ColumnReference(info.Alias, fieldInfo.Name), DbType.Int32)), "max")
                .Where(CriteriaSet.And()
                    .Equal(info.Alias, "zzstate", 0)
                    .Equal(SqlFunctions.SysCustom("IsNumeric", new ColumnReference(info.Alias, fieldInfo.Name)), 1)
                    .Greater(SqlFunctions.Cast(new ColumnReference(info.Alias, fieldInfo.Name), DbType.Int32), 0));
            }

            if (fieldInfo.PrefNDup != null)
            {
                //collect all unique prefixes
                var prefixes = new HashSet<string>();
                foreach (var row in changedrows)
                    prefixes.Add(row.returnValueField(prefndupname).ToString());

                //add the prefix field to the returned query and the group by
                qs.Select(info.Alias, fieldInfo.PrefNDup);
                qs.GroupBy(info.Alias, fieldInfo.PrefNDup);
                qs.WhereCondition.In(info.Alias, fieldInfo.PrefNDup, prefixes);

                //get largest value associated with each prefix
                var matrix = _sp.Execute(qs);

                //convert it into a more usable dictionary
                var prefixmap = new Dictionary<string, int>();
                for (int i = 0; i < matrix.NumRows; i++)
                    prefixmap.Add(matrix.GetString(i, 1), matrix.GetInteger(i, 0));
                //initialize non-existent prefixes at 0
                foreach (var prefix in prefixes)
                    if (!prefixmap.ContainsKey(prefix))
                        prefixmap.Add(prefix, 0);

                //fill in the corresponding value for each row, incrementing each value
                foreach (var row in changedrows)
                {
                    string prefix = row.returnValueField(prefndupname).ToString();
                    int currentSeq = prefixmap[prefix] + 1;
                    //support for manually set sequential numbers, as long as they don't collide with the unique series of the db
                    if (!fieldInfo.isEmptyValue(row.returnValueField(fieldname)))
                    {
                        var rowvalue = Convert.ToInt32(row.returnValueField(fieldname));
                        if (rowvalue > currentSeq)
                            currentSeq = rowvalue;
                    }
                    row.insertNameValueField(fieldname, currentSeq);
                    prefixmap[prefix] = currentSeq;
                }
            }
            else //no prefix, full table scan
            {
                //just get the next largest value
                int currentSeq = DBConversion.ToInteger(_sp.ExecuteScalar(qs));
                foreach (var row in changedrows)
                {
                    currentSeq++;
                    //support for manually set sequential numbers, as long as they don't collide with the unique series of the db
                    if (!fieldInfo.isEmptyValue(row.returnValueField(fieldname)))
                    {
                        var rowvalue = Convert.ToInt32(row.returnValueField(fieldname));
                        if (rowvalue > currentSeq)
                            currentSeq = rowvalue;
                    }
                    row.insertNameValueField(fieldname, currentSeq);
                }
            }
        }
    }


    private List<Area> GetCurrentSnapshot(IEnumerable<string> keys, AreaInfo info)
    {
        DataTable tableValueParam = QueryUtils.CreateKeyListType(keys);
        var criteria = CriteriaSet.And().In(info.Alias, info.PrimaryKeyName, tableValueParam);
        var rows = Area.searchList(info.Alias, _sp, _user, criteria);
        //rows should be sorted by pk to facilitate matching
        rows.Sort((x, y) => x.QPrimaryKey.CompareTo(y.QPrimaryKey));
        return rows;
    }


    private void InvokeCalcBlock(IEnumerable<string> keys, AreaInfo info)
    {
        //naming used in calcblock sproc is the physical table name without the system prefix
        //there are expections for views and for manual tables but those should not be part of block calculations
        string tableName = info.TableName.Substring(info.QSystem.Length).ToUpperInvariant();
        List<IDbDataParameter> paramList = [_sp.CreateParameter("x", keys)];
        _sp.ExecuteProcedure($"Genio_CalcBlock_{tableName}", paramList);
    }

    private class Propagations
    {
        private Dictionary<string, HashSet<string>> tablesToUpdate = new Dictionary<string, HashSet<string>>();

        //helper function to manage the dictionary
        private HashSet<string> GetRowsToUpdate(string target)
        {
            if (!tablesToUpdate.TryGetValue(target, out HashSet<string> result))
            {
                result = new HashSet<string>();
                tablesToUpdate.Add(target, result);
            }
            return result;
        }

        public void Add(string table, string pk) => GetRowsToUpdate(table).Add(pk);
        public IEnumerable<string> GetTables() => tablesToUpdate.Keys;
        public IEnumerable<string> GetPks(string table) => tablesToUpdate[table];
    }


    private void DetermineFpPropagation(Propagations propagations, Area newrow, Area oldrow)
    {
        AreaInfo info = oldrow?.Information ?? newrow?.Information;
        if (info.EndofPeriodFields == null)
            return;
        foreach (var campoFp in info.EndofPeriodFields)
        {
            Field fieldinfo = info.DBFields[campoFp];
            EndPeriodFormula ep = (EndPeriodFormula)fieldinfo.Formula;
            ep.DeterminePropagation(_sp, newrow, oldrow, (string alias, string pk, Area newrow, Area oldrow) =>
                propagations.Add(alias, pk));
        }
    }


    private void DetermineClPropagation(Propagations propagations, Area newrow, Area oldrow)
    {
        AreaInfo info = oldrow?.Information ?? newrow?.Information;
        if (info.ArgsListAggregate is null)
            return;
        foreach (var cl in info.ArgsListAggregate)
            cl.DeterminePropagation(newrow, oldrow, (string alias, string pk, Area newrow, Area oldrow) =>
                propagations.Add(alias, pk));
    }


    private void DetermineSrPropagation(Propagations propagations, Area newrow, Area oldrow)
    {
        AreaInfo info = oldrow?.Information ?? newrow?.Information;
        if (info.RelatedSumArgs is null)
            return;
        foreach (var sr in info.RelatedSumArgs)
            sr.DeterminePropagation(newrow, oldrow, (string alias, string pk, Area newrow, Area oldrow, decimal diff) =>
                propagations.Add(alias, pk));
    }


    private void DetermineUvPropagation(Propagations propagations, Area newrow, Area oldrow)
    {
        AreaInfo info = oldrow?.Information ?? newrow?.Information;
        if (info.LastValueArgs is null)
            return;
        foreach (var uv in info.LastValueArgs)
            uv.DeterminePropagation(newrow, oldrow, (string alias, string pk, Area newrow, Area oldrow) =>
                propagations.Add(alias, pk));
    }


    private void PropagateReplicas(Area newrow, Area oldrow)
    {
        //insertions don't need to propagate replicas since there is nothing to propagate to
        if (oldrow == null)
            return;

        AreaInfo info = oldrow.Information;

        if (info.FieldsParametersReplicas == null)
                return;

        //we group all the updates to a table into this dictionary
        Dictionary<string, UpdateQuery> updates = new Dictionary<string, UpdateQuery>();

        for (int i = 0; i < info.FieldsParametersReplicas.Length; i++)
        {
            Field campoReplica = info.DBFields[info.FieldsParametersReplicas[i]];
            object valorReplica = newrow.returnValueField(info.Alias + "." + campoReplica.Name);
            object oldReplica = oldrow.returnValueField(info.Alias + "." + campoReplica.Name);

            //only propagate if the value actually changed
            if (!valorReplica.Equals(oldReplica))
            {
                foreach (ReplicaDestination target in campoReplica.ReplicaDestinationList)
                {
                    if (!updates.TryGetValue(target.ReplicaDestinationTable + "_" + target.ForeignKey, out UpdateQuery uq))
                    {
                        // MH (25/09/2017) - Alterado to utilizar "destino.TabelaDestinoReplica" em vez do "Alias"  no Where do UpdateQuery.
                        // Alias referencia a table atual e não a table que vamos change. Ex: Alias: "factura" e TargetTable: "linhas da fatura".
                        uq = new UpdateQuery().Update(target.ReplicaDestinationSystem, target.ReplicaDestinationTable)
                            .Where(CriteriaSet.And().Equal(target.ReplicaDestinationTable, target.ForeignKey, newrow.QPrimaryKey));
                        updates.Add(target.ReplicaDestinationTable + "_" + target.ForeignKey, uq);
                    }

                    var dbValue = QueryUtils.ToValidDbValue(valorReplica, campoReplica);
                    uq.Set(target.ReplicaTargetFields, dbValue);
                }
            }
        }

        //TODO: Find a way to aggregate replica propagations into a minimal set of queries
        // The dificulty here is that each row may need to propagate to different columns.
        // We could also try to batch all the updates, but this is a list update
        // and not a row update, hence, is incompatible with the new updateBlock in sp.

        //Then we do the actual updates to the database
        foreach (UpdateQuery uq in updates.Values)
            _sp.Execute(uq);
    }

    private void DeleteDependencies(IEnumerable<string> keys, AreaInfo info)
    {
        if (info.ChildTable == null)
            return;
        var tvp = QueryUtils.CreateKeyListType(keys);
        foreach (ChildRelation child in info.ChildTable)
        {
            if (child.ProcWhenDelete == DeleteProc.DM)
            {
                AreaInfo childInfo = Area.GetInfoArea(child.ChildArea);
                //construct an update query that clears the corresponding foreign keys
                foreach (var fk in child.RelatedFields)
                {
                    UpdateQuery uq = new UpdateQuery()
                        .Update(childInfo.QSystem, childInfo.TableName)
                        .Set(fk, null)
                        .Where(CriteriaSet.And().In(childInfo.TableName, fk, tvp));
                    _sp.Execute(uq);
                }
            }
            else if (child.ProcWhenDelete == DeleteProc.AP)
            {
                //collect all the rows that need to be deleted, then cascade delete
                foreach (var fk in child.RelatedFields)
                {
                    var childrows = Area.searchList(child.ChildArea, _sp, _user, CriteriaSet.And().In(child.ChildArea, fk, tvp));
                    DeleteBlock(childrows);
                }
            }
            else if (child.ProcWhenDelete == DeleteProc.NA || child.ProcWhenDelete == DeleteProc.AN)
            {
                //Note: we assume this block delete is never being called in a pseudo-new record set (in that case AN needs to handle like AP instead of NA)
                //query for existing records and if there are any throw an exception
                foreach (var fk in child.RelatedFields)
                {
                    AreaInfo childInfo = Area.GetInfoArea(child.ChildArea);
                    var childrows = Area.searchList(child.ChildArea, _sp, _user, CriteriaSet.And().In(child.ChildArea, fk, tvp), [childInfo.PrimaryKeyName]);
                    if (childrows.Count > 0)
                        throw new BusinessException(
                            "This table has related records and can't be deleted.",
                            "GenioDataset.DeleteDependencies",
                            info.Alias.ToUpper() + " has related records and can't be deleted. The related table: " + childInfo.Alias.ToUpper());
                }
            }
        }
    }

    private void CreateHist(IEnumerable<Area> newrows, IEnumerable<Area> oldrows, AreaInfo info)
    {
        if (info.HistoryList == null)
            return;

        foreach (var histTable in info.HistoryList)
        {
            //collect changed rows
            var changedrows = newrows;
            if (oldrows != null)
            {
                changedrows = newrows.Zip(oldrows, (Area newrow, Area oldrow) => {
                    foreach (var histField in histTable.CreateHistFields)
                        if (!newrow.returnValueField(info.Alias + "." + histField).Equals(oldrow.returnValueField(info.Alias + "." + histField)))
                            return newrow;
                    return null;
                }).Where(x => x is not null).ToList();
            }

            if (!changedrows.Any())
                continue;

            //prepare the bulk insert list
            List<Area> histRows = new List<Area>();
            foreach (var row in changedrows)
            {
                Area areaHist = Area.createArea(histTable.CreateHistTables, _user, _user.CurrentModule);
                foreach (var histField in histTable.CreateHistFields)
                    areaHist.insertNameValueField(histTable.CreateHistTables + "." + histField, row.returnValueField(info.Alias + "." + histField));
                areaHist.insertNameValueField(histTable.CreateHistTables + "." + info.PrimaryKeyName, row.QPrimaryKey);
                histRows.Add(areaHist);
            }

            //bulk insert
            InsertBlock(histRows);
        }
    }


    private void CreateStRecords(IEnumerable<Area> newrows, AreaInfo info)
    {
        if (info.SumCreateRecords == null)
            return;

        foreach (var stFormula in info.SumCreateRecords)
        {
            //find out the unique combinations of (non-empty) st fields
            var groups = new Dictionary<string, object[]>();
            var rowkeys = new List<string>();
            foreach (var row in newrows)
            {
                var concatkey = new StringBuilder();
                var valueList = new object[stFormula.STSourceFields.Length];
                var hasValues = true;
                int vx = 0;
                foreach (var stField in stFormula.STSourceFields)
                {
                    var fieldInfo = info.DBFields[stField];
                    object value = row.returnValueField(info.Alias + "." + stField);
                    if (fieldInfo.isEmptyValue(value))
                    {
                        hasValues = false;
                        break;
                    }
                    concatkey.Append(value.ToString());
                    concatkey.Append("_");
                    valueList[vx++] = value;
                }

                if (hasValues)
                {
                    var key = concatkey.ToString();
                    if (!groups.ContainsKey(key))
                        groups.Add(key, valueList);
                    rowkeys.Add(key);
                }
                else
                    rowkeys.Add("");
            }

            //query the db to find out what combinations don't exist yet
            var inserts = new List<Area>();
            var groupkeys = new Dictionary<string, string>();
            foreach (var group in groups)
            {
                //TODO: optimize this so we can query all groups at the same time
                var foundrows = _sp.returnFieldsListConditions([stFormula.TargetIntKey], stFormula.TargetTable, stFormula.STTargetFields, group.Value);
                if (foundrows.Count == 0)
                {
                    //group does not exist yet so we need to create it
                    var strow = Area.createArea(stFormula.AliasTargetTab, _user, _user.CurrentModule);
                    for (int i = 0; i < stFormula.STTargetFields.Length; i++)
                        strow.insertNameValueField(stFormula.AliasTargetTab + "." + stFormula.STTargetFields[i], group.Value[i]);
                    inserts.Add(strow);
                }
                else
                {
                    //group already exists so we need to index its pk to the group concatkey
                    groupkeys.Add(group.Key, foundrows[0].ToString());
                }
            }

            //insert those rows
            InsertBlock(inserts);

            //add to the dictionary the pk for each inserted group
            foreach (var row in inserts)
            {
                var concatkey = new StringBuilder();
                foreach (var stField in stFormula.STTargetFields)
                {
                    concatkey.Append(row.returnValueField(stFormula.AliasTargetTab + "." + stField).ToString());
                    concatkey.Append("_");
                }
                groupkeys.Add(concatkey.ToString(), row.QPrimaryKey);
            }

            //fill in the fk's to the newly inserted st rows in the corresponding records
            int ix = 0;
            foreach (var row in newrows)
            {
                var rowkey = rowkeys[ix];
                if (rowkey != "" && groupkeys.TryGetValue(rowkey, out string pk))
                    row.insertNameValueField(info.Alias + "." + stFormula.TargetRelField, pk);
                ix++;
            }
        }
    }
}
