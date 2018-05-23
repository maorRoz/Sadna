using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class PriceGreaterThan : Condition
    {
        public PriceGreaterThan(PolicyType type, string subject, string value, int id) : base(type, subject, value, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return price >= Double.Parse(_value);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "Price", ">=", _value };
        }
        public override string GetMyType()
        {
            return "PriceGreaterThan";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((PriceGreaterThan)obj);
        }
        private bool Equals(PriceGreaterThan obj)
        {
            bool answer = true;
            if (_value == null)
                answer = answer && (obj._value == null);
            if (Subject == null)
                answer = answer && (obj.Subject == null);
            if (_value != null)
                answer = answer && (_value.Equals(obj._value));
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
    }
}
