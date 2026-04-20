using Administration.AuxClass;
using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using ExecuteQueryCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Administration.Models;
using System.Threading.Tasks;
using Quidgest.Persistence.GenericQuery;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using System.Threading;
using DbAdmin;

namespace Administration.Controllers
{
    public class DbAdminController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
    {
        private static RdxParamUpgradeSchema RdxItem = null;
        private static CancellationTokenSource reindexCTknSrc = null;

        private static IndexesModel IndexesItem = new IndexesModel()
        {

            ResultMsg = "",
            AlertType = AlertTypeEnum.info,
            Indexes = new List<IndexModel>()
            {
                new IndexModel()
                {
                    Num = 0,
                    OrderCol = 0, // coluna de ordenação
                    Active = false,
                    ProgressMessage ="",
                    ProgressPercent =0,
                    Completed = true,
                    LastUpdate = DateTime.MinValue,
                    UnusedIndexesList = new List<UnusedIndexItem>(),
                    IndexType = "Unused",
                    IndexTitle = "INDICES_POUCO_USADOS47529"
                },
                new IndexModel()
                {
                    Num = 1,
                    OrderCol = 0, // coluna de ordenação
                    Active = false,
                    ProgressMessage ="",
                    ProgressPercent =0,
                    Completed = true,
                    LastUpdate = DateTime.MinValue,
                    RecommendedIndexesList = new List<RecommendedIndexItem>(),
                    IndexType = "Recommended",
                    IndexTitle = "INDICES_RECOMENDADOS25802"
                }
            }
        };

        private static DataQualityModel DqItem = new DataQualityModel()
        {
            ResultMsg = "",
            Incoherencies = new List<IncoherencyModel>()
            {

                new IncoherencyModel()
                {
                    Num = 0,
                    Active = false,
                    ProgressMessage = "",
                    ProgressPercent = 0,
                    Completed = true,
                    LastUpdate = DateTime.MinValue,
                    IncoherentRelations = new List<IncoherentRelationItem>(),
                    IncoherenceType = "IncoherentRelation",
                    IncoherenceTitle = "INCOERENCIA_DE_RELAC38138",
                    RelationMode=HardCodedLists.RelationsMode.AMBAS,
                    ViewsIsChecked = false,
                    NullsIsChecked = true
                },
                new IncoherencyModel()
                {
                    Num = 1,
                    Active = false,
                    ProgressMessage = "",
                    ProgressPercent = 0,
                    Completed = true,
                    LastUpdate = DateTime.MinValue,
                    RelationMode = HardCodedLists.RelationsMode.DIRETAS,
                    OrphanRelations = new List<OrphanRelation>(),
                    IncoherenceType = "OrphanRelation",
                    IncoherenceTitle = "REGISTOS_ORFAOS26691",
                    ViewsIsChecked = false
                }
            }
        };

        private static AreasPathwaysList _relacoesCoerentes = new AreasPathwaysList()
        {
            AreaPathwaysList = new List<AreaPathways>()
        };

        [HttpPost]
        public IActionResult GetMaintenanceLogDetails([FromBody] int logIndex)
        {
            try
            {
                DBMaintenance dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
                var logDetails = dbMaintenance.GetMaintenanceLogDetails(logIndex);

                return Json(logDetails);
            }
            catch (DBMaintenance.DBMaintenanceException ex)
            {
                Log.Error(ex.Message);
                return Json(new { ResultMsg = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Json(new { ResultMsg = Resources.Resources.AN_ERROR_OCCURED_WHI37264 });
            }
        }

        [HttpPost]
        public IActionResult GetMaintenanceLogs([FromBody] int numLogs)
        {
            try
            {
                DBMaintenance dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
                List<RdxOperationInfo> logInfo = dbMaintenance.GetMaintenanceLogsInfo(numLogs);

                return Json(logInfo);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Json(new { ResultMsg = Resources.Resources.AN_ERROR_OCCURED_WHI37264 });
            }
        }

        public DbAdminModel initDbModel(DataSystemXml dataSystem, ConfigurationXML conf, string year)
        {
            //read cfg table from the database
            //1- Database nay not exist
            //2- Table cfg may not exist
            //3- Column Qversion may not exist
            //4- Column value may be null

            //***********************************

            DbAdminModel model = new DbAdminModel();

            DBMaintenance dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
            model.reindexMenu = dbMaintenance.GetReindexScripts();
            model.Items = new List<ReindexFunctionItem>();

            if (dataSystem == null)
            {
                model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;
                return model;
            }

            model.DSName = dataSystem.Name;

            List<RdxOperationLog> scriptLog = RdxOperationLog.readAggregateXML(PersistentSupport.LogReindexPath());
            RdxOperationLog lastLog = scriptLog?
                .Where(log => log != null && log.DataSystem == model.DSName)
                .LastOrDefault();

            List<ReIndexFunction> modelItems = model.reindexMenu.ReIndexItems
                .Where(item =>
                    // Log database options only appear if the log database is configured
                    item.Connection != ConnectionType.Log || (dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count != 0)
                )
                .ToList();

            model.Items.AddRange(DBMaintenance.LoadReindexFunctionItems(lastLog, modelItems));

            // Gather the essential information of the latest reindex operation, if there is one
            if (lastLog != null)
                model.LastLogInfo = DBMaintenance.CreateOperationInfo(lastLog, (scriptLog.Count - 1), model.Items);

            // read upgrade scripts version
            model.VersionUpgrScripts = CSGenio.framework.Configuration.VersionUpgrIndxGen;

            model.DBSchema = dataSystem.Schemas[0].Schema;
            if (PersistentSupport.TestDBConnection(year))
            {
                // read DB version
                model.VersionDb = CSGenio.framework.Configuration.GetDbVersion(year);
                //read DB Size
                model.DBSize = AuxFunctions.GetDBSize(year, model.DBSchema);
                // read upgrade index version
                model.VersionUpgrIndx = CSGenio.framework.Configuration.GetDbUpgrIndx(year);
            }

            // read app version from the code watermark
            model.VersionApp = CSGenio.framework.Configuration.Version;

            //Calculate suggested timeout. 1* for each 10GB
            int mult = ((int)model.DBSize / (10 * 1024)) + 1;
            model.Timeout = model.reindexMenu.timeout * mult;

            string reindexIndexPath = dbMaintenance.GetIndexReindexPath();

            // read reindex version
            model.VersionReIdx = readReIndexVersion(model, reindexIndexPath, out bool reindexExists);

            // check if the reindex directory exists for the correct version of the site
            model.UpgradeIsAvailable = false;

            // the available version should also be larger than the DB version
            if (model.VersionApp > model.VersionDb && reindexExists)
            {
                if (model.VersionApp == model.VersionReIdx)
                    model.UpgradeIsAvailable = true;
                else if (model.VersionApp < model.VersionReIdx)
                    // ReIndex bigger than AppVersion
                    model.ResultMsg = Resources.Resources.A_VERSAO_DOS_FICHEIR33338;
                else
                    // ReIndex smaller than AppVersion
                    model.ResultMsg = Resources.Resources.A_VERSAO_DOS_FICHEIR26494;
            }

            // if version save in cfg is higher than templates DB schema, then force a full reindex
            if (model.VersionDb > model.VersionApp)
                model.Zero = true;

            model.ReindexActive = RdxItem != null;

            return model;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!AuxFunctions.CheckXMLIsValid(configManager))
                return Json(new { redirect = "config_migration" });

            ConfigurationXML conf = configManager.GetExistingConfig();

            List<DbAdminModel> maintenanceModels = [];
            conf.DataSystems.ForEach(ds => maintenanceModels.Add(initDbModel(ds, conf, ds.Name)));

            return Json(maintenanceModels);
        }

        private int readReIndexVersion(DbAdminModel model, string path, out bool reindexExists)
        {

            int reIdxVersion = 0;

            // Parse the info xml to ensure the upgrade version is the correct one
            if (System.IO.File.Exists(path))
            {
                reindexExists = true;
                var doc = new System.Xml.XmlDocument();
                doc.Load(path);
                var xe = doc.DocumentElement;
                xe = xe.GetElementsByTagName("Definicoes")[0] as System.Xml.XmlElement;
                xe = xe.GetElementsByTagName("Versao")[0] as System.Xml.XmlElement;
                reIdxVersion = int.Parse(xe.InnerText, CultureInfo.InvariantCulture);
            }
            else
            {
                reindexExists = false;
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = String.Format(Resources.Resources._0__NAO_EXISTE__A_AT23585, path);
            }

            return reIdxVersion;
        }


        public RdxParamUpgradeSchema startReindexation(DbAdminModel[] models, string Year, CancellationToken cToken, int currentModelIdx = 0)
        {
            DbAdminModel model = models[currentModelIdx];
            RdxParamUpgradeSchema rdxParam = new RdxParamUpgradeSchema()
            {
                Username = model.DbUser,
                Password = model.DbPsw,
                Year = Year,
                DirFilestream = model.DirFilestream,
                Zero = model.Zero,
                Origin = "Database Maintenance Interface"
            };

            if (!ModelState.IsValid) {
                model.AlertType = AlertTypeEnum.danger;
                throw new BusinessException(Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860, "DbAdminController.reindex", Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860);
            }

            List<ReindexFunctionItem> rdxFunctions = GetSelectedReindexFunctionItems(model.Items);
            List<string> allSelectedItems = rdxFunctions.Select(x => x.Id).ToList(); //Get the ids

            //If no scripts were selected
            if(!rdxFunctions.Where(x => x.Selectable == true).Any())
            {
                rdxParam = new RdxParamUpgradeSchema();

                rdxParam.Progress.Message = Resources.Resources.NAO_FORAM_SELECIONAD28047;
                rdxParam.Progress.State = RdxProgressStatus.FINISHED;

                return rdxParam;
            }

            DBMaintenance dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
            dbMaintenance.StartReindexation(rdxParam, null,
                allSelectedItems,
                null,
                //We use this event to replace the message that it is displayed to the user
                (sender, eventArgs, status) =>
                {
                    rdxParam.Progress = status.Clone();
                    if (status.State == RdxProgressStatus.SUCCESS) {
                        if(++currentModelIdx < models.Length)
                        {
                            string nextYear = models[currentModelIdx].DSName;
                            RdxItem = startReindexation(models, nextYear, cToken, currentModelIdx);
                        }
                        else
                        {
                            model.AlertType = AlertTypeEnum.success;
                            rdxParam.Progress.State = RdxProgressStatus.FINISHED;
                            rdxParam.Progress.Message = Resources.Resources.A_OPERACAO_FOI_CONCL36721;
                        }
                    }
                },
                cToken, model.Timeout);

            return rdxParam;
        }

        private static List<ReindexFunctionItem> GetSelectedReindexFunctionItems(List<ReindexFunctionItem> reindexFunctionItems)
        {
            bool upgradeVersionChecked = reindexFunctionItems.First(item => item.Id == "UPGRADECLIENTS").Value;
            reindexFunctionItems.First(item => item.Id == "UPGRADECLIENT1").Value = upgradeVersionChecked;
            reindexFunctionItems.First(item => item.Id == "UPGRADECLIENT2").Value = upgradeVersionChecked;

            return reindexFunctionItems.Where(x => x.Value == true).ToList(); //Get all the selected items
        }

        [HttpPost]
        public IActionResult Start([FromBody] DbAdminModel[] models)
        {
            try
            {
                if(!ModelState.IsValid)            
                    throw new BusinessException(Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860, "DbAdminController.reindex", Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860);                    

                //Check if something is running
                if (RdxItem?.Progress.State == RdxProgressStatus.RUNNING)
                    return Json(new { Success = true });

                //Dispose previous cancellation token source if it exists
                reindexCTknSrc?.Dispose();

                //Create cancellation token
                reindexCTknSrc = new CancellationTokenSource();
                CancellationToken cToken = reindexCTknSrc.Token;

                //Start reindex
                RdxItem = startReindexation(models, models[0].DSName, cToken);
            }
            catch (GenioException e)
            {
                if(RdxItem == null)
                    RdxItem = new RdxParamUpgradeSchema();

                DbAdminModel model = models.FirstOrDefault(model => model.DSName == RdxItem.Year);

                if (model != null)
                    model.AlertType = AlertTypeEnum.danger;

                RdxItem.Progress.Message = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                RdxItem.Progress.State = RdxProgressStatus.ERROR;
            }
            catch (Exception e)
            {
                if(RdxItem == null)
                    RdxItem = new RdxParamUpgradeSchema();

                DbAdminModel model = models.FirstOrDefault(model => model.DSName == RdxItem.Year);

                if (model != null)
                    model.AlertType = AlertTypeEnum.danger;

                RdxItem.Progress.Message = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                RdxItem.Progress.State = RdxProgressStatus.ERROR;
            }

            if (RdxItem.Progress.State == RdxProgressStatus.ERROR) return Json(new { Success = false, Message = RdxItem.Progress.Message, AlertType = AlertTypeEnum.danger });
            else return Json(new { Success = true, AlertType = AlertTypeEnum.success });
        }

        [HttpGet]
        public IActionResult Progress()
        {
            MaintenanceProgress progress = new();

            if (RdxItem != null)
            {
                progress.Count = RdxItem.Progress.Percentage();
                progress.Message = RdxItem.Progress.Message;
                progress.ActualScript = RdxItem.Progress.ActualScript ?? "";
                progress.ActualModel = RdxItem.Year;
                progress.Status = RdxItem.Progress.State.ToString();
            }

            return Json(progress);
        }

        [HttpGet]
        public IActionResult CancelReindex(){
            if(reindexCTknSrc == null){
                return Json(new { Sucess = false, Message = Resources.Resources.THE_REINDEX_TASK_IS_22139, AlertType = AlertTypeEnum.info });
            }

            try
            {
                reindexCTknSrc.Cancel();
                return Json(new { Success = true, AlertType = AlertTypeEnum.info });
            }
            catch(Exception e)
            {
                return Json(new { Success = false, Message = e.Message, AlertType = AlertTypeEnum.danger });
            }
        }

        [HttpGet]
        public IActionResult Backup()
        {
            var model = new DbBackupModel();

            model.Load(PersistentSupport.GetDefaultBackupsLocation());

            return Json(model);
        }


        [HttpPost]
        public IActionResult Backup([FromBody]DbBackupModel model)
        {
            try
            {
                model.ResultMsg = string.Empty;

                if (!ModelState.IsValid) {
                    model.AlertType = AlertTypeEnum.danger;
                    throw new BusinessException(Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860, "DbAdminController.Backup", Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860);
                }
                model.AlertType = AlertTypeEnum.success;
                model.ResultMsg = String.Format(Resources.Resources.BACKUP_DA_BASE_DE_DA01918,
                DBMaintenance.BackupDatabase(CurrentYear, model.DbUser, model.DbPsw));
            }
            catch (GenioException e)
            {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + " : " + e.Message;
            }
            catch (Exception e)
            {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
            }

            model.Load(PersistentSupport.GetDefaultBackupsLocation());
            return Json(model);
        }


        [HttpPost]
        public IActionResult DeleteBackup([FromBody] DbBackupModel model)
        {
            model.ResultMsg = string.Empty;
            model.Load(PersistentSupport.GetDefaultBackupsLocation());

            //validate input
            var backup = model.BackupFiles.Find(x => x.Filename == model.BackupItem);
            if (backup != null)
            {
                DBMaintenance.DeleteBackup(Path.Combine(PersistentSupport.GetDefaultBackupsLocation(), model.BackupItem));
                model.BackupFiles.Remove(backup);
            }
            else{
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = Resources.Resources.O_FICHEIRO_INDICADO_18158;
            }

            return Json(model);
        }


        [HttpPost]
        public IActionResult Restore([FromBody] DbBackupModel model)
        {
            model.ResultMsg = string.Empty;
            model.Load(PersistentSupport.GetDefaultBackupsLocation());

            if (model.BackupItem == null || model.BackupItem.Length == 0) {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = Resources.Resources.NENHUM_FICHEIRO_DE_B40914;
            }
            else
            {
                try
                {
                    if (!ModelState.IsValid) {
                        model.AlertType = AlertTypeEnum.danger;
                        throw new BusinessException(Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860, "DbAdminController.Restore", Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860);
                    }
                    string backupsRoot = PersistentSupport.GetDefaultBackupsLocation();
                    DBMaintenance.RestoreDatabase(CurrentYear, model.DbUser, model.DbPsw, backupsRoot, model.BackupItem);

                    model.AlertType = AlertTypeEnum.success;
                    model.ResultMsg = Resources.Resources.BASE_DE_DADOS_RESTAU48748;
                }
                catch (GenioException e)
                {
                    model.AlertType = AlertTypeEnum.danger;
                    model.ResultMsg = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + " : " + e.Message;
                }
                catch (Exception e)
                {
                    model.AlertType = AlertTypeEnum.danger;
                    model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                }
            }

            return Json(model);
        }



        [HttpGet]
        public IActionResult Indexes()
        {
            IndexesItem.ResultMsg = string.Empty;
            return Json(IndexesItem);
        }

        [HttpPost]
        public IActionResult DBIndexesStart([FromBody] IndexModel model)
        {

            var IncoherenceType = model.IndexType;
            IndexModel Active_IndexItem = IndexesItem.Indexes.FirstOrDefault(x => x.IndexType == model.IndexType);

            if (!Active_IndexItem.Active)
            {
                Active_IndexItem.Active = true;
                Active_IndexItem.ProgressPercent = 0;
                Active_IndexItem.ProgressMessage = "";
                Active_IndexItem.Completed = false;
                Active_IndexItem.Year = CurrentYear;

                switch (IncoherenceType)
                {
                    case "Unused":
                        {
                            Active_IndexItem.UnusedIndexesList = new List<UnusedIndexItem>();
                            break;
                        }
                    case "Recommended":
                    default:
                        {
                            Active_IndexItem.RecommendedIndexesList = new List<RecommendedIndexItem>();
                            break;
                        }
                }

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        switch (IncoherenceType)
                        {
                            case "Unused":
                                {
                                    Active_IndexItem.UnusedIndexesList = FindUnusedIndexes((msg, p) => { Active_IndexItem.ProgressMessage = msg; Active_IndexItem.ProgressPercent = p; }, Active_IndexItem.Year);
                                    break;
                                }
                            case "Recommended":
                            default:
                                {
                                    Active_IndexItem.RecommendedIndexesList = FindRecommendedIndexes((msg, p) => { Active_IndexItem.ProgressMessage = msg; Active_IndexItem.ProgressPercent = p; }, Active_IndexItem.Year);
                                    break;
                                }
                        }

                        Active_IndexItem.ProgressMessage = "";
                    }
                    catch (GenioException e)
                    {
                        model.AlertType = AlertTypeEnum.danger;
                        DqItem.ResultMsg = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + ": " + e.Message;
                    }
                    catch (Exception e)
                    {
                        model.AlertType = AlertTypeEnum.danger;
                        DqItem.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                    }
                    Active_IndexItem.LastUpdate = DateTime.Now;
                    Active_IndexItem.Completed = true;
                    Active_IndexItem.ProgressPercent = 100;
                    Active_IndexItem.Active = false;
                });
            }
            return Indexes();
        }

        private static List<UnusedIndexItem> FindUnusedIndexes(Action<string, int> OnProgress, string year)
        {
            List<UnusedIndexItem> res = new List<UnusedIndexItem>();

            try
            {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);

                if (sp.DatabaseType != DatabaseType.SQLSERVER && sp.DatabaseType != DatabaseType.SQLSERVERCOMPAT)
                    throw new FrameworkException("Not supported database type", "Indexes", "Not supported database type");

            //TODO: support for multiple Dbms types
                string sql = @"SELECT
                                (dm_ius.user_seeks + dm_ius.user_scans + dm_ius.user_lookups) as OrderCol,
                                'DROP INDEX '+OBJECT_NAME(dm_ius.object_id)+'.'+i.name AS Drop_Index,
								o.name AS ObjectName
								, i.name AS IndexName
								, dm_ius.user_seeks AS UserSeeks
								, dm_ius.user_scans AS UserScans
								, dm_ius.user_lookups AS UserLookups
								, dm_ius.user_updates AS UserUpdates
								, p.TableRows
								, (select col.name + ' ' FROM sys.index_columns ic
								INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id
								where i.object_id = ic.object_id and i.index_id = ic.index_id
								FOR XML PATH('')
								) as Column_names
								FROM sys.dm_db_index_usage_stats dm_ius
								INNER JOIN sys.indexes i ON i.index_id = dm_ius.index_id
								AND dm_ius.OBJECT_ID = i.OBJECT_ID
								INNER JOIN sys.objects o ON dm_ius.OBJECT_ID = o.OBJECT_ID
								INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
								INNER JOIN (SELECT SUM(p.rows) TableRows, p.index_id, p.OBJECT_ID
								FROM sys.partitions p GROUP BY p.index_id, p.OBJECT_ID) p
								ON p.index_id = dm_ius.index_id AND dm_ius.OBJECT_ID = p.OBJECT_ID
								WHERE OBJECTPROPERTY(dm_ius.OBJECT_ID, 'IsUserTable') = 1
								AND dm_ius.database_id = DB_ID()
								AND i.type_desc = 'nonclustered'
								AND i.is_primary_key = 0
								AND i.is_unique_constraint = 0
								and dm_ius.user_lookups = 0
								AND dm_ius.user_seeks = 0
								AND dm_ius.user_scans = 0
                                --and dm_ius.user_updates <> 0 -- This line excludes indexes SQL Server hasn’t done any work with (no harm for them to exist if they haven't been used yet)
                                --ORDER BY (dm_ius.user_seeks + dm_ius.user_scans + dm_ius.user_lookups) ASC, dm_ius.user_updates DESC
                                ";//Ordenação do lado do cliente

                var dataMatrix = sp.executeQuery(sql);


                for (int i = 0; i < dataMatrix.NumRows; i++)
                {
                    UnusedIndexItem item = new UnusedIndexItem()
                    {

                        OrderCol = dataMatrix.GetNumeric(i, "OrderCol"),
                        Drop_Index = dataMatrix.GetString(i, "Drop_Index"),
                        ObjectName = dataMatrix.GetString(i, "ObjectName"),
                        IndexName = dataMatrix.GetString(i, "IndexName"),
                        UserSeeks = dataMatrix.GetInteger(i, "UserSeeks"),
                        UserScans = dataMatrix.GetInteger(i, "UserScans"),
                        UserLookups = dataMatrix.GetInteger(i, "UserLookups"),
                        UserUpdates = dataMatrix.GetInteger(i, "UserUpdates"),
                        TableRows = dataMatrix.GetInteger(i, "TableRows"),
                        ColumnNames = dataMatrix.GetString(i, "Column_names")
                    };

                    if (OnProgress != null)
                    {
                        string msg = "";
                        msg = string.Format(Resources.Resources.A_PROCESSAR_O_INDICE23295, item.ObjectName.ToUpper(), i.ToString(), dataMatrix.NumRows.ToString());
                        OnProgress(msg, i * 100 / dataMatrix.NumRows);
                    }

                    res.Add(item);
                }
            }
            catch
                {
               return null; //proteger eventuais falhas.
            }

            return res;

        }
        private static List<RecommendedIndexItem> FindRecommendedIndexes(Action<string, int> OnProgress, string year)
        {
            List<RecommendedIndexItem> res = new List<RecommendedIndexItem>();

            try
                    {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);

                if (sp.DatabaseType != DatabaseType.SQLSERVER && sp.DatabaseType != DatabaseType.SQLSERVERCOMPAT)
                    throw new FrameworkException("Not supported database type", "Indexes", "Not supported database type");

                //TODO: support for multiple Dbms types
                string sql = @"SELECT
                            CAST((dm_migs.avg_user_impact*(dm_migs.user_seeks+dm_migs.user_scans)) AS decimal(15,2)) OrderCol,
                            object_name(dm_mid.object_id,dm_mid.database_id) AS [TableName],
                            dm_mid.equality_columns as EqualityColumns,
                            dm_mid.inequality_columns as InequalityColumns,
                            dm_mid.included_columns as IncludedColumns,
                            dm_migs.last_user_seek AS Last_User_Seek,
                            dm_migs.user_seeks as UserSeeks,
                            dm_migs.user_scans as UserScans,
                            CAST(dm_migs.avg_user_impact AS decimal (5,2)) Avg_User_Impact,
                            CAST((dm_migs.avg_user_impact*(dm_migs.user_seeks+dm_migs.user_scans)) AS decimal (15,2)) Avg_Estimated_Impact,
                            'CREATE INDEX [AUTODETECTED_IX_' + object_name(dm_mid.object_id,dm_mid.database_id) + '_'
                            + REPLACE(REPLACE(REPLACE(ISNULL(dm_mid.equality_columns,''),', ','_'),'[',''),']','') +
                            CASE WHEN dm_mid.equality_columns IS NOT NULL AND dm_mid.inequality_columns IS NOT NULL THEN '_' ELSE '' END
                            + REPLACE(REPLACE(REPLACE(ISNULL(dm_mid.inequality_columns,''),', ','_'),'[',''),']','')
                            +CASE WHEN dm_mid.included_columns IS NOT NULL AND dm_mid.included_columns IS NOT NULL THEN '_' ELSE '' END
                            + REPLACE(REPLACE(REPLACE(ISNULL(dm_mid.included_columns,''),', ','_'),'[',''),']','') +
                            + ']'
                            + ' ON ' + dm_mid.statement
                            + ' (' + ISNULL (dm_mid.equality_columns,'')
                            + CASE WHEN dm_mid.equality_columns IS NOT NULL AND dm_mid.inequality_columns IS NOT NULL THEN ',' ELSE '' END
                            + ISNULL (dm_mid.inequality_columns, '')
                            + ')'
                            + ISNULL (' INCLUDE (' + dm_mid.included_columns + ')', '') AS Create_Statement
                            FROM sys.dm_db_missing_index_groups dm_mig
                            INNER JOIN sys.dm_db_missing_index_group_stats dm_migs
                            ON dm_migs.group_handle = dm_mig.index_group_handle
                            INNER JOIN sys.dm_db_missing_index_details dm_mid
                            ON dm_mig.index_handle = dm_mid.index_handle
                            WHERE dm_mid.database_ID = DB_ID()
                            -- ORDER BY Avg_Estimated_Impact DESC
                            "; //Ordenação do lado do cliente

                var dataMatrix = sp.executeQuery(sql);

                for (int i = 0; i < dataMatrix.NumRows; i++)
                {

                    /*
                     The advice from Microsoft is to follow these guidelines:
                         a) List the equality columns first (leftmost in the column list).
                         b) List the inequality columns after the equality columns (to the right of equality columns listed).
                         c) List the include columns in the INCLUDE clause of the CREATE INDEX statement.
                         d) To determine an effective order for the equality columns, order them based on their selectivity; that is, list the most selective columns first.
                    */
                    RecommendedIndexItem item = new RecommendedIndexItem()
                    {

                        OrderCol = (decimal)dataMatrix.GetNumeric(i, "OrderCol"),
                        TableName = dataMatrix.GetString(i, "TableName"),
                        EqualityColumns = dataMatrix.GetString(i, "EqualityColumns"),
                        InequalityColumns = dataMatrix.GetString(i, "InequalityColumns"),
                        IncludedColumns = dataMatrix.GetString(i, "IncludedColumns"),
                        Last_User_Seek = dataMatrix.GetDate(i, "Last_User_Seek"),
                        UserSeeks = dataMatrix.GetInteger(i, "UserSeeks"),
                        UserScans = dataMatrix.GetInteger(i, "UserScans"),
                        Avg_User_Impact = (decimal)dataMatrix.GetNumeric(i, "Avg_User_Impact"),
                        Avg_Estimated_Impact = (decimal)dataMatrix.GetNumeric(i, "Avg_Estimated_Impact"),
                        Create_Statement = dataMatrix.GetString(i, "Create_Statement"),
                    };

                    if (OnProgress != null)
                    {
                        string msg = "";

                        msg = string.Format(Resources.Resources.A_PROCESSAR_O_INDICE23295, item.TableName.ToUpper(), i.ToString(), dataMatrix.NumRows.ToString());
                        OnProgress(msg, i * 100 / dataMatrix.NumRows);
                }

                    res.Add(item);
            }
            }
            catch
            {
                return null; //proteger eventuais falhas.
            }

            return res;
        }


        class AreaPathways
        {
            public Tuple<Relation, Relation> End_to_end_relation;
            public List<Relation> Relation_path_list;
        }
        class AreasPathwaysList
        {
            public List<AreaPathways> AreaPathwaysList;
        }

        public static List<Relation> GetRelationListBetweenAreas(string barea, string oarea)
        {
            List<Relation> relations = new List<Relation>();
            AreaInfo area_areaInfo = Area.GetInfoArea(barea);
            if (area_areaInfo.Pathways != null)
            {
                string area_link = area_areaInfo.Pathways[oarea];
                if (area_areaInfo.Pathways.ContainsKey(oarea))
                    if (oarea == area_link)
                    {
                        Relation oarea_Relation = area_areaInfo.ParentTables[oarea];
                        relations.Add(oarea_Relation);
                    }
                    else
                    {
                        List<Relation> other_relations = null;
                        if (area_areaInfo.Pathways.ContainsKey(area_link))
                            other_relations = GetRelationListBetweenAreas(barea, area_link);
                        if (other_relations != null)
                            relations.AddRange(other_relations);

                        other_relations = GetRelationListBetweenAreas(area_link, oarea);
                        if (other_relations != null)
                            relations.AddRange(other_relations);
                    }

                //caso n seja direto (nunca ocorreu esta situação, mas just in case):
                if (relations.Count == 0)
                    foreach (var target in area_areaInfo.Pathways)
            {
                        relations.AddRange(GetRelationListBetweenAreas(target.Key, oarea));
                    }
            }
            return relations;
        }

                //caminhos directos
        public static void GetAreaDualRelationPathways(AreaInfo info, bool UseViews)
        {
            if (info.Pathways != null)
                foreach (var target in info.Pathways)
                    if (target.Key == target.Value)
                        {
                            //ver se exists outra table relacionada com esta que também permita chegar ao target por uma relação directa
                            foreach (string outra in info.Pathways.Keys)
                            {
                                AreaInfo infoOutra = Area.GetInfoArea(outra);
                            if (infoOutra.Pathways != null && infoOutra.Pathways.ContainsKey(target.Value) && infoOutra.Pathways[target.Value] == target.Value)
                                {
                                        //então estas duas relações têm de ter o mesmo Qvalue
                                        Relation rbase = info.ParentTables[target.Value];
                                        Relation routra = infoOutra.ParentTables[target.Value];

                                    if (!UseViews && (info.PersistenceType == CSGenio.business.PersistenceType.View || infoOutra.PersistenceType == CSGenio.business.PersistenceType.View))
                                        continue;

                                        //só queremos verificar as relações fisicas, se a mesma relação exists entre outras areas do mesmo dominio seria a mesma validação
                                    if (!_relacoesCoerentes.AreaPathwaysList.Exists(x => x.End_to_end_relation.Item1.SourceTable == rbase.SourceTable && x.End_to_end_relation.Item1.SourceRelField == rbase.SourceRelField
                                      && x.End_to_end_relation.Item2.SourceTable == routra.SourceTable && x.End_to_end_relation.Item2.SourceRelField == routra.SourceRelField))
                                    {
                                        AreaPathways areaPathways = new AreaPathways();

                                        areaPathways.End_to_end_relation = new Tuple<Relation, Relation>(rbase, routra);
                                        List<Relation> Relation_path_list = new List<Relation>();
                                        //Falta verificar e acrescentar que relações intermédias existem para chegar da área base à outra área que tem relação com o destino
                                        Relation_path_list.AddRange(GetRelationListBetweenAreas(rbase.AliasSourceTab, routra.AliasSourceTab));


                                        areaPathways.Relation_path_list = Relation_path_list;
                                        _relacoesCoerentes.AreaPathwaysList.Add(areaPathways);
                                    }
                                }
                                    }
                                }
                            }


        private static AreasPathwaysList CacheValidDualRelations(bool UseViews)
        {
            if (_relacoesCoerentes.AreaPathwaysList != null && _relacoesCoerentes.AreaPathwaysList.Count > 0)
                return _relacoesCoerentes;

            _relacoesCoerentes = new AreasPathwaysList() { AreaPathwaysList = new List<AreaPathways>() };

            foreach (string area in Area.ListaAreas)
            {
                AreaInfo info = Area.GetInfoArea(area);
                if (info.QSystem != CSGenio.framework.Configuration.Program)
                    continue;
                GetAreaDualRelationPathways(info, UseViews);
            }

            return _relacoesCoerentes;
        }


        [HttpGet]
        public IActionResult DataQuality()
        {
            DqItem.ResultMsg = string.Empty;
            return Json(DqItem);
        }

        [HttpPost]
        public IActionResult DataQualityStart([FromBody] IncoherencyModel model)
        {
            var IncoherenceType = model.IncoherenceType;
            IncoherencyModel Active_DqItem = DqItem.Incoherencies.FirstOrDefault(x => x.IncoherenceType == model.IncoherenceType);

            if (!Active_DqItem.Active)
            {
                Active_DqItem.Active = true;
                Active_DqItem.ProgressPercent = 0;
                Active_DqItem.ProgressMessage = "";
                Active_DqItem.Completed = false;
                Active_DqItem.Year = CurrentYear;

                Active_DqItem.RelationMode = model.RelationMode;
                Active_DqItem.NullsIsChecked = model.NullsIsChecked;
                Active_DqItem.ViewsIsChecked = model.ViewsIsChecked;

                switch (IncoherenceType)
        {
                    case "OrphanRelation":
                        {
                            Active_DqItem.OrphanRelations = new List<OrphanRelation>();
                            break;
                        }
                    case "IncoherentRelation":
                    default:
            {
                            Active_DqItem.IncoherentRelations = new List<IncoherentRelationItem>();
                            break;
                        }
                }

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        switch (IncoherenceType)
                        {
                            case "OrphanRelation":
                                {
                                    Active_DqItem.OrphanRelations = FindOrphanRelations((msg, p) => { Active_DqItem.ProgressMessage = msg; Active_DqItem.ProgressPercent = p; }, Active_DqItem.Year, Active_DqItem.ViewsIsChecked);
                                    break;
                                }
                            case "IncoherentRelation":
                            default:
                                {
                                    Active_DqItem.IncoherentRelations = FindIncoeherentRelations((msg, p) => { Active_DqItem.ProgressMessage = msg; Active_DqItem.ProgressPercent = p; }, Active_DqItem.Year, Active_DqItem.ViewsIsChecked, Active_DqItem.NullsIsChecked, Active_DqItem.RelationMode);
                                    break;
                                }
                        }

                        Active_DqItem.ProgressMessage = "";
                    }
                    catch (GenioException e)
                    {
                        model.AlertType = AlertTypeEnum.danger;
                        DqItem.ResultMsg = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + ": " + e.Message;
                    }
                    catch (Exception e)
                    {
                        model.AlertType = AlertTypeEnum.danger;
                        DqItem.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                    }
                    Active_DqItem.LastUpdate = DateTime.Now;
                    Active_DqItem.Completed = true;
                    Active_DqItem.ProgressPercent = 100;
                    Active_DqItem.Active = false;
                });
            }
            return DataQuality();
        }

        [HttpGet]
        public IActionResult IndexesProgress()
        {
            var num = FromQuery("num");
            int ProgressPercent = 100;
            string ProgressMessage = "";
            bool isActive = false;

            IndexModel Active_IndexItem = IndexesItem.Indexes.ElementAt(int.Parse(num));
            if (Active_IndexItem != null)
            {
                ProgressPercent = Active_IndexItem.ProgressPercent;
                ProgressMessage = Active_IndexItem.ProgressMessage;
                isActive = Active_IndexItem.Active;
            }
            var Result = new
            {
                Count = ProgressPercent,
                Message = ProgressMessage,
                isActive
            };

            return Json(Result);
        }

        [HttpGet]
        public IActionResult DataQualityProgress()
        {
            var num = FromQuery("num");
            int ProgressPercent = 100;
            string ProgressMessage = "";
            bool isActive = false;

            IncoherencyModel Active_DqItem = DqItem.Incoherencies.ElementAt(int.Parse(num));
            if (Active_DqItem != null)
            {
                ProgressPercent = Active_DqItem.ProgressPercent;
                ProgressMessage = Active_DqItem.ProgressMessage;
                isActive = Active_DqItem.Active;
            }
            var Result = new
            {
                Count = ProgressPercent,
                Message = ProgressMessage,
                isActive
            };

            return Json(Result);
        }

		private static List<IncoherentRelationItem> FindIncoeherentRelations(Action<string, int> OnProgress, string year, bool UseViews, bool UseNulls, HardCodedLists.RelationsMode ModeRelations)
        {
            List<IncoherentRelationItem> res = new List<IncoherentRelationItem>();

            AreasPathwaysList dualRelations = CacheValidDualRelations(UseViews);

            var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);
            sp.Timeout = 300;
            int ix = 0;
            int total = 0;

            foreach (var dualRelation in dualRelations.AreaPathwaysList)
            {
                switch (ModeRelations)
                {
                    case HardCodedLists.RelationsMode.DIRETAS:
                        {
                            if (dualRelation.Relation_path_list.Count > 1) //ignora as que não têm relação direta (apenas 1 tabela intermédia)
                                continue;
                        }
                        break;
                    case HardCodedLists.RelationsMode.INDIRETAS:
                        {
                            if (dualRelation.Relation_path_list.Count == 1)
                                continue;
                        }
                        break;
                    case HardCodedLists.RelationsMode.AMBAS:
                        break;
                    default:
                        break;
                }


                //get areas Info
                var area_table1 = Area.GetInfoArea(dualRelation.End_to_end_relation.Item1.AliasSourceTab);
                var area_table2 = Area.GetInfoArea(dualRelation.End_to_end_relation.Item2.AliasSourceTab);
                var area_destn = Area.GetInfoArea(dualRelation.End_to_end_relation.Item2.AliasTargetTab);

                //get str alias so its easier to read
                string table1_destnfield = dualRelation.End_to_end_relation.Item1.SourceRelField;
                string table2_destnfield = dualRelation.End_to_end_relation.Item2.SourceRelField;

                //ignore views option
                if (!UseViews && (area_table1.PersistenceType == CSGenio.business.PersistenceType.View || area_table2.PersistenceType == CSGenio.business.PersistenceType.View || area_destn.PersistenceType == CSGenio.business.PersistenceType.View))
                    continue;

                try
            {
                    Relation table1_destn = dualRelation.End_to_end_relation.Item1;
                    Relation table2_destn = dualRelation.End_to_end_relation.Item2;

                    string Path_str = table1_destn.AliasSourceTab.ToUpper();
                    SelectQuery sql_details = new SelectQuery()// sql_details
                    .Select(area_table1.Alias, area_table1.PrimaryKeyName)
                    .Select(area_table1.Alias, table1_destnfield)

                    //a partir da tabela de origem
                    .From(area_table1.QSystem, area_table1.TableName, area_table1.Alias);
                    //passando OBRIGATORIAMENTE (INNER JOIN) pelas tabelas intermédias alternativas à relação direta calculada
                    for (int i = 0; i < dualRelation.Relation_path_list.Count; i++)
                    {
                        var relation = dualRelation.Relation_path_list.ElementAt(i);
                        sql_details.Select(relation.AliasSourceTab, relation.SourceRelField);
                        sql_details.Select(relation.AliasTargetTab, relation.TargetRelField);
                        sql_details.Join(relation.TargetTable, relation.AliasTargetTab, TableJoinType.Inner).On(CriteriaSet.And().Equal(relation.AliasSourceTab, relation.SourceRelField, relation.AliasTargetTab, relation.TargetRelField));
                        if (i != dualRelation.Relation_path_list.Count - 1)
                            Path_str += " > " + relation.AliasTargetTab;
                        else
                            Path_str += " > " + relation.AliasTargetTab.ToUpper(); //para marcar visualmente em Maiúsculas as tabelas que realmente têm direção direta com a tabela de destino
                    }
                    //até ao destino
                    Path_str += " > " + area_destn.Alias.ToUpper();

                    //para ficar com os selects pela ordem desejada (no seguimento do modelo)
                    //sql_details.Select(area_table2.Alias, area_table2.PrimaryKeyName);
                    sql_details.Select(area_table2.Alias, table2_destn.SourceRelField);
                    //não necessita disto, já tem os dados necessários: sql_details.Join(area_destn.TableName, area_destn.Alias, TableJoinType.Left).On(CriteriaSet.And().Equal(table2_destn.AliasSourceTab, table2_destn.SourceRelField, table2_destn.AliasTargetTab, table2_destn.TargetRelField));

                    //onde as chaves de ligação ao destino sejam diferentes. Com uso de chaves a null permitirá que uma das chaves seja nula (mas a outra não).
                    if (UseNulls)
                    {
                        sql_details.Where(
                            CriteriaSet.Or().NotEqual(table1_destn.AliasSourceTab, table1_destn.SourceRelField, area_table2.Alias, table2_destn.SourceRelField)
                                .SubSet(
                                    CriteriaSet.Or()
                                    .SubSet(
                                        CriteriaSet.And()
                                        .Equal(area_table1.Alias, table1_destn.SourceRelField, null)
                                        .NotEqual(area_table2.Alias, table2_destn.SourceRelField, null)
                                    )
                                    .SubSet(
                                        CriteriaSet.And()
                                        .Equal(area_table2.Alias, table2_destn.SourceRelField, null)
                                        .NotEqual(area_table1.Alias, table1_destn.SourceRelField, null)
                                    )
                            )
                        );
                    }
                    else
                    {
                        sql_details.Where(CriteriaSet.Or().NotEqual(table1_destn.AliasSourceTab, table1_destn.SourceRelField, area_table2.Alias, table2_destn.SourceRelField));
                    }

                    sql_details.noLock = true;

                    var renderer = new QueryRenderer(sp);
                    renderer.SchemaMapping = sp.SchemaMapping;
                    string sql_details_str = renderer.GetSql(sql_details); //Simulates and saves the query that collects all the cases for this inconsistence

                    //Query that will group the cases (this one will be ran against the DB)
                    SelectQuery sql = new SelectQuery()
                    .Select(SqlFunctions.Count("*"), "COUNT")
                    .From(sql_details, "sql_details");

                    int CountIR = CSGenio.persistence.DBConversion.ToInteger(sp.executeScalar(sql)); //Inconsistence type counter
                    total += CountIR;
                    if (OnProgress != null)
                    {
                        string msg = "";
                        if (CountIR > 0)
                        {
                            msg = string.Format(Resources.Resources.DETECTADAS__0__INCOE62559, CountIR.ToString(), area_table1.Alias.ToUpper(), area_table2.Alias.ToUpper(), dualRelation.End_to_end_relation.Item2.AliasTargetTab.ToUpper(), ix, dualRelations.AreaPathwaysList.Count, res.Count.ToString(), total.ToString());
                            OnProgress(msg, ix * 100 / dualRelations.AreaPathwaysList.Count);
                        }
                    }
                    ix++;
                    if (CountIR > 0)
                {
                    var item = new IncoherentRelationItem()
                    {
                            Table1 = area_table1.Alias.ToUpper(),
                            Fk1 = table1_destnfield,
                            Table2 = area_table2.Alias.ToUpper(),
                            Fk2 = table2_destnfield,
                            Destination = area_destn.Alias.ToUpper() + "." + area_destn.PrimaryKeyName,
                            CountIR = CountIR,
                            Path = Path_str,
                            Sql = sql_details_str
                    };
                    res.Add(item);
            }
                }
                catch
                {
                    continue; //proteger eventuais falhas.
                }
            }
            return res;
        }

        private static List<OrphanRelation> FindOrphanRelations(Action<string, int> OnProgress, string year, bool UseViews)
        {
            List<OrphanRelation> res = new List<OrphanRelation>();

            var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);
            sp.Timeout = 300;
            int ix = 0;
            int total = 0;

            foreach (var area in Area.ListaAreas)
            {
                AreaInfo area_table1 = Area.GetInfoArea(area);
                if (area_table1.QSystem != CSGenio.framework.Configuration.Program)
                    continue;

                //get areas Info
                foreach (var destn in area_table1.ParentTables)
                {
                    AreaInfo area_destn;
                    try
                    {
                        area_destn = Area.GetInfoArea(destn.Key);
                    }
                    catch
                    {
                        continue; //a área não existe, poderá tratar-se de uma réplica
                    }

                    //get str alias so its easier to read
                    string table1_destnfield = destn.Value.SourceRelField;

                    //ignore views option
                    if (!UseViews && (area_table1.PersistenceType == CSGenio.business.PersistenceType.View || area_destn.PersistenceType == CSGenio.business.PersistenceType.View))
                        continue;

                    try
                    {
                        Relation table1_destn = destn.Value;

                        string Path_str = table1_destn.AliasSourceTab.ToUpper();

                        //subquery where not exists primarykey on destn:
                        SelectQuery not_exists_sql = new SelectQuery().Select(area_table1.Alias, table1_destnfield).From(area_destn.QSystem, area_destn.TableName, area_destn.Alias)
                                .Where(CriteriaSet.And().Equal(area_destn.Alias, table1_destn.TargetRelField, area_table1.Alias, table1_destnfield));
                        not_exists_sql.noLock = true;
                        not_exists_sql.PageSize(1);

                        SelectQuery sql_details = new SelectQuery()// sql_details
                        .Select(area_table1.Alias, area_table1.PrimaryKeyName)
                        .Select(area_table1.Alias, table1_destnfield)

                        //a partir da tabela de origem
                        .From(area_table1.QSystem, area_table1.TableName, area_table1.Alias)
                        .Join(area_destn.QSystem, area_destn.TableName, area_destn.Alias, TableJoinType.Left)
                            .On(CriteriaSet.And().Equal(area_destn.Alias, table1_destn.TargetRelField, area_table1.Alias, table1_destnfield))
                            //onde as chaves de ligação ao destino existam mas estejam vazias no destino
                            .Where(CriteriaSet.And()
                                .Equal(SqlFunctions.Custom("EmptyG", new ColumnReference(area_table1.Alias, table1_destn.SourceRelField)), 0)
                                .Equal(SqlFunctions.Custom("EmptyG", new ColumnReference(area_destn.Alias, table1_destn.TargetRelField)), 1)
                                );

                        sql_details.noLock = true;

                        var renderer = new QueryRenderer(sp);
                        renderer.SchemaMapping = sp.SchemaMapping;
                        string sql_details_str = renderer.GetSql(sql_details); //Simulates and saves the query that collects all the cases for this inconsistence
                        //replace params so that the query can be ran by end user
                        sql_details_str = sql_details_str.Replace("@param1", "0").Replace("@param2", "1");

                        //Query that will group the cases (this one will be ran against the DB)
                        SelectQuery sql = new SelectQuery()
                        .Select(SqlFunctions.Count("*"), "COUNT")
                        .From(sql_details, "sql_details");

                        int CountIR = CSGenio.persistence.DBConversion.ToInteger(sp.executeScalar(sql)); //Inconsistence type counter
                        total += CountIR;
                        if (OnProgress != null)
                        {
                            string msg = "";
                            if (CountIR > 0)
                            {
                                msg = string.Format(Resources.Resources.DETECTADOS__0__REGIS59482, CountIR.ToString(), area_table1.Alias.ToUpper(), table1_destn.AliasTargetTab.ToUpper(), ix, Area.ListaAreas.Count, res.Count.ToString(), total.ToString());
                                OnProgress(msg, ix * 100 / Area.ListaAreas.Count);
                            }
                        }

                        if (CountIR > 0)
                        {
                            var item = new OrphanRelation()
                            {
                                Table1 = area_table1.Alias.ToUpper(),
                                Fk1 = table1_destnfield,
                                Destination = area_destn.Alias.ToUpper() + "." + area_destn.PrimaryKeyName,
                                CountOrphans = CountIR,
                                Sql = sql_details_str
                            };
                            res.Add(item);
                        }
                }
                catch
                {
                    continue; //proteger eventuais falhas.
                }
                }
                ix++;
            }
            return res;
        }

        public class ProgressBarStatus
        {
            private string _text;
            public string Text { get { var aux = _text; if (!InProcess) { _text = string.Empty; } return aux; } set { _text = value; } }
            public decimal Percent { get; set; }
            public bool InProcess { get; set; }
            public string EndMsg { get; set; }
        }

        private static ProgressBarStatus _changeYearProgressBarStatus;
        public static ProgressBarStatus ChangeYearProgressBar
        {
            get
            {
                if (_changeYearProgressBarStatus == null)
                {
                    _changeYearProgressBarStatus = new ProgressBarStatus();
                }
                return _changeYearProgressBarStatus;
            }
        }

        /// <summary>
        /// Metodo que permite por AJAX validar o estado da mudança de Qyear
        /// </summary>
        /// <returns>ProgressBarStatus</returns>
        [HttpGet]
        public IActionResult CheckChangeYearProgress()
        {
            return Json(ChangeYearProgressBar);
        }

        [HttpGet]
        public IActionResult ChangeYear()
        {
            #region Innicialização das variaveis
            var model = new ChangeYearModel();

            var conf = configManager.GetExistingConfig();

            model.Year = (DateTime.Now.Year + 1).ToString();
            model.Years = CSGenio.framework.Configuration.Years.Select(y => new SelectListItem() { Text = y, Value = y, Selected = (y == conf.anoDefault) });

            // Validação do file de Configurações
            if (conf.DataSystems.Count == 0)
            {
                ModelState.AddModelError("DbAdminController.ChangeYear", Resources.Resources.FICHEIRO_DE_CONFIGUR13972);
                return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
            }

            DataSystemXml dataSystem = null;
            // Default datasystem
            if (!string.IsNullOrEmpty(conf.anoDefault))
                dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == conf.anoDefault);
            else
                dataSystem = conf.DataSystems.FirstOrDefault();

            if (dataSystem == null)
            {
                ModelState.AddModelError("GET DbAdminController.ChangeYear", Resources.Resources.FICHEIRO_DE_CONFIGUR13972);
                return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
            }
            #endregion

            if (conf.DataSystems.Count == 0)
            {
                ModelState.AddModelError("DbAdminController.ChangeYear", Resources.Resources.FICHEIRO_DE_CONFIGUR13972);
                return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
            }

            model.NewDBSchema = string.Format("{0}{1}", CSGenio.framework.Configuration.Program, model.Year);
            // read DB Size
            var DBSize = AuxFunctions.GetDBSize(dataSystem.Name, dataSystem.Schemas.FirstOrDefault().Schema);

            // Calculate suggested timeout. 1* for each 10GB
            int mult = ((int)DBSize / (10 * 1024)) + 1;
            model.Timeout = 300 * mult;
            model.CriarBD = true;

            return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
        }

        [HttpPost]
        public IActionResult ChangeYear([FromBody] ChangeYearModel model)
        {
            var conf = configManager.GetExistingConfig();

            model.Years = CSGenio.framework.Configuration.Years.Select(y => new SelectListItem() { Text = y, Value = y, Selected = (y == conf.anoDefault) });// Voltar inicializar o array to não ter erros de renderização.
            if (!ModelState.IsValid || ChangeYearProgressBar.InProcess)
                return Json(new { Model = model, Errors = GetModelStateErrors() });
            // 1) Verificar se é preciso de reindexar a base de dados (versões, códigos, etc.)
            // 2) Criar BD (com name que o user indicou -> verificar se é valido)
            // 3) Executar as operações de mudança de ano
            // 4) Forçar a reindexação (só as formulas)
            // 5) Criar os triggers caso se o cliente tiver logs ativos

            // TODO:
            // + Rever o progress bar
            // + Validate se é preciso reindexar a BD
            // + Implementar as "tabelas de historico"
            // + Melhorar estrutura do códigos (listas dos ficheiros sql to executar na criação da BD e Recalculo das formulas)

            #region Innicialização das variaveis
            ChangeYearProgressBar.InProcess = true;
            ChangeYearProgressBar.Percent = 0;
            ChangeYearProgressBar.Text = "Starting ...";

            // Validação do file de Configurações
            if (conf.DataSystems.Count == 0)
            {
                ModelState.AddModelError("DbAdminController.ChangeYear", Resources.Resources.FICHEIRO_DE_CONFIGUR13972);
                ChangeYearProgressBar.InProcess = false;
                return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
            }// Validate se já exists configuração to o Qyear / schema
            else if (model.CriarBD && conf.DataSystems.Any(ds => ds.Name == model.Year || ds.Schemas.Any(s => s.Schema == model.NewDBSchema)))
            {
                ModelState.AddModelError("DbAdminController.ChangeYear", Resources.Resources.JA_EXISTE_A_BASE_DE_59437);
                ChangeYearProgressBar.InProcess = false;
                return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
            }

            DataSystemXml curDataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == model.SrcYear);
            var curDataSchema = curDataSystem.Schemas.FirstOrDefault();

            DataSystemXml dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == model.Year); ;
            if (model.CriarBD || dataSystem == null)
            {
                dataSystem = new DataSystemXml()
                {
                    Type = curDataSystem.Type,
                    Server = curDataSystem.Server,
                    Login = curDataSystem.Login,
                    Password = curDataSystem.Password,
                    Schemas = new List<DataXml>()
                    {
                        new DataXml()
                        {
                            Id = curDataSchema.Id,
                            Schema = model.NewDBSchema
                        }
                    },
                    Name = model.Year
                };

                // Adicionar o novo DataSystems
                conf.DataSystems.Add(dataSystem);
                configManager.StoreConfig(conf);
            }

            // Reload Configuration static instance in server with the new configuration data
            CSGenio.framework.Configuration.ReadConfiguration(conf);
            var dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);
            var reindexOrder = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory).GetReindexScripts();
            var currentCulture = CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper();
            #endregion

            #region Mudança de Qyear
            var RdxUpgradeSchema = new RdxParamUpgradeSchema();
            RdxUpgradeSchema.Year = model.Year;
            RdxUpgradeSchema.Username = model.DbUser;
            RdxUpgradeSchema.Password = model.DbPsw;
            RdxUpgradeSchema.Path = dbMaintenance.GetReindexPath();
            RdxUpgradeSchema.DirFilestream = model.DirFilestream;
            RdxUpgradeSchema.Zero = true;// zero deve ser escolhido pelo user ?
			RdxUpgradeSchema.Origin = "Change year";

            Task.Factory.StartNew(() =>
            {
                try
                {
                    ChangedEventHandler changedExecutionScript = null;
                    // 2) Execução dos scripts da Criação de BD Schema
                    if (model.CriarBD)
                    {
                        var firstFaseRdx = new List<string>() { "CREATEDB", "CREATESP", "CREATESCHEMA", "CREATEHRDSCHEMA", "UPDATEFUNCTIONS", "TBLREBUILD", "UPDATECFG", "UPDATESP", "ADDINDEX" };
                        reindexOrder.ReIndexItems.ForEach(rdxf => rdxf.Selected = firstFaseRdx.Contains(rdxf.Id));
                        reindexOrder.timeout = model.Timeout;
                        reindexOrder.CalculateOrder();
                        RdxUpgradeSchema.OrderExec = reindexOrder.GetOrderToExecute();
                        changedExecutionScript += (sender, eventArgs, status) =>
                        {
                            if (status.State == RdxProgressStatus.SUCCESS)
                            {
                                ChangeYearProgressBar.Text = "";
                                ChangeYearProgressBar.Percent = 20;
                            }
                            else if (status.State == RdxProgressStatus.ERROR)
                            {
                                throw new OperationCanceledException($"{status.Message} - ({status.ActualScript})");
                            }
                            else
                            {
                                ChangeYearProgressBar.Text = string.Format("Creating Schema: {0}", status.ActualScript);
                                ChangeYearProgressBar.Percent = Convert.ToInt32(status.Percentage() * 0.2);
                            }
                        };
                        RdxUpgradeSchema.ChangedExecutionScript += changedExecutionScript;
                        PersistentSupport.upgradeSchema(RdxUpgradeSchema, dataSystem);
                        RdxUpgradeSchema.ChangedExecutionScript -= changedExecutionScript;
                    }
                    ChangeYearProgressBar.Percent = 20;

                    // Código manual (BEFORE_MDANO)
                    ChangeYearProgressBar.Text = "Executing before change year manual code";
                    BeforeChangeYear(curDataSystem, dataSystem);

                    // 3) Execução do script da migração dos dados
                    ChangeYearProgressBar.Text = "Data migration: ChangeYear.sql";
                    RdxUpgradeSchema.OrderExec = new List<RdxScript>() { new RdxScript() { Connection = ConnectionType.Normal, Script = "ChangeYear.sql", Timeout = model.Timeout } };
                    PersistentSupport.upgradeSchema(RdxUpgradeSchema, dataSystem, curDataSchema.Schema);
                    ChangeYearProgressBar.Percent = 70;

                    // 4) Execução dos scripts do Recalculo das formulas
                    var recalcFormulas = new List<string>() { "CREATEFORMULASPROCS", "UPDATEREPLICAS", "RESETCALCFIELDS", "UPDATEFORMULAFIELDS", "FORMULASDAILYUPDATE", "UPDATEINTCOD" };
                    reindexOrder.ReIndexItems.ForEach(rdxf => rdxf.Selected = recalcFormulas.Contains(rdxf.Id));
                    reindexOrder.timeout = model.Timeout;
                    reindexOrder.CalculateOrder();
                    RdxUpgradeSchema.OrderExec = reindexOrder.GetOrderToExecute();
                    changedExecutionScript = null;
                    changedExecutionScript += (sender, eventArgs, status) =>
                    {
                        if (status.State == RdxProgressStatus.SUCCESS)
                        {
                            ChangeYearProgressBar.Text = "";
                            ChangeYearProgressBar.Percent = 90;
                        }
                        else if (status.State == RdxProgressStatus.ERROR)
                        {
                            throw new OperationCanceledException($"{status.Message} - ({status.ActualScript})");
                        }
                        else
                        {
                            ChangeYearProgressBar.Text = string.Format("Recalc formulas: {0}", status.ActualScript);
                            ChangeYearProgressBar.Percent += Convert.ToInt32(status.Percentage() * 0.2);
                        }
                    };
                    RdxUpgradeSchema.ChangedExecutionScript += changedExecutionScript;
                    PersistentSupport.upgradeSchema(RdxUpgradeSchema, dataSystem);
                    RdxUpgradeSchema.ChangedExecutionScript -= changedExecutionScript;
                    ChangeYearProgressBar.Percent = 90;

                    // Código manual (AFTER_MDANO)
                    ChangeYearProgressBar.Text = "Executing after change year manual code";
                    AfterChangeYear(curDataSystem, dataSystem);
                    ChangeYearProgressBar.Percent = 95;

                    model.AlertType = AlertTypeEnum.success;
                    ChangeYearProgressBar.Text = ChangeYearProgressBar.EndMsg = Resources.Resources.MUDANCA_DE_ANO_CONCL59631;
                    ChangeYearProgressBar.Percent = 100;
                }
                catch (GenioException e)
                {
                    model.AlertType = AlertTypeEnum.danger;
                    ChangeYearProgressBar.EndMsg = string.Format("Error. {0}", Translations.Get(e.UserMessage, currentCulture) + ": " + e.Message);
                }
                catch (Exception e)
                {
                    model.AlertType = AlertTypeEnum.danger;
                    ChangeYearProgressBar.EndMsg = string.Format("Error. {0}", Translations.Get(e.Message, currentCulture));
                }
                ChangeYearProgressBar.InProcess = false;
            });
            #endregion

            return Json(new { Model = model, Errors = GetModelStateErrors(), AlertType = AlertTypeEnum.danger });
        }

        private void BeforeChangeYear(DataSystemXml srcDataSystem, DataSystemXml destDataSystem)
        {
            // USE [MANUAL FOR MANADMCS BEFORE_MDANO]
        }

        private void AfterChangeYear(DataSystemXml srcDataSystem, DataSystemXml destDataSystem)
        {
            // USE [MANUAL FOR MANADMCS AFTER_MDANO]
        }

        [HttpGet]
        public IActionResult Security()
        {
            var model = new DbSecurityModel();
            model.MasterKey = true;

            return Json(model);
        }

        [HttpPost]
        public IActionResult SaveTDE([FromBody] DbSecurityModel model)
        {
            model.ResultMsg = "";

            try
            {
                //TODO: This needs to be an overridable function of PersistentSupport so it can be coded differently according to database type.
                // In alternative it can be a script
                CSGenio.framework.Configuration.Reload();
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CurrentYear);
                var dataSystem = CSGenio.framework.Configuration.ResolveDataSystem(CurrentYear, CSGenio.framework.Configuration.DbTypes.NORMAL);
                var database = dataSystem.Schemas[0].Schema;

                //*** execute encryption ***
                string sql = "";
                if (model.MasterKey)
                {
                    if (model.MasterPsw == null || model.MasterPsw.Trim() == "")
                    {
                        model.AlertType = AlertTypeEnum.danger;
                        model.ResultMsg = Resources.Resources.E_OBRIGATORIO_O_PREE06980;
                        return Json(model);
                    }

                    //Create Master Key
                    sql = String.Format(@"USE MASTER; CREATE MASTER KEY ENCRYPTION BY PASSWORD='{0}';", model.MasterPsw);
                    sp.executeQuery(sql);
                }

                //Create Certificate protected by master key
                sql = String.Format(@"USE MASTER; CREATE CERTIFICATE {0}_Cert WITH SUBJECT='Database_Encryption'; ", database);
                sp.executeQuery(sql);

                //Create Database Encryption Key
                sql = String.Format(@"USE {0}; CREATE DATABASE ENCRYPTION KEY WITH ALGORITHM = {1} ENCRYPTION BY SERVER CERTIFICATE {0}_Cert;", database, model.Encryption);
                sp.executeQuery(sql);

                //Enable Encryption
                sql = String.Format(@"ALTER DATABASE {0} SET ENCRYPTION ON;", database);
                sp.executeQuery(sql);

            }
            catch (GenioException ex)
            {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = ex.Message;
            }

            if (model.ResultMsg == null || model.ResultMsg == "") {
                model.AlertType = AlertTypeEnum.success;
                model.ResultMsg = Resources.Resources.BASE_DE_DADOS_COM_EN26288;
            }
            return Json(model);
        }

        [HttpPost]
        public IActionResult CheckStatusTDE([FromBody] DbSecurityModel model)
        {
            model.ResultMsg = string.Empty;
            if (String.IsNullOrEmpty(model.DbUser))
            {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = Resources.Resources.O_USERNAME_NAO_PODE_14383;
                return Json(model);
            }
            else if (String.IsNullOrEmpty(model.DbPsw))
            {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = Resources.Resources.A_PASSWORD_NAO_PODE_51074;
                return Json(model);
            }

            var conf = configManager.GetExistingConfig();

            try
            {
                //testar ligação à BD
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(CurrentYear);
                sp.openConnection();
                var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear); // Default == null

                string sql = "";

                //verificar existencia de master key criada
                sql = @"USE MASTER; SELECT NAME FROM sys.symmetric_keys WHERE symmetric_key_id = 101;";
                if (CSGenio.persistence.DBConversion.ToString(sp.executeScalar(sql)) == "")
                {
                    model.AlertType = AlertTypeEnum.danger;
                    model.ResultMsg = Resources.Resources.NAO_ESTA_CRIADA_A_CH54248;
                    sp.closeConnection();
                    return Json(model);
                }

                //confirmar se está definida que a BD tem encriptação activa
                //poderá vir a ser confirmado também o argumento dm.encryption_state
                //https://docs.microsoft.com/en-us/sql/relational-databases/system-dynamic-management-views/sys-dm-database-encryption-keys-transact-sql?view=sql-server-2017
                sql = String.Format(@"USE MASTER; SELECT db.is_encrypted FROM sys.databases db LEFT OUTER JOIN sys.dm_database_encryption_keys dm ON db.database_id = dm.database_id WHERE db.name = '{0}';", conf.DataSystems[0].Schemas[0].Schema);
                if (CSGenio.persistence.DBConversion.ToInteger(sp.executeScalar(sql)) == 0)
                {
                    model.AlertType = AlertTypeEnum.danger;
                    model.ResultMsg = Resources.Resources.A_BASE_DE_DADOS_NAO_03552;
                    sp.closeConnection();
                    return Json(model);
                }
                sp.closeConnection();
            }
            catch (GenioException ex)
            {
                model.AlertType = AlertTypeEnum.danger;
                model.ResultMsg = ex.Message;
            }
            model.AlertType = AlertTypeEnum.success;
            model.ResultMsg = Resources.Resources.BASE_DE_DADOS_COM_EN26288;

            return Json(model);
        }

    }
}
