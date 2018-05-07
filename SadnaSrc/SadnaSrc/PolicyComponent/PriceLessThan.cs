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
            return new[] { "" + _id, _type.ToString(), _subject, "Price", "<=", _value };
        }
    }
}
