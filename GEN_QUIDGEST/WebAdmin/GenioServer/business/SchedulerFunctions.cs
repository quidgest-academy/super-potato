using System;
using CSGenio.framework;
using CSGenio.persistence;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using CSGenio;
using CSGenio.business;
using ExecuteQueryCore;
using CSGenio.core.persistence;

namespace GenioServer.business
{
    //public enum CallAck
    //{
    //    OK,
    //    Rejected,
    //    Failed
    //}

    //public class QueueResponse
    //{
    //    public string MsgId;
    //    public string Desc;
    //    public CallAck Ack;

    //    /* -status da operação
    //    * -descrição da operação(no cso de erro)
    //    * -token ou id da mensagem
    //    * */
    //}

    public class SchedulerCallFunctions
    {
        private User user;
        private string module;
        private PersistentSupport sp;
        //private Dictionary<string, string> argsDict;

        public SchedulerCallFunctions(User user, string module, PersistentSupport sp)
        {
            this.user = user;
            this.sp = sp;
            this.module = module;
        }

        public QueueResponse CallFunction(string functionName, Dictionary<string, string> argsDict)
        {
            if (Log.IsDebugEnabled) Log.Debug("Call scheduler function: " + functionName);
            functionName = functionName.ToUpper(); //JGF 2022.02.22 Made it case insensitive
            switch (functionName)
            {
                case "REINDEX":
                    // Requires SCRIPTS parameter
                    if (!argsDict.ContainsKey("SCRIPTS"))
                        throw new BusinessException(null, "SchedulerCallFunctions.CallFunction", "Invalid reindex function call: SCRIPTS information is missing.");
                
                    // ZERORTRUE parameter is optional (defaults to false)
                    bool zero = false;
                    if (argsDict.ContainsKey("ZEROTRUE"))
                    {
                        string zeroTrue = argsDict["ZEROTRUE"];
                        if (zeroTrue == "1" || zeroTrue.ToLower() == "true")
                            zero = true;
                    }
                    return Reindex(argsDict["YEARAPP"], argsDict["USERNAME"], argsDict["PASSWORD"], argsDict["SCRIPTS"], zero);                        
                case "TRANSFERLOGS":
                    return TransferLogs(argsDict["YEARAPP"]);
                case "SCHEDULEDPROCESS":
                    return RunScheduledProcess();
                case "NOTIFICATIONS":
                    if (argsDict.ContainsKey("NOTIFICATIONID"))
                        return RunNotifications(argsDict["NOTIFICATIONID"]);
                    else
                        return RunNotifications(null);					
                default:
                    Log.Error($"[CallFunction] {functionName} was not found.");
                    return null;
            }
        }

        #region internal_data_structures
        // Status das Queues da MQQueues
        /// Define o estado a messagem.
        enum MQueueACK
        {
            //to carimbar o status durante o envio

            /// <summary>
            /// quando a queue deu erro a ser enviada to o servidor de queues.
            /// </summary>
            SendFAIL = 0,

            /// <summary>
            /// quando a queue foi enviada com sucesso to o servidor de queues e está 
            /// apenas à espera da resposta do integrador do system target.
            /// </summary>
            SendINPROGRESS = 1,

            /// <summary>
            /// quando a queue foi enviada com sucesso to o servidor de queues e está
            /// apenas à espera da resposta do integrador do system target.
            /// </summary>
            SendEXPIRED = 2,

            //to carimbar o status da resposta

            /// <summary>
            /// quando a queue chegou ao integrador de target e foi integrada correctamente.
            /// </summary>
            ReplyOK = 3,

            /// <summary>
            /// quando a queue foi rejeitada pelo system target(integrador), nesta situação ver mensagem descritiva. 
            /// </summary>
            ReplyREJECT = 4,

            /// <summary>
            /// quando ocorreu um erro técnico a processar a queue pelo system target(integrador), nesta situação ver mensagem descritiva. 
            /// </summary>
            ReplyFAIL = 5
        }

        /// Define o tipo de funcionamento do mecanismo de Queues.
        enum MQueueSRVTYPE
        {
            /// <summary>
            /// Mode classico como sempre esteve a funcionar.
            /// </summary>
            CLASSIC = 0,

            /// <summary>
            /// Mode Journal, por cada queue enviada to o servidor, é guardada uma copia na 
            /// table repositório "mqqueues", to tratamento posterior.
            /// </summary>
            JOURNAL = 1
        }

        /// Define o tipo de operação (MSGTYPE) que a queue vai efectuar.
        enum MQueueTYPE
        {
            /// <summary>
            /// criação de um novo registo.
            /// </summary>
            C = 'C',

            /// <summary>
            /// eliminação de um registo.
            /// </summary>
            D = 'D',

            /// <summary>
            /// edição de um registo.
            /// </summary>
            U = 'U',

            /// <summary>
            /// Serve to verificar se um registo relacionado acima da table em causa (relação N:1) exists e 
            /// gravá-lo se necessário. Se o registo exists, não é editado, to tal serve a queue da própria table.
            /// </summary>
            N = 'N'
        }

        //Define MQ export/import data types
        enum MQDataType
        {
            _STRING_TYPE = 'C',
            _NUMERIC_TYPE = 'N',
            _DATE_TYPE = 'D',
            _LOGIC_TYPE = 'L',
            _CURRENCY_TYPE = '$'
        }
        
        enum MQPropertyField
        {
            TIPO,//= "TIPO",
            COMP,//= "COMP",
            DC,// = "DC",
            VL,// = "VL"
        }

        #endregion

        #region Methods

        private void mqImportMQRecord(Area pTab)
        {
        }

        private string GetAttribute(XmlNode ObjNode, string attribute)
        {
            return ObjNode.Attributes.GetNamedItem(attribute).Value;
        }

        void GetFieldEx(XmlNode ObjNode, string fName, out string value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
                throw new BusinessException(null, "SchedulerCallFunctions.GetFieldEx", "Field not found: " + fName);
            }
        }

        void GetFieldEx(XmlNode ObjNode, string fName, out DateTime value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
				throw new BusinessException(null, "SchedulerCallFunctions.GetFieldEx", "Field not found: " + fName);
            }
        }

        void GetFieldEx(XmlNode ObjNode, string fName, out decimal value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
                throw new BusinessException(null, "SchedulerCallFunctions.GetFieldEx", "Field not found: " + fName);
            }
        }

        void GetFieldEx(XmlNode ObjNode, string fName, out int value)
        {
            if (!GetField(ObjNode, fName,out value))
            {			
                throw new BusinessException(null, "SchedulerCallFunctions.GetFieldEx", "Field not found: " + fName);
            }
        }

        private object GetFielProperty(XmlNode node, MQPropertyField propertie)
        {
            foreach (XmlNode elem in node.ChildNodes)
            {
                if (elem.Name != propertie.ToString())
                    continue;

                if (propertie == MQPropertyField.TIPO)
                    return (MQDataType)elem.InnerText[0];
                else
                    return elem.InnerText;                
            }
            return null;
        }

        private bool GetField(XmlNode ObjNode, string fName, out string value)
        {
            XmlNode node = null;
            foreach (XmlNode item in ObjNode.ChildNodes)
            {
                if (item.Name == fName)
                {
                    node = item;
                    break;
                }
            }
            bool res = false;
            value = "";

            if (node != null)
            {
                res = true;
                MQDataType node_type = (MQDataType)GetFielProperty(node,MQPropertyField.TIPO);

                if (node_type == MQDataType._STRING_TYPE)
                    value = (string)GetFielProperty(node,MQPropertyField.VL);
            }
            return res;
        }

        private bool GetField(XmlNode ObjNode, string fName, out int value)
        {
            XmlNode node = null;
            foreach (XmlNode item in ObjNode.ChildNodes)
            {
                if (item.Name == fName)
                {
                    node = item;
                    break;
                }
            }
            bool res = false;
            value = 0;

            if (node != null)
            {
                res = true;
                MQDataType node_type = (MQDataType)GetFielProperty(node, MQPropertyField.TIPO);
                if (node_type == MQDataType._LOGIC_TYPE) //convert int to string
                    int.TryParse(GetFielProperty(node, MQPropertyField.VL).ToString(),out value);
            }
            return res;
        }

        private bool GetField(XmlNode ObjNode, string fName, out decimal value)
        {
            XmlNode node = null;
            foreach (XmlNode item in ObjNode.ChildNodes)
            {
                if (item.Name == fName)
                {
                    node = item;
                    break;
                }
            }
            bool res = false;
            value = 0;

            if (node != null)
            {
                res = true;
                MQDataType node_type = (MQDataType)GetFielProperty(node, MQPropertyField.TIPO);
                if (node_type == MQDataType._CURRENCY_TYPE || node_type == MQDataType._NUMERIC_TYPE)
                    decimal.TryParse(GetFielProperty(node, MQPropertyField.VL).ToString(),out value);
            }
            return res;
        }

        private bool GetField(XmlNode ObjNode, string fName, out DateTime value)
        {
            XmlNode node = null;
            foreach (XmlNode item in ObjNode.ChildNodes)
            {
                if (item.Name == fName)
                {
                    node = item;
                    break;
                }
            }
            bool res = false;
            value = DateTime.MinValue;

            if (node != null)
            {
                res = true;
                MQDataType node_type = (MQDataType)GetFielProperty(node, MQPropertyField.TIPO);
                if (node_type == MQDataType._DATE_TYPE)//10/11/2014
                    value = DateTime.Parse(GetFielProperty(node, MQPropertyField.VL).ToString());//,"DD/MM/YYYY", CultureInfo.InvariantCulture);// ef->m_Value.ODT().Format(_T("%d/%m/%Y")); //estava %m/%d/%Y JP 12/03/2003
            }
            return res;
        }

        private XmlNodeList GetRelations_1N(XmlNode ObjNode)
        {
            foreach (XmlNode item in ObjNode.ChildNodes)
            {
                if (item.Name == "mqrel1n")
                {
                    return item.ChildNodes;
                }
            }
            return null;
        }

        private XmlNodeList GetRelations_N1(XmlNode ObjNode)
        {
            foreach (XmlNode item in ObjNode.ChildNodes)
            {
                if (item.Name == "mqreln1")
                {
                    return item.ChildNodes;
                }
            }
            return null;
        }
		
		private void mqACKqueue(MQueueACK mQueueACK, System.DateTime dateTime1, System.DateTime dateTime2, string description)
        {
			throw new BusinessException(null, "SchedulerCallFunctions.mqACKqueue", "Method not implemented.");
        }

        /*
        private string GetAttributeEx(XmlElement docEl)
        {
        }
        */

        /*
        private bool GetField(string fName, out int value)
        {
            XmlElement node = queueXml.GetElementById(fName);
            bool res = false;

            if (node != null)
            {
                res = true;
                MQDataType node_type = (MQDataType)GetFielProperty(node, MQPropertyField.TIPO);
                if (node_type == MQDataType._CURRENCY_TYPE || node_type == MQDataType._NUMERIC_TYPE)
                    value = (double)GetFielProperty(node, MQPropertyField.VL);

                else if (node_type == MQDataType._STRING_TYPE)
                    value = (string)GetFielProperty(node, MQPropertyField.VL);

                else if (node_type == MQDataType._DATE_TYPE)
                    value = ((DateTime)GetFielProperty(node, MQPropertyField.VL));// ef->m_Value.ODT().Format(_T("%d/%m/%Y")); //estava %m/%d/%Y JP 12/03/2003

                else if (node_type == MQDataType._LOGIC_TYPE) //convert int to string
                    value = (int)GetFielProperty(node, MQPropertyField.VL);
            }
            return res;
        }*/

        #endregion
        
        public QueueResponse Reindex(string yearApp, string username, string password, string strScripts, bool zero)
        {
            QueueResponse response = new QueueResponse();
            try
            {               
                string pathReindex = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", Configuration.Program + "_ReIdx", "Reindex");
                string pathReindexMenu = pathReindex + Path.DirectorySeparatorChar + "order2Exec.xml";

                ReindexOrder reindexMenu = ReindexOrder.readXML(pathReindexMenu);

                //select scripts to be executed
                foreach (ReIndexFunction item in reindexMenu.ReIndexItems)
                    item.Selected = false;
                    
               string[] scripts = strScripts.Split(';');
                foreach (string script in scripts)
                {
                    reindexMenu.ReIndexItems.Find(delegate(ReIndexFunction RdxFunction) { return RdxFunction.Id == script; }).Selected = true;
                }

                reindexMenu.CalculateOrder();
                List<RdxScript> order2exec = reindexMenu.GetOrderToExecute();
                
                GlobalFunctions gblFunctions = new GlobalFunctions(user, user.CurrentModule);
                var databaseVersionReader = new DatabaseVersionReader(sp);
                var hidratedScripts = gblFunctions.HidrateScripts(order2exec, databaseVersionReader);
                CSGenio.persistence.PersistentSupport.upgradeSchema(yearApp, username, password, hidratedScripts , pathReindex, "", zero); // zero deve ser escolhido pelo user

                response.Ack = business.MQueueACK.ReplyOK;
                return response;
            }
            /*catch (BusinessException ex)
            {
                response.Ack = business.MQueueACK.ReplyFAIL;
                response.Desc = ex.Message;
                return response;
            }
            catch (PersistenceException ex)
            {
                response.Ack = business.MQueueACK.ReplyFAIL;
                response.Desc = ex.Message;
                return response;
            }*/
            catch (Exception ex)
            {
                response.Ack = business.MQueueACK.ReplyFAIL;
                response.Desc = ex.Message;
                return response;
            }
        }

        public QueueResponse TransferLogs(string yearApp)
        {
            QueueResponse response = new QueueResponse();
            try
            {
				// Call log transfer from the destination database
                CSGenio.persistence.PersistentSupport logSp = CSGenio.persistence.PersistentSupport.getPersistentSupportLog(yearApp, "");
                logSp.transferLog(false, new TransferLogOperation());

                response.Ack = business.MQueueACK.ReplyOK;
                return response;
            }
            catch (Exception ex)
            {
                response.Ack = business.MQueueACK.ReplyFAIL;
                response.Desc = ex.Message;
                return response;
            }
        }     

        public QueueResponse RunScheduledProcess()
        {
            QueueResponse response = new QueueResponse();
            try
            {              
                CSGenio.business.async.GenioWorker worker = new CSGenio.business.async.GenioWorker(user);
                worker.Work();

                response.Ack = business.MQueueACK.ReplyOK;
                return response;
            }
            catch (Exception ex)
            {
                response.Ack = business.MQueueACK.ReplyFAIL;
                response.Desc = ex.Message;
                return response;
            }
        }		
		
        public QueueResponse RunNotifications(string notifid)
        {
            QueueResponse response = new QueueResponse();
            try
            {
                var notifications = PersistentSupport.getNotifications();

                //if the id is filled then we run just that one notification
                if (notifid != null && notifid != string.Empty)
                {
                    if (notifications.ContainsKey(notifid))
                    {
                        var viewModel = (Notification)notifications[notifid];
                        viewModel.RunOpen(sp, user);
                        response.Ack = business.MQueueACK.ReplyOK;
                    }
                    else
                    {
                        response.Desc = "notification id not found: " + notifid;
                        response.Ack = business.MQueueACK.ReplyFAIL;
                    }
                }
                //if the id is empty then we run all the notifications
                else
                {
                    foreach (Notification notification in notifications.Values)
                        notification.RunOpen(sp, user);
                    response.Ack = business.MQueueACK.ReplyOK;
                }
            }
            catch (Exception ex)
            {
                response.Ack = business.MQueueACK.ReplyFAIL;
                response.Desc = ex.Message;                
            }
            return response;
        }



        public QueueResponse AsyncProcess(Dictionary<string, string> args)
        {
            QueueResponse response = new QueueResponse();

            

            return response;
        }

    }
}
