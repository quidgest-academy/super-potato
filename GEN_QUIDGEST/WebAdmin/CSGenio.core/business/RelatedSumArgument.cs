using System;

namespace CSGenio.business
{
    /// <summary>
    /// Esta classe representa a propriedade do Qfield ser argumento de soma relacionada
    /// </summary>
    public class RelatedSumArgument : Formula
    {
        private readonly decimal _number; //se é number, o numero parsed

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="aliasSR">alias da table do Qfield que é soma relacionada</param>
        /// <param name="campoSR">Qfield que é soma relacionada</param>
        /// <param name="sinal">sign do Qfield</param>
        /// <param name="isCampo">se é Qfield ou inteiro</param>
        public RelatedSumArgument(string aliasOrigem, string aliasSR, string campoSR, string campoArg, char sign, bool isCampo)
        {
            AliasSource = aliasOrigem;
            AliasSR = aliasSR;
            SRField = campoSR;
            ArgField = campoArg;
            Signal = sign;
            IsField = isCampo;
            _number = IsField ? 0m : Convert.ToDecimal(ArgField);
        }

        /// <summary>
        /// alias da table do campo que é somado
        /// </summary>
        public string AliasSource { get; set; }
        /// <summary>
        /// alias da tabela do campo que é soma relacionada
        /// </summary>
        public string AliasSR { get; set; }
        /// <summary>
        /// campo que é o destino do somatório
        /// </summary>
        public string SRField { get; set; }
        /// <summary>
        /// campo que é a fonte da soma
        /// </summary>
        public string ArgField { get; set; }
        /// <summary>
        /// sinal do campo. positivo '+' ou negativo '-'
        /// </summary>
        public char Signal { get; set; }
        /// <summary>
        /// se é uma referencia a um campo, caso contrário é um literal numérico (contagem)
        /// </summary>
        public bool IsField { get; set; }

        public delegate void FormulaPropagationHandler(string alias, string pk, Area newrow, Area oldrow, decimal diff);

        public void DeterminePropagation(Area newrow, Area oldrow, FormulaPropagationHandler onPropagate)
        {
            bool inserted = oldrow == null;
            bool deleted = newrow == null;
            if (inserted && deleted)
                throw new InvalidOperationException("At least one of newrow or oldrow must be non-null");
            AreaInfo info = deleted ? oldrow.Information : newrow.Information;

            //The relation may be defined in another set of areas, in case the current area uses a false key
            //in that case the relation "borrows" the metadata of the area where the SR was defined.
            var rel = Area.GetInfoArea(AliasSource).ParentTables[AliasSR];

            var infoargkey = info.DBFields[rel.SourceRelField];
            string valueFieldname = info.Alias + "." + ArgField;
            string relFieldname = info.Alias + "." + rel.SourceRelField;

            decimal GetArgValue(Area row) => IsField ? Convert.ToDecimal(row.returnValueField(valueFieldname)) : _number;
            object GetArgKey(Area row) => row.returnValueField(relFieldname);
            void PropagateChange(object key, decimal value)
            {
                if (!infoargkey.isEmptyValue(key) && value != 0)
                    onPropagate(AliasSR, key as string, newrow, oldrow, value);
            }

            if (inserted)
            {
                PropagateChange(GetArgKey(newrow), GetArgValue(newrow));
            }
            else if (deleted)
            {
                PropagateChange(GetArgKey(oldrow), -GetArgValue(oldrow));
            }
            else //changed
            {
                decimal newArgValue = GetArgValue(newrow);
                object newArgKey = GetArgKey(newrow);

                decimal oldArgValue = GetArgValue(oldrow);
                object oldArgKey = GetArgKey(oldrow);

                //pseudo-new rows behave as if they have been just inserted
                if (oldrow.Zzstate != 0)
                {
                    oldArgValue = 0m;
                    oldArgKey = infoargkey.GetValorEmpty();
                }

                if (!oldArgKey.Equals(newArgKey))
                {
                    //fk change, so its as if we deleted the oldkey and inserted the newkey
                    PropagateChange(oldArgKey, -oldArgValue);
                    PropagateChange(newArgKey, newArgValue);
                }
                else if (!newArgValue.Equals(oldArgValue))
                {
                    //we only changed the value within the same fk, so update the diff
                    PropagateChange(newArgKey, newArgValue - oldArgValue);
                }
            }
        }
    }

}
