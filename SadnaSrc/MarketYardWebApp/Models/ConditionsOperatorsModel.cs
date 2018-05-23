using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;

namespace MarketYardWebApp.Models
{
    public class ConditionsOperatorsModel:UserModel
    {
	    public string[] Operators;
	    public string[] Conditions;
	    public ConditionsOperatorsModel(int systemId, string state, string message, string[] operators, string[] conditions) : base(systemId, state, message)
	    {
		    Operators = operators;
		    Conditions = conditions;
	    }
    }
}
