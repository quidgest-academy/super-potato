using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CSGenio.business
{
    /// <summary>
    /// Descreve os tipos possíveis de fórmulas internas.
    /// </summary>
    public class EndPeriodFormula : Formula
    {
        private string campoData;//Qfield que marca o start de periodo
        private string campoAgrupar;//Qfield que permite agrupar

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="campoData">recebe o Qfield data</param>
        /// <param name="campoAgrupar">recebe o Qfield que agrupa</param>
        public EndPeriodFormula(string campoData, string campoAgrupar)
        {
            this.campoData = campoData;
            this.campoAgrupar = campoAgrupar;
        }

        /// <summary>
        /// Decrementa o Qvalue de fim de periodo de acordo com a formatação
        /// </summary>
        /// <param name="valor">O Qvalue to o início do periodo</param>
        /// <param name="formatacao">A formataçao do Qvalue</param>
        /// <returns>O Qvalue to o fecho do periodo</returns>
        private object DecFimPeriodo(object Qvalue, FieldFormatting formatting)
        {
            if (Field.isEmptyValue(Qvalue, formatting))
                return Field.GetValorEmpty(formatting);

            //TODO: Usar o FieldType e não o FieldFormatting, alias, só devia existir sempre o FieldType
            switch (formatting)
            {
                case FieldFormatting.DATA:
                    return ((DateTime)Qvalue).AddDays(-1);
                case FieldFormatting.INTEIRO:
                    return ((int)Qvalue) - 1;
                case FieldFormatting.DATAHORA:
                    return ((DateTime)Qvalue).AddMinutes(-1);
                case FieldFormatting.DATASEGUNDO:
                    return ((DateTime)Qvalue).AddSeconds(-1);
                case FieldFormatting.FLOAT:
                    return ((decimal)Qvalue) - 1;
                case FieldFormatting.TEMPO:
                    return HourFunctions.HoursAdd(Qvalue as string, -1);
                default:
                    return Qvalue;
            }
        }

        /// <summary>
        /// Propriedade to fazer get e set do Qfield data
        /// </summary>
        public string DateField
        {
            get { return campoData; }
            set { campoData = value; }
        }

        /// <summary>
        /// Propriedade to fazer get e set do Qfield que serve to agrupar
        /// </summary>
        public string GroupField
        {
            get { return campoAgrupar; }
            set { campoAgrupar = value; }
        }

        public object readEndPeriod(PersistentSupport sp, Area area, object start, object grouping, object keyValue)
        {
            SelectQuery querySelect = new SelectQuery()
				.Select(area.Alias, campoData)
                .From(area.QSystem, area.TableName, area.Alias)
				.Where(CriteriaSet.And()
					.NotEqual(area.Alias, area.PrimaryKeyName, keyValue)
					.Greater(area.Alias, campoData, start));
            if (campoAgrupar != null)
            {
                querySelect.WhereCondition.Equal(area.Alias, campoAgrupar, grouping);
            }
            querySelect.OrderBy(area.Alias, campoData, SortOrder.Ascending).PageSize(1);

            object result = sp.ExecuteScalar(querySelect);
            FieldFormatting formData = area.returnFormattingDBField(campoData);
            result = DBConversion.ToInternal(result, formData);
            result = DecFimPeriodo(result, formData);
            return result;
        }

        public string getPreviousRecord(PersistentSupport sp, AreaInfo area, object start, object grouping)
        {
            SelectQuery querySelect = new SelectQuery()
				.Select(area.Alias, area.PrimaryKeyName)
				.From(area.QSystem, area.TableName, area.Alias)
				.Where(CriteriaSet.And()
					.Equal(area.Alias, "zzstate", 0)
					.Lesser(area.Alias, campoData, start));
            if (campoAgrupar != null)
            {
                querySelect.WhereCondition.Equal(area.Alias, campoAgrupar, grouping);
            }
            querySelect.OrderBy(area.Alias, campoData, SortOrder.Descending).PageSize(1);

            object result = sp.ExecuteScalar(querySelect);
            return DBConversion.ToKey(result);
        }

        public List<string> getPreviousRecordList(PersistentSupport sp, List<Area> rows, AreaInfo info)
        {
            //inner query
            SelectQuery querySelect = new SelectQuery()
                .Select("previous", info.PrimaryKeyName)
                .From(info.QSystem, info.TableName, "previous")
                .Where(CriteriaSet.And()
                    .Equal("previous", "zzstate", 0)
                    .Lesser("previous", campoData, info.Alias, campoData));
            if (campoAgrupar != null)
            {
                querySelect.WhereCondition.Equal("previous", campoAgrupar, info.Alias, campoAgrupar);
            }
            querySelect
                .OrderBy("previous", campoData, SortOrder.Descending)
                .PageSize(1);

            var tvp = QueryUtils.CreateKeyListType(rows.Select(x => x.QPrimaryKey));

            //outer query
            SelectQuery outerQuery = new SelectQuery()
                .Select(querySelect, "previous_record")
                .From(info.QSystem, info.TableName, info.Alias)
                .Where(CriteriaSet.And().In(info.Alias, info.PrimaryKeyName, tvp));

            var result = sp.executeReaderOneColumn(outerQuery);
            List<string> res = new List<string>();
            foreach (var r in result)
                res.Add(DBConversion.ToKey(r));
            return res;
        }


        public delegate void FormulaPropagationHandler(string alias, string pk, Area newrow, Area oldrow);

        public void DeterminePropagation(PersistentSupport sp, Area newrow, Area oldrow, FormulaPropagationHandler onPropagate)
        {
            bool inserted = oldrow == null;
            bool deleted = newrow == null;
            if (inserted && deleted)
                throw new InvalidOperationException("At least one of newrow or oldrow must be non-null");
            AreaInfo info = deleted ? oldrow.Information : newrow.Information;

            //The relation may be defined in another set of areas, in case the current area uses a false key
            //in that case the relation "borrows" the metadata of the area where the SR was defined.
            var infoargstart = info.DBFields[DateField];
            var infoarggroup = GroupField == null ? null : info.DBFields[GroupField];

            object GetArgStart(Area row) => row.returnValueField(info.Alias + "." + DateField);
            object GetArgGroup(Area row) => GroupField == null ? null : row.returnValueField(info.Alias + "." + GroupField);
            void PropagateChange(object start, object group)
            {
                if (!infoargstart.isEmptyValue(start))
                {
                    string chaveAnterior = getPreviousRecord(sp, info, start, group);
                    if (!string.IsNullOrEmpty(chaveAnterior))
                        onPropagate(info.Alias, chaveAnterior, newrow, oldrow);
                }
            }

            //TODO: getPreviousRecord currently requires an aditional query.
            // If its done during this algorithm, the we are forcing a row by row calculation.
            // There is a way to calculate all the previous values in bulk, but then it would be the caller
            // that would have the responsibility to supply such oldvalues, with knowledge of the bulk operation.
            // However, having the caller do that might be doing that query prematurely, without knowing
            // what values have effectively changed. Also, we need the previous rows corresponding to both the
            // oldvalues and the newvalues.
            // The query to calculate all the previous_records in bulk is:
            //
            //select codprogr, since, until, codpesso 
            //, (select top 1 codprogr as previous from gqtevcat prev
            //    where prev.zzstate = 0 and prev.since < gqtevcat.since and prev.CODPESSO = gqtevcat.CODPESSO
            //    order by prev.since desc) as previous_record
            //from gqtevcat
            //order by codpesso, since
            //
            //with the outer query changed to list all the records that require updating (or even just 1 in simple case)

            if (inserted)
            {
                PropagateChange(GetArgStart(newrow), GetArgGroup(newrow));
            }
            else if (deleted)
            {
                PropagateChange(GetArgStart(oldrow), GetArgGroup(oldrow));
            }
            else //changed
            {
                var newargstart = GetArgStart(newrow);
                var newarggroup = GetArgGroup(newrow);

                var oldargstart = GetArgStart(oldrow);
                var oldarggroup = GetArgGroup(oldrow);

                //if the record was pseudo-inserted then is like if its value were zero before
                if (oldrow.Zzstate != 0)
                {
                    oldargstart = infoargstart.GetValorEmpty();
                    oldarggroup = (GroupField != null) ? infoarggroup.GetValorEmpty() : null;
                }

                //if the date and group remain the same then there is nothing that needs done
                if (newargstart.Equals(oldargstart) && (GroupField == null || newarggroup.Equals(oldarggroup)))
                    return;

                //update the row immediatly before the newstart, in case there is a start
                PropagateChange(newargstart, newarggroup);

                //update the row immediatly before the old, in case there was a start
                PropagateChange(oldargstart, oldarggroup);
            }
        }

    }
}
