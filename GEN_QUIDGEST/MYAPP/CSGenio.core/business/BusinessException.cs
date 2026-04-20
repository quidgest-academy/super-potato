using System;
using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.business
{
    /// <summary>
    /// This class represents business errors that occur during application execution.
    /// </summary>
    public class BusinessException : GenioException
    {
        private static string exceptionName = "Business Exception";

        /// <summary>
        /// Initializes a new instance of the BusinessException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        /// <param name="errorStack">The error stack.</param>
        public BusinessException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException, string[] errorStack)
            : base(userMessage, exceptionSite, exceptionCause, innerException, errorStack)
        {

        }


        /// <summary>
        /// Initializes a new instance of the BusinessException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        public BusinessException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException)
            : base(userMessage, exceptionSite, exceptionCause, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BusinessException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        public BusinessException(string userMessage, string exceptionSite, string exceptionCause)
            : this(userMessage, exceptionSite, exceptionCause, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BusinessException class.
        /// </summary>
        /// <param name="ep">Persistence exception.</param>
        public BusinessException(PersistenceException ep)
            : this(ep.UserMessage, ep.ExceptionSite, ep.Message, ep)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BusinessException class.
        /// </summary>
        /// <param name="ep">Framework exception.</param>
        public BusinessException(FrameworkException ef)
            : this(ef.UserMessage, ef.ExceptionSite, ef.Message, ef)
        {
        }

        protected override void LogError()
        {
            Log.Info(FormatLog(exceptionName));
        }
    }
}
