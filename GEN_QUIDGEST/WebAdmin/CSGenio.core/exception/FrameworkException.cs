using System;

namespace CSGenio.framework
{
    /// <summary>
    /// This class represents general errors that occur during application execution.
    /// </summary>
    public class FrameworkException : GenioException
    {
        private static string exceptionName = "FrameWork Exception";

        /// <summary>
        /// Initializes a new instance of the FrameworkException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        public FrameworkException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException)
            : base(userMessage, exceptionSite, exceptionCause, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FrameworkException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        public FrameworkException(string userMessage, string exceptionSite, string exceptionCause)
            : this(userMessage, exceptionSite, exceptionCause, null)
        {
        }

        protected override void LogError()
        {
            LogError(exceptionName);
        }
    }
}
