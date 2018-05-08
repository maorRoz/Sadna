using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class PriceLessThan : Condition
    {
        public PriceLessThan(PolicyType type, string subject, string value, int id) : base(type, subject, value, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return price <= Double.Parse(_value);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "Price", "<=", _value };
        }
        public override string GetMyType()
        {
            return "PriceLessThan";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((PriceLessThan)obj);
        }
        private bool Equals(PriceLessThan obj)
        {
            return obj._value.Equals(_value) && obj.ID.Equals(ID) && obj.Subject.Equals(Subject)
                   && obj.Type.Equals(Type);
        }
    }
}
