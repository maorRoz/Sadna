using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class PurchaseHistoryModel : UserModel
    {
        public PurchaseHistoryModel(int systemId, string state, string subject) : base(systemId, state,null)
        {

        }
    }
}
