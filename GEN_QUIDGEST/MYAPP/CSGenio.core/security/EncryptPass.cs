using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for EncryptPass
/// </summary>
namespace GenioServer.security
{
    public static class EncryptPass
    {
        private static RSACryptoServiceProvider rsaCryptoServiceProvider = null;
        private static RSAParameters parametersRSA;
        /// <summary>
        /// construtor
        /// </summary>
        static EncryptPass()
        {
            CspParameters RSAParams = new CspParameters();
            RSAParams.Flags = CspProviderFlags.NoFlags;
            rsaCryptoServiceProvider = new RSACryptoServiceProvider(2048, RSAParams);
        }
        /// <summary>
        /// Obter parametros to conseguir ficar com a key publica e privada
        /// </summary>
        public static void GetParameterRSA()
        {
            parametersRSA = rsaCryptoServiceProvider.ExportParameters(false);
        }
        public static byte[] exponent()
        {
            return parametersRSA.Exponent;
        }
        public static byte[] Modulus()
        {
            return parametersRSA.Modulus;
        }

        public static byte[] Decrypt(byte[] txt)
        {
            byte[] result;

            result = rsaCryptoServiceProvider.Decrypt(txt, false);

            return result;
        }
    }
}