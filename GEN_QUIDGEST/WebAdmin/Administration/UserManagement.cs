using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using Administration.Models;
using CSGenio;
using CSGenio.framework;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using DbAdmin;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration
{
    /// <summary>
    /// This is for use to manage users (create and disable) on application
    /// </summary>
    public class UserManagement : IUserManagementService
    {
        private readonly IConfigurationManager _configManager;

        public UserManagement(IConfigurationManager configManager)
        {
            _configManager = configManager;
        }
        
		/// <summary>
        /// Return a list of all Permitions accept for each user and for each module on application
        /// </summary>
        /// <returns>All permitions of each module</returns>
        public List<ModulesLevel> GetPermissions()
        {
            List<ModulesLevel> perm = new List<ModulesLevel>();
            perm.Add(new ModulesLevel("FOR", Resources.Resources.MY_APPLICATION56216, "1", Resources.Resources.CONSULTA40695));
            perm.Add(new ModulesLevel("FOR", Resources.Resources.MY_APPLICATION56216, "2", Resources.Resources.OFFICER20358));
            perm.Add(new ModulesLevel("FOR", Resources.Resources.MY_APPLICATION56216, "3", Resources.Resources.AGENT00994));
            perm.Add(new ModulesLevel("FOR", Resources.Resources.MY_APPLICATION56216, "99", Resources.Resources.ADMINISTRADOR57294));
	
            return perm;
        }

        private PersistentSupport getSP()
        {
            var conf = _configManager.GetExistingConfig();
            var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == Configuration.DefaultYear); // Default == null

            if (dataSystem == null)
                return null;

            return PersistentSupport.getPersistentSupport(dataSystem.Name);
        }

        /// <summary>
        /// To search one user by name
        /// </summary>
        /// <param name="username">Name to search</param>
        /// <returns>If "username" exist than return PK field of user</returns>
        private string getUser(string username)
        {
            string codUsr = "";
            var sp = getSP();

            try
            {
                sp.openConnection();

                //verificar se já existe um utilizador com o mesmo nome
                SelectQuery userQuery = new SelectQuery()
                    .Select(CSGenioApsw.FldCodpsw)
                    .From("USERLOGIN", "psw")
                    .PageSize(1);

                CriteriaSet where = new CriteriaSet(CriteriaSetOperator.And);
                where.Equal(CSGenioApsw.FldNome, username);
                where.Equal(CSGenioApsw.FldZzstate, 0);

                userQuery.Where(where);
                codUsr = CSGenio.persistence.DBConversion.ToString(sp.ExecuteScalar(userQuery));
            }
            catch { }
            finally
            {
                sp.closeConnection();
            }

            return codUsr;
        }

        private void saveUserLevel(User user, string codUser, PersistentSupport sp, string modulo, string level)
        {
            var persisted = CSGenioAuserauthorization.searchList(sp, user, CriteriaSet.And()
                                .Equal(CSGenioAuserauthorization.FldSistema, "VVC")
                                .Equal(CSGenioAuserauthorization.FldCodpsw, codUser)
                                .Equal(CSGenioAuserauthorization.FldModulo, modulo) 
                                .Equal(CSGenioAuserauthorization.FldZzstate, 0));
            Role role = Role.GetRole(level);
            if(!persisted.Any(x=>x.IsRole(modulo, role)))
            {
                CSGenioAuserauthorization.InsertRole(sp, user, codUser, modulo, role);
            }
        }
        
		/// <summary>
        /// Create user
        /// </summary>
        /// <param name="username">name for user</param>
        /// <param name="password">password to use for the same user</param>
        /// <param name="levels">priviledge for access for each module</param>
        /// <returns>true if create successful</returns>
        public bool CreateUserWithPassAndLevels(string username, string password, List<ModulesLevel> levels)
        {
            //Check if user exist and return if true
            if (getUser(username) != "")
                return false;

            try
            {
                var sp = getSP();
                sp.openConnection();

                //Temporary user to insert the new one
                User user = SysConfiguration.CreateWebAdminUser(userName: "WebServer");

                CSGenioApsw userPsw = new CSGenioApsw(user, "FOR");
                userPsw.ValNome = username;
                userPsw.ValEmail = String.Empty;
                userPsw.ValPhone = String.Empty;
                userPsw.ValStatus = 0;
                if (password != "")
                {
                    string pswEnc = GenioServer.security.PasswordFactory.Encrypt(password);
                    userPsw.ValPassword = pswEnc;
                    userPsw.ValSalt = "";
                    userPsw.ValPswtype = Configuration.Security.PasswordAlgorithms.ToString();
                }
                userPsw.insert(sp);


                //save all access levels
                if (levels != null && levels.Count != 0)
                {
                    string codUsr = getUser(username);
                    foreach (ModulesLevel level in levels)
                        saveUserLevel(user, codUsr, sp, level.Module, level.Level);
                }

                sp.closeConnection();

                return true;
            }
            catch
            {
                return false;
            }
        }

		/// <summary>
        /// Create User
        /// </summary>
        /// <param name="username">name for user</param>
        /// <param name="password">password to use for the same user</param>
        /// <returns>true if create successful</returns>
        public bool CreateUserWithPass(string username, string password)
        {
            return CreateUserWithPassAndLevels(username, password, new List<ModulesLevel>());
        }

		/// <summary>
        /// Create User
        /// </summary>
        /// <param name="username">name for user</param>
        /// <param name="levels">priviledge for access for each module</param>
        /// <returns>true if create successful</returns>
        public bool CreateUserWithLevels(string username, List<ModulesLevel> levels)
        {
            if (levels == null)
                return false;

            return CreateUserWithPassAndLevels(username, "", levels);
        }

		/// <summary>
        /// Create User
        /// </summary>
        /// <param name="username">name for user</param>
        /// <returns>true if create successful</returns>
        public bool CreateUser(string username)
        {
            //Can be one sistem only with windows authentication and only be mandatory to save username
            return CreateUserWithPassAndLevels(username, "", null);
        }

		/// <summary>
        /// Disable User
        /// </summary>
        /// <param name="username">name of user</param>
        /// <returns>true if disable successful</returns>
        public bool DeleteUser(string username)
        {
            //check if exist user to disable that
            string codUsr = getUser(username);
            if (String.IsNullOrEmpty(codUsr))
                return false;

            try
            {
                var sp = getSP();
                sp.openConnection();

                User user = SysConfiguration.CreateWebAdminUser(userName: "WebServer");

                //update the user data
                CriteriaSet where = new CriteriaSet(CriteriaSetOperator.And);
                where.Equal("psw", "codpsw", codUsr);
                where.Equal("psw", "zzstate", "0");
                CSGenioApsw userPsw = new CSGenioApsw(user, "FOR");
                CSGenioApsw userPswAux = CSGenioApsw.searchList(sp, user, where).First();
                userPsw.ValCodpsw = userPswAux.ValCodpsw;
                userPsw.ValNome = userPswAux.ValNome;
                userPsw.ValEmail = userPswAux.ValEmail;
                userPsw.ValPhone = userPswAux.ValPhone;
                userPsw.ValStatus = 2; //Doesn't remove user but put that disable with value 2
                userPsw.ValCertsn = userPswAux.ValCertsn;
                userPsw.ValPswtype = userPswAux.ValPswtype;
                userPsw.update(sp);

                sp.closeConnection();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Test(string s)
        {
            return s;
        }
    }
}
