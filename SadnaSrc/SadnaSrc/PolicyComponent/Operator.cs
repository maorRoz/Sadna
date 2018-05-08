using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public abstract class Operator : PurchasePolicy
    {
        internal readonly PurchasePolicy _cond1;
        internal readonly PurchasePolicy _cond2;

        public Operator(PolicyType type, string subject, PurchasePolicy cond1, PurchasePolicy cond2, int id) : base(type,
            subject, id)
        {
            _cond1 = cond1;
            _cond2 = cond2;
        }
        public string[] GetPolicyStringValues()
        {
            return new[]
            {
                "'" + ID + "'",
                "'" + GetMyType() + "'",
                "'" + PrintEnum(Type) + "'",
                "'" + Subject + "'",
                "'" + _cond1.ID + "'",
                "'" + _cond2.ID + "'",
                "'" + PrintBoolean(IsRoot) + "'"

            };
        }

        public object[] GetPolicyValuesArray()
        {
            return new object[]
            {
                ID,
                GetMyType(),
                PrintEnum(Type),
                Subject,
                _cond1.ID,
                _cond2.ID,
                PrintBoolean(IsRoot)
            };
        }
    }
}
