using CSGenio.business;
using CSGenio.persistence;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using static CSGenio.framework.QueryInfo;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

namespace CSGenio.framework
{

    public class WritePosition
    {
        public WritePosition(int r_TitlesStart, int r_TitlesSize, int r_Header, int r_HeaderSize, int c_HeaderSize, int r_Data, int r_Data_End, int c_Start, int r_Aggregate)
        {
            R_TitlesStart = r_TitlesStart;
            R_TitlesSize = r_TitlesSize;
            R_Header = r_Header;
            R_HeaderSize = r_HeaderSize;
            C_HeaderSize = c_HeaderSize;
            R_Data = r_Data;
            R_Data_End = r_Data_End;
            C_Start = c_Start;
            R_Aggregate = r_Aggregate;
        }

        public int R_TitlesStart { get; set; }
        public int R_TitlesSize { get; set; }
        public int R_Header { get; set; }
        public int R_HeaderSize { get; set; }
        public int C_HeaderSize { get; set; }
        public int R_Data { get; set; }
        public int R_Data_End { get; set; }
        public int C_Start { get; set; }
        public int R_Aggregate { get; set; }
    }
    public class QueryToExcel
    {
        public void Convert(string path)
        {
            FileInfo newFile = new FileInfo(path);
            Excel = new ExcelPackage(newFile);

            try
            {
                CreateStyles(Excel);

                foreach (var item in WorksheetInfo)
                {
                    int startingPosition = R_TITLE_START;

                    ExcelWorksheet ws = Excel.Workbook.Worksheets.Add(item.WorkSheetName);
                    SetWorksheetFooterHeader(ws, item.HeaderFooterProperties);
                    Dictionary<string, int> chartTracker = new Dictionary<string, int>();
                    int lastChartRow = 0;

                    foreach (var prop in item.QueryProperties.Select((query, index) => new { query, index }))
                    {
                        var query = prop.query;
                        int index = prop.index;

                        if (!query.Data.GetColumns().Any())
                            continue;

                        DataMatrix data = query.Data.GetDataMatrix(Sp);
                        // Posição onde vai ser desenhada cada parte do excel
                        WritePosition writePositions = BuildWritePosition(startingPosition, C_START, query, data.NumRows);

                        /*
							The variable lastChartRow is used to automatically place the charts so that they don't overlap.
							Context: For each QueryInfo, which contains a table, titles, columns, charts, etc the DrawGrid function is called once,
							with this in mind, we needed to have a way to track the position of the charts executed in previous DrawGrid calls,
							and this is where the lastChartRow variable comes in handy. It is initialized outside DrawGrid, sent into DrawGrid to get the
							chart limits (to be used later), and then we use 'ref' so that the original value is changed to be used and updated in the
							following DrawGrid calls and chart placements.
						*/
						//desenho do template
                        query.Template.DrawGrid(ws, query, data, $"table{ws.Name.Replace(" ", string.Empty)}{index}", writePositions, ref lastChartRow, chartTracker);

                        // Starting position is recalculaed with the previous data position, returned data and query spacing
                        startingPosition = query.Template.GetLastRow() + SPACE_QUERIES;
                    }                    
                }
                Excel.Save();                
            }
            catch (Exception e)
            {
                throw new FrameworkException("Não foi possível concluir a exportação com sucesso!", "QueryToExcel.Convert", e.Message);
            }
        }
        public void AddWorksheet(WorksheetQueries worksheet)
        {
            WorksheetInfo.Add(worksheet);
        }
        public byte[] ExcelAsByteArray()
        {
            if(Excel == null)            
                return null;
            else
                return Excel.GetAsByteArray();            
        }
        public static WritePosition BuildWritePosition(int startingRow, int startingColumn, QueryInfo queryInfo, int rowCount = 0)
        {
            int titleSize = queryInfo.QueryTitles.Count;
            int headerColumns = queryInfo.Data.GetColumns().Count();
            int headerRows = queryInfo.GetHighestColumnPlacementDepth();
            int header_pos = startingRow;
            if (titleSize > 0)
                header_pos += titleSize + SPACE_TITLES_QUERIES;

            int data_pos = header_pos + headerRows;
            int dataEnd_pos = data_pos;
            if (rowCount > 0)
                dataEnd_pos += rowCount - 1;
            int aggregate_pos = dataEnd_pos + 1;

            return new WritePosition(startingRow, titleSize, header_pos, headerRows, headerColumns, data_pos, dataEnd_pos, startingColumn, aggregate_pos);
        }

        #region Styling
        private static void ConvertStyleFormat(StyleFormat style, ExcelStyle excel)
        {
            if (style != null && excel != null)
            {
                if (style.Font != null)
                {
                    if (!string.IsNullOrEmpty(style.Font.Name))
                        excel.Font.Name = style.Font.Name;
                    excel.Font.Bold = style.Font.Bold;
                    excel.Font.Italic = style.Font.Italic;
                    if (style.Font.Size > 0)
                        excel.Font.Size = style.Font.Size;
                    if (style.Font.Color != Color.Empty)
                        excel.Font.Color.SetColor(style.Font.Color);
                }
                if (style.Border != null)
                {
                    excel.Border.Top.Style = (ExcelBorderStyle)style.Border.Top;
                    excel.Border.Bottom.Style = (ExcelBorderStyle)style.Border.Bottom;
                    excel.Border.Left.Style = (ExcelBorderStyle)style.Border.Left;
                    excel.Border.Right.Style = (ExcelBorderStyle)style.Border.Right;
                }
                excel.WrapText = style.WrapText;
                excel.HorizontalAlignment = (ExcelHorizontalAlignment)style.HorizontalAlign;
                excel.VerticalAlignment = (ExcelVerticalAlignment)style.VerticalAlign;

                if (style.BackgroundColor != Color.Empty)
                {
                    excel.Fill.PatternType = ExcelFillStyle.Solid;
                    excel.Fill.BackgroundColor.SetColor(style.BackgroundColor);
                }
            }
        }

        private void CreateStyles(ExcelPackage excel)
        {
            foreach (var style in Styles)
            {
                var excelStyle = excel.Workbook.Styles.CreateNamedStyle(style.Key);
                ConvertStyleFormat(style.Value, excelStyle.Style);
            }
        }

        private void SetWorksheetFooterHeader(ExcelWorksheet ws, QueryHeaderFooter hf)
        {
            if(hf != null)
            {
                ws.HeaderFooter.FirstHeader.LeftAlignedText = hf.firstPageHeader.Left;
                ws.HeaderFooter.FirstHeader.CenteredText = hf.firstPageHeader.Center;
                ws.HeaderFooter.FirstHeader.RightAlignedText = hf.firstPageHeader.Right;

                ws.HeaderFooter.OddHeader.LeftAlignedText = hf.oddPagesHeader.Left;
                ws.HeaderFooter.OddHeader.CenteredText = hf.oddPagesHeader.Center;
                ws.HeaderFooter.OddHeader.RightAlignedText = hf.oddPagesHeader.Right;

                ws.HeaderFooter.EvenHeader.LeftAlignedText = hf.evenPagesHeader.Left;
                ws.HeaderFooter.EvenHeader.CenteredText = hf.evenPagesHeader.Center;
                ws.HeaderFooter.EvenHeader.RightAlignedText = hf.evenPagesHeader.Right;

                ws.HeaderFooter.FirstFooter.LeftAlignedText = hf.firstPageFooter.Left;
                ws.HeaderFooter.FirstFooter.CenteredText = hf.firstPageFooter.Center;
                ws.HeaderFooter.FirstFooter.RightAlignedText = hf.firstPageFooter.Right;

                ws.HeaderFooter.OddFooter.LeftAlignedText = hf.oddPagesFooter.Left;
                ws.HeaderFooter.OddFooter.CenteredText = hf.oddPagesFooter.Center;
                ws.HeaderFooter.OddFooter.RightAlignedText = hf.oddPagesFooter.Right;

                ws.HeaderFooter.EvenFooter.LeftAlignedText = hf.evenPagesFooter.Left;
                ws.HeaderFooter.EvenFooter.CenteredText = hf.evenPagesFooter.Center;
                ws.HeaderFooter.EvenFooter.RightAlignedText = hf.evenPagesFooter.Right;
            }
        }
        #endregion

        #region Format and values
        public static ColumnFormat GetColumnFormat(Type t)
        {
            if (t == typeof(DateTime))
                return ColumnFormat.DateTimeField;
            if (t == typeof(short) || t == typeof(int) || t == typeof(long) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
                return ColumnFormat.IntField;
            if (t == typeof(float) || t == typeof(decimal) || t == typeof(double))
                return ColumnFormat.NumericField;
            if (t == typeof(string))
                return ColumnFormat.TextField;

            return ColumnFormat.GeneralField;
        }

        public static ColumnFormat GetColumnFormat(Field f)
        {
            if (f.FieldType == FieldType.CURRENCY)
                return QueryInfo.ColumnFormat.CurrencyField;

            switch (f.FieldFormat)
            {
                case FieldFormatting.INTEIRO:
                case FieldFormatting.LOGICO:
                    return ColumnFormat.IntField;
                case FieldFormatting.FLOAT:
                    return new QueryInfo.NumericColumnFormat(f.Decimals);
                case FieldFormatting.DATA:
                    return ColumnFormat.DateField;
                case FieldFormatting.DATAHORA:
                    return ColumnFormat.DateTimeField;
                case FieldFormatting.DATASEGUNDO:
                    return ColumnFormat.DateTimeSecondsField;
            }

            return null; //assume a formatação de acordo com o tipo de coluna
        }

        public static ColumnFormat GetColumnFormat(FieldRef f)
        {
            return GetColumnFormat(Area.GetFieldInfo(f));
        }

        public static object GetValue(Type t, DataMatrix data, int row, int col)
        {
            //no treatment for datetime because of minvalue. We let the row return null in this case
            //only fields that have a default value need a specific treatment

            if (t == typeof(short) || t == typeof(int)  || t == typeof(ushort) || t == typeof(uint))
				return data.GetInteger(row, col);
			if (t == typeof(float) || t == typeof(decimal) || t == typeof(double) || t == typeof(long) || t == typeof(ulong))
				return data.GetNumeric(row, col);

            return data.GetDirect(row, col);
        }
        #endregion

        #region Constructor
        public QueryToExcel(PersistentSupport suportePersistente, User user, Dictionary<string, StyleFormat> styles = null)
        {
            Sp = suportePersistente;
            User = user;
            WorksheetInfo = new List<WorksheetQueries>();            
            Styles = styles ?? new Dictionary<string, StyleFormat>();
        }
        #endregion

        #region Classes
        public class WorksheetQueries
        {
            public WorksheetQueries(string workSheetName, ICollection<QueryInfo> queryProperties, QueryHeaderFooter headerFooter = null)
            {
                WorkSheetName = workSheetName;
                QueryProperties = queryProperties;
                HeaderFooterProperties = headerFooter;
            }
            public WorksheetQueries(string workSheetName, QueryInfo queryProperties, QueryHeaderFooter headerFooter = null)
            {
                WorkSheetName = workSheetName;
                QueryProperties = new List<QueryInfo>(new QueryInfo[] { queryProperties });
                HeaderFooterProperties = headerFooter;
            }

            public string WorkSheetName { get; set; }

            public ICollection<QueryInfo> QueryProperties { get; set; }
            public QueryHeaderFooter HeaderFooterProperties { get; set; }
        }

        #endregion

        #region Constants

        // Default text and data positions
        // These are passed into WritePositions class and ajusted accordingly

        // Index of Title
        public const int R_TITLE_START = 1;
        // Index of first column
        public const int C_START = 1;
        // Spacing between queries in the same worksheet
        public const int SPACE_QUERIES = 2;
        // Spacing between titles and queries
        public const int SPACE_TITLES_QUERIES = 1;
        #endregion

        #region Properties
        public ICollection<WorksheetQueries> WorksheetInfo { get; private set; }
        private ExcelPackage Excel;
        private PersistentSupport Sp;
        private User User;
        private Dictionary<string, StyleFormat> Styles;
        #endregion
    }

    #region Styles
    public class StyleFormat
    {
        public class FontStyle
        {
            public string Name { get; set; }
            public float Size { get; set; }
            public bool Bold { get; set; }
            public bool Italic { get; set; }
            public Color Color { get; set; }

            public FontStyle()
            {
                Color = Color.Empty;
                Name = "";
                Size = 0;
                Bold = false;
                Italic = false;
            }
        }
        public class BorderStyle
        {
            public BorderStyle(BorderType around)
            {
                Top = around;
                Bottom = around;
                Left = around;
                Right = around;
            }
            public BorderStyle()
            {
                Top = BorderType.None;
                Bottom = BorderType.None;
                Left = BorderType.None;
                Right = BorderType.None;
            }
            public BorderType Top { get; set; }
            public BorderType Bottom { get; set; }
            public BorderType Left { get; set; }
            public BorderType Right { get; set; }
        }
        public enum BorderType
        {
            None = 0,
            Thin = 4,
            Thick = 10,
            Medium = 11,
            Double = 12
        }
        public enum VerticalAlignment
        {
            Top = 0,
            Center = 1,
            Bottom = 2
        }
        public enum HorizontalAlignment
        {
            General = 0,
            Left = 1,
            Center = 2,
            Right = 4,
            Justify = 7
        }
        public FontStyle Font { get; set; }
        public BorderStyle Border { get; set; }
        public bool WrapText { get; set; }
        public HorizontalAlignment HorizontalAlign { get; set; }
        public VerticalAlignment VerticalAlign { get; set; }
        public Color BackgroundColor { get; set; }

        public StyleFormat()
        {
            WrapText = false;
            HorizontalAlign = HorizontalAlignment.General;
            VerticalAlign = VerticalAlignment.Center;
            BackgroundColor = Color.Empty;
        }
    }
    #endregion

    #region Text options

    public class QueryItem
    {
        public QueryItem(string text, string style = "")
        {
            Item = text;
            Style = style;
        }

        public QueryItem(Image img, string style = "")
        {
            Item = img;
            Style = style;
        }

        public readonly object Item;
        public string Style { get; set; }
    }

    #endregion

    #region Query definition
    public class QueryInfo
    {
        #region Classes
        public enum MergeType
        {
            Never,
            OnEqual,
            OnEqualAndNoChangesLeft
        }
        public enum PlacementMerge
        {
            Never,
            OnEqual,
            OnVerticalEqual
        }

        public enum QTableStyles
        {
            None = 0,
            Custom = 1,
            Light1 = 2,
            Light2 = 3,
            Light3 = 4,
            Light4 = 5,
            Light5 = 6,
            Light6 = 7,
            Light7 = 8,
            Light8 = 9,
            Light9 = 10,
            Light10 = 11,
            Light11 = 12,
            Light12 = 13,
            Light13 = 14,
            Light14 = 15,
            Light15 = 16,
            Light16 = 17,
            Light17 = 18,
            Light18 = 19,
            Light19 = 20,
            Light20 = 21,
            Light21 = 22,
            Medium1 = 23,
            Medium2 = 24,
            Medium3 = 25,
            Medium4 = 26,
            Medium5 = 27,
            Medium6 = 28,
            Medium7 = 29,
            Medium8 = 30,
            Medium9 = 31,
            Medium10 = 32,
            Medium11 = 33,
            Medium12 = 34,
            Medium13 = 35,
            Medium14 = 36,
            Medium15 = 37,
            Medium16 = 38,
            Medium17 = 39,
            Medium18 = 40,
            Medium19 = 41,
            Medium20 = 42,
            Medium21 = 43,
            Medium22 = 44,
            Medium23 = 45,
            Medium24 = 46,
            Medium25 = 47,
            Medium26 = 48,
            Medium27 = 49,
            Medium28 = 50,
            Dark1 = 51,
            Dark2 = 52,
            Dark3 = 53,
            Dark4 = 54,
            Dark5 = 55,
            Dark6 = 56,
            Dark7 = 57,
            Dark8 = 58,
            Dark9 = 59,
            Dark10 = 60,
            Dark11 = 61
        }
        #region ColumnFormats
        public abstract class ColumnFormat
        {
            #region BaseFormats
            public static readonly ColumnFormat IntField = new NumericColumnFormat(0);
            public static readonly ColumnFormat NumericField = new NumericColumnFormat(2);
            public static readonly ColumnFormat CurrencyField = new CurrencyColumnFormat("€", 2);
            public static readonly ColumnFormat TextField = new CustomColumnFormat("@");
            public static readonly ColumnFormat DateField = new CustomColumnFormat(Configuration.DateFormat.Date);
            public static readonly ColumnFormat DateTimeField = new CustomColumnFormat(Configuration.DateFormat.DateTime);
            public static readonly ColumnFormat DateTimeSecondsField = new CustomColumnFormat(Configuration.DateFormat.DateTimeSeconds);
            public static readonly ColumnFormat HourMinuteField = new CustomColumnFormat("[h]:mm");
            public static readonly ColumnFormat TimeField = new CustomColumnFormat(Configuration.DateFormat.Time);
            public static readonly ColumnFormat GeneralField = new CustomColumnFormat("");
            #endregion
            public abstract string Format { get; }
        }

        public class CustomColumnFormat : ColumnFormat
        {
            private readonly string format;
            public CustomColumnFormat(string format)
            {
                this.format = format;
            }
            public override string Format => format;
        }

        public class NumericColumnFormat : ColumnFormat
        {
            private readonly int decimalPlaces;

            public NumericColumnFormat(int decimalPlaces)
            {
                this.decimalPlaces = decimalPlaces;
            }

            public override string Format => "0" + (decimalPlaces > 0 ? $".{new String('0', decimalPlaces)}" : "");
        }
        public class CurrencyColumnFormat : NumericColumnFormat
        {
            private readonly string symbol;

            public CurrencyColumnFormat(string symbol, int decimalPlaces) : base(decimalPlaces)
            {
                this.symbol = symbol;
            }

            public override string Format => symbol + base.Format;
        }
        #endregion
        public class ColumnPlacement
        {
            public string Name { get; set; }
            public ColumnPlacement Placement { get; set; }
            public int Height { get; set; }
            public PlacementMerge Merge { get; set; }

            public ColumnPlacement(string name, PlacementMerge merge = PlacementMerge.OnEqual, int height = 1)
            {
                Name = name;
                Placement = null;
                Height = height;
                Merge = merge;
            }

            public ColumnPlacement(string name, ColumnPlacement placement, PlacementMerge merge = PlacementMerge.OnEqual, int height = 1)
            {
                Name = name;
                Placement = placement;
                Height = height;
                Merge = merge;
            }
            public ColumnPlacement(string name, string placement, PlacementMerge merge = PlacementMerge.OnEqual, int height = 1)
            {
                Name = name;
                Placement = new ColumnPlacement(placement);
                Height = height;
                Merge = merge;
            }

            public static int GetDepth(ColumnPlacement placement)
            {
                if (placement.Placement == null)
                    return placement.Height;

                return placement.Height + GetDepth(placement.Placement);
            }

            public override string ToString()
            {
                string builder = Name;
                ColumnPlacement place = Placement;
                while (place != null)
                {
                    builder = string.Format("{0}<-{1}", place.Name, builder);
                    place = place.Placement;
                }

                return builder;
            }

            public static implicit operator string(ColumnPlacement t)
            {
                return t.ToString();
            }
        }
        public class ColumnInfo
        {
            public ColumnInfo(string fieldName, ColumnFormat fieldFormat = null, MergeType merge = MergeType.Never, AggregateColumn aggregate = null)
            {
                Placement = new ColumnPlacement(fieldName);
                FieldName = fieldName;
                FieldFormat = fieldFormat;
                Merge = merge;
                Aggregate = aggregate;
            }
            public ColumnInfo(string fieldName, ColumnPlacement placement, ColumnFormat fieldFormat = null, MergeType merge = MergeType.Never, AggregateColumn aggregate = null)
            {
                FieldName = fieldName;
                FieldFormat = fieldFormat;
                Merge = merge;
                Placement = placement;
                Aggregate = aggregate;
            }


            public string FieldName { get; set; }
            public ColumnFormat FieldFormat { get; set; }
            public MergeType Merge { get; set; }
            public ColumnPlacement Placement { get; set; }
            public AggregateColumn Aggregate { get; set; }

        }
        public interface IQueryData
        {
            IEnumerable<string> GetColumns();
            DataMatrix GetDataMatrix(PersistentSupport sp);
        }
        public class QuerySelect : IQueryData
        {
            private SelectQuery query;
            private DataMatrix mat;

            public QuerySelect(SelectQuery query)
            {
                this.query = query;
                this.mat = null;
            }

            public IEnumerable<string> GetColumns()
            {
                return query.SelectFields.Select(x => x.Alias);
            }

            public DataMatrix GetDataMatrix(PersistentSupport sp)
            {
                if (mat == null)
                    mat = sp.Execute(query);

                return mat;
            }
        }
        public class QueryChart
        {
            public QueryChart(string title, eChartType chartType, ChartSerie serie, eChartStyle excelStyle = eChartStyle.None, ChartStyle customStyle = null)
            {
                this.ChartTitle = title;
                this.ChartType = chartType;
                this.ChartSeries = new List<ChartSerie>();
                this.ChartSeries.Add(serie);
                this.ChartSize = 100;
                this.ChartLocation = null;
                this.ExcelStyle = excelStyle;
                this.CustomStyle = customStyle;
            }
        
            public QueryChart(string title, eChartType chartType, List<ChartSerie> series, eChartStyle excelStyle = eChartStyle.None, ChartStyle customStyle = null)
            {
                this.ChartTitle = title;
                this.ChartType = chartType;
                this.ChartSeries = series;
                this.ChartSize = 100;
                this.ChartLocation = null;
                this.ExcelStyle = excelStyle;
                this.CustomStyle = customStyle;
            }
        
            public string ChartTitle { get; set; }
            public eChartType ChartType { get; set; }
            public eChartStyle ExcelStyle { get; set; }
            public ChartStyle CustomStyle { get; set; }
            /// <summary>
            /// Location of the chart on the excelPage (using cells)
            /// </summary>
            public ChartPosition ChartLocation { get; set; }
            /// <summary>
            /// Can be used as an object of type 'int' for size percentage or type 'Size' for pixel width/height
            /// </summary>
            public object ChartSize { get; set; }
            public List<ChartSerie> ChartSeries { get; set; }
        
            public class ChartPosition
            {
                public int xAxis { get; set; }
                public int yAxis { get; set; }
                public int xOffset { get; set; }
                public int yOffset { get; set; }
        
                public ChartPosition(int _x, int _y)
                {
                    this.xAxis = _x;
                    this.yAxis = _y;
                    this.xOffset = 0;
                    this.yOffset = 0;
                }
        
                public ChartPosition(int _x, int _y, int _xOff, int _yOff)
                {
                    this.xAxis = _x;
                    this.yAxis = _y;
                    this.xOffset = _xOff;
                    this.yOffset = _yOff;
                }
            }

            public class ChartSerie
            {
                public ChartSerie() { }

                /// <summary>
                /// Reads the vertical data of a given column, and plots a new serie
                /// </summary>
                /// <param name="title">Name of the serie</param>
                /// <param name="_xAxisColumn">Column name with data for the X Axis of the chart</param>
                /// <param name="_yAxisColumn">Column name with data for the Y Axis of the chart</param>
                /// <param name="chartGroup">Grouping name (charts containing same name defined here will combine in on chart)</param>
                /// <param name="_serieStyle">Style of the Serie</param>
                public ChartSerie(string title, string _xAxisColumn, string _yAxisColumn, string chartGroup = null, SerieStyle _serieStyle = null)
                {
                    this.SerieTitle = title;
                    this.ChartGroup = chartGroup;
                    this.xAxis = _xAxisColumn;
                    this.yAxis = _yAxisColumn;
                    this.IsHorizontalColumn = false;
                    this.SerieStyle = _serieStyle ?? new SerieStyle();
                }

                /// <summary>
                /// Reads the horizontal data of a given column, and plots a new serie
                /// </summary>
                /// <param name="horizontalSerieTitle">Horizontal column name</param>
                /// <param name="_yAxisColumn">Column name that contains the data to be plotted in the chart</param>
                /// <param name="chartGroup">Grouping name (charts containing same name defined here will combine in on chart)</param>
                /// <param name="_serieStyle">Style of the Serie</param>
                public ChartSerie(string horizontalSerieTitle, string _yAxisColumn, string chartGroup = null, SerieStyle _serieStyle = null)
                {
                    this.SerieTitle = horizontalSerieTitle;
                    this.ChartGroup = chartGroup;
                    this.xAxis = "Column_Headers";
                    this.yAxis = _yAxisColumn;
                    this.IsHorizontalColumn = true;
                    this.SerieStyle = _serieStyle ?? new SerieStyle();
                }

                //Use Chart Series to allow multiple table linking
                public string SerieTitle { get; set; }
                public string ChartGroup { get; set; }
                public string xAxis { get; set; }
                public string yAxis { get; set; }
                public bool IsHorizontalColumn { get; set; }
                public SerieStyle SerieStyle { get; set; }
            }

            public class ChartStyle
            {
                public bool RoundedCorners { get; set; }
                public bool IsPrintable { get; set; }
                public Color? BorderColor { get; set; }
                public int BorderTransparency { get; set; }
                public eLineStyle BorderStyle { get; set; }
                public int BorderWidth { get; set; }
                public Color? BackgroundColor { get; set; }
                public int ChartTransparency { get; set; }
                public eFillStyle ColorStyle { get; set; }
                public eTickLabelPosition LabelPosition { get; set; }

                public ChartStyle()
                {
                    this.RoundedCorners = true;
                    this.IsPrintable = true;
                    this.BorderColor = null;
                    this.BorderTransparency = 0;
                    this.BorderStyle = eLineStyle.Solid;
                    this.BorderWidth = 1;
                    this.BackgroundColor = null;
                    this.ChartTransparency = 0;
                    this.ColorStyle = eFillStyle.SolidFill;
                    this.LabelPosition = eTickLabelPosition.Low;
                }

                public ChartStyle(Color? _backgroundColor = null, int _transparency = 0, eFillStyle _colorStyle = eFillStyle.SolidFill, Color? _borderColor = null, int _borderTransparency = 0,
                    eLineStyle _borderStyle = eLineStyle.Solid, int _borderWidth = 1, bool _roundCorners = true, bool _isPrintable = true, eTickLabelPosition _labelPos = eTickLabelPosition.Low)
                {
                    this.BackgroundColor = _backgroundColor;
                    this.ChartTransparency = _transparency;
                    this.ColorStyle = _colorStyle;
                    this.BorderColor = _borderColor;
                    this.BorderTransparency = _borderTransparency;
                    this.BorderStyle = _borderStyle;
                    this.BorderWidth = _borderWidth;
                    this.RoundedCorners = _roundCorners;
                    this.IsPrintable = _isPrintable;
                    this.LabelPosition = _labelPos;
                }
            }
        
            public class SerieStyle
            {
                public eCrossBetween CrossBetween { get; set; }
                public eCrosses Crosses { get; set; }
                public eLabelPosition LabelPosition { get; set; }
                
                #region Border Style
                public int BorderWidth { get; set; }
                public eLineCap BorderLineCap { get; set; }
                public eLineStyle BorderLineStyle { get; set; }
                public Color? BorderFillColor { get; set; }
                public eFillStyle BorderFillStyle { get; set; }
                public int BorderFillTransparency { get; set; }
                #endregion
        
                #region Serie Fill Style
                public Color? FillColor { get; set; }
                public eFillStyle FillStyle { get; set; }
                public int FillTransparency { get; set; }
                #endregion

                public bool UseDefaults { get; set; }
        
                public SerieStyle()
                {
                    this.UseDefaults = true;
                    this.BorderFillColor = null;
                    this.BorderFillStyle = eFillStyle.SolidFill;
                    this.BorderFillTransparency = 0;
                    this.BorderLineCap = eLineCap.Round;
                    this.BorderLineStyle = eLineStyle.Solid;
                    this.BorderWidth = 2;
                    this.CrossBetween = eCrossBetween.Between;
                    this.Crosses = eCrosses.AutoZero;
                    this.FillColor = null;
                    this.FillStyle = eFillStyle.SolidFill;
                    this.FillTransparency = 0;
                    this.LabelPosition = eLabelPosition.BestFit;
                }

                public SerieStyle(Color? _serieColor = null, eFillStyle _serieStyle = eFillStyle.SolidFill, int _serieTransparency = 0, Color? _borderColor = null,
                    eFillStyle _borderFillStyle = eFillStyle.SolidFill, eLineStyle _borderLineStyle = eLineStyle.Solid, eLineCap _borderLineCap = eLineCap.Round,
                    int _borderTransparency = 0, int _borderWidth = 2, eCrosses _crosses = eCrosses.AutoZero, eCrossBetween _crossB = eCrossBetween.Between, eLabelPosition _labelPosition = eLabelPosition.BestFit)
                {
                    this.UseDefaults = false;
                    this.FillColor = _serieColor;
                    this.FillStyle = _serieStyle;
                    this.FillTransparency = _serieTransparency;
                    this.BorderFillColor = _borderColor;
                    this.BorderFillStyle = _borderFillStyle;
                    this.BorderLineStyle = _borderLineStyle;
                    this.BorderLineCap = _borderLineCap;
                    this.BorderFillTransparency = _borderTransparency;
                    this.BorderWidth = _borderWidth;
                    this.Crosses = _crosses;
                    this.CrossBetween = _crossB;
                    this.LabelPosition = _labelPosition;
                }
            }

            public class LineSerieStyle : SerieStyle
            {
                public Color? LineColor { get; set; }
                public int LineWidth { get; set; }
                public bool SmoothLines { get; set; }
                public eMarkerStyle MarkerStyle { get; set; }
                public int MarkerSize { get; set; }
                public Color? MarkerColor { get; set; }

                /// <summary>
                /// Uses the defaults while allowing Excel to manage serie colors automatically
                /// </summary>
                public LineSerieStyle() : base()
                {
                    this.SmoothLines = false;
                    this.LineColor = null;
                    this.MarkerStyle = eMarkerStyle.Circle;
                    this.MarkerSize = 5;
                    this.MarkerColor = null;
                    this.LineWidth = 2;
                }

                /// <summary>
                /// Use a unique color for line and marker while allowing more optional changes to be made.
                /// </summary>
                /// <param name="_serieColor">Color to be used for the line, marker and marker line</param>
                /// <param name="_smoothLines">Show curved (smooth) lines</param>
                /// <param name="_lineWidth">Width of the line displayed in the chart</param>
                /// <param name="_markerStyle">Style of the marker (data point in the plot area)</param>
                /// <param name="_markerSize">Size of the marker</param>
                /// <param name="_serieStyle">Style of the plotted line</param>
                /// <param name="_serieTransparency">NOT IMPLEMENTED</param>
                /// <param name="_borderColor">Border color of the ploted data</param>
                /// <param name="_borderFillStyle">Style of the color of the plotted data (NOT WORKING)</param>
                /// <param name="_borderLineStyle">Style of border lines in the plotted data</param>
                /// <param name="_borderLineCap">Additional styling of the border lines</param>
                /// <param name="_borderTransparency">NOT WORKING</param>
                /// <param name="_borderWidth">Thickness of the border</param>
                /// <param name="_crosses">Where the axis cross</param>
                /// <param name="_crossB">Where both the axis meet</param>
                /// <param name="_labelPosition">Position of the labels on the chart</param>
                public LineSerieStyle(Color _serieColor, bool _smoothLines = false, int _lineWidth = 2, eMarkerStyle _markerStyle = eMarkerStyle.Circle, int _markerSize = 5, eFillStyle _serieStyle = eFillStyle.SolidFill,
                    int _serieTransparency = 0, Color? _borderColor = null, eFillStyle _borderFillStyle = eFillStyle.SolidFill, eLineStyle _borderLineStyle = eLineStyle.Solid, eLineCap _borderLineCap = eLineCap.Round,
                    int _borderTransparency = 0, int _borderWidth = 1, eCrosses _crosses = eCrosses.AutoZero, eCrossBetween _crossB = eCrossBetween.Between, eLabelPosition _labelPosition = eLabelPosition.BestFit) : base(_serieColor, _serieStyle, _serieTransparency,
                        _borderColor, _borderFillStyle, _borderLineStyle, _borderLineCap, _borderTransparency, _borderWidth, _crosses, _crossB, _labelPosition)
                {
                    this.SmoothLines = _smoothLines;
                    this.LineColor = _serieColor;
                    this.MarkerStyle = _markerStyle;
                    this.MarkerSize = _markerSize;
                    this.MarkerColor = _serieColor;
                    this.LineWidth = _lineWidth;
                }

                /// <summary>
                /// Use different colors for line and marker while allowing more optional changes to be made.
                /// </summary>
                /// <param name="_lineColor">Color of the plotted line</param>
                /// <param name="_markerColor">Color of the plotted marker</param>
                /// <param name="_smoothLines">Show curved (smooth) lines</param>
                /// <param name="_lineWidth">Width of the line displayed in the chart</param>
                /// <param name="_markerStyle">Style of the marker (data point in the plot area)</param>
                /// <param name="_markerSize">Size of the marker</param>
                /// <param name="_serieStyle">Style of the plotted line</param>
                /// <param name="_serieTransparency">NOT IMPLEMENTED</param>
                /// <param name="_borderColor">Border color of the ploted data</param>
                /// <param name="_borderFillStyle">Style of the color of the plotted data (NOT WORKING)</param>
                /// <param name="_borderLineStyle">Style of border lines in the plotted data</param>
                /// <param name="_borderLineCap">Additional styling of the border lines</param>
                /// <param name="_borderTransparency">NOT WORKING</param>
                /// <param name="_borderWidth">Thickness of the border</param>
                /// <param name="_crosses">Where the axis cross</param>
                /// <param name="_crossB">Where both the axis meet</param>
                /// <param name="_labelPosition">Position of the labels on the chart</param>
                public LineSerieStyle(Color _lineColor, Color _markerColor, bool _smoothLines = false, int _lineWidth = 2, eMarkerStyle _markerStyle = eMarkerStyle.Circle, int _markerSize = 5, eFillStyle _serieStyle = eFillStyle.SolidFill,
                    int _serieTransparency = 0, Color? _borderColor = null, eFillStyle _borderFillStyle = eFillStyle.SolidFill, eLineStyle _borderLineStyle = eLineStyle.Solid, eLineCap _borderLineCap = eLineCap.Round,
                    int _borderTransparency = 0, int _borderWidth = 1, eCrosses _crosses = eCrosses.AutoZero, eCrossBetween _crossB = eCrossBetween.Between, eLabelPosition _labelPosition = eLabelPosition.BestFit) : base(null, _serieStyle, _serieTransparency,
                        _borderColor, _borderFillStyle, _borderLineStyle, _borderLineCap, _borderTransparency, _borderWidth, _crosses, _crossB, _labelPosition)
                {
                    this.SmoothLines = _smoothLines;
                    this.LineColor = _lineColor;
                    this.MarkerStyle = _markerStyle;
                    this.MarkerSize = _markerSize;
                    this.MarkerColor = _markerColor;
                    this.LineWidth = _lineWidth;
                }
            }

            public class PieSerieStyle : SerieStyle
            {
                public int PieExplosion { get; set; }
                public bool RandomColors { get; set; }

                /// <summary>
                /// Creates a Radar/Pie Chart with default Excel settings
                /// </summary>
                public PieSerieStyle() : base()
                {
                    this.PieExplosion = 0;
                    this.RandomColors = true;
                }

                /// <summary>
                /// Creates a Radar/Pie chart with the given Color, while allowing additional settings to be changed
                /// </summary>
                /// <param name="_serieColor">Main color of the serie</param>
                /// <param name="_pieExplosion">Separation between slices (pie data)</param>
                /// <param name="_randomColors">Let excel use random colors for the serie</param>
                /// <param name="_serieStyle">Style of the plotted line</param>
                /// <param name="_serieTransparency">NOT IMPLEMENTED</param>
                /// <param name="_borderColor">Border color of the ploted data</param>
                /// <param name="_borderFillStyle">Style of the color of the plotted data (NOT WORKING)</param>
                /// <param name="_borderLineStyle">Style of border lines in the plotted data</param>
                /// <param name="_borderLineCap">Additional styling of the border lines</param>
                /// <param name="_borderTransparency">NOT WORKING</param>
                /// <param name="_borderWidth">Thickness of the border</param>
                /// <param name="_crosses">Where the axis cross</param>
                /// <param name="_crossB">Where both the axis meet</param>
                /// <param name="_labelPosition">Position of the labels on the chart</param>
                public PieSerieStyle(Color? _serieColor, int _pieExplosion = 0, bool _randomColors = true, eFillStyle _serieStyle = eFillStyle.SolidFill,
                    int _serieTransparency = 0, Color? _borderColor = null, eFillStyle _borderFillStyle = eFillStyle.SolidFill, eLineStyle _borderLineStyle = eLineStyle.Solid, eLineCap _borderLineCap = eLineCap.Round,
                    int _borderTransparency = 0, int _borderWidth = 1, eCrosses _crosses = eCrosses.AutoZero, eCrossBetween _crossB = eCrossBetween.Between, eLabelPosition _labelPosition = eLabelPosition.BestFit) : base(_serieColor, _serieStyle, _serieTransparency,
                        _borderColor, _borderFillStyle, _borderLineStyle, _borderLineCap, _borderTransparency, _borderWidth, _crosses, _crossB, _labelPosition)
                {
                    this.PieExplosion = _pieExplosion;
                    this.RandomColors = _randomColors;
                }
            }

            public class ScatterSerieStyle : SerieStyle
            {
                public Color? LineColor { get; set; }
                public int LineWidth { get; set; }
                public eMarkerStyle MarkerStyle { get; set; }
                public int MarkerSize { get; set; }
                public Color? MarkerColor { get; set; }
                public Color? MarkerLineColor { get; set; }

                /// <summary>
                /// Creates a Radar/Pie Chart with default Excel settings
                /// </summary>
                public ScatterSerieStyle() : base()
                {
                    this.LineColor = null;
                    this.LineWidth = 2;
                    this.MarkerStyle = eMarkerStyle.None;
                    this.MarkerSize = 4;
                    this.MarkerColor = null;
                    this.MarkerLineColor = null;
                }

                /// <summary>
                /// Use a unique color for line, marker and marker line while allowing more optional changes to be made.
                /// </summary>
                /// <param name="_serieColor">Color to be used for the line, marker and marker line</param>
                /// <param name="_markerStyle">Style of the marker (data point in the plot area)</param>
                /// <param name="_markerSize">Size of the marker</param>
                /// <param name="_lineWidth">Width of the line displayed in the chart</param>
                /// <param name="_smoothLines">Show curved (smooth) lines</param>
                /// <param name="_serieStyle">Style of the plotted line</param>
                /// <param name="_serieTransparency">NOT IMPLEMENTED</param>
                /// <param name="_borderColor">Border color of the ploted data</param>
                /// <param name="_borderFillStyle">Style of the color of the plotted data (NOT WORKING)</param>
                /// <param name="_borderLineStyle">Style of border lines in the plotted data</param>
                /// <param name="_borderLineCap">Additional styling of the border lines</param>
                /// <param name="_borderTransparency">NOT WORKING</param>
                /// <param name="_borderWidth">Thickness of the border</param>
                /// <param name="_crosses">Where the axis cross</param>
                /// <param name="_crossB">Where both the axis meet</param>
                /// <param name="_labelPosition">Position of the labels on the chart</param>
                public ScatterSerieStyle(Color _serieColor, eMarkerStyle _markerStyle = eMarkerStyle.None, int _markerSize = 4, int _lineWidth = 0, eFillStyle _serieStyle = eFillStyle.SolidFill,
                    int _serieTransparency = 0, Color? _borderColor = null, eFillStyle _borderFillStyle = eFillStyle.SolidFill, eLineStyle _borderLineStyle = eLineStyle.Solid, eLineCap _borderLineCap = eLineCap.Round,
                    int _borderTransparency = 0, int _borderWidth = 1, eCrosses _crosses = eCrosses.AutoZero, eCrossBetween _crossB = eCrossBetween.Between, eLabelPosition _labelPosition = eLabelPosition.BestFit) : base(_serieColor, _serieStyle, _serieTransparency,
                        _borderColor, _borderFillStyle, _borderLineStyle, _borderLineCap, _borderTransparency, _borderWidth, _crosses, _crossB, _labelPosition)
                {
                    this.LineColor = _serieColor;
                    this.MarkerStyle = _markerStyle;
                    this.MarkerSize = _markerSize;
                    this.MarkerColor = _serieColor;
                    this.MarkerLineColor = _serieColor;
                    this.LineWidth = _lineWidth;
                }

                /// <summary>
                /// Use different colors for line, marker and marker line while allowing more optional changes to be made.
                /// </summary>
                /// <param name="_lineColor">Color of the plotted line</param>
                /// <param name="_markerColor">Color of the plotted marker</param>
                /// <param name="_smoothLines">Show curved (smooth) lines</param>
                /// <param name="_lineWidth">Width of the line displayed in the chart</param>
                /// <param name="_markerStyle">Style of the marker (data point in the plot area)</param>
                /// <param name="_markerSize">Size of the marker</param>
                /// <param name="_serieStyle">Style of the plotted line</param>
                /// <param name="_serieTransparency">NOT IMPLEMENTED</param>
                /// <param name="_borderColor">Border color of the ploted data</param>
                /// <param name="_borderFillStyle">Style of the color of the plotted data (NOT WORKING)</param>
                /// <param name="_borderLineStyle">Style of border lines in the plotted data</param>
                /// <param name="_borderLineCap">Additional styling of the border lines</param>
                /// <param name="_borderTransparency">NOT WORKING</param>
                /// <param name="_borderWidth">Thickness of the border</param>
                /// <param name="_crosses">Where the axis cross</param>
                /// <param name="_crossB">Where both the axis meet</param>
                /// <param name="_labelPosition">Position of the labels on the chart</param>
                public ScatterSerieStyle(Color _lineColor, Color _markerColor, Color _markerLineColor, int _lineWidth = 0, eMarkerStyle _markerStyle = eMarkerStyle.None, int _markerSize = 4, eFillStyle _serieStyle = eFillStyle.SolidFill,
                    int _serieTransparency = 0, Color? _borderColor = null, eFillStyle _borderFillStyle = eFillStyle.SolidFill, eLineStyle _borderLineStyle = eLineStyle.Solid, eLineCap _borderLineCap = eLineCap.Round,
                    int _borderTransparency = 0, int _borderWidth = 1, eCrosses _crosses = eCrosses.AutoZero, eCrossBetween _crossB = eCrossBetween.Between, eLabelPosition _labelPosition = eLabelPosition.BestFit) : base(null, _serieStyle, _serieTransparency,
                        _borderColor, _borderFillStyle, _borderLineStyle, _borderLineCap, _borderTransparency, _borderWidth, _crosses, _crossB, _labelPosition)
                {
                    this.LineColor = _lineColor;
                    this.MarkerStyle = _markerStyle;
                    this.MarkerSize = _markerSize;
                    this.MarkerColor = _markerColor;
                    this.MarkerLineColor = _markerLineColor;
                    this.LineWidth = _lineWidth;
                }
            }
        }
        public class QueryDataset : IQueryData
        {
            private DataSet dataSet;

            public QueryDataset(DataSet dataSet)
            {
                this.dataSet = dataSet;
            }

            public IEnumerable<string> GetColumns()
            {
                return dataSet.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName);
            }

            public DataMatrix GetDataMatrix(PersistentSupport sp)
            {
                return new DataMatrix(dataSet);
            }
        }
        public class GridStyle
        {
            public GridStyle()
            {
                Header = "";
                FirstRowStripe = "";
                SecondRowStripe = "";
                Aggregate = "";
            }

            public string Header { get; set; }
            public string FirstRowStripe { get; set; }
            public string SecondRowStripe { get; set; }
            public string Aggregate { get; set; }

        }
        private class ColumnHeader
        {
            public ColumnHeader(string name, QueryInfo.PlacementMerge placement)
            {
                Name = name;
                Merge = placement;
            }

            public string Name { get; set; }
            public QueryInfo.PlacementMerge Merge { get; set; }
        }
        public abstract class ExcelGridTemplate
        {
            protected readonly GridStyle styles;
            protected ExcelWorksheet worksheet;
            protected QueryInfo info;
            protected WritePosition positions;
            protected DataMatrix data;
            protected string gridName;
            protected int lastChartRow;
            protected Dictionary<string, int> chartTracker;

            public bool CreateFilters { get; set; }
            public bool BlockColumnsFormat { get; set; }
            public ExcelGridTemplate(GridStyle styles, bool createFilters, bool blockColumnsFormat)
            {
                this.styles = styles;
                this.CreateFilters = createFilters;
                this.BlockColumnsFormat = blockColumnsFormat;
            }
            protected virtual void CreateTitles()
            {
                int i = 0;
                foreach (var tit in info.QueryTitles)
                {
                    int MergedCols = positions.C_HeaderSize - 1;
                    ExcelRange range = worksheet.Cells[positions.R_TitlesStart + i, positions.C_Start, positions.R_TitlesStart + i, positions.C_Start + MergedCols];

                    //only merge if it is more then one cell
                    //merged cells are excluded from autofit
                    if (MergedCols > 0)
                        range.Merge = true;

                    if (!string.IsNullOrEmpty(tit.Style))
                        range.StyleName = tit.Style;

                    if (tit.Item != null)
                    {
                        if (tit.Item is string text)
                        {
                            range.Value = text;
                        }
                        else if (tit.Item is Image image)
                        {
                            //a dll tem uma incoerência
                            //sempre que se altera a altura das linhas o size das imagens, que já existirem na sheet, também alteram.
                            //este comportamento acontece mesmo que as imagens sejam definidas como absolutas

                            var cell = range.Start;
                            var row = worksheet.Row(cell.Row);
                            int topOffset = 10;
                            int leftOffset = 10;
                            var pic = worksheet.Drawings.AddPicture($"{gridName}Img{i}", image);
                            //ajustar a linha ao size da imagem tendo em conta o offset definido
                            row.Height = GetHeight(image.Height + (topOffset * 2));
                            pic.SetSize(image.Width, image.Height);
                            pic.SetPosition(cell.Row - 1, topOffset, cell.Column - 1, leftOffset);
                        }
                    }
                    i++;
                }
            }

            private static double GetHeight(int pix)
            {
                return pix * 72 / 96;
            }
            protected virtual void CreateHeader()
            {
                int[,] rowMerge;
                int[,] colMerge;
                ColumnHeader[,] columnHierarchy;

                BuildRowAndColMerge(out columnHierarchy, out rowMerge, out colMerge);

                for (int row = 0; row < positions.R_HeaderSize; row++)
                {
                    for (int col = 0; col < positions.C_HeaderSize; col++)
                    {
                        // Rows or columns that are influenced by a merge and are not the first one are ignored
                        if (rowMerge[row, col] == -1 || colMerge[row, col] == -1)
                            continue;

                        int rowSum = rowMerge[row, col];
                        int colSum = colMerge[row, col];

                        ExcelRange range = worksheet.Cells[positions.R_Header + row, positions.C_Start + col, positions.R_Header + row + rowSum, positions.C_Start + col + colSum];

                        if (!string.IsNullOrEmpty(styles?.Header))
                            range.StyleName = styles.Header;

                        range.Value = columnHierarchy[row, col].Name;

                        //only merge if it is more then one cell
                        //merged cells are excluded from autofit
                        if (rowSum > 0 || colSum > 0)
                            range.Merge = true;
                    }
                }
            }
            protected virtual void WriteData()
            {
                List<DataColumn> columns = new List<DataColumn>(data.DbDataSet.Tables[0].Columns.Cast<DataColumn>());

                //apply cells data style before format to avoid losing format
                //ensure that at least one row have the table style format
                for (int rowIndex = 1; rowIndex == 1 || rowIndex <= data.NumRows; rowIndex++)
                {
                    string rowStyle = rowIndex % 2 != 0 ? styles?.FirstRowStripe : styles?.SecondRowStripe;
                    if (!string.IsNullOrEmpty(rowStyle))
                    {
                        int excelRow = positions.R_Data + rowIndex - 1;
                        worksheet.Cells[excelRow, positions.C_Start, excelRow, positions.C_Start + positions.C_HeaderSize - 1].StyleName = rowStyle;
                    }
                }

                //apply data format before set values (performance issues)
                for (int i = 0; i < columns.Count; i++)
                {
                    DataColumn column = columns[i];
                    ColumnInfo columnInfo = info.QueryFields.FirstOrDefault(x => string.Compare(x.FieldName, column.ColumnName, true) == 0);

                    // Format data with correct type
                    FormatRowTypes(positions.C_Start + i, column, columnInfo);
                }

                //write data in cells
                if (data.NumRows > 0)
                {
                    List<object[]> rows = new List<object[]>();
                    for (int r = 0; r < data.NumRows; r++)
                    {
                        List<object> row = new List<object>();
                        for (int c = 0; c < data.NumCols; c++)
                        {
                            DataColumn column = columns[c];
                            object value = QueryToExcel.GetValue(column.DataType, data, r, c);
                            if (value == DBNull.Value)
                                value = null;

                            row.Add(value);
                        }
                        rows.Add(row.ToArray());
                    }
                    var dataRange = worksheet.Cells[positions.R_Data, positions.C_Start].LoadFromArrays(rows);
                }

            }

            protected virtual int CreateCharts()
            {
                const int tblOffset = 2;
                int chartStartPos = this.positions.R_Header - tblOffset > this.lastChartRow + 1 ? (this.positions.R_Header - tblOffset) : (this.lastChartRow + 1); //Lowest row used
                int chartEndPos = this.positions.C_HeaderSize; //Last Column
            
                eChartType[] ignoreCharts = {
                    eChartType.StockHLC,    //Not supported
                    eChartType.StockOHLC,   //Not supported
                    eChartType.StockVHLC,   //Not supported
                    eChartType.StockVOHLC,  //Not supported
                    eChartType.Column3D     //Not Implemented
                };
            
                foreach (QueryChart newChart in this.info.QueryCharts)
                {
                    //If the selected chart is not yet implemented, throw an exception
                    if (ignoreCharts.Contains(newChart.ChartType))
                        continue;
                        //throw new NotImplementedException($"{Enum.GetName(typeof(eChartType), newChart.ChartType)} chart is currently not supported. Please change the chart type!");
            
                    foreach (QueryChart.ChartSerie serie in newChart.ChartSeries)
                    {
                        bool isNewChart = true; //If its *NOT* part of a group, create a new chart
                        int trackedChart = -1;  //Idx of the chart, to allow grouping later
            
                        if (serie.ChartGroup != null)
                        {
                            //If a chart from the group already exists, then create a new serie for the group
                            if (this.chartTracker.TryGetValue(serie.ChartGroup, out trackedChart))
                            {
                                if (trackedChart != -1)
                                    isNewChart = false;
                            }
                            else
                                this.chartTracker.Add(serie.ChartGroup, this.worksheet.Drawings.Count()); //since it is going to be created we alread add the non-zero index
                        }
            
                        ExcelChart chart = null;
                        if (isNewChart)
                        {
                            int chartNum = this.worksheet.Drawings.OfType<ExcelChart>().Count() + 1;
                            chart = this.worksheet.Drawings.AddChart($"Chart{chartNum}", newChart.ChartType);
            
                            chart.Title.Text = newChart.ChartTitle;
            
                            //If no position is overriten then the chart will be on the side of the first table it is linked to
                            if (newChart.ChartLocation == null)
                                chart.SetPosition(chartStartPos, 0, chartEndPos + tblOffset, 0);
                            else
                                chart.SetPosition(newChart.ChartLocation.yAxis, newChart.ChartLocation.yOffset, newChart.ChartLocation.xAxis, newChart.ChartLocation.xOffset);
            
                            //If the defined ChartSize is not of type int not Size then the default of 100% will be chosen
                            if (newChart.ChartSize.GetType() == typeof(int))
                                chart.SetSize((int)newChart.ChartSize);
                            else
                                if (newChart.ChartSize.GetType() == typeof(Size))
                                chart.SetSize(((Size)newChart.ChartSize).Width, ((Size)newChart.ChartSize).Height);
                            else
                                chart.SetSize(100);
            
                            chart.DisplayBlanksAs = eDisplayBlanksAs.Zero;

                            if (newChart.CustomStyle != null)
                                chart = ApplyChartStyle(chart, newChart.CustomStyle);
                            else
                                chart.Style = newChart.ExcelStyle;
            
                            //Track the farthest and lowest position of the charts to allow correct positioning
                            chartEndPos = chart.To.Column;
                            if (chart.To.Row > this.lastChartRow)
                                this.lastChartRow = chart.To.Row;
                        }

                        ExcelRange xAxis = GetColumnAddress(serie.xAxis ?? "", true, serie.IsHorizontalColumn);
                        ExcelRange yAxis = GetColumnAddress(serie.yAxis ?? "", true, serie.IsHorizontalColumn);
                        //In case of bubble chart and others, allow the addition of another Axis (Bubble size maybe)

                        ExcelChartSerie chartSerie;
                        if (isNewChart)
                            chartSerie = chart.Series.Add(yAxis, xAxis);
                        else
                            chartSerie = ((ExcelChart)this.worksheet.Drawings[trackedChart]).Series.Add(yAxis, xAxis);
            
                        if(!serie.SerieStyle.UseDefaults)
                            chartSerie = ApplySerieStyles(chartSerie, serie.SerieStyle, newChart.ChartType);
            
                        //Get Series Address
                        //If the gathered name is not a column inside the current table (info) then use the name, otherwise get the address for the column
                        if (this.info.Data.GetColumns().Contains(serie.SerieTitle))
                            chartSerie.HeaderAddress = GetColumnAddress(serie.SerieTitle, isHorizontal: serie.IsHorizontalColumn);
                        else
                            chartSerie.HeaderAddress = GetColumnAddress(serie.yAxis, isHorizontal: serie.IsHorizontalColumn);
                    }
                }
                return this.lastChartRow; //Return the updated last chart position
            }
            
            #region ChartTypes
            //public bool IsType3D(eChartType chartType)
            //{
            //    if (chartType != eChartType.Area3D && chartType != eChartType.AreaStacked3D && chartType != eChartType.AreaStacked1003D && chartType != eChartType.BarClustered3D && chartType != eChartType.BarStacked3D && chartType != eChartType.BarStacked1003D && chartType != eChartType.Column3D && chartType != eChartType.ColumnClustered3D && chartType != eChartType.ColumnStacked3D && chartType != eChartType.ColumnStacked1003D && chartType != eChartType.Line3D && chartType != eChartType.Pie3D && chartType != eChartType.PieExploded3D && chartType != eChartType.ConeBarClustered && chartType != eChartType.ConeBarStacked && chartType != eChartType.ConeBarStacked100 && chartType != eChartType.ConeCol && chartType != eChartType.ConeColClustered && chartType != eChartType.ConeColStacked && chartType != eChartType.ConeColStacked100 && chartType != eChartType.CylinderBarClustered && chartType != eChartType.CylinderBarStacked && chartType != eChartType.CylinderBarStacked100 && chartType != eChartType.CylinderCol && chartType != eChartType.CylinderColClustered && chartType != eChartType.CylinderColStacked && chartType != eChartType.CylinderColStacked100 && chartType != eChartType.PyramidBarClustered && chartType != eChartType.PyramidBarStacked && chartType != eChartType.PyramidBarStacked100 && chartType != eChartType.PyramidCol && chartType != eChartType.PyramidColClustered && chartType != eChartType.PyramidColStacked && chartType != eChartType.PyramidColStacked100 && chartType != eChartType.Surface && chartType != eChartType.SurfaceTopView && chartType != eChartType.SurfaceTopViewWireframe)
            //        return chartType == eChartType.SurfaceWireframe;
            //    return true;
            //}
            
            public bool IsTypeLine(eChartType chartType)
            {
                if (chartType != eChartType.Line && chartType != eChartType.LineMarkers && chartType != eChartType.LineMarkersStacked100 && chartType != eChartType.LineStacked && chartType != eChartType.LineStacked100)
                    return chartType == eChartType.Line3D;
                return true;
            }
            
            public bool IsTypeScatterBubble(eChartType chartType)
            {
                if (chartType != eChartType.XYScatter && chartType != eChartType.XYScatterLines && chartType != eChartType.XYScatterLinesNoMarkers && chartType != eChartType.XYScatterSmooth && chartType != eChartType.XYScatterSmoothNoMarkers && chartType != eChartType.Bubble)
                    return chartType == eChartType.Bubble3DEffect;
                return true;
            }
            
            public bool IsTypeSurface(eChartType chartType)
            {
                if (chartType != eChartType.Surface && chartType != eChartType.SurfaceTopView && chartType != eChartType.SurfaceTopViewWireframe)
                    return chartType == eChartType.SurfaceWireframe;
                return true;
            }
            
            public bool IsTypeShape(eChartType chartType)
            {
                if (chartType != eChartType.BarClustered3D && chartType != eChartType.BarStacked3D && chartType != eChartType.BarStacked1003D && chartType != eChartType.BarClustered3D && chartType != eChartType.BarStacked3D && chartType != eChartType.BarStacked1003D && chartType != eChartType.Column3D && chartType != eChartType.ColumnClustered3D && chartType != eChartType.ColumnStacked3D && chartType != eChartType.ColumnStacked1003D && chartType != eChartType.ConeBarClustered && chartType != eChartType.ConeBarStacked &&
                    chartType != eChartType.ConeBarStacked100 && chartType != eChartType.ConeCol && chartType != eChartType.ConeColClustered && chartType != eChartType.ConeColStacked && chartType != eChartType.ConeColStacked100 && chartType != eChartType.CylinderBarClustered && chartType != eChartType.CylinderBarStacked && chartType != eChartType.CylinderBarStacked100 && chartType != eChartType.CylinderCol && chartType != eChartType.CylinderColClustered && chartType != eChartType.CylinderColStacked &&
                    chartType != eChartType.CylinderColStacked100 && chartType != eChartType.PyramidBarClustered && chartType != eChartType.PyramidBarStacked && chartType != eChartType.PyramidBarStacked100 && chartType != eChartType.PyramidCol && chartType != eChartType.PyramidColClustered && chartType != eChartType.PyramidColStacked)
                    return chartType == eChartType.PyramidColStacked100;
                return true;
            }
            
            public bool IsTypePercentStacked(eChartType chartType)
            {
                if (chartType != eChartType.AreaStacked100 && chartType != eChartType.BarStacked100 && chartType != eChartType.BarStacked1003D && chartType != eChartType.ColumnStacked100 && chartType != eChartType.ColumnStacked1003D && chartType != eChartType.ConeBarStacked100 && chartType != eChartType.ConeColStacked100 && chartType != eChartType.CylinderBarStacked100 && chartType != eChartType.CylinderColStacked && chartType != eChartType.LineMarkersStacked100 && chartType != eChartType.LineStacked100 && chartType != eChartType.PyramidBarStacked100)
                    return chartType == eChartType.PyramidColStacked100;
                return true;
            }
            
            public bool IsTypeStacked(eChartType chartType)
            {
                if (chartType != eChartType.AreaStacked && chartType != eChartType.AreaStacked3D && chartType != eChartType.BarStacked && chartType != eChartType.BarStacked3D && chartType != eChartType.ColumnStacked3D && chartType != eChartType.ColumnStacked && chartType != eChartType.ConeBarStacked && chartType != eChartType.ConeColStacked && chartType != eChartType.CylinderBarStacked && chartType != eChartType.CylinderColStacked && chartType != eChartType.LineMarkersStacked && chartType != eChartType.LineStacked && chartType != eChartType.PyramidBarStacked)
                    return chartType == eChartType.PyramidColStacked;
                return true;
            }
            
            public bool IsTypeClustered(eChartType chartType)
            {
                if (chartType != eChartType.BarClustered && chartType != eChartType.BarClustered3D && chartType != eChartType.ColumnClustered3D && chartType != eChartType.ColumnClustered && chartType != eChartType.ConeBarClustered && chartType != eChartType.ConeColClustered && chartType != eChartType.CylinderBarClustered && chartType != eChartType.CylinderColClustered && chartType != eChartType.PyramidBarClustered)
                    return chartType == eChartType.PyramidColClustered;
                return true;
            }
            
            public bool IsTypePieDoughnut(eChartType chartType)
            {
                if (chartType != eChartType.Pie && chartType != eChartType.PieExploded && chartType != eChartType.PieOfPie && chartType != eChartType.Pie3D && chartType != eChartType.PieExploded3D && chartType != eChartType.BarOfPie && chartType != eChartType.Doughnut)
                    return chartType == eChartType.DoughnutExploded;
                return true;
            }
            
            public bool IsTypeRadar(eChartType chartType)
            {
                if (chartType != eChartType.Radar && chartType != eChartType.RadarFilled)
                    return chartType == eChartType.RadarMarkers;
                return true;
            }
            #endregion

            public ExcelChart ApplyChartStyle(ExcelChart chart, QueryChart.ChartStyle customStyle)
            {
                chart.RoundedCorners = customStyle.RoundedCorners;
                //chart.Locked = customStyle.LockChart;
                chart.Print = customStyle.IsPrintable;

                if(customStyle.BorderColor != null) chart.Border.Fill.Color = (Color)customStyle.BorderColor;
                //chart.Border.Fill.Transparancy = customStyle.BorderTransparency; //Not implemented yet
                //chart.Border.Fill.Style = customStyle.BorderStyle; //Not implemented yet
                chart.Border.Width = customStyle.BorderWidth;
                chart.Border.LineStyle = customStyle.BorderStyle;
                
				if(chart.XAxis != null)
                    chart.XAxis.TickLabelPosition = customStyle.LabelPosition;

                if (customStyle.BackgroundColor != null)
                {
                    chart.Fill.Color = (Color)customStyle.BackgroundColor;
                    chart.PlotArea.Fill.Color = (Color)customStyle.BackgroundColor;
                }
                //chart.Fill.Transparancy = customStyle.ChartTransparency; //Not implemented yet
                //chart.Fill.Style = customStyle.ColorStyle; //Not implemented yet

                return chart;
            }
            
            public ExcelChartSerie ApplySerieStyles(ExcelChartSerie serie, QueryChart.SerieStyle style, eChartType chartType)
            {
                //ExcelChartSerieDataLabel dataLabel; //Not implemented
                PropertiesToBorder(serie, style, chartType);
                PropertiesToFill(serie, style, chartType);

                if (IsTypeLine(chartType))
                {
                    ExcelLineChartSerie lineSerie = (ExcelLineChartSerie)serie;
                    QueryChart.LineSerieStyle lineStyle = ((QueryChart.LineSerieStyle)style);
                    lineSerie.Smooth = lineStyle.SmoothLines;
                    lineSerie.Marker = lineStyle.MarkerStyle;
                    lineSerie.MarkerSize = lineStyle.MarkerSize;
                    if (lineStyle.MarkerColor != null) lineSerie.MarkerLineColor = (Color)lineStyle.MarkerColor;
                    if (lineStyle.LineColor != null) lineSerie.LineColor = (Color)lineStyle.LineColor;
                    lineSerie.LineWidth = lineStyle.LineWidth;
                }
            
                if(IsTypeScatterBubble(chartType))
                {
                    if(chartType != eChartType.Bubble && chartType != eChartType.Bubble3DEffect)
                    {
                        ExcelScatterChartSerie scatterSerie = (ExcelScatterChartSerie)serie;
                        QueryChart.ScatterSerieStyle scatterStyle = (QueryChart.ScatterSerieStyle)style;
                        scatterSerie.Marker = scatterStyle.MarkerStyle;
                        scatterSerie.MarkerSize = scatterStyle.MarkerSize;
                        if (scatterStyle.MarkerColor != null) scatterSerie.MarkerColor = (Color)scatterStyle.MarkerColor;
                        if (scatterStyle.MarkerLineColor != null) scatterSerie.MarkerLineColor = (Color)scatterStyle.MarkerLineColor;
                        if (scatterStyle.LineColor != null) scatterSerie.LineColor = (Color)scatterStyle.LineColor;
                        scatterSerie.LineWidth = scatterStyle.LineWidth;
                    }
                }
            
                if (IsTypePieDoughnut(chartType))
                {
                    ExcelPieChartSerie pieSerie = (ExcelPieChartSerie)serie;
                    QueryChart.PieSerieStyle radarStyle = (QueryChart.PieSerieStyle)style;
                    pieSerie.Explosion = radarStyle.PieExplosion;
            
                    if (radarStyle.RandomColors)
                        return pieSerie;
                }
            
                return serie;
            }
            
            private ExcelChartSerie PropertiesToBorder(ExcelChartSerie _serie, QueryChart.SerieStyle style, eChartType type)
            {
                _serie.Border.LineStyle = style.BorderLineStyle;
                _serie.Border.LineCap = style.BorderLineCap;
                _serie.Border.Width = style.BorderWidth;
                _serie = FillBorder(_serie, style, type);
                return _serie;
            }
            
            private ExcelChartSerie PropertiesToFill(ExcelChartSerie _serie, QueryChart.SerieStyle style, eChartType type)
            {
                if (style.FillColor != null && !IsTypeLine(type) && !IsTypeScatterBubble(type))
                    _serie.Fill.Color = (Color)style.FillColor;
                //_serie.Fill.Transparancy = int.MaxValue * (style.FillTransparency/100);
                _serie.Fill.Style = style.FillStyle;
                return _serie;
            }
            
            private ExcelChartSerie FillBorder(ExcelChartSerie _serie, QueryChart.SerieStyle style, eChartType type)
            {
                if (style.BorderFillColor != null && !IsTypeLine(type) && !IsTypeScatterBubble(type))
                    _serie.Border.Fill.Color = (Color)style.BorderFillColor;
                //_serie.Border.Fill.Transparancy = int.MaxValue * (style.BorderFillTransparency / 100);
                _serie.Border.Fill.Style = style.BorderFillStyle;
                return _serie;
            }
            
            //Based on the column name get the address for the column or the data associated with that column (((((((vertical or horizontal)))))))
            public ExcelRange GetColumnAddress(string columnName, bool columnData = false, bool isHorizontal = false)
            {
                if (!isHorizontal)
                {
                    for (int colIdx = this.positions.C_Start; colIdx <= this.positions.C_HeaderSize; colIdx++)
                        if (this.worksheet.Cells[this.positions.R_Header, colIdx].Text == columnName)
                        {
                            if (!columnData)
                                return this.worksheet.Cells[this.positions.R_Header, colIdx, this.positions.R_Header, colIdx];
                            else
                                return this.worksheet.Cells[this.positions.R_Data, colIdx, this.positions.R_Data_End, colIdx];
                        }
                }
                else
                {
                    if (columnData && columnName == "Column_Headers") //xAxis - Column Headers
                        return this.worksheet.Cells[this.positions.R_Header, this.positions.C_Start+1, this.positions.R_Header, this.positions.C_HeaderSize];
                    else
                    {
                        for (int rowIdx = this.positions.R_Data; rowIdx <= this.positions.R_Data_End; rowIdx++)
                        {
                            //yAxis needs to be processed, aswell as the data associated with the header
                            //ColumnName also needs to be processed for the Horizontal Series
                            if (this.worksheet.Cells[rowIdx, this.positions.C_Start].Text == columnName)
                            {
                                if(!columnData)
                                    return this.worksheet.Cells[rowIdx, this.positions.C_Start, rowIdx, this.positions.C_Start];
                                else
                                    return this.worksheet.Cells[rowIdx, this.positions.C_Start + 1, rowIdx, this.positions.C_HeaderSize];
                            }
                        }
                    }
                }

                return this.worksheet.Cells[1,1];
            }

            protected abstract void CreateTable();
            protected abstract void CreateAggregateColumn(int colIndex, AggregateColumn aggregate);
            protected abstract void FormatAggregateRow();
            private bool HasAggregateColumns()
            {
                bool HasAggregateColumns = false;
                for (int i = 0; i < positions.C_HeaderSize; i++)
                {
                    string columnName = data.DbDataSet.Tables[0].Columns[i].ColumnName;
                    ColumnInfo column = info.QueryFields.FirstOrDefault(x => string.Compare(x.FieldName, columnName, true) == 0);

                    if (column != null && column.Aggregate != null)
                    {
                        HasAggregateColumns = true;
                        break;
                    }
                }
                return HasAggregateColumns;
            }
            protected virtual void CreateAggregateColumns()
            {
                if (HasAggregateColumns())
                {
                    FormatAggregateRow();

                    for (int i = 0; i < positions.C_HeaderSize; i++)
                    {
                        string columnName = data.DbDataSet.Tables[0].Columns[i].ColumnName;
                        ColumnInfo column = info.QueryFields.FirstOrDefault(x => string.Compare(x.FieldName, columnName, true) == 0);

                        if (column != null && column.Aggregate != null)
                        {
                            CreateAggregateColumn(i, column.Aggregate);
                        }
                    }
                }
            }
            protected abstract void SetFilter();
            protected virtual void ApplyColumnFit()
            {
                worksheet.Cells.AutoFitColumns();
            }
            protected virtual void MergeRows()
            {
                int totalRowSize = data.NumRows + 1;
                List<DataColumn> columns = new List<DataColumn>(data.DbDataSet.Tables[0].Columns.Cast<DataColumn>());
                for (int i = 0; i < columns.Count; i++)
                {
                    DataColumn column = columns[i];
                    ColumnInfo columnInfo = info.QueryFields.FirstOrDefault(x => string.Compare(x.FieldName, column.ColumnName, true) == 0);

                    // Merge and center row based on equality of data
                    if (columnInfo != null && columnInfo.Merge != MergeType.Never)
                        MergeAndCenterRows(i, columnInfo.Merge);
                }
            }
            protected virtual void MergeAndCenterRows(int column, MergeType mergeType)
            {
                int mergeSize = 0;
                int start = -1;

                // We only merge and center when we have more than 1 row
                for (int j = 1; j < data.NumRows; j++)
                {
                    bool equal = false;

                    // Columns from left to right should be equal for merge
                    // Otherwise merge could be applied to different groupings
                    List<object> current = new List<object>();
                    List<object> previous = new List<object>();

                    int i = 0;
                    if (mergeType == MergeType.OnEqual)
                        i = column;
                    for (; i <= column; i++)
                    {
                        previous.Add(data.GetDirect(j - 1, i));
                        current.Add(data.GetDirect(j, i));
                    }

                    if (current.SequenceEqual(previous))
                    {
                        if (mergeSize == 0)
                            start = j - 1;
                        mergeSize++;
                        equal = true;
                    }

                    if (mergeSize > 0 && (!equal || j + 1 == data.NumRows))
                        worksheet.Cells[positions.R_Data + start, column + 1, positions.R_Data + start + mergeSize, column + 1].Merge = true;


                    if (!equal)
                        mergeSize = 0;
                }

                Action<int, int, int, int> CenterCellRange = (FromRow, FromCol, ToRow, ToCol) =>
                {
                    worksheet.Cells[FromRow, FromCol, ToRow, ToCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[FromRow, FromCol, ToRow, ToCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                };

                CenterCellRange(positions.R_Data, column + 1, positions.R_Data_End, column + 1);
            }

            public int GetLastRow()
            {
                if (HasAggregateColumns())
                    return positions.R_Aggregate;
                else
                    return positions.R_Data_End;
            }

            private void BuildRowAndColMerge(out ColumnHeader[,] columnHierarchy, out int[,] rowMerge, out int[,] colMerge)
            {
                // Creates matrix with the representation of the columns without any merge
                // Columns that should merge are repeated
                // The size of the matrix is R X C, where R is the tallest column and C is the number of columns
                columnHierarchy = BuildColumnHierarchyMatrix();

                // Representarion of the row merges that sould be done
                // Same size as the columnHierarchy 
                // The first of the rows that are to be merged contains the number of rows to be merged, whereas the following contain -1 representing that they are doing to be merged
                // Rows that are not influenced by a merged have value 0
                rowMerge = new int[positions.R_HeaderSize, positions.C_HeaderSize];

                // Same as rowMerge but for column
                colMerge = new int[positions.R_HeaderSize, positions.C_HeaderSize];

                int previous;
                int mergeSize = 0;
                int start = -1;

                // Fills the rowMerge matrix
                for (int col = 0; col < positions.C_HeaderSize; col++)
                {
                    previous = 0;
                    start = 0;
                    //only make sense to apply merge to rows if there is more than one row
                    if (positions.R_HeaderSize > 1)
                    {
                        rowMerge[0, col] = -1;
                        for (int row = 1; row < positions.R_HeaderSize; row++)
                        {
                            // Default value of a column is -1
                            // -1 represents that the column will be merged and shoul not be writen to a cell
                            rowMerge[row, col] = -1;
                            previous = row - 1;

                            // Only merge if previous row was equal
                            bool equal = string.Compare(columnHierarchy[previous, col].Name, columnHierarchy[row, col].Name, true) == 0
                                && (columnHierarchy[row, col].Merge == QueryInfo.PlacementMerge.OnVerticalEqual || columnHierarchy[row, col].Merge == QueryInfo.PlacementMerge.OnEqual);

                            if (equal)
                            {
                                if (mergeSize == 0)
                                    start = row - 1;

                                mergeSize++;
                            }

                            // Merge size is postponed until we find a different row or we are at the last row
                            if (!equal || row == positions.R_HeaderSize - 1)
                            {
                                if (!equal && row == positions.R_HeaderSize - 1)
                                    rowMerge[row, col] = 0;

                                rowMerge[start, col] = mergeSize;
                                start = row;
                                mergeSize = 0;
                            }
                        }
                    }
                }

                // Fills the colMerge matrix
                for (int row = 0; row < positions.R_HeaderSize; row++)
                {
                    previous = 0;
                    start = 0;

                    //only make sense to apply merge to columns if there is more than one column
                    if (positions.C_HeaderSize > 1)
                    {
                        colMerge[row, 0] = -1;
                        for (int col = 1; col < positions.C_HeaderSize; col++)
                        {
                            // Default value of a column is -1
                            // -1 represents that the column will be merged and shoul not be writen to a cell
                            colMerge[row, col] = -1;
                            previous = col - 1;

                            // Only merge if previous column was equal, previous row was equal or it was the first one, and merge type is set to OnEqual
                            bool equal = string.Compare(columnHierarchy[row, previous].Name, columnHierarchy[row, col].Name, true) == 0
                                && (row - 1 < 0 || string.Compare(columnHierarchy[row - 1, previous].Name, columnHierarchy[row - 1, col].Name, true) == 0)
                                && columnHierarchy[row, col].Merge == QueryInfo.PlacementMerge.OnEqual;


                            if (equal)
                            {
                                if (mergeSize == 0)
                                    start = col - 1;

                                mergeSize++;
                            }


                            // Merge size is postponed until we find a something different (!equal) row or we are at the last row
                            if (!equal || col == positions.C_HeaderSize - 1)
                            {
                                if (!equal && col == positions.C_HeaderSize - 1)
                                    colMerge[row, col] = 0;

                                colMerge[row, start] = mergeSize;
                                start = col;
                                mergeSize = 0;
                            }
                        }
                    }
                }
            }
            private ColumnHeader[,] BuildColumnHierarchyMatrix()
            {
                ColumnHeader[,] columnHierarchy = new ColumnHeader[positions.R_HeaderSize, positions.C_HeaderSize];
                List<string> columns = new List<string>(info.Data.GetColumns());
                for (int col = 0; col < columns.Count; col++)
                {
                    string fieldName = columns[col];
                    int row = positions.R_HeaderSize - 1;
                    ColumnInfo column = info.QueryFields.FirstOrDefault(x => string.Compare(x.FieldName, fieldName, true) == 0);
                    if (column != null)
                    {
                        ColumnPlacement placement = column.Placement;
                        while (placement != null)
                        {
                            for (int i = placement.Height - 1; i >= 0; i--)
                                columnHierarchy[row--, col] = new ColumnHeader(placement.Name, placement.Merge);

                            placement = placement.Placement;
                        }
                        int previousRow = row + 1;
                        for (; row >= 0; row--)
                            columnHierarchy[row, col] = columnHierarchy[previousRow, col];
                    }
                    else
                    {
                        for (; row >= 0; row--)
                            columnHierarchy[row, col] = new ColumnHeader(fieldName, QueryInfo.PlacementMerge.OnEqual);
                    }
                }

                return columnHierarchy;
            }
            protected void FormatRowTypes(int col, DataColumn column, ColumnInfo columnInfo)
            {
                ColumnFormat columnFormat = columnInfo?.FieldFormat ?? QueryToExcel.GetColumnFormat(column.DataType);
                //universo de dados a formatar
                //por omissão aplica-se apenas sobre a quantidade de linhas com dados
                //se for to bloquear a coluna toda então formatamos até ao final da folha
                if (BlockColumnsFormat == true)
                    worksheet.Column(col).Style.Numberformat.Format = columnFormat.Format;
                else
                {
                    worksheet.Cells[positions.R_Data, col, positions.R_Data_End, col].Style.Numberformat.Format = columnFormat.Format;
                }
            }
            public void DrawGrid(ExcelWorksheet worksheet, QueryInfo info, DataMatrix data, string gridName, WritePosition positions, ref int lastChartRow, Dictionary<string, int> chartTracker = null)
            {
                this.worksheet = worksheet;
                this.info = info;
                this.positions = positions;
                this.data = data;
                this.gridName = gridName;
                this.lastChartRow = lastChartRow;
                this.chartTracker = chartTracker;

                CreateTitles();
                CreateHeader();
                CreateTable();
                WriteData();
                CreateAggregateColumns();
                SetFilter();
                ApplyColumnFit();
                MergeRows();
                lastChartRow = CreateCharts();
            }
        }
        public class CustomGridTemplate : ExcelGridTemplate
        {
            public CustomGridTemplate(GridStyle styles = null, bool createFilters = false, bool blockColumnsFormat = false)
                : base(styles, createFilters, blockColumnsFormat)
            { }
            protected override void CreateTable()
            { }

            protected override void FormatAggregateRow()
            {
                if (!string.IsNullOrEmpty(styles?.Aggregate))
                    worksheet.Cells[positions.R_Aggregate, positions.C_Start, positions.R_Aggregate, positions.C_Start + positions.C_HeaderSize - 1]
                        .StyleName = styles.Aggregate;
            }
            protected override void CreateAggregateColumn(int colIndex, AggregateColumn aggregate)
            {
                if (colIndex >= 0)
                {
                    var aggregateCell = worksheet.Cells[positions.R_Aggregate, colIndex + 1];
                    string columns = $"R{positions.R_Data}C{colIndex + 1}:R{positions.R_Data_End}C{colIndex + 1}";
                    string func = aggregate.GetAggregateFunction(columns);

                    if (!string.IsNullOrEmpty(func))
                    {
                        aggregateCell.FormulaR1C1 = func;
                    }
                    else if (!string.IsNullOrEmpty(aggregate.Label))
                    {
                        aggregateCell.Value = aggregate.Label;
                    }

                    aggregateCell.Style.Numberformat.Format = aggregate.ColFormat.Format;
                }


            }
            protected override void SetFilter()
            {
                //there is a bug on autofilter that does not allow to be set as false
                if (CreateFilters)
                {
                    //set filter on the last header row
                    int row_HeaderEnd = positions.R_Header + info.GetHighestColumnPlacementDepth() - 1;
                    var header = worksheet.Cells[row_HeaderEnd, positions.C_Start, row_HeaderEnd, positions.C_Start + positions.C_HeaderSize - 1];
                    header.AutoFilter = true;
                }

            }


        }
        public class TableTemplate : ExcelGridTemplate
        {
            public TableTemplate(QTableStyles tableStyle, GridStyle customStyle = null, bool createFilters = false, bool blockColumnsFormat = false)
                : base(customStyle, createFilters, blockColumnsFormat)
            {
                TableStyle = tableStyle;
            }

            private readonly QTableStyles TableStyle;
            private ExcelTable table;

            protected override void CreateTable()
            {
                int row_HeaderEnd = positions.R_Header + info.GetHighestColumnPlacementDepth() - 1;
                ExcelRange tableRange = worksheet.Cells[
                    row_HeaderEnd,
                    positions.C_Start,
                    positions.R_Data_End, //ensure that at least one row have the table style format
                    positions.C_Start + positions.C_HeaderSize - 1];

                table = worksheet.Tables.Add(tableRange, gridName);
                table.TableStyle = (TableStyles)TableStyle;
                table.ShowHeader = true;
                table.ShowFilter = false;
            }

            protected override void FormatAggregateRow()
            {
                if (table != null)
                {
                    table.ShowTotal = true;
                    if (!string.IsNullOrEmpty(styles?.Aggregate))
                        table.TotalsRowCellStyle = styles.Aggregate;
                }
            }

            protected override void CreateAggregateColumn(int colIndex, AggregateColumn aggregate)
            {
                if (table != null)
                {
                    if (table.Columns.Count > colIndex && colIndex >= 0)
                    {
                        var column = table.Columns[colIndex];
                        string func = aggregate.GetAggregateFunction($"[{column.Name}]");

                        if (!string.IsNullOrEmpty(func))
                        {
                            column.TotalsRowFormula = func;
                        }
                        else if (!string.IsNullOrEmpty(aggregate.Label))
                        {
                            column.TotalsRowLabel = aggregate.Label;
                        }

                        var aggregateCell = worksheet.Cells[positions.R_Aggregate, colIndex + 1];
                        aggregateCell.Style.Numberformat.Format = aggregate.ColFormat.Format;
                    }
                }
            }

            protected override void SetFilter()
            {
                if (table != null)
                {
                    table.ShowFilter = CreateFilters;
                }
            }

            protected override void MergeAndCenterRows(int column, MergeType mergeType)
            {
                throw new InvalidOperationException("it's not allowed to merge cells in a table");
            }


        }
        #endregion

        #region Constructor
        public QueryInfo(SelectQuery query, ICollection<ColumnInfo> fields = null, List<QueryItem> titles = null, ExcelGridTemplate template = null, List<QueryChart> charts = null)
            : this(fields, titles, template, charts)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            Data = new QuerySelect(query);
        }

        public QueryInfo(DataSet data, ICollection<ColumnInfo> fields = null, List<QueryItem> titles = null, ExcelGridTemplate template = null, List<QueryChart> charts = null)
            : this(fields, titles, template, charts)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Data = new QueryDataset(data);
        }

        public QueryInfo(IQueryData data, ICollection<ColumnInfo> fields = null, List<QueryItem> titles = null, ExcelGridTemplate template = null, List<QueryChart> charts = null)
            : this(fields, titles, template, charts)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Data = data;
        }
        private QueryInfo(ICollection<ColumnInfo> fields, List<QueryItem> titles, ExcelGridTemplate template, List<QueryChart> charts = null)
        {
            QueryFields = fields ?? new List<ColumnInfo>();
            QueryTitles = titles ?? new List<QueryItem>();
            QueryCharts = charts ?? new List<QueryChart>();
            Template = template ?? new CustomGridTemplate();
        }

        #endregion

        #region Properties
        public readonly IQueryData Data;
        public readonly ICollection<ColumnInfo> QueryFields;
        public readonly ICollection<QueryItem> QueryTitles;
        public readonly List<QueryChart> QueryCharts;
        public readonly ExcelGridTemplate Template;
        #endregion

        public static ColumnPlacement BuildPlacement(params string[] placemet)
        {
            return BuildPlacement(placemet, PlacementMerge.Never);
        }

        public static ColumnPlacement BuildPlacement(string[] placemet, PlacementMerge merge = PlacementMerge.Never)
        {
            ColumnPlacement place = null;
            ColumnPlacement iterator = null;
            if (placemet.Length > 0)
            {
                place = new ColumnPlacement(placemet[placemet.Length - 1], merge);
                iterator = place;
                for (int i = placemet.Length - 2; i >= 0; i--)
                {
                    ColumnPlacement low = new ColumnPlacement(placemet[i], merge);
                    iterator.Placement = low;
                    iterator = low;
                }
            }

            return place;
        }
        
        public int GetHighestColumnPlacementDepth()
        {
            return QueryFields.Any() ? QueryFields.Max(x => ColumnPlacement.GetDepth(x.Placement)) : 1;
        }
    }
    #endregion

    public class HeaderFooterProps
    {
        public string Left { get; set; }
        public string Center { get; set; }
        public string Right { get; set; }

        public HeaderFooterProps()
        {
            Left = "";
            Center = "";
            Right = "";
        }

        public HeaderFooterProps(string left, string center, string right)
        {
            Left = left;
            Center = center;
            Right = right;
        }
    }

    public class QueryHeaderFooter
    {
        //FirstPage Header and Footer
        public HeaderFooterProps firstPageHeader { get; set; }
        public HeaderFooterProps firstPageFooter { get; set; }

        //OddPage Header and Footer
        public HeaderFooterProps oddPagesHeader { get; set; }
        public HeaderFooterProps oddPagesFooter { get; set; }

        //EvenPage Header and Footer
        public HeaderFooterProps evenPagesHeader { get; set; }
        public HeaderFooterProps evenPagesFooter { get; set; }

        public bool alignWithMargins;

        //Empty object, all strings empty
        public QueryHeaderFooter()
        {
            firstPageHeader = new HeaderFooterProps();
            firstPageFooter = new HeaderFooterProps();
            oddPagesHeader = new HeaderFooterProps();
            oddPagesFooter = new HeaderFooterProps();
            evenPagesHeader = new HeaderFooterProps();
            evenPagesFooter = new HeaderFooterProps();
            alignWithMargins = true;
        }

        //Set header and footer while having control over first, odd and even pages
        public QueryHeaderFooter(bool alignMargin, HeaderFooterProps firstPageH = null, HeaderFooterProps firstPageF = null, HeaderFooterProps oddPagesH = null,
            HeaderFooterProps oddPagesF = null, HeaderFooterProps evenPagesH = null, HeaderFooterProps evenPagesF = null)
        {
            alignWithMargins = alignMargin;
            firstPageHeader = firstPageH ?? new HeaderFooterProps();
            firstPageFooter = firstPageF ?? new HeaderFooterProps();
            oddPagesHeader = oddPagesH ?? new HeaderFooterProps();
            oddPagesFooter = oddPagesF ?? new HeaderFooterProps();
            evenPagesHeader = evenPagesH ?? new HeaderFooterProps();
            evenPagesFooter = evenPagesF ?? new HeaderFooterProps();
        }

        //Set an header and footer for all pages
        public QueryHeaderFooter(bool alignMargins, HeaderFooterProps header, HeaderFooterProps footer)
        {
            alignWithMargins = alignMargins;
            firstPageHeader = header;
            firstPageFooter = footer;
            oddPagesHeader = header;
            oddPagesFooter = footer;
            evenPagesHeader = header;
            evenPagesFooter = footer;
        }
    }

    public class AggregateColumn
    {
        public readonly AggregateFunction Function;
        public readonly ColumnFormat ColFormat;
        public readonly string Label;

        public AggregateColumn(AggregateFunction function, ColumnFormat format)
        {
            Function = function;
            ColFormat = format ?? throw new ArgumentNullException(nameof(format));
            Label = "";
        }

        public AggregateColumn(string label)
        {
            Function = AggregateFunction.None;
            ColFormat = QueryInfo.ColumnFormat.GeneralField;
            Label = label;
        }

                public string GetAggregateFunction(string columns)
        {
            if (string.IsNullOrEmpty(columns))
                return "";

            const string subtotalTemplate = "=SUBTOTAL({0}, {1})";

            return Function switch
            {
                //Formulas com subtotal
                AggregateFunction.Average => string.Format(subtotalTemplate, "101", columns),
                AggregateFunction.Count => string.Format(subtotalTemplate, "102", columns),
                AggregateFunction.CountA => string.Format(subtotalTemplate, "103", columns),
                AggregateFunction.Max => string.Format(subtotalTemplate, "104", columns),
                AggregateFunction.Min => string.Format(subtotalTemplate, "105", columns),
                AggregateFunction.Product => string.Format(subtotalTemplate, "106", columns),
                AggregateFunction.StdDev => string.Format(subtotalTemplate, "107", columns),
                AggregateFunction.StdDevP => string.Format(subtotalTemplate, "108", columns),
                AggregateFunction.Sum => string.Format(subtotalTemplate, "109", columns),
                AggregateFunction.Variance => string.Format(subtotalTemplate, "110", columns),
                AggregateFunction.VarianceP => string.Format(subtotalTemplate, "111", columns),

                //Formulas custom
                AggregateFunction.CountDistinct =>
                    $"=SUM(IF(FREQUENCY(MATCH({columns},{columns},0),MATCH({columns},{columns},0))>0,1))",

                _ => ""
            };
        }

    }
    
    public enum AggregateFunction
    {
        None,
        Average,
        Count,
        CountA,
        CountDistinct,
        Max,
        Min,
        Product,
        StdDev,
        StdDevP,
        Sum,
        Variance,
        VarianceP,
    }
}
