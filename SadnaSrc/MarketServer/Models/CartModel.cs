using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MarketServer.Models
{
    public class CartModel: UserModel
    {
        public CartItemModel[] Items { get; set; }
        public CartModel(int systemId, string state,string message,string[] itemData) : base(systemId,state,message)
        {
            Items = new CartItemModel[itemData.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new CartItemModel(itemData[i]);
            }
        }

        public class CartItemModel
        {
            public string Name { get; set; }
            public string Store { get; set; }
            public string Quantity { get; set; }
            public string UnitPrice { get; set; }
            public string FinalPrice { get; set; }

            public CartItemModel(string data)
            {
                var dataParam = data.Split(new[]{"Name : "," Store : "," Quantity : "," Unit Price : "," Final Price : "},StringSplitOptions.RemoveEmptyEntries);
                Name = dataParam[0];
                Store = dataParam[1];
                Quantity = dataParam[2];
                UnitPrice = dataParam[3];
                FinalPrice = dataParam[4];
            }
        }

    }


}
