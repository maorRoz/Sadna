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
            string[] op1 = _cond1.GetData();
            string[] op2 = _cond2.GetData();
            List<string> res = new List<string>();
            foreach (string str in op1)
            {
                res.Add(str);
            }
            res.Add("AND");
            foreach (string str in op2)
            {
                res.Add(str);
            }

            return res.ToArray();
        }
    }
}
