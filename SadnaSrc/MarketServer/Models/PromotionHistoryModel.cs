using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class PromotionHistoryModel : UserModel
    {
        public PromotionItemModel[] Items { get; set; }
        public string Store { get; set; }
        public PromotionHistoryModel(int systemId, string state,string store, string[] history) : base(systemId, state, null)
        {
            Store = store;
            Items = new PromotionItemModel[history.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new PromotionItemModel(history[i]);
            }
        }

        public class PromotionItemModel
        {
            public string Promoter { get; set; }
            public string Promoted { get; set; }
            public string Permissions { get; set; }
            public string Date { get; set; }
            public string Description { get; set; }

            public PromotionItemModel(string data)
            {
                var dataParam = data.Split(new[] { "Store: ", " Promoter: ", " Promoted: ", " Permissions: ", " Date: ", " Description: "}, StringSplitOptions.RemoveEmptyEntries);
                Promoter = dataParam[1];
                Promoted = dataParam[2];
                Permissions = dataParam[3];
                Date = dataParam[4];
                Description = dataParam[5];
            }
        }
    }
}
