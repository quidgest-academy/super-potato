using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework
{
    public static class CrystalConversion
    {
        /// <summary>
        /// Converte um objecto de Crystal to string
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToString(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToString", "Invalid Operation.");
        }

        /// <summary>
        /// Converte uma string to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromString(string Qvalue)
        {
            if (Qvalue == null)
                return "\"\"";
            else
                return "\"" + Qvalue.ToUpper() + "\"";
        }

        /// <summary>
        /// Converte um objecto de Crystal to numérico
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static decimal ToNumeric(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToNumeric", "Invalid Operation.");
        }
        /// <summary>
        /// Converte um numérico to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromNumeric(decimal Qvalue)
        {
            return Qvalue.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Converte um objecto de Crystal to inteiro
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static int ToInteger(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToInteger", "Invalid Operation.");
        }
        /// <summary>
        /// Converte um inteiro to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromInteger(int Qvalue)
        {
            return Convert.ToString(Qvalue);
        }

        /// <summary>
        /// Converte um objecto de Crystal to uma data
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static DateTime ToDateTime(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToDateTime", "Invalid Operation.");
        }

        /// <summary>
        /// Converte uma data to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromDateTime(DateTime Qvalue, bool hasTime, bool hasSeconds)
        {
            try
            {
                if (Qvalue == DateTime.MinValue)
                    return "";

                if (!hasTime)
                {
                    return $"Date({Qvalue.ToString("yyyy, MM, dd")})";
                }
                else
                {
                    string format = "yyyy, MM, dd, HH, mm";
                    if (hasSeconds)
                        format += ", ss";
                    else
                        format += ", 0";

                    return $"DateTime({Qvalue.ToString(format)})";
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Converte um objecto de Crystal to um lógico
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static int ToLogic(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToLogic", "Invalid Operation.");
        }

        /// <summary>
        /// Converte um lógico to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromLogic(int Qvalue)
        {
            return Qvalue.ToString();
        }

        /// <summary>
        /// Converte um objecto de Crystal to uma key primária ou estrangeira
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToKey(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToKey", "Invalid Operation.");
        }

        /// <summary>
        /// Converte uma key primária ou estrangeira to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromKey(string Qvalue)
        {
            if (string.IsNullOrEmpty(Qvalue))
                return "\"{00000000-0000-0000-0000-000000000000}\"";
            else
                return "\"{" + Qvalue.ToUpper() + "}\"";
        }

        /// <summary>
        /// Converte um objecto de Crystal to um binário
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static byte[] ToBinary(object Qvalue)
        {
            throw new FrameworkException(null, "CrystalConversion.ToBinary", "Invalid Operation.");
        }

        /// <summary>
        /// Converte uma binário to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
        public static string FromBinary(byte[] Qvalue)
        {
            if (Qvalue == null)
                return "";
            else
            {
                Byte[] file = (Byte[])Qvalue;
                string ficheiroString = "";
                ficheiroString = BitConverter.ToString(file).Replace("-", string.Empty);
                return ficheiroString;
            }
        }

        // (RS 2011.01.20) From acordo com http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa-in-c
        // esta conversão de byte array to string hexadecimal é bastante mais eficiente
        // TODO: testar e usar
        /*
        private static string ByteArrayToHex(byte[] barray) 
        {
            char[] c = new char[barray.Length * 2];
            byte b;
            for (int i = 0; i < barray.Length; ++i)
            {
                b = ((byte)(barray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(barray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c); 
        }
        */

        /// <summary>
        /// Converte um objecto de Crystal to um objecto interno
        /// </summary>
        /// <param name="valor">O objecto de Crystal</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static object ToInternal(object Qvalue, FieldFormatting formatting)
        {
            throw new FrameworkException(null, "CrystalConversion.ToInterno", "Invalid Operation.");
        }

        /// <summary>
        /// Converte um objecto interno to um objecto de Crystal
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to Crystal</returns>
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
                        int tmpInt;
                        if (!int.TryParse(Qvalue.ToString(), out tmpInt))
							throw new ArgumentException("Error converting " + Qvalue.ToString() + " to int.");
                        return FromLogic(tmpInt);
                    case FieldFormatting.FLOAT:
                        if (!decimal.TryParse(Qvalue.ToString(), out decimal tmpDbl))
							throw new ArgumentException("Error converting " + Qvalue.ToString() + " to numeric.");
                        return FromNumeric(tmpDbl);
                    case FieldFormatting.DATA:
                        DateTime tmpDt1;
                        if (!DateTime.TryParse(Qvalue.ToString(), out tmpDt1))
                            throw new ArgumentException("Error converting " + Qvalue.ToString() + " to DateTime.");
                        return FromDateTime(tmpDt1, false, false);
                    case FieldFormatting.DATAHORA:
                        DateTime tmpDtH;
                        if (!DateTime.TryParse(Qvalue.ToString(), out tmpDtH))
                            throw new ArgumentException("Error converting " + Qvalue.ToString() + " to DateTime.");
                        return FromDateTime(tmpDtH, true, false);
                    case FieldFormatting.DATASEGUNDO:
                        DateTime tmpDtS;
                        if (!DateTime.TryParse(Qvalue.ToString(), out tmpDtS))
							throw new ArgumentException("Error converting " + Qvalue.ToString() + " to DateTime.");
                        return FromDateTime(tmpDtS, true, true);
                    case FieldFormatting.JPEG:
                    case FieldFormatting.BINARIO:
                        return FromBinary(Qvalue as byte[]);
                    default:
						throw new ArgumentException("Format not recognized: " + formatting.ToString());
                }
            }
            catch (Exception ex)
            {
				throw new FrameworkException(null, "CrystalConversion.FromInterno", "Error converting " + Qvalue.ToString() + ": " + ex.Message, ex);
            }
        }
    }
}
