using System;
using Microsoft.Isam.Esent.Interop;

namespace HyperMsg.Controller.Data
{
    public class TransactionSession : IDisposable
    {
        private readonly Transaction _transaction;
        private bool _completed;
        private bool _disposed;

        internal TransactionSession(Session sessionId, string path)
        {
            SessionId = sessionId;
            JET_DBID dbid;
            Api.JetAttachDatabase(sessionId, path, AttachDatabaseGrbit.None);
            Api.JetOpenDatabase(sessionId, path, string.Empty, out dbid, OpenDatabaseGrbit.None);
            DatabaseId = dbid;
            _transaction = new Transaction(sessionId);
        }

        ~TransactionSession()
        {
            Dispose(false);
        }

        internal JET_DBID DatabaseId { get; private set; }
        internal Session SessionId { get; private set; }

        internal void Complete()
        {
            _completed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_completed)
                {
                    _transaction.Commit(CommitTransactionGrbit.None);
                }
                else
                {
                    _transaction.Rollback();
                }

                Api.JetCloseDatabase(SessionId, DatabaseId, CloseDatabaseGrbit.None);

                SessionId.Dispose();
            }

            _disposed = true;
        }
    }
}