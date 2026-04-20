using Administration.Models;
using CSGenio.business;
using CSGenio.framework;
using Microsoft.AspNetCore.Mvc;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using DbAdmin;

namespace Administration.Controllers
{
    public class RoleController : ControllerBase
    {
        private static readonly DBUserManagement _dbUserManagement = new DBUserManagement();

        [HttpGet]
        public IActionResult GetUsersForModule(string module, string roleId)
        {
            try
            {
                string currentYear = GetYearFromRoute();
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(currentYear);
                var users = _dbUserManagement.GetUsersWithoutRoleForModule(module, roleId, sp);

                return Ok(new
                {
                    Success = true,
                    UserList = users
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetRole(string module, string roleId)
        {
            //Get the specified module
            var moduleRole = ModuleRoleModel.ALL_MODULE_ROLES.Find(x => x.Role == roleId && x.Module == module);
            var role = Role.GetRole(roleId);

            var matrixUsers = GetUsersOfRole(module, roleId);
            var userList = new List<object>();
            for (int i = 0; i < matrixUsers.NumRows; i++)
            {
                var name = matrixUsers.GetString(i, CSGenioApsw.FldNome);
                var date = matrixUsers.GetDate(i, CSGenioAuserauthorization.FldDataCria);
                userList.Add(new { UserName = name, ChangedDate = date.ToShortDateString() });
            }

            var matrixUsersAbove = GetUsersRolesAbove(module, roleId);
            var userAboveList = new List<object>();
            for (int i = 0; i < matrixUsersAbove.NumRows; i++)
            {
                var name = matrixUsersAbove.GetString(i, CSGenioApsw.FldNome);
                var date = matrixUsersAbove.GetDate(i, CSGenioAuserauthorization.FldDataCria);
                var roleAboveId = matrixUsersAbove.GetString(i, CSGenioAuserauthorization.FldRole);
                var level = matrixUsersAbove.GetInteger(i, CSGenioAuserauthorization.FldNivel);
                var roleAbove = ModuleRoleModel.GetRole(module, roleAboveId, level) ?? new ModuleRoleModel();;
                var codpsw = matrixUsersAbove.GetString(i, CSGenioAuserauthorization.FldCodpsw);
                userAboveList.Add(new { UserName = name, ChangedDate = date.ToShortDateString(), roleAbove.Designation,Codpsw = codpsw });
            }

            var parentRoles = Role.ALL_ROLES.Values.Where(p => role.HasRole(p) && p != role).Select(r => r.Id);
            var parents = ModuleRoleModel.ALL_MODULE_ROLES.Where(mr => parentRoles.Contains(mr.Role) && mr.Module == module);

            var childrenRoles = Role.ALL_ROLES.Values.Where(c => c.HasRole(role) && c != role).Select(r => r.Id);
            var children = ModuleRoleModel.ALL_MODULE_ROLES.Where(mr => childrenRoles.Contains(mr.Role) && mr.Module == module);

            string moduleDescription = UsersController.GetModuleName(moduleRole.Module);

            return Json(new
            {
                moduleRole.Module,
                ModuleDescription = moduleDescription,
                moduleRole.Designation,
                moduleRole.Description,
                UserList = userList,
                UserAboveList = userAboveList,
                Parents = parents.ToList(),
                Children = children.ToList()
            });
        }

        private DataMatrix GetUsersOfRole(string module, string roleId)
        {

            PersistentSupport sp = GetPersistentSupport();
            SelectQuery query = new SelectQuery()
                .Select(CSGenioApsw.FldNome)
                .Select(CSGenioAuserauthorization.FldDataCria)
                .From(Area.AreaUSERAUTHORIZATION)
                    .Join(Area.AreaPSW.Table, Area.AreaPSW.Alias)
                    .On(CriteriaSet.And()
                        .Equal(CSGenioAuserauthorization.FldCodpsw, CSGenioApsw.FldCodpsw))
                .Where(RoleCriteriaSet(module, roleId));

            return sp.Execute(query);
        }

        /// <summary>
        /// Creates a criteria set for user authorization to filter by module and role. Also checks levels.s
        /// </summary>
        internal static CriteriaSet RoleCriteriaSet(string module, string roleId)
        {
            int roleNum;
            if (!int.TryParse(roleId, out roleNum))
                roleNum = -1;

            return CriteriaSet.And()
                    .Equal(CSGenioAuserauthorization.FldModulo, module)
                    .SubSet(CriteriaSet.Or()
                        .Equal(CSGenioAuserauthorization.FldRole, roleId)
                        .Equal(CSGenioAuserauthorization.FldNivel, roleNum));
        }

        private DataMatrix GetUsersRolesAbove(string module, string roleId)
        {

            CriteriaSet roleCriteria = CriteriaSet.Or();
            foreach (var role in Role.GetRole(roleId).AllRolesAbove())
            {
                roleCriteria = roleCriteria.Equal(CSGenioAuserauthorization.FldRole, role.Id);
                int roleNum;
                if (int.TryParse(role.Id, out roleNum) && roleId != role.Id)
                    roleCriteria = roleCriteria.Equal(CSGenioAuserauthorization.FldNivel, roleNum);
            }
            var sp = GetPersistentSupport();

            SelectQuery query = new SelectQuery()
                .Select(CSGenioApsw.FldNome)
                .Select(CSGenioAuserauthorization.FldDataCria)
                .Select(CSGenioAuserauthorization.FldRole)
                .Select(CSGenioAuserauthorization.FldCodpsw)
                .Select(CSGenioAuserauthorization.FldNivel)
                .From(Area.AreaUSERAUTHORIZATION)
                    .Join(Area.AreaPSW.Table, Area.AreaPSW.Alias)
                    .On(CriteriaSet.And()
                        .Equal(CSGenioAuserauthorization.FldCodpsw, CSGenioApsw.FldCodpsw))
                .Where(CriteriaSet.And()
                    .Equal(CSGenioAuserauthorization.FldModulo, module)
                    .SubSet(roleCriteria));

            return sp.Execute(query);
        }
    }
}
