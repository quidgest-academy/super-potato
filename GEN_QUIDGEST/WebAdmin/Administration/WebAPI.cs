using System.Text;
using System.Xml.Linq;
using Administration.Models;
using GenioServer.business;
using CSGenio;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using ExecuteQueryCore;
using DbAdmin;
using CSGenio.core.persistence;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration
{
    public sealed class ChannelArgs
    {
        public static readonly string OPERATION = "OPERATION";
        public static readonly string GET = "GET";
        public static readonly string ACK = "ACK";
        public static readonly string USERNAME = "USERNAME";
        public static readonly string PASSWORD = "PASSWORD";
        public static readonly string YEARAPP = "YEARAPP";
        public static readonly string QUEUENAME = "QUEUENAME";
        public static readonly string QUEUEDATA = "QUEUEDATA";
        public static readonly string QUEUEID = "QUEUEID";
        public static readonly string FUNCTION = "FUNCTION";
        public static readonly string MSG = "MSG";
        public static readonly string SCRIPTS = "SCRIPTS";
        public static readonly string LANGUAGE = "LANGUAGE";
        public static readonly string ZEROTRUE = "ZEROTRUE";
		public static readonly string FILESTREAM = "FILESTREAM";
        public static readonly string NOCONNECTION = "NOCONNECTION";
    }

    public class WebAPI(IConfigurationManager configManager) : IAdminService
    {

        private readonly string DEFAULTLANGUAGE = "en-US";

        public QApiCallAck QApi(List<Administration.AuxClass.KeyValuePair<string, string>> args)
        {
            QApiCallAck response = new QApiCallAck();
            QueueResponse qResponse = new QueueResponse();

            string username = Encoding.Unicode.GetString(Convert.FromBase64String(args.Find(x => x.Key == ChannelArgs.USERNAME).Value));//TODO: cifrar
            args.Remove(args.Find(x => x.Key == ChannelArgs.USERNAME));
            args.Add(new Administration.AuxClass.KeyValuePair<string, string>() { Key = ChannelArgs.USERNAME, Value = username });

            string password = Encoding.Unicode.GetString(Convert.FromBase64String(args.Find(x => x.Key == ChannelArgs.PASSWORD).Value));//TODO: cifrar
            args.Remove(args.Find(x => x.Key == ChannelArgs.PASSWORD));
            args.Add(new Administration.AuxClass.KeyValuePair<string, string>() { Key = ChannelArgs.PASSWORD, Value = password });

            string function = args.Find(x => x.Key == ChannelArgs.FUNCTION).Value;
            args.Remove(args.Find(x => x.Key == ChannelArgs.FUNCTION));

            string language = args.Find(x => x.Key == ChannelArgs.LANGUAGE).Value;
            args.Remove(args.Find(x => x.Key == ChannelArgs.LANGUAGE));

            string yearApp = args.Find(x => x.Key == ChannelArgs.YEARAPP).Value;

            bool noConnection = false;
            if (args.Exists(x => x.Key == ChannelArgs.NOCONNECTION))
            {
                _ = bool.TryParse(
                    args.Find(x => x.Key == ChannelArgs.NOCONNECTION).Value,
                    out noConnection
                );
            }

            CSGenio.persistence.PersistentSupport sp = null;
            CSGenio.framework.User user = null;

            try
            {
                user = SysConfiguration.CreateWebAdminUser(yearApp);
                user.Language = string.IsNullOrEmpty(language)
                    ? DEFAULTLANGUAGE.Replace("-", "").ToUpper()
                    : language;

                Log.SetContext("utilizador", "WebAPI_QApi");

                if (!noConnection)
                {
                    sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
                    sp.openTransaction();
                }

                SchedulerCallFunctions fcaller = new SchedulerCallFunctions(user, user.CurrentModule, sp);
                qResponse = fcaller.CallFunction(
                    function,
                    args.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value)
                );
                
                if (!noConnection && sp != null)
                    sp.closeTransaction();

				//Return the same value as the inner function
                if(qResponse != null)
                {
                    response.Desc = qResponse.Desc;
                    response.Ack = ConvertAck(qResponse.Ack);
                }
            }
            catch (Exception ex)
            {
                response.Desc = ex.Message;
                response.Ack = CallAck.Failed;

                if (!noConnection && sp != null)
                    sp.rollbackTransaction();

                Log.Error($"Error handling WebApi call: {ex.Message}");
                return response;
            }
            
            return response;
        }

        /// <summary>
        /// Converts an MQueueAck object to CallAck
        /// </summary>
        private CallAck ConvertAck(MQueueACK queueACK)
        {
            switch (queueACK)
            {
                case MQueueACK.ReplyOK:
                case MQueueACK.SendReplyOK:
                    return CallAck.OK;
                case MQueueACK.ReplyFAIL:
                    return CallAck.Failed;
                case MQueueACK.ReplyREJECT:
                    return CallAck.Rejected;
                default:
                    return CallAck.Failed;
            }
        }

        public void ProcessMessage(string queueName, string year, string message)
        {
            var user = new CSGenio.framework.User("msmq", "", year);
            user.CurrentModule = "MQQ";//Must be generated.
            user.AddModuleRole("MQQ", Role.ADMINISTRATION);
            user.Language = DEFAULTLANGUAGE.Replace("-","").ToUpper();
            PersistentSupport sp = null;
            GenioServer.business.MessageQueueProcessor mqproc = null;
            Log.SetContext("utilizador", "WebAPI_ProcessMessage");

            try
            {
                sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
                sp.QueueMode = true;                
                mqproc = new GenioServer.business.MessageQueueProcessor(user, user.CurrentModule, sp, message);

                IsAckResponse isACKTest = mqproc.IsACKQueue();
                if (isACKTest.IsACK)
                {
                    if(GenFunctions.emptyG(isACKTest.QueueKey)==1)
                        throw new BusinessException("Não existe a queue original da ACK - " + queueName, "WebAPI.ProcessMessage", "Não existe a queue original da ACK - " + queueName);

					sp.openTransaction();
                    mqproc.ProcessACKQueue(queueName, isACKTest.QueueKey);
                    sp.closeTransaction();
                }
                else
                {
                    if (!mqproc.QueueExists(queueName))
                        throw new BusinessException("Não existe a rotina de processmento para a queue " + queueName, "WebAPI.ProcessMessage", "Não existe a rotina de processmento para a queue " + queueName);

                    try
                    {
						sp.openTransaction();
                        var response = mqproc.ProcessQueue(queueName);

                        if( response != null && response.Ack == MQueueACK.ReplyIGNORE)
                        {
                            //Processes that respond with Ignore do not wish to send ACK
                            sp.closeTransaction();
                            return;
                        }
                        if (response != null && response.Ack != MQueueACK.ReplyOK)
                        {
                            sp.rollbackTransaction();
                            sp.openTransaction();
                            mqproc.mqACKqueue(response.Ack, DateTime.Now, DateTime.Now, response.Desc);
                            sp.closeTransaction();
                            
                            Log.Error(queueName + " " + response.Desc);
                            return;
                        }
						else
						{
							sp.closeTransaction();
							sp.openTransaction();
							mqproc.mqACKqueue(MQueueACK.ReplyOK, DateTime.Now, DateTime.Now, response != null ? response.Desc : "");
                            sp.closeTransaction();
						}
                    }
                    catch (Exception exMQ)
                    {
                        try
                        {
                            sp.rollbackTransaction();
                        }
                        catch (Exception ex2)
                        {
                            //Nothing we can do about a failed rollback, we just log it
                            Log.Error(queueName + " " + ex2.Message);
                        }
                        try
                        {
                            //But we do need to report the processing failure to the ACK system
                            sp.openTransaction();
                            if (mqproc != null)
                            {
                                mqproc = new GenioServer.business.MessageQueueProcessor(user, user.CurrentModule, sp, message);
                                mqproc.mqACKqueue(MQueueACK.ReplyFAIL, DateTime.Now, DateTime.Now, exMQ.Message);
                            }
                            sp.closeTransaction();
                        }
                        catch (Exception ex2)
                        {
                            //If we cannot even record the ACK failure then we can only log in the filesystem                            
                            Log.Error(queueName + " " + ex2.Message);
                            sp.rollbackTransaction();
                        }

                    }
                }              
                
            }
            catch (Exception ex)
            {                
                Log.Error(queueName + " " + ex.Message);

                if (sp != null)            
                    sp.rollbackTransaction();
            }
        }
        
        public List<GlobalFunctions.FunctionInformation> GetAllSchedulerFuncs()
        {
            return GlobalFunctions.GetSchedulerFuncs();
        }

        public string[] GetAllMessages(string queueName, string year)
        {
            CSGenio.persistence.PersistentSupport sp = null;
            CSGenio.framework.User user = null;
			
            try
            {
                Log.SetContext("utilizador", "WebAPI_GetAllMessages");
                user = new CSGenio.framework.User("msmq", "", year);
                user.CurrentModule = "MQQ";
                user.AddModuleRole("MQQ", Role.ADMINISTRATION);
                user.Language = DEFAULTLANGUAGE.Replace("-","").ToUpper();
                sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
                sp.openTransaction();
                GenioServer.business.MessageQueueProcessor mqproc = new GenioServer.business.MessageQueueProcessor(user, user.CurrentModule, sp, "");
                var res = mqproc.GetAllMessages(queueName);
                sp.closeTransaction();

                return res.ToArray();
            }
            catch(Exception ex)
            {
                Log.Error(queueName + " " + ex.Message);
                sp.rollbackTransaction();
            }
            return new string[0];
        }


        public string GetOneMessage(string queueName, string year)
        {
            CSGenio.persistence.PersistentSupport sp = null;
            CSGenio.framework.User user = null;

            try
            {
                user = new CSGenio.framework.User("msmq", "", year);
                user.CurrentModule = "MQQ";
                user.AddModuleRole("MQQ", Role.ADMINISTRATION);
                user.Language = DEFAULTLANGUAGE.Replace("-","").ToUpper();
                sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
                sp.openTransaction();
                GenioServer.business.MessageQueueProcessor mqproc = new GenioServer.business.MessageQueueProcessor(user, user.CurrentModule, sp, "");
                var res = mqproc.GetQueue(queueName, "GET");
                sp.closeTransaction();

                return res.AckQueue;
            }
            catch (Exception ex)
            {
                Log.SetContext("utilizador", "WebAPI_GetAllMessages");
                Log.Error(queueName + " " + ex.Message);

                sp.rollbackTransaction();
            }
            return string.Empty;
        }

        public QApiCallAck WebAdminApi(List<Administration.AuxClass.KeyValuePair<string, string>> args)
        {
            QApiCallAck response = new QApiCallAck();

            string username, password, yearApp, strScripts;

            username   = Encoding.Unicode.GetString(Convert.FromBase64String(args.Find(x => x.Key == ChannelArgs.USERNAME).Value));//TODO: cifrar//System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(
            password   = Encoding.Unicode.GetString(Convert.FromBase64String(args.Find(x => x.Key == ChannelArgs.PASSWORD).Value));//TODO: cifrar//System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(
            yearApp    = args.Find(x => x.Key == ChannelArgs.YEARAPP).Value;
            strScripts = args.Find(x => x.Key == ChannelArgs.SCRIPTS).Value;

            string[] scripts = strScripts.Split(';');
			
			string filestreamDir = "";
            if (scripts.Contains("CREATEDB"))
                filestreamDir = args.Find(x => x.Key == ChannelArgs.FILESTREAM).Value;


            Models.DbAdminModel model = new Models.DbAdminModel();
            model.DbUser = username;
            model.DbPsw  = password;
            var conf = configManager.GetExistingConfig();

            if (conf.DataSystems.Count == 0)
            {
                response.Ack = CallAck.Failed;
                response.Desc = "Invalid configuration file, please run the System Configuration first.";
                return response;
            }
           
            //Get order2execPath
            var dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
            string reindexScriptsPath = dbMaintenance.GetReindexPath();
            string order2execPath = System.IO.Path.Combine(reindexScriptsPath, "order2Exec.xml");
            
            try
            {
                model.reindexMenu = dbMaintenance.GetReindexScripts(order2execPath);
                model.Items = new List<ReindexFunctionItem>();

                //select scripts to be executed
                foreach (ReIndexFunction item in model.reindexMenu.ReIndexItems)
                    item.Selected = false;
                foreach (string script in scripts)
                {
                    model.reindexMenu.ReIndexItems.Find(delegate (ReIndexFunction RdxFunction) { return RdxFunction.Id == script; }).Selected = true;
                }
                model.DirFilestream = filestreamDir;


                model.reindexMenu.CalculateOrder();
                List<RdxScript> order2exec = model.reindexMenu.GetOrderToExecute();
                
                User user = SysConfiguration.CreateWebAdminUser(yearApp);
                GlobalFunctions gblFunctions = new GlobalFunctions(user, user.CurrentModule);
                PersistentSupport sp = PersistentSupport.getPersistentSupport(yearApp);
                DatabaseVersionReader versionReader = new DatabaseVersionReader(sp);
                var hidratedScripts = gblFunctions.HidrateScripts(order2exec, versionReader);
                PersistentSupport.upgradeSchema(yearApp, model.DbUser, model.DbPsw, hidratedScripts, reindexScriptsPath, model.DirFilestream, true); // zero deve ser escolhido pelo user       
            }
            catch (BusinessException ex)
            {
                response.Ack = CallAck.Failed;
                response.Desc = ex.Message;
                return response;
            }
            catch (PersistenceException ex)
            {
                response.Ack = CallAck.Failed;
                response.Desc = ex.Message;
                return response;
            }

            response.Ack = CallAck.OK;
            return response;
        }

        public QApiCallAck Maintenance(List<Administration.AuxClass.KeyValuePair<string, string>> args)
        {
            QApiCallAck response = new QApiCallAck();

            string username, password, op, msg;

            username = Encoding.Unicode.GetString(Convert.FromBase64String(args.Find(x => x.Key == ChannelArgs.USERNAME).Value));//TODO: cifrar//System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(
            password = Encoding.Unicode.GetString(Convert.FromBase64String(args.Find(x => x.Key == ChannelArgs.PASSWORD).Value));//TODO: cifrar//System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(
            op = args.Find(x => x.Key == ChannelArgs.OPERATION).Value;
            msg = args.Find(x => x.Key == ChannelArgs.MSG).Value;

            string pathConfig = Configuration.GetConfigPath();
            System.IO.File.WriteAllText(System.IO.Path.Combine(pathConfig, "App_Offline.htm"), msg);

            response.Ack = CallAck.OK;
            return response;
        }
    }
}