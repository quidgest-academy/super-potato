using CSGenio.persistence;
using DocumentFormat.OpenXml.Bibliography;
using NUnit.Framework;
using Quidgest.Persistence.GenericQuery;

namespace WebTest
{
    public class QueryRendererSqlServerTest
    {
        private QueryRenderer renderer;

        [SetUp]
        public void Setup()
        {
            var sp = new PersistentSupportSQLServer();
            renderer = new QueryRenderer(sp);
        }


        [Test]
        public void SqlValueNumeric()
        {
            SelectQuery query = new SelectQuery().Select(new SqlValue(10),"number");
            var sql = renderer.GetSql(query);

            Assert.IsTrue(sql.Contains("number"));
            Assert.AreEqual(1, renderer.ParameterList.Count);
            Assert.AreEqual(10, renderer.ParameterList[0].Value);
        }

        [Test]
        public void SqlValueString()
        {
            SelectQuery query = new SelectQuery().Select(new SqlValue("SomeText"), "string");
            var sql = renderer.GetSql(query);

            Assert.IsTrue(sql.Contains("string"));
            Assert.AreEqual(1, renderer.ParameterList.Count);
            Assert.AreEqual("SomeText", renderer.ParameterList[0].Value);
        }

        [Test]
        public void SqlValueNamedParameter()
        {
            SelectQuery query = new SelectQuery().Select(new SqlValue("SomeText", "ParameterWithName"), "string");
            var sql = renderer.GetSql(query);

            Assert.IsTrue(sql.Contains("string"));
            Assert.AreEqual(1, renderer.ParameterList.Count);
            Assert.AreEqual("SomeText", renderer.ParameterList[0].Value);
            Assert.AreEqual("@ParameterWithName", renderer.ParameterList[0].ParameterName);
        }

        [Test]
        public void SqlLiteralNumeric()
        {
            SelectQuery query = new SelectQuery().Select(new SqlLiteral(10.1), "literalNumber");
            var sql = renderer.GetSql(query);

            Assert.IsTrue(sql.Contains("10"));
            Assert.AreEqual(0, renderer.ParameterList.Count);
        }

        [Test]
        public void SqlLiteralString()
        {
            SelectQuery query = new SelectQuery().Select(new SqlLiteral("SomeText"), "literalText");
            var sql = renderer.GetSql(query);

            Assert.IsTrue(sql.Contains("'SomeText'"));
            Assert.AreEqual(0, renderer.ParameterList.Count);
        }

        [Test]
        public void SqlLiteralEscapeString()
        {
            SelectQuery query = new SelectQuery().Select(new SqlLiteral("O'Riley"), "escapedText");
            var sql = renderer.GetSql(query);

            Assert.IsTrue(sql.Contains("'O''Riley'"));
            Assert.AreEqual(0, renderer.ParameterList.Count);
        }

    }
}
