﻿using System;
using System.Collections.Generic;
using System.Text;
using HyperMsg.Broker.Data.Entities;
using Microsoft.Isam.Esent.Interop;

namespace HyperMsg.Broker.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IConnectionProvider _connectionProvider;
        private const string TableName = "Messages";

        public MessageRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public MessageEntity Get(Guid id)
        {
            using (var session = _connectionProvider.OpenSession())
            using (var table = new Table(session.SessionId, session.DatabaseId, TableName, OpenTableGrbit.None))
            {
                Api.JetSetCurrentIndex(session.SessionId, table, "UIX_MESSAGEID");
                Api.MakeKey(session.SessionId, table, id, MakeKeyGrbit.NewKey);

                if (Api.TrySeek(session.SessionId, table, SeekGrbit.SeekEQ))
                {
                    return Create(session.SessionId, table);
                }

                return null;
            }
        }

        public IEnumerable<MessageEntity> Get(int count)
        {
            using (var session = _connectionProvider.OpenSession())
            using (var table = new Table(session.SessionId, session.DatabaseId, TableName, OpenTableGrbit.None))
            {
                var entities = new List<MessageEntity>();

                if (Api.TryMoveFirst(session.SessionId, table))
                {
                    do
                    {
                        entities.Add(Create(session.SessionId, table));
                    }
                    while (Api.TryMoveNext(session.SessionId, table) && entities.Count < count);
                }

                return entities;
            }
        }

        public void Add(MessageEntity messageEntity)
        {
            using (var session = _connectionProvider.OpenSession())
            using (var table = new Table(session.SessionId, session.DatabaseId, TableName, OpenTableGrbit.None))
            using (var updater = new Update(session.SessionId, table, JET_prep.Insert))
            {
                var columnDesc = Api.GetTableColumnid(session.SessionId, table, "MessageId");
                Api.SetColumn(session.SessionId, table, columnDesc, messageEntity.MessageId);

                columnDesc = Api.GetTableColumnid(session.SessionId, table, "Body");
                Api.SetColumn(session.SessionId, table, columnDesc, messageEntity.Body, Encoding.Unicode);

                updater.Save();
                session.Complete();
            }
        }

        public void Remove(Guid id)
        {
            using (var session = _connectionProvider.OpenSession())
            using (var table = new Table(session.SessionId, session.DatabaseId, TableName, OpenTableGrbit.None))
            {
                Api.JetSetCurrentIndex(session.SessionId, table, "UIX_MESSAGEID");
                Api.MakeKey(session.SessionId, table, id, MakeKeyGrbit.NewKey);

                if (Api.TrySeek(session.SessionId, table, SeekGrbit.SeekEQ))
                {
                    Api.JetDelete(session.SessionId, table);
                }

                session.Complete();
            }
        }

        private static MessageEntity Create(Session sessionId, JET_TABLEID table)
        {
            var entity = new MessageEntity();

            var columnId = Api.GetTableColumnid(sessionId, table, "Id");
            entity.Id = Api.RetrieveColumnAsInt32(sessionId, table, columnId) ?? -1;

            var columnMessageId = Api.GetTableColumnid(sessionId, table, "MessageId");
            entity.MessageId = Api.RetrieveColumnAsGuid(sessionId, table, columnMessageId) ?? Guid.Empty;

            var columnDesc = Api.GetTableColumnid(sessionId, table, "Body");
            entity.Body = Api.RetrieveColumnAsString(sessionId, table, columnDesc, Encoding.Unicode);

            return entity;
        }
    }
}