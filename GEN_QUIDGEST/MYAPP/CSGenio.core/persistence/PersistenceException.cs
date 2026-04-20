using System;
using CSGenio.framework;

namespace CSGenio.persistence
{
    /// <summary>
    /// This class represents persistence errors that occur during application execution.
    /// </summary>
    public class PersistenceException : GenioException
    {
        public static string ERRO_CONEXAO = "Erro ao abrir a conexão.";
        public static string ERRO_QUERY = "Erro na execução do query.";

        private static string exceptionName = "Persistence Exception";
        private bool retryable = false;

        /// <summary>
        /// Initializes a new instance of the PersistenceException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        /// <param name="retry">Defines if the exception is retryable.</param>
        public PersistenceException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException, bool retry)
            : base(userMessage, exceptionSite, exceptionCause, innerException)
        {
            this.retryable = retry;
        }

        /// <summary>
        /// Initializes a new instance of the PersistenceException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        public PersistenceException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException)
            : this(userMessage, exceptionSite, exceptionCause, innerException, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PersistenceException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        public PersistenceException(string userMessage, string exceptionSite, string exceptionCause)
            : this(userMessage, exceptionSite, exceptionCause, null, false)
        {
        }

        /// <summary>
        /// True if the Exception is retryable
        /// </summary>
        public bool IsRetryable
        {
            get
            {
                return retryable;
            }
        }

        protected override void LogError()
        {
            LogError(exceptionName);
        }
    }
}
