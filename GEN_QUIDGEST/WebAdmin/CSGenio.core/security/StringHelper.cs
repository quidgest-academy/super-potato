using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Security;

namespace GenioServer.security
{
    public class StringHelper
    {
        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            byte[] result = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return result;
        }

        public static string BytesToHexString(byte[] input)
        {
            StringBuilder hexString = new StringBuilder(64);

            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(String.Format("{0:X2}", input[i]));
            }
            return hexString.ToString();
        }

        /// <summary>
        /// Converts a plain text password into a SecureString object.
        /// </summary>
        /// <param name="plainPassword">The plain text password.</param>
        /// <returns>A SecureString object representing the password.</returns>
        public static SecureString GetSecureString(string plainPassword)
        {
            SecureString securePassword = new SecureString();
            foreach (char c in plainPassword)
            {
                securePassword.AppendChar(c);
            }
            return securePassword;
        }
    }
}