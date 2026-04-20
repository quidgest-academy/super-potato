using CSGenio;
using CSGenio.framework;
using CSGenio.persistence;
using ExecuteQueryCore;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
    public class MaintenanceAPIController(CSGenio.config.IConfigurationManager configManager) : ControllerBase  
    {
        private enum Status
        {
            NULL,
            Ongoing,
            Reindexing,
            Stopped,
            Cancelling,
            Cancelled
        }

        private static Status MaintenanceStatus;
        private static RdxParamUpgradeSchema RdxItem = null;
        private static CancellationTokenSource cancelTknSrc = null;

        private static Timer timer;
        private static Timer cancelTimer;
        private static AutoResetEvent autoEvent = new AutoResetEvent(false);
        private static DbAdminController dbAdminController;

        private static bool reindex = false, auto_end = false;


        /// <summary>
        /// Method called by POST that is used to start the maintence
        /// 
        /// Possible Parameters:
        ///  - options: {
        ///        reindex: false,
        ///        reindex_items: ["CREATEDB",  //If left empty, it will full reindex
        ///             "CREATESP",
        ///             "DROPFK",
        ///             "CREATEHRDSCHEMA",
        ///             "DELETELOGTRIGGERS",
        ///             "CREATESCHEMA",
        ///             "TBLREBUILD",
        ///             "DROPCOLUMNS",
        ///             "UPDATECFG",
        ///             "UPDATESP",
        ///             "DELETETMPDB",
        ///             "ADDINDEX",
        ///             "INVALIDZZSTATE",
        ///             "CREATEFORMULASPROCS",
        ///             "UPDATEREPLICAS",
        ///             "RESETCALCFIELDS",
        ///             "UPDATEFORMULAFIELDS",
        ///             "FORMULASDAILYUPDATE",
        ///             "UPDATEINTCOD",
        ///             "CREATELOGTRIGGERS",
        ///             "CREATELOGVIEWS",
        ///             "CREATEDBLOG",
        ///             "CREATESCHEMALOG",
        ///             "CREATELOGVIEWSLOG",
        ///             "CREATEDEFAULTUSER",
        ///             "GRANTACCESSSP",
        ///             "CREATEFK",
        ///             "SHRINKDB",
        ///             "DELROWS",
        ///             "CTRLRECORDS",
        ///             "FK2NULL"],
        ///        auto_end_maintenance: false
        ///    }
        /// </summary>
        /// <param name="data">Json value</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Start([FromBody] JObject data)
        {
            try
            {
                PersistentSupport sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);

                //Get body params
                string[] reindex_items = null;
                if (data["options"] != null)
                {
                    reindex = (data["options"]["reindex"] != null) ? data["options"]["reindex"].ToObject<bool>() : false;
                    auto_end = (data["options"]["auto_end_maintenance"] != null) ? data["options"]["auto_end_maintenance"].ToObject<bool>() : false;
                    reindex_items = (data["options"]["reindex_items"] != null) ? data["options"]["reindex_items"].ToObject<string[]>() : null;
                }

                //If maintenance is already running, error out
                Maintenance.GetMaintenanceStatus(sp);
                if (Maintenance.Current.IsActive)
                {
                    return Json(new { Success = false, Message = Resources.Resources.A_MAINTENANCE_TASK_H03437 });
                }
                if (Maintenance.Current.IsScheduled)
                {
                    return Json(new { Success = false, Message = Resources.Resources.A_MAINTENANCE_TASK_I22024 });
                }

                //If no options are passed, no need to progress
                if (!reindex && auto_end)
                {
                    return Json(new { Success = true });
                }

                //In case the maintenance fails, return error
                if (!Maintenance.ScheduleMaintenance(sp, DateTime.Now))
                {
                    return Json(new { Success = false, Message = Resources.Resources.THERE_HAS_BEEN_AN_IN10114 });
                }
                MaintenanceStatus = Status.Ongoing;

                if (reindex) //Reindex if needed
                {
                    MaintenanceStatus = Status.Reindexing;
                    cancelTknSrc = new CancellationTokenSource();
                    CancellationToken cToken = cancelTknSrc.Token;
                    dbAdminController = new DbAdminController(configManager);

                    string Year = CurrentYear;
                    var conf = configManager.GetExistingConfig();

                    var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == Year); // Default == null 

                    Models.DbAdminModel model = dbAdminController.initDbModel(dataSystem, conf, Year);

                    if(reindex_items != null && reindex_items.Length > 0)
                    {
                        model.Items.ForEach(itm => itm.Value = false);
                        foreach (string item in reindex_items)
                        {
                            model.Items.Where(itm => itm.Id == item).First().Value = true;
                        }
                    }

                   //Check if something is running
                    if (RdxItem != null)
                    {
                        if (RdxItem.Progress.State == RdxProgressStatus.RUNNING)
                            return Json(new { Success = true });
                    }
                    RdxItem = dbAdminController.startReindexation([model], Year, cToken);    

                    if (timer != null)
                        timer.Dispose();

                    timer = new Timer(autoMaintenanceEnder, autoEvent, 1000, 500);                               
                }
                
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult End()
        {
            try
            {   
                if (cancelTimer != null)
                {
                    cancelTimer.Dispose();
                    cancelTimer = null;
                }
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                if (RdxItem != null && RdxItem.Progress.State == RdxProgressStatus.CANCELLED)
                {
                    MaintenanceStatus = Status.Cancelling;
                    cancelTknSrc.Cancel();

                    if (timer != null)
                        timer.Dispose();

                    cancelTimer = new Timer(cancelMaintenance, autoEvent, 1000, 500);

                    return Json(new { Success = true, Message = Resources.Resources.CANCELING_TASK___32461 });
                }
                RdxItem = null;

                PersistentSupport sp = PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
                if (Maintenance.DisableMaintenance(sp))
                {
                    if(MaintenanceStatus == Status.Stopped || MaintenanceStatus == Status.Cancelled || MaintenanceStatus == Status.NULL)
                    {
                        return Json(new { Success = false, Message = Resources.Resources.THERE_ARE_NO_TASKS_C61278 });
                    }

                    if (MaintenanceStatus == Status.Cancelling)
                        MaintenanceStatus = Status.Cancelled;
                    else
                        MaintenanceStatus = Status.Stopped;

                    if(cancelTknSrc != null)
                    {
                        cancelTknSrc.Dispose();
                    }

                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false, Message = Resources.Resources.THERE_HAS_BEEN_AN_IN33843 });
                }
            }
            catch(Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            switch (MaintenanceStatus)
            {
                case Status.Ongoing:
                    return Json(new { Status = Resources.Resources.ONGOING11687, Message = Resources.Resources.THE_MAINTENANCE_TASK09821 });

                case Status.Reindexing:
                    if(dbAdminController == null)                    
                        return Json(new { Status = Resources.Resources.REINDEXING27609, Message = Resources.Resources.THE_MAINTENANCE_TASK58259 });                    
                    else                    
                        return Json(new
                        {
                            Status = Resources.Resources.REINDEXING27609,
                            Message = Resources.Resources.THE_MAINTENANCE_TASK58259,
                            Progress = new
                            {
                                Count = RdxItem != null ? RdxItem.Progress.Percentage() : 0,
                                ActualScript = RdxItem != null && (RdxItem.Progress.ActualScript != null && RdxItem.Progress.ActualScript != "") ? RdxItem.Progress.ActualScript + "..." : "",
                                Completed = RdxItem != null ? RdxItem.Progress.State == RdxProgressStatus.SUCCESS : true
                            }
                        });
                    
                case Status.NULL:
                case Status.Stopped:
                    return Json(new { Status = Resources.Resources.STOPPED40364, Message = Resources.Resources.THERE_IS_NO_MAINTENA45585 });

                case Status.Cancelling:
                    return Json(new { Status = Resources.Resources.CANCELLING20813, Message = Resources.Resources.THE_MAINTENANCE_TASK60357 });

                case Status.Cancelled:
                    return Json(new { Status = Resources.Resources.CANCELLED31809, Message = Resources.Resources.THE_MAINTENANCE_TASK36524 });

                default:
                    return Json(new { Status = Resources.Resources.ERROR42688, Message = Resources.Resources.THE_CURRENT_STATUS_I33421 });
            }
        }

        private void autoMaintenanceEnder(object state)
        {
            if (RdxItem != null)
            {
                if (auto_end) End();                
                else
                {
                    MaintenanceStatus = Status.Ongoing;

                    if (timer != null)
                        timer.Dispose();
                }
                return;
            }

            if (RdxItem.Progress.State == RdxProgressStatus.SUCCESS || RdxItem.Progress.State == RdxProgressStatus.CANCELLED)
            {
                if (auto_end) End();
                else
                {
                    MaintenanceStatus = Status.Ongoing;

                    if (timer != null)
                        timer.Dispose();
                }
            }       
        }

        private void cancelMaintenance(object state)
        {
            if (RdxItem.Progress.State == RdxProgressStatus.CANCELLED)
            {
                End();
            }     
        }
    }
}