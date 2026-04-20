using System;
using System.Collections.Generic;

namespace CSGenio.core.di
{

    /// <summary>
    /// Log provider implementation for errors and debugging information.
    /// </summary>
    public interface ILogImpl
    {
        /// <summary>
        /// EventTracking flag to enable or disable event tracking feature for debugging purposes
        /// </summary>
        bool EventTracking { get; set; }

        /// <summary>
        /// Adds an error message to the log file.
        /// </summary>
        /// <remarks>
        /// When log4net is selected, it uses its ThreadContext to store a thread-specific list of errors.
        /// This is activated only when the EventTracking feature is enabled and is specifically for debugging purposes.
        /// It allows you to capture errors that occurred during a specific request and send them to the client-side.
        /// Note: In MVC applications, the ThreadContext is cleared at the beginning and end of each request cycle.
        /// </remarks>
        /// <param name="msg">The error message to be logged.</param>
        void Error(string msg);

        /// <summary>
        /// Adiciona uma mensagem de trace ao file de log
        /// </summary>
        /// <param name="msg">A mensagem</param>
        /// <remarks>
        /// So deve ser activada em ambiente de desenvolvimento. Sempre que a mensagem for
        ///  composta através de métodos dispendiosos esta chamada deve ser protegida com um if
        ///  a IsDebugEnabled
        /// </remarks>
        void Debug(string msg);

        /// <summary>
        /// Adds a warning message to the choosen log system.
        /// </summary>
        /// <param name="msg">The warning message to be logged.</param>
        /// <remarks>
        /// Logs a message in a warning format to the choosen logging mechanism.
        /// </remarks>
        void Warning(string msg);

        /// <summary>
        /// Adds an info message to the choosen log system.
        /// </summary>
        /// <param name="msg">The info message to be logged.</param>
        /// <remarks>
        /// Logs a message in an info format to the choosen logging mechanism.
        /// </remarks>
        void Info(string msg);

        /// <summary>
        /// Verifica se queremos mesmo tentar por mensagens de tracing
        /// Evita o peso de construir a mensagem à custa de um if extra
        /// </summary>
        bool IsDebugEnabled { get; }

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
        IDisposable SetContext(object context);

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
        IDisposable SetContext(string context, object value);

        /// <summary>
        /// Retrieves a list of errors specific to the current thread context.
        /// </summary>
        /// <returns>A List of string containing the errors.</returns>
        List<string> GetThreadErrors();

        /// <summary>
        /// Clears the error cache for the current thread context.
        /// </summary>
        /// <remarks>
        /// This ensures that old error messages do not persist beyond the current lifecycle.
        /// Particularly useful to reset the state at the end of a request in MVC applications.
        /// </remarks>
        void ClearThreadErrorsCache();
    }
}
