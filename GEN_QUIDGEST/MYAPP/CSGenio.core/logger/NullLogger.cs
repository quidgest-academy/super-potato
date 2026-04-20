using CSGenio.core.di;
using System;
using System.Collections.Generic;

namespace CSGenio.core.logger;

/// <summary>
/// Placeholder logger for GenioDI
/// This logger does not depend on anything, and does not log anywhere other than its own memory.
/// Might be useful for unit testing, to assert certain algorithms emit important logs.
/// </summary>
public class NullLogger : ILogImpl
{
    private List<string> _logs = new List<string>();

    /// <inheritdoc/>
    public bool EventTracking { get; set; }

    /// <inheritdoc/>
    public bool IsDebugEnabled => true;

    /// <inheritdoc/>
    public void ClearThreadErrorsCache() => _logs.Clear();

    /// <inheritdoc/>
    public void Debug(string msg) => _logs.Add(msg);

    /// <inheritdoc/>
    public void Error(string msg) =>
        _logs.Add(msg);

    /// <inheritdoc/>
    public List<string> GetThreadErrors() => _logs;

    /// <inheritdoc/>
    public void Info(string msg) => _logs.Add(msg);

    /// <inheritdoc/>
    public IDisposable SetContext(object context) => null;

    /// <inheritdoc/>
    public IDisposable SetContext(string context, object value) => null;

    /// <inheritdoc/>
    public void Warning(string msg) => _logs.Add(msg);
}
