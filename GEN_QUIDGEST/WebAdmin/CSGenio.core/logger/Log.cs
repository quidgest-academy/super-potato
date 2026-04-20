using CSGenio.core.di;
using System;
using System.Collections.Generic;

namespace CSGenio.framework
{
    /// <inheritdoc cref="ILogImpl" />
    /// <remarks>
    /// Backwards compatibility with old code.
    /// This class will be removed in future versions.
    /// </remarks>
    //[Obsolete("Use the GenioDI.Log methods instead")]
    public static class Log
    {
        /// <inheritdoc cref="ILogImpl.EventTracking" />
        public static bool EventTracking => GenioDI.Log.EventTracking;

        /// <inheritdoc cref="ILogImpl.Error" />
        public static void Error(string msg) => GenioDI.Log.Error(msg);

        /// <inheritdoc cref="ILogImpl.Debug" />
        public static void Debug(string msg) => GenioDI.Log.Debug(msg);

        /// <inheritdoc cref="ILogImpl.Info" />
        public static void Info(string msg) => GenioDI.Log.Info(msg);

        /// <inheritdoc cref="ILogImpl.Warning" />
        public static void Warning(string msg) => GenioDI.Log.Warning(msg);

        /// <inheritdoc cref="ILogImpl.IsDebugEnabled" />
        public static bool IsDebugEnabled => GenioDI.Log.IsDebugEnabled;

        /// <inheritdoc cref="ILogImpl.SetContext" />
        public static IDisposable SetContext(object context) => GenioDI.Log.SetContext(context);
        public static IDisposable SetContext(string context, object value) => GenioDI.Log.SetContext(context, value);

        /// <inheritdoc cref="ILogImpl.SetEventTracking" />
        public static void SetEventTracking(bool isActive)
        {
            GenioDI.Log.EventTracking = isActive;
        }

        /// <inheritdoc cref="ILogImpl.GetThreadErrors" />
        public static List<string> GetThreadErrors() => GenioDI.Log.GetThreadErrors();

        /// <inheritdoc cref="ILogImpl.ClearThreadErrorsCache" />
        public static void ClearThreadErrorsCache() => GenioDI.Log.ClearThreadErrorsCache();
    }
}