using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using CSGenio.framework;

namespace CSGenio.persistence
{
    /// <summary>
    /// Classe que agrega todas as conversões de dados entre a base de dados e os tipos internos
    /// </summary>
    public class DBConversion
    {
        /// <summary>
        /// Converte um objecto de base de dados to string
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToString(object Qvalue)
        {
            if (Qvalue == null || Qvalue == DBNull.Value)
                return "";
            return Qvalue.ToString();
        }

        /// <summary>
        /// Converte uma string to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromString(string Qvalue)
        {
            string Qresult = "";
            if (Qvalue == null)
                Qresult = "''";
            else
                Qresult = "'" + Qvalue.Replace("'", "''") + "'";

            return Qresult;
        }

        /// <summary>
        /// Converte um objecto de base de dados to numérico
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static decimal ToNumeric(object Qvalue)
        {
            if (Qvalue == null || Qvalue == DBNull.Value)
                return 0;
            if (Qvalue is double)
                return (decimal)((double)Qvalue);
            if (Qvalue is float)
                return Convert.ToDecimal(Qvalue);
            if (Qvalue is byte)
                return (decimal)((byte)Qvalue);
            if (Qvalue is short)
                return (decimal)((short)Qvalue);
            if (Qvalue is int)
                return (decimal)((int)Qvalue);
            if (Qvalue is long)
                return (decimal)((long)Qvalue);
            if (Qvalue is decimal)
                return (decimal)Qvalue;
            if (Qvalue is string)
            {
                if (Qvalue.Equals(""))
                    return 0;

                decimal temp = 0;
                if (!decimal.TryParse(Qvalue.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out temp) &&
                    !decimal.TryParse(Qvalue.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out temp))
                    return 0;
                else
                    return temp;
            }

            return 0;
        }
        /// <summary>
        /// Converte um numérico to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromNumeric(decimal Qvalue)
        {
            return Qvalue.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Converte um objecto de base de dados to inteiro
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static int ToInteger(object Qvalue)
        {
            try
            {
                return Convert.ToInt32(Qvalue);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converte um inteiro to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromInteger(int Qvalue)
        {
            return Convert.ToString(Qvalue);
        }

        /// <summary>
        /// Converte um objecto de base de dados to uma data
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static DateTime ToDateTime(object Qvalue)
        {
            if (Qvalue == null || Qvalue == DBNull.Value)
                return DateTime.MinValue;
            else if (Qvalue is DateTime)
                return (DateTime)Qvalue;
            else
                throw new FrameworkException(null, "DBConversion.ToDateTime", "Invalid object type to convert: " + Qvalue.ToString());
        }

        /// <summary>
        /// Converte uma data to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromDateTime(DateTime Qvalue)
        {
#pragma warning disable 618 //TODO ficam marcados assim explicitamente to saber quer tem de ser mudado
            return FromDateTime(Qvalue, Configuration.LegacyConnectionType);
#pragma warning restore 618
        }

        public static string FromDateTime(DateTime Qvalue, DatabaseType link)
        {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            DateTime data = (DateTime)Qvalue;
            if (data.Equals(DateTime.MinValue))
                return "NULL";

            string dataString;
            switch (link)
            {
                case DatabaseType.ORACLE:
                    dataString = "TO_DATE('" + data.Year + "/" + data.Month + "/" + data.Day + " " + data.Hour + ":" + data.Minute + ":" + data.Second + "', 'YYYY/MM/DD hh24:mi:ss')";
                    break;
                case DatabaseType.SQLSERVER:
                case DatabaseType.SQLSERVERCOMPAT:
                    dataString = "convert(datetime, '" + data.Day + "/" + data.Month + "/" + data.Year + " " + data.ToLongTimeString() + "', 103)";
                    break;
                default:
                    throw new FrameworkException(null, "DBConversion.FromDateTime", "Connection type not recognized: " + link.ToString());
            }

            return dataString;
        }

        /// <summary>
        /// Converte um objecto de base de dados to um lógico
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static int ToLogic(object Qvalue)
        {
            if (Qvalue == null || Qvalue == DBNull.Value)
                return 0;
            else if (Qvalue is byte)
                return (byte)Qvalue;
            else if (Qvalue is int)
                return (int)Qvalue;
            else if (Qvalue is short)
                return (short)Qvalue;
            else if (Qvalue is decimal)
                return decimal.ToInt32((decimal)Qvalue);
            else if (Qvalue is double)
                return Convert.ToInt32(Qvalue);
            else if (Qvalue is string)
            {
                if (Qvalue.Equals(""))
                    return 0;
                else
                {
                    int temp = 0;
                    if (int.TryParse(Qvalue.ToString(), out temp))
                        return temp;
                    else
                        return 0;
                }
            }
            else if (Qvalue is bool)
            {
                return ((bool)Qvalue) ? 1 : 0;
            }
            return 0;
        }
        /// <summary>
        /// Converte um lógico to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromLogic(int Qvalue)
        {
            return Qvalue.ToString();
        }

        /// <summary>
        /// Converte um objecto de base de dados to uma key primária ou estrangeira
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToKey(object Qvalue)
        {
            string valorString = "";
            if (Qvalue == null || Qvalue == DBNull.Value)
                return valorString;
            else
            {
                if (Qvalue is string)
                    valorString = ((string)Qvalue).Trim('\'');
                else if (Qvalue is Guid)
                    valorString = (Qvalue.ToString()).Trim('\'');
				else if (Qvalue is byte[]) //RAW16 - Oracle
                    valorString = BitConverter.ToString((byte[])Qvalue).Replace("-", "");
                else
                    valorString = Qvalue.ToString();

                if (valorString == "00000000-0000-0000-0000-000000000000")
                    valorString = "";
            }
            return valorString;
        }

        /// <summary>
        /// Converte uma key primária ou estrangeira to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromKey(string Qvalue)
        {
            string Qresult = "NULL";
            if (Qvalue != null)
            {
                Qresult = Qvalue.ToString().Trim('\''); ;
                if (Qresult.Length == 0 || Qresult == "00000000-0000-0000-0000-000000000000")
                    Qresult = "NULL";
                else
                    Qresult = "'" + Qresult + "'";
            }
            return Qresult;
        }

        /// <summary>
        /// Converte um objecto de base de dados to um binário
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static byte[] ToBinary(object Qvalue)
        {
            if (Qvalue == null || Qvalue == DBNull.Value || Qvalue.Equals(""))
                return new Byte[0];
            else

                return (Byte[])Qvalue;
        }

        /// <summary>
        /// Converts a database object to an encrypted data object.
        /// </summary>
        /// <param name="value">The database object</param>
        /// <returns>The object converted to the built-in type</returns>
        public static EncryptedDataType ToEncrypted(object value)
        {
            if (value == null || value == DBNull.Value)
                return new EncryptedDataType();
            else
                return new EncryptedDataType(value, null);
        }

        /// <summary>
        /// Converte uma binário to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromBinary(byte[] Qvalue)
        {
            if (Qvalue == null)
                return "''";
            else
            {
                Byte[] file = (Byte[])Qvalue;
                string ficheiroString = "";
                //(RS 2011.01.20) To oracle os binários foram implementados históricamente como varchar2(4000)
                // infelizmente isto tem sérias limitações ao size. A medida certa seria passar a usar CLOB ou BLOB.
#pragma warning disable 618 //TODO ficam marcados assim explicitamente to saber quer tem de ser mudado
                if (Configuration.LegacyConnectionType == DatabaseType.ORACLE)
                    ficheiroString = "'" + BitConverter.ToString(file).Replace("-", string.Empty) + "'";
                else
                    ficheiroString = "0x" + BitConverter.ToString(file).Replace("-", string.Empty);
#pragma warning restore 618
                return ficheiroString;
            }
        }

        private static string ValidateGeography(object fieldValue)
        {
            return Conversion.internalString2Geography(fieldValue);
        }

        /// <summary>
        /// Converte um objecto de base de dados to um geography
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToGeography(object fieldValue)
        {
            //this is specific to Postgres, so it should belong to the PersistentSupport class
            //however, in that logic, this whole class should be specific of database vendor
            if (fieldValue is NpgsqlTypes.NpgsqlPoint np)
                return $"POINT({np.X} {np.Y})";
            return ValidateGeography(fieldValue);
        }

        /// <summary>
        /// Converte um Qfield geography to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromGeography(object fieldValue)
        {
            return ValidateGeography(fieldValue);
        }

        /// <summary>
		/// Converts a value read from the DB to a valid geographic value that can be used to do calculations
		/// </summary>
		/// <param name="fieldValue">The value of the field</param>
		/// <returns>A geographic object with the converted value</returns>
        public static CSGenio.framework.Geography.GeographicData ToGeographicShape(object fieldValue)
        {
            return Conversion.internalString2GeoShape(fieldValue);
        }

        /// <summary>
		/// Converts a geographic value to a value that can be stored in the DB
		/// </summary>
		/// <param name="fieldValue">The value of the field</param>
		/// <returns>A string with the converted value</returns>
        public static string FromGeographicShape(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return "";

            string stringValue = fieldValue.ToString();
            return stringValue == "Null" ? "" : stringValue;
        }

        /// <summary>
        /// Converts a encrypted data object to a value that can be stored in the DB
        /// </summary>
        /// <param name="value">The value of the field</param>
        /// <returns>A string with the converted value</returns>
        public static string FromEncrypted(object value)
        {
            // The 'EncryptedDataType' class contains a converter to string that always returns the encrypted value.
            // However, it may be necessary to add another type of conversion in the future.
            return FromString((string)value);
        }

        /// <summary>
        /// Converte um objecto de base de dados to um objecto interno
        /// </summary>
        /// <param name="valor">O objecto de base de dados</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static object ToInternal(object Qvalue, FieldFormatting formatting)
        {
            switch (formatting)
            {
                case FieldFormatting.INTEIRO:
                case FieldFormatting.LOGICO:
                    return ToLogic(Qvalue);
                case FieldFormatting.FLOAT:
                    return ToNumeric(Qvalue);
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    return ToDateTime(Qvalue);
                case FieldFormatting.TEMPO:
                case FieldFormatting.CARACTERES:
                    return ToString(Qvalue);
                case FieldFormatting.GEOGRAPHY:
                    return ToGeography(Qvalue);
                case FieldFormatting.GEO_SHAPE:
                case FieldFormatting.GEOMETRIC:
                    return ToGeographicShape(Qvalue);
                case FieldFormatting.GUID:
                    return ToKey(Qvalue);
                case FieldFormatting.JPEG:
                case FieldFormatting.BINARIO:
                    return ToBinary(Qvalue);
                case FieldFormatting.ENCRYPTED:
                    return ToEncrypted(Qvalue);
            }
            throw new FrameworkException(null, "DBConversion.ToInterno", "Format not recognized: " + formatting.ToString());
        }

        /// <summary>
        /// Converte um objecto interno to um objecto de base de dados
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to base de dados</returns>
        public static string FromInternal(object Qvalue, FieldFormatting formatting)
        {
            try
            {
                switch (formatting)
                {
                    case FieldFormatting.TEMPO:
                    case FieldFormatting.CARACTERES:
                        return FromString(Qvalue as string);
                    case FieldFormatting.GUID:
                        return FromKey(Qvalue as string);
                    case FieldFormatting.INTEIRO:
                    case FieldFormatting.LOGICO:
                        return FromLogic((int)Qvalue);
                    case FieldFormatting.FLOAT:
                        return FromNumeric((decimal)Qvalue);
                    case FieldFormatting.DATAHORA:
                    case FieldFormatting.DATA:
                    case FieldFormatting.DATASEGUNDO:
                        return FromDateTime((DateTime)Qvalue);
                    case FieldFormatting.JPEG:
                    case FieldFormatting.BINARIO:
                        return FromBinary(Qvalue as byte[]);
                    case FieldFormatting.GEOGRAPHY:
                        return FromGeography(Qvalue);
                    case FieldFormatting.GEO_SHAPE:
                    case FieldFormatting.GEOMETRIC:
                        return FromGeographicShape(Qvalue);
                    case FieldFormatting.ENCRYPTED:
                        return FromEncrypted(Qvalue);
                    default:
                        throw new FrameworkException(null, "DBConversion.FromInterno", "Format not recognized: " + formatting.ToString());
                }
            }
			catch (GenioException ex)
			{
				if (ex.ExceptionSite == "DBConversion.FromInterno")
					throw;
				throw new FrameworkException(ex.UserMessage, "DBConversion.FromInterno", "Error converting from internal data type to database type: " + ex.Message, ex);
			}
            catch (Exception ex) //os cast podem falhar
            {
                throw new FrameworkException(null, "DBConversion.FromInterno", "Error converting from internal data type to database type: " + ex.Message, ex);
            }
        }
    }
}