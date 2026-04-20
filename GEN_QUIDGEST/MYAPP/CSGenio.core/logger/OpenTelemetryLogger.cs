using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSGenio.core.di
{
    /// <summary>
    /// OpenTelemetry based logger implementation
    /// </summary>
    public class OpenTelemetryImpl : ILogImpl
    {
        private readonly ILogger _logger;

        /// <summary>
        /// EventTracking flag to enable or disable event tracking feature for debugging purposes
        /// </summary>
        public bool EventTracking { get; set; } = false;

        public OpenTelemetryImpl(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger("Genio.Server");
        }

        /// <summary>
        /// Adds an error message to the log file.
        /// </summary>
        /// <param name="msg">The error message to be logged.</param>
        public void Error(string msg)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(msg);
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
            if(_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug(msg);
        }

        /// <summary>
        /// Adds an info message to the log system.
        /// </summary>
        /// <param name="msg">The info message to be logged.</param>
        public void Info(string msg)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation(msg);
        }

        /// <summary>
        /// Adds an warning message to the log system.
        /// </summary>
        /// <param name="msg">The warning message to be logged.</param>
        public void Warning(string msg)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(msg);
        }

        /// <summary>
        /// Verifica se queremos mesmo tentar por mensagens de tracing
        /// Evita o peso de construir a mensagem à custa de um if extra
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Inicializa um marcador de estado desta thread de processamento
        /// Permite às mensagens de erro subsequentes saber em que contexto foram invocadas
        /// </summary>
        /// <param name="context">O contexto a inicializar em formato key:object ou string</param>
        /// <example>
        /// Um bom sitio to usar é to marcar o user que está no contexto
        /// </example>
        /// <remarks>
        /// Em ASP.Net tem de se ter cuidado com thread agility:
        /// http://blog.marekstoj.com/2011/12/log4net-contextual-properties-and.html
        /// </remarks>
        public IDisposable SetContext(object context)
        {
            if (context is string) return _logger.BeginScope(new { other = context.ToString() });
            if (context is Dictionary<string, object>) return _logger.BeginScope((Dictionary<string, object>)context);

            Dictionary<string, object> parsedContext = new Dictionary<string, object>();
            foreach (PropertyInfo prop in context.GetType().GetProperties())
                parsedContext[prop.Name] = prop.GetValue(context, null);

            return _logger.BeginScope(parsedContext);
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
            return new List<string>();
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

        }
    }
}
