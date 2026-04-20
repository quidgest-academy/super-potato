using Administration.AuxClass;
using CSGenio;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.core.persistence;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Administration.Models;

namespace Administration.Controllers
{
    public class DashboardController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
    {
        // GET: api/Dashboard
        [HttpGet]
        public IActionResult Index()
        {
            var model = new Models.DashboardModel()
            {
                HasConfig = false,
                HasValidConfig = false,
				IsBetaTesting = false,
                HasEnvironment = false,
                HasDiffVersion = false,
                HasSGBDVersion = false,
				HasDiffUserSettingsVersion = false,
                VersionDb = 0,
                VersionIdxDb = 0,
                VersionDbGen = CSGenio.framework.Configuration.VersionDbGen,
                VersionIdxDbGen = CSGenio.framework.Configuration.VersionIdxDbGen,
                VersionUpgrIndxGen = CSGenio.framework.Configuration.VersionUpgrIndxGen,
				UserSettingsVersion = CSGenio.framework.Configuration.UserSettingsVersion
            };

            //Check if configuration exists
            if (!configManager.Exists())
            {
                return Json(new { Success = false, CurrentMaintenance = Maintenance.Current, model = new { ResultErrors = calculateErrors(model) } });
            }
            model.HasConfig = true;

            //check if config have the last version
            if (!AuxFunctions.CheckXMLIsValid(configManager))
                model.ResultErrors = calculateErrors(model);
            else { model.HasValidConfig = true; }

            //depois verificar se está válido
            CSGenio.framework.Configuration.Reload();
            DataSystemXml dataSystem;
            try
            {
                dataSystem = CSGenio.framework.Configuration.ResolveDataSystem(CurrentYear, CSGenio.framework.Configuration.DbTypes.NORMAL);
                if (dataSystem == null)
                    throw new Exception();

                if (!PersistentSupport.TestDBConnection(CurrentYear))
                {
                    model.ResultErrors = Resources.Resources.A_BASE_DE_DADOS_NAO_59749;
                    return Json(new { Success = true, CurrentMaintenance = Maintenance.Current, model });
                }
            }
            catch(Exception)
            {
                model.ResultErrors = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;
                return Json(new { Success = true, CurrentMaintenance = Maintenance.Current, model });
            }

            //leitura do configuracaoXML to colocar nas variaveis visiveis ao cliente
            DatabaseType tpConn = dataSystem.GetDatabaseType();
            model.TpSGBD = tpConn.ToString();
            model.SGBDServer = dataSystem.Server;
            if (!string.IsNullOrEmpty(dataSystem.Service) || !string.IsNullOrEmpty(dataSystem.ServiceName))
            {
                model.SGBDServer += "(";
                if (!string.IsNullOrEmpty(dataSystem.Service))
                    model.SGBDServer += dataSystem.Service;
                if (!string.IsNullOrEmpty(dataSystem.ServiceName))
                    model.SGBDServer += (!string.IsNullOrEmpty(dataSystem.Service)? " | ": "") + dataSystem.ServiceName;
                model.SGBDServer += ")";
            }
            model.DBSchema = dataSystem.Schemas[0].Schema;

            //validar ligação com servidor
            bool serverConnection = true;
			
			if (CSGenio.framework.Configuration.QAEnvironment == 1)
                model.IsBetaTesting = true;

            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            try
            {
                string sql = "";
                sp.openConnection();
                if (tpConn == DatabaseType.ORACLE)
                    sql = "SELECT* FROM v$version WHERE banner LIKE 'Oracle%'";
                if (tpConn == DatabaseType.POSTGRES)
                    sql = "SELECT version();";
                else
                    sql = "SELECT @@version";
                model.SGBDVersion = (string)sp.executeScalar(sql);
                sp.closeConnection();
                
            }
            catch (Exception e)
            {
                sp.closeConnection();
                model.SGBDVersion = e.Message;
                model.HasSGBDVersion = true;
                serverConnection = false;
            }

            if (serverConnection && sp.CheckIfDatabaseExists())
            {
                try 
                {
                    sp.openConnection();
                    var dbVersionReader = new DatabaseVersionReader(sp);
                    model.VersionDb = dbVersionReader.GetDbVersion();
                    model.VersionIdxDb = dbVersionReader.GetDbIndexVersion();
                    model.VersionUpgrIndx = dbVersionReader.GetDbUpgradeVersion();
					model.CurrentUserSettingsVersion = dbVersionReader.GetDbUserSettingsVersion();

                    model.DBSize = AuxFunctions.GetDBSize(CurrentYear, model.DBSchema);
                    sp.closeConnection();
                }
                catch (Exception)
                {
                    model.VersionDb = 0;
                    model.VersionIdxDb = 0;
                    model.VersionUpgrIndx = 0;
					model.CurrentUserSettingsVersion = 0;
                }

                // Check for maintenance Status
                Maintenance.GetMaintenanceStatus(sp);
            }
            
            //colocação de aviso de erros
            if (model.VersionDb != model.VersionDbGen)
                model.HasDiffVersion = true;
            if (model.VersionUpgrIndx < model.VersionUpgrIndxGen)
                model.HasDiffVersion = true;
            if (model.VersionIdxDb != model.VersionIdxDbGen)
                model.HasDiffIdxVersion = true;
			// Check if current user settings version is below the latest version
			if (model.CurrentUserSettingsVersion < model.UserSettingsVersion)
				model.HasDiffUserSettingsVersion = true;

            //Informação do PC
            // >> SO
            model.SODesc = String.Format("{0} | {1}",
                System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);

            // >> Name da máquina
            model.PCDesc = Environment.MachineName;

            // >> CPU
            model.HardwProcDesc = String.Format("{0} | Threads: {1}", 
                Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER", EnvironmentVariableTarget.Machine), Environment.ProcessorCount);
            // >> Memory
            // The amount of physical memory mapped to the process context.
            var usedMemory = Environment.WorkingSet / 1024 / 1024;
            model.HardwMemDesc = String.Format("{0}mb (used)", usedMemory);

            // >> Drives
            model.HardwDrivDesc = "";
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            var sbHDD = new StringBuilder();
            foreach (DriveInfo d in allDrives)
                if (d.IsReady && d.DriveType == DriveType.Fixed)
                    sbHDD.AppendFormat("<b>{0}</b> | <i>{1}</i> {2, 15} MB <i>of</i> {3, 15} GB<br />", d.Name, Resources.Resources.ESPACO_LIVRE_13565, d.TotalFreeSpace / 1024 / 1024, d.TotalSize / 1024 / 1024 / 1024);
            model.HardwDrivDesc = sbHDD.ToString();

            //colocar visivel a zona de ambiente
            model.HasEnvironment = true;

            model.ResultErrors = calculateErrors(model);

            return Ok(new { Success = true, CurrentMaintenance = Maintenance.Current, model });
        }

        private string calculateErrors(Models.DashboardModel model)
        {
            string msg = "";
            if (!model.HasEnvironment)
                msg = Resources.Resources.NAO_FOI_ENCONTRADO_N28305;
			else if (!model.HasValidConfig)
                msg = Resources.Resources.E_NECESSARIO_PROCEDE36325 + "<br />";
            else if (model.HasSGBDVersion)
                msg = Resources.Resources.ERRO_NO_ACESSO_AO_SE43775 + "<br />";
            else if (model.HasDiffVersion && model.VersionDb == -1)
                msg = Resources.Resources.NAO_FOI_POSSIVEL_LOC30521 + "<br />";
            else if((model.HasDiffVersion || model.HasDiffUserSettingsVersion) && model.VersionDb != -1)
                msg = Resources.Resources.VERSAO_DE_BASE_DE_DA22093 + " <b>" + Resources.Resources.EXECUTE_A_OPERACAO_D01219 + "</b><br />";
            else if(model.HasDiffIdxVersion && model.VersionIdxDb != 0)
                msg = Resources.Resources.VERSAO_DE_INDICES_IN26316 + " <b>" + Resources.Resources.EXECUTE_A_OPERACAO_D01219 + "</b><br />";

            return msg;
        }

        [HttpPost]
        public JsonResult CreateConfiguration()
        {
            try
            {
                configManager.CreateNewConfig();                
                Configuration.Reload();
                return Json(new { Success = true });
            }
            catch (Exception e)
            {  
                return Json(new { Success = false, Message = e.Message }) ;
            }
        }

        [HttpPost]
        public IActionResult DisableMaintenance() {
            PersistentSupport sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
            if (Maintenance.DisableMaintenance(sp)) {
                return Json(new { Success = true, CurrentMaintenance = Maintenance.Current, message = "Maintenance Disabled" });
            } else  {
                return Json(new { Success = false, CurrentMaintenance = Maintenance.Current, message = "Error Disabling Maintenance" });
            }
        }

        //private static string Scheduleformat = "yyyy-MM-ddTHH:mm:ssZ";
        [HttpPost]
        public IActionResult ScheduleMaintenance([FromBody]DateTimeModel data)
        {
            if (data == null)
                return DisableMaintenance();

            PersistentSupport sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);
            if (Maintenance.ScheduleMaintenance(sp, data.Date))
            {
                return Json(new { Success = true, CurrentMaintenance = Maintenance.Current, message = "Maintenance Disabled" });
            }
            else
            {
                return Json(new { Success = false, CurrentMaintenance = Maintenance.Current, message = "Error Disabling Maintenance" });
            }
        }
    }
}
