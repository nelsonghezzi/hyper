﻿using HyperMsg;
using HyperMsg.Broker.Data.Entities;
using HyperMsg.Broker.Data.Repositories;
using HyperMsg.Broker.Services;
using Moq;
using NUnit.Framework;
using Tests.HyperMsg.Broker.Support;

namespace Tests.HyperMsg.Broker
{
    [TestFixture]
    public class MessageServiceTests : TestBase<MessageService>
    {
        [Test]
        public void PostAddsMessageToDatabase()
        {
            var message = new BrokeredMessage();
            message.SetBody(new User { Forename = "Homer", Surname = "Simpson" });

            Subject.Post(message);

            MockFor<IMessageRepository>().Verify(r => r.Add(
                It.Is<MessageEntity>(e => e.MessageId == message.Id)), Times.Once);
        }
    }
}
