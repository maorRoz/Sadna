﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    [DataContract]
    public class DataPoint
    {
	    public DataPoint(string x, int y)
	    {
		    this.X = x;
		    this.Y = y;
	    }

	    //Explicitly setting the name to be used while serializing to JSON.
	    [DataMember(Name = "x")]
	    public string X = null;

	    //Explicitly setting the name to be used while serializing to JSON.
	    [DataMember(Name = "y")]
	    public int? Y = null;
    }
}

