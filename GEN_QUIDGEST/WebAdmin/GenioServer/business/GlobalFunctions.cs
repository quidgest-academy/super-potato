using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.core.persistence;
using GenioServer.security;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR IMPORTS]/
// USE /[MANUAL FOR IMPORTS GlobalFunctions]/

namespace CSGenio.business
{
    /// <summary>
    /// Summary description for GlobalFunctions.
    /// </summary>
    public sealed partial class GlobalFunctions
    {
        static Hashtable todasFuncoes;
        private User user;
        private string module;
        private PersistentSupport sp;

        /// <summary>
        /// Funções Globais do programa
        /// </summary>
        static GlobalFunctions()
        {
            try
            {
                initTodasFuncoes();
            }
            catch (Exception exc)
            {
                Log.Error($"[GlobalFunctions] Error in static constructor: {exc.Message}");
                throw;
            }
        }

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="utilizador">User em sessão</param>
        /// <param name="modulo">module que o user está a aceder</param>
        /// <param name="sp">suporte persistente que o user está a utilizar</param>
        public GlobalFunctions(User user, string module, PersistentSupport sp)
        {
            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("Cria instância de GlobalFunctions. [utilizador] {0} [modulo {1}", ( user != null ? user.Name : ""), module));

            this.user = user;
            this.module = module;
            this.sp = sp;
            if (this.sp == null && user != null)
                this.sp = PersistentSupport.getPersistentSupport(user.Year, User.Name);
        }

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="utilizador">User em sessão</param>
        /// <param name="modulo">module que o user está a aceder</param>
        public GlobalFunctions(User user, string module) : this(user, module, null)
        { }

        /// <summary>
        /// module
        /// </summary>
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// user
        /// </summary>
        public User User
        {
            get { return user; }
        }

        /// <summary>
        /// Função to verificar se a função é válida
        /// </summary>
        /// <param name="funcao">name da função</param>
        /// <returns>true se for válida, false caso contrário</returns>
        public static bool functionValidate(string function)
        {
            return todasFuncoes.ContainsKey(function);
        }

        private void checkFunctionArgs(string[] obj, int minLength = 4)
        {
            if (obj.Length < minLength)
                throw new ArgumentOutOfRangeException("obj", $"Object that represents the function arguments has a length inferior to {minLength}");
        }

        /// <summary>
        /// Executes a named global function
        /// </summary>
        /// <param name="nome">Name of the function</param>
        /// <param name="obj">function arguments</param>
        /// <returns>The result of the executed function</returns>
        public object executeFunction(string name,string[] obj)
        {
            try
            {
                if (obj == null)
                    throw new ArgumentNullException("obj", "The string array that was passed to executeFunction is null");

                if (!todasFuncoes.ContainsKey(name))
                    throw new ArgumentException("The function required is unknown", "name");

                if (Log.IsDebugEnabled) Log.Debug($"Executing global function: {name} args: {String.Join(", ", obj)}");

                switch ((int) todasFuncoes[name])
                {
                    case 0:
                        {
                            checkFunctionArgs(obj, 4);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            string arg2 = obj[2];
                            string arg3 = obj[3];
                            return password_alterar(arg0,arg1,arg2,arg3);
                        }
                    case 1:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return password_verificaAntiga(arg0,arg1);
                        }
                    case 2:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return validateSignature(arg0, arg1);
                        }
                    case 3:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return returnFieldsSignature(arg0, arg1);
                        }
                    case 4:
                        {
                            checkFunctionArgs(obj, 3);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            string arg2 = obj[2];
                            return writeSignature(arg0, arg1, arg2);
                        }
                    case 5:
                        {
                            checkFunctionArgs(obj, 2);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            return password_gerar(arg0, arg1);
                        }
                    case 6:
                        {
                            checkFunctionArgs(obj, 4);

                            string arg0 = obj[0];
                            string arg1 = obj[1];
                            string arg2 = obj[2];
                            string arg3 = obj[3];
                            return CreateDocumQweb(arg0,arg1,arg2,arg3);
                        }
                    case 7:
                        {
                            return GetUserProfile();
                        }
                    default:
                        throw new BusinessException(null, "GlobalFunctions.executaFuncao", "Unknown function name: " + name);
                }
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "GlobalFunctions.executaFuncao")
                    throw;
                throw new BusinessException(ex.UserMessage, "GlobalFunctions.executaFuncao", "Error executing global function " + name + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BusinessException(null, "GlobalFunctions.executaFuncao", "Error executing global function " + name + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Regista o Certificado na base de dados se a identificacion e password fizerem match
        /// </summary>
        /// <param name="page"></param>
        /// <param name="identificaco"></param>
        /// <param name="password"></param>
        /// <param name="ano"></param>
        /// <param name="certificado"></param>
        public void regista_certificado(string identificacion, string password, ClientCertificate Qcertificate)
        {
            //Verifica a password (se não for correcta, uma excepção é lançada)
            GenioServer.security.UserPassCredential credential = new GenioServer.security.UserPassCredential();
            credential.Year = Configuration.DefaultYear;
            credential.Username = identificacion;
            credential.Password = password;


            User principal = null;
            try
            {
                principal = GenioServer.security.SecurityFactory.Authenticate(credential);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Dados de login incorretos.", "GlobalFunctions.regista_certificado", ex.Message, ex);
            }

            SelectQuery certificateNotUsedQuery = new SelectQuery()
                .Select(CSGenioApsw.FldCertsn)
                .From(CSGenioApsw.AreaPSW)
                .Where(CriteriaSet.And()
                    .Equal(CSGenioApsw.FldCertsn, Qcertificate.returnSerialNumber()))
                .PageSize(1);

            DataMatrix certificateNotUsed = sp.Execute(certificateNotUsedQuery);

            if (certificateNotUsed.NumRows > 0)
                throw new BusinessException("Dados de login incorretos.", "GlobalFunctions.regista_certificado", "Certificate already used by another user.");

            user = GenioServer.security.UserFactory.ReadEphs(principal);

            //Numero Serie do Certificado
            registerCertificateSerialNumber(Qcertificate.returnSerialNumber());
        }

        /// <summary>
        /// Regista o Certificado na base de dados (User já autenticado).
        /// </summary>
        /// <param name="identificaco"></param>
        /// <param name="ano"></param>
        /// <param name="certificado"></param>
        public void regista_certificado(String identificacion, ClientCertificate Qcertificate)
        {
            SelectQuery certificateNotUsedQuery = new SelectQuery()
                .Select(CSGenioApsw.FldCertsn)
                .From(CSGenioApsw.AreaPSW)
                .Where(CriteriaSet.And()
                    .Equal(CSGenioApsw.FldCertsn, Qcertificate.returnSerialNumber()))
                .PageSize(1);

            DataMatrix certificateNotUsed = sp.Execute(certificateNotUsedQuery);

            if (certificateNotUsed.NumRows > 0)
                throw new BusinessException("Dados de login incorretos.", "GlobalFunctions.regista_certificado", "Certificate already used by another user.");

            //Numero Serie do Certificado
            registerCertificateSerialNumber(Qcertificate.returnSerialNumber());
        }

        /// <summary>
        /// Método to preencher uma instancia da table psw e validar se o Qcertificate está associado a alguma conta.
        /// </summary>
        /// <param name="psw">instancia da classe psw</param>
        /// <param name="certificado">Certificado Cliente</param>
        /// <returns>classe psw preenchida e validada</returns>
        public CSGenioApsw certificado_preencheValida(CSGenioApsw psw, ClientCertificate Qcertificate)
        {
            try
            {
                psw.insertNameValueField("psw.codpsw", "");
                psw.insertNameValueField("psw.password", "");
                psw.insertNameValueField("psw.nome", "");
                psw.insertNameValueField("psw.certsn", "");

                sp.selectOne(CriteriaSet.And().Equal("psw", "certsn", Qcertificate.returnSerialNumber()), null, psw, "");
                return psw;
            }
            catch (GenioException ex)
            {
                throw new BusinessException("Erro na validação das credenciais.", "GlobalFunctions.certificado_preencheValida", "Error validating certificate: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to validar se a password antiga inserida está válida
        /// </summary>
        /// <param name="passAntiga">password antiga</param>
        /// <returns>true se estiver válida, excepção caso contrário</returns>
        public bool password_verificaAntiga(string codpsw, string oldPass)
        {
            try
            {
                AreaInfo pswInfo = CSGenioApsw.GetInformation();
                //string codpsw = User.Codpsw;
                CSGenioApsw area = new CSGenioApsw(user, user.CurrentModule);
                if (sp.getRecord(area, codpsw, new[] { "password", "salt", "pswtype" }) && GenioServer.security.PasswordFactory.IsOK(oldPass, area.ValPassword, area.ValSalt, area.ValPswtype))
                    return true;
                else
                    throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_verificaAntiga", "The old password is not correct.");
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "GlobalFunctions.password_verificaAntiga")
                    throw;
                if (ex.UserMessage == null)
                    throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_verificaAntiga", "The old password is not correct.");
                else
                    throw new BusinessException("Erro na verificação da password antiga: " + ex.UserMessage, "GlobalFunctions.password_verificaAntiga", "The old password is not correct.");
            }
            catch (Exception ex)
            {
                throw new BusinessException("Erro na verificação da password antiga.", "GlobalFunctions.password_verificaAntiga", "Error verifying old password: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método to change uma password
        /// </summary>
        /// <param name="passAntiga">password antiga</param>
        /// <param name="passNova">nova password</param>
        /// <param name="repeticaoPassNova">repetição da nova password</param>
        /// <returns>true a alteração for bem sucedida, false caso contrário</returns>
        public object password_alterar(string codpsw, string oldPass, string newPass,string newPassRepetition)
        {
            newPass = newPass.TrimEnd();
            newPass = newPass.TrimStart();
            newPassRepetition = newPassRepetition.TrimEnd();
            newPassRepetition = newPassRepetition.TrimStart();
            PersistentSupport sp = PersistentSupport.getPersistentSupport(User.Year, User.Name);

            try
            {
                if (string.IsNullOrEmpty(codpsw))
                    codpsw = User.Codpsw;

                if (User.Codpsw == codpsw || User.IsAdminInAnyModule())
                {
                    // Change the user's password
                    foreach (var identityProvider in SecurityFactory.IdentityProviderList)
                        if (identityProvider.HasUsernameAuth())
                            SecurityFactory.StoreCredential(identityProvider.Id, user, oldPass, newPass);
                }
                return true;
            }
            catch (GenioException ex)
            {
                if (ex.ExceptionSite == "GlobalFunctions.password_alterar")
                    throw;
                if (ex.UserMessage == null)
                    throw new BusinessException("A mudança de password falhou. Por favor corrija os erros e tente de novo", "GlobalFunctions.password_alterar", "Error changing password: " + ex.Message, ex);
                else
                    throw new BusinessException("Erro na alteração da password: " + ex.UserMessage, "GlobalFunctions.password_alterar", "Error changing password: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // [RC] 06/06/2017 We must rollback in every error situation
                //if (ex is PersistenceException)
                    //sp.rollbackTransaction();
                throw new BusinessException("A mudança de password falhou. Por favor corrija os erros e tente de novo", "GlobalFunctions.password_alterar", "Error changing password: " + ex.Message, ex);
            }
        }

        public object password_gerar(string codpsw, string email)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(User.Year, User.Name);

            try
            {
                if (string.IsNullOrEmpty(Configuration.PP_Name) || string.IsNullOrEmpty(Configuration.PP_Email))
                    throw new BusinessException("Não podem ser geradas passwords sem o smtp e email de envio configurados.", "GlobalFunctions.password_gerar", "Email is not configured.");

                CSGenioApsw psw = new CSGenioApsw(User, module);

                if (string.IsNullOrEmpty(codpsw))
                    codpsw = User.Codpsw;

                if (User.Codpsw == codpsw || User.IsAdminInAnyModule())
                {
                    var newpass = GenioServer.security.PasswordFactory.StringRandom(9, true);
                    sendEmail(Configuration.PP_Name, "password CAV", email, newpass);

                    string passwordEncriptada = GenioServer.security.PasswordFactory.Encrypt(newpass);
                    psw.insertNameValueField("psw.password", passwordEncriptada.Replace("'", "''"));
                    psw.insertNameValueField("psw.codpsw", codpsw);
                    psw.insertNameValueField("psw.salt", "");
                    psw.insertNameValueField("psw.pswtype", Configuration.Security.PasswordAlgorithms.ToString());
                    sp.openTransaction();
                    psw.change(sp, (CriteriaSet)null);
                    sp.closeTransaction();
                }
                return true;
            }
            catch (GenioException ex)
            {
                sp.rollbackTransaction();
                if (ex.ExceptionSite == "GlobalFunctions.password_gerar")
                    throw;
                if (ex.UserMessage == null)
                    throw new BusinessException("Erro ao gerar password.", "GlobalFunctions.password_gerar", "Error generating password: " + ex.Message, ex);
                else
                    throw new BusinessException("Erro ao gerar password: " + ex.UserMessage, "GlobalFunctions.password_gerar", "Error generating password: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // [RC] 06/06/2017 We must rollback in every error situation
                //if (ex is PersistenceException)
                    //sp.rollbackTransaction();
                sp.rollbackTransaction();
                throw new BusinessException("Erro ao gerar password.", "GlobalFunctions.password_gerar", "Error generating password: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Implementação do método iif
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="teste"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static T iif<T>(bool test, T v1, T v2)
        {
            return (test ? v1 : v2);
        }

        public static T iif<T>(SqlBoolean test, T v1, T v2)
        {
            return (test ? v1 : v2);
        }

        public static T iif<T>(int test, T v1, T v2)
        {
            return (test == 1 ? v1 : v2);
        }

        /// <inheritdoc cref="GenFunctions.DateAddDays"/>
		[Obsolete("Use GenFunctions.DateAddDays instead")]
        public static DateTime SumDays(DateTime data, decimal days)
			=> GenFunctions.DateAddDays(data, days);

        /// <inheritdoc cref="GenFunctions.IsLeapYear"/>
		[Obsolete("Use GenFunctions.IsLeapYear instead")]
		public static bool IsLeapYear(int year)
			=> GenFunctions.IsLeapYear(year);

        /// <inheritdoc cref="GenFunctions.Capitalize"/>
		[Obsolete("Use GenFunctions.Capitalize instead")]
        public static string Capitalize(string text)
			=> GenFunctions.Capitalize(text);

        /// <inheritdoc cref="GenFunctions.CapitalizeInitials"/>
		[Obsolete("Use GenFunctions.CapitalizeInitials instead")]
        public static string CapitalizeInitials(string text)
			=> GenFunctions.CapitalizeInitials(text);

        /// <inheritdoc cref="GenFunctions.atoi"/>
		[Obsolete("Use GenFunctions.atoi instead")]
        public static int atoi(object a)
			=> GenFunctions.atoi(a);

        /// <inheritdoc cref="GenFunctions.IntToString"/>
		[Obsolete("Use GenFunctions.IntToString instead")]
        public static string IntToString(decimal a)
			=> GenFunctions.IntToString(a);

        /// <inheritdoc cref="GenFunctions.NumericToString"/>
		[Obsolete("Use GenFunctions.NumericToString instead")]
        public static string NumericToString(decimal Qvalue, int decimalDigits)
			=> GenFunctions.NumericToString(Qvalue, decimalDigits);

        /// <inheritdoc cref="GenFunctions.emptyD"/>
		[Obsolete("Use GenFunctions.emptyD instead")]
        public static int emptyD(object data)
			=> GenFunctions.emptyD(data);

        /// <inheritdoc cref="GenFunctions.emptyG"/>
		[Obsolete("Use GenFunctions.emptyG instead")]
        public static int emptyG(object characters)
			=> GenFunctions.emptyG(characters);

        /// <inheritdoc cref="GenFunctions.emptyC"/>
		[Obsolete("Use GenFunctions.emptyC instead")]
        public static int emptyC(object characters)
			=> GenFunctions.emptyC(characters);

        /// <inheritdoc cref="GenFunctions.emptyN"/>
		[Obsolete("Use GenFunctions.emptyN instead")]
        public static int emptyN(object Qvalue)
			=> GenFunctions.emptyN(Qvalue);

        /// <inheritdoc cref="GenFunctions.emptyT"/>
		[Obsolete("Use GenFunctions.emptyT instead")]
        public static int emptyT(object characters)
			=> GenFunctions.emptyT(characters);

        /// <inheritdoc cref="GenFunctions.emptyL"/>
		[Obsolete("Use GenFunctions.emptyL instead")]
        public static int emptyL(object Qvalue)
			=> GenFunctions.emptyL(Qvalue);

        /// <inheritdoc cref="GenFunctions.FormatDate"/>
		[Obsolete("Use GenFunctions.FormatDate instead")]
        public static string FormatDate(DateTime Qvalue, string format)
			=> GenFunctions.FormatDate(Qvalue, format);

        /// <inheritdoc cref="GenFunctions.LTRIM"/>
		[Obsolete("Use GenFunctions.LTRIM instead")]
        public static string LTRIM(string Qvalue)
			=> GenFunctions.LTRIM(Qvalue);

        /// <inheritdoc cref="GenFunctions.RTRIM"/>
		[Obsolete("Use GenFunctions.RTRIM instead")]
        public static string RTRIM(string Qvalue)
			=> GenFunctions.RTRIM(Qvalue);

        /// <inheritdoc cref="GenFunctions.Year"/>
		[Obsolete("Use GenFunctions.Year instead")]
        public static int Year(DateTime Qvalue)
			=> GenFunctions.Year(Qvalue);

        /// <inheritdoc cref="GenFunctions.Month"/>
		[Obsolete("Use GenFunctions.Month instead")]
        public static int Month(DateTime Qvalue)
			=> GenFunctions.Month(Qvalue);

        /// <inheritdoc cref="GenFunctions.Day"/>
		[Obsolete("Use GenFunctions.Day instead")]
        public static int Day(DateTime Qvalue)
			=> GenFunctions.Day(Qvalue);

        /// <inheritdoc cref="GenFunctions.strYear"/>
		[Obsolete("Use GenFunctions.strYear instead")]
        public static string strYear(DateTime Qvalue)
			=> GenFunctions.strYear(Qvalue);

        /// <inheritdoc cref="GenFunctions.Today"/>
		[Obsolete("Use GenFunctions.Today instead")]
        public static DateTime Today()
			=> GenFunctions.Today();

        /// <inheritdoc cref="GenFunctions.Now"/>
		[Obsolete("Use GenFunctions.Now instead")]
        public static DateTime Now()
			=> GenFunctions.Now();

        /// <inheritdoc cref="GenFunctions.HoursToDouble"/>
		[Obsolete("Use GenFunctions.HoursToDouble instead")]
        public static decimal HoursToDouble(string time)
			=> GenFunctions.HoursToDouble(time);

        /// <inheritdoc cref="GenFunctions.HoursAdd"/>
		[Obsolete("Use GenFunctions.HoursAdd instead")]
        public static string HoursAdd(string time, decimal minutes)
			=> GenFunctions.HoursAdd(time, minutes);

        /// <inheritdoc cref="GenFunctions.KeyToString"/>
		[Obsolete("Use GenFunctions.KeyToString instead")]
        public static string KeyToString(string key)
			=> GenFunctions.KeyToString(key);

        /// <inheritdoc cref="GenFunctions.StringToKey"/>
		[Obsolete("Use GenFunctions.StringToKey instead")]
        public static string StringToKey(string str)
			=> GenFunctions.StringToKey(str);

        /// <inheritdoc cref="GenFunctions.DoubleToHours"/>
		[Obsolete("Use GenFunctions.DoubleToHours instead")]
        public static string DoubleToHours(decimal time)
			=> GenFunctions.DoubleToHours(time);

        /// <inheritdoc cref="GenFunctions.CreateDateTime"/>
		[Obsolete("Use GenFunctions.CreateDateTime instead")]
        public static DateTime CreateDateTime(decimal year, decimal month, decimal day, decimal hour, decimal minute, decimal second)
			=> GenFunctions.CreateDateTime(year, month, day, hour, minute, second);

        /// <inheritdoc cref="GenFunctions.CreateDateTime"/>
		[Obsolete("Use GenFunctions.CreateDateTime instead")]
        public static DateTime CreateDateTime(decimal year, decimal month, decimal day)
			=> GenFunctions.CreateDateTime(year, month, day);

        /// <inheritdoc cref="GenFunctions.DateSetTime"/>
		[Obsolete("Use GenFunctions.DateSetTime instead")]
        public static DateTime DateSetTime(DateTime date, string time)
			=> GenFunctions.DateSetTime(date, time);

        /// <inheritdoc cref="GenFunctions.DateCompare"/>
		[Obsolete("Use GenFunctions.DateCompare instead")]
        public static int DateCompare(DateTime date1, DateTime date2)
			=> GenFunctions.DateCompare(date1, date2);

        /// <inheritdoc cref="GenFunctions.CreateDuration"/>
		[Obsolete("Use GenFunctions.CreateDuration instead")]
        public static TimeSpan CreateDuration(int days, int hours, int minutes, int seconds)
			=> GenFunctions.CreateDuration(days, hours, minutes, seconds);

        /// <inheritdoc cref="GenFunctions.DateDiff"/>
		[Obsolete("Use GenFunctions.DateDiff instead")]
        public static TimeSpan DateDiff(DateTime startDate, DateTime endDate)
			=> GenFunctions.DateDiff(startDate, endDate);

        /// <inheritdoc cref="GenFunctions.DateDiffPart"/>
		[Obsolete("Use GenFunctions.DateDiffPart instead")]
        public static decimal DateDiffPart(DateTime startDate, DateTime endDate, string unit)
			=> GenFunctions.DateDiffPart(startDate, endDate, unit);

        /// <inheritdoc cref="GenFunctions.DateAddDuration"/>
		[Obsolete("Use GenFunctions.DateAddDuration instead")]
        public static DateTime DateAddDuration(DateTime date, TimeSpan duration)
			=> GenFunctions.DateAddDuration(date, duration);

        /// <inheritdoc cref="GenFunctions.DateSubtractDuration"/>
		[Obsolete("Use GenFunctions.DateSubtractDuration instead")]
        public static DateTime DateSubtractDuration(DateTime date, TimeSpan duration)
			=> GenFunctions.DateSubtractDuration(date, duration);

        /// <inheritdoc cref="GenFunctions.DateAddYears"/>
		[Obsolete("Use GenFunctions.DateAddYears instead")]
        public static DateTime DateAddYears(DateTime date, decimal years)
			=> GenFunctions.DateAddYears(date, years);

        /// <inheritdoc cref="GenFunctions.DateAddMonths"/>
		[Obsolete("Use GenFunctions.DateAddMonths instead")]
        public static DateTime DateAddMonths(DateTime date, decimal months)
			=> GenFunctions.DateAddMonths(date, months);

        /// <inheritdoc cref="GenFunctions.DateAddDays"/>
		[Obsolete("Use GenFunctions.DateAddDays instead")]
        public static DateTime DateAddDays(DateTime date, decimal days)
			=> GenFunctions.DateAddDays(date, days);

        /// <inheritdoc cref="GenFunctions.DateAddHours"/>
		[Obsolete("Use GenFunctions.DateAddHours instead")]
        public static DateTime DateAddHours(DateTime date, decimal hours)
			=> GenFunctions.DateAddHours(date, hours);

        /// <inheritdoc cref="GenFunctions.DateAddMinutes"/>
		[Obsolete("Use GenFunctions.DateAddMinutes instead")]
        public static DateTime DateAddMinutes(DateTime date, decimal minutes)
			=> GenFunctions.DateAddMinutes(date, minutes);

        /// <inheritdoc cref="GenFunctions.DateAddSeconds"/>
		[Obsolete("Use GenFunctions.DateAddSeconds instead")]
        public static DateTime DateAddSeconds(DateTime date, decimal seconds)
			=> GenFunctions.DateAddSeconds(date, seconds);

        /// <inheritdoc cref="GenFunctions.DateGetYear"/>
		[Obsolete("Use GenFunctions.DateGetYear instead")]
        public static int DateGetYear(DateTime date)
			=> GenFunctions.DateGetYear(date);

        /// <inheritdoc cref="GenFunctions.DateGetMonth"/>
		[Obsolete("Use GenFunctions.DateGetMonth instead")]
        public static int DateGetMonth(DateTime date)
			=> GenFunctions.DateGetMonth(date);

        /// <inheritdoc cref="GenFunctions.DateGetDay"/>
		[Obsolete("Use GenFunctions.DateGetDay instead")]
        public static int DateGetDay(DateTime date)
			=> GenFunctions.DateGetDay(date);

        /// <inheritdoc cref="GenFunctions.DateGetHour"/>
		[Obsolete("Use GenFunctions.DateGetHour instead")]
        public static int DateGetHour(DateTime date)
			=> GenFunctions.DateGetHour(date);

        /// <inheritdoc cref="GenFunctions.DateGetMinute"/>
		[Obsolete("Use GenFunctions.DateGetMinute instead")]
        public static int DateGetMinute(DateTime date)
			=> GenFunctions.DateGetMinute(date);

        /// <inheritdoc cref="GenFunctions.DateGetSecond"/>
		[Obsolete("Use GenFunctions.DateGetSecond instead")]
        public static int DateGetSecond(DateTime date)
			=> GenFunctions.DateGetSecond(date);

        /// <inheritdoc cref="GenFunctions.DurationTotalDays"/>
		[Obsolete("Use GenFunctions.DurationTotalDays instead")]
        public static decimal DurationTotalDays(TimeSpan duration)
			=> GenFunctions.DurationTotalDays(duration);

        /// <inheritdoc cref="GenFunctions.DurationTotalHours"/>
		[Obsolete("Use GenFunctions.DurationTotalHours instead")]
        public static decimal DurationTotalHours(TimeSpan duration)
			=> GenFunctions.DurationTotalHours(duration);

        /// <inheritdoc cref="GenFunctions.DurationTotalMinutes"/>
		[Obsolete("Use GenFunctions.DurationTotalMinutes instead")]
        public static decimal DurationTotalMinutes(TimeSpan duration)
			=> GenFunctions.DurationTotalMinutes(duration);

        /// <inheritdoc cref="GenFunctions.DurationTotalSeconds"/>
		[Obsolete("Use GenFunctions.DurationTotalSeconds instead")]
        public static decimal DurationTotalSeconds(TimeSpan duration)
			=> GenFunctions.DurationTotalSeconds(duration);

        /*****/

        /// <inheritdoc cref="GenFunctions.DateFloorDay"/>
		[Obsolete("Use GenFunctions.DateFloorDay instead")]
        public static DateTime DateFloorDay(DateTime date)
			=> GenFunctions.DateFloorDay(date);

        /// <inheritdoc cref="GenFunctions.maxN"/>
		[Obsolete("Use GenFunctions.maxN instead")]
        public static decimal maxN(decimal obj1, decimal obj2)
			=> GenFunctions.maxN(obj1, obj2);

        /// <inheritdoc cref="GenFunctions.minN"/>
		[Obsolete("Use GenFunctions.minN instead")]
        public static decimal minN(decimal obj1, decimal obj2)
			=> GenFunctions.minN(obj1, obj2);

        /// <inheritdoc cref="GenFunctions.maxD"/>
		[Obsolete("Use GenFunctions.maxD instead")]
        public static DateTime maxD(DateTime obj1, DateTime obj2)
			=> GenFunctions.maxD(obj1, obj2);

        /// <inheritdoc cref="GenFunctions.minD"/>
		[Obsolete("Use GenFunctions.minD instead")]
        public static DateTime minD(DateTime obj1, DateTime obj2)
			=> GenFunctions.minD(obj1, obj2);

        /// <inheritdoc cref="GenFunctions.GetCurrentDay"/>
		[Obsolete("Use GenFunctions.GetCurrentDay instead")]
        public static DateTime GetCurrentDay()
			=> GenFunctions.GetCurrentDay();

        /// <inheritdoc cref="GenFunctions.GetCurrentMonth"/>
		[Obsolete("Use GenFunctions.GetCurrentMonth instead")]
        public static int GetCurrentMonth()
			=> GenFunctions.GetCurrentMonth();


        /// <inheritdoc cref="GenFunctions.GetCurrentYear"/>
		[Obsolete("Use GenFunctions.GetCurrentYear instead")]
        public static int GetCurrentYear()
			=> GenFunctions.GetCurrentYear();


        /// <inheritdoc cref="GenFunctions.LEFT"/>
		[Obsolete("Use GenFunctions.LEFT instead")]
        public static string LEFT(string arg, int nrElem)
			=> GenFunctions.LEFT(arg, nrElem);


        /// <inheritdoc cref="GenFunctions.RIGHT"/>
		[Obsolete("Use GenFunctions.RIGHT instead")]
        public static string RIGHT(string arg, int nrElem)
			=> GenFunctions.RIGHT(arg, nrElem);


        /// <inheritdoc cref="GenFunctions.SubString"/>
		[Obsolete("Use GenFunctions.SubString instead")]
        public static string SubString(string arg, int start, int nrElem)
			=> GenFunctions.SubString(arg, start, nrElem);


        /// <inheritdoc cref="GenFunctions.IndexOf"/>
		[Obsolete("Use GenFunctions.IndexOf instead")]
        public static int IndexOf(string str, string substr)
			=> GenFunctions.IndexOf(str, substr);


		[Obsolete("Use GenFunctions.Round instead")]
        public static decimal Round(decimal num, int digits)
			=> GenFunctions.Round(num, digits);


        /// <inheritdoc cref="GenFunctions.abs"/>
		[Obsolete("Use GenFunctions.abs instead")]
        public static decimal abs(decimal num)
			=> GenFunctions.abs(num);


        /// <inheritdoc cref="GenFunctions.CompareDates"/>
		[Obsolete("Use GenFunctions.CompareDates instead")]
        public static int CompareDates(DateTime date1, DateTime date2)
			=> GenFunctions.CompareDates(date1, date2);


        /// <inheritdoc cref="GenFunctions.LengthString"/>
		[Obsolete("Use GenFunctions.LengthString instead")]
        public static int LengthString(string a)
			=> GenFunctions.LengthString(a);


        /// <inheritdoc cref="GenFunctions.Incidenc"/>
		[Obsolete("Use GenFunctions.Incidenc instead")]
        public static decimal Incidenc(decimal unitValue, decimal amount, decimal pdiscount, int prec)
			=> GenFunctions.Incidenc(unitValue, amount, pdiscount, prec);


        /// <inheritdoc cref="GenFunctions.VATValue"/>
		[Obsolete("Use GenFunctions.VATValue instead")]
        public static decimal VATValue(decimal incidenc, decimal rate_iva, int vatprice, int prec)
			=> GenFunctions.VATValue(incidenc, rate_iva, vatprice, prec);


        /// <inheritdoc cref="GenFunctions.RoundQG"/>
        [Obsolete("Use GenFunctions.RoundQG instead")]
        public static decimal RoundQG(decimal x, int c)
            => GenFunctions.RoundQG(x, c);

        /// <summary>
        /// Função que devolve um bool se a string corresponder à expressão regular
        /// </summary>
        /// <param name="expression">Expressão a ser avaliada</param>
        /// <param name="pattern">Padrão em expressão regular</param>
        /// <returns>true se a expressão é validate, false caso contrário</returns>
        public static bool RegExpr(string expression, string pattern)
        {
            Regex re = new Regex(pattern);
            if (re.IsMatch(expression))
                return (true);
            else
                return (false);
        }

        /// <summary>
        /// Função que devolve um bool se a string corresponder ao wildcard
        /// </summary>
        /// <param name="expression">Expressão a ser avaliada</param>
        /// <param name="pattern">Padrão wildcard</param>
        /// <returns>true se a expressão é validate, false caso contrário</returns>
        public static bool RegExprWild(string expression, string pattern)
        {
            pattern = WildcardToRegExpr(pattern);
            return RegExpr(expression, pattern);
        }

        /// <summary>
        /// Função que converte wildcards to Regex
        /// </summary>
        /// <param name="pattern">Padrão wildcard</param>
        /// <returns>Padrão em expressão regular</returns>
        public static string WildcardToRegExpr(string pattern)
        {
            return "^" + Regex.Escape(pattern).
              Replace("\\*", ".*").
              Replace("\\?", ".") + "$";
        }

        /// <summary>
        /// Função to enviar um e-mail
        /// </summary>
        /// <param name="smtp">cliente smtp</param>
        /// <param name="de">endereço de source</param>
        /// <param name="para">endereço de target</param>
        /// <param name="assunto">subject</param>
        /// <param name="corpo">body do email</param>
        public void sendEmail(string smtp, string de, string to, string subject, string body,User user)
        {
            SmtpClient smtpCliente = new SmtpClient();
            System.Net.Mail.MailMessage mensagem = new System.Net.Mail.MailMessage();
            try
            {
                //host smtp
                smtpCliente.Host = smtp;

                //endereço source
                mensagem.From = new MailAddress(de);

                // endereço target
                mensagem.To.Add(to);
                mensagem.Subject = subject;

                //body da mensagem
                mensagem.Body = body ;

                // Send SMTP mail
                smtpCliente.Send(mensagem);

            }
            catch (Exception ex)
            {
                Log.Error("Erro a enviar email: " + ex.Message);
            }
        }


        /// <inheritdoc cref="Math.Floor"/>
        [Obsolete("Use Math.Floor instead")]
        public static decimal Floor(decimal number)
        {
            return Math.Floor(number);
        }

        /// <summary>
        /// Método que permite enviar um email
        /// </summary>
        /// <param name="origem">email de quem envia o email</param>
        /// <param name="assunto">subject do email</param>
        /// <param name="destino">email de quem recebe o email</param>
        /// <param name="corpoEmail">body do email</param>
        public static void sendEmail(string source,string subject,string target,string emailBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(Configuration.PP_Email);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(source, target);
                message.Subject = subject;
                message.Body = emailBody;
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Não foi possível enviar o email.", "GlobalFunctions.enviarEmail", "Couldn't send the email: " + ex.Message, ex);
            }
        }


        /// <inheritdoc cref="GenFunctions.DateDiffPart"/>
        [Obsolete("Use GenFunctions.DateDiffPart instead")]
        public static decimal Diferenca_entre_Datas(DateTime dt_start, DateTime dt_end, string scale)
            => GenFunctions.DateDiffPart(dt_start, dt_end, scale);

        /// <summary>
        /// Created by [SF] at [2017.03.23]
        /// Query to retirar todos os dados necessÃ¡rios to enviar pelo querystring
        /// </summary>
        /// <param name="id">parametro do id da table associada ao docums</param>
        /// <returns>Dados do documento</returns>
        public DataMatrix ReturnValueSignPdf(string id, string table, string Qfield, string keyPK)
        {
            DataMatrix result = null;
            try
            {
                SelectQuery query = new SelectQuery()
                    .Select(CSGenioAdocums.FldNome)
                    .Select(CSGenioAdocums.FldDocument)
                    .Select(CSGenioAdocums.FldDocpath)
                    .Select(CSGenioAdocums.FldCoddocums)
                    .Select(CSGenioAdocums.FldVersao)
                    .From(table)
                    .Join("docums", "DOCUMS")
                    .On(CriteriaSet.And().Equal(table, Qfield+"fk", "DOCUMS", "documid"))
                    .Where(CriteriaSet.And().Equal(table, keyPK, id)
                    .Equal(table, "zzstate", 0)
                    .NotEqual("DOCUMS", "VERSAO", "CHECKOUT"))
                    .OrderBy(CSGenioAdocums.FldDatacria, Quidgest.Persistence.GenericQuery.SortOrder.Descending).Page(1);

                result = sp.Execute(query);
                return result;
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// Created by [SF] at [2017.03.16]
        /// Função to comprimir uma string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Devolve a sigla iso da moeda por omissão definida
        /// </summary>
        /// <returns>A sigla iso da moeda por omissão definida</returns>
        public string GetDefaultCurrency()
        {
            return "EUR";
        }

        /// <summary>
        /// Verifica se uma assinatura é válida
        /// </summary>
        /// <param name="nomePrograma">Name do programa (prefixo das tables)</param>
        /// <param name="nomeTabela">O name da table</param>
        /// <param name="nomeCodigoInterno">O name do Qfield do codigo interno</param>
        /// <param name="codigoInterno">O Código Interno utilizado</param>
        /// <returns>Devolve um array com o codigo do hash usado no momento e o Qvalue dos fields to assinar</returns>
        public string validateSignature(string QtableName, string internalCode)
        {
            string nomeTabelaBD = Configuration.Program + QtableName.ToUpper();
            string HashRegis = "";

            Area a = Area.createArea(QtableName, user, user.CurrentModule);
            sp.getRecord(a, internalCode);
            string codhashcd = a.returnValueField(QtableName + ".codhashcd") as string;
            if (string.IsNullOrEmpty(a.QPrimaryKey) || string.IsNullOrEmpty(codhashcd))
                throw new BusinessException("Erro na validação da assinatura.", "GlobalFunctions.validarAssinatura", "The record with code " + internalCode + " wasn't found.");
            if (!a.AccessRightsToConsult())
                throw new BusinessException("Erro na validação da assinatura.", "GlobalFunctions.validarAssinatura", "User has no read access rights to this record.");

            string hashTablename = "hashcd";
            SelectQuery query = new SelectQuery()
                .Select(hashTablename, "campos")
                .From(hashTablename)
                .Where(CriteriaSet.And().Equal(hashTablename, "codhashcd", codhashcd));
            string camposAssinatura = sp.executeScalar(query) as string ?? "";
            string[] camposArray = camposAssinatura.Split(',');

            if (!(camposArray.Length > 0))
                throw new BusinessException("Não existem campos para assinar.", "GlobalFunctions.validarAssinatura", "The record with code " + internalCode + " has no fields to sign.");
            else
            {
                Dictionary<string, framework.Field> fields = Area.GetInfoArea(QtableName).DBFields;
                //To todos os fields definidos no array
                foreach (string fieldName in camposArray)
                {
                    //framework.Field Qfield;
                    fields.TryGetValue(fieldName, out var Qfield);
                    //Se for uma data é preciso ter em atençao a formatting
                    if (Qfield.FieldType.Equals(CSGenio.framework.FieldType.DATE))
                    {
                        DateTime d = (DateTime)a.returnValueField(QtableName + "." + fieldName);
                        if(d == DateTime.MinValue)
                            HashRegis += "__/__/____";
                        else
                            HashRegis += d.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        HashRegis += a.returnValueField(QtableName + "." + fieldName).ToString();
                    }
                }

                string assinatura = "";
                try
                {
                    String pTxt = HashRegis.Trim();
                    byte[] plainText = Encoding.Unicode.GetBytes(pTxt);

                    assinatura = System.Text.Encoding.ASCII.GetString((byte[])a.returnValueField(QtableName + ".hashcode"));
                    byte[] encodedMessage = Convert.FromBase64String(assinatura);
                    ContentInfo contentInfo = new ContentInfo(plainText);
                    SignedCms signedCms = new SignedCms(contentInfo, true);
                    signedCms.Decode(encodedMessage);
                    signedCms.CheckSignature(true);
                    //Poderia usar-se o Qcertificate embutido na assinatura to algum proposito...
                    //X509Certificate2 certificate = signedCms.Certificates[0];
                }
                catch (System.Security.Cryptography.CryptographicException e)
                {
                    // So efecuta um update se houver alteração
                    if (!string.IsNullOrEmpty(assinatura))
                    {
                        UpdateQuery update = new UpdateQuery()
                            .Update(nomeTabelaBD)
                            .Set("hashcode", null)
                            .Where(CriteriaSet.And().Equal(nomeTabelaBD, a.PrimaryKeyName, internalCode));
                        sp.openConnection();
                        sp.Execute(update);
                        sp.closeConnection();
                    }

                    //lanca erro
                    throw new BusinessException("Erro na validação da assinatura.", "GlobalFunctions.validarAssinatura", "Error validating signature: " + e.Message, e);
                }
            }
            return "1";
        }

        public string[] returnFieldsSignature(IArea area, string internalCode)
        {
            return returnFieldsSignature(area.QSystem, area.TableName, internalCode);
        }

        /// <summary>
        /// Retorna informação to se poder assinar um documento
        /// </summary>
        /// <param name="nomePrograma">Name do programa (prefixo das tables)</param>
        /// <param name="nomeTabela">O name da table</param>
        /// <param name="nomeCodigoInterno">O name do Qfield do codigo interno</param>
        /// <param name="codigoInterno">O Código Interno utilizado</param>
        /// <returns>Devolve um array com o codigo do hash usado no momento e o Qvalue dos fields to assinar</returns>
        //[Obsolete("User 'string devolverCamposAssinatura(IArea area, string codigoInterno)' instead")]
        public string[] returnFieldsSignature(string QtableName, string internalCode)
        {
            return returnFieldsSignature(null, QtableName, internalCode);
        }

        private string[] returnFieldsSignature(string schema, string QtableName, string internalCode)
        {
            String nomeTabelaBD = Configuration.Program + QtableName.ToUpper();
            String nomeChavePrimaria = Area.GetInfoArea(QtableName).PrimaryKeyName;

            String HashRegis = "";

            // Que fields usar ? //////////////////////////////////////////////////////////////////////
            string tableName = "hashcd";
            SelectQuery qsCampos = new SelectQuery()
                .Select("hashcd", "codhashcd")
                .Select("hashcd", "campos")
                .From(tableName, "hashcd")
                .Where(CriteriaSet.And()
                    .Equal(SqlFunctions.Upper(new ColumnReference("hashcd", "tabela")), nomeTabelaBD.ToUpper()))
                .OrderBy("hashcd", "datacria", Quidgest.Persistence.GenericQuery.SortOrder.Descending)
                .PageSize(1);

            //Vai buscar os fields
            DataMatrix registoHash = sp.Execute(qsCampos);
            String codhashcd = registoHash.GetString(0, "hashcd.codhashcd");
            String fields = registoHash.GetString(0, "hashcd.campos");

            // Se não forem definidos fields to a table retorna uma excepção
            if (!(fields.Length > 0))
            {
                throw new BusinessException("Não foram definidos campos para a assinatura.", "GlobalFunctions.devolverCamposAssinatura", "The table " + nomeTabelaBD + " has no fields.");
            }

            // Quais sao os Qvalues desses fields ? ///////////////////////////////////////////////////
            //Constroi uma query to ir buscar os Qvalues desses fields
            string[] camposArray = fields.Split(',');

            SelectQuery qs = new SelectQuery();
            foreach (string Qfield in camposArray)
            {
                qs.Select(nomeTabelaBD, Qfield);
            }
            qs.From(schema, nomeTabelaBD, nomeTabelaBD);
            qs.Where(CriteriaSet.And()
                .Equal(nomeTabelaBD, nomeChavePrimaria, internalCode));

            //Vai buscar os Qvalues
            DataMatrix fieldsvalues = sp.Execute(qs);

            /* Esta verificação deve ser feita do lado do cliente
            //Por vezes usa-se o @ no Qfield opercria outra vezes não
            String userAt = "@" + this.user.Name;
            String user = this.user.Name;

            String criadorDocumento = fieldsvalues["opercria"].ToString();
            if (!(user.ToUpper().Equals(criadorDocumento.ToUpper()) || userAt.ToUpper().Equals(criadorDocumento.ToUpper()))){
                throw new BusinessException("Não pode assinar um documento que não foi criado por si.", "GlobalFunctions.devolverCamposAssinatura", "The user " + user + " is trying to sign a document created by " + criadorDocumento + ".");
            }
            */

            Dictionary<string, framework.Field> camps = Area.GetInfoArea(QtableName).DBFields;
            //To todos os fields definidos no array
            foreach (string fieldName in camposArray)
            {
                framework.Field Qfield;
                camps.TryGetValue(fieldName, out Qfield);
                //Se for uma data é preciso ter em atençao a formatting
                if (Qfield.FieldType.Equals(CSGenio.framework.FieldType.DATE) || Qfield.FieldType.Equals(CSGenio.framework.FieldType.DATETIME))
                {
                    String l = fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString();
                    if (l.Equals(""))
                        HashRegis += "__/__/____";
                    else
                        HashRegis += l.Replace("-", "/").Substring(0, 10);
                }
                else
                {
                        if (Qfield.FieldType.GetFormatting() == FieldFormatting.GUID && fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString().Length != 0)
                            HashRegis += fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString().ToUpper();
                        else
                            HashRegis += fieldsvalues.GetDirect(0, nomeTabelaBD + "." + fieldName).ToString();
                }
            }

            // Create um "array" com o codigo da hash que foi utilizada e os Qvalues
            string[] result = new string[2];
            result[0] = codhashcd;
            result[1] = HashRegis.Trim();
            return result;
        }

        /// <summary>
        /// Verifica e escreve a assinatura na base de dados
        /// </summary>
        /// <param name="nomePrograma">Name do programa (prefixo das tables)</param>
        /// <param name="nomeTabela">O name da table</param>
        /// <param name="nomeCodigoInterno">O name do Qfield do codigo interno</param>
        /// <param name="codigoInterno">O codigo interno utilizado</param>
        /// <param name="codhashcd">O codigo da hash utilizada</param>
        /// <param name="texto">O text que foi assinado (to se poder verificar com a assinatura)</param>
        /// <param name="assinatura">A assinatura</param>
        /// <returns>Devolve 1 se a assinatura for bem sucedida ou 0 no caso contrário.</returns>
        public string writeSignature(string QtableName, string internalCode, string signatureInfo)
        {
            // O name da table na base de dados é o name do module concatenado com a table
            string nomeTabelaBD = Configuration.Program + QtableName.ToUpper();
            string nomeChavePrimaria = Area.GetInfoArea(QtableName).PrimaryKeyName;

            string[] assinaturaCampos = signatureInfo.Split(';');
            string codhashcd = assinaturaCampos[0];
            string text = assinaturaCampos[1];
            string assinatura = assinaturaCampos[2];

            try
            {
                //Verificar a assinatura antes de escrever
                byte[] plainText = Encoding.Unicode.GetBytes(text);
                byte[] encodedMessage = Convert.FromBase64String(assinatura);
                ContentInfo contentInfo = new ContentInfo(plainText);
                SignedCms signedCms = new SignedCms(contentInfo, true);
                signedCms.Decode(encodedMessage);
                signedCms.CheckSignature(true);

                UpdateQuery update = new UpdateQuery()
                    .Update(nomeTabelaBD)
                    .Set("hashcode", assinatura)
                    .Set("codhashcd", codhashcd)
                    .Where(CriteriaSet.And().Equal(nomeTabelaBD, nomeChavePrimaria, internalCode));

                sp.openConnection();
// USE /[MANUAL FOR VALIDAASSINA]/
                //int linhas = sp.executeNonQuery(query);
                int linhas = sp.Execute(update);
                if (linhas != 1)
                    throw new BusinessException("Ocorreu um erro ao assinar.", "GlobalFunctions.escreverAssinatura", "There were " + linhas + " records updated.");
// USE /[MANUAL FOR ONASSINA]/

                sp.closeConnection();
                return "1";
            }
            catch (System.Security.Cryptography.CryptographicException e)
            {
                throw new BusinessException("Assinatura invalida, o documento não foi assinado.", "GlobalFunctions.escreverAssinatura", "Error saving the signature: " + e.Message, e);
            }
        }

        /// <summary>
        /// Regista o number de serie do Qcertificate na db
        /// </summary>
        /// <param name="numerodeserie"></param>
        /// <returns></returns>
        public int registerCertificateSerialNumber(String serialNumber)
        {
            UpdateQuery queryUp = new UpdateQuery()
                .Update("USERLOGIN")
                .Set("certsn", serialNumber)
                .Where(CriteriaSet.And()
                    .Equal("USERLOGIN", "codpsw", user.Codpsw));

            int n = sp.Execute(queryUp);

            return n;
        }

        /// <summary>
        /// Created by [FA] at [2012.10.29]
        /// Updated by [CJP] at [2014.10.27]
        /// Gera o documento através de um template
        /// </summary>
        /// <param name="res">Hashtable com o historial</param>
        /// <param name="area">Name da Área dos templates</param>
        /// <param name="campo">Name do Qfield que contém a key primária da table dos templates</param>
        /// <param name="valor">Value da key primária do template to geração</param>
        /// <returns>FileInfo do documento gerado</returns>
        /// <remarks>Retorna null se não existirem queries manuais</remarks>
        public FileInfo CreateDocum(Hashtable kV, string area, string Qfield, string Qvalue)
        {
#if NETFRAMEWORK
            Area areaObj = Area.createArea(area, user, user.CurrentModule);
            string[] fields = new string[] { area + ".path", area + ".outname", area + ".tpdoc" };
            areaObj.insertNamesFields(fields);
            areaObj.selectOne(CriteriaSet.And().Equal(area, Qfield, Qvalue), null, "", sp);
            sp.closeConnection();

            String path = Conversion.internalString2InternalValidString(areaObj.returnValueField(area + ".path"));
            String outname = Conversion.internalString2InternalValidString(areaObj.returnValueField(area + ".outname"));
            String tpdoc = Conversion.internalString2InternalValidString(areaObj.returnValueField(area + ".tpdoc"));

            var engine = new GenioServer.business.DocumentEngine(this.sp, this.user, kV);
            String output = engine.GenerateDocument(path, outname, tpdoc);
            FileInfo info = new FileInfo(output);

            return info;
#else
            throw new NotImplementedException("DocumentEngine is not available for .Net Core");
#endif
        }

        /// <summary>
        /// Created by [CJP] at [2014.10.27]
        /// Gera o documento através de um template
        /// </summary>
        /// <param name="res">string com o historial</param>
        /// <param name="area">Name da Área dos templates</param>
        /// <param name="campo">Name do Qfield que contém a key primária da table dos templates</param>
        /// <param name="valor">Value da key primária do template to geração</param>
        /// <returns>string com o Qresult do documento gerado</returns>
        /// <remarks>Implementação QWeb</remarks>
        public string CreateDocumQweb(string res, string area, string Qfield, string Qvalue)
        {
            String[] r = res.Split('|');
            Hashtable kV = new Hashtable();
            for (int i = 0; i < r.Length - 1; i += 2)
                kV[r[i]] = r[i + 1];

            String codlig = kV["key"].ToString();

            FileInfo documentoGerado = CreateDocum(kV, area, Qfield, Qvalue);

            if (documentoGerado != null)
            {
                Resource recfich = new ResourceFile(documentoGerado.Name, documentoGerado.FullName);
                string recSer = QResources.CreateTicketEncryptedBase64(user.Name, user.Location, recfich);
                string linkRec = System.Web.HttpUtility.UrlEncode(recSer);
                //to apanhar os casos em que chega a null
                return linkRec + "[" + "Documento criado com sucesso!" + "[" + documentoGerado.Name;
            }
            else
                return "";
        }


        /// <summary>
        /// Created by [ARR] at [2015-05-25]
        /// Função que identifica o perfil do user
        /// </summary>
        /// <returns></returns>
        public List<object> GetUserProfile()
        {
            List<object> res = new List<object>();
            string name = user.Name;
            ResourceQuery foto = null;

            List<Profile> profiles = Profile.GetProfiles();

            //apenas construimos a query se tivermos perfis definidos
            if (profiles.Count > 0)
            {
                //vamos juntar tudo numa so query
                //porque so vamos retornar dados se existir apenas um perfil válido
                SelectQuery sqlProfile = null;
                foreach (Profile p in profiles)
                {
                    SelectQuery sql = new SelectQuery();

                    //adicionar a key primária da table profile
                    sql.Select(p.Key, "chave");

                    //indicar a table do profile numa das colunas to que seja genérico a leitura final
                    sql.Select(new SqlValue(p.ProfileArea.Alias), "tabela");
                    //definir a source dos dados
                    sql.From(p.ProfileArea);

                    foreach(Relation rel in p.Relations)
                    {
                        sql.Join(rel.SourceTable, rel.AliasSourceTab)
                            .On(CriteriaSet.And().Equal(rel.AliasSourceTab, rel.SourceRelField, rel.AliasTargetTab, rel.TargetRelField));
                    }

                    //aplicar o limite do user
                    sql.Where(CriteriaSet.And().Equal(CSGenioApsw.FldCodpsw, User.Codpsw));

                    //se a query principal ainda não tiver definida então passa a ser esta
                    //se já tiver definida então adicionamos um union
                    if (sqlProfile == null)
                        sqlProfile = sql;
                    else
                        sqlProfile.Union(sql, false); //com false to não dar os registos repetidos
                }

                DataMatrix mat = sp.Execute(sqlProfile);
                //só vamos buscar os Qvalues se tivermos apenas um registo de pessoa associado ao user
                if (mat.NumRows == 1)
                {
                    string cod = mat.GetKey(0, "chave");
                    string table = mat.GetString(0, "tabela");
                    Area profileArea = Area.createArea(table, User, User.CurrentModule);

                    Profile profile = profiles.Find(x => x.ProfileArea.Alias == table);

                    //se encontrarmos o profile que identificado na query
                    //e conseguirmos criar uma area tendo como referencia a area base do profile
                    //então vamos buscar o registo
                    if (profile != null && profileArea != null )
                    {
                        //vamos buscar os fields do profile
                        if (sp.getRecord(profileArea, cod, new string[]{ profile.Name.Field, profile.Photo.Field }))
                        {
                            name = DBConversion.ToString(profileArea.returnValueField(profile.Name.FullName));
                            name = GetShortName(name, 30); //reduzir o size do name

                            //apenas cria um resource se existir mesmo uma foto na BD to o registo indicado
                            Byte[] fotoByte = DBConversion.ToBinary(profileArea.returnValueField(profile.Photo.FullName));
                            if (fotoByte.Length != 0)
                                foto = new ResourceQuery(profileArea, profile.Photo.Field, cod);
                        }
                    }
                }
            }

            //o name adiciona sempre seja o do registo ou o do username do login
            res.Add(name);
            //mesmo que a foto vá a null a comunicação consegue traduzir
            res.Add(foto);
            return res;
        }

        /// <summary>
        /// Created by [ARR] at [2015-05-25]
        /// Função que reduz o size do name tendo em conta um maximo indicado
        /// </summary>
        /// <returns></returns>
        public string GetShortName(string name, int size)
        {
            name = name.Trim();

            if (string.IsNullOrEmpty(name))
                return "";

            //vamos fazer split do name pelos espaços to depois avaliarmos
            string[] nomes = name.Split(' ');

            //se o name estiver dentro do size pertendido então retornamos
            //se apenas tiver um name também retornamos independentemente do size máximo (o interface irá cortar o excesso)
            if (name.Length <= size || nomes.Length == 1)
                return name;
            else if (nomes.Length > 1) //apenas se tiver mais do que um name
            {
                string primeiro = nomes[0];
                string ultimo = nomes[nomes.Length - 1];

                //se o primeiro mais o ultimo mais o espaço entre eles
                //couber no size definido então retornamos o primeiro e o ultimo name
                if ((primeiro.Length + ultimo.Length + 1) <= size)
                {
                    //retorna o primeiro e o ultimo
                    return string.Join(" ", new string[] { primeiro, ultimo });
                }
                else if ((primeiro.Length + 3) <= size) //primeiro mais espaço mais redução do ultimo name (ex: Pedro S.)
                {
                    //vamos reduzir o ultimo name à 1ª letra do name mais um ponto (ex: Santos = S.)
                    //retorna o primeiro e o ultimo reduzido
                    return string.Join(" ", new string[] { primeiro, ultimo[0] + "." });
                }
                else
                    return primeiro; //retorna apenas o primeiro independentemente do size máximo (o interface irá cortar o excesso)
            }

            return "";
        }

        /// <summary>
        /// Hidrate the list of scripts (Adding reindex functions delegates).
        /// This funtions must be called before "upgradeSchema()"
        /// </summary>
        /// <param name="scripts">List of scripts</param>
        /// <param name="versionReader">A reader for the database version</param>
        /// <param name="zero">Full reindexation</param>
        /// <returns></returns>
        public List<ExecuteQueryCore.RdxScript> HidrateScripts(List<ExecuteQueryCore.RdxScript> scripts, IVersionReader versionReader, bool zero = false)
        {
            int upgrindx;
            decimal dbVersion;

            try
            {
                upgrindx = versionReader.GetDbUpgradeVersion();
                dbVersion = versionReader.GetDbVersion();
            }
            catch (Exception)
            {
                upgrindx = Configuration.VersionUpgrIndxGen;
                dbVersion = 0;
            }


            /*
            * In the database there is a field that stores the version of the last upgrade routine that was ran, its called upgrindx.
            * Despite looking like it on the surface, the script do not run in a sequencial order (like version 1, 2, 3 ...), instead
            * they are sorted from lowest version to highest AND FROM BEFORE THE SCHEMA TO AFTER. This order is does NOT change, unless
            * the user edits something ofc.
            *
            * Example:
            *
            * Scritps: 1 - Before schema | 2 - After schema | 3 - After schema | 4 - Before schema
            *
            * Instead of running them sequencially -> 1, 2, 3, 4
            * We sort them like explained above -> 1, 4, 2, 3
            *
            * In this example, if every script ran fine, the number that is stored on the database will 3, since its the version that was
            * ran last.
            *
            * Here what we do is we fetch the last ran version and we clear all the scripts that were ran beforehand
            */
            ReindexFunctions rdxfunc = new ReindexFunctions(sp, user, zero);
            for (int i = scripts.Count - 1; i >= 0; i--)
            {
                //Check if script needs to be run by looking at the specified Min and Max DB versions
                if ((!String.IsNullOrEmpty(scripts[i].MinDbVersion) && Convert.ToInt32(scripts[i].MinDbVersion) != 0 && Convert.ToInt32(scripts[i].MinDbVersion) > dbVersion)
                        || (!String.IsNullOrEmpty(scripts[i].MaxDbVersion) && Convert.ToInt32(scripts[i].MaxDbVersion) != 0 && Convert.ToInt32(scripts[i].MaxDbVersion) < dbVersion))
                {
                    scripts.RemoveAt(i);
                    continue;
                }

                if (scripts[i].Script == "UpgradeClient" || scripts[i].Script == "UpgradeClient.sql")
                    //Since the scripts are not sequencially executed, we compare their indexes and not the versions themselfs to know if
                    //they need to be removed or not
                    if (i < scripts.IndexOf(scripts.Find(ord => ord.Version == upgrindx)))
                    {
                        scripts.RemoveAt(i);
                        continue;
                    }

                if (scripts[i].Type != null && scripts[i].Type.ToUpper() == "CS")
                {
                    if (scripts[i].Version == 0)
                        scripts[i].Execute = (Action<System.Threading.CancellationToken>)Delegate.CreateDelegate(typeof(Action<System.Threading.CancellationToken>), rdxfunc, rdxfunc.GetType().GetMethod(scripts[i].Script));
                    else
                        scripts[i].Execute = (Action<System.Threading.CancellationToken>)Delegate.CreateDelegate(typeof(Action<System.Threading.CancellationToken>), rdxfunc, rdxfunc.GetType().GetMethod(scripts[i].Script + scripts[i].Version.ToString()));
                }
            }

            //We remove the last ran index last because if we do it before the others, we wont know what index to compare the records to
            int idx = scripts.IndexOf(scripts.Find(ord => ord.Version == upgrindx && (ord.Script == "UpgradeClient" || ord.Script == "UpgradeClient.sql")));
            if (idx != -1)
                scripts.RemoveAt(idx);

            return scripts;
        }




        /// <summary>
        /// Access to value of the certain EPH.
        /// </summary>
        /// <param name="usr">Current user</param>
        /// <param name="ephID">EPH Identifier</param>
        /// <returns>EPH (first) Value</returns>
        public static string GetEph(User user, string ephID)
        {
            var values = user.GetEph(user.CurrentModule, ephID);
            if (values != null && values.Length > 0)
                return values[0];

            return null;
        }

        /// <summary>
        /// Check if a user as access to a certain role
        /// </summary>
        /// <param name="usr">User we want to check</param>
        /// <param name="roleId">Role Identifier</param>
        /// <returns>true if the user has access to a certain role</returns>
        public static bool HasRole(User user, string roleId)
        {
            var role = Role.GetRole(roleId);
            return user.VerifyAccess(role);
        }

        /// <summary>
        /// Checks if a given feature is active in this application for this client.
        /// </summary>
        /// <param name="feature">The feature name</param>
        /// <returns>True if the feature is active</returns>
        public static CSGenio.business.Logical IsFeatureActive(string feature)
        {
            switch (feature)
            {
                default :
                    return 0;
            }
        }

        /// <summary>
        /// Converts the given Genio language id to it's correspondent platform language id
        /// </summary>
        /// <param name="languageId">The language id to convert</param>
        /// <returns>A string with the language id, or null if the specified id doesn't exist</returns>
        public static string GetClientLang(string languageId)
        {
            switch (languageId)
            {
                case "eng":
                    return "ENUS";
            }

            return null;
        }

        /// <summary>
        /// My application theme variables
        /// </summary>
        private static readonly Dictionary<string, string> MYAPP_THEME_VARIABLES = new Dictionary<string, string>()
        {
            { "$footer-bg", "transparent" },
            { "$menu-sidebar-width", "16rem" },
            { "$menu-behaviour", "partial_collapse" },
            { "$compactheader", "false" },
            { "$save-icon", "floppy-disk" },
            { "$compactstyle", "true" },
            { "$border-radius", "0.25rem" },
            { "$table-striped", "false" },
            { "$table-head-inverse", "false" },
            { "$table-vertical-border", "true" },
            { "$enable-table-wrap", "true" },
            { "$font-size-base", "0.7rem" },
            { "$font-family-sans-serif", "\"Lato\", Roboto, \"Helvetica Neue\", Arial, sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\", \"Noto Color Emoji\"" },
            { "$font-headings", "$font-family-sans-serif" },
            { "$headings-text-transform", "uppercase" },
            { "$primary", "#D69E98" },
            { "$secondary", "#C9B793" },
            { "$highlight", "#ff8241" },
            { "$action-focus-width", "2px" },
            { "$action-focus-style", "solid" },
            { "$action-focus-color", "#201060" },
            { "$input-focus-color", "rgba(0, 169, 206, 0.35)" },
            { "$button-focus-color", "rgba(238, 96, 2, 0.5)" },
            { "$body-bg", "$white" },
            { "$body-color", "#202428" },
            { "$input-btn-padding-y", "0.26rem" },
            { "$input-btn-padding-x", "0.25rem" },
            { "$enable-switch-single-label", "false" },
            { "$wizard-steps", "circle" },
            { "$wizard-content", "standard" },
            { "$btn-align-right", "false" },
            { "$menu-multi-level", "true" },
            { "$primary-light", "#F3904A" },
            { "$primary-dark", "#A34C29" },
            { "$success", "#28a745" },
            { "$danger", "#b71c1c" },
            { "$light", "#EAEBEC" },
            { "$red", "#b71c1c" },
            { "$info", "#17a2b8" },
            { "$warning", "#ffa900" },
            { "$gray", "#7C858D" },
            { "$gray-light", "#C4C5CA" },
            { "$gray-dark", "#40474F" },
            { "$navbar-font-size", "0.8rem" },
            { "$navbar-font-weight", "400" },
            { "$tab-style", "line" },
            { "$group-border-top", "none" },
            { "$group-border-bottom", "none" },
            { "$input-bg", "transparent" },
            { "$input-bg-readonly", "rgb($neutral-light-rgb / 0.25)" },
            { "$hover-item", "rgb($primary-light-rgb / 0.5)" },
            { "$header-bg", "$background" },
            { "$header-color", "$on-background" },
            { "$navbar-bg", "$primary" },
            { "$navbar-color", "$on-primary" },
            { "$menu-multi-level-border", "false" }
        };

        /// <summary>
        /// Access to value of the certain Variable in current app.
        /// </summary>
        /// <param name="variable">variable name</param>
        /// <returns>theme variable Value</returns>
        public static string GetAppThemeVariable(string variable)
        {
            return "";
        }

        /// <summary>
        /// Access to value of the certain Variable from a specific app.
        /// </summary>
        /// <param name="appID">apps acronym</param>
        /// <param name="variable">variable name</param>
        /// <returns>theme variable Value</returns>
        public static string GetThemeVariable(string appID, string variable)
        {
            if ("MYAPP" == appID)
                return MYAPP_THEME_VARIABLES[variable];
            return "";
        }

        /// <summary>
        /// Splits the provided geometry collection into a list of geometries/polygons
        /// </summary>
        /// <param name="geometry">The geometry</param>
        /// <returns>A collection of geometries/polygons</returns>
        public static ICollection<CSGenio.framework.Geography.GeographicShape> SplitGeometry(CSGenio.framework.Geography.GeographicData geometry)
        {
            return CSGenio.framework.Geography.GeographicData.SplitGeometry(geometry);
        }

        /// <summary>
        /// Transforms a list of geometries/polygons into a geometry collection
        /// </summary>
        /// <param name="geometries">The list</param>
        /// <returns>A geometry collection with all the geometries in the list</returns>
        public static CSGenio.framework.Geography.GeographicData JoinGeometries(ICollection<CSGenio.framework.Geography.GeographicShape> geometries)
        {
            return CSGenio.framework.Geography.GeographicData.JoinGeometries(geometries);
        }

        /// <summary>
        /// Converts a given string to a QR code representation
        /// </summary>
        /// <param name="text">The string to convert</param>
        /// <returns>A byte array representing the result of the convertion</returns>
        public static byte[] StringToQRcode(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            // Error correction: Level Q (Quartile) - 25% of data bytes can be restored.
            QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
            System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Returns the access level associated with the provided role.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public static decimal GetLevelFromRole(decimal level, string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return level;

            Role role = Role.GetRole(roleId);

            if (role != null && (role.Type == RoleType.LEVEL || role.Type == RoleType.SYSTEM))
                return role.GetLevelInt();
            return 0;
        }

        /// <summary>
        /// Import the users from a domain active directory is not already in PSW table
        /// </summary>
        /// <param name="domain">Domain from AD</param>
        /// <returns>Status of the import</returns>
        public StatusMessage ImportUsersFromAD(string domain)
        {
#if !NETFRAMEWORK
            if (!OperatingSystem.IsWindows())
                return StatusMessage.Error("Functionality only available in Windows OS");
#endif
            if (!CSGenio.framework.Configuration.Security.IdentityProviders.Exists(p => p.Type.Equals("GenioServer.security.LdapIdentityProvider")))
                return StatusMessage.Error(Translations.Get("O tipo de login não permite a importação a partir de Active directory.", user.Language));

            int usersCreated = 0;
            try
            {
                sp.openConnection();

                List<string> userList = new List<string>();

                using (var context = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, domain))
                {
                    using (var searcher = new System.DirectoryServices.AccountManagement.PrincipalSearcher(new System.DirectoryServices.AccountManagement.UserPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            string usercontrol = de.Properties["userAccountControl"].Value.ToString();

                            //Disable acounts code 514 = NORMAL_ACCOUNT (512) + ACCOUNTDISABLE (2)
                            if (!usercontrol.Equals("514"))
                                userList.Add(de.Properties["samAccountName"].Value.ToString());
                        }
                    }
                }

                //Checks for each user if is not already in the database
                foreach (string usr in userList)
                {
                    SelectQuery selQuery = new SelectQuery()
                        .Select(CSGenioApsw.FldCodpsw)
                        .From(Area.AreaPSW)
                        .Where(CriteriaSet.And()
                            .Equal(CSGenioApsw.FldNome, usr)
                            .Equal(CSGenioApsw.FldZzstate, 0)
                        )
                        .PageSize(1);

                    var userExist = sp.Execute(selQuery);

                    //If the user doesn't existe , create
                    if (userExist.NumRows == 0)
                    {
                        CSGenioApsw userPsw = new CSGenioApsw(User)
                        {
                            ValNome = usr
                        };
                        userPsw.insert(sp);
                        usersCreated++;
                    }
                }
                sp.closeConnection();
            }
            catch (Exception ex)
            {
                return StatusMessage.Error(ex.Message);
            }
            return StatusMessage.OK(string.Format(Translations.Get("Importação concluída com sucesso. Foram importados {0} utilizadores", user.Language), usersCreated));
        }

        private static string GetApiProperty(string property, bool isRequired = true)
        {
            bool exists = Configuration.ExistsProperty(property);

            if (!exists)
            {
                if (isRequired)
                    CSGenio.framework.Log.Error(string.Format("Error while trying to obtain external service token: property \"{0}\" not found!", property));

                return null;
            }

            return Configuration.GetProperty(property) ?? "";
        }

        /// <summary>
        /// Tries to obtain an access token for an external service, using the data in the specified configuration
        ///
        /// The configuration must have the following properties:
        /// - [configId]_BASE_URL: The base url for the service
        /// - [configId]_TOKEN_PATH: The path to get the token, relative to the base url
        /// - [configId]_USERNAME: The username to access the service
        /// - [configId]_PASSWORD: The password to access the service
        /// - [configId]_SERVICE_TYPE: The type of the service (ex: ArcGis)
        ///
        /// Additional properties can eventually be added, when necessary, for specific implementations
        /// </summary>
        /// <param name="configId">The id (prefix) of the configuration (in WebAdmin)</param>
        /// <returns>A token to access the external service, or null if some error prevents it's obtainment</returns>
        public static string GetExternalServiceToken(string configId)
        {
            if (string.IsNullOrWhiteSpace(configId))
            {
                CSGenio.framework.Log.Error("Error while trying to obtain external service token: the config id can't be empty!");
                return null;
            }

            string serviceType = GetApiProperty(configId + "_SERVICE_TYPE");

            if (string.IsNullOrWhiteSpace(serviceType))
                return null;

            string baseUrl = GetApiProperty(configId + "_BASE_URL"),
                tokenPath = GetApiProperty(configId + "_TOKEN_PATH");

            // Takes care of trailing slashes in the url.
            if (baseUrl.EndsWith(@"/"))
                baseUrl = baseUrl.Remove(baseUrl.Length - 1);
            if (!tokenPath.StartsWith(@"/"))
                tokenPath = "/" + tokenPath;

            var parameters = new Dictionary<string, string>()
            {
                { "baseUrl", baseUrl },
                { "tokenPath", tokenPath },
                { "username", GetApiProperty(configId + "_USERNAME") },
                { "password", GetApiProperty(configId + "_PASSWORD") }
            };

            // All required parameters must be filled.
            if (parameters.Values.Any(v => string.IsNullOrWhiteSpace(v)))
                return null;

            // Add optional parameters.
            parameters["expiration"] = GetApiProperty(configId + "_EXPIRATION", false);

            Type type = Type.GetType(string.Format("CSGenio.business.{0}", serviceType));
            IMapServiceProvider provider = (IMapServiceProvider) Activator.CreateInstance(type);
            return provider.GetToken(parameters);
        }

        /// <summary>
        /// Tries to obtain the glob record
        /// </summary>
        /// <returns>The glob record, if it exists, or null if it doesn't</returns>
        public DbArea GetGlob()
        {
            return null;
        }
    }
}
