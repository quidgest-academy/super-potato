using System;

namespace CSGenio.business
{
    /// <summary>
    /// Esta classe representa um campo a ser agregado pela formula de concatena linhas
    /// </summary>
    public class ListAggregateArgument : Formula
    {
        /// <summary>
        /// Constructor da classe
        /// </summary>
        public ListAggregateArgument(string aliasOrigem, string aliasLG, string campoLG, string campoArg, string sortingField, string campoSeparador)
        {
            AliasSource = aliasOrigem;
            AliasLG = aliasLG;
            LGField = campoLG;
            ArgField = campoArg;
            SortField = sortingField;
            SeparatorField = campoSeparador;
        }

        /// <summary>
        /// Alias da tabela que vai ser agregada
        /// </summary>
        public string AliasSource { get; set; }
        /// <summary>
        /// Alias da tabela de destino da agregacao
        /// </summary>
        public string AliasLG { get; set; }
        /// <summary>
        /// Campo de destino da agregacao
        /// </summary>
        public string LGField { get; set; }
        /// <summary>
        /// Campo que vai ser agregado
        /// </summary>
        public string ArgField { get; set; }
        /// <summary>
        /// Campo que vai definir por ordem a agregacao vai ser feita
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// Caracteres separadores de cada item agregado
        /// </summary>
        public string SeparatorField { get; set; }

        public delegate void FormulaPropagationHandler(string alias, string pk, Area newrow, Area oldrow);

        public void DeterminePropagation(Area newrow, Area oldrow, FormulaPropagationHandler onPropagate)
        {
            bool inserted = oldrow == null;
            bool deleted = newrow == null;
            if (inserted && deleted)
                throw new InvalidOperationException("At least one of newrow or oldrow must be non-null");
            AreaInfo info = deleted ? oldrow.Information : newrow.Information;

            //The relation may be defined in another set of areas, in case the current area uses a false key
            //in that case the relation "borrows" the metadata of the area where the SR was defined.
            var rel = Area.GetInfoArea(AliasSource).ParentTables[AliasLG];

            var infoargvalue = info.DBFields[ArgField];
            var infoargsort = info.DBFields[SortField];
            var infoargkey = info.DBFields[rel.SourceRelField];

            object GetArgValue(Area row) => row.returnValueField(info.Alias + "." + ArgField);
            object GetArgSort(Area row) => row.returnValueField(info.Alias + "." + SortField);
            object GetArgKey(Area row) => row.returnValueField(info.Alias + "." + rel.SourceRelField);
            void PropagateChange(object key, object value)
            {
                if (!infoargkey.isEmptyValue(key) && !infoargvalue.isEmptyValue(value))
                    onPropagate(AliasLG, key as string, newrow, oldrow);
            }

            if (inserted)
            {
                PropagateChange(GetArgKey(newrow), GetArgValue(newrow));
            }
            else if (deleted)
            {
                PropagateChange(GetArgKey(oldrow), GetArgValue(oldrow));
            }
            else //changed
            {
                var newargvalue = GetArgValue(newrow);
                var newargsort = GetArgSort(newrow);
                var newargkey = GetArgKey(newrow);

                var oldargvalue = GetArgValue(oldrow);
                var oldargsort = GetArgSort(oldrow);
                var oldargkey = GetArgKey(oldrow);

                //pseudo-new rows behave as if they have been just inserted
                if (oldrow.Zzstate != 0)
                {
                    oldargvalue = infoargvalue.GetValorEmpty();
                    oldargsort = infoargsort.GetValorEmpty();
                    oldargkey = infoargkey.GetValorEmpty();
                }

                if (!oldargkey.Equals(newargkey))
                {
                    //fk change, so its as if we deleted the oldkey and inserted the newkey
                    PropagateChange(oldargkey, oldargvalue);
                    PropagateChange(newargkey, newargvalue);
                }
                else
                {
                    //key stayed empty, nothing to do
                    if (infoargkey.isEmptyValue(newargkey))
                        return;

                    //we only changed the value within the same fk, so check for changes in the sources
                    if (!newargsort.Equals(oldargsort) || !newargvalue.Equals(oldargvalue))
                        onPropagate(AliasLG, newargkey as string, newrow, oldrow);
                }
            }
        }

    }
}
