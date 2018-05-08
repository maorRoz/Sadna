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
            return address.Contains(_value);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "Address", "=", _value };
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
            return obj._value.Equals(_value) && obj.ID.Equals(ID) && obj.Subject.Equals(Subject)
                   && obj.Type.Equals(Type);
        }
    }
}
