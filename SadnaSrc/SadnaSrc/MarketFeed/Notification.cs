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
        public int Receiver { get; }
        public string Message { get; }

        public string Status { get; set; }

        private static readonly Random Random = new Random();
        public Notification(int receiver, string message)
        {
            Id = GenerateId();
            Receiver = receiver;
            Message = message;
            Status = "Pending";
        }

        public Notification(string id, int receiver, string message)
        {
            Id = id;
            Receiver = receiver;
            Message = message;
            Status = "Pending";
        }

        public object[] ToData()
        {
            return new object[] { Id,Receiver,Message,Status};
        }

        private static string GenerateId()
        {
            return ((char)Random.Next(97, 123)) + "" + ((char)Random.Next(97, 123)) + "" + Random.Next(1000, 10000);
        }
    }
}
