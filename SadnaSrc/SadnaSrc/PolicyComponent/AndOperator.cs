using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class AndOperator : Operator
    {
        public AndOperator(PolicyType type, string subject, PurchasePolicy cond1, PurchasePolicy cond2, int id) : base(type, subject, cond1, cond2, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return _cond1.Evaluate(username, address, quantity, price) &&
                    _cond2.Evaluate(username, address, quantity, price);
        }

        public override string[] GetData()
        {
            return new[] {"" + ID, "AND", "" + _cond1.ID, "" + _cond2.ID};
        }

        public override string GetMyType()
        {
            return "AndOperator";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((AndOperator)obj);
        }
        private bool Equals(AndOperator obj)
        {
            return obj.ID.Equals(ID) && obj.Subject.Equals(Subject)
                   && obj.Type.Equals(Type)&&obj._cond1.Equals(_cond1) && obj._cond2.Equals(_cond2);
        }
    }
}
