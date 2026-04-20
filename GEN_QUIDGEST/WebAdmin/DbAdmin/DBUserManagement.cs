using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioServer.security;
using log4net;
using Quidgest.Persistence.GenericQuery;

namespace DbAdmin
{
    public class DBUserManagement
    {
        public class UserBasicInfo
        {
            public string Codpsw { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(DBUserManagement));

        /// <summary>
        /// Retrieves a list of users who do not have the specified role assigned for a given module.
        /// </summary>
        /// <param name="module">The module for which the users are being checked.</param>
        /// <param name="roleId">The role ID to filter users who do not have this role.</param>
        /// <param name="sp">Persistent support object to execute queries.</param>
        /// <returns>A list of users who do not have the specified role in the given module.</returns>
        public List<UserBasicInfo> GetUsersWithoutRoleForModule(string module, string roleId, PersistentSupport sp)
        {
            SelectQuery usersByModule = new SelectQuery()
                .Select(CSGenioApsw.FldCodpsw)
                .Select(CSGenioApsw.FldNome)
                .Select(CSGenioApsw.FldEmail)
                .From(Area.AreaPSW)
                .Join(Area.AreaUSERAUTHORIZATION.Table, TableJoinType.Left)
                .On(CriteriaSet.And()
                    .Equal(CSGenioApsw.FldCodpsw, CSGenioAuserauthorization.FldCodpsw)
                    .Equal(CSGenioAuserauthorization.FldModulo, module)
                    .Equal(CSGenioAuserauthorization.FldRole, roleId)
                    .Equal(CSGenioAuserauthorization.FldZzstate, 0)
                    .Equal(CSGenioApsw.FldZzstate, 0)
                );

            var result = sp.Execute(usersByModule);

            var userList = new List<UserBasicInfo>();

            for (int i = 0; i < result.NumRows; i++)
            {
                userList.Add(new UserBasicInfo
                {
                    Codpsw = result.GetString(i, CSGenioApsw.FldCodpsw),
                    UserName = result.GetString(i, CSGenioApsw.FldNome),
                    Email = result.GetString(i, CSGenioApsw.FldEmail)
                });
            }

            return userList;
        }
        public bool RemoveUserRole(string cod, string module, string roleId, PersistentSupport sp)
        {
            var criteriaSet = CriteriaSet.And()
                .Equal(CSGenioAuserauthorization.FldCodpsw, cod)
                .Equal(CSGenioAuserauthorization.FldModulo, module)
                .Equal(CSGenioAuserauthorization.FldRole, roleId)
                .Equal(CSGenioAuserauthorization.FldZzstate, 0);

            var roles = CSGenioAuserauthorization.searchList(sp, SysConfiguration.CreateWebAdminUser(), criteriaSet);

            if (roles.Count == 1)
            {
                roles[0].delete(sp);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a user already has a role assigned for a module and assigns it if not.
        /// </summary>
        /// <param name="sp">Persistent support object.</param>
        /// <param name="codpsw">User identifier.</param>
        /// <param name="module">Module name.</param>
        /// <param name="roleId">Role ID.</param>
        public void AssignRoleIfNotExists(PersistentSupport sp, string codpsw, string module, string roleId)
        {
            var criteriaSet = CriteriaSet.And()
                .Equal(CSGenioAuserauthorization.FldCodpsw, codpsw)
                .Equal(CSGenioAuserauthorization.FldModulo, module)
                .Equal(CSGenioAuserauthorization.FldRole, roleId)
                .Equal(CSGenioAuserauthorization.FldZzstate, 0);

            var existing = CSGenioAuserauthorization.searchList(sp, SysConfiguration.CreateWebAdminUser(), criteriaSet);

            if (!existing.Any())
            {
                CSGenioAuserauthorization.InsertRole(sp, SysConfiguration.CreateWebAdminUser(), codpsw, module, Role.GetRole(roleId));
            }
        }

        /// <summary>
        /// Initializes or updates the list of roles associated with this application in the user provider
        /// </summary>
        public void SetupProviders()
        {
            foreach (var provider in SecurityFactory.RoleProviderList)
                if(provider.HasUserDirectory)
                    provider.SetupUserDirectory();
        }
    }
}
