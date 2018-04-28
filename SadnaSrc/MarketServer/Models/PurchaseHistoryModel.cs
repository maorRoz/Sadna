using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class PurchaseHistoryModel : UserModel
    {
		public PurchaseItemModel[] Items { get; set; }
		public PurchaseHistoryModel(int systemId, string state, string subject, string[] history) : base(systemId, state,null)
        {
			Items = new PurchaseItemModel[history.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new PurchaseItemModel(history[i]);
			}
		}

		public class PurchaseItemModel
		{
			public string User { get; set; }
			public string Product { get; set; }
			public string Store { get; set; }
			public string Sale { get; set; }
			public string Quantity { get; set; }
			public string Price { get; set; }
			public string Date { get; set; }

			public PurchaseItemModel(string data)
			{
				var dataParam = data.Split(new[] { "User: ", " Product: ", " Store: ", " Sale: ", " Quantity: "," Price: ", " Date: " }, StringSplitOptions.RemoveEmptyEntries);
				User = dataParam[0];
				Product = dataParam[1];
				Store = dataParam[2];
				Sale = dataParam[3];
				Quantity = dataParam[4];
				Price = dataParam[5];
				Date = dataParam[6];
			}
		}


	}
}
