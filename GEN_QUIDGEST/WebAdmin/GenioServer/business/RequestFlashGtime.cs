using System;
using CSGenio.business;
using System.Collections.Generic;
using CSGenio.persistence;
using CSGenio.framework;
using System.Data;
using System.Text;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.IO;
using System.Xml;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace CSGenio.business
{
	/// <summary>
	/// Classe que representa um pedido html do gráfico gtime
	/// </summary>
		public abstract class PedidoFlashGTime : ExtControl
	{
       /// <summary>
        /// Enumerado com os tipos de comando possíveis
        /// </summary>
        public enum CommandType { LoadChart, RefreshChart, EditPeriod, DeletePeriod, InsertEvent, ViewPeriod, MovePeriod, ResizePeriod, CustomAction };
        
        /// <summary>
        /// Enumerado com os tipos de resposta possíveis
        /// </summary>
        public enum CommandResponseType { LoadChart, EventResponse, RefreshChart, InsertEvent, EditPeriod, DeletePeriod, MovePeriod, ResizePeriod, CustomAction };

        /// <summary>
        /// Tipo de comando do pedido
        /// </summary>
        protected CommandType tpComando;

        /// <summary>
        /// Parâmetro do pedido
        /// </summary>
        protected string parametro;

        /// <summary>
        /// Campos de historial do pedido
        /// </summary>
        protected string[] camposHistorial;

        /// <summary>
        /// Utilizador em Sessão
        /// </summary>
        protected User user;
		
		/// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="args">argumentos do pedido</param>
        /// <param name="Utilizador">Utilizador em sessão</param>
        public PedidoFlashGTime(string[] args, User user)
        {
            int nrArgs = args.Length;//nº de argumentos no pedido
            if (nrArgs < 3)//se tiver menos que 3 é erro, 1- comando,  2 - parametro , 3 - nome do grafico
				throw new BusinessException("Erro a carregar o Grafico HTML.", "PedidoFlashGTime", "Insufficient number of arguments.");
            this.tpComando = (CommandType)Enum.Parse(typeof(CommandType), args[0]);//tipo de comando
            this.parametro = args[1];//parâmetro
            this.camposHistorial = new string[nrArgs - 3];//campos do historial
            for (int i = 3; i < nrArgs; i++)
            {
                this.camposHistorial[i - 3] = args[i];
                //this.camposHistorialResposta += "[" + args[i];
            }
            this.user = user;//Utilizador em sessão
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
                switch (tpComando)
                {
					case CommandType.LoadChart:
                        response.AddRange(new string[] { CommandResponseType.LoadChart.ToString(), loadChart(false)});
                        break;
					case CommandType.RefreshChart:
                        response.AddRange(new string[] { CommandResponseType.RefreshChart.ToString(), loadChart(true) });
                        break;
                    case CommandType.InsertEvent:
                        {
                            string[] args = parametro.Split(';');
                            DateTime day = DateTime.Parse(args[1]);
                            string id = args[0];
                            response.AddRange(new string[] { CommandResponseType.InsertEvent.ToString(), AddEvent(id, day) });
                        }
                        break;
                    case CommandType.DeletePeriod:
                        response.AddRange(new string[] { CommandResponseType.DeletePeriod.ToString(), DeletePeriod(parametro) });
                        break;
                    case CommandType.EditPeriod:
                        response.AddRange(new string[] { CommandResponseType.EditPeriod.ToString(), EditPeriod(parametro) });
                        break;
                    case CommandType.ViewPeriod:
                        response.AddRange(new string[] { CommandResponseType.EventResponse.ToString(), ViewPeriod(parametro) });
                        break;
                    case CommandType.MovePeriod:
                        {
                            string[] args = parametro.Split(';');
                            DateTime startDate = DateTime.Parse(args[1]);
                            DateTime endDate = DateTime.Parse(args[2]);
                            string id = args[0];
                            response.AddRange(new string[] { CommandResponseType.MovePeriod.ToString(), MovePeriod(id, startDate, endDate) });
                        }
                        break;
                    case CommandType.ResizePeriod:
                        {
                            string[] args = parametro.Split(';');
                            DateTime startDate = DateTime.Parse(args[1]);
                            DateTime endDate = DateTime.Parse(args[2]);
                            string id = args[0];
                            response.AddRange(new string[] { CommandResponseType.ResizePeriod.ToString(), ResizePeriod(id, startDate, endDate) });
                        }
                        break;
					case CommandType.CustomAction:
                        {
                            string[] args = parametro.Split(';');
                            string customAction = args[0];
                            string period = args[1];
                            response.AddRange(new string[] { CommandResponseType.CustomAction.ToString(), CustomAction(customAction, period) });
                        }
                        break;
                    default:
                        throw new BusinessException(null, "PedidoFlashGTime.processRequest", "Command not defined: " + tpComando.ToString());
                }
				response.AddRange(this.camposHistorial);
                return response.ToArray();
            }
            catch (GenioException ex)
			{
				if (ex.ExceptionSite == "PedidoFlashGTime.processRequest")
					throw;
				throw new BusinessException(ex.UserMessage, "PedidoFlashGTime.processRequest", "Error processing Flash request: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new BusinessException(null, "PedidoFlashGTime.processRequest", "Error processing Flash request: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Método que devolve ou coloca o parametro do pedido
        /// </summary>
        public string Parametro
        {
            get { return parametro; }
        }

        /// <summary>
        /// Método que devolve ou coloca os campos do historial
        /// </summary>
        public string[] CamposHistorial
        {
            get { return camposHistorial; }
            set { camposHistorial = value; }
        }


#pragma warning disable CS0414
        [DataContract]
        public class MARKTYPE
        {
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
            [XmlAttribute(AttributeName = "MINDISPLACEMENT")]
            public string MINDISPLACEMENT { get; set; }
            [XmlAttribute(AttributeName = "DISPLACEMENTUNIT")]
            public string DISPLACEMENTUNIT { get; set; }
            [XmlAttribute(AttributeName = "MARKUNIT")]
            public string MARKUNIT { get; set; }
            [XmlAttribute(AttributeName = "AUTH")]
            public string AUTH { get; set; }
            [XmlAttribute(AttributeName = "STYLE")]
            public string STYLE { get; set; }
			[XmlAttribute(AttributeName = "STYLECLASS")]
            public string STYLECLASS { get; set; }
        }
		
		[DataContract]
        public class CUSTOMACTION
        {
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
            [XmlAttribute(AttributeName = "TITLE")]
            public string TITLE { get; set; }
        }

        [DataContract]
        public class PERIOD
        {
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
            [XmlAttribute(AttributeName = "START")]
            public string START { get; set; }
            [XmlAttribute(AttributeName = "END")]
            public string END { get; set; }
            [XmlAttribute(AttributeName = "REPEAT")]
            public string REPEAT { get; set; }
            [XmlAttribute(AttributeName = "TYPE")]
            public string TYPE { get; set; }
            [XmlAttribute(AttributeName = "TITLE")]
            public string TITLE { get; set; }
			[XmlAttribute(AttributeName = "TEXT")]
            public string TEXT { get; set; }
        }

        [DataContract]
        public class GROUP
        {
            [XmlElement(ElementName = "PERIOD")]
            public List<PERIOD> PERIOD { get; set; }
            [XmlElement(ElementName = "GROUP")]
            public List<GROUP> subgrupo { get; set; }

            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
            [XmlAttribute(AttributeName = "TITLE")]
            public string TITLE { get; set; }
            [XmlAttribute(AttributeName = "LINEHEIGHT")]
            public string LINEHEIGHT { get; set; }
            [XmlAttribute(AttributeName = "AUTHINS")]
            public string AUTHINS { get; set; }
            [XmlAttribute(AttributeName = "MINDISPLACEMENT")]
            public string MINDISPLACEMENT { get; set; }
            [XmlAttribute(AttributeName = "DISPLACEMENTUNIT")]
            public string DISPLACEMENTUNIT { get; set; }
        }


        [DataContract]
        public class GTIME
        {
            [XmlElement(ElementName = "MARKTYPE")]
            public List<MARKTYPE> MARKTYPE { get; set; }
            [XmlElement(ElementName = "CUSTOMACTION")]
            public List<CUSTOMACTION> CUSTOMACTION { get; set; }
            [XmlElement(ElementName = "GROUP")]
            public List<GROUP> GROUP { get; set; }
            [XmlAttribute(AttributeName = "EDIT")]
            public string EDIT { get; set; }
            [XmlAttribute(AttributeName = "TITLE")]
            public string TITLE { get; set; }
            [XmlAttribute(AttributeName = "SHOWHEADER")]
            public string SHOWHEADER { get; set; }
            [XmlAttribute(AttributeName = "SHOWTREE")]
            public string SHOWTREE { get; set; }
            [XmlAttribute(AttributeName = "SHOWPRINT")]
            public string SHOWPRINT { get; set; }
            [XmlAttribute(AttributeName = "SHOWINSERT")]
            public string SHOWINSERT { get; set; }
            [XmlAttribute(AttributeName = "STYLESHEET")]
            public string STYLESHEET { get; set; }
            [XmlAttribute(AttributeName = "LANGUAGE")]
            public string LANGUAGE { get; set; }
            [XmlAttribute(AttributeName = "LANGFILE")]
            public string LANGFILE { get; set; }
            [XmlAttribute(AttributeName = "STARTDATE")]
            public string STARTDATE { get; set; }
            [XmlAttribute(AttributeName = "ENDDATE")]
            public string ENDDATE { get; set; }
            [XmlAttribute(AttributeName = "DAYSTARTHOUR")]
            public string DAYSTARTHOUR { get; set; }
            [XmlAttribute(AttributeName = "DAYENDHOUR")]
            public string DAYENDHOUR { get; set; }
            [XmlAttribute(AttributeName = "LINEHEIGHT")]
            public string LINEHEIGHT { get; set; }
            [XmlAttribute(AttributeName = "ALLOWZOOM")]
            public string ALLOWZOOM { get; set; }
            [XmlAttribute(AttributeName = "STARTZOOM")]
            public string STARTZOOM { get; set; }
			[XmlAttribute(AttributeName = "SHOWZOOM")]
			public string SHOWZOOM { get; set; }
            [XmlAttribute(AttributeName = "DATENOW")]
            public string DATENOW { get; set; }

			public GTIME()
            {
				EDIT = "N";
                TITLE = "";
                LANGUAGE = "PTPT";
                LANGFILE = "GtimeLang";
                DAYSTARTHOUR = "0";
                DAYENDHOUR = "24";
                ALLOWZOOM = "";     
            }
			
			public string Serialize()
            {
                XmlSerializer xsSubmit = new XmlSerializer(typeof(GTIME));
                var subReq = this;
                var xml = "";

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                using (var sww = new StringWriter())
                {
                    using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true, Encoding = Encoding.ASCII }))
                    {
                        xsSubmit.Serialize(writer, subReq, ns);
                        xml = sww.ToString();
                    }
                }

                return xml;
            }
        }
        #pragma warning restore CS0414

        /// <summary>
        /// This methods returns a json file with all the necessary data to draw a calendar.
        /// Just edit the calendar instance that is created in the beginning of the method.
        /// </summary>
        /// <returns>A xml string representing with the serialization of the calendar instance</returns>
        public abstract string loadChart(bool refresh);
		
		public abstract string ViewPeriod(string period);
        
		public abstract string EditPeriod(string period);
        
        public abstract string DeletePeriod(string period);
		
		public abstract string AddEvent(string id, DateTime day);
		
		public abstract string MovePeriod(string id, DateTime startDate, DateTime endDate);
		
		public abstract string ResizePeriod(string id, DateTime startDate, DateTime endDate);
		
		public abstract string CustomAction(string customAction, string period);

	}
}
