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

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((Notification)obj);
        }
        private bool Equals(Notification obj)
        {
            return obj.Receiver == Receiver && obj.Message == Message && obj.Status == Status;
        }

        public override int GetHashCode()
        {
            var hashCode = 1116485059;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + Receiver.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status);
            return hashCode;
        }
    }
}
