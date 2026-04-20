using CSGenio.framework;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSGenio.core.di
{

    /// <summary>
    /// Log4net based logger implementation
    /// </summary>
    public class Log4NetImpl : ILogImpl
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// EventTracking flag to enable or disable event tracking feature for debugging purposes
        /// </summary>
        public bool EventTracking { get; set; } = false;

        /// <summary>
        /// Adds an error message to the log file.
        /// </summary>
        /// <remarks>
        /// Utilizes log4net's ThreadContext to store a thread-specific list of errors.
        /// This is activated only when the EventTracking feature is enabled and is specifically for debugging purposes.
        /// It allows you to capture errors that occurred during a specific request and send them to the client-side.
        /// Note: In MVC applications, the ThreadContext is cleared at the beginning and end of each request cycle.
        /// </remarks>
        /// <param name="msg">The error message to be logged.</param>
        public void Error(string msg)
        {
            // Log the error message to file
            log.Error(msg);

            // Check if the event tracking feature is active for debugging
            if (EventTracking)
            {
                // ThreadContext allows for storing contextual information tied to a specific thread.
                // This enables logging of multiple events in a sequence safely, per thread.
                // https://logging.apache.org/log4net/release/manual/contexts.html

                // Retrieve the error list from the current thread's context
                var errorList = log4net.ThreadContext.Properties["ErrorList"] as List<string>;
                // Initialize the error list if it does not exist
                if (errorList == null)
                {
                    errorList = new List<string>();
                    log4net.ThreadContext.Properties["ErrorList"] = errorList;
                }

                // Append the new error message to the thread-specific error list
                errorList.Add(msg);

                // Limit the error list to the last 20 entries for memory efficiency
                if (errorList.Count > 20)
                {
                    errorList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Adiciona uma mensagem de trace ao file de log
        /// </summary>
        /// <param name="msg">A mensagem</param>
        /// <remarks>
        /// So deve ser activada em ambiente de desenvolvimento. Sempre que a mensagem for
        ///  composta através de métodos dispendiosos esta chamada deve ser protegida com um if
        ///  a IsDebugEnabled
        /// </remarks>
        public void Debug(string msg)
        {
            log.Debug(msg);
        }

        /// <summary>
        /// Adds an info message to the log system.
        /// </summary>
        /// <param name="msg">The info message to be logged.</param>
        public void Info(string msg)
        {
            log.Info(msg);
        }

        /// <summary>
        /// Adds an warning message to the log system.
        /// </summary>
        /// <param name="msg">The warning message to be logged.</param>
        public void Warning(string msg)
        {
            log.Warn(msg);
        }

        /// <summary>
        /// Verifica se queremos mesmo tentar por mensagens de tracing
        /// Evita o peso de construir a mensagem à custa de um if extra
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        /// <summary>
        /// Inicializa um marcador de estado desta thread de processamento
        /// Permite às mensagens de erro subsequentes saber em que contexto foram invocadas
        /// </summary>
        /// <param name="context">O contexto a inicializar no formato key:object ou string</param>
        /// <example>
        /// Um bom sitio to usar é to marcar o user que está no contexto
        /// </example>
        /// <remarks>
        /// Em ASP.Net tem de se ter cuidado com thread agility:
        /// http://blog.marekstoj.com/2011/12/log4net-contextual-properties-and.html
        /// </remarks>
        public IDisposable SetContext(object context)
        {
            try
            {
                if (context is string)
                {
                    log4net.ThreadContext.Properties["other"] = context.ToString();
                    log4net.LogicalThreadContext.Properties["other"] = context.ToString();
                }
                else if (context is Dictionary<string, object> tCtx)
                {
                    foreach(var kvp in tCtx)
                    {
                        log4net.ThreadContext.Properties[kvp.Key] = kvp.Value;
                        log4net.LogicalThreadContext.Properties[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    // Log4Net properties are read only so unfortunately we need this contraption
                    foreach (PropertyInfo prop in context.GetType().GetProperties())
                    {
                        log4net.ThreadContext.Properties[prop.Name] = prop.GetValue(context, null);
                        log4net.LogicalThreadContext.Properties[prop.Name] = prop.GetValue(context, null);
                    }
                }
            }
            catch (Exception)
            {
                throw new FrameworkException("Unsupported context format", "Log4NetLogger.SetContext", "Unsupported context format");
            }

            return null;
        }

        /// <summary>
        /// Inicializa um marcador de estado desta thread de processamento
        /// Permite às mensagens de erro subsequentes saber em que contexto foram invocadas
        /// </summary>
        /// <param name="context">O contexto a inicializar</param>
        /// <param name="value">O Qvalue a por no contexto</param>
        /// <example>
        /// Um bom sitio to usar é to marcar o user que está no contexto
        /// </example>
        /// <remarks>
        /// Em ASP.Net tem de se ter cuidado com thread agility:
        /// http://blog.marekstoj.com/2011/12/log4net-contextual-properties-and.html
        /// </remarks>	
        public IDisposable SetContext(string context, object value)
        {
            return SetContext(new Dictionary<string, object> { { context, value } });
        }

        /// <summary>
        /// Retrieves a list of errors specific to the current thread context.
        /// </summary>
        /// <returns>A List of string containing the errors.</returns>
        public List<string> GetThreadErrors()
        {
            // Fetches the error list from the current thread context for debugging when EventTracking is active
            return log4net.ThreadContext.Properties["ErrorList"] as List<string>;
        }

        /// <summary>
        /// Clears the error cache for the current thread context.
        /// </summary>
        /// <remarks>
        /// This ensures that old error messages do not persist beyond the current lifecycle.
        /// Particularly useful to reset the state at the end of a request in MVC applications.
        /// </remarks>
        public void ClearThreadErrorsCache()
        {
            // Nullifies the error list in the current thread context, effectively clearing it
            log4net.ThreadContext.Properties["ErrorList"] = null;
        }
    }
}
