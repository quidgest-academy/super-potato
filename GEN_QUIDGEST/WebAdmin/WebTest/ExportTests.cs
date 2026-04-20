using CSGenio.framework;
using CSGenio.persistence;
using MigraDocCore.DocumentObjectModel.Tables;
using NUnit.Framework;
using System.Data;
using System.IO.Compression;

namespace WebTest
{
    public class ExportsTest
    {
        [Test]
        public void GetTemplateOdsTest()
        {
            var exporter = new ExportToODS();
            var stream = exporter.GetOdsTemplateStream();
            var template = new ZipArchive(stream);
            Assert.AreEqual(15, template.Entries.Count);
        }

        [Test]
        public void GetContentXmlFileTest()
        {
            var exporter = new ExportToODS();
            var stream = exporter.GetOdsTemplateStream();
            var content = exporter.GetContentXmlFile(stream);

            Assert.AreEqual("#document", content.Name);
        }

        public static List<Exports.QColumn> TestColumns()
        {
            var field = new Field("TestArea", "TestCol", FieldType.TEXT);
            field.FieldDescription = "Test Column";

            return new List<Exports.QColumn>
            {
                new Exports.QColumn("TestCol", field)
            };
        }

        public static DataMatrix TestData()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            dataSet.Tables[0].Columns.Add("TestCol", typeof(string));
            var row = dataSet.Tables[0].NewRow();
            row["TestCol"] = "xpto";
            dataSet.Tables[0].Rows.Add(row);
            return new DataMatrix(dataSet);
        }

        [Test]
        public void GetOdsTest()
        {
            //Arrange
            var exporter = new ExportToODS();
            var columns = TestColumns();
            var data = TestData();

            //Act
            var bytes = exporter.GetOds(data, columns, null);
            
            //Assert
            var stream = new MemoryStream(bytes);
            var content = exporter.GetContentXmlFile(stream);
            Assert.AreEqual("#document", content.Name);
            Assert.That(content.OuterXml.Contains("Test Column"), "The column description wasn't found");
            Assert.That(content.OuterXml.Contains("xpto"), "The value wasn't found");
        }
    }
}
