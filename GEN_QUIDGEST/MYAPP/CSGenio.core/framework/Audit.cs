using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;

namespace CSGenio.framework
{
	
	/// <summary>
	/// Audit Class</summary>
	/// <remarks>
	/// Class used to audit actions requested by the client to the server </remarks>
    public static class Audit
    {
		/// <summary>
        /// Register an action in <system>mem table
        /// </summary>
        /// <param name="user"> Instance of Genio.framework.user </param>
        /// <param name="routine"> tag for action being registered</param>
        /// <param name="obs"> observations field </param>
		/// <param name="hostName">DNS name of the host</param>
		/// <param name="ipAddress">IP of the client requesting the action</param>
        public static void registMem(User user, string routine, string obs, string hostName, string ipAddress)
        {
            try
            {
                hostName = DetermineCompName(hostName);
            }
            catch (Exception)
            { 
				//ignores host name, uses IP instead.
            }
            // reduce to a maximum of 20 chars (db column limit)
            hostName = hostName.Substring(0, Math.Min(hostName.Length, 20));

            CSGenioAmem logItem = new CSGenioAmem(user);
            logItem.ValCodmem = Guid.NewGuid().ToString();
            logItem.ValLogin = user.Name;
            logItem.ValAltura = DateTime.Now;
            logItem.ValRotina = routine;
            logItem.ValObs = obs;
            logItem.ValZzstate = 0;
            logItem.ValHostid = hostName;
			logItem.ValClientid = ipAddress;
            logItem.UserRecord = false;

            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year);
            try
            {
				sp.openConnection();
                logItem.insert(sp);
                sp.closeConnection();
            }
            catch (Exception)
            {
                sp.closeConnection();
                Log.Error(string.Format("[Audit.registMem] Erro ao escrever na tabela de log '{0}mem'.", Configuration.Program));
            }
        }
		
		/// <summary>
        /// Register audit to save on Log<system>All
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="field">Field</param>
        /// <param name="cod">Code</param>
        /// <param name="op">Operation Type</param>
        /// <param name="val">Value</param>
        /// <param name="user">User</param>
        public static void RegistLogAll (string table, string field, string cod, string op, string val, User user)
        {
			//If no user is given, this would do nothing because no database would be referenced
			if (user == null || string.IsNullOrEmpty(user.Year)){
				Log.Error(string.Format("[Audit.RegistLogAll] User is not defined. Can't determine which database to access for log. [{0}->{1}]", table, field));
				return;
			}
				
            string who = string.IsNullOrEmpty(user.Name) ? "Undefined" : user.Name;

            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            sp.openConnection();

            
            string tableLog = "log" + Configuration.Program + "all";

            InsertQuery insertQ = new InsertQuery()
                .Into(tableLog)
                    .Value("COD", cod)
                    .Value("LOGTABLE", table)
                    .Value("LOGFIELD", field)
                    .Value("OP", op)
                    .Value("VAL", val)
                    .Value("DATE", DateTime.Now)
                    .Value("WHO", who);

            try
            {
                sp.Execute(insertQ);
                sp.closeConnection();
            }
            catch (Exception e)
            {
                Log.Error(string.Format("[Audit.RegistLogAll] Erro ao escrever na tabela de logAll {0}.", e.Message));
            }
        }

		/// <summary>
        /// Register login action or logout action in <system>mem table
        /// </summary>
        /// <param name="user"> Instance of Genio.framework.user </param>
        /// <param name="routine"> tag for action being registered</param>
        /// <param name="obs"> observations field </param>
		/// <param name="hostName">DNS name of the host</param>
		/// <param name="ipAddress">IP of the client requesting the action</param>
        public static void registLoginOut(User user, string routine, string obs, string hostName, string ipAddress)
        {
            registLoginOut(user, user.Name, routine, obs, hostName, ipAddress);
        }
		
		/// <summary>
        /// Register login action or logout action in <system>mem table
        /// </summary>
        /// <param name="user"> Instance of Genio.framework.user </param>
        /// <param name="userToSave"> Username to register on Database </param>
        /// <param name="routine"> tag for action being registered</param>
        /// <param name="obs"> observations field </param>
		/// <param name="hostName">DNS name of the host</param>
		/// <param name="ipAddress">IP of the client requesting the action</param>
        public static void registLoginOut(User user, string userToSave, string routine, string obs, string hostName, string ipAddress)
        {
            if (!Configuration.AuditTag.RegistLoginOut)
                return;

            hostName = DetermineCompName(hostName);
            // reduce to a maximum of 20 chars (db column limit)
            hostName = hostName.Substring(0, Math.Min(hostName.Length, 20));

            CSGenioAmem logItem = new CSGenioAmem(user);
            logItem.ValCodmem = Guid.NewGuid().ToString();
            logItem.ValLogin = userToSave;
            logItem.ValAltura = DateTime.Now;
            logItem.ValRotina = routine;
            logItem.ValObs = obs;
            logItem.ValZzstate = 0;
            logItem.ValHostid = hostName;
			logItem.ValClientid = ipAddress;
            logItem.UserRecord = false;

            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year);
            try
            {
				sp.openConnection();
                logItem.insert(sp);
                sp.closeConnection();
            }
            catch (Exception)
            {
				sp.closeConnection();
                Log.Error(string.Format("[Audit.registLoginOut] Erro ao escrever na tabela de log '{0}mem'.", Configuration.Program));
            }

            // Log with logger system
            Log.Info(String.Format("User {0} on host {1}: {2} - {3}", userToSave, hostName, routine, obs));
        }

	   /// <summary>
	   /// Register requesetd action in log<system>pro table
	   /// </summary>
	   /// <param name="user"> Instance of Genio.framework.user </param>
	   /// <param name="acao"> tag for action being registered</param>
        public static void registAction(User user, string action)
        {
            if (!Configuration.AuditTag.RegistActions)
                return;

            string table = "log" + Configuration.Program + "pro";

            InsertQuery insertQ = new InsertQuery()
                .Into(table)
                    .Value("Login", user.Name)
                    .Value("Data", DateTime.Now)
                    .Value("Accao", action);

            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            sp.openConnection();
            try
            {
                sp.Execute(insertQ);
                sp.closeConnection();
            }
            catch (Exception)
            {
                Log.Error(string.Format("[Audit.RegistActions] Erro ao escrever na tabela de log {0}.", table));
            }
        }

        /*
         * Auxiliary methods  
         */

        /// <summary>
        /// Get full domain name of a machine from DNS server 
        /// </summary>
        /// <param name="IP"> IP of machine of interest </param>
        private static string DetermineCompName(string IP)
        {
            try
            {
                System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
                System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
                string[] compName = GetIPHost.HostName.ToString().Split('.');
                if (!String.IsNullOrEmpty(compName[0]))
                    return compName[0];
                else
                    return "";
            }
            catch { return "Unknown"; }
        }
    }
}
