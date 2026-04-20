
using CSGenio.framework;


namespace CSGenio.business
{
    /// <summary>
    /// This class represent exceptions that aggregate multiple errors. 
    /// Use this when you need to process multiple errors instead of stopping only on the first
    /// </summary>
    public class FieldValidationException : BusinessException
    {
        private static string exceptionName = "Field Validation Exception";
        

        public StatusMessage StatusMessage;

        public FieldValidationException(StatusMessage statusMessage, string exceptionSite) :
            base(statusMessage.PrintMessages(), exceptionSite, "Field validation failed", null, statusMessage.GetStackMessages())
        {
            this.StatusMessage = statusMessage;
        }

        protected override void LogError()
        {
            Log.Info(FormatLog(exceptionName));
        }
    }

    public class InvalidAccessException : BusinessException
    {
        private static string exceptionName = "Invalid Access Exception";


        public StatusMessage StatusMessage;

        public InvalidAccessException(StatusMessage statusMessage, ConditionType type) :
            base(statusMessage.PrintMessages(), $"Accessing record in {type} mode", $"Invalid access to {type} exception", null)
        {
            this.StatusMessage = statusMessage;
        }

        protected override void LogError()
        {
            Log.Info(FormatLog(exceptionName));
        }
    }
}
