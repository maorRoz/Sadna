using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class UsernameEquals : Condition
    {
        public UsernameEquals(PolicyType type, string subject, string value, int id) : base(type, subject, value, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return username == _value;
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "Username", "=", _value };
        }
    }
}
