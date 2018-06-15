using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class ProductInStoreCategoriesModel:UserModel
    {
	    public readonly string Store;
	    public readonly string Product;
		public CategoryListModel.CategoryItem[] Items { get; set; }

	    public ProductInStoreCategoriesModel(int systemId, string state, string message, string store, string product, string[] itemData) : base(systemId, state, message)
	    {
		    Store = store;
		    Product = product;
		    Items = new CategoryListModel.CategoryItem[itemData.Length];
		    for (int i = 0; i < Items.Length; i++)
		    {
			    Items[i] = new CategoryListModel.CategoryItem(itemData[i]);
		    }
		}
	}
}
