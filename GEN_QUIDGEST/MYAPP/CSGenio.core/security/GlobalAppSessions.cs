using System;
using System.Security.Principal;
using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using CSGenio.framework;

namespace GenioServer.security
{
	/// <summary>
    /// Singleton class to store session and logged users so it can be accessed within the application
    /// TODO: implement methods to access the data and a way to send this information to QuidServer so que Web logged users
    /// are also listed on "Shutdown"
    /// </summary>
    public class GlobalAppSessions
    {
        private System.Collections.Generic.List<UserSession> Sessions;
        private static Object _lock = new Object();
        private static GlobalAppSessions instance;

        private GlobalAppSessions()
        {
            this.Sessions = new System.Collections.Generic.List<UserSession>();
        }

        public static GlobalAppSessions Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new GlobalAppSessions();
                }
                return instance;
            }
        }

        /// <summary>
        /// Function to get total number of sessions by aplication username logon
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public int GetSessionsByLogin(string login, string ipAddr)
        {
            if (string.IsNullOrEmpty(login))
                return 0;

            lock (_lock)
            {
                if (true)
                {

                    if (string.IsNullOrEmpty(ipAddr))
                        // It only allow 1 session with the same login
                        return Instance.Sessions.FindAll(x => x.Username == login).Count;
                    else
                        // Since it has ip address it means it is permits multiple sessions on the same ip, 
                        // so it counts sessions on different ip addresses
                        return Instance.Sessions.FindAll(x => x.Username == login && x.ip != ipAddr).Count;
                }
            }
        }

        /// <summary>
        /// Thread safe remove
        /// </summary>
        /// <param name="sessionId"></param>
        public void Remove(string sessionId)
        {
            lock (_lock)
            {
                GlobalAppSessions.Instance.Sessions.Remove(GlobalAppSessions.Instance.Sessions.Find(x => x.SessionId == sessionId));
            }
        }

        /// <summary>
        /// Thread safe add or update existing session data
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="username"></param>
        /// <param name="ipaddr"></param>
        public void AddOrUpdate(string sessionId, string username, string ipaddr)
        {
            this.AddOrUpdate(new UserSession() { SessionId = sessionId, Username = username, LastActivity = DateTime.Now, ip=ipaddr });
        }

        /// <summary>
        /// Thread safe add or update existing session data
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="username"></param>
        public void AddOrUpdate(string sessionId, string username)
        {
            this.AddOrUpdate(new UserSession() {SessionId = sessionId, Username = username, LastActivity = DateTime.Now });
        }

        /// <summary>
        /// Thread safe add or update existing session data
        /// </summary>
        /// <param name="sessionId"></param>
        public void AddOrUpdate(string sessionId)
        {
            this.AddOrUpdate(new UserSession() { SessionId = sessionId, LastActivity = DateTime.Now });
        }

        /// <summary>
        /// Thread safe add or update existing session data
        /// </summary>
        /// <param name="session"></param>
        public void AddOrUpdate(UserSession session)
        {
            if (Configuration.Security == null)
                return;

            if (Configuration.Security.AllowMultiSessionPerUser == GenioServer.security.MultiSessionMode.PerIp)
            {
                if (GetSessionsByLogin(session.Username, session.ip) >= 1)
                    throw new FrameworkException("O nome de utilizador já está autenticado noutro dispositivo.", "GlobalAppSessions", "O nome de utilizador já está autenticado noutro dispositivo.");
            }
            else if (Configuration.Security.AllowMultiSessionPerUser == GenioServer.security.MultiSessionMode.Strict)
            {
                if (GetSessionsByLogin(session.Username, "") >= 1)
                    throw new FrameworkException("O nome de utilizador já está autenticado.", "GlobalAppSessions", "O nome de utilizador já está autenticado.");
            }

            lock (_lock)
            {
                int idx = GlobalAppSessions.Instance.Sessions.FindIndex(x => x.SessionId == session.SessionId);
                if (idx>0)
                    GlobalAppSessions.Instance.Sessions[idx] = session;
                else
                    GlobalAppSessions.Instance.Sessions.Add(session);
            }
        }
    }

    /// <summary>
    /// Represents the basic info of a logged user
    /// </summary>
    public class UserSession
    {
        public DateTime LastActivity;
        public string SessionId;
        public string Username;
        public string ip;
    }
}