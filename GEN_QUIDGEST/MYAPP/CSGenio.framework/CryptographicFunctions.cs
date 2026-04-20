using System;
using System.IO;
using System.Security.Cryptography;

namespace CSGenio.framework
{
    /// <summary>
    /// Classe com funções utilitárias relativas à criptografia
    /// </summary>
    public class CryptographicFunctions
    {
        /// <summary>
        /// Decifra um byte array cifrado com AES e devolve o conteudo original
        /// </summary>
        /// <param name="key">Key simétrica</param>
        /// <param name="data">Dados cifrados</param>
        /// <returns>Dados decifrados</returns>
        public static byte[] DecryptData(byte[] key, byte[] data)
        {
            if (data.Length < 16)
                throw new ArgumentException("Data must be at least 16 bytes long to extract IV.", "data");

            MemoryStream streamOut = new MemoryStream();
            using (Aes alg = Aes.Create())
            {
                byte[] iv = new byte[16];
                Array.Copy(data, 0, iv, 0, 16);
                alg.IV = iv;

                ICryptoTransform decryptor = alg.CreateDecryptor(key, alg.IV);
                using (MemoryStream streamIn = new MemoryStream(data, 16, data.Length-16))
                using (CryptoStream cryptoStream = new CryptoStream(streamIn, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(streamOut);
                }
            }
            streamOut.Flush();
            return streamOut.ToArray();
        }

        /// <summary>
        /// Cifra um byte array e devolve o conteudo cifrado
        /// </summary>
        /// <param name="key">Key simétrica</param>
        /// <param name="data">Dados originais</param>
        /// <returns>Dados cifrados</returns>
        public static byte[] EncryptData(byte[] key, byte[] data)
        {
            using (Aes alg = Aes.Create())
            {
                alg.GenerateIV();
                ICryptoTransform encryptor = alg.CreateEncryptor(key, alg.IV);
                using (MemoryStream stream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                {
                    stream.Write(alg.IV, 0, alg.IV.Length);

                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    stream.Flush();
                    byte[] dataCrypto = stream.ToArray();
                    return dataCrypto;
                }
            }
        }
    }
}
