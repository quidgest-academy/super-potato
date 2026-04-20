using System;

namespace CSGenio.persistence
{

    /// <summary>
    /// This class ensures that the connection and transaction state are perserved in a given scope.
    /// Use it when you have a persistent support that was passed to your scope and you have no idea of what state it will be in. 
    /// When the scope finishes, the initial state is restored.
    /// This class only closes what it opens, if the initial state is opened the original transaction is kept open
    /// </summary>
    public class ScopedPersistentSupport : IDisposable
    {
        private readonly bool connectionWasOpen;
        private readonly bool transactionWasOpen;

        private readonly PersistentSupport persistentSupport;

        
        /// <summary>
        /// Creates a scoped persistent support. This constructor should always be used in an using clause
        /// If the connection or transaction were closed it will be opened after this call.
        /// </summary>
        /// <param name="sp"></param>
        public ScopedPersistentSupport(PersistentSupport sp)
        {
            persistentSupport = sp;
            lock (sp)
            {
                transactionWasOpen = !sp.TransactionIsClosed;
                connectionWasOpen = !sp.ConnectionIsClosed;

                if (sp.ConnectionIsClosed)
                {
                    persistentSupport.openConnection();
                }
                if(sp.TransactionIsClosed)
                {
                    persistentSupport.openTransaction();
                }

            }
        }

        public void Dispose()
        {
            if (!transactionWasOpen)
                persistentSupport.closeTransaction();

            if (connectionWasOpen && persistentSupport.ConnectionIsClosed)
                persistentSupport.openConnection();
            else if (!connectionWasOpen)
                persistentSupport.closeConnection();
        }


    }
}
