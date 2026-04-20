using System;
using CSGenio.business;
using System.Collections.Generic;
using CSGenio.framework;
using System.Text;
using CSGenio.persistence;
using System.IO;
using System.Collections;
using System.Linq;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Classe que representa um pedido flash da scorecard
    /// </summary>
    public abstract class RequestFlashBSC
		: ExtControl
    {
        /// <summary>
        /// Enumerado com os tipos de comando possíveis
        /// LoadFlash é o comando que corresponde ao pedido com a estrutura do flash
        /// LoadObjects é o comando que corresponde ao pedido com os dados do flash
        /// </summary>
        public enum CommandType { InitChart, LoadChart, LoadLang, GetChartXml, LoadFlash, LoadObjects, CARTAO_CLICK, VALOR_CLICK, RefreshGraficoXml, VISÃO_Click, PERSPECTIVA_Click, OBJECTIVO_Click, INDICADOR_Click, CAIXA_Click, PERSPECTIVA_Novo, OBJECTIVO_Novo, INDICADOR_Novo, CAIXA_Novo, INDICADOR_NovaRela, PERSPECTIVA_Elim, OBJECTIVO_Elim, INDICADOR_Elim };

        /// <summary>
        /// Enumerado com os tipos de resposta possíveis
        /// LoadObjXml é a resposta ao comando LoadFlash
        /// LoadGraficoXml é a resposta ao comando LoadObjects
        /// RefreshGraficoXml é a resposta ao comando RefreshGraficoXml
        /// </summary>
        public enum CommandResponseType { InitChart, LoadChart, LoadLang, RenameElem, GetChartXml, LoadObjXml, LoadGraficoXml, RefreshGraficoXml, CARTAO_CLICK, VALOR_CLICK, PERSPECTIVA_Click, OBJECTIVO_Click, INDICADOR_Click, CAIXA_Click, PERSPECTIVA_Novo, OBJECTIVO_Novo, INDICADOR_Novo, CAIXA_Novo, INDICADOR_NovaRela, PERSPECTIVA_Elim, OBJECTIVO_Elim, INDICADOR_Elim };

        /// <summary>
        /// Parâmetros a enviar to o flash
        /// </summary>
        public static Hashtable parametersFlash;

        /// <summary>
        /// Histórico de IDs
        /// </summary>
        public static string histID;

        /// <summary>
        /// Type de comando do pedido
        /// </summary>
        private CommandType commandType;

        /// <summary>
        /// Parâmetro do pedido
        /// </summary>
        private string parameter;

        /// <summary>
        /// Fields de historial do pedido
        /// </summary>
        private string[] camposHistorial;

        /// <summary>
        /// User em Sessão
        /// </summary>
        public static User user;

        /// <summary>
        /// Variável estática que vai que vai ter todas as scorecards do programa
        /// </summary>
        protected static Dictionary<string, CreatesFlashScorecard> todosFlashes;

        /// <summary>
        /// Função que vai ser invocada to criar uma scorecard do tipo do pedido
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="utilizador">user</param>
        /// <returns>O Pedido flash vai ser inicializado</returns>
        public delegate RequestFlashBSC CreatesFlashScorecard(string[] args, User user);


        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="utilizador">user em sessão</param>
        public RequestFlashBSC(string[] args, User user)
        {
            int nrArgs = args.Length;//nº de argumentos no pedido
            if (nrArgs < 3)//se tiver menos que 3 é erro, 1- comando,  2 - parametro , 3 - name do grafico
				throw new BusinessException("Erro a carregar o Flash do Scorecard.", "RequestFlashBSC", "Insufficient number of arguments.");
            this.commandType = (CommandType)Enum.Parse(typeof(CommandType), args[0]);//tipo de comando
            this.parameter = args[1];//parâmetro
            this.camposHistorial = new string[nrArgs - 3];//fields do historial
            for (int i = 3; i < nrArgs; i++)
            {
                this.camposHistorial[i - 3] = args[i];
            }
            RequestFlashBSC.user = user;
            //this.user = user;//user em sessão
        }

        /// <summary>
        /// Função que executa o pedido
        /// </summary>
        /// <returns>a resposta ao pedido</returns>
        public object processRequest(string function)
        {
			List<string> response = new List<string>();
            try
            {
				//TODO: Each type of control should have its own class representation. This class should be reviewed
                if (function == "GHT_BSCGAUGE")
                {
                    switch (commandType)
                    {
                        case CommandType.LoadChart:
                            response.AddRange(new string[] { CommandResponseType.LoadLang.ToString() });
                            break;
                        case CommandType.LoadLang:
                            response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLGauge() });
                            break;
                        default:
                            throw new BusinessException(null, "PedidoFlashBSC.processRequest", "Function not defined: " + function);
                    }

                    return response.ToArray();
                }
                else if (function == "GHT_BSDASHBOARD")
                {
                    switch (commandType)
                    {
                        case CommandType.LoadChart:
                            response.AddRange(new string[] { CommandResponseType.LoadLang.ToString() });
                            break;
                        case CommandType.LoadLang:
                            response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLDashboard() });
                            break;
                        default:
                            throw new BusinessException(null, "PedidoFlashBSC.processRequest", "Function not defined: " + function);
                    }

                    return response.ToArray();
                }
                else if (function == "GHT_BSCHISTOG")
                {
                    switch (commandType)
                    {
                        case CommandType.LoadChart:
                            response.AddRange(new string[] { CommandResponseType.LoadLang.ToString() });
                            break;
                        case CommandType.LoadLang:
                            response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLHistogram() });
                            break;
                        default:
                            throw new BusinessException("Comando inválido sobre o controlo: " + function, "PedidoFlashBSC.processRequest", "");
                    }

                    return response.ToArray();
                }
                else if (function == "GHT_BSCARD")
                {
                    switch (commandType)
                    {
                        case CommandType.LoadChart:
                            response.AddRange(new string[] { CommandResponseType.LoadLang.ToString() });
                            break;
                        case CommandType.LoadLang:
                            response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLScorecard() });
                            break;
                        default:
                            throw new BusinessException("Comando inválido sobre o controlo: " + function, "PedidoFlashBSC.processRequest", "");
                    }

                    return response.ToArray();
                }
                else if (function == "GHT_BSRELAT")
                {
                    switch (commandType)
                    {
                        case CommandType.LoadChart:
                            response.AddRange(new string[] { CommandResponseType.LoadLang.ToString() });
                            break;
                        case CommandType.LoadLang:
                            response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), ReturnXMLReport() });
                            break;
                        default:
                            throw new BusinessException("Comando inválido sobre o controlo: " + function, "PedidoFlashBSC.processRequest", "");
                    }

                    return response.ToArray();
                }
                else
                {
					switch (commandType)
					{
						//Begin Deprecated
						case CommandType.LoadObjects:
							response.AddRange(new string[] { CommandResponseType.LoadObjXml.ToString(), loadFlash()});
							break;
						case CommandType.LoadFlash:
							{
								if (function == "GSW_BSCARDS")
									response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLScorecard()});
								else if (function == "GSW_BSHISTORICO")
									response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLHistogram()});
								else if (function == "GSW_BSMAPAESTR")
									response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLMap()});
								else if (function == "GSW_BSDASHBOARD")
									response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLDashboard()});
								else if (function == "GSW_BSGAUGE")
									response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLGauge()});
								else if(function == "GSW_BSRELAT")
									response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), ReturnXMLReport() });
								else
									throw new BusinessException(null, "RequestFlashBSC.processRequest", "Function not defined: " + function);
								break;
							}
						//End Deprecated
						case CommandType.InitChart:
							response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), returnXMLMap() });
							break;
						case CommandType.RefreshGraficoXml:
							response.AddRange(new string[] { CommandResponseType.RenameElem.ToString(), refreshXMLScorecard()});
							break;
						case CommandType.CARTAO_CLICK:
							response.AddRange(new string[] { CommandResponseType.CARTAO_CLICK.ToString(), getInternalCode()});
							break;						
						case CommandType.VALOR_CLICK:
							response.AddRange(new string[] { CommandResponseType.VALOR_CLICK.ToString(), getInternalCode()});
							break;						
						case CommandType.PERSPECTIVA_Click:
							response.AddRange(new string[] { CommandResponseType.PERSPECTIVA_Click.ToString(), strategicMapEventHandler()});
							break;						
						case CommandType.OBJECTIVO_Click:
							response.AddRange(new string[] { CommandResponseType.OBJECTIVO_Click.ToString(), strategicMapEventHandler()});
							break;						
						case CommandType.INDICADOR_Click:
							response.AddRange(new string[] { CommandResponseType.INDICADOR_Click.ToString(), strategicMapEventHandler()});
							break;						
						case CommandType.CAIXA_Click:
							response.AddRange(new string[] { CommandResponseType.CAIXA_Click.ToString(), strategicMapEventHandler() });
							break;
						case CommandType.CAIXA_Novo:
							response.AddRange(new string[] { CommandResponseType.CAIXA_Novo.ToString(), strategicMapEventHandler() });
							break;						
						case CommandType.PERSPECTIVA_Novo:
							response.AddRange(new string[] { CommandResponseType.PERSPECTIVA_Novo.ToString(), setHist()});
							break;						
						case CommandType.OBJECTIVO_Novo:
							response.AddRange(new string[] { CommandResponseType.OBJECTIVO_Novo.ToString(), setHist()});
							break;						
						case CommandType.INDICADOR_Novo:
							response.AddRange(new string[] { CommandResponseType.INDICADOR_Novo.ToString(), createNewIndicator().ToString()});
							break;						
						case CommandType.INDICADOR_NovaRela:
							response.AddRange(new string[] { CommandResponseType.INDICADOR_NovaRela.ToString(), setHist()});
							break;						
						case CommandType.PERSPECTIVA_Elim:
							response.AddRange(new string[] { CommandResponseType.PERSPECTIVA_Elim.ToString(), strategicMapEventHandler()});
							break;						
						case CommandType.OBJECTIVO_Elim:
							response.AddRange(new string[] { CommandResponseType.OBJECTIVO_Elim.ToString(), strategicMapEventHandler()});
							break;						
						case CommandType.INDICADOR_Elim:
							response.AddRange(new string[] { CommandResponseType.INDICADOR_Elim.ToString(), strategicMapEventHandler()});
							break;						
						case CommandType.GetChartXml:
							{
								if (parseXML())
									response.AddRange(new string[] { CommandResponseType.GetChartXml.ToString(), "OK" });
								else
									throw new BusinessException(null, "RequestFlashBSC.processRequest", "Erro ao gravar o flash.");
								break;
							}
						default:
							throw new BusinessException(null, "RequestFlashBSC.processRequest", "Command not defined: " + commandType.ToString());
					}
					response.AddRange(this.camposHistorial);
					return response.ToArray();
                }
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "RequestFlashBSC.processRequest")
					throw;
				throw new BusinessException(ex.UserMessage, "RequestFlashBSC.processRequest", "Error processing Flash request: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "RequestFlashBSC.processRequest", "Error processing Flash request: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método que devolve ou coloca o name do comando
        /// </summary>
        public CommandType QCommandType
        {
            get { return commandType; }
        }

        /// <summary>
        /// Método que devolve ou coloca o parametro do pedido
        /// </summary>
        public string Parameter
        {
            get { return parameter; }
        }

        /// <summary>
        /// Método que devolve ou coloca os fields do historial
        /// </summary>
        public string[] HistoryFields
        {
            get { return camposHistorial; }
            set { camposHistorial = value; }
        }

        /// <summary>
        /// Método que devolve ou coloca o user
        /// </summary>
        public User User
        {
            get { return user; }

        }

        /// <summary>
        /// Este método corresponde ao pedido de estrutura do flash
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string loadFlash();

        /// <summary>
        /// Este método corresponde ao pedido de estrutura do flash
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string returnXMLScorecard();

        /// <summary>
        /// Este método corresponde ao pedido de estrutura do flash
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string returnXMLHistogram();

        /// <summary>
        /// Este método corresponde ao pedido de estrutura do flash de mapa estratégico
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string returnXMLMap();
		
		/// <summary>
        /// Este método corresponde ao pedido de XML do gráfico flash Dashboard
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string returnXMLDashboard();
		
		/// <summary>
        /// Este método corresponde ao pedido de XML do gráfico flash Gauge
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string returnXMLGauge();

		/// <summary>
        /// Este método corresponde ao pedido de XML do gráfico flash Relatorio
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string ReturnXMLReport();

        /// <summary>
        /// Este método corresponde ao pedido de estrutura do flash de mapa estratégico
        /// </summary>
        /// <returns>uma string com o xml que corresponde à estrutura do flash </returns>
        public abstract string strategicMapEventHandler();

        /// <summary>
        /// Método usado to responder aos pedidos LoadObject que correspondem aos registos
        /// que vão ser visualizados no flash
        /// </summary>
        /// <param name="refresh">true se é um refresh, false se é um carregamento inicial</param>
        /// <returns>uma string com o xm que corresponde aos registos que devem ser visualizados no flash</returns>
        public abstract string returnXML(bool refresh);

        /// <summary>
        /// Método usado to carregar os códigos internos através dos códigos da Instância
        /// </summary>
        /// <returns>uma string com o código interno que corresponde ao registo que deve ser visualizado no flash</returns>
        public abstract string getInternalCode();

        /// <summary>
        /// Definição de histórico de IDS
        /// </summary>
        /// <returns>uma string com o código interno que corresponde ao registo que deve ser visualizado no flash</returns>
        public abstract string setHist();

        /// <summary>
        /// Método usado to fazer parse e gravar o XML do Mapa Estratégico
        /// </summary>
        /// <returns>se a operação foi bem sucedida retorna verdadeiro</returns>
        public abstract bool parseXML();
		
		/// <summary>
        /// Criação de Novo Indicador
        /// </summary>
        /// <returns>retorna a key primária</returns>
        public abstract string createNewIndicator();

        /// <summary>
        /// Método usado to fazer refresh a elementos do FLASH
        /// </summary>
        /// <returns>se a operação foi bem sucedida retorna verdadeiro</returns>
        public abstract string refreshXMLScorecard();

        public abstract bool cardClick();
    }
}
