using CSGenio.business;
using CSGenio.framework;
using CSGenio.framework.Geography;
using System;
using System.Collections.Generic;

namespace CSGenio.core.messaging
{
    public class AreaDataset
    {
        public Dictionary<string, AreaDatasetTable> Tables { get; set; } = new Dictionary<string, AreaDatasetTable>();

        public AreaDatasetTable AddTable(string table)
        {
            if(Tables.ContainsKey(table)) 
                return Tables[table];

            var x = new AreaDatasetTable();
            Tables.Add(table, x);
            return x;
        }
    }

    public class AreaDatasetTable
    {
        public Dictionary<string, Area> Updated { get; set; } = new Dictionary<string, Area>();
        public List<string> Deleted { get; set; } = new List<string>();
    }


    /// <summary>
    /// Adds methods to convert between QueueMessage and AreaDataset
    /// </summary>
    public static class QueueMessageExtensions
    {
        public static void FromDataset(this QueueMessage message, AreaDataset data, PublisherMetadata pub)
        {
            foreach (var tab in data.Tables)
            {
                var queueTable = new QueueTable();
                queueTable.Id = tab.Key;

                var pubtab = pub.Tables.Find(x => x.Table == tab.Key);
                var areainfo = Area.GetInfoArea(pubtab.Table);
                foreach (var col in pubtab.Fields)
                {
                    var fieldinfo = areainfo.DBFields[col];
                    queueTable.Columns.Add(new QueueField
                    {
                        Id = col,
                        DataType = fieldinfo.FieldFormat.ToString(),
                    });
                }

                foreach (var upd in tab.Value.Updated)
                {
                    var row = new object[queueTable.Columns.Count];
                    int i = 0;
                    foreach (var col in queueTable.Columns)
                        row[i++] = upd.Value.returnValueField(upd.Value.Alias + "." + col.Id);
                    queueTable.Rows.Add(row);
                }

                foreach (var del in tab.Value.Deleted)
                    queueTable.RowsDelete.Add(del);

                message.Tables.Add(queueTable);
            }
        }

        public static object FormatFieldValue(object value, FieldFormatting format)
        {
            switch (format)
            {
                case FieldFormatting.LOGICO:
                    return ((int)value) == 1;
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    if((DateTime)value == DateTime.MinValue)
                        return "";
                    else
                        return ((DateTime)value).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFF");
                case FieldFormatting.BINARIO:
                case FieldFormatting.JPEG:
                    return Convert.ToBase64String((byte[])value);
                case FieldFormatting.GEO_SHAPE:
                case FieldFormatting.GEOMETRIC:
                    return value.ToString();
                default:
                    return value;
            }
        }


        public static AreaDataset ToDataSet(this QueueMessage message, SubscriberMetadata sub, User user)
        {
            var dataset = new AreaDataset();
            foreach (var table in message.Tables)
            {
                var st = sub.Tables.Find(t => t.Alias == table.Id);
                var dstable = dataset.AddTable(st.Name);

                var coltypes = new FieldFormatting[table.Columns.Count];
                for (int i = 0; i < coltypes.Length; i++)
                    if (Enum.TryParse<FieldFormatting>(table.Columns[i].DataType, out var ff))
                        coltypes[i] = ff;
                    else
                        coltypes[i] = FieldFormatting.CARACTERES;

                foreach (var row in table.Rows)
                {
                    var areaname = st.Name;
                    var area = Area.createArea(areaname, user, user.CurrentModule);
                    for (int i = 0; i < row.Length; i++)
                    {
                        var value = ParseFieldValue(row[i], coltypes[i]);
                        area.insertNameValueField(area.Alias + "." + table.Columns[i].Id, value);
                    }

                    dstable.Updated.Add(area.QPrimaryKey, area);
                }

                if (table.RowsDelete != null)
                    dstable.Deleted.AddRange(table.RowsDelete);
            }
            return dataset;
        }

        private static object ParseFieldValue(object value, FieldFormatting format)
        {
            switch (format)
            {
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    if (string.IsNullOrEmpty(value as string))
                        return DateTime.MinValue;
                    else
                        return DateTime.Parse(value as string, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
                case FieldFormatting.BINARIO:
                case FieldFormatting.JPEG:
                    return Convert.FromBase64String(value as string);
                case FieldFormatting.GEO_SHAPE:
                case FieldFormatting.GEOMETRIC:
                    return GeographicData.GetGeographyFromText(value as string);
                default:
                    return value;
            }
        }
    }
}
