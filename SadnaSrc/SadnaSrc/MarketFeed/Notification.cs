using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public class Notification
    {   
        public string Id { get; }
        public string Receiver { get; }
        public string Message { get; }

        public string Status { get; }

        private static readonly Random Random = new Random();
        public Notification(string receiver, string message)
        {
            Id = GenerateId();
            Receiver = receiver;
            Message = message;
            Status = "Unread";
        }

        public Notification(string id, string receiver, string message)
        {
            Id = id;
            Receiver = receiver;
            Message = message;
            Status = "Unread";
        }

        private static string GenerateId()
        {
            return ((char)Random.Next(97, 123)) + "" + ((char)Random.Next(97, 123)) + "" + Random.Next(1000, 10000);
        }
    }
}
