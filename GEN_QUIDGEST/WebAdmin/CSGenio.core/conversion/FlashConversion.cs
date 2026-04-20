using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework
{
    public static class FlashConversion
    {
        /// <summary>
        /// Converte um objecto de flash to uma data
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static DateTime ToDateTime(object Qvalue)
        {
            if (Qvalue == null)
                return DateTime.MinValue;
            else
                return Convert.ToDateTime(Qvalue);
        }

        /// <summary>
        /// Converte uma data to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromDateTime(DateTime Qvalue, bool hasTime, bool hasSeconds)
        {
            try
            {
                if (Qvalue == DateTime.MinValue)
                    return "";
                StringBuilder sb = new StringBuilder();
               

                //Assume que a data vai ser fornecida ao Qweb e portanto deve ser sempre entregue no format Dia/month/Qyear
                sb.Append( Qvalue.Year.ToString().PadLeft(4, '0') + "/" );
                sb.Append( Qvalue.Month.ToString().PadLeft(2, '0') + "/" );
                sb.Append( Qvalue.Day.ToString().PadLeft(2, '0') );

                if (hasTime)
                {
                    sb.Append( " " + Qvalue.Hour.ToString().PadLeft(2, '0') + ":" );
                    sb.Append( Qvalue.Minute.ToString().PadLeft(2, '0') );
                    if (hasSeconds)
                        sb.Append( ":" + Qvalue.Second.ToString().PadLeft(2, '0') );
                }

                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }		
 
    }
}
