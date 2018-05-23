using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketYardWebApp.Models
{
	
	public class OptionsCheckBoxStoreModel:StoreItemModel
    {
	    public CheckBoxListModel Items;
	    public CheckBoxListModel SelectedOptions;

	    public OptionsCheckBoxStoreModel() : base(0, "", "", "")
	    {
		    
		}

		public OptionsCheckBoxStoreModel(int systemId, string state, string message, string store, CheckBoxListModel lst) : base(systemId, state, message, store)
	    {
		    Items = lst;
	    }
    }
}
