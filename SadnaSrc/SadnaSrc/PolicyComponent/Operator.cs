using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public abstract class Operator : PurchasePolicy
    {
        protected readonly PurchasePolicy _cond1;
        protected readonly PurchasePolicy _cond2;

        public Operator(PolicyType type, string subject, PurchasePolicy cond1, PurchasePolicy cond2, int id) : base(type,
            subject, id)
        {
            _cond1 = cond1;
            _cond2 = cond2;
        }
    }
}
