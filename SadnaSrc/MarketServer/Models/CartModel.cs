using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MarketServer.Models
{
    public class CartModel: UserModel
    {
        public CartItemModel[] Items { get; set; }
        public CartModel(int systemId, string state,string[] itemData) : base(systemId,state)
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
                var dataParam = data.Split(':');
                Name = dataParam[1].Substring(1, dataParam[1].Length - "Store".Length - 3 * " ".Length);
                Store = dataParam[2].Substring(1,dataParam[2].Length - "Quantity".Length - 3 * " ".Length);
                Quantity = dataParam[3].Substring(1 , dataParam[3].Length - "UnitPrice".Length - 3 * " ".Length);
                UnitPrice = dataParam[4].Substring(1 , dataParam[4].Length - "FinalPrice".Length - 3 * " ".Length);
                FinalPrice = dataParam[5];
            }
        }

    }


}
