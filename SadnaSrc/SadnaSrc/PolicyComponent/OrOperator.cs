﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class OrOperator : Operator
    {
        public OrOperator(PolicyType type, string subject, PurchasePolicy cond1, PurchasePolicy cond2, int id) : base(type, subject, cond1, cond2, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return _cond1.Evaluate(username, address, quantity, price) ||
                    _cond2.Evaluate(username, address, quantity, price);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "OR", "" + _cond1.ID, "" + _cond2.ID };
        }

        public override string GetMyType()
        {
            return "OrOperator";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((OrOperator)obj);
        }
        private bool Equals(OrOperator obj)
        {
            bool answer = true;
            if (_cond1 == null)
                answer = answer && (obj._cond1 == null);
            if (Subject == null)
                answer = answer && (obj.Subject == null);
            if (_cond1 != null)
                answer = answer && (_cond1.Equals(obj._cond1));
            if (Subject != null)
                answer = answer && (Subject.Equals(obj.Subject));
            answer = answer && obj.ID.Equals(ID) && obj.Type.Equals(Type);
            return answer;
        }
    }
}
