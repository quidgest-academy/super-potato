using Administration.AuxClass;
using Administration.Models;
using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using GenioServer.security;
using DbAdmin;


namespace Administration.Controllers
{
    public class ManageUsersController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return MainGet(null);
        }

        [HttpGet]
        [Route("[action]")]
        [ActionName("MainGet")]
        public IActionResult MainGet(ManageUsersModel model = null)
        {
            var cod = FromQuery("cod");
            var mod = FromQuery("mod");
            if (model == null)
                model = new ManageUsersModel();

            // Load url parameters
            if (cod != null)
                model.CodUser = cod;
            if (mod != null)
                model.ModForm = mod.ToString();

            if (model.ModForm == "3")
                model.SubmitValue = "DESEJA_ELIMINAR_ESTA45806";
            else
                model.SubmitValue = "GRAVAR_CONFIGURACAO36308";

            //caso o mode do form for inválido
            if (model.ModForm == null || (model.ModForm != "1" && model.ModForm != "2" && model.ModForm != "3"))
                return Json(new { Success = false, redirect = "users" });

			//Add identity providers. Used for disabling password change if there is no QuidgestIdentityProvider.
			model.IdentityProviders = new List<string>();
            var allProviders = CSGenio.framework.Configuration.Security.IdentityProviders;
            model.IdentityProviders = allProviders.Select(x => x.Type).ToList();

            paintPageOptions(ref model);

            return Json(new { Success = true, model = model });
        }

        private void paintPageOptions(ref ManageUsersModel model)
        {
            addModules(ref model);

            // preecher array com privilégios
            if (model.CodUser != null && model.CodUser != String.Empty)
                loadDBUser(ref model);

        }

        private void loadDBUser(ref ManageUsersModel model)
        {
            var conf = configManager.GetExistingConfig();
            var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear); // Default == null

            if (dataSystem == null)
            {
                model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972;
                return;
            }

            try
            {
                model.Privileges = new List<ModuleRoleModel>();

                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(dataSystem.Name);
                sp.openConnection();
                SelectQuery userQuery = new SelectQuery()
                    .Select(CSGenioApsw.FldNome)
                    .Select(CSGenioApsw.FldEmail)
                    .Select(CSGenioApsw.FldPhone)
                    .Select(CSGenioApsw.FldStatus)
					.Select(CSGenioApsw.FldPsw2favl)
                    .Select(CSGenioApsw.FldPsw2fatp)
                    .From("USERLOGIN", "psw")
                    .Where(CriteriaSet.And()
                        .Equal(CSGenioApsw.FldCodpsw, model.CodUser)
                    );
                DataMatrix dataSetUsr = sp.Execute(userQuery);
                //DataMatrix dataSetUsr = sp.executeQuery("select nome, email, phone, status from userlogin where codpsw = '" + model.CodUser + "'");


                SelectQuery selUsrAuth = new SelectQuery()
                    .Select(CSGenioAuserauthorization.FldModulo)
                    .Select(CSGenioAuserauthorization.FldRole)
                    .Select(CSGenioAuserauthorization.FldNivel)
                    .From(Area.AreaUSERAUTHORIZATION)
                    .Where(CriteriaSet.And()
                        .Equal(CSGenioAuserauthorization.FldSistema, "FOR")
                        .Equal(CSGenioAuserauthorization.FldCodpsw, model.CodUser)
                        .Equal(CSGenioAuserauthorization.FldZzstate, 0)
                    )
                    .OrderBy(CSGenioAuserauthorization.FldModulo, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);
                DataMatrix dataSetAut = sp.Execute(selUsrAuth);

                sp.closeConnection();

                if (dataSetUsr.NumRows == 1)
                {
                    model.Username = dataSetUsr.GetString(0, 0);
                    model.Email = dataSetUsr.GetString(0, 1);
                    model.Phone = dataSetUsr.GetString(0, 2);
                    model.StatusFirstLogin = (dataSetUsr.GetInteger(0, 3) == 1 ? true : false);
                    model.StatusDisableLogin = (dataSetUsr.GetInteger(0, 3) == 2 ? true : false);
					model.Psw2faVL = dataSetUsr.GetString(0, 4);
                    model.Psw2faTP = dataSetUsr.GetString(0, 5);
                    //JGF 2021.01.18 Changed visibilty and readonly rules to be able to show to the user what the current value is, but forbid change once invalidated
					model.ShowInvalidate2FA = conf.HasAnyApplicationWith2FA();                
                    model.BlockInvalidate2FA = String.IsNullOrEmpty(model.Psw2faVL);
                    model.Invalidate2FA = String.IsNullOrEmpty(model.Psw2faVL);

                    for (int lin = 0; lin < dataSetAut.NumRows; lin++)
                    {
                        string module = dataSetAut.GetString(lin, 0);
                        string roleId = dataSetAut.GetString(lin, 1);
                        int level = dataSetAut.GetInteger(lin, 2);
                        var role = ModuleRoleModel.GetRole(module, roleId, level);
                        if(role != null)
                            model.Privileges.Add(role);
                    }
                    model.AssignedRoles = model.ToDictionaryList(model.Privileges);
                }
            }
            catch
            {
                //we ignore errors for now (version will look as 0)
            }
        }

        private void addModules(ref ManageUsersModel model)
        {
            model.Modules = new List<Module>();
            model.Modules.Add(new Module("FOR", Resources.Resources.MY_APPLICATION56216));

            //Check if the module only has levels.
            foreach(var module in model.Modules)
            {
                module.OnlyLevels = model.AvaiableRoles[module.Cod].All(r =>  Role.GetRole(r.Role).Type != RoleType.ROLE);
            }
        }


        [HttpPost]
        public IActionResult SaveConfig([FromBody]ManageUsersModel model)
        {
            var cod = FromQuery("cod");
            var mod = FromQuery("mod");
            if (!ModelState.IsValid)
                return Json(new { Success = false, model = new { ResultMsg = Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860 } });

            var conf = configManager.GetExistingConfig();
            var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear); // Default == null

            if (dataSystem == null)
                return Json(new { Success = false, model = new { ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR13972 } });

            List<object> ignoredRoles = new List<object>();
            try
            {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(dataSystem.Name);
                sp.openConnection();

                //check if a user with the same name already exists
                SelectQuery userQuery = new SelectQuery()
                    .Select(CSGenioApsw.FldCodpsw)
                    .From("USERLOGIN", "psw")
                    .PageSize(1);

                CriteriaSet where = new CriteriaSet(CriteriaSetOperator.And);
                where.Equal(CSGenioApsw.FldNome, model.Username);
                where.Equal(CSGenioApsw.FldZzstate, 0);

                if (model.ModForm != "1") //!= de introduce
                    where.NotEqual(CSGenioApsw.FldCodpsw, model.CodUser);

                userQuery.Where(where);
                var userExist = sp.ExecuteScalar(userQuery);

                //string verfiUser = String.Format("select TOP 1 CODPSW from userlogin where nome = '{0}' and zzstate = 0", model.Username);
                //if (model.ModForm != "1") //!= de introduce
                //    verfiUser += " and codpsw != '" + model.CodUser + "'";
                //var userExist = sp.executeScalar(verfiUser);

                if (userExist != null)
                {
                    //replace de %s to o format do csharp
                    var regex = new Regex(Regex.Escape("%s"));
                    var msg = regex.Replace(Resources.Resources.A_FICHA_COM_O_VALOR_35649, "{0}", 1);
                    msg = regex.Replace(msg, "{1}", 1);

                    //model.ResultMsg = String.Format(msg, model.Username, Resources.Resources.NOME47814);
                    //paintPageOptions(ref model);
                    return Json(new { Success = false, model = new { ResultMsg = Resources.Resources.O_NOME_DE_UTILIZADOR53911 } });
                }

                if (model.ModForm == "3")
                {
                    Guid codpswG;
                    int codpswI;
                    if (Guid.TryParse(model.CodUser, out codpswG) || Int32.TryParse(model.CodUser, out codpswI))
                    {
                        deleteUser(model, sp);
                    }
                }
                else
                {
                    //Check if this is a new user
                    string error = saveUser(ref model, sp);
                    if (error != "")
                    {
                        //model.ResultMsg = Resources.Resources.ResourceManager.GetString(error);
                        //paintPageOptions(ref model);
                        return Json(new { Success = false, model = new { ResultMsg = Resources.Resources.ResourceManager.GetString(error) } });
                    }

                    Guid codpswG;
                    int codpswI;
                    if (Guid.TryParse(model.CodUser, out codpswG) || Int32.TryParse(model.CodUser, out codpswI))
                    {

                        var adminUser = SysConfiguration.CreateWebAdminUser();
                        var criteriaSet = CriteriaSet.And()
                                .Equal(CSGenioAuserauthorization.FldSistema, "FOR")
                                .Equal(CSGenioAuserauthorization.FldCodpsw, model.CodUser)
                                .Equal(CSGenioAuserauthorization.FldZzstate, 0);

                        var existing = CSGenioAuserauthorization.searchList(sp, adminUser, criteriaSet);

                        model.Privileges = model.FromDictionaryList(model.AssignedRoles);

                        //Add the new records if necessary
                        foreach (var moduleRole in model.Privileges)
                        {
                            var role = Role.GetRole(moduleRole.Role);
                            var rolesAbove = GetRolesAbove(model, moduleRole);
                            ignoredRoles.AddRange(rolesAbove);

                            //Check if the role we are trying to insert exists.
                            if (!existing.Any(x => x.IsRole(moduleRole.Module, role)) && !rolesAbove.Any())
                                //Insert the necessary UserAuthorization record
                                CSGenioAuserauthorization.InsertRole(sp, adminUser, model.CodUser, moduleRole.Module, role);
                        }

                        //Delete redundant roles
                        var redundantRoles = GetRedundantRoles(existing, model);
                        foreach(var moduleRole in redundantRoles)
                            moduleRole.delete(sp);

                        //JGF 2021.06.02 Check for possible duplicates and delete them
                        var duplicateRoles = GetDuplicateRoles (existing);
                        foreach (var moduleRole in duplicateRoles)
                            moduleRole.delete(sp);
                    }
                }

                sp.closeConnection();
                return Json(new { Success = true , ignoredRoles});
            }
            catch (BusinessException e)
            {
                model.ResultMsg = Translations.Get(e.UserMessage, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                return Json(new { Success = false, model = new { model.ResultMsg } });
            }
            catch (Exception e)
            {
                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
				return Json(new { Success = false, model = new { ResultMsg = model.ResultMsg } });
            }
			//Unreachable code detected, i will maintain that to know how to go to main page if necessary
            /*paintPageOptions(ref model);
            return MainGet(model);*/
        }

        /// <summary>
        /// Checks if there are roles that make this role useless and adds them to a List 
        /// </summary>
        /// <param name="model">Manage users model</param>
        /// <param name="moduleRole">Role to check</param>
        /// <returns>List of roles that are above the role to check and therefore useless</returns>
        private static List<ModuleRoleModel> GetRolesAbove(ManageUsersModel model, ModuleRoleModel moduleRole)
        {
            var role = Role.GetRole(moduleRole.Role);
            List<ModuleRoleModel> rolesAbove = model.Privileges
                .Where(x => x.Module == moduleRole.Module && role.Id != x.Role && role.HasRole(x.Role)).ToList();
            return rolesAbove;
        }

        /// <summary>
        /// Finds the redundant role records a user is trying to save.
        /// </summary>
        /// <param name="existing">List of roles to save</param>
        /// <param name="model">Manage users model</param>
        /// <returns>List of the roles that are redundant and therefore useless</returns>
        private static List<CSGenioAuserauthorization> GetRedundantRoles(List<CSGenioAuserauthorization> existing, ManageUsersModel model)
        {
            var redundantRoles = new List<CSGenioAuserauthorization>();
            foreach(var moduleRole in existing)
            {
                Role role = Role.GetRole(moduleRole.ValRole);
                //Check if a role was deleted and is invalid
                if (role == Role.INVALID)
                    redundantRoles.Add(moduleRole);

                //Check if a role was removed, or some other higher role was inserted
                else if (!model.Privileges.Any(x => x.Module == moduleRole.ValModulo && x.Role == role.Id) ||
                    model.Privileges.Any(x => x.Module == moduleRole.ValModulo && role.HasRole(x.Role) && role.Id != x.Role)
                    )
                    redundantRoles.Add(moduleRole);
            }
            return redundantRoles;
        }

        // <summary>
        /// Finds duplicate roles a user is trying to save.
        /// </summary>
        /// <param name="existing">List of roles to save</param>
        /// <returns>List of roles that are duplicated and therefore useless</returns>
        private static List<CSGenioAuserauthorization> GetDuplicateRoles(List<CSGenioAuserauthorization> existing)
        {
            var duplicateRoles = new List<CSGenioAuserauthorization>();
            foreach(var record in existing)
                {
                    Role role = Role.GetRole(record.ValRole);
                    if (existing.Any(x => x.IsRole(record.ValModulo, role) && x.ValCodua.CompareTo(record.ValCodua) > 0)) //Hack to check for duplicates
                        duplicateRoles.Add(record);
                }
            return duplicateRoles;
        }

        [HttpPost]
        public void DeleteRole(string codpsw, string module, string roleId)
        {
            var sp = GetPersistentSupport();
            var user = SysConfiguration.CreateWebAdminUser();
            var role = Role.GetRole(roleId);
            var criteriaSet = CriteriaSet.And()
                .Equal(CSGenioAuserauthorization.FldCodpsw, codpsw)
                .Equal(CSGenioAuserauthorization.FldModulo, module);

            if (role.Type == RoleType.LEVEL)
            {
                //If the model was set in backoffice the level won't be filled
                criteriaSet = criteriaSet.SubSet(CriteriaSet.Or()
                    .Equal(CSGenioAuserauthorization.FldRole, roleId)
                    .Equal(CSGenioAuserauthorization.FldNivel, role.GetLevelInt()));
            }
            else
            {
                criteriaSet  = criteriaSet.Equal(CSGenioAuserauthorization.FldRole, roleId);
            }

            sp.openTransaction();
            var roles = CSGenioAuserauthorization.searchList(sp, user, criteriaSet);
            if(roles.Count == 1)
            {
                roles[0].delete(sp);
            }
            sp.closeTransaction();
        }

        [HttpPost]
        public void InsertRole(string codpsw, string module, string roleId)
        {
            var sp = GetPersistentSupport();
            var user = SysConfiguration.CreateWebAdminUser();
            var role = Role.GetRole(roleId);
            sp.openTransaction();
            CSGenioAuserauthorization.InsertRole(sp, user, codpsw, module, role);
            sp.closeTransaction();
        }

        private void deleteUser(ManageUsersModel model, PersistentSupport sp)
        {
            User user = SysConfiguration.CreateWebAdminUser();

            CriteriaSet where = new CriteriaSet(CriteriaSetOperator.And);
            where.Equal("psw", "codpsw", model.CodUser);
            where.Equal("psw", "zzstate", "0");
            CSGenioApsw userPsw = new CSGenioApsw(user);
            CSGenioApsw userPswAux = CSGenioApsw.searchList(sp, user, where).First();
            userPsw.ValCodpsw = userPswAux.ValCodpsw;
            userPsw.delete(sp);
        }

        [HttpPost]
        public IActionResult UserDeleteFromTable(string cod)
        {
            if (string.IsNullOrEmpty(cod))
            {
                return Json(new { Success = false, Message = Resources.Resources.O_CODIGO_NAO_PODE_SE62260 });
            }

            CSGenio.persistence.PersistentSupport sp = null;

            try
            {
                var conf = configManager.GetExistingConfig();
                var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear);

                if (dataSystem == null)
                {
                    return Json(new { Success = false, Message = Resources.Resources.CONFIGURACOES_NAO_EN22975 });
                }

                sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(dataSystem.Name);
                sp.openConnection();

                // Config model with user ID
                var model = new ManageUsersModel { CodUser = cod };
                deleteUser(model, sp);

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }

            finally {
                if (sp != null)
                {
                    sp.closeConnection();
                }
            }
        }

        private string saveUser(ref ManageUsersModel model, PersistentSupport sp)
        {
            string message = "";
            User user = SysConfiguration.CreateWebAdminUser();

            if (model.CodUser == null || model.CodUser == String.Empty)
            {
                //inserção de um novo user
                int status = (model.StatusFirstLogin ? 1 : 0);
                status = (model.StatusDisableLogin ? 2 : status);

                Password password = null;
                if (model.PasswordChange)
                {
                    password = new Password( model.PasswordNew, model.PasswordConfirm);
                }
                var factory = new UserFactory(sp, user);
                var userPsw = factory.CreateNewPsw( model.Username, model.Email, model.Phone, status, password);
                
                userPsw.ValOpercria = "WebAdmin";
                userPsw.ValDatacria = DateTime.Now;
                userPsw.insert(sp);
                model.CodUser = userPsw.ValCodpsw;
            }
            else
            {
                //actualização de dados dos utilizadores
                CriteriaSet where = new CriteriaSet(CriteriaSetOperator.And);
                where.Equal("psw", "codpsw", model.CodUser);
                where.Equal("psw", "zzstate", "0");
                CSGenioApsw userPsw = new CSGenioApsw(user, "FOR");
                CSGenioApsw userPswAux = CSGenioApsw.searchList(sp, user, where).First();
                userPsw.ValCodpsw = userPswAux.ValCodpsw;
                userPsw.ValNome = userPswAux.ValNome;
                userPsw.ValEmail = model.Email;
                userPsw.ValPhone = model.Phone;
				//If user is disabled by max attemps then when we enable again the attemps will be marked with 0
                if (userPswAux.ValStatus == 2 && !model.StatusDisableLogin)
                    userPsw.ValAttempts = 0;
                userPsw.ValStatus = (model.StatusFirstLogin ? 1 : 0);
                userPsw.ValStatus = (model.StatusDisableLogin ? 2 : userPsw.ValStatus);
                userPsw.ValCertsn = userPswAux.ValCertsn;
				if (model.Invalidate2FA)
                {
                    userPsw.ValPsw2favl = "";
                    userPsw.ValPsw2fatp = "";
                }
                if (model.PasswordChange)
                {
                    try
                    {
                        // Authorize user to get object with information about all years
                        User userBeingChanged = SecurityFactory.Authorize(new()
                        {
                            AuthenticationType = "internal",
                            Name = model.Username,
                            IsAuthenticated = true,
                            IdProperty = GenioIdentityType.InternalId
                        });

                        // Change the user's password
                        foreach (var identityProvider in SecurityFactory.IdentityProviderList)
                            if (identityProvider.HasUsernameAuth())
                                SecurityFactory.StoreCredential(identityProvider.Id, userBeingChanged, null, model.PasswordNew);
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                    }
                }
                
                userPsw.ValOpermuda = "WebAdmin";
                userPsw.ValDatamuda = DateTime.Now;
                userPsw.update(sp);
            }
            return message;
        }

        [HttpGet]
        public IActionResult GetModules()
        {
            try
            {
                var model = new ManageUsersModel();
                addModules(ref model);

                string search = FromQuery("global_search");

                var filteredModules = string.IsNullOrEmpty(search)
                    ? model.Modules
                    : model.Modules.Where(m => m.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                return Json(new { recordsTotal = filteredModules.Count, data = filteredModules });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message, recordsTotal = 0 });
            }
        }

        [HttpPost]
        public IActionResult RemoveUserRole(string cod, string module, string roleId)
        {
            CSGenio.persistence.PersistentSupport sp = null; 
            try
            {
                var conf = configManager.GetExistingConfig();
                var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear);

                if (dataSystem == null)
                {
                    return Json(new { Success = false, Message = Resources.Resources.CONFIGURACOES_NAO_EN22975 });
                }

                sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(dataSystem.Name);
                var userManagement = new DBUserManagement();
                sp.openTransaction();
                var success = userManagement.RemoveUserRole(cod, module, roleId, sp);

                return Json(new { Success = success });
            }
            catch (Exception ex)
            {
                if (sp != null)
                {
                    sp.rollbackTransaction();
                }
                return Json(new { Success = false, Message = ex.Message });
            }
            finally
            {
                if (sp != null)
                {
                    sp.closeTransaction();
                }
            }
        }

        [HttpPost]
        public IActionResult AssignRoleToUsers(string users, string module, string roleId)
        {
            if (string.IsNullOrEmpty(users) || string.IsNullOrEmpty(module) || string.IsNullOrEmpty(roleId))
            {
                return Json(new { Success = false, Message = "Parámetros inválidos." });
            }

            CSGenio.persistence.PersistentSupport sp = null;

            try
            {
                var conf = configManager.GetExistingConfig();
                var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == CurrentYear);

                if (dataSystem == null)
                {
                    return Json(new { Success = false, Message = "Configurações não encontradas." });
                }

                sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(dataSystem.Name);
                sp.openConnection();

                var userList = users.Split(',').ToList();
                var userManagement = new DBUserManagement();

                foreach (var codpsw in userList)
                {
                    userManagement.AssignRoleIfNotExists(sp, codpsw, module, roleId);
                }

                sp.closeConnection();
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
            finally
            {
                if (sp != null)
                {
                    sp.closeConnection();
                }
            }
        }
    }
}
