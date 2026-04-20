using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;


namespace GenioServer.security
{
    public class PasswordFactory
    {
        //variáveis utilizadas to o Argon2
        private const int argTime = 3;          //t= A time cost, which defines the execution time
        private const int argMemory = 8192;     //m= A memory cost, which defines the memory usage
        private const int argParallelism = 1;   //p= A parallelism degree, which defines the number of threads
        private const int argVersion = 19;      //v= Argon2 version (defaults to the most recent version)

        public enum PswTypes
        {
            ARG,    //argon2d
            QUI     //Quidgest
        }

        /// <summary>
        /// Função to encriptar uma string
        /// </summary>
        /// <param name="I">string a ser encriptada</param>
        /// <param name="c0">inteiro usado na encriptação</param>
        /// <param name="c1">inteiro usado na encriptação</param>
        /// <returns>string encriptada</returns>
        private static string password_encriptarComBug(string I, int c0, int c1)
        {
            // as passwords nulas não são convertidas
            if (I.Length != 0)
            {
                // acrescenta chars à String
                I += "Quidgest!";

                // calcula a seed ou gera uma (0 a 31 + 33) - ao calcular, se não for válida termina
                int seed;
                if (c0 == 0)
                {
                    var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
                    var b = new byte[1];
                    rnd.GetBytes(b);
                    seed = (b[0] & 0b00011111) + 33;
                }
                else
                {
                    int C = ~I[0] & 0xFF;
                    int C0 = (C >> 4);
                    int C1 = (C & 0xF);
                    c0 -= C0;
                    c1 -= C1;
                    if (c0 != c1)
                        return I;
                    seed = c0;
                }

				if(I.Length > 20)
					return I;

                // Encripta a String
                while (I.Length != 20)
                    I += ' ';
                char[] Ic = I.ToCharArray();
                int i, i0, i1;
                for (i = 0, i0 = 18, i1 = 19; i <= 9; i++, i0 -= 2, i1 -= 2)
                {
                    int C = (int)((~Ic[i]) & 0xFF);
                    int C0 = (C >> 4), C1 = (C & 0xF);
                    Ic[i0] = (char)(seed + C0);
                    Ic[i1] = (char)(seed + C1);
                    int S = (seed & 3) + 1;
                    seed += S;
                }
                I = "";
                for (int j = 0; j < Ic.Length; j++)
                    I += Ic[j];
            }
            return I;
        }


        /// <summary>
        /// Encrypt e validar password do tipo Argon2
        /// https://github.com/mheyman/Isopoh.Cryptography.Argon2
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private static string password_encriptarArgon2(string pass, string salt)
        {
            string hash = "";
            if (pass.Length != 0)
            {
                //Se não existir salt usa por default um random de 8 characters
                if (salt.Length == 0)
                    salt = StringRandom(8, true);

                var cfg = new Isopoh.Cryptography.Argon2.Argon2Config
                {
                    Type = Isopoh.Cryptography.Argon2.Argon2Type.DataDependentAddressing,
                    TimeCost = argTime,
                    MemoryCost = argMemory,
                    Lanes = argParallelism,
                    Password = Encoding.UTF8.GetBytes(pass),
                    Salt = Encoding.UTF8.GetBytes(salt),
                };

                //gerar a hash da pass enviada por parametro
                hash = Isopoh.Cryptography.Argon2.Argon2.Hash(cfg);

                //só vou retirar do hash o start do hash que inclui o "$argon2d$"
                hash = hash.Substring(9);
            }

            return hash;
        }

        /// <summary>
        /// Verificar se password correcta
        /// </summary>
        /// <param name="passEnc">Password Encriptada</param>
        /// <param name="pass">Password a ser verificada</param>
        /// <returns></returns>
        private static bool password_checkArgon2(string passEnc, string pass, string salt)
        {
            //Exemplo:   $argon2d$v=19$m=8192,t=3,p=1$N3FxQ2xEVGM$8bjpNgC32Adf0X1Pj9Sgo3hH1Cz88OhR89AOdRJu6NI
            //Vou assumir que tem uma parte sempre fixa + salt + pass, o salt por agora vai incluir também as variáveis iniciais (v, m, t, p)
            passEnc = String.Format("$argon2d${0}", passEnc);

            return Isopoh.Cryptography.Argon2.Argon2.Verify(passEnc, Encoding.UTF8.GetBytes(pass), 1);
        }

        /// <summary>
        /// Função to redirecionar to o tipo de encriptação correcto
        /// </summary>
        /// <param name="I">string a ser encriptada</param>
        /// <param name="encripted">string usada na encriptação (salt)</param>
        /// <param name="passwordAlgorithm">Encryption algorithm</param>
        /// <returns>string encriptada</returns>
        private static string password_encriptar(string I, string encripted, PasswordAlgorithms passwordAlgorithm)
        {
            if (passwordAlgorithm == PasswordAlgorithms.ARG)
                return password_encriptarArgon2(I, encripted);
            else
				// [RC] 03/10/2017 No need to convert the password string to upper here,
			    // password_encriptarQuid will do that
                return password_encriptarQuid(I, encripted);
        }


        /// <summary>
        /// New função de encriptação corrigida
        /// </summary>
        /// <param name="I">string a ser encriptada</param>
        /// <param name="c0">inteiro usado na encriptação</param>
        /// <param name="c1">inteiro usado na encriptação</param>
        /// <returns>string encriptada</returns>
        private static string password_encriptarQuid(string I, string encripted)
        {
            // as passwords nulas não são convertidas
            if (I != null && I.Length != 0)
            {
                //todas as chamadas a esta função faziam por default o upper
                //serpassei to o start da encriptação
                I = I.ToUpper();

                // as passwords nulas não são convertidas
                int size = I.Length;

                // size da password (mínimo de 9) e da string encriptada (dobro mais 2)
                if (size < 9) size = 9;
                int encsize = size * 2 + 2;

                // acrescenta chars à String
                I += "Quidgest!";

                // calcula a seed ou gera uma (0 a 31 + 33) - ao calcular, se não for válida termina
                int seed;
                if (string.IsNullOrEmpty(encripted))
                {
                    var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
                    var b = new byte[1];
                    rnd.GetBytes(b);
                    seed = (b[0] & 0b00011111) + 33;
                }
                else
                {
					if (encripted.Length < encsize)
                        return I;

                    int C = (int)((~I.ToCharArray()[0]) & 0xFF);
                    int C0 = (C >> 4);
                    int C1 = (C & 0xF);
                    int c0 = encripted[encsize - 2];
                    int c1 = encripted[encsize - 1];
                    c0 -= C0;
                    c1 -= C1;
                    if (c0 != c1)
                        return I;
                    seed = c0;
                }

                // Encripta a String
                while (I.Length != encsize)
                    I += ' ';
                char[] Ic = I.ToCharArray();
                char[] Ix = (char[])Ic.Clone();
                int i, i0, i1;
                for (i = 0, i0 = encsize - 2, i1 = encsize - 1; i <= size; i++, i0 -= 2, i1 -= 2)
                {
                    int C = (int)((~Ix[i]) & 0xFF);
                    int C0 = (C >> 4), C1 = (C & 0xF);
                    Ic[i0] = (char)(seed + C0);
                    Ic[i1] = (char)(seed + C1);
                    int S = (seed & 3) + 1;
                    seed += S;
                }
                I = "";
                for (int j = 0; j < Ic.Length; j++)
                    I += Ic[j];
            }
            return I;
        }

        /// <summary>
        /// Function to create a hash from the original password.
        /// </summary>
        /// <param name="password">Password to be Hashed</param>
        /// <returns>The Password Hashed</returns>
        public static string Encrypt(string password)
        {
            return Encrypt(password, Configuration.Security.PasswordAlgorithms);
        }

        /// <summary>
        /// Function to create a hash from the original password.
        /// </summary>
        /// <param name="password">Password to be Hashed</param>
        /// <param name="passwordAlgorithms">Encryption algorithm</param>
        /// <returns>The Password Hashed</returns>
        public static string Encrypt(string password, PasswordAlgorithms passwordAlgorithm)
        {
            return password_encriptar(password, "", passwordAlgorithm);
        }


        private static bool password_checkQuidgest (string strPassword, string strEncripted)
        {
            if (strEncripted == password_encriptarQuid(strPassword, strEncripted))
                return true;
            else if (strEncripted != null && strEncripted.Length == 20 && strPassword != null)
            {
                // encripta a password com a mesma semente
                string aux = password_encriptarComBug(strPassword.ToUpper(), strEncripted[18], strEncripted[19]);
                // retorna o Qresult
                return (aux == strEncripted);
            }
            else
                return false;
        }

        /// <summary>
        /// Função to verificar se a password é a correcta
        /// </summary>
        /// <param name="strPassword">password inserida</param>
        /// <param name="strEncripted">password encriptada</param>
        ///
        /// <returns>true se for correcta, false caso contrário</returns>
        public static bool IsOK(string strPassword, string strEncripted, string salt, string type)
        {
            //RMR(2016-09-07) - Doesn't limit the password length when verifying the old password
            //if (strPassword.Length > 9)
            //    return false;

            // Temporary bugfix
            if (string.IsNullOrEmpty(type))
                return password_checkQuidgest(strPassword, strEncripted);

            //JGF 2018.01.23 If the password type has something not recognizable, use checkQuidgest by default.
            PswTypes pswType = PswTypes.QUI;
            if (Enum.IsDefined(typeof(PswTypes), type))
            {
                pswType = (PswTypes)Enum.Parse(typeof(PswTypes), type);
            }

            switch (pswType)
            {
                case PswTypes.ARG:
                    return password_checkArgon2(strEncripted, strPassword, salt);
                case PswTypes.QUI:
                default:
                    return password_checkQuidgest(strPassword, strEncripted);
            }
        }

        /// <summary>
        /// Geração de string random
        /// </summary>
        /// <param name="len">Size da string a ser retornada</param>
        /// <param name="hasCaps">Se a string deve de incluir characters maiúsculos</param>
        /// <returns></returns>
        public static string StringRandom(int len, bool hasCaps)
        {
            var chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            if (hasCaps)
                chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var stringChars = new char[len];
            var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
            var b = new byte[len];
            rnd.GetBytes(b);
            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[ b[i] % chars.Length ];

            return new String(stringChars);
        }

        /// <summary>
        /// Função to gerar uma nova password
        /// </summary>
        /// <returns>a nova password</returns>
        public static string New()
        {
            return StringRandom(9, false);
        }

        /// <summary>
        /// Score the strength of a password
        /// </summary>
        /// <returns>The password to check</returns>
		public static double scorePassword(string pass)
        {
            double score = 0.0;
            if (pass == "")
                return score;//PasswordStrength.Pobre;

            // award every unique letter until 5 repetitions
            Dictionary<char, int> letters = new Dictionary<char, int>();
            foreach (char x in pass)
            {
                if (letters.ContainsKey(x))
                    letters[x] += 1;
                else
                    letters.Add(x, 1);
                score += 5.0 / Convert.ToInt32(letters[x]);
            }

            int variationCount = 0;
            variationCount += Regex.IsMatch(pass, @"\d") ? 1: 0; //digits
            variationCount += Regex.IsMatch(pass, @"[a-z]") ? 1 : 0; //lower
            variationCount += Regex.IsMatch(pass, @"[A-Z]") ? 1 : 0; //upper
            variationCount += Regex.IsMatch(pass, @"\W") ? 1 : 0; //nonWords

            score += (variationCount - 1) * 10;

            return score;
        }


        /// <summary>
        /// The ecription function for the password fields
        /// </summary>
        /// <param name="plainPasswordText">The decrypted value of the field.</param>
        /// <param name="passwordType">The Password Algorithms</param>
        /// <returns>Encrypted data</returns>
        public static string EncryptPasswordField(string plainPasswordText, string passwordType)
        {
            // The function will need the encryption type information that is stored in one of the fields of the record.
            if(plainPasswordText != null && !string.IsNullOrEmpty(passwordType))
            {
                Enum.TryParse(passwordType, out PasswordAlgorithms pswType);
                string encriptedString = Encrypt(plainPasswordText, pswType);
                return encriptedString;
            }

            return null;
        }


    }
}