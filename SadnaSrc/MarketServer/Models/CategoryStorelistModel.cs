using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    public class CategoryStorelistModel:StoreItemModel
    {
	    public CategoryItem[] Items { get; set; }
	    public CategoryStorelistModel(int systemId, string state, string message, string store,string[] itemData) : base(systemId, state, message, store)
	    {
		    Items = new CategoryItem[itemData.Length];
		    for (int i = 0; i < Items.Length; i++)
		    {
			    Items[i] = new CategoryItem(itemData[i]);
		    }
	    }

	    public class CategoryItem
	    {
		    public string Name { get; set; }

		    public CategoryItem(string data)
		    {
			    Name = data;

		    }
	    }
	}
}
