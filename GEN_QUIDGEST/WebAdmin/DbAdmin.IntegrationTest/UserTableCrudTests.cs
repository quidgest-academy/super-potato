using CSGenio.business;
using CSGenio.framework;
using NUnit.Framework;
using Quidgest.Persistence.GenericQuery;
using DbAdmin;
using CSGenio.persistence;

namespace DbAdmin.IntegrationTest
{
    public class UserTableCrudTests : DatabaseTransactionFixture
    {
        [Test]
        public void InsertUser()
        {
            var codpsw = InsertTestUser();

            Assert.AreEqual(0, emptyKey(codpsw));
        }

        private string InsertTestUser()
        {
            CSGenioApsw newUser = new CSGenioApsw(_user);
            newUser.ValNome = "IntegrationTester";
            newUser.insert(sp);
            return newUser.ValCodpsw;
        }

        private void AssignRoleToUser(CSGenioApsw user, string module, Role role)
        {
            CSGenioAs_ua userAuthorization = new CSGenioAs_ua(_user)
            {
                ValSistema = "TST",
                ValModulo = module,
                ValRole = role.Id,
                ValCodpsw = user.ValCodpsw
            };

            userAuthorization.insert(sp);
        }

        private void AssignMultipleRolesToUser(CSGenioApsw user, string module, List<Role> roles)
        {
            foreach (var role in roles)
            {
                CSGenioAs_ua userAuthorization = new CSGenioAs_ua(_user)
                {
                    ValSistema = "TST",
                    ValModulo = module,
                    ValRole = role.Id,
                    ValCodpsw = user.ValCodpsw
                };

                userAuthorization.insert(sp);
            }
        }

        private static Role CreateRole(string title, Role[] subRoles = null)
        {
            Role newRole = new Role(RoleType.ROLE, title, subRoles ?? []);
            Role.ALL_ROLES.Add(title, newRole);

            return newRole;
        }

        [Test]
        public void ReadUser()
        {
            //Arrange
            var codpsw = InsertTestUser();
            //Act
            CSGenioApsw returnedUser = CSGenioApsw.search(sp, codpsw, _user);
            //Assert
            Assert.AreEqual(codpsw, returnedUser.ValCodpsw);
            Assert.AreEqual("IntegrationTester", returnedUser.ValNome);
        }

        [Test]
        public void EditUser()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw returnedUser = CSGenioApsw.search(sp, codpsw, _user);

            //Act
            returnedUser.ValNome = "IntegrationTester2";
            returnedUser.update(sp);

            //Assert
            Assert.AreEqual(codpsw, returnedUser.ValCodpsw);
            Assert.AreEqual("IntegrationTester2", returnedUser.ValNome);
        }

        [Test]
        public void DeleteUser()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw existingUser = CSGenioApsw.search(sp, codpsw, _user);

            //Act
            existingUser.delete(sp);

            //Assert
            CSGenioApsw searchedUser = CSGenioApsw.search(sp, codpsw, _user);
            Assert.IsNull(searchedUser);
        }

        [Test]
        public void InsertUserAuthorization()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw user = CSGenioApsw.search(sp, codpsw, _user);
            Role role = Role.ADMINISTRATION;

            //Act
            AssignRoleToUser(user, "TBL", role);

            //Assert
            var roles = CSGenioAs_ua.searchList(sp, _user, CriteriaSet.And().Equal(CSGenioAs_ua.FldCodpsw, user.ValCodpsw));
            Assert.AreEqual(roles.Count, 1);
        }

        [Test]
        public void InsertDuplicateUserAuthorizationFails()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw user = CSGenioApsw.search(sp, codpsw, _user);
            Role role = Role.ADMINISTRATION;
            AssignRoleToUser(user, "TBL", role);

            //Act & Assert
            Assert.Throws<BusinessException>(() => AssignRoleToUser(user, "TBL", role));
        }

        public static int emptyKey(object characters)
        {
            if (
                characters == null
                || characters.Equals("")
                || characters.Equals(Guid.Empty.ToString())
                || characters.Equals(Guid.Empty.ToString("B"))
                || characters.Equals("0")
            )
                return 1;
            else
                return 0;
        }

        [Test]
        public void GetUsersWithoutRoleForModule_IsIdempotent()
        {
            // Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw existingUser = CSGenioApsw.search(sp, codpsw, _user);
            DBUserManagement dbUserManagement = new DBUserManagement();
            string module = "TBL";
            Role role = Role.ADMINISTRATION;

            List<DBUserManagement.UserBasicInfo> usersBefore = dbUserManagement.GetUsersWithoutRoleForModule(module, role.Id, sp);
            bool userExistsBefore = usersBefore.Exists(u => u.Codpsw == codpsw);

            // Act
            List<DBUserManagement.UserBasicInfo> usersAfter = dbUserManagement.GetUsersWithoutRoleForModule(module, role.Id, sp);
            bool userExistsAfter = usersAfter.Exists(u => u.Codpsw == codpsw);

            // Assert
            Assert.AreEqual(userExistsBefore, userExistsAfter);
        }

        [Test]
        public void RemoveUserRole_WhenUserExist_ReturnsTrue()
        {
            // Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw user = CSGenioApsw.search(sp, codpsw, _user);
            Role roleToRemove = CreateRole("test1");
            Role roleToKeep = CreateRole("test2");
            string module = "TBL";
            AssignMultipleRolesToUser(user, module, new List<Role> { roleToRemove, roleToKeep });

            DBUserManagement dbUserManagement = new DBUserManagement();
            var rolesBefore = CSGenioAs_ua.searchList(sp, _user,
                CriteriaSet.And().Equal(CSGenioAs_ua.FldCodpsw, user.ValCodpsw));
            Assert.AreEqual(2, rolesBefore.Count);
            Assert.IsTrue(rolesBefore.Any(r => r.ValRole == roleToRemove.Id));
            Assert.IsTrue(rolesBefore.Any(r => r.ValRole == roleToKeep.Id));

            // Act
            bool success = dbUserManagement.RemoveUserRole(user.ValCodpsw, module, roleToRemove.Id, sp);

            // Assert
            Assert.IsTrue(success);
            var rolesAfter = CSGenioAs_ua.searchList(sp, _user,
                CriteriaSet.And().Equal(CSGenioAs_ua.FldCodpsw, user.ValCodpsw));
            // Ensure only the intended role was removed
            Assert.AreEqual(1, rolesAfter.Count);
            Assert.IsFalse(rolesAfter.Any(r => r.ValRole == roleToRemove.Id));
            Assert.IsTrue(rolesAfter.Any(r => r.ValRole == roleToKeep.Id));
        }

        [Test]
        public void RemoveUserRole_WhenUserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            DBUserManagement dbUserManagement = new DBUserManagement();
            string codpsw = Guid.NewGuid().ToString();
            string module = "TBL";
            string roleId = Role.ADMINISTRATION.Id;

            // Act
            bool success = dbUserManagement.RemoveUserRole(codpsw, module, roleId, sp);

            // Assert
            Assert.IsFalse(success);
        }
    }
}
