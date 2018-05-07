using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;

namespace MarketWeb.Models
{
	public class CheckBoxModel 
	{
		public string Name { get; set; }
		public bool IsChecked { get; set; }

	

	}

	public class CheckBoxListModel:StoreItemModel
	{
		public List<CheckBoxModel> Items { get; set; }

		public CheckBoxListModel(int systemId, string state, string message, string store) :base(systemId,state,message,store)
		{

		}

	}
}
