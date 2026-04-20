using Administration.Models;
using CSGenio;
using CSGenio.framework;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.ServiceModel;

using Quidgest.Persistence;
using CSGenio.business;
using Quidgest.Persistence.GenericQuery;
using CSGenio.persistence;
using DbAdmin;
using SortOrder = Quidgest.Persistence.GenericQuery.SortOrder;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration.Controllers
{
    public class ManageReportsController(IConfigurationManager configManager) : ControllerBase
    {	
	    private PersistentSupport _sp;
        private PersistentSupport sp
        {
            get
            {
                if (_sp == null)
                    _sp = PersistentSupport.getPersistentSupport(CurrentYear);
                return _sp;
            }
        }

        private User _user;
        private User user
        {
            get
            {
                if (_user == null)
                {
                    _user = SysConfiguration.CreateWebAdminUser(CurrentYear);
                }
                return _user;
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ManageReportsModel();

            var conf = configManager.GetExistingConfig();
            if (Directory.Exists(conf.pathReports))
            {
                model.ReportList = CheckReportStatus(conf);
                model.ReportsPath = conf.pathReports;
                model.ReportsServerUrl = conf.ssrsServer.url;
                model.ReportsServerPath = conf.ssrsServer.path;
                model.DeployActive = !task_completed;
            }
            else
            {
                model.ResultMsg = Resources.Resources.NAO_FOI_POSSIVEL_ACE65321 + "<br />" + conf.pathReports;
            }

            return Json(model);
        }

        private static List<ReportItem> CheckReportStatus(ConfigurationXML conf)
        {
            List<ReportItem> mergedReportList = new List<ReportItem>();

            //read the last instalation list
            var installList = ReportInstallXml.Load(Path.Combine(conf.pathReports, "ReportInstall.xml"));

            //compare it to the current report directory list
            var files = Directory.EnumerateFiles(conf.pathReports);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                if (ext == ".rdl")
                {
                    ReportItem ri = new ReportItem();
                    ri.ReportType = ext;
                    ri.Name = Path.GetFileName(file);
                    ri.DateFile = Directory.GetLastWriteTime(file);
                    ri.Hash = CalculateMD5(file);

                    //find the report in the install list
                    var installedReport = installList.Find(x => x.Report == ri.Name);
                    if (installedReport == null)
                        ri.Status = "new";
                    else
                    {
                        ri.DateInstall = installedReport.Date;
                        ri.Dynamic = installedReport.Dynamic;
                        ri.Error = installedReport.Error;
                        if (installedReport.Hash != ri.Hash)
                            ri.Status = "modified";
                        else
                            ri.Status = string.IsNullOrEmpty(ri.Error)? "ok": "error";

                        //remove the matching report so we can find deleted entries in the end
                        installList.Remove(installedReport);
                    }
                    mergedReportList.Add(ri);
                }
            }

            //process deleted reports
            foreach(var install in installList)
            {
                ReportItem ri = new ReportItem();
                ri.ReportType = Path.GetExtension(install.Report);
                ri.Name = install.Report;
                ri.DateFile = null;
                ri.DateInstall = install.Date;
                ri.Hash = install.Hash;
                ri.Status = "deleted";
                ri.Error = install.Error;
                mergedReportList.Add(ri);
            }

            return mergedReportList;
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        [HttpPost]
        public IActionResult StartDeploy([FromBody]ReportFormModel reportForm)
        {
            var scope = reportForm.Scope;
            var dynamic = reportForm.Dynamic;
            var delete = reportForm.Delete;

            // This cannot be placed inside a task otherwise it will cause
            // issues when getting the data from the route params
            string currentYear = CurrentYear;

            task_completed = false;
            task_numScript = 0;
            task_totalScript = 0;
            task_currentScript = Resources.Resources.INICIANDO_A_INSTALAC31812;

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    //read app configuration
                    var conf = configManager.GetExistingConfig();

                    var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == currentYear);

                    if (dataSystem == null)
                        throw new BusinessException("Could not find a valid dataSystem with the curresponding year.", "ManageReportsController.StartDeploy",
                            "Could not find a valid dataSystem with the curresponding year.");

                    //Validate dataSystem data
                    var schema = dataSystem.Schemas.FirstOrDefault(new DataXml());
                    if (schema == null || string.IsNullOrEmpty(dataSystem.Server) || string.IsNullOrEmpty(dataSystem.Name)
                        || string.IsNullOrEmpty(schema.Id) || string.IsNullOrEmpty(schema.Schema))
                        throw new BusinessException("Please configure a database connection.", "ManageReportsController.StartDeploy",
                            "Please configure a database connection.");

                    //Build binding from appsettings.json
                    BasicHttpBinding rsBinding = (BasicHttpBinding)new EndpointFactory("ReportingService2010Soap").GetBinding();

                    //Set endpoint address from using user input
                    EndpointAddress rsEndpointAddress = new EndpointAddress(conf.ssrsServer.url + "/ReportService2010.asmx");

                    //Build WCF Client
                    ReportingService2010SoapClient rs = new ReportingService2010SoapClient(rsBinding, rsEndpointAddress);

                    if (conf.ssrsServer.ContainsCredentials())
                        rs.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(conf.ssrsServer.UsernameDecode, conf.ssrsServer.PasswordDecode, conf.ssrsServer.Domain);
                    else
                        rs.ClientCredentials.Windows.ClientCredential = (System.Net.NetworkCredential)System.Net.CredentialCache.DefaultCredentials;

                    //create the deployment directory
                    //--------------------------------------------
                    if (!string.IsNullOrEmpty(conf.ssrsServer.path))
                    {
                        task_currentScript = Resources.Resources.CRIAR_DIRECTORIA_DE_09451;
                        CreateFolders(rs, conf.ssrsServer.path);
                    }
                    task_currentScript = Resources.Resources.CRIAR_FONTE_DE_DADOS29380;
                    string ds_path = CreateDataSource(rs, conf.ssrsServer.path, dataSystem);
                    //--------------------------------------------

                    //merge all the data again
                    var statusAll = CheckReportStatus(conf);
                    //filter according to the scope
                    var statusFilter = statusAll.FindAll(x => x.Status != "deleted");
                    if (scope == "Different")
                        statusFilter = statusAll.FindAll(x => x.Status == "modified" || x.Status == "new");
                    if (scope == "Newer")
                        statusFilter = statusAll.FindAll(x => x.DateFile > x.DateInstall || x.Status == "new");

                    task_currentScript = Resources.Resources.ACTUALIZAR_RELATORIO27442;
                    task_totalScript = statusFilter.Count;
                    foreach(var report in statusFilter)
                    {
                        task_currentScript = report.Name;

                        report.DateInstall = DateTime.Now;
                        report.Dynamic = dynamic;
                        try //Allow each report to fail individually
                        {
                            report.Hash = CalculateMD5(Path.Combine(conf.pathReports, report.Name));

                            //deploy the report to the server
                            //--------------------------------------------
                            var report_file = ProcessReport(Path.Combine(conf.pathReports, report.Name), dynamic, dataSystem.Server, dataSystem.Port, dataSystem.Schemas[0].Schema, ds_path);
                            var warnings = CreateReportInServer(rs, report_file, report.Name, conf.ssrsServer.path, dataSystem.LoginDecode(), dataSystem.PasswordDecode());

                            if (warnings != null && warnings.Length > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (var w in warnings)
                                    sb.AppendLine(w.Message);
                                task_message = sb.ToString();
                                report.Error = task_message;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("[( " + report.Name + " )] - " + e.Message);
                            report.Error = e.Message;
                            task_message = e.Message;
                            continue;
                        }
                        //--------------------------------------------

                        task_numScript++;
                    }

                    //process updated install status
                    var installList = new List<ReportInstallXml>();
                    var deleteList = new List<ReportInstallXml>();
                    foreach (var report in statusAll)
                    {
                        var item = new ReportInstallXml();
                        item.Date = report.DateInstall.Value;
                        item.Dynamic = report.Dynamic;
                        item.Hash = report.Hash;
                        item.Report = report.Name;
                        item.Error = report.Error;

                        if (delete && report.Status == "deleted")
                            deleteList.Add(item);
                        else
                            installList.Add(item);
                    }

                    if(delete)
                    {
                        task_currentScript = Resources.Resources.APAGAR_RELATORIOS_NA12725;

                        //delete unnecessary reports
                        //--------------------------------------------
                        foreach(var item in deleteList)
                        {
                            try
                            {
                                rs.DeleteItem(new TrustedUserHeader(), "/" + conf.ssrsServer.path + "/" + item.Report);
                            }
                            catch(Exception e)
                            {
                                //if we can´t delete, there is nothing more we can do.
                                Log.Error(e.Message);
                            }
                        }
                        //--------------------------------------------
                    }

                    //persist the installation log
                    ReportInstallXml.Save(installList, Path.Combine(conf.pathReports, "ReportInstall.xml"));
                }
                catch (Exception e)
                {
                    task_message = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                    Log.Error(e.Message);
                }
                task_completed = true;
            });

            return Json(new { DeployActive = !task_completed });
        }

        private static int task_totalScript = 0;
        private static int task_numScript = 0;
        private static string task_currentScript = "";
        private static string task_message = "";
        private static bool task_completed = true;

        [HttpGet]
        public IActionResult ProgressDeploy()
        {
            var Result = new
            {
                Count = task_totalScript == 0 ? 0 : task_numScript * 100 / task_totalScript,
                Message = task_message,
                ActualScript = task_currentScript,
                Completed = task_completed
            };

            return Json(Result);
        }
		
		private static string getConn (string server, string port = "", string database = null)
        {
            CSGenio.framework.Configuration.Reload();
            var datasystems = CSGenio.framework.Configuration.DataSystems;
            if (datasystems.Count == 0)
                throw new FrameworkException("Config doesn't have any datasystem configured", "ManageReportsController.ProcessReport", "Config doesn't have any datasystem configured");

            var sp = PersistentSupport.getPersistentSupport(CSGenio.framework.Configuration.DefaultYear);

            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(sp.Connection.ConnectionString);
            builder.DataSource = server + (String.IsNullOrEmpty(port) ? "" : ", " + port);
            if(database == null)
                builder.InitialCatalog = "<toReplace>";
            builder.ApplicationIntent = System.Data.SqlClient.ApplicationIntent.ReadOnly;
            builder.Remove("user id");
            builder.Remove("password");			

            return builder.ToString();
        }

        private byte [] ProcessReport(string filename, bool isDynamic, string server, string port, string database, string datasource)
        {
            // Load the document and set the root element.
            XmlDocument doc = new XmlDocument();
            using (XmlTextReader reader = new XmlTextReader(filename))
                doc.Load(reader);

            //Select the cd node with the matching title
            XmlElement root = doc.DocumentElement;

            if (isDynamic)
            {
                bool hasReadOnlyIntent = root.GetElementsByTagName("ReportParameter")
                    .Cast<XmlNode>()
                    .Any(p => p.Attributes["Name"]?.Value == "ReadOnlyIntent");

                string conn = "=\"" + getConn(server, port).Replace("\"","\"\"") + "\"";
                conn = conn.Replace("<toReplace>", "\"+Parameters!Database.Value+\"");
                if (hasReadOnlyIntent)
                {
                    conn = conn.Replace(
                        "ApplicationIntent=ReadOnly\"",
                        "ApplicationIntent=\" & IIF(Parameters!ReadOnlyIntent.Value, \"ReadOnly\", \"ReadWrite\")"
                    );
                }

                //Replace only if "Parameters!Database.Value" are the last value
                conn = conn.Replace("+\"\"", "");
                conn = conn.Replace("+\";\"", "");
				
                //Reports with dynamic connection strings only work with embedded datasources
                XmlNodeList nodes = root.GetElementsByTagName("ConnectString");
                foreach (XmlNode nodeSTR in nodes)
                    nodeSTR.InnerText = conn;
            }
            else //static reports
            {
                //Reports with static connection string can use shared external data sources
                XmlNodeList nodes = root.GetElementsByTagName("DataSourceName");
                foreach (XmlNode nodeSTR in nodes)
                {
                    nodeSTR.InnerText = database;
                }

                nodes = root.GetElementsByTagName("DataSource");
                foreach (XmlNode nodeSTR in nodes)
                {
                    nodeSTR.Attributes["Name"].InnerText = database;
                }

                nodes = root.GetElementsByTagName("DataSourceReference");
                foreach (XmlNode nodeSTR in nodes)
                {
                    nodeSTR.InnerText = datasource;
                }

                //however, they can also use embedded datasources
                nodes = root.GetElementsByTagName("ConnectString");
				string conn = getConn(server, port, database);
                foreach (XmlNode nodeSTR in nodes)
                    nodeSTR.InnerText = conn;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                return ms.ToArray();
            }
        }

        private void CreateFolders(ReportingService2010SoapClient rs, string deployPath)
        {
            string[] folders = deployPath.Split('/');

            string parent_folder = "/";
            for (int i = 0; i < folders.Length; i++)
            {
                if (!CheckExist(rs, "Folder", parent_folder, folders[i]))
                    rs.CreateFolder(new TrustedUserHeader(), folders[i], parent_folder, null, out var itemInfo);
                if (i > 0)
                    parent_folder += "/";
                parent_folder += folders[i];
            }

        }

        /// Checks if the folder exists or not.
        /// <param name="type">Type of object to find</param>
        /// <param name="path">Parent folder path</param>
        /// <param name="folderName">Name of the folder to search</param>
        /// <returns>True if found, else false.</returns>
        /// <remarks>
        /// Type can be one of:
        /// Component	    A report part.
        /// DataSource      A data source.
        /// Folder          A folder.
        /// Model           A model.
        /// LinkedReport    A linked report.
        /// Report          A report.
        /// Resource        A resource.
        /// DataSet         A shared dataset.
        /// Site            A SharePoint site.
        /// Unknown         An item not associated with any known type.
        /// </remarks>
        private bool CheckExist(ReportingService2010SoapClient rs, string type, string path, string folderName)
        {
            string _path = path + (path == "/" ? "" : "/") + folderName;

            // Condition criteria.
            SearchCondition[] conditions;

            // Condition criteria.
            SearchCondition condition = new SearchCondition();
            condition.Condition = ConditionEnum.Equals;
            condition.ConditionSpecified = true;
            condition.Name = "Name";
            condition.Values = new string[] { folderName };
            conditions = new SearchCondition[1];
            conditions[0] = condition;

            rs.FindItems(new TrustedUserHeader(), path, BooleanOperatorEnum.Or, null, conditions, out var _returnedItems);

            // Iterate thr´ each report properties to get the path.
            foreach (CatalogItem item in _returnedItems)
            {
                if (item.TypeName == type)
                {
                    if (item.Path == _path)
                        return true;
                }
            }
            return false;
        }


        private Warning[] CreateReportInServer(ReportingService2010SoapClient rs, byte [] definition, string reportName, string deployPath, string username, string password)
        {
            Warning[] warnings = null;

            string parent = deployPath;
            if (deployPath[0] != '/')
                parent = "/" + parent;

            //FAS [ 2019/09/25 ]
            //Due to the way that reports are generated in GenioMVC (just the name without extension), the extension (.rdl) must be replaced
            reportName = reportName.Replace(".rdl", "");
            if (CheckExist(rs, "Report", parent, reportName))
                rs.DeleteItem(new TrustedUserHeader(), parent + (deployPath.Length > 0 ? "/" : "") + reportName);
            rs.CreateCatalogItem(new TrustedUserHeader(), "Report", reportName, parent, true, definition, null, out CatalogItem report, out warnings);

            //Set the credentials for all the report embedded datasources
            rs.GetItemDataSources(new TrustedUserHeader(), report.Path, out var datasources);
            foreach (var ds in datasources)
            {
                var dsd = ds.Item as DataSourceDefinition;
                if (dsd != null)
                {
                    dsd.UserName = username;
                    dsd.Password = password;
                    dsd.CredentialRetrieval = CredentialRetrievalEnum.Store;
                }
            }
            rs.SetItemDataSources(new TrustedUserHeader(), report.Path, datasources);

            return warnings;
        }

        private string CreateDataSource(ReportingService2010SoapClient rs, string deployPath, DataSystemXml ds)
        {
            string parent = deployPath;
            
            if (parent[0] != '/') parent = "/" + parent;
            string name = "DS_" + ds.Schemas[0].Id;
            // Define the data source definition.
            DataSourceDefinition definition = new DataSourceDefinition();
            definition.CredentialRetrieval = CredentialRetrievalEnum.Store;
            definition.ConnectString = getConn(ds.Server, ds.Port, ds.Schemas[0].Schema);

            definition.Enabled = true;
            definition.EnabledSpecified = true;
            definition.Extension = "SQL";
			//windows authentication
            if (ds.Schemas[0].ConnWithDomainUser)
                definition.WindowsCredentials = true;
			
            definition.ImpersonateUserSpecified = false;

            definition.UserName = ds.LoginDecode();
            definition.Password = ds.PasswordDecode();

            rs.CreateDataSource(new TrustedUserHeader(), name, parent, true, definition, null, out CatalogItem itemInfo);

            return parent + "/" + name;
        }
		
		/// <summary>
        /// Get the list slot report
        /// It is prepared to receive parameters through queryparameter        
        /// </summary>
        /// <returns>Json({ Success, recordsTotal, data})</returns>
        [HttpGet]
        public IActionResult GetReportSpot()
        {
            try
            {
                //Search text
                string search = FromQuery("global_search");
                //Sort indice (field)
                string order = FromQuery("sort[0].name");
                //Sort directions (asc/desc)
                string orderDir = FromQuery("sort[0].order");
                //Page number
                int page = Convert.ToInt32(FromQuery("page"));
                //number of rows per pages
                int pageSize = Convert.ToInt32(FromQuery("per_page"));
                
                List<FieldRef> orderBy = new List<FieldRef>() { CSGenioAreportlist.FldCodreport, CSGenioAreportlist.FldReport, CSGenioAreportlist.FldSlotid,
                    CSGenioAreportlist.FldTitulo, CSGenioAreportlist.FldDatacria};

                int indiceOrder = 0;

                if (!String.IsNullOrEmpty(order))
                    indiceOrder = GenFunctions.atoi(order);

                SortOrder sortOrder = SortOrder.Ascending;
                if (orderDir == "desc")
                    sortOrder = SortOrder.Descending;

                CriteriaSet selWhere = CriteriaSet.And();
                selWhere.SubSet(CriteriaSet.And().Equal(CSGenioAreportlist.FldZzstate, 0));

                //the search text was filled 
                if (!String.IsNullOrEmpty(search))
                {
                    string searchValue = "%" + search + "%";
                    selWhere.SubSet(
                        CriteriaSet.Or()
                            .Like("reportlist", "report", searchValue)
                            .Like("reportlist", "slotid", searchValue)
                            .Like("reportlist", "titulo", searchValue)
                        );
                }

                SelectQuery selQuery = new SelectQuery()
                    .Select(CSGenioAreportlist.FldCodreport)
                    .Select(CSGenioAreportlist.FldReport)
                    .Select(CSGenioAreportlist.FldSlotid)
                    .Select(CSGenioAreportlist.FldTitulo)
                    .Select(CSGenioAreportlist.FldDatacria)
                    .From("reportlist") 
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
                
                List<object> dataResult = new List<object>();

                for (int lin = 0; lin < dataSet.NumRows; lin++)
                {
                    dataResult.Add(new List<string> {
                    dataSet.GetKey(lin, 0)
                    , dataSet.GetString(lin, 1)
                    , dataSet.GetString(lin, 2)
                    , dataSet.GetString(lin, 3)
                    , dataSet.GetDate(lin, 4).ToString()                    
                });
                }
                return Json(new { Success = true, recordsTotal = total, data = dataResult });
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Server Error" });
            }
        }

        /// <summary>
        /// Answered crud actions events for slot report form
        /// </summary>
        /// <param name="mod">Form mode ('new', 'edit', 'delete')</param>
        /// <param name="codreport">Record key</param>
        /// <returns>Json (Form ViewModel)</returns>
        [HttpGet]
        public IActionResult ManageSlotReport(string mod, string codreport)
        {
            CSGenioAreportlist model = null;
            
            //if form is in insert mode
            if (mod.Equals("new") )            
                model = new CSGenioAreportlist(user);                            
            else            
                model = CSGenioAreportlist.search(sp, codreport, user);            
                       
            var viewmodel = new SlotReportsModel();
            if(model != null )
                viewmodel.MapFromModel(model);
            
            viewmodel.FormMode = mod;

            return Json(viewmodel);
        }

        /// <summary>
        /// Save the slot report
        /// </summary>
        /// <param name="modelView">ViewModel</param>
        /// <returns>Json with a status content(.Success)</returns>
        [HttpPost]
        public IActionResult SaveReportSpot([FromBody] SlotReportsModel modelView)
        {
            sp.openConnection();
            CSGenioAreportlist model = CSGenioAreportlist.search(sp, modelView.ValCodreport, user);
            if (modelView.FormMode == "new" || model == null)            
                model = new CSGenioAreportlist(user);
            
            try
            {
                //map viewmodel to model
                modelView.MapToModel(model);

                //execute diferent action based on form mode
                switch (modelView.FormMode)
                {
                    case "new":
                        {
                            model.insert(sp);
                            break;
                        }
                    case "edit":
                        {
                            model.update(sp);
                            break;
                        }
                    case "delete":
                        {
                            model.delete(sp);
                            break;
                        }
                    default:
                        break;
                }

                sp.closeConnection();
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                return Json(new { Success = false, model });
            }
        }		
		
    }
}