using System;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
	/// <summary>
	/// Descreve os tipos possíveis de fórmulas internas.
	/// </summary>
	public class ReplicaFormula : Formula
	{
        //20060608
        private string alias;//alias da table de onde vamos retirar o Qfield
        private string Qfield;//Qfield da table que vamos copiar
       

        public ReplicaFormula(string alias, string Qfield)
        {
            this.alias = alias;
            this.Qfield = Qfield;            
        }

        /// <summary>
        /// função que devolve o Qvalue da replica
        /// </summary>
        /// <param name="valorRelacao">Qvalue do Qfield que faz a relação</param>
        /// <returns>Qvalue da replica</returns>
        public object getReplicaValue(string system, string table, string relationalField, string relationValue, PersistentSupport sp) //DQ 07/06/2006 : Os tipos dos fields devem estar como object.
        {
            try
            {
                //SO 20061211 alteração do constructor QuerySelect
                SelectQuery query = new SelectQuery()
                    .Select(alias, Qfield)
                    .From(system, table, alias)
                    .Where(CriteriaSet.And()
                        .Equal(alias, relationalField, relationValue));

                object Qresult = sp.ExecuteScalar(query);
                return Qresult;
            }
            catch (GenioException ex)
            {
                throw new BusinessException(ex.UserMessage, "Formulareplica.getReplicaValue", "Error getting replica value: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new BusinessException(null, "Formulareplica.getReplicaValue", "Error getting replica value: " + ex.Message, ex);
            }
        }

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        public string Field
        {
            get { return Qfield; }
            set { Qfield = value; }
        }

	}
}
