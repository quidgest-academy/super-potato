using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework
{
    public class HourFunctions
    {
        /// <summary>
        // Converte um Qfield com o format de horas do Genio em um real com o
        // número de horas decorridas desde 00:00 com os minutes nas digits decimais
        /// </summary>
        /// <param name="cHoras0">A hour em format __:__</param>
        /// <returns>O número de horas decorridos desde 00:00</returns>
        public static decimal HoursToDouble(string cHours0)
        {
            if (cHours0.Length < 5)
                return 0;
            cHours0 = cHours0.Replace('_', '0');
            int.TryParse(cHours0.Substring(0, 2), out int h0);
            int.TryParse(cHours0.Substring(3, 2), out int m0);
            if (h0 < 0 || h0 > 23 || m0 < 0 || m0 > 59)
                return 0;
            return h0 + (m0 / 60m);
        }

        /// <summary>
        ///  Adiciona minutes a um fields no format de horas do Genio
		/// Não decresce de 00:00 ou incrementa de 23:59
        /// </summary>
        /// <param name="cHoras0">A hour em format __:__</param>
        /// <param name="minutos">O number de minutes a adicionar</param>
        /// <returns>A nova hour com os minutes adicionados</returns>
        public static string HoursAdd(string cHours0, decimal minutes)
        {
            if (cHours0 == null)
                return "__:__";
            if (cHours0.Length < 5)
                return "__:__";
            cHours0 = cHours0.Replace('_', '0');
            Int32.TryParse(cHours0.Substring(0, 2), out int h0);
            Int32.TryParse(cHours0.Substring(3, 2), out int m0);
            if (h0 < 0 || h0 > 23 || m0 < 0 || m0 > 59)
                return "__:__";

            int resInt = h0 * 60 + m0 + (int)minutes;
            if (resInt < 0) resInt = 0;
            if (resInt > 23 * 60 + 59) resInt = 23 * 60 + 59;

            return (resInt / 60).ToString("D2") + ':' + (resInt % 60).ToString("D2");
        }
    }
}
