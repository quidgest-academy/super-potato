using CSGenio.framework;
using CSGenio.persistence;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using static CSGenio.framework.Exports;

namespace CSGenio.framework;

public class ExportToODS
{
    // Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
    private static string[,] namespaces = new string[,]
    {
    {"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
    {"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
    {"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
    {"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},
    {"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
    {"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
    {"dc", "http://purl.org/dc/elements/1.1/"},
    {"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
    {"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
    {"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
    {"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
    {"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
    {"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
    {"math", "http://www.w3.org/1998/Math/MathML"},
    {"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
    {"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
    {"ooo", "http://openoffice.org/2004/office"},
    {"ooow", "http://openoffice.org/2004/writer"},
    {"oooc", "http://openoffice.org/2004/calc"},
    {"dom", "http://www.w3.org/2001/xml-events"},
    {"xforms", "http://www.w3.org/2002/xforms"},
    {"xsd", "http://www.w3.org/2001/XMLSchema"},
    {"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
    {"rpt", "http://openoffice.org/2005/report"},
    {"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
    {"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
    {"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
    };

    public Stream GetOdsTemplateStream()
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream("CSGenio.core.resources.Exports.template.ods");
    }

    public XmlDocument GetContentXmlFile(Stream zipStream)
    {
        // Get file(in zip archive) that contains data ("content.xml").
        ZipArchive zipFile = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var contentZipEntry = zipFile.GetEntry("content.xml");               

        var stream = contentZipEntry.Open();

        // Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
        XmlDocument contentXml = new XmlDocument();
        contentXml.Load(stream);

        return contentXml;
    }

    private XmlNamespaceManager InitializeXmlNamespaceManager(XmlDocument xmlDocument)
    {
        XmlNamespaceManager nmsManager = new XmlNamespaceManager(xmlDocument.NameTable);

        for (int i = 0; i < namespaces.GetLength(0); i++)
            nmsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);

        return nmsManager;
    }

    // In ODF sheet is stored in table:table node
    private XmlNodeList GetTableNodes(XmlDocument contentXmlDocument, XmlNamespaceManager nmsManager)
    {
        return contentXmlDocument.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
    }

    /// <summary>
    /// Generate OpenDocument SpreadSheet file with table content.
    /// </summary>
    public byte[] GetOds(DataMatrix values, List<QColumn> columns, User user = null)
    {
        var templateStream = GetOdsTemplateStream();
        XmlDocument contentXml = this.GetContentXmlFile(templateStream);

        XmlNamespaceManager nmsManager = this.InitializeXmlNamespaceManager(contentXml);

        XmlNode automaticStylesNode = this.GetAutomaticStylesNode(contentXml, nmsManager);

        this.CreateColumnsStyles(automaticStylesNode, columns.Count);
        this.CreateRowsStyles(automaticStylesNode);

        XmlNode sheetsRootNode = this.GetSheetsRootNodeAndRemoveChildrens(contentXml, nmsManager);

        this.SaveSheet(values, sheetsRootNode, columns, user);

        MemoryStream ms = SaveContentXml(contentXml);

        byte[] buffer = new byte[ms.Length];
        ms.Seek(0, SeekOrigin.Begin);
        ms.Flush();
        ms.Read(buffer, 0, (int)ms.Length);
        return buffer;
    }

    private XmlNode GetAutomaticStylesNode(XmlDocument contentXml, XmlNamespaceManager nmsManager)
    {
        return contentXml.SelectNodes("/office:document-content/office:automatic-styles", nmsManager).Item(0);
    }

    private void CreateColumnsStyles(XmlNode automaticStylesNode, int numColumns)
    {
        XmlDocument ownerDocument = automaticStylesNode.OwnerDocument;

        for (int i = 0; i < numColumns; i++)
        {
            XmlElement style = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));

            XmlAttribute styleName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
            styleName.Value = "co" + (i + 2).ToString();
            style.Attributes.Append(styleName);

            XmlAttribute styleFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
            styleFamily.Value = "table-column";
            style.Attributes.Append(styleFamily);

            XmlElement tableColumnProperties = ownerDocument.CreateElement("style:table-column-properties", this.GetNamespaceUri("style"));

            XmlAttribute foBreakBefore = ownerDocument.CreateAttribute("fo:break-before", this.GetNamespaceUri("fo"));
            foBreakBefore.Value = "auto";
            tableColumnProperties.Attributes.Append(foBreakBefore);

            //XmlAttribute styleColumnWidth = ownerDocument.CreateAttribute("style:column-width", this.GetNamespaceUri("style"));
            //double tableWidth = 27.5; //Vamos assumir a largura de uma pagina A4 menos 2,2cm de margens. Ou seja 29.7 - 2.2 = 27.5cm
            //double colPercent = (col.Size * tableWidth) / QColumn.Sum(columns);
            //styleColumnWidth.Value = Math.Round(colPercent, 2).ToString().Replace(",", ".") + "cm";
            //styleColumnWidth.Value = "auto";
            //tableColumnProperties.Attributes.Append(styleColumnWidth);

            XmlAttribute styleAutoWidth = ownerDocument.CreateAttribute("style:use-optimal-column-width", this.GetNamespaceUri("style"));
            styleAutoWidth.Value = "true";
            tableColumnProperties.Attributes.Append(styleAutoWidth);

            style.AppendChild(tableColumnProperties);

            automaticStylesNode.AppendChild(style);
        }
    }

    private void CreateRowsStyles(XmlNode automaticStylesNode)
    {
        XmlDocument ownerDocument = automaticStylesNode.OwnerDocument;

        #region Style Cell Header
        XmlElement styleHeader = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));
        XmlAttribute styleName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
        styleName.Value = "ce1";
        styleHeader.Attributes.Append(styleName);
        XmlAttribute styleFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
        styleFamily.Value = "table-cell";
        styleHeader.Attributes.Append(styleFamily);
        XmlAttribute parentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", this.GetNamespaceUri("style"));
        parentStyleName.Value = "Default";
        styleHeader.Attributes.Append(parentStyleName);

        XmlElement tableCellProperties = ownerDocument.CreateElement("style:table-cell-properties", this.GetNamespaceUri("style"));
        XmlAttribute foBackColor = ownerDocument.CreateAttribute("fo:background-color", this.GetNamespaceUri("fo"));
        foBackColor.Value = "#008DD2";
        tableCellProperties.Attributes.Append(foBackColor);
        XmlAttribute textAlignSource = ownerDocument.CreateAttribute("style:text-align-source", this.GetNamespaceUri("style"));
        textAlignSource.Value = "fix";
        tableCellProperties.Attributes.Append(textAlignSource);
        XmlAttribute repeatContent = ownerDocument.CreateAttribute("style:repeat-content", this.GetNamespaceUri("style"));
        repeatContent.Value = "false";
        tableCellProperties.Attributes.Append(repeatContent);
        XmlAttribute foBorder = ownerDocument.CreateAttribute("fo:border", this.GetNamespaceUri("fo"));
        //foBorder.Value = "0.002cm solid #000000";
        tableCellProperties.Attributes.Append(foBorder);

        XmlElement paragraphProperties = ownerDocument.CreateElement("style:paragraph-properties", this.GetNamespaceUri("style"));
        XmlAttribute textAlign = ownerDocument.CreateAttribute("fo:text-align", this.GetNamespaceUri("fo"));
        textAlign.Value = "center";
        paragraphProperties.Attributes.Append(textAlign);
        XmlAttribute marginLeft = ownerDocument.CreateAttribute("fo:margin-left", this.GetNamespaceUri("fo"));
        marginLeft.Value = "0cm";
        paragraphProperties.Attributes.Append(marginLeft);

        XmlElement textProperties = ownerDocument.CreateElement("style:text-properties", this.GetNamespaceUri("style"));
        XmlAttribute color = ownerDocument.CreateAttribute("fo:color", this.GetNamespaceUri("fo"));
        color.Value = "#FFFFFF";
        textProperties.Attributes.Append(color);
        #endregion

        styleHeader.AppendChild(tableCellProperties);
        styleHeader.AppendChild(paragraphProperties);
        styleHeader.AppendChild(textProperties);
        automaticStylesNode.AppendChild(styleHeader);

        #region Style Cell Body
        XmlElement styleBody = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));
        XmlAttribute styleBodyName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
        styleBodyName.Value = "ce2";
        styleBody.Attributes.Append(styleBodyName);
        XmlAttribute styleBodyFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
        styleBodyFamily.Value = "table-cell";
        styleBody.Attributes.Append(styleBodyFamily);
        XmlAttribute styleBodyParentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", this.GetNamespaceUri("style"));
        styleBodyParentStyleName.Value = "Default";
        styleBody.Attributes.Append(styleBodyParentStyleName);

        XmlElement styleBodyTableCellProperties = ownerDocument.CreateElement("style:table-cell-properties", this.GetNamespaceUri("style"));
        XmlAttribute styleBodyTextAlignSource = ownerDocument.CreateAttribute("style:text-align-source", this.GetNamespaceUri("style"));
        styleBodyTextAlignSource.Value = "fix";
        styleBodyTableCellProperties.Attributes.Append(styleBodyTextAlignSource);
        XmlAttribute styleBodyRepeatContent = ownerDocument.CreateAttribute("style:repeat-content", this.GetNamespaceUri("style"));
        styleBodyRepeatContent.Value = "false";
        styleBodyTableCellProperties.Attributes.Append(styleBodyRepeatContent);
        XmlAttribute styleBodyFoBorder = ownerDocument.CreateAttribute("fo:border", this.GetNamespaceUri("fo"));
        //styleBodyFoBorder.Value = "0.002cm solid #000000";
        styleBodyTableCellProperties.Attributes.Append(styleBodyFoBorder);

        XmlElement styleBodyParagraphProperties = ownerDocument.CreateElement("style:paragraph-properties", this.GetNamespaceUri("style"));
        XmlAttribute styleBodyTextAlign = ownerDocument.CreateAttribute("fo:text-align", this.GetNamespaceUri("fo"));
        styleBodyTextAlign.Value = "start";
        styleBodyParagraphProperties.Attributes.Append(styleBodyTextAlign);
        XmlAttribute styleBodyMarginLeft = ownerDocument.CreateAttribute("fo:margin-left", this.GetNamespaceUri("fo"));
        styleBodyMarginLeft.Value = "0cm";
        styleBodyParagraphProperties.Attributes.Append(styleBodyMarginLeft);
        #endregion

        styleBody.AppendChild(styleBodyTableCellProperties);
        styleBody.AppendChild(styleBodyParagraphProperties);
        automaticStylesNode.AppendChild(styleBody);

        #region Style Alternative Cell Body
        XmlElement altstyleBody = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));
        XmlAttribute altstyleBodyName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
        altstyleBodyName.Value = "ce3";
        altstyleBody.Attributes.Append(altstyleBodyName);
        XmlAttribute altstyleBodyFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
        altstyleBodyFamily.Value = "table-cell";
        altstyleBody.Attributes.Append(styleBodyFamily);
        XmlAttribute altstyleBodyParentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", this.GetNamespaceUri("style"));
        altstyleBodyParentStyleName.Value = "Default";
        altstyleBody.Attributes.Append(altstyleBodyParentStyleName);

        XmlElement altstyleBodyTableCellProperties = ownerDocument.CreateElement("style:table-cell-properties", this.GetNamespaceUri("style"));
        XmlAttribute altfoBackColor = ownerDocument.CreateAttribute("fo:background-color", this.GetNamespaceUri("fo"));
        altfoBackColor.Value = "#d3d3d3";
        altstyleBodyTableCellProperties.Attributes.Append(altfoBackColor);
        XmlAttribute altstyleBodyTextAlignSource = ownerDocument.CreateAttribute("style:text-align-source", this.GetNamespaceUri("style"));
        altstyleBodyTextAlignSource.Value = "fix";
        altstyleBodyTableCellProperties.Attributes.Append(altstyleBodyTextAlignSource);
        XmlAttribute altstyleBodyRepeatContent = ownerDocument.CreateAttribute("style:repeat-content", this.GetNamespaceUri("style"));
        altstyleBodyRepeatContent.Value = "false";
        altstyleBodyTableCellProperties.Attributes.Append(altstyleBodyRepeatContent);
        XmlAttribute altstyleBodyFoBorder = ownerDocument.CreateAttribute("fo:border", this.GetNamespaceUri("fo"));
        //altstyleBodyFoBorder.Value = "0.002cm solid #000000";
        altstyleBodyTableCellProperties.Attributes.Append(altstyleBodyFoBorder);

        XmlElement altstyleBodyParagraphProperties = ownerDocument.CreateElement("style:paragraph-properties", this.GetNamespaceUri("style"));
        XmlAttribute altstyleBodyTextAlign = ownerDocument.CreateAttribute("fo:text-align", this.GetNamespaceUri("fo"));
        altstyleBodyTextAlign.Value = "start";
        altstyleBodyParagraphProperties.Attributes.Append(altstyleBodyTextAlign);
        XmlAttribute altstyleBodyMarginLeft = ownerDocument.CreateAttribute("fo:margin-left", this.GetNamespaceUri("fo"));
        altstyleBodyMarginLeft.Value = "0cm";
        altstyleBodyParagraphProperties.Attributes.Append(altstyleBodyMarginLeft);
        #endregion

        altstyleBody.AppendChild(altstyleBodyTableCellProperties);
        altstyleBody.AppendChild(altstyleBodyParagraphProperties);
        automaticStylesNode.AppendChild(altstyleBody);
    }

    private XmlNode GetSheetsRootNodeAndRemoveChildrens(XmlDocument contentXml, XmlNamespaceManager nmsManager)
    {
        XmlNodeList tableNodes = this.GetTableNodes(contentXml, nmsManager);

        XmlNode sheetsRootNode = tableNodes.Item(0).ParentNode;
        // remove sheets from template file
        foreach (XmlNode tableNode in tableNodes)
            sheetsRootNode.RemoveChild(tableNode);

        return sheetsRootNode;
    }

    private void SaveSheet(DataMatrix values, XmlNode sheetsRootNode, List<QColumn> columns, User user = null)
    {
        XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;
        XmlNode sheetNode = ownerDocument.CreateElement("table:table", this.GetNamespaceUri("table"));

        XmlAttribute sheetName = ownerDocument.CreateAttribute("table:name", this.GetNamespaceUri("table"));
        sheetName.Value = "Folha";
        sheetNode.Attributes.Append(sheetName);

        XmlAttribute styleName = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
        styleName.Value = "ta1";
        sheetNode.Attributes.Append(styleName);

        XmlAttribute print = ownerDocument.CreateAttribute("table:print", this.GetNamespaceUri("table"));
        print.Value = "false";
        sheetNode.Attributes.Append(print);

        this.SaveColumnDefinition(columns.Count, sheetNode, ownerDocument);

        this.CreateTableHeaders(sheetNode, ownerDocument, columns);

        this.SaveRows(values, columns, sheetNode, ownerDocument, user);

        sheetsRootNode.AppendChild(sheetNode);
    }

    private void SaveColumnDefinition(int numColumns, XmlNode sheetNode, XmlDocument ownerDocument)
    {
        for (int i = 0; i < numColumns; i++)
        {
            XmlNode columnDefinition = ownerDocument.CreateElement("table:table-column", this.GetNamespaceUri("table"));

            XmlAttribute styleCell = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
            styleCell.Value = "co" + (i + 2).ToString();
            columnDefinition.Attributes.Append(styleCell);

            XmlAttribute defaultStyleCell = ownerDocument.CreateAttribute("table:default-cell-style-name", this.GetNamespaceUri("table"));
            defaultStyleCell.Value = "ce2";
            columnDefinition.Attributes.Append(defaultStyleCell);

            sheetNode.AppendChild(columnDefinition);
        }
    }

    private void CreateTableHeaders(XmlNode sheetNode, XmlDocument ownerDocument, List<QColumn> columns)
    {
        XmlNode rowNode = ownerDocument.CreateElement("table:table-row", this.GetNamespaceUri("table"));
        XmlAttribute style = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
        style.Value = "ro1";
        rowNode.Attributes.Append(style);

        foreach (var col in columns)
        {
            XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", this.GetNamespaceUri("table"));

            XmlAttribute styleCell = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
            styleCell.Value = "ce1";
            cellNode.Attributes.Append(styleCell);

            XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", this.GetNamespaceUri("office"));
            valueType.Value = "string";
            cellNode.Attributes.Append(valueType);

            XmlElement cellValue = ownerDocument.CreateElement("text:p", this.GetNamespaceUri("text"));
            cellValue.InnerText = col.Description;
            cellNode.AppendChild(cellValue);

            rowNode.AppendChild(cellNode);
        }

        sheetNode.AppendChild(rowNode);
    }

    private void SaveRows(DataMatrix values, List<QColumn> columns, XmlNode sheetNode, XmlDocument ownerDocument, User user = null)
    {
        for (int i = 0; i < values.NumRows; i++)
        {
            XmlNode rowNode = ownerDocument.CreateElement("table:table-row", this.GetNamespaceUri("table"));
            XmlAttribute style = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
            style.Value = "ro1";
            rowNode.Attributes.Append(style);

            for (int c = 0; c < columns.Count; c++)
            {
                this.SaveCell(values.GetDirect(i, columns[c].Name), columns[c], rowNode, ownerDocument, i, user);
            }

            sheetNode.AppendChild(rowNode);
        }
    }

    private void SaveCell(object value, QColumn column, XmlNode rowNode, XmlDocument ownerDocument, int rowIndex, User user = null)
    {
        XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", this.GetNamespaceUri("table"));

        XmlAttribute styleCell = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
        styleCell.Value = (rowIndex % 2 == 0 ? "ce2" : "ce3");
        cellNode.Attributes.Append(styleCell);

        // We save values as text (string)
        XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", this.GetNamespaceUri("office"));
        valueType.Value = "string";
        cellNode.Attributes.Append(valueType);

        XmlElement cellValue = ownerDocument.CreateElement("text:p", this.GetNamespaceUri("text"));
        cellValue.InnerText = getTextFromData(value, column, user);
        cellNode.AppendChild(cellValue);

        rowNode.AppendChild(cellNode);
    }

    private MemoryStream SaveContentXml(XmlDocument contentXml)
    {
        MemoryStream zipStream = new MemoryStream();
        
        var templateStream = GetOdsTemplateStream();
        // Copy the resource stream to the writable MemoryStream
        templateStream.CopyTo(zipStream);
        zipStream.Seek(0, SeekOrigin.Begin);

        // Open the ZIP archive in Update mode
        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Update, leaveOpen: true))
        {
            // Remove existing content.xml 
            var existingEntry = archive.GetEntry("content.xml");
            existingEntry.Delete();

            // Create a new content.xml entry in the ZIP archive
            var newEntry = archive.CreateEntry("content.xml");

            // Save the XmlDocument directly to the entry stream
            using (Stream entryStream = newEntry.Open())
            {
                contentXml.Save(entryStream);
            }
            
        }
        return zipStream;
    }

    private string GetNamespaceUri(string prefix)
    {
        for (int i = 0; i < namespaces.GetLength(0); i++)
        {
            if (namespaces[i, 0] == prefix)
                return namespaces[i, 1];
        }

        throw new InvalidOperationException("Can't find that namespace URI");
    }
}

