﻿using System;
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
            String subjectVal;
            if (Subject == null)
                subjectVal = "'NULL'";
            else
                subjectVal = "'" + Subject + "'";
            String cond1val;
            if (_cond1 == null)
                cond1val = "-1";
            else
                cond1val = "'" + _cond1.ID + "'";
            String cond2val;
            if (_cond2 == null)
                cond2val = "-1";
            else
                cond2val="'" + _cond2.ID + "'";


                return new[]
            {
                "" + ID,
                "'" + GetMyType() + "'",
                "'" + PrintEnum(Type) + "'",
                subjectVal,
                cond1val,
                cond2val,
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
    }
}
