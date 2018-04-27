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
                Name = dataParam[1];
                Store = dataParam[3];
                Quantity = dataParam[5];
            //    UnitPrice = dataParam[7];
            //    FinalPrice = dataParam[9];
            }
        }

    }


}
