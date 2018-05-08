using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class QuantityLessThan : Condition
    {
        public QuantityLessThan(PolicyType type, string subject, string value, int id) : base(type, subject, value, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return quantity <= Int32.Parse(_value);
        }

        public override string[] GetData()
        {
            return new[] {""+ID, "Quantity", "<=", _value };
        }
        public override string GetMyType()
        {
            return "QuantityLessThan";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((QuantityLessThan)obj);
        }
        private bool Equals(QuantityLessThan obj)
        {
            return obj._value.Equals(_value) && obj.ID.Equals(ID) && obj.Subject.Equals(Subject)
                   && obj.Type.Equals(Type);
        }
    }
}
