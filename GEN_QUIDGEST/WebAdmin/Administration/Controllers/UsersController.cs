using Administration.AuxClass;
using Administration.Models;
using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using DbAdmin;
using GenioServer.security;

namespace Administration.Controllers
{
    public class UsersController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new UsersModel();
            model.Users = new List<UserItem>();

            CSGenio.framework.Configuration.Reload();
            if(!PersistentSupport.TestDBConnection(CurrentYear))
                model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;

            //list ldap identity providers
            var conf = configManager.GetExistingConfig();
            model.IdentityProviders = conf.Security.SelectMany(p => p.IdentityProviders)
                .Where(p => p.Type.Equals("GenioServer.security.LdapIdentityProvider"))
                .Select(p => new AuxFunctions.SelectlistElement()
                {
                    Value = p.Name,
                    Text = string.IsNullOrEmpty(p.Description) ? p.Name : p.Description
                })
                .ToList();

            return Json(new { Success = true, model = model });
        }

        /// <summary>
        /// Returns the module description resource
        /// </summary>
        /// <param name="moduleId">Module acronym</param>
        public static string GetModuleName(string moduleId)
        {
            var model = new UsersModel();
            addModules(ref model);
            var module = model.Modules.FirstOrDefault(x => x.Cod == moduleId);
            return module.Description;
        }

        /// <summary>
        /// Returns a list of all roles in this application to a client.
        /// The strings are already localized.
        /// </summary>
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = ModuleRoleModel.ALL_MODULE_ROLES.Select(
                r => new
                {
                    r.Role,
                    r.Module,
                    r.Description,
                    ModuleName = GetModuleName(r.Module),
                    Designation = Resources.Resources.ResourceManager.GetString(r.Designation)
                });
            string search = FromQuery("global_search");
            if(!String.IsNullOrEmpty(search))
            {
                roles = roles.Where(r => r.Designation.Contains(search) || r.ModuleName.Contains(search));
            }
            return Json(roles.ToList());
        }

        [HttpGet]
        public IActionResult GetUserList()
        {
            try
            {
                var model = new UsersModel();
                model.Users = new List<UserItem>();

                var component = FromQuery("component");

                addModules(ref model);

                string search = FromQuery("global_search");
                string orderDir = FromQuery("sort[0][order]");
                int page = Convert.ToInt32(FromQuery("page"));
                int pageSize = Convert.ToInt32(FromQuery("per_page"));

                SortOrder sortOrder = SortOrder.Ascending;
                if (orderDir == "desc")
                    sortOrder = SortOrder.Descending;

                //Construct return query
                SelectQuery selQuery = new SelectQuery()
                    .Select(CSGenioApsw.FldCodpsw)
                    .Select(CSGenioApsw.FldNome)
                    .From(Area.AreaPSW.Table, Area.AreaPSW.Alias);

                //Filter with search bar result
                if (!String.IsNullOrEmpty(search))
                {
                    string searchValue = "%" + search + "%";
                    selQuery = selQuery.Where(CriteriaSet.And()
                        .Like(CSGenioApsw.FldNome, searchValue));
                }

                //Set pagesize and offset
                selQuery = selQuery
                    .PageSize(pageSize)
                    .Page(page)
                    .OrderBy(CSGenioApsw.FldNome, sortOrder);
                selQuery.noLock = true;

                List<object> dataResult = new List<object>();

                // Init Persistent Support
                var sp = AuxFunctions.GetPersistentSupport(configManager, CurrentYear);

                sp.openConnection();
                DataMatrix dataSet = sp.Execute(selQuery);
                int total = DBConversion.ToInteger(sp.ExecuteScalar(QueryUtils.buildQueryCount(selQuery)));
                sp.closeConnection();

                if (dataSet.NumRows > 0)
                {
                    List<string> userList = new List<string>();
                    for (int i = 0; i < dataSet.NumRows; i++)
                    {
                        userList.Add(dataSet.GetKey(i, 0));
                    }

                    //get the module and the level from the database to display in the users table
                    var userAuthorization = CSGenioAuserauthorization.searchList(sp, SysConfiguration.CreateWebAdminUser(), CriteriaSet.And()
                        .In(CSGenioAuserauthorization.FldCodpsw, userList)
                        .Equal(CSGenioAuserauthorization.FldSistema, "FOR")
                        .Equal(CSGenioAuserauthorization.FldZzstate, 0));

                    for (int i = 0; i < dataSet.NumRows; i++)
                    {
                        string codpsw = dataSet.GetKey(i, 0);
                        string userName = dataSet.GetKey(i, 1);
                        var userRoles = userAuthorization
                            .Where(x => x.ValCodpsw == codpsw)
                            .Select(x => ModuleRoleModel.GetRole(x.ValModulo, x.ValRole,(int) x.ValNivel))
                            .Where(x => x != null) //GetRole might return null
                            .ToList();
                        dataResult.Add(new { Codpsw = codpsw, Nome = userName, privileges = userRoles });
                    }
                }

                return Json(new { recordsTotal = total, data = dataResult, modules = model.Modules });
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Error", recordsTotal = 0, data = new List<object>()});
            }

        }

        /// <summary>
        /// Gets the list of all users with information if they belong or not to a given role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUsersWithRole(string module, string roleId)
        {
            var userList = new List<object>();

            string search = FromQuery("global_search");
            string orderDir = FromQuery("sort[0][order]");
            int page = Convert.ToInt32(FromQuery("page"));
            int pageSize = Convert.ToInt32(FromQuery("per_page"));

            SortOrder sortOrder = SortOrder.Ascending;
            if (orderDir == "desc")
                sortOrder = SortOrder.Descending;

            //Construct return query
            SelectQuery selQuery = new SelectQuery()
                .Select(CSGenioApsw.FldCodpsw)
                .Select(CSGenioApsw.FldNome)
                .Select(SqlFunctions.Sum(SqlFunctions.Iif( //Returns if the user is in this role
                    RoleController.RoleCriteriaSet(module, roleId), 1, 0)),
                    "inRole")
                .From(Area.AreaUSERAUTHORIZATION)
                    .Join(Area.AreaPSW.Table, Area.AreaPSW.Alias)
                    .On(CriteriaSet.And()
                        .Equal(CSGenioAuserauthorization.FldCodpsw, CSGenioApsw.FldCodpsw))
                .GroupBy(CSGenioApsw.FldCodpsw)
                .GroupBy(CSGenioApsw.FldNome);


            //Filter with search bar result
            if (!String.IsNullOrEmpty(search))
            {
                string searchValue = "%" + search + "%";
                selQuery = selQuery.Where(CriteriaSet.And()
                    .Like(CSGenioApsw.FldNome, searchValue));
            }

            //Set pagesize and offset
            selQuery = selQuery
                .PageSize(pageSize)
                .Page(page)
                .OrderBy(CSGenioApsw.FldNome, sortOrder);
            selQuery.noLock = true;

            PersistentSupport sp = GetPersistentSupport();
            sp.openConnection();
            DataMatrix dataSet = sp.Execute(selQuery);

            int total = DBConversion.ToInteger(sp.ExecuteScalar(QueryUtils.buildQueryCount(selQuery)));
            sp.closeConnection();

            //Fill the user list with matrix results
            for (int i = 0; i < dataSet.NumRows; i++)
            {
                string codpsw = dataSet.GetString(i, CSGenioApsw.FldCodpsw);
                string name =  dataSet.GetString(i, CSGenioApsw.FldNome);
                bool inRole = dataSet.GetInteger(i, "inRole") == 1;
                userList.Add(new { Name = name, Cod = codpsw, InRole = inRole});
            }

            var role = ModuleRoleModel.GetRole(module, roleId);

            return Json(new { total, userList, role.Designation });

		}

		[HttpPost]
        public IActionResult ImportUsersFromAD(string providerId)
        {
            User user = SysConfiguration.CreateWebAdminUser();

            var conf = configManager.GetExistingConfig();
            var provider = conf.Security.SelectMany(p => p.IdentityProviders)
                .Where(p => p.Type.Equals("GenioServer.security.LdapIdentityProvider") && p.Name == providerId)
                .FirstOrDefault();
            if (provider == null)
                return Json(new { Status.E });

            var ldap = SecurityFactory.ParseIdentityProvider(provider) as LdapIdentityProvider;
            if (ldap == null)
                return Json(new { Status.E });

            //When possible, refactor the ImportUsersFromAD that uses a deprecated method to
            //use the more generic:
            //ldap.ImportUsers(adminUser, adminPsw);
            //and split the user creation into another function

            PersistentSupport sp = AuxFunctions.GetPersistentSupport(configManager, CurrentYear);
            StatusMessage st =  new GlobalFunctions(user, user.CurrentModule, sp).ImportUsersFromAD(ldap.Domain);
            return Json(new { Status = st.Status.ToString(), });
        }


        public static void addModules(ref UsersModel model)
        {
            model.Modules.Add(new UsersModule("FOR", Resources.Resources.MY_APPLICATION56216));
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            User user = SysConfiguration.CreateWebAdminUser();
            var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\temp\" + "user-list.xlsx";

            //verify if exist temp file
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath); //Delete any leftovers
            }

            /* Build Select Queries */

            //Page 1
            SelectQuery p1Query = new SelectQuery()
                    .Select(CSGenioApsw.FldNome, Resources.Resources.NOME_DE_UTILIZADOR58858)
                    .Select(SqlFunctions.Iif(CriteriaSet.Or().NotEqual(CSGenioApsw.FldStatus, "0").NotEqual(CSGenioApsw.FldZzstate, "0"),
                    Resources.Resources.INACTIVE23138, Resources.Resources.ACTIVE03270), Resources.Resources.ESTADO07788)
                    .From(Area.AreaPSW.Table, Area.AreaPSW.Alias)
                    .OrderBy(CSGenioApsw.FldNome, SortOrder.Ascending);

            p1Query.noLock = true;


            //Page 2
            SelectQuery p2Query = new SelectQuery()
                    .Select(CSGenioApsw.FldNome, Resources.Resources.NOME_DE_UTILIZADOR58858)
                    .Select(CSGenioAuserauthorization.FldModulo, Resources.Resources.MODULO59907)
                    .Select(CSGenioAuserauthorization.FldNivel, Resources.Resources.ROLE60946)
                    .Select(CSGenioAuserauthorization.FldRole, "role")
                    .From(Area.AreaPSW.Table, Area.AreaPSW.Alias)
                    .Join(CSGenioAuserauthorization.AreaUSERAUTHORIZATION).On(CriteriaSet.And().Equal(CSGenioAuserauthorization.FldCodpsw, CSGenioApsw.FldCodpsw))
                    .Where(CriteriaSet.And()
                    .Equal(CSGenioAuserauthorization.FldSistema, "FOR")
                    .Equal(CSGenioAuserauthorization.FldZzstate, 0))
                    .OrderBy(CSGenioApsw.FldNome, SortOrder.Ascending)
                    .OrderBy(CSGenioAuserauthorization.FldModulo, SortOrder.Ascending);
            p2Query.noLock = true;
            /* ------------------- */

            /* Fetch configs and initiate sp */
            var conf = configManager.GetExistingConfig();

            var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear); // Default == null
            if (dataSystem == null)
                throw new FrameworkException(Resources.Resources.FICHEIRO_DE_CONFIGUR13972, "UserController -> GetUserList", Resources.Resources.FICHEIRO_DE_CONFIGUR13972);
            /* ----------------------------- */

            //Get data from page 2 query
            PersistentSupport sp = PersistentSupport.getPersistentSupport(dataSystem.Name);
            DataMatrix p2Dm = sp.Execute(p2Query);

            //Define new DataMatrix with the final query data
            DataMatrix finalData = new DataMatrix(new System.Data.DataSet());
            finalData.DbDataSet.Tables.Add(new System.Data.DataTable());
            finalData.DbDataSet.Tables[0].Columns.Add(Resources.Resources.NOME_DE_UTILIZADOR58858, typeof(string)).AllowDBNull = true;
            finalData.DbDataSet.Tables[0].Columns.Add(Resources.Resources.MODULO59907, typeof(string)).AllowDBNull = true;
            finalData.DbDataSet.Tables[0].Columns.Add(Resources.Resources.ROLE60946, typeof(string)).AllowDBNull = true;

            for (int i = 0; i < p2Dm.NumRows; i++) //Transfer data between DMs and change designations
            {
                object[] objs = new object[] {
                    p2Dm.DbDataSet.Tables[0].Rows[i][0].ToString(),
                    p2Dm.DbDataSet.Tables[0].Rows[i][1].ToString(),
                    p2Dm.DbDataSet.Tables[0].Rows[i][2].ToString()
                };

                finalData.DbDataSet.Tables[0].Rows.Add(objs);

                string module = p2Dm.DbDataSet.Tables[0].Rows[i][1].ToString();
                string role = p2Dm.DbDataSet.Tables[0].Rows[i][3].ToString();
                string s_level = p2Dm.DbDataSet.Tables[0].Rows[i][2].ToString();
                int level = Convert.ToInt32(!string.IsNullOrWhiteSpace(s_level) ? s_level : 0);

                ModuleRoleModel roleModel = ModuleRoleModel.GetRole(module, role, level);

                if (roleModel != null)
                    finalData.DbDataSet.Tables[0].Rows[i][2] = Resources.Resources.ResourceManager.GetString(roleModel.Designation);
                else
                    finalData.DbDataSet.Tables[0].Rows[i][2] = "";
            }

            /* Generate Excel */
            QueryToExcel toExcel = new QueryToExcel(sp, user);
            toExcel.AddWorksheet(new QueryToExcel.WorksheetQueries(Resources.Resources.NOME_DE_UTILIZADOR58858, new QueryInfo(p1Query)));
            toExcel.AddWorksheet(new QueryToExcel.WorksheetQueries(Resources.Resources.USER_ROLES25359, new QueryInfo(finalData.DbDataSet)));
            toExcel.Convert(filePath);
            /* ------------- */

            //Build and return file
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "user-list.xlsx");
        }
    }
}
