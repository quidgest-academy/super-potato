using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioServer.business;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using DbAdmin;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration.Controllers
{
    public class MessageQueueController(IConfigurationManager configManager) : ControllerBase
    {
        private static Dictionary<string, QueueProgressStatus> exportProgressList = new Dictionary<string, QueueProgressStatus>();
        //
        // GET: /MessageQueue/
        [HttpGet]
        public IActionResult Index()
        {
            var model = new Models.MessageQueueModel();            

            try
            {
                var conf = configManager.GetExistingConfig();

                model.MQueues = new Models.MessageQueue();
                model.MQueues.Queues = new List<Models.QueueCfg>();
                model.MQueues.Acks = new List<Models.QueueACK>();

                int rownum = 0;
                if (conf.MessageQueueing != null)
                {
                    List<QueueGenio> qList = CSGenio.business.Area.GetAllQueues();
                    foreach (var q in conf.MessageQueueing.Queues)
                    {
                        QueueGenio existQueue = qList.Find(x => x.Name.Equals(q.queue, StringComparison.OrdinalIgnoreCase));
                        if (existQueue != null)
                            model.MQueues.Queues.Add(new Administration.Models.QueueCfg(q) { Rownum = rownum++ });
                    }

                    foreach (var ack in conf.MessageQueueing.ACKS)
                    {
                        model.MQueues.Acks.Add(new Administration.Models.QueueACK(ack) { Rownum = rownum++ });
                    }
                }
                else
                {
                    conf.MessageQueueing = new messagequeueing();
                    conf.MessageQueueing.Queues = new List<CSGenio.Queue>();
                    conf.MessageQueueing.ACKS = new List<ACK>();
                }
				
				// Check if log database exists
                var dataSystem = CSGenio.framework.Configuration.ResolveDataSystem(CSGenio.framework.Configuration.DefaultYear, CSGenio.framework.Configuration.DbTypes.NORMAL); // Default == null
                model.LogDatabaseExists = dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count != 0 ? true : false;
            }
            catch (Exception e)
            {
                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
            }

            return Json(model);
        }

        public IActionResult TestExportQueue(string queue, string year, string conditionField, string conditionOp, string conditionValue)
        {
            int count = 0;
            IEnumerable<string> fields = new List<string>();

            //We count the number of rows that will potentially be exported by this operation
            try
            {
                CSGenio.persistence.PersistentSupport sp = null;
                User user = null;
                QueueGenio queueArea = Area.GetAllQueues().Find(x => x.Name.Equals(queue));
                var info = Area.GetInfoArea(queueArea.Queuearea);

                fields = info.DBFieldsList.Select(x => x.Name);

                user = SysConfiguration.CreateWebAdminUser(year, module: "MQQ");
                sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(user.Year, user.Name);
                sp.openConnection();

                // zzstate needs to be added to simulate the export process
                var condition = CriteriaSet.And().Equal(info.Alias, "zzstate", 0);
                // if its filled out add the condition the user put in the interface
                if (!string.IsNullOrEmpty(conditionField) && !string.IsNullOrEmpty(conditionValue))
                    condition.Criterias.Add(BuildQueueCriteria(conditionField, conditionOp, conditionValue, info));

                SelectQuery s = new SelectQuery()
                    .From(info.TableName, info.Alias)
                    .Where(condition);
                s = QueryUtils.buildQueryCount(s);

                count = (int)DBConversion.ToNumeric(sp.ExecuteScalar(s));

                sp.closeConnection();
            }
            catch (Exception)
            {
                //For now just ignore the errors
            }

            return Json(new { count, fields });
        }

        private static Criteria BuildQueueCriteria(string conditionField, string conditionOp, string conditionValue, AreaInfo info)
        {
            if (conditionOp == "IN")
            {
                return new Criteria(
                    new ColumnReference(info.Alias, conditionField),
                    CriteriaOperator.In,
                    conditionValue.Split(';')
                    );
            }
            else
            {
                return new Criteria(
                    new ColumnReference(info.Alias, conditionField),
                    QueryUtils.ParseEphOperator(conditionOp),
                    conditionValue
                );
            }
        }
		
        [HttpGet]
        public IActionResult exportDataToHistory()
        {
            CSGenio.persistence.PersistentSupport logSp = CSGenio.persistence.PersistentSupport.getPersistentSupportLog(CSGenio.framework.Configuration.DefaultYear, "");
            logSp.transferMSMQLog(true);

            return Ok();
        }

        private bool UpdateProgressBar(QueueProgressStatus status)
        {
            QueueProgressStatus expQProgress = new()
            { 
                id = status.id,
                Message = status.Message
            };

            if (status.Completed)
            {
                expQProgress.Count = 100;
                expQProgress.Completed = true;
            }
            else
            {
                if (status.Total != 0)
                    expQProgress.Count = status.Count * 100 / status.Total;
            }

            lock (exportProgressList)
            {
                if (exportProgressList.ContainsKey(status.id))
                    exportProgressList[status.id] = expQProgress;
                else
                    exportProgressList.Add(status.id, expQProgress);
            }
            return true;
        }

        public IActionResult ExportQueues(string queue, string year, string conditionField, string conditionOp, string conditionValue, string id)
        {
            QueueProgressStatus exportQueueProgress = new QueueProgressStatus { id = id };
            CSGenio.persistence.PersistentSupport sp = null;
            User user = null;

            try
            {
                user = SysConfiguration.CreateWebAdminUser(year, module: "MQQ");
                sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(user.Year, user.Name);

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        QueueGenio queueArea = Area.GetAllQueues().Find(x => x.Name.Equals(queue));
                        var info = Area.GetInfoArea(queueArea.Queuearea);

                        // if its filled out add the condition the user put in the interface
                        CriteriaSet condition = null;
                        if (!string.IsNullOrEmpty(conditionField) && !string.IsNullOrEmpty(conditionValue))
                        {
                            condition = CriteriaSet.And();
                            condition.Criterias.Add(BuildQueueCriteria(conditionField, conditionOp, conditionValue, info));
                        }

                        // start the export process
                        sp.openConnection();
                        MessageQueueProcessor mqproc = new MessageQueueProcessor(user, user.CurrentModule, sp, "");
                        mqproc.ExportAllQueues(queue, UpdateProgressBar, id, condition);
                        sp.closeConnection();
                    }
                    catch (GenioException e)
                    {
                        exportQueueProgress.Message = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + ": " + e.Message;
                    }
                    catch (Exception e)
                    {
                        exportQueueProgress.Message = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                    }
                    exportQueueProgress.Completed = true;
                    UpdateProgressBar(exportQueueProgress);
                });
            }
            catch (GenioException e)
            {
                exportQueueProgress.Message = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + ": " + e.Message;
                exportQueueProgress.Completed = true;
            }
            catch (Exception e)
            {
                exportQueueProgress.Message = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                exportQueueProgress.Completed = true;
            }
            UpdateProgressBar(exportQueueProgress);
            return Json(exportQueueProgress);
        }

        public IActionResult Progress(string id)
        {
            QueueProgressStatus expQueue = null;

            if (exportProgressList.ContainsKey(id))
            {
                expQueue = exportProgressList[id];
            }

            var Result = new
            {
                Count = expQueue != null ? expQueue.Count : 100,
                Message = expQueue != null ? expQueue.Message : "",
                Completed = expQueue != null ? expQueue.Completed : false
            };
            return Json(Result);
        }


        private string ConvertMQstatusString(string val)
        {
            int Qvalue = GenFunctions.atoi(val);
            return ((MQueueACK)Qvalue).ToString();
        }

        [HttpPost]
        public IActionResult QueueProcessActions(string action, string key)
        {
            try
            {
                if (!String.IsNullOrEmpty(action) && (action.Equals("SEND") || action.Equals("END")))
                {
                    try
                    {
                        string status = "0";
                        if (action.Equals("END"))
                            status = "3";

                        UpdateQuery qUpdate = new UpdateQuery()
                            .Update(Area.AreaMQQUEUES)
                            .Set(CSGenioAmqqueues.FldMQStatus, status)
                            .Set(CSGenioAmqqueues.FldDataStatus, DateTime.Now)
                            .Where(CriteriaSet.And().Equal(CSGenioAmqqueues.FldCodmqqueues, key));

                        var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
                        sp.openConnection();
                        sp.Execute(qUpdate);
                        sp.closeConnection();

                        return Json(new { status = "OK", msg = Resources.Resources.A_OPERACAO_FOI_CONCL36721 });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = "E", msg = ex.Message });
                        throw;
                    }

                }
                else
                    return Json(new { status = "E", msg = Resources.Resources.NAO_FOI_POSSIVEL_CON55944 });
            }
            catch (Exception ex)
            {
                return Json(new { status = "E", msg = ex.Message });
            }           
        }

        public struct QueueProcessStatsParams
        {
            public string queue { get; set; }
            public DateTime? dataINI { get; set; }
            public DateTime? dataFIM { get; set; }
            public List<string> acks { get; set; }
        }

        [HttpPost]
        public IActionResult QueueProcessStats([FromBody] QueueProcessStatsParams data)
        {
            var qStat = new Models.QueueStatsModel();
            qStat.StatLines = new List<Models.ItemQueueStats>();
            qStat.ErroStatLines = new List<Models.ItemQueueErrorStats>();

            string queueTable = Area.AreaMQQUEUES.Table + " with (nolock) ";
            string table = Area.AreaMQQUEUES.Table;
            string wherecondition = "";
            string whereconditionExtra = "";
            string whereDatacondition = "";
            string whereDataconditionExtra = "";
            string notInCondition = "";
            List<IDbDataParameter> prmList = new List<IDbDataParameter>();
            List<IDbDataParameter> prmList2 = new List<IDbDataParameter>();
            var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);

            try
            {
                if (!String.IsNullOrEmpty(data.queue))
                {
                    wherecondition = "Where t.queueid = '@queue'";
                    whereconditionExtra = " and queueid = '@queue2'";
                    prmList.Add(sp.CreateParameter("queue", data.queue));
                    prmList2.Add(sp.CreateParameter("queue2", data.queue));
                }                               

                if (data.dataINI.HasValue && data.dataFIM.HasValue &&
                    data.dataINI != DateTime.MinValue && data.dataFIM != DateTime.MinValue)
                {
                    whereDatacondition = "and datastatus BETWEEN @dataini and @datafim ";
                    whereDataconditionExtra = "and datastatus BETWEEN @dataini2 and @datafim2 ";
                    prmList.Add(sp.CreateParameter("dataini", data.dataINI.Value));
                    prmList.Add(sp.CreateParameter("datafim", data.dataFIM.Value));
                    prmList2.Add(sp.CreateParameter("dataini2", data.dataINI.Value));
                    prmList2.Add(sp.CreateParameter("datafim2", data.dataFIM.Value));
                }

                if(data.acks != null)
                {                 
                    if(String.IsNullOrEmpty(wherecondition))
                        wherecondition = "where t.queueid not in('@acks')" ;                    
                    else
                        wherecondition += " and t.queueid not in('@acks')";

                    prmList.Add(sp.CreateParameter("acks", data.acks.Distinct().Aggregate((a, b) => a + "' ,'" + b) ));
                }
                
                string sql = "";
                sql = String.Format("with totais AS (select queueid, count(*) as quantidade from {0} where mqstatus <> '0' {2} group by queueid), " +
                    "TotaisErros AS(select queueid, count(*) as quantidadeErro from {0} where mqstatus in('4', '5') {2} group by queueid), " +
                    "TotaisSucesso AS(select queueid, count(*) as quantidadeSucesso from {0} where mqstatus = '3' {2} group by queueid), " +
                    "TotaisPorEnviar AS (select queueid, count(*) as quantidadePorEnviar from {0} where mqstatus ='0' {2} group by queueid)" +
                    " select t.queueid, t.quantidade, te.quantidadeErro, ts.quantidadeSucesso, tv.quantidadePorEnviar from totais t left join TotaisErros te on t.queueid = te.queueid " +
                    "left join TotaisSucesso ts on t.queueid = ts.queueid left join TotaisPorEnviar tv on t.queueid = tv.queueid {1} {3}", queueTable, wherecondition, whereDatacondition, notInCondition);
                                
                sp.openConnection();
                
                DataMatrix dataStat = sp.executeQuery(sql, prmList);
                if (dataStat.NumRows > 0)
                {
                    for (int i = 0; i < dataStat.NumRows; i++)
                    {
                        qStat.StatLines.Add(new Models.ItemQueueStats { 
                            QueueId = dataStat.GetString(i, 0), 
                            Total = dataStat.GetInteger(i, 1), 
                            Errors = dataStat.GetInteger(i, 2), 
                            Sended = dataStat.GetInteger(i, 3), 
                            ToSend = dataStat.GetInteger(i, 4) 
                        });
                    }
                }
                
                sql = String.Format("with Erros as (select queueid, mqstatus, soundex(resposta) as sresposta, COUNT(*) as qtd from  {0} with(nolock) where (mqstatus in (4,5,6) or (mqstatus = 1 and resposta <> '') and dbo.emptyC(resposta)=0) {1} {2} group by queueid,mqstatus, soundex(resposta))," +
                    "          ErrosFinal as (select queueid, mqstatus, qtd, sresposta, (select top 1 resposta from {0} with(nolock) where soundex(resposta) = q.sresposta and queueid = q.queueid and mqstatus = q.mqstatus) erro from Erros q)" +
                    "		 select queueid, mqstatus, erro, qtd from ErrosFinal where dbo.emptyC(erro) = 0 or(dbo.emptyC(erro) = 1 and mqstatus <> 6)", table, whereconditionExtra, whereDataconditionExtra);
                
                dataStat = sp.executeQuery(sql, prmList2);
                sp.closeConnection();                

                if (dataStat.NumRows > 0)
                {
                    for (int i = 0; i < dataStat.NumRows; i++)
                    {
                        qStat.ErroStatLines.Add(new Models.ItemQueueErrorStats { 
                            QueueId = dataStat.GetString(i, 0), 
                            mqstatus = dataStat.GetString(i, 1), 
                            Errors = dataStat.GetString(i, 2), 
                            Total = dataStat.GetInteger(i, 3) 
                        });
                    }
                }

                return Json(qStat);
            }
            catch (Exception)
            {
                return Json(new { }); // TODO: Return error !
            }
        }


        [HttpGet]
        public IActionResult GetQueueHistory()
        {
            try
            {
                string search = FromQuery("global_search");
                string order = FromQuery("sort[0].name");
                string orderDir = FromQuery("sort[0].order");
                int page = Convert.ToInt32(FromQuery("page"));
                int pageSize = Convert.ToInt32(FromQuery("per_page"));
                string queue = FromQuery("queue");

                List<FieldRef> orderBy = new List<FieldRef>() { CSGenioAmqqueues.FldQueueID, CSGenioAmqqueues.FldTabela, CSGenioAmqqueues.FldTabelaCod
                                                            ,CSGenioAmqqueues.FldMQStatus, CSGenioAmqqueues.FldDataStatus, CSGenioAmqqueues.FldResposta, CSGenioAmqqueues.FldSendnumber
                                                            ,CSGenioAmqqueues.FldDatacria};

                Dictionary<string, string> mqStatusDesc = new Dictionary<string, string>();
                int indiceOrder = 0;

                if (!String.IsNullOrEmpty(order))
                    indiceOrder = GenFunctions.atoi(order);

                SortOrder sortOrder = SortOrder.Ascending;
                if (orderDir == "desc")
                    sortOrder = SortOrder.Descending;

                DateTime retryDate = DateTime.Now.AddDays(-1);

                CriteriaSet selWhere = CriteriaSet.And();
                selWhere.SubSet(
                CriteriaSet.And()
                    .SubSet(CriteriaSet.Or()
                        .SubSet(CriteriaSet.And()
                            .In(CSGenioAmqqueues.FldMQStatus, new string[] { "3", "6" }))
                        .SubSet(CriteriaSet.And()
                            .GreaterOrEqual(CSGenioAmqqueues.FldSendnumber, 3))
                    )
                    .SubSet(CriteriaSet.And()
                        .LesserOrEqual(CSGenioAmqqueues.FldDataStatus, retryDate))
                    )
                    .SubSet(CriteriaSet.And()
                        .Equal(CSGenioAmqqueues.FldZzstate, 0)
                    );

                if (!String.IsNullOrEmpty(queue))
                    selWhere.SubSet(CriteriaSet.And().Equal(CSGenioAmqqueues.FldQueueID, queue));

                if (!String.IsNullOrEmpty(search))
                {
                    string searchValue = "%" + search + "%";
                    selWhere.SubSet(
                        CriteriaSet.Or()
                            .Like("mqqueues", "queueid", searchValue)
                            .Like("mqqueues", "resposta", searchValue)
                            .Like("mqqueues", "tabela", searchValue)
                            .Like("mqqueues", "tabelacod", searchValue)
                            .Like("mqqueues", "sendnumber", searchValue)
                        );
                }

                SelectQuery selQuery = new SelectQuery()
                    .Select(CSGenioAmqqueues.FldCodmqqueues)
                    .Select(CSGenioAmqqueues.FldSendnumber)
                    .Select(CSGenioAmqqueues.FldQueueID)
                    .Select(CSGenioAmqqueues.FldQueueKey)
                    .Select(CSGenioAmqqueues.FldTabela)
                    .Select(CSGenioAmqqueues.FldMQStatus)
                    .Select(CSGenioAmqqueues.FldDataStatus)
                    .Select(CSGenioAmqqueues.FldResposta)
                    .Select(CSGenioAmqqueues.FldDatacria)
                    .Select(CSGenioAmqqueues.FldTabelaCod)
                    .From(Area.AreaMQQUEUES)
                    .Where(selWhere)
                    .PageSize(pageSize)
                    .Page(page)
                    .OrderBy(orderBy[indiceOrder], sortOrder);
                selQuery.noLock = true;

                List<object> dataResult = new List<object>();

                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
                sp.openConnection();
                DataMatrix dataSet = sp.Execute(selQuery);
                int total = DBConversion.ToInteger(sp.ExecuteScalar(QueryUtils.buildQueryCount(selQuery)));
                sp.closeConnection();

                List<QueueGenio> qList = CSGenio.business.Area.GetAllQueues();

                for (int lin = 0; lin < dataSet.NumRows; lin++)
                {
                    dataResult.Add(new List<string> {
                    dataSet.GetKey(lin, 2)
                    , dataSet.GetString(lin, 4)
                    , dataSet.GetKey(lin, 9)
                    , ConvertMQstatusString(dataSet.GetString(lin, 5))
                    , dataSet.GetDate(lin, 6).ToString()
                    , dataSet.GetString(lin, 7)
                    , dataSet.GetNumeric(lin, 1).ToString()
                    , dataSet.GetDate(lin, 8).ToString()
                    , dataSet.GetKey(lin, 0)
                    , (FindQueue(qList, dataSet.GetKey(lin, 2)) ? "QUEUE" : "ACK")
                });
                }

                return Json(new { Success = true, recordsTotal = total, data = dataResult });
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Server Error" });
            }
        }
        
        [HttpGet]
        public IActionResult GetQueueMSG()
        {
            try
            {
                string search = FromQuery("global_search");
                string order = FromQuery("sort[0].name");
                string orderDir = FromQuery("sort[0].order");
                int page = Convert.ToInt32(FromQuery("page"));
                int pageSize = Convert.ToInt32(FromQuery("per_page"));
                string status = FromQuery("status");
                string queue = FromQuery("queue");


                List<FieldRef> orderBy = new List<FieldRef>() { CSGenioAmqqueues.FldQueueID, CSGenioAmqqueues.FldQueueID, CSGenioAmqqueues.FldTabela, CSGenioAmqqueues.FldTabelaCod
                                                            ,CSGenioAmqqueues.FldMQStatus, CSGenioAmqqueues.FldDataStatus, CSGenioAmqqueues.FldResposta, CSGenioAmqqueues.FldSendnumber
                                                            ,CSGenioAmqqueues.FldDatacria};
                Dictionary<string, string> mqStatusDesc = new Dictionary<string, string>();
                int indiceOrder = 0;

                if (!String.IsNullOrEmpty(order))
                    indiceOrder = GenFunctions.atoi(order);

                SortOrder sortOrder = SortOrder.Ascending;
                if (orderDir == "desc")
                    sortOrder = SortOrder.Descending;

                CriteriaSet selWhere = CriteriaSet.And();
                selWhere.SubSet(CriteriaSet.And().Equal(CSGenioAmqqueues.FldZzstate, 0));

                if (!String.IsNullOrEmpty(status))
                {
                    selWhere.SubSet(CriteriaSet.And().Equal(CSGenioAmqqueues.FldMQStatus, status));
                }
                else
                {
                    selWhere.SubSet(CriteriaSet.And().In(CSGenioAmqqueues.FldMQStatus, new string[] { "0", "1", "2", "5" }));
                }

                if (!String.IsNullOrEmpty(queue))
                    selWhere.SubSet(CriteriaSet.And().Equal(CSGenioAmqqueues.FldQueueID, queue));

                if (!String.IsNullOrEmpty(search))
                {
                    string searchValue = "%" + search + "%";
                    selWhere.SubSet(
                        CriteriaSet.Or()
                            .Like("mqqueues", "queueid", searchValue)
                            .Like("mqqueues", "resposta", searchValue)
                            .Like("mqqueues", "tabela", searchValue)
                            .Like("mqqueues", "tabelacod", searchValue)
                            .Like("mqqueues", "sendnumber", searchValue)
                        );
                }

                SelectQuery selQuery = new SelectQuery()
                    .Select(CSGenioAmqqueues.FldCodmqqueues)
                    .Select(CSGenioAmqqueues.FldSendnumber)
                    .Select(CSGenioAmqqueues.FldQueueID)
                    .Select(CSGenioAmqqueues.FldQueueKey)
                    .Select(CSGenioAmqqueues.FldTabela)
                    .Select(CSGenioAmqqueues.FldMQStatus)
                    .Select(CSGenioAmqqueues.FldDataStatus)
                    .Select(CSGenioAmqqueues.FldResposta)
                    .Select(CSGenioAmqqueues.FldDatacria)
                    .Select(CSGenioAmqqueues.FldTabelaCod)
                    .From(Area.AreaMQQUEUES)
                    .Where(selWhere)
                    .PageSize(pageSize)
                    .Page(page)
                    .OrderBy(orderBy[indiceOrder], sortOrder);
                selQuery.noLock = true;

                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
                sp.openConnection();
                DataMatrix dataSet = sp.Execute(selQuery);
                int total = DBConversion.ToInteger(sp.ExecuteScalar(QueryUtils.buildQueryCount(selQuery)));
                sp.closeConnection();

                List<QueueGenio> qList = CSGenio.business.Area.GetAllQueues();
                List<object> dataResult = new List<object>();

                for (int lin = 0; lin < dataSet.NumRows; lin++)
                {
                    dataResult.Add(new List<string> {
                    null
                    , dataSet.GetKey(lin, 2)
                    , dataSet.GetString(lin, 4)
                    , dataSet.GetKey(lin, 9)
                    , ConvertMQstatusString(dataSet.GetString(lin, 5))
                    , dataSet.GetDate(lin, 6).ToString()
                    , dataSet.GetString(lin, 7)
                    , dataSet.GetNumeric(lin, 1).ToString()
                    , dataSet.GetDate(lin, 8).ToString()
                    , dataSet.GetKey(lin, 0)
                    , (FindQueue(qList, dataSet.GetKey(lin, 2)) ? "QUEUE" : "ACK")
                });
                }

                return Json(new { Success = true, recordsTotal = total, data = dataResult });
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Server Error" });
            }
        }

        [HttpPost]
        public IActionResult QueueProcessArquive()
        {
            var queue = FromQuery("queue");
            try
            {
                string table = Area.AreaMQQUEUES.Table;
                string tabelaHistory = table + "_History";
                string where = "";

                List<IDbDataParameter> prmList = new List<IDbDataParameter>();
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);

                if (!String.IsNullOrEmpty(queue))
                {
                    where = " and queueid = '@queue'";
                    prmList.Add(sp.CreateParameter("queue", queue));
                }
                
                string sql = "";
                sql = String.Format("INSERT INTO  {0} ([queueid] ,[ano] ,[username] ,[tabela] ,[tabelacod] ,[queuekey] ,[queue] ,[mqstatus] ,[datastatus] " +
                    ",[operacao] ,[resposta] ,[ZZSTATE] ,[sendnumber] ,[datacria] ,[codmqqueues]) SELECT [queueid] ,[ano] ,[username] ,[tabela] ,[tabelacod] ,[queuekey] ,[queue] " +
                    ",[mqstatus],[datastatus] ,[operacao] ,[resposta] ,[ZZSTATE] ,[sendnumber] ,[datacria] ,[codmqqueues] FROM {1} " +
                    " WHERE (mqstatus IN('3', '6') OR sendnumber = 3) AND dbo.Diferenca_entre_Datas(datastatus, GETDATE(),'D') >= 1 {2} DELETE FROM {1} WHERE (mqstatus IN('3', '6') OR sendnumber = 3) AND" +
                    " dbo.Diferenca_entre_Datas(datastatus, GETDATE(),'D') >= 1 {2} ",tabelaHistory, table, where);
                                
                sp.openConnection();
                sp.executeQuery(sql, prmList);
                sp.closeConnection();

                return Json(new { status = "OK", msg = Resources.Resources.A_OPERACAO_FOI_CONCL36721 });
            }
            catch (Exception ex)
            {
                return Json(new { status = "E", msg =  ex.Message});
            }
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            try
            {
                string search = FromQuery("global_search");
                string order = FromQuery("sort[0].name");
                string orderDir = FromQuery("sort[0].order");
                int page = Convert.ToInt32(FromQuery("page"));
                int pageSize = Convert.ToInt32(FromQuery("per_page"));
                string queue = FromQuery("queue");

                string tableHistory = Area.AreaMQQUEUES.Table + "_History";

                FieldRef queueId = new FieldRef(tableHistory, "QueueID");
                FieldRef queueKey = new FieldRef(tableHistory, "QueueKey");
                FieldRef table = new FieldRef(tableHistory, "Tabela");
                FieldRef tabelaCod = new FieldRef(tableHistory, "TabelaCod");
                FieldRef mqstaus = new FieldRef(tableHistory, "MQStatus");
                FieldRef datastatus = new FieldRef(tableHistory, "DataStatus");
                FieldRef resposta = new FieldRef(tableHistory, "Resposta");
                FieldRef sendNumber = new FieldRef(tableHistory, "Sendnumber");
                FieldRef datacria = new FieldRef(tableHistory, "Datacria");
                FieldRef codmqqueue = new FieldRef(tableHistory, "Codmqqueues");

                List<FieldRef> orderBy = new List<FieldRef>() {queueId, table ,tabelaCod, mqstaus, datastatus ,resposta ,sendNumber, datacria };

                Dictionary<string, string> mqStatusDesc = new Dictionary<string, string>();
                int indiceOrder = 0;

                if (!String.IsNullOrEmpty(order))
                    indiceOrder = GenFunctions.atoi(order);

                SortOrder sortOrder = SortOrder.Ascending;
                if (orderDir == "desc")
                    sortOrder = SortOrder.Descending;

                CriteriaSet selWhere = CriteriaSet.And();

                if (!String.IsNullOrEmpty(queue))
                    selWhere.SubSet(CriteriaSet.And().Equal(queueId, queue));
                
                if (!String.IsNullOrEmpty(search))
                {
                    string searchValue = "%" + search + "%";
                    selWhere.SubSet(
                        CriteriaSet.Or()
                            .Like(queueId, searchValue)
                            .Like(resposta, searchValue)
                            .Like(table, searchValue)
                            .Like(tabelaCod, searchValue)
                            .Like(sendNumber, searchValue)
                        );
                }

                SelectQuery selQuery = new SelectQuery()
                    .Select(codmqqueue)
                    .Select(sendNumber)
                    .Select(queueId)
                    .Select(queueKey)
                    .Select(table)
                    .Select(mqstaus)
                    .Select(datastatus)
                    .Select(resposta)
                    .Select(datacria)
                    .Select(tabelaCod)
                    .From(tableHistory)
                    .Where(selWhere)
                    .PageSize(pageSize)
                    .Page(page)
                    .OrderBy(orderBy[indiceOrder], sortOrder);

                List<object> dataResult = new List<object>();

                //TSX (2019-02-18) - Added option to filter queues on log database (false) or _history table (true)
                var logHistory = FromQuery("LogHistory");
                bool vallogHistory = true;
                if (!string.IsNullOrEmpty(logHistory))
                    vallogHistory = bool.Parse(logHistory);

                PersistentSupport sp;
                if (vallogHistory)
                    sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
                else
                    sp = CSGenio.persistence.PersistentSupport.getPersistentSupportLog(CSGenio.framework.Configuration.DefaultYear, "");
                sp.openConnection();
                DataMatrix dataSet = sp.Execute(selQuery);
                int total = DBConversion.ToInteger(sp.ExecuteScalar(QueryUtils.buildQueryCount(selQuery)));
                sp.closeConnection();

                List<QueueGenio> qList = CSGenio.business.Area.GetAllQueues();

                for (int lin = 0; lin < dataSet.NumRows; lin++)
                {
                    dataResult.Add(new List<string> {
                        null
                        , dataSet.GetKey(lin, 2)
                        , dataSet.GetString(lin, 4)
                        , dataSet.GetKey(lin, 9)
                        , ConvertMQstatusString(dataSet.GetString(lin, 5))
                        , dataSet.GetDate(lin, 6).ToString()
                        , dataSet.GetString(lin, 7)
                        , dataSet.GetNumeric(lin, 1).ToString()
                        , dataSet.GetDate(lin, 8).ToString()
                        , dataSet.GetKey(lin, 0)
                        , (FindQueue(qList, dataSet.GetKey(lin, 2)) ? "QUEUE" : "ACK")
                    });
                }

                return Json(new { Success = true, recordsTotal = total, data = dataResult });
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Server Error" });
            }
        }

        [HttpGet]
        public IActionResult GetChartData()
        {
            try
            {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
                sp.openConnection();
                List<object> lst = new List<object>();
                List<object> lstAck = new List<object>();

                DateTime dtalimt = DateTime.Now.AddHours(-24);

                string sql = "WITH stats as (SELECT codmqqueues, datacria, CAST(datacria as DATE) as Dia,  Hora = FORMAT( cast( FORMAT(datacria,'HH:'+ CAST(CAST( DATEPART(minute, datacria)/20 as INT)*20 as varchar)) as time), N'hh\\.mm' ) " +
                    "                               FROM "+ Area.AreaMQQUEUES.Table + " WITH (nolock) WHERE datacria >= @data and tabelacod <> queuekey) " +
                    "SELECT Dia, Hora, count(*) as Qtd  FROM stats GROUP BY Dia, hora ORDER BY Dia, Hora";

                DataMatrix dm = sp.executeQuery(sql, new List<IDbDataParameter> { sp.CreateParameter("data", dtalimt) });

                for (int i = 0; i < dm.NumRows; i++)
                {
                    lst.Add(new[] { ToJavascriptTimestamp(GenFunctions.DateSetTime(dm.GetDate(i, 0), dm.GetString(i, 1))), dm.GetNumeric(i,2) });
                }
                                
                sql = "WITH stats as (SELECT codmqqueues, datacria, CAST(datacria as DATE) as Dia,  Hora = FORMAT( cast( FORMAT(datacria,'HH:'+ CAST(CAST( DATEPART(minute, datacria)/20 as INT)*20 as varchar)) as time), N'hh\\.mm' ) " +
                    "                               FROM " + Area.AreaMQQUEUES.Table + " WITH (nolock) WHERE datacria >= @data1 and tabelacod = queuekey) " +
                    "SELECT Dia, Hora, count(*) as Qtd  FROM stats GROUP BY Dia, hora ORDER BY Dia, Hora";

                dm = sp.executeQuery(sql, new List<IDbDataParameter> { sp.CreateParameter("data1", dtalimt) });

                for (int i = 0; i < dm.NumRows; i++)
                {
                    lstAck.Add(new[] { ToJavascriptTimestamp(GenFunctions.DateSetTime(dm.GetDate(i, 0), dm.GetString(i, 1))), dm.GetNumeric(i, 2) });
                }
                
                sp.closeConnection();

                var dataArray = new object[] {
                    new {data = lst.ToArray(), label="Queues"},
                    new {data = lstAck.ToArray(), label="Acknowledge"}
                };

                return Json(dataArray);                                
            }
            catch (Exception)
            {
                return Json(new[] { new{} });
            }            
        }	

		private long ToJavascriptTimestamp(DateTime input)
        {
            TimeSpan span = new TimeSpan(new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            DateTime time = input.Subtract(span);
            return (long)(time.Ticks / 10000);
        }		

        private bool FindQueue(List<QueueGenio> queueList, string queue)
        {
            QueueGenio existQueue = queueList.Find(x => x.Name.Equals(queue, StringComparison.OrdinalIgnoreCase));
            return (existQueue != null);                
        }
    }
	
}
