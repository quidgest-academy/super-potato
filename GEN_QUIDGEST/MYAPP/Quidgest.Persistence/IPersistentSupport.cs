using System;
using System.Data;
using Quidgest.Persistence.GenericQuery;

namespace Quidgest.Persistence
{
    public interface IPersistentSupport
    {
        Dialect Dialect { get; }
        IDbDataParameter CreateParameter(object value);
        IDbDataParameter CreateParameter(string name, object value);
    }
}
