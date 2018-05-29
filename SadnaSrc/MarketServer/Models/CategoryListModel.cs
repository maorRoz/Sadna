using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class CategoryListModel:UserModel
    {
	    public CategoryItem[] Items { get; set; }
	    public CategoryListModel(int systemId, string state, string message, string[] itemData) : base(systemId, state, message)
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
