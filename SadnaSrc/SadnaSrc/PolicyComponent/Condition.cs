using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public abstract class Condition : PurchasePolicy
    {
        protected readonly string _value;

        protected Condition(PolicyType type, string subject, string value, int id) : base(type, subject, id)
        {
            _value = value;
        }

        public string[] GetPolicyStringValues()
        {

            string subjectval;
            if (Subject == null)
                subjectval = "'NULL'";
            else
                subjectval = "'"+Subject+ "'";

            string valueVal;
            if (_value == null)
                valueVal = "'NULL'";
            else
                valueVal = "'"+ _value + "'";
            return new[]
            {
                "" + ID,
                "'" + GetMyType() + "'",
                "'" + PrintEnum(Type) + "'",
                subjectval,
                valueVal,
                "'" + PrintBoolean(IsRoot) + "'"
            };
        }

        public object[] GetPolicyValuesArray()
        {

            string subjectval;
            if (Subject == null)
                subjectval = "NULL";
            else
                subjectval = Subject;

            string valueVal;
            if (_value == null)
                valueVal = "NULL";
            else
                valueVal = _value;
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
