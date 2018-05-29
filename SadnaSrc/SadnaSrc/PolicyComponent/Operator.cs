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

        public object[] GetPolicyValuesArray()
        {
            string subjectval;
            if (Subject == null)
                subjectval = "NULL";
            else
                subjectval = Subject;
            int cond1val;
            if (_cond1 == null)
                cond1val = -1;
            else
                cond1val = _cond1.ID;
            int cond2val;
            if (_cond2 == null)
                cond2val = -1;
            else
                cond2val = _cond2.ID;
            return new object[]
            {
                ID,
                GetMyType(),
                PrintEnum(Type),
                subjectval,
                cond1val,
                cond2val,
                PrintBoolean(IsRoot)
            };
        }

	    public override string ToString()
	    {
		    return ID + " " + Type +" "+ Subject + " " + _cond1.ID + " "+ GetMyType()+" " + _cond2.ID;

	    }
    }
}
