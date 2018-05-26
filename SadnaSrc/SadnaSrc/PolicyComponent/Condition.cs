using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public abstract class Condition : PurchasePolicy
    {
        public readonly string Value;

        protected Condition(PolicyType type, string subject, string value, int id) : base(type, subject, id)
        {
            Value = value;
        }

        public object[] GetPolicyValuesArray()
        {

            string subjectval;
            if (Subject == null)
                subjectval = "NULL";
            else
                subjectval = Subject;

            string valueVal;
            if (Value == null)
                valueVal = "NULL";
            else
                valueVal = Value;
            return new object[]
            {
                ID,
                GetMyType(),
                PrintEnum(Type),
                subjectval,
                valueVal,
                PrintBoolean(IsRoot)
            };
        }
    }
}
