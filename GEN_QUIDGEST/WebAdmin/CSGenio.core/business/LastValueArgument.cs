using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections;
using System.Diagnostics;

namespace CSGenio.business
{
    /// <summary>
    /// Argumento de relacao de ultimo Qvalue
    /// </summary>
    public class LastValueArgument
    {
        private string aliasRUV;//table que tem a relação de último Qvalue      
        private string[] camposRUV;//Qfield que são fórmulas relação de último Qvalue
        private string[] camposConsultados;//fields que são argumento da relação de último Qvalue
        private string campoDataConsultada;//Qfield data que limita
        private string campoDataEncerramento;//Qfield data adicional que serve de encerramento
        private bool encerramentoIsToday;//Can be checked to see if the closing value is Today
        private CriteriaSet condition;//condição to filtrar o preenchimento do último Qvalue
        
        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="aliasRUV">table que tem a relação de último Qvalue</param>
        /// <param name="camposRUV">fields que são fórmulas das relações de último Qvalue</param>
        /// <param name="camposConsultados">fields argumentos das relações de último Qvalue</param>
        /// <param name="campoDataConsultada">Qfield data da table consultada</param>
        public LastValueArgument(string aliasRUV, string[] camposRUV, string[] camposConsultados, string campoDataConsultada, CriteriaSet condition, string campoDataEncerramento=null, bool encerramentoIsToday=false)
        {
            this.aliasRUV = aliasRUV;
            this.camposRUV = camposRUV;
            this.camposConsultados = camposConsultados;
            this.campoDataConsultada = campoDataConsultada;
            this.condition = condition;
            this.campoDataEncerramento = campoDataEncerramento;
            this.encerramentoIsToday = encerramentoIsToday;
        }

		public ArrayList ReadLvr(PersistentSupport sp, IArea areaConsulted, string relSourceField, object relationValue, FieldFormatting formattingRelationalField, FieldFormatting formattingSortingField, string TargetRelField = null)
        {
            return ReadLvr(sp, areaConsulted.QSystem, areaConsulted.TableName, relSourceField, relationValue, formattingRelationalField, formattingSortingField, TargetRelField);
        }

        /// <summary>
        /// Faz uma query to ler os ultimos Qvalues actuais
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="tabelaConsultada"></param>
        /// <param name="campoRelOrigem"></param>
        /// <param name="valorRelacao"></param>
        /// <param name="formatacaoCampoRelacao"></param>
        /// <param name="formatacaoCampoConsultado"></param>
        /// <returns>Os Qvalues dos fields a consultar do último Qvalue actual to a relação fornecida. Caso não exista registo o array estará vazio.</returns>
        [Obsolete("Please use 'ArrayList LerRuv(PersistentSupport sp, IArea areaConsultada, string campoRelOrigem, object valorRelacao, FieldFormatting formatacaoCampoRelacao, FieldFormatting formatacaoCampoOrdenacao)' instead")]
        public ArrayList ReadLvr(PersistentSupport sp, string consultedTable, string relSourceField, object relationValue, FieldFormatting formattingRelationalField, FieldFormatting formattingSortingField)
        {
            return ReadLvr(sp, null, consultedTable, relSourceField, relationValue, formattingRelationalField, formattingSortingField);
        }

        private ArrayList ReadLvr(PersistentSupport sp, string schemaConsultado, string consultedTable, string relSourceField, object relationValue, FieldFormatting formattingRelationalField, FieldFormatting formattingSortingField, string TargetRelField = null)
        {
            Debug.Assert(sp != null);
            Debug.Assert(consultedTable != null);
            Debug.Assert(relSourceField != null);
            Debug.Assert(relationValue != null);

            SelectQuery select = new SelectQuery();
            foreach (string field in camposConsultados)
            {
                select.Select(consultedTable, field);
            }
            //SO 20061211 alteração do constructor QuerySelect           

            //Make inner join with relation table
            if(campoDataEncerramento != null && TargetRelField != null && !encerramentoIsToday)
            {
                select.From(schemaConsultado, consultedTable, consultedTable)

                    .Join(schemaConsultado.ToLower() + campoDataEncerramento.Split('.')[0])
                    .On(CriteriaSet.And().Equal(consultedTable, relSourceField, schemaConsultado.ToLower() + campoDataEncerramento.Split('.')[0], TargetRelField))

                    .Where(CriteriaSet.And()
                    .Equal(consultedTable, "zzstate", 0)
                    .Equal(consultedTable, relSourceField, relationValue));
            }
            else
            {
                select.From(schemaConsultado, consultedTable, consultedTable)
                    .Where(CriteriaSet.And()
                    .Equal(consultedTable, "zzstate", 0)
                    .Equal(consultedTable, relSourceField, relationValue));
            }
            

            if (formattingSortingField.Equals(FieldFormatting.DATA) || formattingSortingField.Equals(FieldFormatting.DATAHORA) || formattingSortingField.Equals(FieldFormatting.DATASEGUNDO) )
            {
                if (encerramentoIsToday)
                {
                    select.WhereCondition.LesserOrEqual(consultedTable, campoDataConsultada, DateTime.Now);
                }
                else
                {
                    if(campoDataEncerramento != null && TargetRelField != null)
                    {
                        select.WhereCondition.LesserOrEqual(consultedTable, campoDataConsultada, schemaConsultado.ToLower() + campoDataEncerramento.Split('.')[0], campoDataEncerramento.Split('.')[1]);
                    }
                }
                
                select.OrderBy(consultedTable, campoDataConsultada, SortOrder.Descending);
            }
            else if (formattingSortingField.Equals(FieldFormatting.CARACTERES) || formattingSortingField.Equals(FieldFormatting.TEMPO))
            {
                select.OrderBy(SqlFunctions.Upper(new ColumnReference(consultedTable, campoDataConsultada)), SortOrder.Descending);
            }
            else if (formattingSortingField.Equals(FieldFormatting.FLOAT) ||
                formattingSortingField.Equals(FieldFormatting.LOGICO) ||
                formattingSortingField.Equals(FieldFormatting.INTEIRO))
            {
                select.OrderBy(consultedTable, campoDataConsultada, SortOrder.Descending);
            }
            else
            {
                throw new BusinessException("O campo " + campoDataConsultada + " não pode ser ordenado.", "LastValueArgument.LerRuv", "The field " + campoDataConsultada + " can't be ordered.");
            }
            select.PageSize(1);
            //acrescentar a condição do filtro do último Qvalue
            select.WhereCondition.SubSet(condition);

            return sp.executeReaderOneRow(select);
        }

        /// <summary>
        /// Alias da table que tem a relação de último Qvalue
        /// </summary>
        public string AliasRUV
        {
            get { return aliasRUV; }
        }

        /// <summary>
        /// Field que é fórmula de último Qvalue
        /// </summary>
        public string[] LVRFields
        {
            get { return camposRUV; }
        }

        /// <summary>
        /// Field argumento da relação de último Qvalue
        /// </summary>
        public string[] ConsultedFields
        {
            get { return camposConsultados; }
        }

        /// <summary>
        /// Field data da table consultada
        /// </summary>
        public string ConsultedDateFields
        {
            get { return campoDataConsultada; }
        }

        /// <summary>
        /// Field adicional, não obrigatório que serve de encerramento
        /// </summary>
        public string EndDateField
        {
            get { return campoDataEncerramento; }
        }

        public delegate void FormulaPropagationHandler(string alias, string pk, Area newrow, Area oldrow);

        public void DeterminePropagation(Area newrow, Area oldrow, FormulaPropagationHandler onPropagate)
        {
            bool inserted = oldrow == null;
            bool deleted = newrow == null;
            if (inserted && deleted)
                throw new InvalidOperationException("At least one of newrow or oldrow must be non-null");
            AreaInfo info = deleted ? oldrow.Information : newrow.Information;

            var rel = info.ParentTables[AliasRUV];

            var infoargdate = info.DBFields[ConsultedDateFields];
            var infoargkey = info.DBFields[rel.SourceRelField];

            object GetArgDate(Area row) => row.returnValueField(info.Alias + "." + ConsultedDateFields);
            object GetArgKey(Area row) => row.returnValueField(info.Alias + "." + rel.SourceRelField);
            void PropagateChange(object key, object date)
            {
                if (!infoargkey.isEmptyValue(key) && !infoargdate.isEmptyValue(date))
                    onPropagate(AliasRUV, key as string, newrow, oldrow);
            }

            if (inserted)
            {
                //if the new date is empty then this can never be the last value
                PropagateChange(GetArgKey(newrow), GetArgDate(newrow));
            }
            else if (deleted)
            {
                //if the old date was empty then this was already not the last value
                PropagateChange(GetArgKey(oldrow), GetArgDate(oldrow));
            }
            else //updated
            {
                var newargdate = GetArgDate(newrow);
                var newargkey = GetArgKey(newrow);

                var oldargdate = GetArgDate(oldrow);
                var oldargkey = GetArgKey(oldrow);

                //pseudo-new rows behave as if they have been just inserted
                if (oldrow.Zzstate != 0)
                {
                    oldargdate = infoargdate.GetValorEmpty();
                    oldargkey = infoargkey.GetValorEmpty();
                }

                if (!oldargkey.Equals(newargkey))
                {
                    //fk change, we need to update both records as if old was deleted and new was inserted
                    PropagateChange(oldargkey, oldargdate);
                    PropagateChange(newargkey, newargdate);
                }
                else
                {
                    //key stayed empty, nothing to do
                    if (infoargkey.isEmptyValue(newargkey))
                        return;

                    //date or fields changed then propagate
                    bool changed = false;
                    if (!newargdate.Equals(oldargdate))
                        changed = true;

                    //date stayed empty, nothing to update regardless of field changes
                    if (!changed && infoargdate.isEmptyValue(newargdate))
                        return;

                    //check for value field changes
                    foreach (var uvfield in ConsultedFields)
                    {
                        var newargvalue = newrow.returnValueField(info.Alias + "." + uvfield);
                        var oldargvalue = oldrow.returnValueField(info.Alias + "." + uvfield);

                        if (!newargvalue.Equals(oldargvalue))
                        {
                            changed = true;
                            break;
                        }
                    }

                    //check for condition source changes
                    if(condition != null)
                        foreach (var criteria in condition.Criterias)
                            if (criteria.LeftTerm is ColumnReference left)
                            {
                                var newargvalue = newrow.returnValueField(info.Alias + "." + left.ColumnName);
                                var oldargvalue = oldrow.returnValueField(info.Alias + "." + left.ColumnName);

                                if (!newargvalue.Equals(oldargvalue))
                                {
                                    changed = true;
                                    break;
                                }
                            }

                    //changes need to update even if the date is empty, so the last value can be cleared
                    if(changed)
                        onPropagate(AliasRUV, newargkey as string, newrow, oldrow);
                }
            }
        }

    }
}
