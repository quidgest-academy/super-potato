using Moq;
using System;
using System.Collections.Generic;
using GenioServer.security;
using GenioMVC.Helpers.Menus;
using CSGenio.persistence;
using CSGenio.framework;
using CSGenio.core.di;
using CSGenio;

namespace GenioMVC.UnitTests
{
    public class UserMenuTests
    {

        private const string TEST_MODULE = "AAA";
        private User user;

        [SetUp]
        public void Setup()
        {
            user = SecurityFactory.GetGuest();

            Configuration.Modules.Add(TEST_MODULE);
        }

        private static Mock<IMenuConditionValidator> ThrowingValidator()
        {
            var mock = new Mock<IMenuConditionValidator>();
            mock.Setup(v => v.ValidateCondition(It.IsAny<MenuEntry>(), It.IsAny<string>())).Throws(new Exception("Should not execute"));
            return mock;
        }

        [Test]
        public void UserMenuTests_InvalidModule_NotLoad()
        {
            var loader = new TestMenuLoader();

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsFalse(service.UserHasAccessToModule("InvalidModule"));
            Assert.IsFalse(loader.WasLoaded);
        }

        [Test]
        public void UserMenuTests_PublicModule()
        {
            var loader = new TestMenuLoader();

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsFalse(service.UserHasAccessToModule("Public"));
            Assert.IsFalse(loader.WasLoaded);
        }

        [Test]
        public void UserMenuTests_NoRoleInAnyMenu()
        {
            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }
                }
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsFalse(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_WithRole()
        {
            var user = new User("testuser", "", Configuration.DefaultYear);
            user.AddModuleRole(TEST_MODULE, Role.ADMINISTRATION);
            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }
                }
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsTrue(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_WithConditionNoAccess()
        {
            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", HasCondition = true, Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }
                }
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsFalse(service.UserHasAccessToModule(TEST_MODULE));
        }


        [Test]
        public void UserMenuTests_NonNecessaryCondition()
        {
            var user = new User("testuser", "", Configuration.DefaultYear);
            user.AddModuleRole(TEST_MODULE, Role.ADMINISTRATION);

            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", HasCondition = true, Action = "list", Controller = "ctrl", Action_MVC = "act" },
                    new MenuEntry { ID = "AAA2", RoleId = "1", Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }
                }
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsTrue(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_AllMenusWithCondition()
        {
            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", HasCondition = true, Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }
                }
            });

            var user = new User("testuser", "", Configuration.DefaultYear);
            user.AddModuleRole(TEST_MODULE, Role.ADMINISTRATION);
            var mockValidator = new Mock<IMenuConditionValidator>();
            mockValidator.Setup(v => v.ValidateCondition(It.IsAny<MenuEntry>(), TEST_MODULE)).Returns(true);

            var service = new UserMenuService(loader, mockValidator.Object, user);

            Assert.IsTrue(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_NestedActionableEntry()
        {
            var user = new User("testuser", "", Configuration.DefaultYear);
            user.AddModuleRole(TEST_MODULE, Role.ADMINISTRATION);

            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", Children = new List<MenuEntry>() {
                        new MenuEntry { ID = "AAA1a", RoleId = "1", Action = "list", Controller = "ctrl", Action_MVC = "act" }
                    }}
                }}
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsTrue(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_NonActionableEntry()
        {
            var user = new User("testuser", "", Configuration.DefaultYear);
            user.AddModuleRole(TEST_MODULE, Role.ADMINISTRATION);

            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1" }
                }}
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsFalse(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_TreeLevelMinusOne_SkipsRoleCheck()
        {
            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", TreeLevel = -1, Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }}
            });

            var service = new UserMenuService(loader, ThrowingValidator().Object, user);

            Assert.IsTrue(service.UserHasAccessToModule(TEST_MODULE));
        }

        [Test]
        public void UserMenuTests_ConditionFails()
        {
            var user = new User("testuser", "", Configuration.DefaultYear);
            user.AddModuleRole(TEST_MODULE, Role.ADMINISTRATION);

            var loader = new TestMenuLoader(new List<MenuEntry>() {
                new MenuEntry { ID = TEST_MODULE, Children = new List<MenuEntry>() {
                    new MenuEntry { ID = "AAA1", RoleId = "1", HasCondition = true, Action = "list", Controller = "ctrl", Action_MVC = "act" }
                }}
            });

            var mockValidator = new Mock<IMenuConditionValidator>();
            mockValidator.Setup(v => v.ValidateCondition(It.IsAny<MenuEntry>(), TEST_MODULE)).Returns(false);

            var service = new UserMenuService(loader, mockValidator.Object, user);

            Assert.IsFalse(service.UserHasAccessToModule(TEST_MODULE));
        }

    }
}
