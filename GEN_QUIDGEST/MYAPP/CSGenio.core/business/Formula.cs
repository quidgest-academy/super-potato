using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Classe mãe que caracteriza uma formula
    /// </summary>
    public abstract class Formula
    {
        public object[] returnValueFieldsInternalFormula(Area areaField, List<ByAreaArguments> argumentsListByArea, PersistentSupport sp, FormulaDbContext fdc, int nrArguments, FunctionType tpFunction)
        {
            try
            {
                object[] fieldsValue = new object[nrArguments];
                Area area = null;
                foreach (ByAreaArguments argumentosPorArea in argumentsListByArea)
                {
                    if (argumentosPorArea.AliasName.Equals(areaField.Alias)) //se é a propria area
                    {
                        //descobrir que fields não estão em memória
                        var selectBD = argumentosPorArea.FieldNames
                            .Where(x => !areaField.Fields.ContainsKey(areaField.Alias + "." + x));

                        //ler base de dados
                        if (selectBD.Any() && (tpFunction == FunctionType.INS || tpFunction == FunctionType.DUP))
						{
							//ir buscar a key primária
							string codIntValue = areaField.QPrimaryKey;
							if (codIntValue == "")
								throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "ChavePrimaria is null.");
						
                            sp.getRecord(areaField, codIntValue, selectBD.ToArray()); //<----- Em que situações pode o codigo chegar aqui?
						}

                        //agora já podemos assumir que os fields estão em memoria
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            fieldsValue[argumentosPorArea.FieldsPosition[i]] = areaField.returnValueField(areaField.Alias + "." + argumentosPorArea.FieldNames[i]);

                    }
                    else //é outra área
                    {
						//Aqui já só tenho de ler o que foi preenchido pelo FormulaDbContext, se estiver vazia usamos os Qvalues nulos to as sources das formulas
                        var fk = fdc.GetForeignKeyValue(argumentosPorArea.AliasName, argumentosPorArea.KeyName, sp);
						area = fdc.ReadRecord(argumentosPorArea.AliasName, fk, sp);
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            fieldsValue[argumentosPorArea.FieldsPosition[i]] = area.returnValueField(area.Alias + "." + argumentosPorArea.FieldNames[i]);
                    }
                }

                return fieldsValue;
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "Formula.devolverValorCamposFormulaInterna")
					throw;
				throw new BusinessException(ex.UserMessage, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to obter os Qvalues necessários nas operações internas
        /// </summary>
        /// <param name="areaCampo">area do Qfield</param>
        /// <param name="argumentosPorArea">argumentosPorArea</param>
        /// <param name="sp">Suporte Persistente</param>
        /// <returns></returns>
        //[Obsolete("Please use the overload that uses FormulaDbContext for a more efficient calculation")]
        public object[] returnValueFieldsInternalFormula(Area areaField, List<ByAreaArguments> argumentsListByArea, PersistentSupport sp, int nrArguments, FunctionType tpFunction)
        {
            try
            {
                object[] fieldsValue = new object[nrArguments];
                Area area = null;
                FieldFormatting formField = FieldFormatting.CARACTERES;
                foreach (ByAreaArguments argumentosPorArea in argumentsListByArea)
                {
                    if (!argumentosPorArea.AliasName.Equals(areaField.Alias))
                        area = Area.createArea(argumentosPorArea.AliasName, areaField.User, areaField.Module);
                    else
                        area = null;

                    if (area == null)//se é a propria area
                    {

                        //descobrir que fields não estão em memória
                        var selectBD = argumentosPorArea.FieldNames
                            .Where(x => !areaField.Fields.ContainsKey(areaField.Alias + "." + x));

                        //ler da base de dados
                        if (selectBD.Any() && (tpFunction == FunctionType.INS || tpFunction == FunctionType.DUP))
						{
							//ir buscar a key primária
							string codIntValue = areaField.QPrimaryKey;
							if (codIntValue == "")
								throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "ChavePrimaria is null.");
						
                            sp.getRecord(areaField, codIntValue, selectBD.ToArray());
						}

                        //agora já podemos assumir que os fields estão em memoria
                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            fieldsValue[argumentosPorArea.FieldsPosition[i]] = areaField.returnValueField(areaField.Alias + "." + argumentosPorArea.FieldNames[i]);

                    }
                    else if ((area.Alias.ToUpper()).Equals("GLOB"))//se for a table glob
                    {
                        //query to ir buscar os Qvalues dos fields
                        SelectQuery qs = new SelectQuery();
                        foreach (string field in argumentosPorArea.FieldNames)
                        {
                            qs.Select(area.TableName, field);
                        }
                        qs.From(area.QSystem, area.TableName, area.TableName)
                            //.Where(CriteriaSet.And()
                                //.Equal(area.TableName, area.PrimaryKeyName, codIntValue))
                                .PageSize(1);

                        ArrayList fieldsvalues = sp.executeReaderOneRow(qs);
                        if(fieldsValue.Length == 0)
                            throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "No record found in glob.");

                        for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                        {
                            formField = ((Field)area.DBFields[argumentosPorArea.FieldNames[i]]).FieldFormat;
                            //TSX (2008-10-30) fieldsValue[argumentosPorArea.FieldsPosition[i]] = Conversion.internal2InternalValid(fieldsvalues[i], formField);
                            //prever o caso em que a query nao encontrou nenhum registo
                            if (fieldsvalues.Count == 0)
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = Field.GetValorEmpty(formField);
                            else
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = DBConversion.ToInternal(fieldsvalues[i], formField);
                        }

                    }
                    else if (!area.Alias.Equals(areaField.Alias))
                    {
                        //ir buscar a key primária da table a que os fields pertencem
                        //verificar se o areaField contem a key estrangeira que corresponde à key
                        //primária do Qfield que estamos a procurar se não contem entao tem de se ir ler à base de dados
                        string valorChaveEst;
                        //AV(2010/06/04) Os fields que eram apagados nos forms estavam a ser sobrepostos com o Qvalue na db por isso temos que testar se o Qfield está em memória
                        if (!areaField.Fields.ContainsKey(areaField.Alias + "." + argumentosPorArea.KeyName))//se o Qvalue não está em memória ir ler à BD
                        {
                            string codIntValue = areaField.QPrimaryKey;
                            valorChaveEst = DBConversion.ToKey(sp.returnField(areaField, argumentosPorArea.KeyName, codIntValue));
                            areaField.insertNameValueField(areaField.Alias + "." + argumentosPorArea.KeyName, valorChaveEst);
                        }
                        else
                            valorChaveEst = (string)areaField.returnValueField(areaField.Alias + "." + argumentosPorArea.KeyName);
                        if (valorChaveEst != "")//se a key estrangeira está em memória ou na BD (já está em memória)
                        {
                            //query to ir buscar os Qvalues dos fields
                            sp.getRecord(area, valorChaveEst, argumentosPorArea.FieldNames);
                            //quando o registo não consegue ser posicionado os Qvalues estão a nulo
                            for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = area.returnValueField(area.Alias + "." + argumentosPorArea.FieldNames[i]);
                             
                        }
                        else
                        {
                            for (int i = 0; i < argumentosPorArea.FieldNames.Length; i++)
                            {
                                formField = ((Field)area.DBFields[argumentosPorArea.FieldNames[i]]).FieldFormat;
                                fieldsValue[argumentosPorArea.FieldsPosition[i]] = Field.GetValorEmpty(formField);
                            }
                        }
                    }
                }

                return fieldsValue;

            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "Formula.devolverValorCamposFormulaInterna")
					throw;
				throw new BusinessException(ex.UserMessage, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
				throw new BusinessException(null, "Formula.devolverValorCamposFormulaInterna", "Error getting field values in internal formula: " + ex.Message, ex);
            }
        }
    }

    /// <summary>
    /// Classe que representa um argumento duma formula de operação interna ou formula condição
    /// </summary>
    public class ByAreaArguments
    {
        private string nomeAlias;//name da área a que pertencem os fields que são argumentos da função
        private string nomeChave; //name da key primária to a table dos fields que são argumentos 
        private int[] posicaoCampos; //posição dos fields que são argumentos da função 
        private string[] fieldNames; //name dos fields que são argumentos da função 

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="nomeCampos">Name dos fields que são argumentos</param>
        /// <param name="f">função que corresponde à fórmula</param>
        public ByAreaArguments(string[] fieldNames, int[] posicaoCampos, string nomeAlias, string nomeChave)
        {
            this.nomeAlias = nomeAlias;
            this.posicaoCampos = posicaoCampos;
            this.fieldNames = fieldNames;
            this.nomeChave = nomeChave;
        }


        /// <summary>
        /// Name dos fields que servem de argumentos
        /// </summary>
        public string[] FieldNames
        {
            get { return fieldNames; }
            set { fieldNames = value; }
        }

        /// <summary>
        /// Posicao dos fields que servem de argumentos
        /// </summary>
        public int[] FieldsPosition
        {
            get { return posicaoCampos; }
            set { posicaoCampos = value; }
        }

        /// <summary>
        /// Name do alias da table dos fields que servem de argumentos
        /// </summary>
        public string AliasName
        {
            get { return nomeAlias; }
            set { nomeAlias = value; }
        }

        /// <summary>
        /// Name da key da table dos fields que são argumentos 
        /// </summary>
        public string KeyName
        {
            get { return nomeChave; }
            set { nomeChave = value; }
        }
    }
}
