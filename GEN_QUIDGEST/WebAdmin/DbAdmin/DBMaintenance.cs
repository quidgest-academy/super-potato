using CSGenio.framework;
using CSGenio;
using ExecuteQueryCore;
using System.Globalization;
using CSGenio.persistence;
using CSGenio.business;
using CSGenio.core.persistence;

namespace DbAdmin
{
    /*
    *   DBMaintenance holds all the library methods related to the database operations.
    *   These are the reindexation methods, backing up, restoring, etc.
    */
    public class DBMaintenance
    {

        private readonly string _baseDirectory;

        /// <summary>
        /// Builds a DBMaintenance object
        /// </summary>
        /// <param name="baseDirectory">The base directory of the application, from where the reindex files can be obtain</param>
        public DBMaintenance(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public class DBMaintenanceException : Exception
        {
            public DBMaintenanceException(string message) : base(message) { }
            public DBMaintenanceException(string message, Exception inner) : base(message, inner) { }
        }

        public class MaintenanceLogDetails
        {
            public RdxOperationInfo Info { get; set; }
            public List<ReindexFunctionItem> FunctionDetails { get; set; }
            public List<ReIndexGroup> FunctionGroups { get; set; }
        }

        private void CreateLogFile()
        {
            if(!File.Exists(PersistentSupport.LogReindexPath()))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PersistentSupport.LogReindexPath()));
                File.Create(PersistentSupport.LogReindexPath()).Close();
            }
        }

        /// <summary>
        /// Retrieves information on the latest "numLogs" database maintenance operations, based on the log file.
        /// </summary>
        /// <param name="numLogs">The number of logs to retrieve (the last "numLogs" of the LogReindex file). If 0, retrieves all.</param>
        /// <param name="functions">List of ReindexFunctions that were run during maintenance</param>
        /// <returns>A list of maintenance information objects (RdxOperationInfo)</returns>
        public List<RdxOperationInfo> GetMaintenanceLogsInfo(int numLogs)
        {
            List<ReIndexFunction> functions = GetReindexScripts().ReIndexItems;
            var allLogs = RdxOperationLog.readAggregateXML(PersistentSupport.LogReindexPath());
            if (allLogs.Count == 0)
                return [];

            var retrievedLogs = (numLogs == 0 ? allLogs : allLogs.TakeLast(numLogs)) // If numLogs == 0, take all logs
                .Select(log => (index: allLogs.IndexOf(log), log)) // maintain original log index
                .ToList();

            List<RdxOperationInfo> logsInfo = retrievedLogs
                .Select(entry => {
                    var functionDetails = LoadReindexFunctionItems(entry.log, functions);
                    return CreateOperationInfo(entry.log, entry.index, functionDetails);
                })
                .ToList();

            return logsInfo; 
        }

        /// <summary>
        /// Retrieves details of a specific maintenance log.
        /// </summary>
        /// <param name="logIndex">Index of the log entry</param>
        /// <param name="functions">List of ReindexFunctions to compare against</param>
        /// <returns>MaintenanceLogDetails object with log information</returns>
        public MaintenanceLogDetails GetMaintenanceLogDetails(int logIndex)
        {
            ReindexOrder rdxInfo = GetReindexScripts();
            RdxOperationLog log = RdxOperationLog.FindXML(logIndex, PersistentSupport.LogReindexPath());

            if (log == null)
                throw new DBMaintenanceException($"Log {logIndex} could not be found.");

            List<ReindexFunctionItem> details = LoadReindexFunctionItems(log, rdxInfo.ReIndexItems);

            return new MaintenanceLogDetails
            {
                Info = CreateOperationInfo(log, logIndex, details),
                FunctionDetails = details,
                FunctionGroups = rdxInfo.Reindexgroups
            };
        }

        /// <summary>
        /// Gathers the essential information of a maintenance task, based on its logs.
        /// </summary>
        /// <param name="log">The <see cref="RdxOperationLog"/> entry containing the logs of a given maintenance task.</param>
        /// <param name="index">The index of the log entry.</param>
        /// <param name="items">A list of <see cref="ReindexFunctionItem"/> representing the maintenance functions (with details).</param>
        /// <returns>An <see cref="RdxOperationInfo"/> object with the maintenance log essential info.</returns>
        public static RdxOperationInfo CreateOperationInfo(RdxOperationLog log, int index, List<ReindexFunctionItem> items)
        {
            return new RdxOperationInfo
            {
                Id = index,
                DataSystem = log.DataSystem,
                Database = log.Database,
                Duration = log.Duration,
                StartTime = log.StartTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), // Date format that enables ordering records by operation date
                Origin = log.Origin,
                Success = items.All(item => item.Result.Length == 0)
            };
        }

        /// <summary>
        /// Loads a list of maintenance function items with details based on a given maintenance log.
        /// </summary>
        /// <param name="log">The <see cref="RdxOperationLog"/> entry for which function details are retrieved.</param>
        /// <param name="functions">A list of <see cref="ReIndexFunction"/> that were executed during maintenance.</param>
        /// <returns>A list of <see cref="ReindexFunctionItem"/> objects containing details of the reindexing process.</returns>
        public static List<ReindexFunctionItem> LoadReindexFunctionItems(RdxOperationLog log, List<ReIndexFunction> functions)
        {
            return functions.Select(function =>
            {
                var item = new ReindexFunctionItem();
                item.Load(function, log);
                return item;
            }).ToList();
        }

        /// <summary>
        /// Start the reindexation process
        /// </summary>
        /// <param name="dbUser"></param>
        /// <param name="dbPassword"></param>
        /// <param name="singleScript">Run only the specified script. Invalidates multiscript parameter if this is filled.</param>
        /// <param name="multiScript">Run multiple scripts</param>
        /// <param name="category"></param>
        /// <param name="fullReindex">Execute the full reindexation, checking the schema from scratch</param>
        /// <param name="rdxEvent"></param>
        /// <param name="cToken"></param>
        /// <param name="year"></param>
        /// <param name="filestreamLocation">Folder for Filestream used during database creation</param>
        /// <returns></returns>
        public RdxParamUpgradeSchema StartReindexation(string dbUser, string dbPassword, string singleScript, List<string> multiScript, string category, bool fullReindex, ChangedEventHandler rdxEvent = null, CancellationToken cToken = new CancellationToken(), string year = "", int timeout = 300, string filestreamLocation = "")
        {
            RdxParamUpgradeSchema RdxItem = new RdxParamUpgradeSchema();

            if (string.IsNullOrEmpty(year))
                year = Configuration.DefaultYear;

            //Load data into Reindex Item
            RdxItem.Year = year;
            RdxItem.Username = dbUser;
            RdxItem.Password = dbPassword;
            RdxItem.Zero = fullReindex;
            RdxItem.Origin = "Database Maintenance";
            RdxItem.DirFilestream = filestreamLocation;

            StartReindexation(RdxItem, singleScript, multiScript, category, rdxEvent, cToken, timeout);
            return RdxItem;
        }


        /// <summary>
        /// Starts the database maintenance from an object with parameters. The current state will be stored in the received RdxItem.
        /// </summary>
        /// <param name="RdxItem">An object with reindexation parameters. Order2Exec should not be filledx</param>
        /// <param name="singleScript">Run only the specified script. Invalidates multiscript parameter if this is filled.</param>
        /// <param name="multiScript">Run multiple scripts</param>
        /// <param name="category"></param>
        /// <param name="rdxEvent"></param>
        /// <param name="cToken"></param>
        /// <param name="timeout"></param>
        public void StartReindexation(RdxParamUpgradeSchema RdxItem, string singleScript, List<string> multiScript, string category, ChangedEventHandler rdxEvent = null, CancellationToken cToken = new CancellationToken(), int timeout = 300)
        {
            //Make sure the log file is created, since reindexation is expecting it
            CreateLogFile();
            string dataSystemId = RdxItem.Year;
            
            ReindexOrder reindexMenu = GetReindexScripts();

            //Set scripts timeout
            reindexMenu.timeout = timeout;

            if (Configuration.DataSystems.Count == 0)
                return;

            if (!string.IsNullOrEmpty(singleScript)) //Run single script
            {
                reindexMenu.ReIndexItems.ForEach(item => item.Selected = (item.Id == singleScript));
            }
            else if (multiScript.Count > 0) //Run multiple specified scripts
            {
                foreach (ReIndexFunction function in reindexMenu.ReIndexItems) 
                    function.Selected = multiScript.Contains(function.Id); 
            }
            else if (!string.IsNullOrEmpty(category)) //Run category
            {
                //Go through all the groups and find the one we are looking for
                foreach (ReIndexGroup group in reindexMenu.Reindexgroups)
                {
                    if (group.Name.ToLower() == category.ToLower())
                    {
                        //Go through all the current items and check if they belong to the wanted group
                        //If so mark them as selected, if not set it to false
                        foreach (ReIndexFunction function in reindexMenu.ReIndexItems)
                        {
                            if (group.GroupItems.Any(x => x == function.Id))
                                function.Selected = true;
                            else
                                function.Selected = false;
                        }
                        break;
                    }
                }
            }

            reindexMenu.CalculateOrder();

            //Create User
            User user = SysConfiguration.CreateWebAdminUser(dataSystemId);

            //Load scripts order of execution
            GlobalFunctions gblFunctions = new GlobalFunctions(user, user.CurrentModule);
            PersistentSupport sp = PersistentSupport.getPersistentSupport(dataSystemId);
            DatabaseVersionReader versionReader = new DatabaseVersionReader(sp);
            List<RdxScript> order2exec = gblFunctions.HidrateScripts(reindexMenu.GetOrderToExecute(), versionReader, RdxItem.Zero);

            //If there are no scripts to run, simply finish the reindexation
            if (order2exec == null || order2exec.Count == 0)
                return;

            RdxItem.OrderExec = order2exec;
            RdxItem.Path = GetReindexPath();

            /* Set Reindex default event that updates RdxItem
             * This event should not be triggered if a custom one is passed, as it can
             * cause concurrency issues!
             *
             * Both AdminCLI and the Reindexation pass custom events that handle the messages
             * to be displayed in the RdxStatus
            */
            if (rdxEvent == null) {
                RdxItem.ChangedExecutionScript += (sender, eventArgs, status) =>
                {
                    RdxItem.Progress = status.Clone();

                    // Update messages is if the reindexations finishes without errors or is cancelled
                    if (status.State == RdxProgressStatus.SUCCESS)
                        RdxItem.Progress.Message = "Completed successfully";                 
                    else if(status.State == RdxProgressStatus.CANCELLED)
                        RdxItem.Progress.Message = "Cancelled successfully";
                };
            }            
            else //Add custom event, if exists
                RdxItem.ChangedExecutionScript += rdxEvent;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    PersistentSupport.upgradeSchema(RdxItem, cToken);
                }
                catch (GenioException e)
                {
                    RdxItem.Progress.State = RdxProgressStatus.ERROR;
                    RdxItem.Progress.Message = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()) + ": " + e.Message;
                }
                catch (OperationCanceledException)
                {
                    RdxItem.Progress.State = RdxProgressStatus.CANCELLED;
                }
                catch (Exception e)
                {
                    RdxItem.Progress.State = RdxProgressStatus.ERROR;
                    RdxItem.Progress.Message = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                }
            });  
        }


        /// <summary>
        /// Get the reindex path from a base path to DBAdmin
        /// </summary>
        /// <param name="basePath">DBAdmin path</param>
        /// <returns></returns>
        public string GetReindexPath()
        {
            //Check cache if there is a reindex path
            string path = (string)QCache.Instance.AdminReindexation.Get("webadmin_reindex_path");

            if (!string.IsNullOrEmpty(path))
                return path;
                
            path = Path.Combine(_baseDirectory, "Scripts", Configuration.Program + "_ReIdx", "Reindex");
            if (Directory.Exists(path))
            {
                QCache.Instance.AdminReindexation.Put("webadmin_reindex_path", path);
                return path;
            }
            
            path = Path.Combine(_baseDirectory, "bin", "Scripts", Configuration.Program + "_ReIdx", "Reindex");
            QCache.Instance.AdminReindexation.Put("webadmin_reindex_path", path);

            return path;
        }
        public string GetIndexReindexPath()
        {
            string path = Directory.GetParent(GetReindexPath()).FullName;
            return Path.Combine(path, "infoReindex.xml");
        }

        public ReindexOrder GetReindexScripts(string path = "")
        {            
            if (string.IsNullOrEmpty(path))
                path = Path.Combine(GetReindexPath(), "order2Exec.xml");

            try
            { 
                return ReindexOrder.readXML(path);
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting reindex scripts: {ex}");
                throw;
            }
        }

        public static string BackupDatabase(string year, string username, string password, string saveLocation = "")
        {
            // Use default year if none is given
            if (string.IsNullOrEmpty(year)) 
                year = Configuration.DefaultYear;
            
            if (Configuration.DataSystems.Count == 0)
                throw new BusinessException("There are no DataSystems configured!", "DBMaintenance.BackupDatabase", "There are no DataSystems configured!");

            return PersistentSupport.Backup(year, username, password, saveLocation);
        }

        /// <summary>
        /// Restores a database from a specified backup file.
        /// </summary>
        /// <param name="year">The year of the target database to be restored.</param>
        /// <param name="username">The username used for authenticating the database connection.</param>
        /// <param name="password">The password used for authenticating the database connection.</param>
        /// <param name="backupsRoot">The root directory where backup files are stored.</param>
        /// <param name="filename">The name of the backup file to be restored.</param>
        public static void RestoreDatabase(string year, string username, string password, string backupsRoot, string filename)
        {
            string backupPath = Path.Combine(backupsRoot, filename);
            PersistentSupport.Restore(year, username, password, backupPath);
        }

        public static void DeleteBackup(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                throw new BusinessException("The file specified in the path does not exist", "DBMaintenance.DeleteBackup", "The file specified in the path does not exist");            
        }
    }
}
