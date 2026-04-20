using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PdfSharpCore.Drawing;

namespace CSGenio.framework
{
    public class Exports
    {
        private User user;
        private List<QColumn> colunas = null;

        public Exports(User user)
        {
            this.user = user;
        }

        public byte[] ExportList(string listingControl, CriteriaSet conditions, IList<ColumnSort> orderBy, string exportType, string namedbedit)
        {
            return ExportList(listingControl, conditions, orderBy, exportType, null, namedbedit);
        }

        public byte[] ExportList(string listingControl, CriteriaSet conditions, IList<ColumnSort> orderBy, string exportType, string filename, string namedbedit)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

            IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos =
                PersistentSupport.getControlQueries();
            IDictionary<string, PersistentSupport.overrideDbeditQuery> controlosOverride =
                PersistentSupport.getControlQueriesOverride();

            SelectQuery qs = null;

            if (controlosOverride.ContainsKey(listingControl))
            {
                qs = controlosOverride[listingControl](user, "", conditions, orderBy, sp);
            }
            else
            {
                PersistentSupport.ControlQueryDefinition aux = controlos[listingControl];
                qs = new SelectQuery();
                foreach (SelectField field in aux.SelectFields)
                {
                    qs.SelectFields.Add(field);
                }
                qs.FromTable = aux.FromTable;
                foreach (TableJoin join in aux.Joins)
                {
                    qs.Joins.Add(join);
                }
                qs.Where(CriteriaSet.And()
                    .SubSet(aux.WhereConditions)
                    .SubSet(conditions));
            }

            if (qs.OrderByFields.Count == 0)
            {
                foreach (ColumnSort sort in orderBy)
                {
                    qs.OrderByFields.Add(sort);
                }
            }

            return ExportList(qs, exportType, filename, namedbedit);
        }

        public byte[] ExportList<A>(ListingMVC<A> listing, CriteriaSet conditions, List<QColumn> columns, string exportType, string namedbedit) where A : IArea
        {
            return ExportList<A>(listing, conditions, columns, exportType, null, namedbedit);
        }

        public byte[] ExportList<A>(ListingMVC<A> listing, CriteriaSet conditions, List<QColumn> columns, string exportType, string filename, string namedbedit) where A : IArea
        {
            this.colunas = columns;
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            // TODO: Protect against cases where it receive zero columns.
            //  Otherwise, it will select all columns in the area.
            SelectQuery qs = sp.getSelectQueryFromListingMVC<A>(conditions, listing);

            return ExportList(qs, exportType, filename,namedbedit);
        }

        private byte[] ExportList(SelectQuery qs, string exportType, string filename,string namedbedit)
        {
            ExportType type = this.getExportType(exportType);
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

            //get data
            sp.openConnection();
            DataMatrix res = sp.Execute(qs);
            sp.closeConnection();

            if (colunas == null)
            {
                colunas = new List<QColumn>();
                //get column
                foreach (DataColumn dc in res.DbDataSet.Tables[0].Columns)
                {
                    string[] colcap = dc.Caption.Split('.');
                    AreaInfo ainfo = business.Area.GetInfoArea(colcap[0]);
                    colunas.Add(new QColumn(dc.Caption, ainfo.DBFields[colcap[1]]));
                }
            }

            return ExportList(res, exportType, filename, namedbedit);
        }

        public byte[] ExportList(DataMatrix values, List<QColumn> columns, string exportType, string filename, string namedbedit)
        {
            this.colunas = columns;
            return ExportList(values, exportType, filename, namedbedit);
        }

        public byte[] ExportTemplate(List<QColumn> columns, string exportType, string filename, string namedbedit)
        {
            this.colunas = columns;
            DataTable dt = new DataTable();
            foreach (QColumn campopedido in columns)
            {
                dt.Columns.Add(campopedido.Name, campopedido.Type.GetExternalType());
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            DataMatrix values = new DataMatrix(ds);
            return ExportList(values, exportType, filename, namedbedit);
        }

        public byte[] ExportList(DataMatrix values, string exportType, string filename, string namedbedit)
        {
            ExportType type = this.getExportType(exportType);
            byte[] fileBinary = null;

            switch (type)
            {
                case ExportType.pdf:
                    ExportToPDF pdf = new ExportToPDF();
                    return pdf.GetPDF(values, colunas, namedbedit, user);

                case ExportType.xlsx:
                    ExportToExcel excel = new ExportToExcel();
                    return excel.GetExcel(filename, values, colunas, namedbedit, user);

                case ExportType.ods:
                    ExportToODS ods = new ExportToODS();
                    return ods.GetOds(values, colunas, user);

                case ExportType.csv:
                    ExportToCSV csv = new ExportToCSV();
                    return csv.GetCSV(values, colunas, user);

                case ExportType.xml:
                    ExportToXML xml = new ExportToXML();
                    return xml.GetXML(values, colunas, user);
            }

            return fileBinary;
        }

        public bool ExportListValidation<A>(ListingMVC<A> listing, CriteriaSet conditions, List<QColumn> columns, string exportType) where A : IArea
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            SelectQuery qs = sp.getSelectQueryFromListingMVC<A>(conditions, listing);

            //get data
            sp.openConnection();
            DataMatrix res = sp.Execute(qs);
            sp.closeConnection();


            ExportType type = this.getExportType(exportType);

            switch (type)
            {
                case ExportType.pdf:
                    return ExportToPDF.ValidatePage(columns, res, user);
            }

            return true;
        }

        private ExportType getExportType(string exportType)
        {
            switch (exportType)
            {
                case "csv":
                    return ExportType.csv;

                case "xlsx":
                case "xls":
                    return ExportType.xlsx;

                case "ods":
                    return ExportType.ods;

                case "pdf":
                    return ExportType.pdf;

                case "xml":
                    return ExportType.xml;

                default:
                    throw new FrameworkException("Tipo de exportação não suportado!", "Exports.getExportType", "Tipo de exportação '" + exportType + "' inexistente");
            }
        }

        #region Export

        private enum ExportType
        {
            csv,
            xlsx,
            ods,
            pdf,
            xml
        }

        public class QColumn
        {
            /// <summary>
            /// The FieldRef corresponding to the exported field.
            /// </summary>
            /// <remarks>
            /// The property may be <c>null</c>.
            /// Currently, it's only used for optimized export of list controls in Vue.
            /// </remarks>
            public FieldRef Field { get; private set; }
            public string Name { get; private set; }
            public FieldType Type { get; private set; }
            public FieldFormatting Formatting { get; private set; }
            public string ArrayName { get; private set; }
            public string Description { get; private set; }
            public int Size { get; private set; }
            public int Decimals { get; private set; }
            public bool Visible { get; set; }
            public bool AlwaysExportable { get; set; }

            public QColumn(FieldRef field, FieldType fieldType, string descricao, int size, int decimais, bool visivel, string arrayName = "", bool alwaysExportable = false)
            {
                this.Field = field;
                this.Name = field.FullName;
                this.Type = fieldType;
                this.Formatting = fieldType.GetFormatting();
                this.ArrayName = arrayName;
                this.Description = descricao;
                this.Size = size;
                this.Decimals = decimais;
                this.Visible = visivel;
                this.AlwaysExportable = alwaysExportable;
            }

            public QColumn(string fieldName, Field campoBD)
            {
                this.Name = fieldName;
                this.Description = campoBD.FieldDescription;
                this.Size = Math.Max(campoBD.FieldSize, campoBD.FieldDescription.Length) * 8;
                this.Decimals = campoBD.Decimals;
                this.Type = campoBD.FieldType;
                this.Formatting = campoBD.FieldFormat;
                this.Visible = true;
                this.ArrayName = String.IsNullOrEmpty(campoBD.ArrayName) ? null : campoBD.ArrayName;
            }

            private string m_BaseArea;
            public string BaseArea
            {
                get
                {
                    if (this.m_BaseArea == null)
                    {
                        this.m_BaseArea = this.Name.Split('.')[0];
                        this.m_FieldName = this.Name.Split('.')[1];
                    }

                    return this.m_BaseArea;
                }
            }
            private string m_FieldName;
            public string FieldName
            {
                get
                {
                    if (this.m_FieldName == null)
                    {
                        this.m_BaseArea = this.Name.Split('.')[0];
                        this.m_FieldName = this.Name.Split('.')[1];
                    }

                    return this.m_FieldName;
                }
            }

            public static int Sum(IList<QColumn> columns)
            {
                int sum = 0;

                foreach (QColumn col in columns)
                    sum += col.Size;

                return sum;
            }

            public static List<QColumn> GetVisibleColumns(IList<QColumn> columns)
            {
                List<QColumn> visibleColumns = new List<QColumn>();

                foreach (QColumn col in columns)
                    if (col.Visible)
                        visibleColumns.Add(col);

                return visibleColumns;
            }
        }

        private class ExportToPDF
        {
            readonly static double TableMaxWidth = PdfSharpCore.PageSizeConverter.ToSize(PdfSharpCore.PageSize.A4).Height - Unit.FromCentimeter(2.2).Point;
            readonly static double WidthScaleFactorMin = 0.5; // do not reduce column width by more than half
            readonly static Color TableBorder = new Color(0, 0, 0);
            readonly static Color TableBlue = new Color(235, 240, 249);
            readonly static Color TableGray = new Color(242, 242, 242);
            readonly static Font HeaderFont = new Font("Arial", 12);
            readonly static Font TableFont = new Font("Arial", 10);
            readonly static Unit TableBorderWidth = new Unit(1);
            readonly static XGraphics DefaultGraphics = XGraphics.CreateMeasureContext(new XSize(2000, 2000), XGraphicsUnit.Point, XPageDirection.Downwards);

            private Document document;
            private Table table;

            /// <summary>
            /// Generate pdf document with table content.
            /// </summary>
            public byte[] GetPDF(DataMatrix values, List<QColumn> columns, string namedbedit, User user = null)
            {
                // Create a MigraDoc document
                this.document = new Document();
                document.UseCmykColor = false;
                document.DefaultPageSetup.PageFormat = PageFormat.A4;
                document.DefaultPageSetup.Orientation = Orientation.Landscape;
                document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.FooterDistance = Unit.FromCentimeter(0.5);

                PageSetup pgSetup = document.DefaultPageSetup.Clone();
                document.Info.Title = namedbedit;

                DefineStyles();

                CreatePage(columns, values, namedbedit, user);

                FillContent(columns, values, user);

                // Create a renderer for PDF that uses Unicode font encoding
                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                byte[] buffer;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdfRenderer.Save(ms, false);
                    buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);
                }

                return buffer;
            }

            /// <summary>
            /// Defines the styles used to format the MigraDoc document.
            /// </summary>
            private void DefineStyles()
            {
                // Get the predefined style Normal.
                Style style = this.document.Styles["Normal"];
                // Because all styles are derived from Normal, the next line changes the
                // font of the whole document. Or, more exactly, it changes the font of
                // all styles and paragraphs that do not redefine the font.
                style.Font = TableFont.Clone();

                style = this.document.Styles[StyleNames.Header];
                style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

                style = this.document.Styles[StyleNames.Footer];
                style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

                // Create a new style called Table based on style Normal
                style = this.document.Styles.AddStyle("Table", "Normal");
                style.Font = TableFont.Clone();
                style.Font.Color = Colors.Black;

                // Create a new style called Reference based on style Normal
                style = this.document.Styles.AddStyle("Reference", "Normal");
                style.ParagraphFormat.SpaceBefore = "5mm";
                style.ParagraphFormat.SpaceAfter = "5mm";
                style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
            }

            /// <summary>
            /// Check if the content can fit the PDF page.
            /// PDFSharp/MigraDoc does not split tables by column/vertically.
            /// </summmary>
            public static bool ValidatePage(List<QColumn> columns, DataMatrix values, User user = null)
            {
                // Measure strings for column width estimation
                Font tableHeaderFont = HeaderFont.Clone();
                tableHeaderFont.Bold = true;
                TextMeasurement tmHeader = new TextMeasurement(DefaultGraphics, tableHeaderFont);
                TextMeasurement tmBody = new TextMeasurement(DefaultGraphics, TableFont);
                double totalColWidth = 0;

                for (int i = 0; i < columns.Count; i++)
                {
                    // Minimum column width, based on column title
                    double minColWidth =
                        tmHeader.MeasureString(columns[i].Description, UnitType.Point).Width +
                        TableBorderWidth + Unit.FromCentimeter(0.5).Point;

                    // Find the average width from column values
                    double totalRowWidth = 0;

                    // Look at the first 100 rows in order to determine medium width
                    string columnName = columns[i].Name;
                    int currRow = 0;
                    int maxRows = values.NumRows < 100 ? values.NumRows - 1 : 100;
                    while (currRow <= maxRows)
                    {
                        string rowVal = getTextFromData(values.GetDirect(currRow, columnName), columns[i], user);
                        double colValMeasure = tmBody.MeasureString(rowVal, UnitType.Point).Width + TableBorderWidth + Unit.FromCentimeter(0.25).Point;
                        totalRowWidth += colValMeasure;
                        currRow++;
                    }

                    double columnWidth = (totalRowWidth + minColWidth) / (maxRows + 1);

                    // Enforce minimum column width
                    if (columnWidth < minColWidth)
                        columnWidth = minColWidth;

                    // Add column width to total columns width
                    totalColWidth += columnWidth;
                }

                double scaleFactor = TableMaxWidth / totalColWidth;
                return scaleFactor >= WidthScaleFactorMin;
            }

            /// <summary>
            /// Creates the static parts of the PDF document.
            /// </summary>
            private void CreatePage(List<QColumn> columns, DataMatrix values, String namebdedit, User user = null)
            {
                // Each MigraDoc document needs at least one section.
                Section section = this.document.AddSection();
                section.PageSetup.StartingNumber = 1;

                // Add page header
                Paragraph to = new Paragraph();
                to.Format.Alignment = ParagraphAlignment.Center;
                to.Format.Font = HeaderFont.Clone();
                to.Format.Font.Bold = true;
                //to.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
                to.Format.Font.Color = Colors.Black;
                to.AddText(namebdedit);
                section.Headers.Primary.Add(to);
                section.AddParagraph();
                section.AddParagraph();

                // Add page footer
                Paragraph footer = new Paragraph();
                footer.AddPageField();
                footer.AddChar(Chars.Hyphen);
                footer.AddNumPagesField();
                footer.Format.Alignment = ParagraphAlignment.Center;
                section.Footers.Primary.Add(footer);

                // Create the item table
                this.table = section.AddTable();
                this.table.Style = "Table";
                this.table.Borders.Color = TableBorder;
                this.table.Borders.Width = TableBorderWidth;
                this.table.Borders.Left.Width = 0.5;
                this.table.Borders.Right.Width = 0.5;
                this.table.Rows.LeftIndent = 0;

                // Measure strings for column width estimation
                Font tableHeaderFont = HeaderFont.Clone();
                tableHeaderFont.Bold = true;
                TextMeasurement tmHeader = new TextMeasurement(DefaultGraphics, tableHeaderFont);
                TextMeasurement tmBody = new TextMeasurement(DefaultGraphics, this.document.Styles["Table"].Font.Clone());
                double totalColWidth = 0;

                for (int i = 0; i < columns.Count; i++)
                {
                    // Minimum column width, based on column title
                    double minColWidth =
                        tmHeader.MeasureString(columns[i].Description, UnitType.Point).Width +
                        this.table.Borders.Width + Unit.FromCentimeter(0.5).Point;

                    // Find the average width from column values
                    double totalRowWidth = 0;

                    // Look at the first 100 rows in order to determine medium width
                    string columnName = columns[i].Name;
                    int currRow = 0;
                    int maxRows = values.NumRows < 100 ? values.NumRows - 1 : 100;
                    while (currRow <= maxRows)
                    {
                        string rowVal = getTextFromData(values.GetDirect(currRow, columnName), columns[i], user);
                        double colValMeasure = tmBody.MeasureString(rowVal, UnitType.Point).Width + this.table.Borders.Width + Unit.FromCentimeter(0.25).Point;
                        totalRowWidth += colValMeasure;
                        currRow++;
                    }

                    double columnWidth = (totalRowWidth + minColWidth) / (maxRows + 1);

                    // Enforce minimum column width
                    if (columnWidth < minColWidth)
                        columnWidth = minColWidth;

                    // Create column
                    Column column = this.table.AddColumn(Unit.FromPoint(columnWidth));
                    column.Format.Alignment = ParagraphAlignment.Left;

                    // Add column width to total columns width
                    totalColWidth += columnWidth;
                }

                // Scale column width to the maximum table width
                // Scale factor < 1 - reduction
                // Scale factor > 1 - enlargement
                double scaleFactor = Math.Max(TableMaxWidth / totalColWidth, WidthScaleFactorMin);
                for (int i = 0; i < columns.Count; i++)
                {
                    this.table.Columns[i].Width = this.table.Columns[i].Width * scaleFactor;
                }

                // Create the header of the table
                Row row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.DodgerBlue;
                row.Format.Font.Color = Colors.White;

                this.table.SetEdge(0, 0, columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

                for (int c = 0; c < columns.Count; c++)
                {
                    row.Cells[c].VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[c].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[c], columns[c].Description));
                }
            }

            /// <summary>
            /// Creates the dynamic parts of the PDF document.
            /// </summary>
            private void FillContent(List<QColumn> columns, DataMatrix values, User user = null)
            {
                //run each row of the menu list
                for(int i = 0; i < values.NumRows; i++)
                {
                    Row rowPDF = this.table.AddRow();

                    if (rowPDF.Index % 2 == 0)
                    {
                        rowPDF.Shading.Color = Colors.LightGray;
                    }

                    this.table.SetEdge(0, this.table.Rows.Count - 1, columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75);

                    for (int c = 0; c < columns.Count; c++)
                    {
                        string text =  getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);

                        rowPDF.Cells[c].Format.FirstLineIndent = 0;
                        rowPDF.Cells[c].Format.LeftIndent = 0;
                        rowPDF.Cells[c].Format.RightIndent = 0.25;
                        rowPDF.Cells[c].AddParagraph(AdjustIfTooWideToFitIn(rowPDF.Cells[c], text));
                    }
                }
            }

            /// <summary>
            /// Adjust text value if it is too wide to fit inside the cell
            /// </summary>
            private string AdjustIfTooWideToFitIn(Cell cell, string text)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    Column column = cell.Column;
                    Unit availableWidth = column.Width - column.Table.Borders.Width - cell.Borders.Width - Unit.FromCentimeter(0.25).Point;

                    string[] splitted = text.Split(" ".ToCharArray());

                    Dictionary<string, string> distinct = new Dictionary<string, string>();
                    List<string> tooWideWords = new List<string>();
                    foreach (string s in splitted)
                        if (!distinct.ContainsKey(s))
                        {
                            distinct.Add(s, s);
                            if (TooWide(s, availableWidth))
                                tooWideWords.Add(s);
                        }

                    var adjusted = new StringBuilder(text);
                    foreach (string word in tooWideWords)
                    {
                        var replacementWord = MakeFit(word, availableWidth);
                        adjusted.Replace(word, replacementWord);
                    }

                    return adjusted.ToString();
                }
                else
                    return text;
            }

            /// <summary>
            /// Checks if the word is too wide
            /// </summary>
            private bool TooWide(string word, Unit width)
            {
                TextMeasurement tm = new TextMeasurement(DefaultGraphics, this.document.Styles["Table"].Font.Clone());
                double f = tm.MeasureString(word, UnitType.Point).Width;
                return f > width.Point;
            }

            /// <summary>
            /// Makes the supplied word fit into the available width
            /// </summary>
            /// <returns>modified version of the word with inserted Returns at appropriate points</returns>
            private string MakeFit(string word, Unit width)
            {
                var adjustedWord = new StringBuilder();
                var current = string.Empty;
                foreach (char c in word)
                {
                    if (TooWide(current + c, width))
                    {
                        adjustedWord.Append(current);
                        adjustedWord.Append(Chars.CR);
                        current = c.ToString();
                    }
                    else
                    {
                        current += c;
                    }
                }
                adjustedWord.Append(current);

                return adjustedWord.ToString();
            }
        }

        private class ExportToExcel
        {
            /// <summary>
            /// Generate Excel document with table content.
            /// </summary>
            public byte[] GetExcel(string fileName, DataMatrix values, List<QColumn> columns, string namedbedit, User user = null)
            {
                //temporary file path
                string DocumentPath = AppDomain.CurrentDomain.BaseDirectory + @"\temp\" + fileName;
                FileInfo ExcelFile = new FileInfo(DocumentPath);
                ExcelPackage xmlPackage = new ExcelPackage(ExcelFile);
                //create a worksheet
                ExcelWorksheet worksheet = xmlPackage.Workbook.Worksheets.Add("Excel");
                worksheet.HeaderFooter.FirstHeader.CenteredText = string.Format("&12&\"Arial,Regular Bold\" {0}", namedbedit);

                //fill first row with the columns name
                for (int i = 0; i < columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columns[i].Description;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 141, 210));
                    worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Font.Size = 11;
                    worksheet.Cells[1, i + 1].Style.Font.Name = "Arial";
                }

                int idx = 2;
                //run each row of the menu list
                for(int i = 0; i < values.NumRows; i++)
                {
                    for (int c = 0; c < columns.Count; c++)
                    {
                        if (idx % 2 != 0)
                        {
                            worksheet.Cells[idx, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[idx, c + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(241, 244, 249));
                        }

                        worksheet.Cells[idx, c + 1].Style.Font.Size = 10;
                        worksheet.Cells[idx, c + 1].Style.Font.Name = "Arial";

                        // If the value is null, column type is disregarded
                        if (values.GetDirect(i, columns[c].Name) == null || values.GetDirect(i, columns[c].Name) == DBNull.Value)
                        {
                            worksheet.Cells[idx, c + 1].Value = null;
                            continue;
                        }

                        // Verify column type
                        switch (columns[c].Type.GetFormatting())
                        {
                            case FieldFormatting.DATA:
                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = Configuration.DateFormat.Date;
                                worksheet.Cells[idx, c + 1].Value = values.GetDate(i, columns[c].Name).ToOADate();
                                break;

                            case FieldFormatting.DATAHORA:
                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = Configuration.DateFormat.DateTime;
                                worksheet.Cells[idx, c + 1].Value = values.GetDate(i, columns[c].Name).ToOADate();
                                break;

                            case FieldFormatting.DATASEGUNDO:
                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = Configuration.DateFormat.DateTimeSeconds;
                                worksheet.Cells[idx, c + 1].Value = values.GetDate(i, columns[c].Name).ToOADate();
                                break;

                            case FieldFormatting.INTEIRO:
                            case FieldFormatting.LOGICO:
                                // Ignore logical arrays
                                if (columns[c].Type == FieldType.ARRAY_LOGIC)
                                   goto default;

                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = "0";
                                worksheet.Cells[idx, c + 1].Value = values.GetInteger(i, columns[c].Name);
                                break;

                            case FieldFormatting.FLOAT:
                                // Ignore numeric arrays
                                if (columns[c].Type == FieldType.ARRAY_NUMERIC)
                                   goto default;

                                // Excel already changes number separators for localization
                                string numberFormat = "0";
                                int decimais = columns[c].Decimals;

								//BPM - Add 2 decimals for defaults in the money
                                if (columns[c].Type == FieldType.CURRENCY)
                                    decimais += 2;

                                if (decimais > 0)
                                {
                                    numberFormat += "." + new string('0', decimais);
                                }

                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = numberFormat;
                                worksheet.Cells[idx, c + 1].Value = values.GetNumeric(i, columns[c].Name);
                                break;

                            case FieldFormatting.CARACTERES:
                            default:
                                string text = getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);
                                string sanitizedText = SanitizeForSpreadsheet(text);
                                worksheet.Cells[idx, c + 1].Value = sanitizedText;
                                break;
                        }
                    }
                    idx++;
                }

                // Autofit columns to data values
                worksheet.Cells.AutoFitColumns();

                return xmlPackage.GetAsByteArray();
            }
        }

        private class ExportToCSV
        {
            /// <summary>
            /// Função que converte uma string com characters de quebras de linha to um Qvalue string válido
            /// </summary>
            /// <param name="valorCampo">Qvalue do Qfield</param>
            /// <returns>Qfield string formatado</returns>
            private string memo2String(string Qvalue)
            {
                if (Qvalue.Contains(";"))
                    Qvalue = Qvalue.Replace(";", ",");

                if (Qvalue.Contains("\n\r\n"))
                    Qvalue = Qvalue.Replace("\n\r\n", " ");

                if (Qvalue.Contains("\n\r"))
                    Qvalue = Qvalue.Replace("\n\r", " ");

                if (Qvalue.Contains("\r\n"))
                    Qvalue = Qvalue.Replace("\r\n", " ");

                if (Qvalue.Contains("\n"))
                    Qvalue = Qvalue.Replace("\n", " ");

                if (Qvalue.Contains("\r"))
                    Qvalue = Qvalue.Replace("\r", " ");

                return Qvalue;
            }

            /// <summary>
            /// Generate CSV file with table content.
            /// </summary>
            public byte[] GetCSV(DataMatrix values, List<QColumn> columns, User user = null)
            {
                StringBuilder conteudoCSV = new StringBuilder();

                foreach (var col in columns)
                    conteudoCSV.Append(col.Description + ";");

                conteudoCSV.Append(";\r\n");

                //preenche a table com os dados
                for (int i = 0; i < values.NumRows; i++)
                {
                    for (int c = 0; c < columns.Count; c++)
                    {
                        string text = getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);
                        string sanitizedText = SanitizeForSpreadsheet(text);
                        conteudoCSV.Append(memo2String(sanitizedText) + ";");
                    }
                    conteudoCSV.Append(";\r\n");
                }

                byte[] buffer;
                using (MemoryStream ms = new MemoryStream())
                {
                    var stringBytes = System.Text.Encoding.Default.GetBytes(conteudoCSV.ToString());
                    ms.Write(stringBytes, 0, stringBytes.Length);
                    buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);
                }

                return buffer;
            }
        }

        private class ExportToXML
        {
            /// <summary>
            /// Generate XML file with table content.
            /// </summary>
            public byte[] GetXML(DataMatrix values, List<QColumn> columns, User user = null)
            {
                List<string> tagname = new List<string>();
                foreach (var col in columns)
                    tagname.Add(col.Description);

                using (MemoryStream stream = new MemoryStream())
                {
                    XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("Table");
                    for(int i = 0; i < values.NumRows; i++)
                    {
                        List<string> Qvalues = new List<string>();
                        for (int c = 0; c < columns.Count; c++)
                        {
                            string text = getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);
                            Qvalues.Add(text);
                        }
                        CreateNode(tagname, Qvalues, writer);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();

                    byte[] byteArray = stream.ToArray();
                    writer.Close();

                    return byteArray;
                }
            }

            /// <summary>
            /// Create a XML Node
            /// </summary>
            private void CreateNode(List<string> tags, List<string> Qvalues, XmlTextWriter writer)
            {
                writer.WriteStartElement("Registo");
                for (int i = 0; i < tags.Count; i++ )
                {
                    writer.WriteStartElement(tags[i].Replace(" ", "_"));
                    writer.WriteString(Qvalues[i]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        internal static string getTextFromData(object data, QColumn column, User user = null)
        {
			string lang = "";
			if (user != null)
				lang = user.Language;
            string text = Conversion.internal2String(data, column.Type);
            if ((column.Type == FieldType.ARRAY_TEXT || column.Type == FieldType.ARRAY_NUMERIC || column.Type == FieldType.ARRAY_LOGIC)
                && !string.IsNullOrEmpty(column.ArrayName) && !string.IsNullOrEmpty(text))
            {
                ArrayInfo array = new ArrayInfo(column.ArrayName);
                if (array.Elements.Contains(text))// MH [21/03/2016] - Validação se o código exists. Caso contrario provoca erro de execução.
                    text = array.GetDescription(text, lang);
                else text = string.Empty;
            }

            return text;
        }

        /// <summary>
        /// Sanitizes text to prevent CSV/XLS formula injection attacks when the text is used in those files.
        /// This function wraps the text in double quotes, prepends with a single quote, and escapes
        /// any existing double quotes to ensure the content is treated as literal text by spreadsheet applications.
        /// </summary>
        /// <param name="input">The text to sanitize</param>
        /// <returns>Sanitized text safe for spreadsheet output</returns>
        internal static string SanitizeForSpreadsheet(string input)
        {
            // Handle null or empty input.
            if (string.IsNullOrEmpty(input))
                return input;

            // Check if input starts with dangerous characters or contains field separators/quotes.
            bool needsSanitization = input.Length > 0 &&
                (input[0] == '=' ||
                input[0] == '+' ||
                input[0] == '-' ||
                input[0] == '@' ||
                input[0] == '\t' ||  // Tab (0x09)
                input[0] == '\r' ||  // Carriage return (0x0D)
                input.Contains(",") ||
                input.Contains(";") ||
                input.Contains("\"") ||
                input.Contains("'"));

            if (needsSanitization)
            {
                // Escape any existing double quotes by doubling them.
                string escaped = input.Replace("\"", "\"\"");

                // Prepend with single quote and wrap in double quotes.
                return "\"'" + escaped + "\"";
            }

            return input;
        }

        #endregion

        #region Import

        public List<A> ImportList<A>(List<Exports.QColumn> columns, string exportType, byte[] file) where A : IArea
        {
            ExportType importType = getExportType(exportType);
            List<object[]> rows = new List<object[]>();
            int rowCount = 1;
            List<A> results = new List<A>();

            //import by file Type
            try
            {
                switch (importType)
                {
                    case ExportType.xlsx:
                        rows = ImportExcel(columns, file, ref rowCount);
                        break;
                }
            }
            catch (FormatException)
            {
                throw new FrameworkException(Translations.Get("Ficheiro com dados em formato incorreto"),
                    "Exports.ImportarListagem", Translations.Get("Ficheiro com dados em formato incorreto"));
            }

            if (!this.CheckIfRightFile(rows[0], columns))
            {
                throw new FrameworkException(Translations.Get("Ficheiro com cabeçalho incorrecto"),
                    "Exports.ImportarListagem", Translations.Get("Ficheiro com cabeçalho incorrecto"));
            }

            Dictionary<String, List<Exports.QColumn>> columnsByArea = this.GetColumnsByArea(columns);

            //Process values into DbArea models
            rows.RemoveAt(0);//Start at 1 to avoid reading Header
            foreach (object[] row in rows)
            {
                //List to prevent more than one querie per upper table
                List<string> importedUpperTables = new List<string>();

                A area = (A)Activator.CreateInstance(typeof(A), user);
                for (int col = 0; col < columns.Count; col++)
                {
                    object value = row[col];
                    if (value == null)
                        continue;

                    string fieldBaseArea = columns[col].BaseArea;

                    //Check if foreign Key
                    if (fieldBaseArea != area.Alias)
                    {
                        if (importedUpperTables.Contains(fieldBaseArea)) // upper table searched already
                            continue;

                        //JGF 2022.03.24 Get the foreign key name and not the primary key of the other area
                        var relation = area.ParentTables[fieldBaseArea];
                        string alias = area.Alias + '.' + relation.SourceRelField;

                        importedUpperTables.Add(fieldBaseArea);
                        List<QColumn> searchColumns = columnsByArea[fieldBaseArea];

                        //Get values from this row that come from upper table
                        List<object> upperValues = this.GetValuesFromRow(row, columns, fieldBaseArea);

                        //Get value from Above table with WHERE clause with all fields
                        AreaInfo parentTable = business.Area.GetInfoArea(fieldBaseArea);
                        value = this.ImportFromParent(upperValues, parentTable, searchColumns);

                        area.insertNameValueField(alias, value);
                    }
                    else
                    {
                        area.insertNameValueField(columns[col].Name, value);
                    }


                }
                results.Add(area);
            }

            return results;
        }

        private Dictionary<String, List<Exports.QColumn>> GetColumnsByArea(List<Exports.QColumn> columns)
        {
            Dictionary<String, List<Exports.QColumn>> areas = new Dictionary<String, List<Exports.QColumn>>();
            foreach (Exports.QColumn column in columns)
            {
                if (areas.ContainsKey(column.BaseArea))
                {
                    List<Exports.QColumn> columnsByArea = areas[column.BaseArea];
                    columnsByArea.Add(column);
                } else
                {
                    List<Exports.QColumn> columnsByArea = new List<Exports.QColumn> { column };
                    areas.Add(column.BaseArea, columnsByArea);
                }
            }

            return areas;
        }

        private bool CheckIfRightFile(object[] values, List<Exports.QColumn> columns)
        {
			//Check file header for columns descriptions
            for (int col = 0; col < columns.Count; col++)
            {
                object value = values[col];
                if (value == null || value.ToString() != columns[col].Description)
                {
                    return false;
                }
            }

            return true;
        }

        private List<object> GetValuesFromRow(object[] row, List<QColumn> searchColumns, string fieldBaseArea)
        {
            List<object> upperValues = new List<object>();
            for (int col = 0; col < searchColumns.Count; col++)
            {
                if (searchColumns[col].BaseArea == fieldBaseArea)
                {
                    upperValues.Add(row[col]);
                }
            }

            return upperValues;
        }

        private object ImportFromParent(List<object> upperValues, AreaInfo parentTable, List<QColumn> searchColumns)
        {
            //There should be a cache here to prevent repetitive queries
            object value = null;

            //Check if Find is necessary
            if (upperValues.Count > 0)
            {
				string parentName = parentTable.Alias;
                SelectQuery qs = new SelectQuery();

                qs.Select(parentName, parentTable.PrimaryKeyName);
                qs.From(parentTable.QSystem, parentTable.TableName, parentTable.Alias);
                CriteriaSet where = CriteriaSet.And();

                for (int col = 0; col < searchColumns.Count; col++)
                {
                    where.Equal(parentName, searchColumns[col].FieldName, upperValues[col]);
                }

                qs.Where(where);
                qs.PageSize(1);

                PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

                //get data
                sp.openConnection();
                DataMatrix res = sp.Execute(qs);
                sp.closeConnection();

				//If results, get Key
                if (res.NumRows > 0)
                {
                    value = res.GetDirect(0, 0);
                }
            }

            return value;
        }

		/// <summary>
		/// Convert the Excel datetime format string to a standard format string
		/// </summary>
		/// <param name="format">Excel-specific format string</param>
		/// <returns>Standard datetime format string</returns>
		private string GetNormalizedExcelDateTimeFormatString(string format)
		{
			if (string.IsNullOrEmpty(format))
				return "";

			// Remove non-standard format info (specific to Excel)
			// Replace with standard format info
			// Start string after any prefix characters and brackets (ex: [$f-800])
			return format.Substring(format.IndexOf(']') + 1)
				// Remove suffix characters
				.Replace(";@", "")
				// Remove extra backslashes
				.Replace("\\", "")
				// Change AM/PM to standard tt
				.Replace("AM/PM", "tt")
				// Excel uses lower case m for both months and minutes
				// Change the minute characters to i so the other
				// lowercase m characters that are for months can be changed to uppercase
				// and then the i characters can be changed back to m
				.Replace(":mm", ":ii")
				.Replace("mm:", "ii:")
				.Replace("m", "M")
				.Replace("i", "m")
				// Replace tenths of a second with standard f
				.Replace(".0", ".f")
				// Always replace / with - since Excel is inconsistent in using these characters
				// for format strings and formatted date text
				.Replace("/", "-");
		}

		/// <summary>
		/// Get the parsed Excel cell value using the cell's formatting
		/// or the default formatting if the cell does not have specific formatting
		/// </summary>
		/// <param name="cell">Cell object</param>
		/// <param name="defaultFormatting">Default format string, used for cells that don't have specific formatting</param>
		/// <returns>Datetime object</returns>
		private DateTime GetParsedExcelCellDateTimeValue(ExcelRange cell, string defaultFormatting)
		{
			if (cell.Value is double)
			{
				// Some date formatting options cause the cells to have the date stored as a number
				// which is the number of days since 1900-01-01 or 1904-01-01
				// depending on whether the 1904 date system option is set
				// --------------------------------------------------------------------------------
				// When using the 1900 date system, (default for Excel for Windows, Excel 2011 & 2016 for Mac),
				// this number is always at least 1 more than the actual number of days since 1900-01-01 and
				// in most cases this number will be 2 more than the actual number of days since 1900-01-01
				// because Excel allows the date 1900-02-29 which doesn't exist

				// Get starting year for numeric date values
				int startYear = cell.Worksheet.Workbook.Date1904 ? 1904 : 1900;

				// Get number of days since date system starting date (1900-01-01 or 1904-01-01)
				double daysSinceStart = (double)cell.Value;

				// If using 1900 date system (default)
				if (startYear == 1900)
				{
					// Account for 1900-01-01 being counted as 1
					daysSinceStart--;

					// If the date is 1900-2-29, it's invalid
					if (daysSinceStart == 59)
						throw new System.FormatException();
					// For dates after 1900-02-29 subtract 1
					// so this invalid date is not counted in the number of days since 1900-01-01
					else if (daysSinceStart > 59)
						daysSinceStart--;
				}

				// Return the date that is the elapsed time since the system starting date
				// (1900-01-01 or 1904-01-01)
				return new DateTime(startYear, 1, 1).AddDays(daysSinceStart);
			}
			else if (cell.Value is string)
			{
				string formatting;

				// If the cell has specific formatting, use it, otherwise use the default formatting.
				// There isn't really a way to just check if the formatting given is a datetime type format
				// and the cells don't have data types either.
				if (cell.Style.Numberformat.NumFmtID > 0)
					formatting = GetNormalizedExcelDateTimeFormatString(cell.Style.Numberformat.Format);
				else
					formatting = defaultFormatting;

				DateTimeFieldFormatter formatter = new DateTimeFieldFormatter(formatting);

				// Always replace / with - since Excel is inconsistent in using these characters
				// for format strings and formatted date text
				string cellValue = cell.Value == null ? "" : cell.Value.ToString().Replace("/", "-");

				// Try to parse using the cell's formatted text and format string
				return formatter.parseStringValue(cellValue);
			}

			return DateTime.MinValue;
		}

		/// <summary>
		/// Get the parsed Excel cell value, accounting for the field type
		/// </summary>
		/// <param name="column">Table column object</param>
		/// <param name="cell">Cell object</param>
		/// <returns>Cell value, parsed if necessary</returns>
		private object GetParsedExcelCellValue(QColumn column, ExcelRange cell)
		{
			switch (column.Formatting)
			{
				case FieldFormatting.DATA:
					return GetParsedExcelCellDateTimeValue(cell, Configuration.DateFormat.Date);
				case FieldFormatting.DATAHORA:
					return GetParsedExcelCellDateTimeValue(cell, Configuration.DateFormat.DateTime);
				case FieldFormatting.DATASEGUNDO:
					return GetParsedExcelCellDateTimeValue(cell, Configuration.DateFormat.DateTimeSeconds);
				default:
					return cell.Value;
			}
		}

		/// <summary>
		/// Import Excel sheet data
		/// </summary>
		/// <param name="columns">Table column objects</param>
		/// <param name="file">Raw file data</param>
		/// <param name="rowCount">Output number of rows</param>
		/// <returns>Rows with object values for each cell</returns>
        private List<object[]> ImportExcel(List<Exports.QColumn> columns, byte[] file, ref int rowCount)
        {
            int columnCount = columns.Count;
            List<object[]> results =  new List<object[]>();

            MemoryStream memStream = new MemoryStream(file);
            using (var package = new ExcelPackage(memStream))
            {
                int firstIndex = package.Compatibility.IsWorksheets1Based ? 1 : 0;
                var currentSheet = package.Workbook.Worksheets[firstIndex];

                rowCount = currentSheet.Dimension.End.Row;// Here is where my issue is

                for (int rowIterator = 0; rowIterator < rowCount; rowIterator++)
                {
                    object[] row = new object[columnCount];
                    for(int colIterator = 0; colIterator < columnCount; colIterator++){
                        // Current cell object
						var cell = currentSheet.Cells[rowIterator + 1, colIterator + 1];

						// Corresponding table column object
						QColumn column = columns[colIterator];

						// For the first row (header row) use the raw value (column title).
						// For data rows, get the value and parse if necessary,
						// accounting for the formatting defined in the Excel cell
						// and the application configuration.
						row[colIterator] = (rowIterator == 0) ? cell.Value : GetParsedExcelCellValue(column, cell);
                    }
                    results.Add(row);
                }
            }

            return results;
        }

        #endregion
    }
}
