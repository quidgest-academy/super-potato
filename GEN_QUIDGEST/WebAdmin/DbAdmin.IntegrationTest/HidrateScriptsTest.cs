using CSGenio.business;
using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using ExecuteQueryCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;


namespace DbAdmin.IntegrationTest
{
    public class HidrateScriptsTest
    {
        private GlobalFunctions _globalFunctions;

        private User _user = new User("TestUser", "", "0");

        private Mock<IVersionReader> readerMock;

        private static RdxScript SimpleScript = new RdxScript()
        {
            MinDbVersion = "20",
            MaxDbVersion = "40"
        };

        [SetUp]
        public void Setup()
        {
            CSGenio.GenioDIDefault.UseLog();
            CSGenio.GenioDIDefault.UseDatabase();
            _globalFunctions = new GlobalFunctions(_user, "TST");
            readerMock = new Mock<IVersionReader>();
        }


        /// <summary>
        /// The functions should not fail if there are no scripts to test
        /// </summary>
        [Test]
        public void EmptyScriptListTest()
        {
            var scriptList = new List<ExecuteQueryCore.RdxScript>();
            var result = _globalFunctions.HidrateScripts(scriptList,readerMock.Object );
            Assert.IsEmpty(result);
        }

        /// <summary>
        /// Test cases when the database configuration isn't accessible
        /// </summary>
        [Test]
        public void NoVersionInfo()
        {
            var scriptList = new List<RdxScript>() { SimpleScript };

            readerMock.Setup((vr) => vr.GetDbVersion()).Throws(new Exception());

            var result = _globalFunctions.HidrateScripts(scriptList, readerMock.Object);
            Assert.IsEmpty(result);
        }


        [Test]
        public void ScriptInRange()
        {
            var scriptList = new List<RdxScript>() { SimpleScript };
            readerMock.Setup((vr) => vr.GetDbVersion()).Returns(25);

            var result = _globalFunctions.HidrateScripts(scriptList, readerMock.Object);

            Assert.IsNotEmpty(result);
        }


        [Test]
        public void ScriptOutOfRange()
        {
            var scriptList = new List<RdxScript>() { SimpleScript };

            readerMock.Setup((vr) => vr.GetDbVersion()).Returns(10);

            var result = _globalFunctions.HidrateScripts(scriptList, readerMock.Object);


            Assert.IsEmpty(result);
        }

    }
}
