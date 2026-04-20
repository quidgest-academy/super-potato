using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CSGenio.framework
{
    /// <summary>
    /// Classe com funções to tratamento de strings
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Faz split a uma string, utilizando a virgula (",") como caracter separador
        /// e a barra ("\") como caracter de escape
        /// </summary>
        /// <param name="str">string a dividir</param>
        /// <returns>array com as strings divididas</returns>
        public static string[] Split(string str)
        {
            return Split(str, ",", "\\");
        }

        /// <summary>
        /// Faz split a uma string, permitindo definir um caracter separador e outro de escape
        /// </summary>
        /// <param name="str">string a dividir</param>
        /// <param name="delimChar">caracter separador</param>
        /// <param name="escapeChar">caracter de escape</param>
        /// <returns>array com as strings divididas</returns>
        public static string[] Split(string str, string delimChar, string escapeChar)
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();
            // flag que indica se o caracter de escape foi encontrado
            bool escaped = false;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == delimChar[0])
                {
                    if (escaped)
                    {
                        // se encontramos o caracter separador, mas antes está o caracter de escape
                        // então adicionamos o caracter separador à string actual
                        sb.Append(delimChar);
                        escaped = false;
                    }
                    else
                    {
                        // encontramos o caracter separador, sem o caracter de escape antes
                        // então adiciona-se a string que está no "buffer" to o array do Qresult
                        // e cria-se um novo "buffer"
                        result.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                }
                else if (str[i] == escapeChar[0])
                {
                    if (escaped)
                    {
                        // se encontramos o caracter de escape repetido, "desliga-se" a flag
                        // e acrescenta-se o caracter de escape à string actual
                        escaped = false;
                        sb.Append(escapeChar);
                    }
                    else
                        // se encontramos o caracter de escape, activa-se a flag
                        escaped = true;
                }
                else
                {
                    // se escaped for true, devia dar erro? não se escapou nada!
                    // devia saltar o caracter actual?
                    // por agora fica assim, acrescenta sempre e altera a flag de escape
                    escaped = false;
                    sb.Append(str[i]);
                }
            }

            // se exists text no buffer, acrescenta-se um novo elemento ao array do Qresult
            if (sb.Length > 0)
                result.Add(sb.ToString());

            return result.ToArray();
        }

        /// <summary>
        /// Faz o join de um array de strings, utilizando o separador "," e o caracter de escape "\"
        /// </summary>
        /// <param name="strings">array de strings</param>
        /// <returns>strings concatenada</returns>
        public static string JoinEscaped(string[] strings)
        {
            return JoinEscaped(strings, ",", "\\");
        }

        /// <summary>
        /// Faz o join de um array de strings, permitindo especificar quais os characters to separar os elementos e to escapar
        /// </summary>
        /// <param name="strings">array de strings</param>
        /// <param name="separator">caracter separador</param>
        /// <param name="escape">caracter de escape</param>
        /// <returns>strings concatenadas</returns>
        public static string JoinEscaped(string[] strings, string separator, string escape)
        {
            string[] temp = new string[strings.Length];

            // escapar os characters separadores nas strings
            for (int i = 0; i < strings.Length; i++)
                temp[i] = strings[i].Replace(separator, escape + separator);

            return String.Join(separator, temp);
        }

        /// <summary>
        /// Retorna a string text com a primeira letra capitalizada e a outras em minusculas.
        /// </summary>
        /// <param name="texto">A string a ser transformada</param>
        /// <returns>A string com a primeira letra em capitalLetters e as restantes em minusculas</returns>
        public static string CapFirst(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (text.Length > 1)
                return text.Substring(0, 1).ToUpperInvariant() + text.Substring(1).ToLowerInvariant();
            else
                return text.ToUpperInvariant();
        }
		
		/// <summary>
        /// Remove diacritics from one string. (Remove spaces and accents)
        /// More info at:
        /// https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="removeSpaces">Define if want to remove spaces well</param>
        /// <returns>Modified Original text without Diacritics</returns>
        public static string RemoveDiacritics(string text, bool removeSpaces = true)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (!(removeSpaces && c == ' ') || !removeSpaces)
                        stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static StringBuilder arrayStringToStringBuilderCommaSeparated(string[] array)
        {
            StringBuilder Qresult = new StringBuilder();
            foreach (string valorArray in array)
                Qresult.Append(valorArray + ",");
            if (Qresult.Length > 0)
                Qresult = Qresult.Remove(Qresult.Length - 1, 1);
            return Qresult;
        }
    }
}
