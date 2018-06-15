using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class AddressEquals : Condition
    {
        public AddressEquals(PolicyType type, string subject, string value, int id) : base(type, subject, value, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return address.Contains(Value);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "Address", "=", Value };
        }

        public override string GetMyType()
        {
            return "AddressEquals";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((AddressEquals)obj);
        }
        private bool Equals(AddressEquals obj)
        {
            bool answer = true;
            if (Value == null)
                answer = answer && (obj.Value == null);
            if (Subject == null)
                answer = answer && (obj.Subject == null);
            if (Value != null)
                answer = answer && (Value.Equals(obj.Value));
            if (Subject != null)
                answer = answer && (Subject.Equals(obj.Subject));
            answer = answer && obj.ID.Equals(ID) && obj.Type.Equals(Type);
            return answer;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string getMyTypeVisual()
        {
            return "Address =";
        }
    }
}
