﻿using System;
using HyperMsg;
using HyperMsg.Config;
using HyperMsg.Exceptions;
using HyperMsg.Providers;
using NUnit.Framework;
using Tests.HyperMsg.Support;

namespace Tests.HyperMsg.Providers
{
    [TestFixture]
    public class RemoteMessageProviderTests : TestBase<RemoteMessageProvider>
    {
        [SetUp]
        public override void BeforeEachTest()
        {
            MockFor<IConfigSettings>().Setup(s => s.Address).Returns("http://localhost:8000");
            base.BeforeEachTest();
        }

        [TearDown]
        public void AfterEachTest()
        {
            Subject.Dispose();
        }

        [Test]
        public void SendThrowsExceptionWithNullMessage()
        {
            Assert.That(() => Subject.Send((Message)null),Throws.InstanceOf<ArgumentNullException>());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SendThrowsExceptionWithNoMessageEndPoint(string endPoint)
        {
            var message = new BrokeredMessage {EndPoint = endPoint};
            message.SetBody(new User { Forename = "Homer", Surname = "Simpson" });

            var exception = Assert.Throws<MessageException>(() => Subject.Send(new BrokeredMessage()));

            Assert.That(exception.Message, Is.EqualTo("Message does not contain a valid endpoint."));
        }
    }
}
