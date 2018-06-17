using System;
using Castle.Core.Internal;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class EditProductSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        

        public EditProductSlave(string storename, IUserSeller storeManager, IStoreDL storeDL) : base(storename, storeManager, storeDL)
        {
        }

        public void EditProduct(string productName,string productNewName, string basePrice, string description)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit product in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
                Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
                checkifProductExists(product);
	            EditAllProductFields(productNewName, basePrice, description, ref product);
	            answer = new StoreAnswer(StoreEnum.Success, "Product has been updated!");
				CheckIfNoLegalFound();
                DataLayerInstance.EditProductInDatabase(product);
            }
            catch (StoreException exe)
            {
                answer =  new StoreAnswer((StoreEnum)exe.Status, exe.GetErrorMessage());
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
        }

	    private void EditAllProductFields(string productNewName, string basePrice, string description, ref Product product)
	    {
		    if (productNewName!=null && product.Name != productNewName)
		    {
			    CheckIfProductNameAvailable(productNewName);
			    product.Name = productNewName;
			}

		    if (basePrice != null)
		    {
			    CheckIfProductPriceLegal(basePrice);
			    product.BasePrice = double.Parse(basePrice);
			}

		    if (description != null)
		    {
			    CheckIfDescriptionLegal(description);
			    product.Description = description;
			}
			
		}

       
        private void CheckIfProductNameAvailable(string name)
        {
            Product prod = DataLayerInstance.GetProductByNameFromStore(_storeName, name);
	        if (prod != null || name.IsNullOrEmpty())
	        {
		        throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "product name must be uniqe per shop");
			}
		}

	    private void CheckIfProductPriceLegal(string basePrice)
	    {
		    if (basePrice.IsNullOrEmpty() || !double.TryParse(basePrice, out var newBasePrice) || newBasePrice <= 0)
		    {
			    throw new StoreException(StoreEnum.UpdateProductFail, "value is not leagal");
		    }
		}

	    private void CheckIfDescriptionLegal(string description)
	    {
		    if (description.IsNullOrEmpty())
		    {
			    throw new StoreException(StoreEnum.UpdateProductFail, "Illegal description given");
			}
	    }

	    private void CheckIfNoLegalFound()
	    {
		    if (answer != null) return;
		    MarketLog.Log("StoreCenter", "no leagal attrebute or founed non-leagal value");
		    throw new StoreException(StoreEnum.UpdateProductFail, "no leagal attrebute found");
	    }
	}
}