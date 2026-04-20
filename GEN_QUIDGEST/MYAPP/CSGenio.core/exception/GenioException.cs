using System;

namespace CSGenio.framework
{
    /// <summary>
    /// This class represents errors that occur during application execution.
    /// </summary>
    public abstract class GenioException
        : Exception
    {
        protected string userMessage;
        protected string exceptionSite;
        public string[] ErrorStack { get; protected set;}


        /// <summary>
        /// Initializes a new instance of the GenioException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        /// <param name="errorStack">The Exception instance that caused the current exception.</param>
        public GenioException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException, string[] errorStack)
            : base(exceptionCause, innerException)
        {
            this.userMessage = userMessage;
            this.exceptionSite = exceptionSite ?? "";
            ErrorStack = errorStack;

            LogError();
        }

        /// <summary>
        /// Initializes a new instance of the GenioException class.
        /// </summary>
        /// <param name="userMessage">Message that describes the current exception to the user.</param>
        /// <param name="exceptionSite">Name of the method that throws the current exception.</param>
        /// <param name="exceptionCause">Message that describes the direct cause of the current exception.</param>
        /// <param name="innerException">The Exception instance that caused the current exception.</param>
        public GenioException(string userMessage, string exceptionSite, string exceptionCause, Exception innerException)
            : base(exceptionCause, innerException)
        {
            this.userMessage = userMessage;
            this.exceptionSite = exceptionSite ?? "";

            LogError();
        }

        /// <summary>
        /// Gets and sets the message that describes the current exception to the user.
        /// </summary>
        public string UserMessage
        {
            get
            {
                return userMessage;
            }
            set
            {
                userMessage = value;
            }
        }

        /// <summary>
        /// Gets and sets the message that describes the current exception to the user.
        /// </summary>
        [ObsoleteAttribute("This property is obsolete. Use UserMessage instead.", false)]
        public string MsgUtilizador
        {
            get
            {
                return userMessage;
            }
            set
            {
                userMessage = value;
            }
        }

        /// <summary>
        /// Gets and sets the name of the method that throws the current exception.
        /// </summary>
		public string ExceptionSite
        {
            get
            {
                return exceptionSite;
            }
            set
            {
                exceptionSite = value;
            }
        }

        /// <summary>
        /// Gets and sets the name of the method that throws the current exception.
        /// </summary>
        [ObsoleteAttribute("This property is obsolete. Use ExceptionSite instead.", false)]
        public string ErrorLocation
        {
            get
            {
                return exceptionSite;
            }
            set
            {
                exceptionSite = value;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        [ObsoleteAttribute("This property is obsolete. Use Message instead.", false)]
        public string ErrorCause
        {
            get
            {
                return Message;
            }
        }

        /// <summary>
        /// Formats the log messages.
        /// </summary>
        protected string FormatLog(string exceptionName)
        {
            return string.Format("{0}. [message] {1} [site] {2} [cause] {3}", exceptionName, userMessage, exceptionSite, Message);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        protected void LogError(string exceptionName)
        {
            Log.Error(FormatLog(exceptionName));
        }

        protected abstract void LogError();
    }
}
