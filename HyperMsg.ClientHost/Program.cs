﻿using System;
using System.Runtime.Serialization;
using HyperMsg.Config;
using HyperMsg.Providers;

namespace HyperMsg.ClientHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running the test client...Press return to start");

            try
            {
                Console.ReadLine();
                var message = new BrokeredMessage {EndPoint = "test"};
                message.SetBody(new User { Forename = "Homer", Surname = "Simpson" });
                var provider = new RemoteMessageProvider(new ConfigSettings());
                provider.Send(message);
            }
            catch (Exception error)
            {
                Console.Write(error);
            }

            Console.ReadLine();
        }
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public string Forename { get; set; }
        [DataMember]
        public string Surname { get; set; }
    }
}
