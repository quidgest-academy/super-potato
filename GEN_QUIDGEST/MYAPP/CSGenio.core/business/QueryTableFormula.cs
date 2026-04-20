using System;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    public enum LookupFormulaType  { Next, Previous }

	/// <summary>
	/// Descreve os tipos possíveis de fórmulas internas.
	/// </summary>
	public class QueryTableFormula : Formula
	{
		private string sistemaConsultado;//system da table que vai ser consultada
        private string consultedTable;//table que vai ser consultada
        private string filledDateField;//Qfield data da table que vai ser preenchida
        private string campoDataConsultada;//Qfield data da table que vai ser consultada
        private string campoConsultar;//Qfield que vai ser consultado
        private string campoAgruparConsultada;//Qfield to agrupar na table que vai ser consultada
        private string campoAgruparPreenchida;//Qfield to agrupar na table que vai ser preenchida
        private string campoAgruparConsultada2;//2º Qfield to agrupar na table que vai ser consultada
        private string campoAgruparPreenchida2;//2º Qfield to agrupar na table que vai ser preenchida
        private bool isAgrupar;//se agrupa ou não
        private bool isAgrupar2;//se agrupa com 2 filtros ou não
		private SortOrder tipoOrdenacao;//tipo de ordenação (desc/asc)
        private LookupFormulaType lookupType; //pesquisa ao anterior ou ao seguinte

        /// <summary>
        /// Constructor sem os fields de agrupar
        /// </summary>
		/// <param name="sistemaConsultado">system da table que vai ser consultada</param>
		/// <param name="tabelaConsultada">table que vai ser consultada</param>
        /// <param name="campoDataPreenchida">Qfield data da table que vai ser preenchida</param>
        /// <param name="campoDataConsultada">Qfield data da table consultada</param>
        /// <param name="campoConsultar">Qfield que vai ser consultado</param>
		/// <param name="tipoOrdenacao">tipo de ordenação (desc/asc)</param>
        /// <param name="lookupType">tipo de consulta (anterior/seguinte)</param>
        public QueryTableFormula(string sistemaConsultado, string consultedTable, string filledDateField, string campoDataConsultada,string campoConsultar, SortOrder tipoOrdenacao, LookupFormulaType lookupType)
        {
			this.sistemaConsultado = sistemaConsultado;
            this.consultedTable = consultedTable;
            this.filledDateField = filledDateField;
            this.campoDataConsultada = campoDataConsultada;
            this.campoConsultar = campoConsultar;
            this.isAgrupar = false;
            this.isAgrupar2 = false;
			this.tipoOrdenacao=tipoOrdenacao;
            this.lookupType = lookupType;
        }

        /// <summary>
        /// Constructor com os fields agrupar
        /// </summary>
		/// <param name="sistemaConsultado">system da table que vai ser consultada</param>
		/// <param name="tabelaConsultada">table que vai ser consultada</param>
        /// <param name="campoDataPreenchida">Qfield data da table que vai ser preenchida</param>
        /// <param name="campoDataConsultada">Qfield data da table consultada</param>
        /// <param name="campoConsultar">Qfield que vai ser consultado</param>
		/// <param name="tipoOrdenacao">tipo de ordenação (desc/asc)</param>
        /// <param name="lookupType">tipo de consulta (anterior/seguinte)</param>
        /// <param name="campoAgruparPreenchida">Qfield que vai agrupar na table preenchida</param>
        /// <param name="campoAgruparConsultada">Qfield que vai agrupar na table consultada</param>
		public QueryTableFormula(string sistemaConsultado, string consultedTable, string filledDateField, string campoDataConsultada, string campoConsultar, SortOrder tipoOrdenacao, LookupFormulaType lookupType, string campoAgruparPreenchida, string campoAgruparConsultada)
        {
			this.sistemaConsultado = sistemaConsultado;
			this.consultedTable = consultedTable;
            this.filledDateField = filledDateField;
            this.campoDataConsultada = campoDataConsultada;
            this.campoConsultar = campoConsultar;
            this.campoAgruparPreenchida = campoAgruparPreenchida;
            this.campoAgruparConsultada = campoAgruparConsultada;
            this.isAgrupar = true;
            this.isAgrupar2 = false;
			this.tipoOrdenacao=tipoOrdenacao;
            this.lookupType = lookupType;
        }

        /// <summary>
        /// Constructor com os fields agrupar
        /// </summary>
		/// <param name="sistemaConsultado">system da table que vai ser consultada</param>
		/// <param name="tabelaConsultada">table que vai ser consultada</param>
        /// <param name="campoDataPreenchida">Qfield data da table que vai ser preenchida</param>
        /// <param name="campoDataConsultada">Qfield data da table consultada</param>
        /// <param name="campoConsultar">Qfield que vai ser consultado</param>
		/// <param name="tipoOrdenacao">tipo de ordenação (desc/asc)</param>
        /// <param name="lookupType">tipo de consulta (anterior/seguinte)</param>
        /// <param name="campoAgruparPreenchida">Qfield que vai agrupar na table preenchida</param>
        /// <param name="campoAgruparConsultada">Qfield que vai agrupar na table consultada</param>
        /// <param name="campoAgruparPreenchida2">2º Qfield que vai agrupar na table preenchida</param>
        /// <param name="campoAgruparConsultada2">2º Qfield que vai agrupar na table consultada</param>
		public QueryTableFormula(string sistemaConsultado, string consultedTable, string filledDateField, string campoDataConsultada, string campoConsultar, SortOrder tipoOrdenacao, LookupFormulaType lookupType, string campoAgruparPreenchida, string campoAgruparConsultada, string campoAgruparPreenchida2, string campoAgruparConsultada2)
        {
			this.sistemaConsultado = sistemaConsultado;
			this.consultedTable = consultedTable;
            this.filledDateField = filledDateField;
            this.campoDataConsultada = campoDataConsultada;
            this.campoConsultar = campoConsultar;
            this.campoAgruparPreenchida = campoAgruparPreenchida;
            this.campoAgruparConsultada = campoAgruparConsultada;
            this.campoAgruparPreenchida2 = campoAgruparPreenchida2;
            this.campoAgruparConsultada2 = campoAgruparConsultada2;
            this.isAgrupar = true;
            this.isAgrupar2 = true;
			this.tipoOrdenacao=tipoOrdenacao;
            this.lookupType = lookupType;
        }

        /// <summary>
        /// Função to obter o Qvalue da consulta a table, sem Qfield que agrupa
        /// </summary>
        /// <param name="campoDataPreenchida">Qvalue da data preenchida</param>
        /// <returns>Qvalue consultado</returns>
        public object getCTValue(object filledDateField, FieldFormatting fieldFormatting, PersistentSupport sp)
        {
            try
            {
                //SO 20061211 alteração do constructor QuerySelect
                SelectQuery qs = new SelectQuery()
                    .Select(consultedTable, campoConsultar)
					.From(sistemaConsultado, consultedTable, consultedTable);
                //AV (2010/05/04) a CT só estava a ser feita quando o Qfield de comparação era data ou numérico
                if (fieldFormatting == FieldFormatting.BINARIO || fieldFormatting == FieldFormatting.JPEG)
                {
                    return null;
                }
                else //se for text ou guid não faz sentido ordenar mas sim comparar directamente
                {
                    if (fieldFormatting == FieldFormatting.CARACTERES || fieldFormatting == FieldFormatting.GUID)
                    {
                        qs.Where(CriteriaSet.And()
                            .Equal(consultedTable, campoDataConsultada, (fieldFormatting == FieldFormatting.GUID && String.Equals(filledDateField, "")) ? null : filledDateField));
                    }
                    else //se o Qfield for uma data 
                    {
                        if(lookupType == LookupFormulaType.Next)
                            qs.Where(CriteriaSet.And()
                                .GreaterOrEqual(consultedTable, campoDataConsultada, filledDateField));
                        else
                        qs.Where(CriteriaSet.And()
                            .LesserOrEqual(consultedTable, campoDataConsultada, filledDateField));
                    }
                }

                qs.OrderBy(consultedTable, campoDataConsultada, tipoOrdenacao).PageSize(1);

                 //SO 20060808 comentei o abrir conexão, é aberta na função que invoca esta
                //sp.openConnection();
                //SO 20060814 pode não ser string
                object fieldValue = sp.ExecuteScalar(qs);
                //SO 20060808 comentei o fechar conexão, é fechada na função que invoca esta
                //sp.closeConnection();
                return fieldValue;

            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "QueryTableFormula.getValorConsultaTabela", "Error querying the table: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "QueryTableFormula.getValorConsultaTabela", "Error querying the table: " + ex.Message, ex);
            }

            
        }

        /// <summary>
        /// Função to obter o Qvalue da consulta a table, com fields que agrupam
        /// </summary>
        /// <param name="campoDataPreenchida">Qvalue da data preenchida</param>
        /// <param name="valorCampoAgrupar">Qvalue do Qfield que agrupa</param>
        /// <param name="formCampoAgrupar">formatação do Qfield que agrupa</param>
        /// <returns>Qvalue consultado</returns>
        public object getGroupedCTValue(object filledDateField, FieldFormatting fieldFormatting, object valueGroupField, FieldFormatting formGroupField, PersistentSupport sp)
        {
            try
            {
				object valGroupField = (formGroupField == FieldFormatting.GUID && String.Equals(valueGroupField, "")) ? null : valueGroupField;

                //SO 20061211 alteração do constructor QuerySelect
                SelectQuery qs = new SelectQuery()
                    .Select(consultedTable, campoConsultar)
					.From(sistemaConsultado, consultedTable, consultedTable);

                //AV (2010/05/04) a CT só estava a ser feita quando o Qfield de comparação era data ou numérico
                if (fieldFormatting == FieldFormatting.BINARIO || fieldFormatting == FieldFormatting.JPEG)
                {
                    return null;
                }
                else //se for text ou guid não faz sentido ordenar mas sim comparar directamente
                {
                    if (fieldFormatting == FieldFormatting.CARACTERES || fieldFormatting == FieldFormatting.GUID)
                    {
                        qs.Where(CriteriaSet.And()
                            .Equal(consultedTable, campoAgruparConsultada, valGroupField)
                            .Equal(consultedTable, campoDataConsultada, (fieldFormatting == FieldFormatting.GUID && String.Equals(filledDateField, "")) ? null : filledDateField));
                    }
                    else //se o Qfield for uma data 
                    {
                        if (lookupType == LookupFormulaType.Next)
                            qs.Where(CriteriaSet.And()
                                .Equal(consultedTable, campoAgruparConsultada, valGroupField)
                                .GreaterOrEqual(consultedTable, campoDataConsultada, filledDateField));
                        else
                        qs.Where(CriteriaSet.And()
                            .Equal(consultedTable, campoAgruparConsultada, valGroupField)
                            .LesserOrEqual(consultedTable, campoDataConsultada, filledDateField));
                    }
                }

                qs.OrderBy(consultedTable, campoDataConsultada, tipoOrdenacao).PageSize(1);

                //SO 20060808 comentei o abrir conexão, é aberta na função que invoca esta
                //sp.openConnection();
                //SO 20060814 pode não ser string
                object fieldValue = sp.ExecuteScalar(qs);
                //SO 20060808 comentei o fechar conexão, é fechada na função que invoca esta
                //sp.closeConnection();
                return fieldValue;
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "QueryTableFormula.getGroupedCTValue", "Error querying the table: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "QueryTableFormula.getGroupedCTValue", "Erro na consulta a tabela: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Função to obter o Qvalue da consulta a table, com fields que agrupam
        /// </summary>
        /// <param name="campoDataPreenchida">Qvalue da data preenchida</param>
        /// <param name="valorCampoAgrupar">Qvalue do Qfield que agrupa</param>
        /// <param name="formCampoAgrupar">formatação do Qfield que agrupa</param>
        /// <returns>Qvalue consultado</returns>
        public object getGroupedCTValue(object filledDateField, FieldFormatting fieldFormatting, object valueGroupField, FieldFormatting formGroupField, object valueGroupField2, FieldFormatting formGroupField2, PersistentSupport sp)
        {
            try
            {
				object valGroupField = (formGroupField == FieldFormatting.GUID && String.Equals(valueGroupField, "")) ? null : valueGroupField;
				object valGroupField2 = (formGroupField2 == FieldFormatting.GUID && String.Equals(valueGroupField2, "")) ? null : valueGroupField2;

                //SO 20061211 alteração do constructor QuerySelect
                SelectQuery qs = new SelectQuery()
                    .Select(consultedTable, campoConsultar)
					.From(sistemaConsultado, consultedTable, consultedTable);

                //AV (2010/05/04) a CT só estava a ser feita quando o Qfield de comparação era data ou numérico
                if (fieldFormatting == FieldFormatting.BINARIO || fieldFormatting == FieldFormatting.JPEG)
                {
                    return null;
                }
                else //se for text ou guid não faz sentido ordenar mas sim comparar directamente
                {
                    if (fieldFormatting == FieldFormatting.CARACTERES || fieldFormatting == FieldFormatting.GUID)
                    {
                        qs.Where(CriteriaSet.And()
                            .Equal(consultedTable, campoAgruparConsultada, valGroupField)
                            .Equal(consultedTable, campoAgruparConsultada2, valGroupField2)
                            .Equal(consultedTable, campoDataConsultada, (fieldFormatting == FieldFormatting.GUID && String.Equals(filledDateField, "")) ? null : filledDateField));
                    }
                    else //se o Qfield for uma data 
                    {
                        if(lookupType == LookupFormulaType.Next)
                            qs.Where(CriteriaSet.And()
                                .Equal(consultedTable, campoAgruparConsultada, valGroupField)
                                .Equal(consultedTable, campoAgruparConsultada2, valGroupField2)
                                .GreaterOrEqual(consultedTable, campoDataConsultada, filledDateField));
                        else
                        qs.Where(CriteriaSet.And()
                            .Equal(consultedTable, campoAgruparConsultada, valGroupField)
                            .Equal(consultedTable, campoAgruparConsultada2, valGroupField2)
                            .LesserOrEqual(consultedTable, campoDataConsultada, filledDateField));
                    }
                }

                qs.OrderBy(consultedTable, campoDataConsultada, tipoOrdenacao).PageSize(1);

                //SO 20060808 comentei o abrir conexão, é aberta na função que invoca esta
                //sp.openConnection();
                //SO 20060814 pode não ser string
                object fieldValue = sp.ExecuteScalar(qs);
                //SO 20060808 comentei o fechar conexão, é fechada na função que invoca esta
                //sp.closeConnection();
                return fieldValue;
            }
            catch (GenioException ex)
            {
				throw new BusinessException(ex.UserMessage, "QueryTableFormula.getGroupedCTValue", "Error querying the table: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "QueryTableFormula.getGroupedCTValue", "Error querying the table: " + ex.Message, ex);
            }

        }

        /// <summary>
        /// Table Consultada
        /// </summary>
        public string ConsultedTable
        {
            get { return consultedTable; }
            set { consultedTable = value; }
        }

        /// <summary>
        /// Field de data da table que vai ser preenchida
        /// </summary>
        public string FilledDateFields
        {
            get { return filledDateField; }
            set { filledDateField = value; }
        }

        /// <summary>
        /// Field de data da table que vai ser consultada
        /// </summary>
        public string ConsultedDateFields
        {
            get { return campoDataConsultada; }
            set { campoDataConsultada = value; }
        }

        /// <summary>
        /// Field que vai ser consultado
        /// </summary>
        public string ConsultField
        {
            get { return campoConsultar; }
            set { campoConsultar = value; }
        }

        /// <summary>
        /// Field to agrupar da table que vai ser preenchida
        /// </summary>
        public string FilledGroupField
        {
            get { return campoAgruparPreenchida; }
            set { campoAgruparPreenchida= value; }
        }

        /// <summary>
        /// Field to agrupar da table que vai ser consultada
        /// </summary>
        public string ConsultedGroupField
        {
            get { return campoAgruparConsultada; }
            set { campoAgruparConsultada = value; }
        }

        /// <summary>
        /// 2º Field to agrupar da table que vai ser preenchida
        /// </summary>
        public string Filled2GroupField
        {
            get { return campoAgruparPreenchida2; }
            set { campoAgruparPreenchida2 = value; }
        }

        /// <summary>
        /// 2º Field to agrupar da table que vai ser consultada
        /// </summary>
        public string Consulted2GroupField
        {
            get { return campoAgruparConsultada2; }
            set { campoAgruparConsultada2 = value; }
        }

        /// <summary>
        /// Se é to agrupar
        /// </summary>
        public bool IsGroup
        {
            get { return isAgrupar; }
            set { isAgrupar = value; }
        }

        /// <summary>
        /// Se é to agrupar com 2 filtros
        /// </summary>
        public bool IsGroup2
        {
            get { return isAgrupar2; }
            set { isAgrupar2 = value; }
        }

        /// <summary>
        /// Type de Sort
        /// </summary>
        public SortOrder SortingType
        {
            get { return tipoOrdenacao; }
            set { tipoOrdenacao = value; }
        }
	}
}
