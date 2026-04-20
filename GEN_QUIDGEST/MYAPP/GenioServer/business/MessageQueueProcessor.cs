using System;
using CSGenio.framework;
using CSGenio.persistence;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using CSGenio.business;
using Quidgest.Persistence.GenericQuery;
using System.Linq;

namespace GenioServer.business
{
    public class MessageQueueProcessor
    {
        static Dictionary<string,int> AllQueues;
        static Dictionary<string,int> AllQueuesError;
        static Dictionary<string,int> AllQueuesOk;
        static Dictionary<string,int> AllQueuesProgress;
        private User user;
        private string module;
        private PersistentSupport sp;
        private XmlDocument queueXml;
        
        private Object thisLock = new Object();

        static MessageQueueProcessor()
        {

            AllQueues = new Dictionary<string, int>();

            AllQueuesError = new Dictionary<string, int>();

            AllQueuesOk = new Dictionary<string, int>();

            AllQueuesProgress = new Dictionary<string, int>();
        }

        public MessageQueueProcessor(User user, string module, PersistentSupport sp, string messageData)
        {
            this.user = user;
            this.sp = sp;
            this.module = module;
            if(!string.IsNullOrEmpty(messageData))
                LoadXML(messageData);
        }

        private DateTime GetQueueTimestamp()
        {
            if (long.TryParse(GetAttributeIfExists(queueXml.DocumentElement, "timestamp") ?? "", out long tsmili))
                return DateTimeOffset.FromUnixTimeMilliseconds(tsmili).UtcDateTime;
            return DateTime.UtcNow;
        }

        public QueueResponse ProcessQueue(string channelId)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug("ProcessQueue: " + channelId + " args: " + queueXml.ToString() );

                string queue = channelId;

                //O processador vai passar a basear-se no Id que vem dentro do XML da queue e não no Id que vem na chamada do webservice
                if (queueXml.ChildNodes.Count > 0)
                {
                    XmlNode MainNode = queueXml.DocumentElement;
                    queue = GetAttribute(MainNode, "queue");
                }
                
                if(AllQueues.ContainsKey(queue)) switch (AllQueues[queue])
                {
                    default:
                        {
                            return null;
                        }
                }  

                return null;             
            }
            catch (Exception ex)
            {
                var scopeContext = Log.SetContext(new {user = "MessageQueueProcessor_ProcessMessage"});
                Log.Error("Queue " + channelId + ". " + ex.Message);
                
                if(scopeContext != null) scopeContext.Dispose();
                return null;
            }
        }
        
        /// <summary>
        /// Altera o estado da queue to ser reenviada
        /// </summary>
        /// <param name="key">Key da queue</param>
        public void ACKStateRetry(string key)
        {
            try
            {
                UpdateQuery uq = new UpdateQuery()
                           .Set(CSGenioAmqqueues.FldMQStatus, 0)
                           .Set(CSGenioAmqqueues.FldDataStatus, DateTime.Now)
                           .Update(Area.AreaMQQUEUES)
                           .Where(CriteriaSet.And().Equal(CSGenioAmqqueues.FldQueueKey, key));
                sp.Execute(uq);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[{0}] ACKStateRetry error: {1}", key, ex.Message));
            }           
        }

        /// <summary>
        /// Altera o estado da queue to finalizado
        /// </summary>
        /// <param name="key">key da queue</param>
        public void ACKStateResolved(string key)
        {
            try
            {
                UpdateQuery uq = new UpdateQuery()
                           .Set(CSGenioAmqqueues.FldMQStatus, 3)
                           .Set(CSGenioAmqqueues.FldDataStatus, DateTime.Now)
                           .Update(Area.AreaMQQUEUES)
                           .Where(CriteriaSet.And().Equal(CSGenioAmqqueues.FldQueueKey, key));
                sp.Execute(uq);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[{0}] ACKStateResolved error: {1}", key, ex.Message));
            }
        }
        
        public bool ProcessQueueError(string name, string key, MQueueACK ack, string error)
        {
            try
            {                
                if (AllQueuesError.ContainsKey(name)) switch (AllQueuesError[name])
                {
                    default:
                        {
                            return false;
                        }
                }
				return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
		
        public bool ProcessQueueOk(string name, string key, MQueueACK ack)
        {
            try
            {                
                if (AllQueuesOk.ContainsKey(name)) switch (AllQueuesOk[name])
                {
                    default:
                        {
                            return false;
                        }
                }
				return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool ProcessQueueProgress(string name, string key, MQueueACK ack, decimal progress, string message)
        {
            try
            {                
                if (AllQueuesProgress.ContainsKey(name)) switch (AllQueuesProgress[name])
                {
                    default:
                        {
                            return false;
                        }
                }
				return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        
        public bool QueueExists(string channelId)
        {
            string queue = channelId;
            //O processador vai passar a basear-se no Id que vem dentro do XML da queue e não no Id que vem na chamada do webservice
            if (queueXml.ChildNodes.Count > 0)
            {
                XmlNode MainNode = queueXml.DocumentElement;
                queue = GetAttribute(MainNode, "queue");
            }

            if (AllQueues.ContainsKey(queue))
                return true;
            else
                return false;
        }

        public void LoadXML(string messageData)
        {            
            queueXml = new XmlDocument();
            queueXml.LoadXml(messageData);                        
        }
        
        public IsAckResponse IsACKQueue()
        {
            string key = "";
            string type = "";
            string isACK = "";
            string codmqqueues = "";

            XmlNode MainNode = queueXml.DocumentElement;
            key = GetAttribute(MainNode, "guid");
            type = GetAttribute(MainNode, "tp");
            isACK = GetAttributeIfExists(MainNode, "isackqueue");

            if (isACK == null || !isACK.Equals("true"))
                return new IsAckResponse { IsACK = false, QueueKey = "" };
                        
            SelectQuery selQuery = new SelectQuery()
                    .Select(CSGenioAmqqueues.FldCodmqqueues)
                    .From(Area.AreaMQQUEUES)
                    .Where(CriteriaSet.And()
                        .Equal(CSGenioAmqqueues.FldQueueKey, key)
                        .NotEqual(CSGenioAmqqueues.FldQueueKey, CSGenioAmqqueues.FldTabelaCod)
                    );
            selQuery.noLock = true;
            DataMatrix data = sp.Execute(selQuery);
            if (data.NumRows > 0)
                codmqqueues = data.GetKey(0, 0);

            return new IsAckResponse { IsACK = true, QueueKey = codmqqueues};
        }

        public void ExportAllQueues(String queue, Func<QueueProgressStatus, bool> progressMethod, string idStatus, CriteriaSet criteria=null)
        {
            try
            {
                QueueGenio queueArea = Area.GetAllQueues().Find(x => x.Name.Equals(queue));
                if (queueArea != null)
                {                    
                    Type type = Area.GetTypeArea(queueArea.Queuearea.ToLower());
                    System.Reflection.MethodInfo methodInfo = type.GetMethod("searchList", new[] { typeof(PersistentSupport), typeof(User), typeof(CriteriaSet), typeof(string[]), typeof(bool), typeof(bool) });
                    CriteriaSet condition = CriteriaSet.And().Equal(queueArea.Queuearea.ToLower(), "zzstate", 0);
                    if (criteria != null) condition.SubSet(criteria);
                    var result = methodInfo.Invoke(null, new object[] { sp, user, condition, null, false, false });

                    if (result != null)
                    {
                        List<object> listAreas = new List<object>((IEnumerable<Object>)result);
                        int total = listAreas.Count;
                        for( int i = 0; i < total; i++ )
                        {
                            DbArea db = (DbArea)listAreas[i];
                            lock (thisLock)
                                db.insertQueue(sp, "U", null, queue);
                            progressMethod(new QueueProgressStatus {id = idStatus, Total = total, Count = i + 1});
                        }
                        progressMethod(new QueueProgressStatus { id = idStatus, Total = total, Count = total, Completed = true });
                    }                    
                }
            }
            catch (Exception ex)
            {
                Log.Error("Erro na exportação de queues. queue: " + queue + " " + ex.Message);                
            }            
        }
        

        public bool ProcessACKQueue(string name, string codQueue)
        {            
            try
            {
                if (queueXml.ChildNodes.Count > 0)
                {
                    string ano_app, table_rec, key, queue, desc, mqstatus;
                    decimal progress;
                    MQueueACK statusMQ;

                    table_rec = queueXml.DocumentElement.GetAttribute("table");

                    XmlNode MainNode = queueXml.DocumentElement;
                    ano_app = GetAttribute(MainNode, "year");                    
                    key = GetAttribute(MainNode, "guid");
                    queue = GetAttribute(MainNode, "queue");                                                            
                    GetFieldEx(MainNode, "mqstatus", out mqstatus);
                    GetFieldEx(MainNode, "descr", out desc);
                    GetField(MainNode, "progress", out progress);

                    if (!String.IsNullOrEmpty(mqstatus) && !String.IsNullOrEmpty(key))
                    {                        
                        statusMQ = (MQueueACK)Enum.Parse(typeof(MQueueACK), mqstatus);
                        
                        UpdateQuery uQuery = new UpdateQuery()
                            .Update(Area.AreaMQQUEUES)
                            .Set(CSGenioAmqqueues.FldMQStatus, (int)statusMQ)
                            .Set(CSGenioAmqqueues.FldResposta, desc.Length > 200 ? desc.Substring(0, 200) : desc)
                            .Set(CSGenioAmqqueues.FldDataStatus, DateTime.Now)
                            .Where(CriteriaSet.And()
                                .Equal(CSGenioAmqqueues.FldCodmqqueues, codQueue)
                            );
                        sp.Execute(uQuery);
                        
                        if(statusMQ != MQueueACK.ReplyOK && statusMQ != MQueueACK.ReplyPROGRESS)
                            ProcessQueueError(queue, key, statusMQ, desc);

                        if(statusMQ == MQueueACK.ReplyOK)
							ProcessQueueOk(queue, key, statusMQ);

                        if(statusMQ == MQueueACK.ReplyPROGRESS)
                            ProcessQueueProgress(queue, key, statusMQ, progress, desc);
                                                
                        return true;
                    }
                }
                else
                {
                    Log.Error("[ProcessACKQueue] Queue fora de formato " + name);
                }
                return false ;         
            }
            catch (Exception ex)
            {
                Log.Error("Erro a processar ACKQUEUE: " + name + " " + ex.Message);
                return false;
            }
        }
        
        #region internal_data_structures
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

        private string GetAttributeIfExists(XmlNode ObjNode, string attribute)
        {
            if (ObjNode.Attributes != null && ObjNode.Attributes[attribute] == null)
                return null;

            return ObjNode.Attributes.GetNamedItem(attribute).Value;
        }
        
        void GetFieldEx(XmlNode ObjNode, string fName, out string value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
                throw new BusinessException(null, "MessageQueueProcessor.GetFieldEx", "Field was not found: " + fName);
            }
        }
        void GetFieldEx(XmlNode ObjNode, string fName, out DateTime value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
                throw new BusinessException(null, "MessageQueueProcessor.GetFieldEx", "Field was not found: " + fName);
            }
        }
        void GetFieldEx(XmlNode ObjNode, string fName, out decimal value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
                throw new BusinessException(null, "MessageQueueProcessor.GetFieldEx", "Field was not found: " + fName);
            }
        }
        void GetFieldEx(XmlNode ObjNode, string fName, out int value)
        {
            if (!GetField(ObjNode, fName, out value))
            {
                throw new BusinessException(null, "MessageQueueProcessor.GetFieldEx", "Field was not found: " + fName);
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
                MQDataType node_type = (MQDataType)GetFielProperty(node, MQPropertyField.TIPO);

                if (node_type == MQDataType._STRING_TYPE)
                    value = MQXml.DecodeXML((string)GetFielProperty(node, MQPropertyField.VL));
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
                    int.TryParse(GetFielProperty(node, MQPropertyField.VL).ToString(), out value);
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
                {
                    // acrescentada a conversão para numerico usando o InvariantCulture semelhante à alteração efetuada no envio da queue
                    string vlrStr = DBConversion.ToString(GetFielProperty(node, MQPropertyField.VL));
                    decimal.TryParse(vlrStr,System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value);
                }
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
            string[] formats = { "dd/MM/yyyy", "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss" };
            if (node != null)
            {
                res = true;
                MQDataType node_type = (MQDataType)GetFielProperty(node, MQPropertyField.TIPO);
                if (node_type == MQDataType._DATE_TYPE)//10/11/2014
					DateTime.TryParseExact(GetFielProperty(node, MQPropertyField.VL).ToString(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out value);
                    //DateTime.TryParse(GetFielProperty(node, MQPropertyField.VL).ToString(),out value);//,"DD/MM/YYYY", CultureInfo.InvariantCulture);// ef->m_Value.ODT().Format(_T("%d/%m/%Y")); //estava %m/%d/%Y JP 12/03/2003
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

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTab"></param>
        /// <param name="sqlCheck"></param>
        /// <param name="codRel"></param>
        /// <param name="msgType"></param>
        public void mqImportMQRecord(DbArea pTab, SelectQuery sqlCheck, out string codRel, string msgType)
        {
            pTab.QueueMode = true;
            bool haRegisto = false;
            codRel = "";
            //Verifica se deve procurar o registo com o código interno da table *pTab
            if (sqlCheck != null)
            {
                //Verifica se o registo exists na base de dados
                ArrayList resArr = this.sp.executeReaderOneRow(sqlCheck);
                if (resArr.Count > 0)
                    codRel = resArr[0].ToString();

                haRegisto = (string.IsNullOrEmpty(codRel)) ? false : true;

                //Posiciona o registo encontrado
                if (haRegisto && msgType != "C") pTab.QPrimaryKey = codRel;
            }

            mqImportMQRecordInternal(pTab, ref codRel, msgType, haRegisto);
        }

        private void mqImportMQRecordInternal(DbArea pTab, ref string codRel, string msgType, bool haRegisto)
        {
            //Se a mensagem não é de eliminação de registo
            if (msgType != "D")
            {
                //Edita o registo encontrado
                if (haRegisto && msgType != "C")
                {
                    pTab.update(this.sp);
                }
                //Se ainda não exists, cria um registo novo
                else if (!haRegisto)
                {
                    pTab.insert(this.sp);
                    codRel = pTab.QPrimaryKey;
                }
            }
            //Se a mensagem é do tipo D, apaga o registo encontrado
            else if (msgType == "D" && haRegisto)
            {
                pTab.delete(this.sp);
            }
        }

        /// <summary>
        /// Obtem a lista de todas as mensagens pendentes de uma queue.
        /// Passa as mensagens to o estado de enviadas.
        /// </summary>
        /// <param name="queueName">O name da queue</param>
        /// <returns>A lista de mensagens</returns>
        public List<string> GetAllMessages(string queueName)
        {
            List<string> res = new List<string>();            
            MQueueACK mQueueAck = MQueueACK.SendINPROGRESS;
            DataMatrix pQueues = null;
            SelectQuery selQuery;

            CSGenio.ACK ackOBJ = Configuration.MessageQueueing.ACKS.Find(p => p.ACKqueue.Equals(queueName));
            if (ackOBJ != null)                
            {
                int blockSize = 100;
                if (ackOBJ.BlockSize > 0)
                    blockSize = ackOBJ.BlockSize;

                //Os acknowledgements são sempre enviados em mode de fire and forget
                mQueueAck = MQueueACK.SendReplyOK;               

                selQuery = new SelectQuery()
                    .Select(CSGenioAmqqueues.FldCodmqqueues)                    
                    .Select(CSGenioAmqqueues.FldQueue)
                    .From(Area.AreaMQQUEUES)
                    .Where(CriteriaSet.And()                        
                        .In(CSGenioAmqqueues.FldMQStatus, new string[] {"0", "1", "2", "3", "4", "5", "8"})
                        .Equal(CSGenioAmqqueues.FldChannelID, queueName)
                    )
                    .OrderBy(CSGenioAmqqueues.FldDataStatus,SortOrder.Ascending)
                    .PageSize(blockSize);

				//Estas rows vão ser updated de seguida
                selQuery.updateLock = true;
                pQueues = sp.Execute(selQuery);
            }
            else
            {
                CSGenio.Queue queueOBJ = Configuration.MessageQueueing.Queues.Find(p=>p.channelId.Equals(queueName));
                if(queueOBJ != null)
                {
                    int blockSize = 100;
                    if (queueOBJ.BlockSize > 0)
                        blockSize = queueOBJ.BlockSize;

                    if(queueOBJ.Journal)
                    {
                        int jtimeout = Configuration.MessageQueueing.Journaltimeout;
                        if (jtimeout == 0) jtimeout = 60;

                        int jmaxsendnumber = Configuration.MessageQueueing.Maxsendnumber;
                        if (jmaxsendnumber == 0) jmaxsendnumber = 3;

                        DateTime retryDate = DateTime.Now.AddMinutes(-jtimeout);

                        //A enviar de imediato:
                        //0 - mensagens por enviar

                        //A enviar depois do timeout configurado
                        //1 - mensagens que já foram to o broker mas não receberam resposta
                        //2 - mensagens que já foram entregues mas não receberam resposta
                        //5 - mensagens que voltaram com um erro volátil (que vale a pena voltar a tentar)
                       
                        selQuery = new SelectQuery()
                            .Select(CSGenioAmqqueues.FldCodmqqueues)                            
                            .Select(CSGenioAmqqueues.FldQueue)
                            .From(Area.AreaMQQUEUES)
                            .Where(CriteriaSet.And()
                                .Equal(CSGenioAmqqueues.FldChannelID, queueName)
                                          .SubSet(CriteriaSet.And()
                                                .SubSet(CriteriaSet.Or()
                                                    .SubSet(CriteriaSet.And()
                                                        .Equal(CSGenioAmqqueues.FldMQStatus, ((int)MQueueACK.SendFAIL).ToString()))
                                                    .SubSet(CriteriaSet.And()
                                                        .In(CSGenioAmqqueues.FldMQStatus, new string[] { "1", "2", "5" })
                                                        .LesserOrEqual(CSGenioAmqqueues.FldDataStatus, retryDate)
                                                        .Lesser(CSGenioAmqqueues.FldSendnumber, jmaxsendnumber))))
                            )
                            .OrderBy(CSGenioAmqqueues.FldDataStatus, SortOrder.Ascending)
                            .PageSize(blockSize);
						//Estas rows vão ser updated de seguida
                        selQuery.updateLock = true;
                        pQueues = sp.Execute(selQuery);
                    }
                    else
                    {   
                        //Se não tem journaling isto é fire and forget e marca-se logo como finalizada
                        mQueueAck = MQueueACK.SendReplyOK;
                                         
                        selQuery = new SelectQuery()
                            .Select(CSGenioAmqqueues.FldCodmqqueues)
                            .Select(CSGenioAmqqueues.FldQueue)
                            .From(Area.AreaMQQUEUES)
                            .Where(CriteriaSet.And()
                                .Equal(CSGenioAmqqueues.FldMQStatus, ((int)MQueueACK.SendFAIL).ToString())
                                .Equal(CSGenioAmqqueues.FldChannelID, queueName)
                            )
                            .OrderBy(CSGenioAmqqueues.FldDataStatus, SortOrder.Ascending)
                            .PageSize(blockSize);

						//Estas rows vão ser updated de seguida
                        selQuery.updateLock = true; 
                        pQueues = sp.Execute(selQuery);
                    }
                }                
            }            

            if (pQueues == null)
            {
                Log.Error("GetAllMessages failed to find queue " + queueName);
            }
            else
            {
                string[] codes = new string[pQueues.NumRows];
                for (int i = 0; i < pQueues.NumRows; i++)
                {
				    var payload = pQueues.GetBinary(i, 1);
                    //if the payload is gzipped then decompress it
                    if (payload.Length > 2 && payload[0] == 0x1f && payload[1] == 0x8b)
                    {
                        using (var memory = new MemoryStream(payload))
                        using (var decompress = new System.IO.Compression.GZipStream(memory, System.IO.Compression.CompressionMode.Decompress))
                        using (var result = new MemoryStream(payload.Length))
                        {
                            decompress.CopyTo(result);
                            payload = result.ToArray();
                        }
                    }
					
                    res.Add(System.Text.Encoding.Default.GetString(payload));
                    codes[i] = pQueues.GetKey(i, 0);
                }

                if (pQueues.NumRows > 0)
                {
                    UpdateQuery uQuery = new UpdateQuery()
                        .Update(Area.AreaMQQUEUES)
                        .Set(CSGenioAmqqueues.FldMQStatus, (int)mQueueAck)
                        .Set(CSGenioAmqqueues.FldDataStatus, DateTime.Now)
                        .Set(CSGenioAmqqueues.FldSendnumber, SqlFunctions.Add(new ColumnReference("", "sendnumber"), 1))
                        .Where(CriteriaSet.And()
                            .In(CSGenioAmqqueues.FldCodmqqueues, codes)
                        );
                    sp.Execute(uQuery);
                }
            }
            return res;
        }

        
        public QueueResponse GetQueue(string QueueName, string Operation)
        {
            QueueResponse QueueAckObj = new QueueResponse();
            QueueAckObj.Ack = MQueueACK.ReplyOK;

            try
            {
                CSGenioAmqqueues mqqueue = new CSGenioAmqqueues(user, this.module);
                  SelectQuery selQuery = new SelectQuery()
                                       .Select(CSGenioAmqqueues.FldCodmqqueues)
                                       .From(CSGenioAmqqueues.AreaMQQUEUES)
                                       .Where(CriteriaSet.And()
                                        .Equal(CSGenioAmqqueues.FldMQStatus, (int)MQueueACK.SendFAIL)
                                        .Equal(CSGenioAmqqueues.FldChannelID, QueueName))
                                       .OrderBy(CSGenioAmqqueues.FldDataStatus, SortOrder.Ascending)
                                       .PageSize(1)
                                       .Offset(0);
                
                DataMatrix dx = sp.Execute(selQuery);

                if (dx.NumRows == 0)
                {
                    QueueAckObj.Desc = "Queue empty";
                    QueueAckObj.Ack = MQueueACK.ReplyFAIL;
                }
                else
                {
                    mqqueue = CSGenioAmqqueues.search(sp,dx.GetKey(0,CSGenioAmqqueues.FldCodmqqueues),user);
                    if (Operation == "GET")
                        QueueAckObj.AckQueue = System.Text.Encoding.Default.GetString(mqqueue.ValQueue);
                    else if (Operation == "ACK")
                    {
                        QueueAckObj.MsgId = mqqueue.ValCodmqqueues;
                        //TODO rever o casting                      
                        mqqueue.ValMQStatus = ((int)MQueueACK.SendINPROGRESS).ToString();
                        mqqueue.ValDataStatus = DateTime.Today;
                        //mqqueue.update(sp);
                    }
                    QueueAckObj.Ack = MQueueACK.ReplyOK;
                }
            }
            catch (System.Exception ex)
            {
                QueueAckObj.Desc = ex.Message;
                QueueAckObj.Ack = MQueueACK.ReplyFAIL;
            }
            return QueueAckObj;
        }

        /// <summary>
        /// Obtém a queue de ACK configurada(configuracoes.xml) to queue passada por parâmetro
        /// </summary>
        /// <param name="queue">O name da queue</param>
        /// <returns>ACK queue</returns>
        public string GetACKQueue(string queue)
        {
            if (Configuration.MessageQueueing.ACKS.Count > 0)
            {
                CSGenio.ACK ack = Configuration.MessageQueueing.ACKS.Find(p => p.Source.Equals(queue));
                if (ack != null)
                    return ack.ACKqueue;
            }
            return null;
        }

        public QueueResponse mqACKqueue(MQueueACK mQueueACK, System.DateTime dtorig, System.DateTime dtproc, string description, int progress = 0)
        {
            QueueResponse QueueAckObj = new QueueResponse();
            QueueAckObj.Ack = mQueueACK;
            QueueAckObj.Desc = description;

            if (mQueueACK.Equals(MQueueACK.ReplyPROGRESS))
                QueueAckObj.progress = progress;

            if(queueXml == null)
                return new QueueResponse { Ack = MQueueACK.ReplyIGNORE };

            //TODO: passar a gravar as mensagens na BD e só apagar quando o retorno do envio = OK!
            if (queueXml.ChildNodes.Count > 0)
            {
                string ano_app, table_rec, operation, key, system, queue;

                table_rec = queueXml.DocumentElement.GetAttribute("table");

                XmlNode MainNode = queueXml.DocumentElement;
                ano_app = GetAttribute(MainNode, "year");
                operation = GetAttribute(MainNode, "tp");
                key = GetAttribute(MainNode, "guid");
                queue = GetAttribute(MainNode, "queue");
                system = GetAttribute(MainNode, "sistema");

                XmlDocument xml = new XmlDocument();
                XmlElement xmlMainElem;//nós de 1º level
                //XmlElement xmlNTable;//nós de 2º level - Tabelas 1N e N1
                //XmlElement xmlNTableField;//nós de 3º level - fields das tables 1N e N1

                XmlDeclaration xml_decl;
                xml_decl = xml.CreateXmlDeclaration("1.0", null, null);
                xml_decl.Encoding = "ISO-8859-1";

                XmlElement xml_root = xml.DocumentElement;

                xmlMainElem = xml.CreateElement("", "mqrec", "");
                xml.InsertBefore(xml_decl, xml_root);
                xmlMainElem.SetAttribute("table", table_rec);
                xmlMainElem.SetAttribute("guid", key);
                xmlMainElem.SetAttribute("sistema", system);
                xmlMainElem.SetAttribute("queue", queue);
                
                string queueACK = GetACKQueue(queue);
                //RS 06-07-2017: Se não estiver configurada uma queue de ACK não envia ack
                // No futuro a forma correcta de fazer isto é a propria mensagem vir marcada com "exige ACK" a true/false
                if (queueACK == null)
                    return QueueAckObj;
                queue = queueACK;
                
                xmlMainElem.SetAttribute("tp", operation);
                xmlMainElem.SetAttribute("year", ano_app);
                xmlMainElem.SetAttribute("isackqueue", "true");

                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "key", FieldType.TEXT, key, 6));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "mqstatus", FieldType.TEXT, mQueueACK, 80));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "descr", FieldType.TEXT, description, 80));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "dtorig", FieldType.DATE, dtorig, 8));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "dtproc", FieldType.DATE, dtproc, 8));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "modulo", FieldType.TEXT, module, 3));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "operacao", FieldType.TEXT, operation, 2));
                xml.AppendChild(xmlMainElem);
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, "progress", FieldType.INTEGER, progress, 5));
                xml.AppendChild(xmlMainElem);
                
                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                xml.WriteTo(tx);
           
                CSGenioAmqqueues mqqueue = new CSGenioAmqqueues(user, this.module);
                mqqueue.ValAno =  user.Year;
                mqqueue.ValUsername = user.Name;
                mqqueue.ValTabela = table_rec;
                mqqueue.ValTabelaCod = key;
                mqqueue.ValQueueKey = key;
                mqqueue.ValQueue = System.Text.Encoding.Default.GetBytes(sw.ToString());
                mqqueue.ValQueueID = queue;
				mqqueue.ValChannelID = queue;
                //TODO rever o casting
                mqqueue.ValMQStatus = ((int)QueueAckObj.Ack).ToString();                
                mqqueue.ValDataStatus = DateTime.Now;
                mqqueue.ValDatacria = DateTime.Now;
                mqqueue.ValResposta = (description.Length > 200 ? description.Substring(0, 200) : description);
                mqqueue.ValOperacao = operation;
                mqqueue.insert(sp);

                if (mQueueACK == MQueueACK.ReplyFAIL)
                    Log.Error("MessageQueuing: " + sw.ToString());
            }
            return QueueAckObj;
        }       
        
        /// <summary>
        /// Create uma queue na table de queue. Mas essa que só serve to executar funções atraves do Quidserver utilizando o controlo Botão to queue   
        /// </summary>
        /// <param name="parameterList">Dicionário com a lista de parâmetros a serem introduzidas nos xml</param>
        /// <param name="queue">Name da queue</param>
        /// <param name="user">User</param>
        /// <returns>Sucesso/Insucesso</returns>
        public bool BQCreateQueue(Dictionary<string, string> parameterList,string queue)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                XmlElement xmlMainElem;
                XmlDeclaration xml_decl;
                xml_decl = xml.CreateXmlDeclaration("1.0", null, null);
                xml_decl.Encoding = "ISO-8859-1";
                string id = (parameterList.ContainsKey("id") ? parameterList["id"] : "");
                string queue_guid = Guid.NewGuid().ToString("N");

                XmlElement xml_root = xml.DocumentElement;

                xmlMainElem = xml.CreateElement("", "mqrec", "");
                xml.InsertBefore(xml_decl, xml_root);
                xmlMainElem.SetAttribute("guid", queue_guid);
                xmlMainElem.SetAttribute("sistema", Configuration.Program);
                xmlMainElem.SetAttribute("queue", queue);
                xmlMainElem.SetAttribute("tp", "E");
                xmlMainElem.SetAttribute("year", user.Year);

                foreach (var key in parameterList.Keys)
                {
                    xmlMainElem.AppendChild(MQXml.FieldAdd(xml, key, parameterList[key]));
                    xml.AppendChild(xmlMainElem);
                }

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                xml.WriteTo(tx);

                sp.openTransaction();

                CSGenioAmqqueues mqqueue = new CSGenioAmqqueues(user, user.CurrentModule);
                mqqueue.ValAno = user.Year;
                mqqueue.ValUsername = user.Name;
                mqqueue.ValTabelaCod = id;
                mqqueue.ValQueueKey = queue_guid;
                mqqueue.ValQueue = System.Text.Encoding.Default.GetBytes(sw.ToString());
                mqqueue.ValQueueID = queue;
				mqqueue.ValChannelID = queue;
                mqqueue.ValMQStatus = "0";
                mqqueue.ValDataStatus = DateTime.Now;
                mqqueue.ValDatacria = DateTime.Now;
                mqqueue.ValOperacao = "E";
                mqqueue.insert(sp);
                sp.closeTransaction();

                return true;
            }
            catch (Exception)
            {
                sp.rollbackTransaction();
                return true;                                
            }
        }
        





        /// <summary>
        /// Call back function to catch reindex trace messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="status"></param>
        public void traceReindex(object sender, EventArgs e, ExecuteQueryCore.RdxStatus status)
        {
            string message = string.Format("Creating Schema: {0}", status.ActualScript);
            int percentage = 0;

            if (status.State == ExecuteQueryCore.RdxProgressStatus.SUCCESS)
            {
                message = "Finish reindex";
                percentage = 100;
            }
            else
            {
                percentage = Convert.ToInt32(status.Percentage());
            }
            mqACKqueue(MQueueACK.ReplyPROGRESS, DateTime.Now, DateTime.Now, message, percentage);
        }

        /// <summary>
        /// Reindex database, and allow callback ro return progress message
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="orderExec">Ordered execution list</param>
        /// <param name="zero">if true, reindex with /zero</param>
        /// <param name="changedExecutionScript">Callback funtion for progress tracing</param>
        public void TracedReindex(string year, string username, string password, List<ExecuteQueryCore.RdxScript> orderExec, bool zero, ExecuteQueryCore.ChangedEventHandler changedExecutionScript = null)
        {
            ExecuteQueryCore.RdxParamUpgradeSchema param = new ExecuteQueryCore.RdxParamUpgradeSchema();
            param.Year = year;
            param.Username = username;
            param.Password = password;
            param.OrderExec = orderExec;
            param.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", Configuration.Program + "_ReIdx\\Reindex");            
            param.Zero = zero;
            param.Origin = "External";
            param.ChangedExecutionScript += changedExecutionScript;

            PersistentSupport.upgradeSchema(param);
        }
    }
    
}
