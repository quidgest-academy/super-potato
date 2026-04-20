using System;
using CSGenio.business;
using System.Collections.Generic;
using CSGenio.framework;
using System.Text;
using CSGenio.persistence;
//using System.Web.UI;
using System.IO;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Classe que representa um pedido flash do workflow
    /// </summary>
    public abstract class RequestFlashWorkflow
		: ExtControl
    {
        /// <summary>
        /// Enumerado com os tipos de eventos possíveis
        /// LoadFlash é o comando que corresponde ao pedido com a estrutura do flash
        /// </summary>
        public enum CommandType 
		{
			LoadFlash, 
            GetGraficoXml,
			RenameElem,
            EVENT_Click,
            ACTIV_Click,
            GATEW_Click,
            EVENT_Elim,
            ACTIV_Elim,
            GATEW_Elim,
            GROUP_Elim,
            ANNOT_Elim,
            CONN_Elim,
            EVENT_New,
            ACTIV_New,
            GATEW_New,
            GROUP_New,
            ANNOT_New,
            CONN_New,
            IntermSave
        };

        /// <summary>
        /// Enumerado com os tipos de resposta possíveis
        /// LoadGraficoXml é a resposta ao comando LoadFlash
        /// GetGraficoXml é a resposta ao comando GetGraficoXml
        /// </summary>
        public enum CommandResponseType 
		{
			LoadGraficoXml, 
			LoadGrafico, 
			LoadLangXml, 
			LoadLangFile, 
			GetGraficoXml, 
			RenameElem,
			EVENT_Click,
            ACTIV_Click,
            GATEW_Click,
            EVENT_Elim,
            ACTIV_Elim,
            GATEW_Elim,
            GROUP_Elim,
            ANNOT_Elim,
            CONN_Elim,
            EVENT_New,
            ACTIV_New,
            GATEW_New,
            GROUP_New,
            ANNOT_New,
            CONN_New,
		};

        /// <summary>
        /// Parâmetros a enviar to o flash
        /// </summary>
        public static string parametrosFlash;

        /// <summary>
        /// Histórico de IDs
        /// </summary>
        public static string histID;

        /// <summary>
        /// Identificador
        /// </summary>
        public string _id;

        /// <summary>
        /// Type de comando do pedido
        /// </summary>
        private CommandType tipoComando;

        /// <summary>
        /// Parâmetro do pedido
        /// </summary>
        private string parametro;

        /// <summary>
        /// Fields de historial do pedido
        /// </summary>
        private string[] camposHistorial;

        /// <summary>
        /// Fields de argumento do pedido
        /// </summary>
        public string[] _args;

        /// <summary>
        /// User em Sessão
        /// </summary>
        public static User user;

        /// <summary>
        /// Função que vai ser invocada to criar um workflow do tipo do pedido
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="utilizador">user</param>
        /// <returns>O Pedido flash vai ser inicializado</returns>
        public delegate RequestFlashWorkflow CreatesFlashWorkflow(string[] args, User user);

		/// <summary>
		/// estrutura de dados que representa as actividades num workflow
		/// </summary>
        public IList<RequestFlashWFActivity> Atividades = new List<RequestFlashWFActivity>();
		
		/// <summary>
		/// estrutura de dados que representa os eventos num workflow
		/// </summary>
        public IList<RequestFlashWFEvent> Eventos = new List<RequestFlashWFEvent>();
		
		/// <summary>
		/// estrutura de dados que representa os gateways num workflow
		/// </summary>
        public IList<RequestFlashWFGateway> Gateways = new List<RequestFlashWFGateway>();
		
		/// <summary>
		/// estrutura de dados que representa as relações num workflow
		/// </summary>
        public IList<RequestFlashWFRela> Relacoes = new List<RequestFlashWFRela>();

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="utilizador">user em sessão</param>
        public RequestFlashWorkflow(string[] args, User user)
        {
            // nº de argumentos no pedido
			int nrArgs = args.Length;
            
			// se tiver menos que 3 é erro, 1- comando,  2 - parametro , 3 - name do grafico
			if (nrArgs < 3)
				throw new BusinessException("Erro a carregar o Flash do Workflow.", "RequestFlashWorkflow.RequestFlashWorkflow", "Insufficient number of arguments.");
            
			// tipo de comando
			this.tipoComando = (CommandType)Enum.Parse(typeof(CommandType), args[0]);
            
			// parâmetro
			this.parametro = args[1];
			
			// fields do historial
            this.camposHistorial = new string[nrArgs - 3];
            for (int i = 3; i < nrArgs; i++)
            {
                this.camposHistorial[i - 3] = args[i];
            }
            RequestFlashWorkflow.user = user;
        }

        /// <summary>
        /// Função que executa o pedido
        /// </summary>
        /// <returns>a resposta ao pedido</returns>
		public override object processRequest()
        {
			// Last updated by [CJP] at [2014.11.21]
			List<string> response = new List<string>();
            try
            {
                switch (tipoComando)
                {
                    // case CommandType.LoadGrafico:
						// Fazer o load do file xml de línguas
						// Tratado automaticamente no javacript 
                        // response.AddRange(new string[] { CommandResponseType.LoadLangFile.ToString(), "../../../../../Content/flashes/WflowLANG.xml;"+this.User.Language});
                        // break;

                    case CommandType.LoadFlash:
                        response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), loadFlash(), LoadStaticOptions() });
						break;						
                    case CommandType.GetGraficoXml:
                        if (parseXML())
                            response.AddRange(new string[] { CommandResponseType.GetGraficoXml.ToString(), WriteToBD().ToString()  });
                        else
                            throw new BusinessException(null, "RequestFlashWorkflow",Translations.Get("Ocorreu um erro ao salvar o workflow.", user.Language));
						break;                     
                    case CommandType.IntermSave:
                        if (parseXML())
                            response.AddRange(new string[] { CommandResponseType.GetGraficoXml.ToString(), WriteToBD(false).ToString() });
                        else
                            throw new BusinessException(null, "RequestFlashWorkflow", Translations.Get("Ocorreu um erro ao salvar o workflow.", user.Language));
                        break;
					 case CommandType.RenameElem:
                        response.AddRange(new string[] { CommandResponseType.RenameElem.ToString(), "OK" });
                        break;					
					 case CommandType.EVENT_New:
						response.AddRange(new string[] { CommandResponseType.RenameElem.ToString(), EventHandlerEventNew() });
                        break;						
					case CommandType.ACTIV_New:
						response.AddRange(new string[] { CommandResponseType.RenameElem.ToString(), EventHandlerActivNew() });
                        break;						
					case CommandType.GATEW_New:
						response.AddRange(new string[] { CommandResponseType.RenameElem.ToString(), EventHandlerGatewNew() });
                        break;					 
					 case CommandType.CONN_New:
                        response.AddRange(new string[] { CommandResponseType.CONN_New.ToString() , EventHandlerRelaNew() == true? "OK" : "NOK" });
                        break;					
					case CommandType.EVENT_Click:
						response.AddRange(new string[] {CommandResponseType.EVENT_Click.ToString(), getInternalCode() });
                        break;					
					case CommandType.ACTIV_Click:
                        response.AddRange(new string[] {CommandResponseType.ACTIV_Click.ToString(), getInternalCode() });
                        break;						
					case CommandType.GATEW_Click:
                        response.AddRange(new string[] { CommandResponseType.GATEW_Click.ToString(), getInternalCode() });
                        break;						
					case CommandType.ACTIV_Elim:
                        response.AddRange(new string[] { CommandResponseType.ACTIV_Elim.ToString(), EventHandlerActivDelete() == true? "OK" : "NOK" });
                        break;
                    case CommandType.EVENT_Elim:
                        response.AddRange(new string[] { CommandResponseType.EVENT_Elim.ToString(), EventHandlerEventDelete() == true? "OK" : "NOK" });
                        break;						
					case CommandType.GATEW_Elim:
                        response.AddRange(new string[] { CommandResponseType.EVENT_Elim.ToString(), EventHandlerGatewDelete() == true? "OK" : "NOK"});
                        break;						
					case CommandType.CONN_Elim:
                        response.AddRange(new string[] { CommandResponseType.CONN_Elim.ToString(), EventHandlerRelaDelete() == true? "OK" : "NOK" });
                        break;						
                    default:
                        throw new BusinessException(null, "RequestFlashWorkflow.processRequest", "Command not defined: " + tipoComando.ToString());
                }
				response.AddRange(this.camposHistorial);
                return response.ToArray();
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "RequestFlashWorkflow.processRequest")
					throw;
				throw new BusinessException(ex.UserMessage, "RequestFlashWorkflow.processRequest", "Error processing Flash request: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "RequestFlashWorkflow.processRequest", "Error processing Flash request: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método que devolve ou coloca o name do comando
        /// </summary>
        public CommandType QCommandType
        {
            get { return tipoComando; }
        }

        /// <summary>
        /// Método que devolve ou coloca o parametro do pedido
        /// </summary>
        public string Parameter
        {
            get { return parametro; }
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
        /// Método usado to devolver o xml correspondente aos elementos do workflow
        /// </summary>
        /// <returns>uma string com o xml que corresponde aos registos que devem ser visualizados no flash</returns>
        public abstract string toXML();

        /// <summary>
        /// Método que recebe o XML e preenche a informação lançada
        /// </summary>
        /// <returns>uma string com a resposta</returns>
        public abstract bool parseXML();
        
		/// <summary>
        /// Método usado to carregar os códigos internos através dos códigos da Instância
		/// Usado to poder abrir o form respetivo
        /// </summary>
        /// <returns>uma string com o código interno que corresponde ao registo que deve ser visualizado no flash</returns>
		public abstract string getInternalCode();

        /// <summary>
        /// Método que trata o evento EVENT_New (Criação de um evento)
        /// </summary>
        /// <returns>string to renomear o evento</returns>
        public abstract string EventHandlerEventNew();
		
		/// <summary>
        /// Método que trata o evento ACTIV_New (Criação de uma atividade)
        /// </summary>
        /// <returns>string to renomear a atividade</returns>
        public abstract string EventHandlerActivNew();
		
		/// <summary>
        /// Método que trata o evento GATEW_New (Criação de um gateway)
        /// </summary>
        /// <returns>string to renomear o gateway</returns>
        public abstract string EventHandlerGatewNew();
		
		/// <summary>
        /// Método que trata o evento CONN_New (Criação de uma relação entre dois elementos)
        /// </summary>
        /// <returns>true se a operação foi bem sucedida</returns>
        public abstract bool EventHandlerRelaNew();
        
		/// <summary>
        /// Método que remove um evento do workflow
        /// </summary>
        /// <returns>true se a operação for bem sucedida</returns>
        public abstract bool EventHandlerEventDelete();
		
		/// <summary>
        /// Método que remove uma atividade do workflow
        /// </summary>
        /// <returns>true se a operação for bem sucedida</returns>
        public abstract bool EventHandlerActivDelete();
		
		/// <summary>
        /// Método que remove um gateway do workflow
        /// </summary>
        /// <returns>true se a operação for bem sucedida</returns>
        public abstract bool EventHandlerGatewDelete();
		
		/// <summary>
        /// Método que remove uma relação do workflow
        /// </summary>
        /// <returns>true se a operação for bem sucedida</returns>
        public abstract bool EventHandlerRelaDelete() ;

        protected abstract string LoadStaticOptions();

        protected virtual bool WriteToBD(bool isFinal = true)
        {
            return true;
        }
    }
}




