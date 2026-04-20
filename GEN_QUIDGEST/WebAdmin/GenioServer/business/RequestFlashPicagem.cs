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
using System.Xml;

namespace CSGenio.business
{

    //classe que representa o horário de trabalho recebido do flash
    //Exemplo de horario recebido
    /*  <HORTRABALHO>
        <GRAF INICIO = "09:30§13:00§12:04§19:31§13:18§§09:30§13:20"
        FIM = "12:00§17:30§13:18§§17:30§§23:59§23:59"
        LISTA = "1§1§2§2§3§§4§4"
        MOTIVO = "§03 ASS. FAM.Men. 10"
        TLMOTIVO = "01 DISPENSA§02 INT. CAFÉ§03 ASS. FAM.Men. 10§04 FÉRIAS§05 1/2  FÉRIAS§06 SERV. EXTERNO§07 MATERNIDADE§08 AMAMENTAÇÃO§09 DOENÇA  &lt; 60 D§10 DOENÇA &gt; 60 D§11 INTERNAMENTO§12 ASS. FAMILIA§13 TRAB. ESTUDANTE§14 INJUSTIF§15 LIÇ. S/V 90 D§16 LIÇ. S/V 1 ANO§17 LIÇ. S/V LD§18 ATR. TRANS.§19 NASCIMENTO§20 GRAVIDEZ RISCO§21 ACID. SERV.§22 T. AMBULATÓRIO§23 ART.º 66º 1/2§24 ART.º 66§25 DADOR SANGUE§26 SAIDA JUDICIAL§27 JUSTIFICADO§28 ALMOÇO NATAL§29 GREVE TRANS.§30 JUNTA MÉDICA§31 ALMOÇO§32 FESTA NATAL§33 ELEIÇÕES§34 AVARIA TRANS.§35 GREVE§36 FALTA INJUS. 1/2§37 FALTA INJUS.§38 PROVAS/CONCURSO§39 MÉD/CASLISBOA§40 GUIA MARCHA§41 FUNERAL§42 ALMOÇO/SERVIÇO§43 ESTÁGIO§44 PEREGRINAÇÃO§45 PATERNIDADE§46 DESBARATIZAÇÃO§47 LIC. PARENTAL§48 COMPENSAÇÃO§49 FORMAÇÃO§50 NOJO§51 CASAMENTO§52 SEGURO§53 TOL. PONTO§54 REQUISIÇÃO"/>
        CHORARIO = S
        CPICAGEM = S
        CPERIODO S
        CATRASO = S
        OBSERVAC = observacoes
        </HORTRABALHO>*/

    public class WorkSchedule
    {
        public List<string> start = new List<string>();//start de cada Qvalue
        public List<string> fim = new List<string>();//fim de cada Qvalue
        public List<string> lista = new List<string>();//lista de tipos de Qvalues
        public List<string> motivo = new List<string>();//lista de motivos de atraso
        public List<string> lista_motivos = new List<string>();//lista de motivos de atrasos geral
        public bool horarioConsulta;//se o horário é de consulta, i.e não pode ser alterado
        public bool picagemConsulta;//se as picagens são de consulta, i.e não podem ser alteradas 
        public bool periodoConsulta;//se os periodos de presença são de consulta, i.e não podem ser alterados
        public bool atrasoConsulta;//se os atrasos são só de consulta, i.e não podem ser alterados
        public enum RegistrationType { horario = 1, periodo = 2, atraso = 3, picagem = 4 };//tipo de registo (adicionado à variável lista)
        public static char separador = '§';//separador utilizado na comunicação com o flash
        public string isDirigente;//se é dirigente
        public string observacoes; //se são observacoes

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="consultaHorario">se o horário é de consulta, i.e não pode ser alterado</param>
        /// <param name="consultaPicagem">se as picagens são de consulta, i.e não podem ser alteradas</param>
        /// <param name="consultaPeriodo">se os periodos de presença são de consulta, i.e não podem ser alterados</param>
        /// <param name="consultaAtraso">se os atrasos são só de consulta, i.e não podem ser alterados</param>
        public WorkSchedule(bool consultaHorario, bool consultaPicagem, bool consultaPeriodo, bool consultaAtraso, string dirigente)
        {
            horarioConsulta = consultaHorario;
            picagemConsulta = consultaPicagem;
            periodoConsulta = consultaPeriodo;
            atrasoConsulta = consultaAtraso;
            isDirigente = dirigente;

        }
       
        /// <summary>
        /// Método que constroi o xml a enviar to o flash
        /// </summary>
        /// <returns></returns>
        public string buildXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootNode = xmlDoc.CreateElement("HORTRABALHO");

            XmlElement graf = xmlDoc.CreateElement("GRAF");

            string valInicio = "";
            foreach (string valorInicio in start)
            {
                if (!string.IsNullOrEmpty(valInicio))
                    valInicio += separador;
               
                valInicio += valorInicio;
            }
            graf.SetAttribute("INICIO", valInicio);
            string valFim = "";
            foreach (string valorFim in fim)
            {
                if (!string.IsNullOrEmpty(valFim))
                    valFim += separador;                
                
				valFim += valorFim;
            }
            graf.SetAttribute("FIM", valFim);
            string valLista = "";
            foreach (string valorLista in lista)
            {
                if (!string.IsNullOrEmpty(valLista))
                    valLista += separador;
                
                valLista += valorLista;
            }
            graf.SetAttribute("LISTA", valLista);
            string valMotivo = separador.ToString();
            foreach (string valorMotivo in motivo)
            {
                valMotivo += valorMotivo + separador;
            }
            graf.SetAttribute("MOTIVO", valMotivo);
            string valT1Motivo = "";
            foreach (string valorTlMotivo in lista_motivos)
            {
                if (!string.IsNullOrEmpty(valT1Motivo))
                    valT1Motivo += separador;
                
				valT1Motivo += valorTlMotivo;
            }
            graf.SetAttribute("TLMOTIVO", valT1Motivo);
            if (horarioConsulta)
                graf.SetAttribute("CHORARIO","S");
            if (picagemConsulta)
            {
                graf.SetAttribute("CPICAGEM", "S");
                graf.SetAttribute("PERIODO","S");
                graf.SetAttribute("CATRASO","S");
            }
            if (isDirigente.Equals("S"))
                graf.SetAttribute("CDIRIG","S");
            else
                graf.SetAttribute("CDIRIG","N");
            graf.SetAttribute("OBSERVAC",observacoes);

            rootNode.AppendChild(graf);
            return rootNode.OuterXml;
        }        
    }

	/// <summary>
	/// Classe que representa um pedido flash de picagem
	/// </summary>
	public abstract class RequestFlashAttendance
		: ExtControl
	{
        /// <summary>
        /// Enumerado com os tipos de comando possíveis
        /// LoadFlash é o comando que corresponde ao pedido dos dados do flash
        /// GetGraficoXml é o comando que corresponde ao pedido to introduce as alterações que o user forneceu
        /// </summary>
        public enum CommandType { LoadFlash, GetGraficoXml };

        /// <summary>
        /// Enumerado com os tipos de resposta possíveis
        /// LoadGraficoXml é a resposta do servidor to carregar o gráfico
        /// GetGraficoXml é a resposta ao xml que o servidor recebe com os dados alterados pelo user
        /// </summary>
        public enum CommandResponseType { LoadGraficoXml, GetGraficoXml };

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
        /// User em Sessão
        /// </summary>
        private User user;

        /// <summary>
        /// Função que vai ser invocada to criar um flash picagem do tipo do pedido
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="utilizador">user</param>
        /// <returns>O Pedido flash vai ser inicializado</returns>
        public delegate RequestFlashAttendance CreatesFlashAttendance(string[] args,User user);
        
        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="utilizador">user em sessão</param>
        public RequestFlashAttendance(string[] args, User user)
        {
            int nrArgs = args.Length;//nº de argumentos no pedido
            if (nrArgs < 3)//se tiver menos que 3 é erro, 1- comando,  2 - parametro , 3 - name do grafico
				throw new BusinessException("Erro a carregar o Flash de picagem.", "RequestFlashAttendance.RequestFlashAttendance", "Insufficient number of arguments.");
            this.tipoComando = (CommandType)Enum.Parse(typeof(CommandType), args[0]);//tipo de comando
            this.parametro = args[1];//parâmetro
            this.camposHistorial = new string[nrArgs - 3];//fields do historial
            for (int i = 3; i < nrArgs; i++)
            {
                this.camposHistorial[i - 3] = args[i];
                //this.camposHistorialResposta += "[" + args[i];
            }
            this.user = user;//user em sessão
        }

        /// <summary>
        /// Função que executa o pedido
        /// </summary>
        /// <returns>a resposta ao pedido</returns>
        public override object processRequest()
        {
			List<string> response = new List<string>();
            try
            {
                switch (tipoComando)
                {
                    case CommandType.LoadFlash:
                        response.AddRange(new string[] { CommandResponseType.LoadGraficoXml.ToString(), sendFlash()});
                        break;						
                    case CommandType.GetGraficoXml:
                        response.AddRange(new string[] { CommandResponseType.GetGraficoXml.ToString(), receivesFlash()});
                        break;						
                    default:
						throw new BusinessException(null, "RequestFlashAttendance.processRequest", "Command not defined: " + tipoComando.ToString());
                }
				response.AddRange(this.camposHistorial);
                return response.ToArray();
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "RequestFlashAttendance.processRequest")
					throw;
				throw new BusinessException(ex.UserMessage, "RequestFlashAttendance.processRequest", "Error processing Flash request: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "RequestFlashAttendance.processRequest", "Error processing Flash request: " + ex.Message, ex);
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
        public abstract string sendFlash();

        /// <summary>
        /// Método usado to responder aos pedidos LoadObject que correspondem aos registos
        /// que vão ser visualizados no flash
        /// </summary>
        /// <returns>uma string com o xml que corresponde aos registos que devem ser visualizados no flash</returns>
        public abstract string receivesFlash();
	}
}
