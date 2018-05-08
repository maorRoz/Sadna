using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class QuantityGreaterThan : Condition
    {
        public QuantityGreaterThan(PolicyType type, string subject, string value, int id) : base(type, subject, value, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return quantity >= Int32.Parse(_value);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "Quantity", ">=", _value};
        }
        public override string GetMyType()
        {
            return "QuantityGreaterThan";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((QuantityGreaterThan)obj);
        }
        private bool Equals(QuantityGreaterThan obj)
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
    }
}
