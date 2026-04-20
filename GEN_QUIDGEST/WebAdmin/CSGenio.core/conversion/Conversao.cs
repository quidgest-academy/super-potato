using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CSGenio.framework
{
    /// <summary>
    /// SO 20061003
    /// Classe que trata das conversões de string to tipo interno e vice-versa
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// Método que converte de string to tipo interno
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield em string</param>
        /// <param name="formatacaoCampo">formatação do Qfield</param>
        /// <returns>o Qvalue do Qfield em tipo interno</returns>
        public static object string2TypeInternal(string fieldValue,FieldFormatting fieldFormatting)
        {
            object valorInterno;

            switch(fieldFormatting)
            {
              case FieldFormatting.INTEIRO:
              case FieldFormatting.LOGICO:
                 valorInterno = string2Int(fieldValue);
                 break;
              case FieldFormatting.FLOAT:
                 valorInterno = string2Numeric(fieldValue);
                 break;
              case FieldFormatting.DATA:
                 valorInterno = dateString2DateTime(fieldValue);
                 break;
              case FieldFormatting.DATAHORA:
              case FieldFormatting.DATASEGUNDO:
                 valorInterno = dateTimeString2DateTime(fieldValue);
                 break;
              case FieldFormatting.TEMPO:
                 valorInterno = timeString2Time(fieldValue);
                 break;
              default:
                 valorInterno = fieldValue;
                 break;
            }
            return valorInterno;
        }

        /// <summary>
        /// Método que transforma o Qvalue do objecto em numérico
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qvalue do Qfield convertido</returns>
        public static decimal string2Numeric(string fieldValue)
        {
            //AV 20090525 o "." é usado como separador dos milhares por isso não pode ser convertido em ","
            //fieldValue = fieldValue.Replace('.',',');
            NumberFormatInfo provider = new NumberFormatInfo( );
            provider.NumberDecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            provider.NumberGroupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;

            return string2Numeric(fieldValue, provider);
        }

        /// <summary>
        /// Convert a string to numeric
        /// </summary>
        /// <param name="fieldValue">String value</param>
        /// <param name="provider">The number format info</param>
        /// <returns>The converted value or Zero when conversion was not succeeded</returns>
        public static decimal string2Numeric(string fieldValue, NumberFormatInfo provider)
        {
            try
            {
                if (fieldValue.Equals(""))
                    return 0;
                decimal Qvalue = Convert.ToDecimal(fieldValue, provider);

                return Qvalue;
            }
            catch (Exception ex)
            {
                Log.Error("string2Numeric Error converting from string to numeric: " + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Método que transforma o Qvalue do objecto em inteiro
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qvalue do Qfield convertido</returns>
        public static int string2Int(object fieldValue)
        {
            try
            {
                if (fieldValue.Equals(""))
                    return 0;

                int Qvalue = Convert.ToInt32(fieldValue);
                return Qvalue;
            }
            catch (Exception ex)
            {
				Log.Error("Conversion.string2 In Error converting from string to int: " + ex.Message);
                return 0;
            }
        }

        //é mais eficiente compilar as expressões regulares apenas uma vez e depois partilhar e reutilizar
        private static Regex Data4 = new Regex("((19|[2-9][0-9])[0-9][0-9])[-/.]([1-9]|0[1-9]|1[012])[-/.]([1-9]|0[1-9]|[12][0-9]|3[01])");
        private static Regex Data2 = new Regex("([1-9]|0[1-9]|[12][0-9]|3[01])[-/.]([1-9]|0[1-9]|1[012])[-/.]((19|[2-9][0-9])[0-9][0-9])");

        /// <summary>
        /// Método que converte uma data no format string to format datetime
        /// Pressupoe que as datas estão na forma day/month/Qyear
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static DateTime dateString2DateTime(string dataString)
        {
            try
            {
                if (dataString.Equals(""))
                    return DateTime.MinValue;

                string[] dataSplit = dataString.Split('/');
                if (dataSplit.Length == 1)
                    dataSplit = dataString.Split('-');
                if (dataSplit.Length != 3)
                {
                    dataSplit = dataString.Split('-');
                    if (dataSplit.Length != 3)
                        throw new ArgumentException("String not recognized as a date: " + dataString);
                }

                //o último atomo do split pode trazer consigo as horas ex: 2015 0:00:00
				//voltamos a fazer o split por espaço e aproveitamos apenas o que está à esquerda do espaço
                dataSplit[2] = dataSplit[2].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries)[0];
                /*validate datas na forma aaaa-mm-dd, o caracter separador pode ser .,/ ou -*/
                Regex expReg;
                if (dataSplit[0].Trim().Length == 4)
                    expReg = Data4;
                else
                    expReg = Data2;

                Match Qresult = expReg.Match(dataString);
                if (Qresult.Success)
                {
                    string Qyear;
                    string day;
                    if (dataSplit[0].Trim().Length == 4)
                    {
                        Qyear = Qresult.Groups[1].Value;
                        day = Qresult.Groups[3].Value;
                    }
                    else
                    {
                        Qyear = Qresult.Groups[3].Value;
                        day = Qresult.Groups[1].Value;
                    }
                    int anoInt = int.Parse(Qyear);
                    string month = Qresult.Groups[2].Value;

                    if (month.Length == 1)
                        month = "0" + month;
                    if (day.Length == 1)
                        day = "0" + day;

                    /*verificar se é um month de 30 days e o input de days é 31*/
                    if (day.Equals("31") && (month.Equals("04") || month.Equals("06") || month.Equals("09") || month.Equals("11")))
                        throw new ArgumentException("The month " + month + " doesn't have 31 days.");
                    /*Fevereiro nao tem 30, nem 31*/
                    else if (day.CompareTo("30") >= 0 && month.Equals("02"))
                        throw new ArgumentException("February has less than 30 days.");
                    /*Se o Qyear é bissexto*/
                    else if (month.Equals("02") && day.Equals("29") && !(anoInt % 4 == 0 && (anoInt % 100 != 0 || anoInt % 400 == 0)))
                        throw new ArgumentException("February doesn't have 29 days.");
                }
                else
                    throw new ArgumentException("String not recognized as a date: " + dataString);
                DateTime data;
                if (dataSplit[2].Trim().Length == 4)
                    data = new DateTime(int.Parse(dataSplit[2].Trim()), int.Parse(dataSplit[1].Trim()), int.Parse(dataSplit[0].Trim()));
                else
                    data = new DateTime(int.Parse(dataSplit[0].Trim()), int.Parse(dataSplit[1].Trim()), int.Parse(dataSplit[2].Trim()));

                return data;
            }
            catch (Exception ex)
            {
				Log.Error("Conversion.dateString2DateTime " + ex.Message);
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Método que converte uma data no format string to format datetime
        /// Pressupoe que as datas estão na forma day/month/Qyear
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static DateTime dateTimeString2DateTime(string dataString)
        {
            try
            {
                if (string.IsNullOrEmpty(dataString))
                    return DateTime.MinValue;

                // separa fields
                string[] dataSplit = dataString.Split('/','-',' ',':');

                //for datetime and datetime second
                if (dataSplit.Length != 5 && dataSplit.Length != 6)
                {
                    throw new ArgumentException("String not recognized as a date: " + dataString);
                }

                int hour = int.Parse(dataSplit[3].Trim());
                int minute = int.Parse(dataSplit[4].Trim());
                int second = dataSplit.Length == 6 ? int.Parse(dataSplit[5].Trim()) : 0;

                /*validate datas na forma aaaa-mm-dd, o caracter separador pode ser .,/ ou -*/
                Regex expReg;
                // verifica se é data com século ou não
                if (dataSplit[0].Trim().Length == 4)
                    expReg = Data4;
                else
                    expReg = Data2;

                Match Qresult = expReg.Match(dataString);

                if (Qresult.Success)
                {
                    string Qyear;
                    string day;
                    if (dataSplit[0].Trim().Length == 4)
                    {
                        Qyear = Qresult.Groups[1].Value;
                        day = Qresult.Groups[3].Value;
                    }
                    else
                    {
                        Qyear = Qresult.Groups[3].Value;
                        day = Qresult.Groups[1].Value;
                    }
                    int anoInt = int.Parse(Qyear);
                    string month = Qresult.Groups[2].Value;

                    if (month.Length == 1)
                        month = "0" + month;
                    if (day.Length == 1)
                        day = "0" + day;

                    /*verificar se é um month de 30 days e o input de days é 31*/
                    if (day.Equals("31") && (month.Equals("04") || month.Equals("06") || month.Equals("09") || month.Equals("11")))
                        throw new ArgumentException("The month " + month + " doesn't have 31 days.");
                    /*Fevereiro nao tem 30, nem 31*/
                    else if (day.CompareTo("30") >= 0 && month.Equals("02"))
                        throw new ArgumentException("February has less than 30 days.");
                    /*Se o Qyear é bissexto*/
                    else if (month.Equals("02") && day.Equals("29") && !(anoInt % 4 == 0 && (anoInt % 100 != 0 || anoInt % 400 == 0)))
                        throw new ArgumentException("February desn't have 29 days.");

                    // MA 20101019 Se a hour é inválida
                    else if (hour > 23 || hour < 0 || minute > 59 || minute < 0 || second > 59 || second < 0)
                    {
                        throw new ArgumentException("Invalid time: " + dataString);
                    }
                }
                else
                    throw new ArgumentException("String not recognized as a date: " + dataString);
                DateTime data;
                if (dataSplit[2].Trim().Length == 4)
                    data = new DateTime(int.Parse(dataSplit[2].Trim()), int.Parse(dataSplit[1].Trim()), int.Parse(dataSplit[0].Trim()), hour, minute, second);
                else
                    data = new DateTime(int.Parse(dataSplit[0].Trim()), int.Parse(dataSplit[1].Trim()), int.Parse(dataSplit[2].Trim()), hour, minute, second);

                return data;
            }
			catch (Exception ex)
            {
                /*
                 * From the analysis of the original code,
                 *  it was concluded that the Framework exception could only be used to log the error in the error log.
                 * And to keep the same functioning as the function had.
                 * We'll just log the error and return the default value.
                 */
                Log.Error("Conversion.dateTimeString2DateTime " + ex.Message);
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Recebe uma string que representa um Qvalue em horas e tenta converter to uma string vália de horas "HH:MM"
        /// </summary>
        /// <param name="horaString">horas</param>
        /// <returns>horas formatadas</returns>
        public static string timeString2Time(string time)
        {
            // se for vazio
            if (time == null || time == "" || time == "__:__")
                return "__:__";

            // retirar os underscores to aceitar horas no format _2:23
            time = time.Replace("_", "");

            string[] tempoArr = time.Split(':');

            if (tempoArr.Length != 2)
                throw new ArgumentException("Invalid time format. Need hour and minute parts. " + time);

            int horas, minutes;

            // parse das horas
            if (!int.TryParse(tempoArr[0], out horas))
                throw new ArgumentException("Invalid time format. Hour is not a number. " + time);

            // parse dos minutes
            if (!int.TryParse(tempoArr[1], out minutes))
                throw new ArgumentException("Invalid time format. Minute is not a number. " + time);

            // validação dos Qvalues
            if (horas < 0 || horas > 23 || minutes < 0 || minutes > 59)
                throw new ArgumentException("Invalid time format. Range overflow for hour or minute. " + time);

            // volta a converter to string, com o pading correcto to as horas e minutes
            string horasRes = horas.ToString().PadLeft(2, '0');
            string minutosRes = minutes.ToString().PadLeft(2, '0');

            return horasRes + ":" + minutosRes;
        }

        /// <summary>
        /// Método que converte uma data no format string to format datetime
        /// Pressupoe que as datas estão na forma day/month/Qyear
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static DateTime dateString2DateTime(string dataString, FieldFormatting formField)
        {
            try
            {
                if (string.IsNullOrEmpty(dataString))
                    return DateTime.MinValue;

                string[] dataSplit = dataString.Split('/');
                if (dataSplit.Length == 1)
                    dataSplit = dataString.Split('-');
                if (dataSplit.Length != 3)
                {
                    dataSplit = dataString.Split('-');
                    if (dataSplit.Length != 3)
                        throw new ArgumentException("String not recognized as date: " + dataString);
                }
                DateTime data;
                //day/month/Qyear
                if (dataSplit[2].Trim().Length == 4)
                    data = new DateTime(int.Parse(dataSplit[2].Trim()), int.Parse(dataSplit[1].Trim()), int.Parse(dataSplit[0].Trim()));
                else
                {
                    string temp = dataSplit[2];
                    string[] dataSplit2 = temp.Split(' ');
                    int Qyear = int.Parse(dataSplit2[0]);
                    int day = int.Parse(dataSplit[0].Trim());
                    int month = int.Parse(dataSplit[1].Trim());

                    data = new DateTime(Qyear, month, day);
                }
                return data;
            }
			catch (Exception ex)
            {
                /*
                 * From the analysis of the original code,
                 *  it was concluded that the Framework exception could only be used to log the error in the error log.
                 * And to keep the same functioning as the function had.
                 * We'll just log the error and return the default value.
                 */
                Log.Error("Conversion.dateString2DateTime " + ex.Message);
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Método que transforma o Qvalue do objecto em booleano
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qvalue do Qfield convertido</returns>
        public static bool string2Bool(string fieldValue)
        {
            try
            {
                if (string.IsNullOrEmpty(fieldValue))
                    return false;

                switch(fieldValue.ToLower())
                {
                    case "0":
                    case "false":
                        return false;

                    case "1":
                    case "true":
                        return true;
                }

                bool Qvalue = Convert.ToBoolean(fieldValue);
                return Qvalue;
            }
            catch (Exception ex)
            {
                /*
                 * From the analysis of the original code,
                 *  it was concluded that the Framework exception could only be used to log the error in the error log.
                 * And to keep the same functioning as the function had.
                 * We'll just log the error and return the default value.
                 */
                Log.Error("string2Bool Error converting from String to Boolean: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <param name="forCampo">formatação do Qfield</param>
        /// <returns>Qfield formatado</returns>
        public static object internal2InternalValid(object fieldValue, FieldFormatting forField)
        {
            switch (forField)
            {
                case FieldFormatting.INTEIRO:
                case FieldFormatting.LOGICO:
                    return internalInt2InternalValidInt(fieldValue);
                case FieldFormatting.FLOAT:
                    return internalNumeric2InternalValidNumeric(fieldValue);
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATA:
                case FieldFormatting.DATASEGUNDO:
                    return internalDateTime2InternalValidDateTime(fieldValue);
                case FieldFormatting.TEMPO:
                case FieldFormatting.CARACTERES:
                    return internalString2InternalValidString(fieldValue);
                case FieldFormatting.GUID:
                    return internalKey2InternalValidKey(fieldValue);
                case FieldFormatting.JPEG:
                    return internalBinary2InternalValidByteArray(fieldValue);
                case FieldFormatting.BINARIO:
                    return internalBinary2InternalValidByteArray(fieldValue);
                case FieldFormatting.GEOGRAPHY:
                    return internalString2Geography(fieldValue);
                case FieldFormatting.GEO_SHAPE:
                case FieldFormatting.GEOMETRIC:
                    return internalString2GeoShape(fieldValue);
                case FieldFormatting.ENCRYPTED:
                    return internal2InternalValidEncrypted(fieldValue);
                default:
                    throw new ArgumentException("Unkown FieldFormatting " + forField, nameof(forField));
            }
        }

		/// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue geography válido to executar calculos
        /// </summary>
        /// <param name="fieldValue">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado: POINT(Lng Lat)</returns>
        public static string internalString2Geography(object fieldValue)
        {
            string stringValue = "";

            if (fieldValue == null || fieldValue == DBNull.Value)
                return stringValue;
            else if (fieldValue is NpgsqlTypes.NpgsqlPoint np)
                return $"POINT({np.X} {np.Y})";
            else
            {
                stringValue = fieldValue.ToString();
                if (stringValue == "Null")
                    return "";
                else if (stringValue.StartsWith("POINT ("))
                    return stringValue.Replace("POINT (", "POINT("); // Não deve ter espaço depois do POINT
            }

            return stringValue;
        }

		/// <summary>
		/// Converts a value read from the DB to a valid geographic value that can be used to do calculations
		/// </summary>
		/// <param name="fieldValue">The value of the field</param>
		/// <returns>A geographic object with the converted value</returns>
        public static CSGenio.framework.Geography.GeographicData internalString2GeoShape(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return null;

            return CSGenio.framework.Geography.GeographicData.GetGeographyFromText(fieldValue.ToString());
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue string válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static string internalString2InternalValidString(object fieldValue)
        {
            string valorString = "";
            if (fieldValue == null || fieldValue == DBNull.Value)
                return valorString;
            else
            {
                //RS,CN (2009.10.01) Foi retirado o trim de plicas, isto nunca devia ser feito aqui
                if (fieldValue is string)
                    valorString = (string) fieldValue;
                else
                    valorString = fieldValue.ToString();
            }
            return valorString;
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue string válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static string internalKey2InternalValidKey(object fieldValue)
        {
            string valorString = "";
            if (fieldValue == null || fieldValue == DBNull.Value)
                return valorString;
            else
            {
                if (fieldValue is string)
                    valorString = ((string)fieldValue).Trim('\'');
                else if (fieldValue is Guid)
                {
                    valorString = (fieldValue.ToString()).Trim('\'');
                    if (valorString == "00000000-0000-0000-0000-000000000000")
                        valorString = "";
                }
				else if (fieldValue is byte[]) //RAW16 - Oracle
                    valorString = BitConverter.ToString((byte[])fieldValue).Replace("-", "");
                else
                    valorString = fieldValue.ToString();
            }
            return valorString;
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue int válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static int internalInt2InternalValidInt(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return 0;
            else if (fieldValue is int)
                return (int)fieldValue;
            else if (fieldValue is byte)
                return (byte)fieldValue;
            else if (fieldValue is bool)
                return (bool)fieldValue ? 1 : 0;
            else if (fieldValue is short)
                return (short)fieldValue;
            else if (fieldValue is long)
                return (int)((long)fieldValue);
            else if (fieldValue is decimal)
                return decimal.ToInt32((decimal)fieldValue);
            else if (fieldValue is double)
                return Convert.ToInt32(fieldValue);
            else if (fieldValue is float)
                return Convert.ToInt32(fieldValue);
            else if (fieldValue is string)
            {
                if (fieldValue.Equals(""))
                    return 0;
                else
                {
                    int Qvalue = 0;
                    if (int.TryParse(fieldValue.ToString(), out Qvalue))
                        return int.Parse(fieldValue.ToString());
                    else
                        return 0;
                }
            }
            else
                throw new ArgumentException("Unkown field type", nameof(fieldValue));
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue double válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">valor do campo</param>
        /// <returns>Normalized numeric value</returns>
        public static decimal internalNumeric2InternalValidNumeric(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return 0;
            else if (fieldValue is double)
                return Convert.ToDecimal(fieldValue);
            else if (fieldValue is float)
                return Convert.ToDecimal(fieldValue);
            else if (fieldValue is byte)
                return (byte)fieldValue;
            else if (fieldValue is short)
                return (short)fieldValue;
            else if (fieldValue is int)
                return (int)fieldValue;
            else if (fieldValue is long)
                return (long)fieldValue;
            else if (fieldValue is decimal)
                return (decimal)fieldValue;
            else if (fieldValue is string)
            {
                if (fieldValue.Equals(""))
                    return 0;
                else
                {
                    if (decimal.TryParse(fieldValue.ToString(), out decimal Qvalue))
                        return Qvalue;
                    else
                        return 0;
                }
            }
            else
                throw new ArgumentException("Error converting from database value to double: " + fieldValue);
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue data válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static DateTime internalDateTime2InternalValidDateTime(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return DateTime.MinValue;

            fieldValue = Convert.ToDateTime(fieldValue);

            if (fieldValue is DateTime)
                return (DateTime) fieldValue;
            else
                throw new ArgumentException("Error converting from database value to DateTime: " + fieldValue);
        }

        /// <summary>
        ///assinatura antiga to assegurar compatibilidade
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static Byte[] internalImage2InternalValidImage(object fieldValue)
        {
            return internalBinary2InternalValidByteArray(fieldValue);
        }

        /// <summary>
        /// Função que converte o Qvalue lido da BD to um Qvalue string válido to executar calculos
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static Byte[] internalBinary2InternalValidByteArray(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value || fieldValue.Equals(""))
                return new Byte[0];
            else
                return (Byte[])fieldValue;
        }

        /// <summary>
        /// Function that converts the read value from the Database to a valid encrypted object value.
        /// </summary>
        /// <param name="fieldValue">The field value read from the database</param>
        /// <returns>Encrypted data type object</returns>
        public static EncryptedDataType internal2InternalValidEncrypted(object fieldValue)
        {
            if (fieldValue is string)
                return new EncryptedDataType(fieldValue.ToString(), null);
            else if(fieldValue is EncryptedDataType)
                return (EncryptedDataType)fieldValue;

            // Empty or null value
            return new EncryptedDataType();
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno string em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        [Obsolete("Use persistence.ConversaoBD.FromString(valorCampo) instead")]
        public static string internalString2DbString(object fieldValue)
        {
            string Qresult="";
            if (fieldValue == null)
                Qresult = "''";
            else
            {
               Qresult = "'" + fieldValue.ToString().Replace("'", "''") + "'";
            }
            return Qresult;
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno string em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        [Obsolete("Use persistence.ConversaoBD.FromKey(valorCampo) instead")]
        public static string internalKey2DbKey(object fieldValue)
        {
            string Qresult = "NULL";
            if (fieldValue != null)
            {
                Qresult = fieldValue.ToString().Trim('\'');
                if (Qresult.Length == 0 || Qresult == "00000000-0000-0000-0000-000000000000")
                    Qresult = "NULL";
                else
                    Qresult = "'" + Qresult + "'";
            }
            return Qresult;
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno string que representa um GUID numa string de size 32
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string sem '{', '}' e '-' to ser aceite na BD</returns>
        public static string internalKeyGUID2DbKey32(object fieldValue)
        {
            string Qresult = "NULL";
            if (fieldValue != null)
            {
                Qresult = fieldValue.ToString().Trim('\'');
                if (Qresult.Length == 0 || Qresult == "00000000-0000-0000-0000-000000000000")
                    Qresult = "NULL";
                else
                {
                    if (fieldValue.ToString().Length == 36)
                    {
                        string[] aux = Qresult.Split('-');
                        if (aux.Length == 5)
                            Qresult = aux[0] + aux[1] + aux[2] + aux[3] + aux[4];
                    }
                }
            }
            return "'" + Qresult + "'";
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno int em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalInt2DbInt(object fieldValue)
        {
            if (fieldValue == null)
                return "0";
            else
                return fieldValue.ToString();
        }

        /// <summary>
        //assinatura antiga to assegurar compatibilidade
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        [Obsolete("Use persistence.ConversaoBD.FromBinary(valorCampo) instead")]
        public static string internalJpeg2DbJpeg(object fieldValue)
        {
            return internalByteArray2DbBinary(fieldValue);
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno jpeg em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        [Obsolete("Use persistence.ConversaoBD.FromBinary(valorCampo) instead")]
        public static string internalByteArray2DbBinary(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return "''";
            else
            {
                if (fieldValue is Byte[])
                {
                    Byte[] file = (Byte[])fieldValue;
                    string ficheiroString = "0x" + BitConverter.ToString(file).Replace("-", string.Empty);
                    return ficheiroString;

                }
                else
                    throw new ArgumentException("Error converting from internal type to string: " + fieldValue.ToString());
            }
        }

        /// <summary>
        /// Função que converte do tipo interno to string
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield no format interno</param>
        /// <param name="forCampo">formatação do Qfield</param>
        /// <returns>Qvalue do Qfield em string</returns>
        /// SO 20070514 alterei de FieldFormatting to FieldType porque no caso das arrays
        /// numéricas o tipo de Qfield é double, mas interface espera uma string e não um double.
        /// Se a array não está preenchida deve ser enviada uma string vazia e não 0
        public static string internal2String(object fieldValue, FieldType tpField)
        {
            try
            {
                FieldFormatting forField = tpField.GetFormatting();
                switch (forField)
                {
                    case FieldFormatting.INTEIRO:
                    case FieldFormatting.LOGICO:
                        return internalInt2String(fieldValue);
                    case FieldFormatting.FLOAT:
                        if (tpField.Equals(FieldType.ARRAY_NUMERIC))
                            return internalArrayNumerical2String(fieldValue);
                        else
                            return internalNumeric2String(fieldValue);
                    case FieldFormatting.DATAHORA:
                    case FieldFormatting.DATASEGUNDO:
                        if (fieldValue is DateTime)
                            return dateTime2DateString((DateTime)fieldValue, forField);
                        else
                            return fieldValue.ToString();
                    case FieldFormatting.DATA:
                        return internalDateTime2String(fieldValue);
                    case FieldFormatting.TEMPO:
                    case FieldFormatting.CARACTERES:
                    case FieldFormatting.GUID:
                        return internalString2String(fieldValue);
                    case FieldFormatting.JPEG:
                        return fieldValue.ToString();
                    case FieldFormatting.GEOGRAPHY:
                        return internalString2Geography(fieldValue);
                    case FieldFormatting.GEO_SHAPE:
                    case FieldFormatting.GEOMETRIC:
                        return internalString2GeoShape(fieldValue).ToString();
                    default:
                        throw new ArgumentException("The format is not defined: " + forField.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new FrameworkException(null, "Conversion.tipoInterno2String", "Error converting from internal type to string: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Função que converte do tipo interno to string
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield no format interno</param>
        /// <param name="tipoCampo">Type de Qfield</param>
        /// <returns>Qvalue do Qfield em string</returns>
        public static string internal2String(object fieldValue, Type fieldType)
        {
            if (fieldType.Equals(typeof(System.Byte))
                || fieldType.Equals(typeof(System.Int16))
                || fieldType.Equals(typeof(System.Int32))
                || fieldType.Equals(typeof(System.Int64)))
                return internalInt2String(fieldValue);

            if (fieldType.Equals(typeof(System.Double)) || fieldType.Equals(typeof(System.Decimal)) || fieldType.Equals(typeof(System.Single)))
                return internalNumeric2String(fieldValue);

            if (fieldType.Equals(typeof(System.DateTime)))
                return internalDateTime2String(fieldValue);

            if (fieldType.Equals(typeof(System.String)) || fieldType.Equals(typeof(System.Guid)))
                return internalString2String(fieldValue);

            throw new ArgumentException("The type is not defined: " + fieldType.ToString());
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno string em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalString2String(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return "";
            else
                return fieldValue.ToString();
        }


        /// <summary>
        /// Função que transforma um Qfield de tipo interno double em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalNumeric2String(object fieldValue)
        {
            if (fieldValue == null || fieldValue ==DBNull.Value)
                return "0.0";
            else
                return fieldValue.ToString();
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno double em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalArrayNumerical2String(object fieldValue)
		{
            if (fieldValue == null || fieldValue == DBNull.Value)
                return "";
            // MA + SL 20100528 Este if fazia com que um preenchimento do array com 0 levasse o controlo a não mostrar nada e foi introduzido no day 15/5/2007 na alteração 2867 por RS
            //else if (fieldValue.ToString() == "0.0" || fieldValue.ToString() == "0")
            //    return "";
            else
                return fieldValue.ToString();
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno int em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalInt2String(object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return "0";
            else
                return fieldValue.ToString();
        }

        /// <summary>
        /// Função que transforma um Qfield de tipo interno DateTime em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalDateTime2String(object fieldValue)
        {
            if (fieldValue == null || fieldValue ==DBNull.Value)
                return "";

            if (fieldValue is DateTime)
            {
                DateTime data = (DateTime)fieldValue;
                if (data.Equals(DateTime.MinValue))
                    return "";
                else
                    return Conversion.dateTime2DateString(data, FieldFormatting.DATA);
            }
            if (fieldValue is string)
                return fieldValue.ToString();
            else
                throw new ArgumentException("Error converting internal date type to string: " + fieldValue.ToString());
        }


        /// <summary>
        /// Função que transforma um Qfield de tipo interno DateTime em string aceite numa query
        /// </summary>
        /// <param name="valorCampo">Qvalue Qfield</param>
        /// <param name="formCampo">formatação do Qfield</param>
        /// <returns>a string com o Qfield convertido to ser aceite na BD</returns>
        public static string internalDateTime2String(object fieldValue, FieldFormatting formField)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return "";

            if (fieldValue is DateTime)
            {
                DateTime data = (DateTime)fieldValue;
                if (data.Equals(DateTime.MinValue))
                    return "";
                else
                    return Conversion.dateTime2DateString(data, formField);
            }
            if (fieldValue is string)
                return fieldValue.ToString();
            else
                throw new ArgumentException("Error converting internal date type to string: " + fieldValue.ToString());
        }

        /// <summary>
        /// Método que converte uma data no format string to format datetime
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        //SO 20060831
        public static string dateTime2DateString(DateTime data, FieldFormatting formField)
        {
            if (data.Equals(DateTime.MinValue))
                return "";
            string dataString = "";

            //Assume que a data vai ser fornecida ao Qweb e portanto deve ser sempre entregue no format Dia/month/Qyear
            dataString += data.Day.ToString().PadLeft(2, '0') + "/";
            dataString += data.Month.ToString().PadLeft(2, '0') + "/";
            dataString += data.Year.ToString().PadLeft(4, '0');

            if (formField.Equals(FieldFormatting.DATAHORA) || formField.Equals(FieldFormatting.DATASEGUNDO))
            {
                dataString += " " + data.Hour.ToString().PadLeft(2, '0') + ":";
                dataString += data.Minute.ToString().PadLeft(2, '0');
                if (formField.Equals(FieldFormatting.DATASEGUNDO))
                    dataString += ":" + data.Second.ToString().PadLeft(2, '0');
            }

            return dataString;
        }


        /// <summary>
        /// Função que converte uma string com characters de quebras de linha to um Qvalue string válido
        /// </summary>
        /// <param name="valorCampo">Qvalue do Qfield</param>
        /// <returns>Qfield string formatado</returns>
        public static string memo2String(string Qvalue)
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

        public static string stringConditions2String(string Qvalue)
        {
            if (Qvalue.StartsWith("''") && Qvalue.EndsWith("''"))
            {
                Qvalue = Qvalue.Trim('\'');
                Qvalue = "'" + Qvalue + "'";
            }
            else
                Qvalue = Qvalue.Trim('\'');

            return Qvalue;
        }
    }
}