using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
	public class GetProductInfoSlave: AbstractStoreCenterSlave
	{
		public MarketAnswer Answer;
		public GetProductInfoSlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName,
			storeManager, storeDL)
		{

		}

		public void GetProductInfo(string productName)
			{
				try
				{
					MarketLog.Log("StoreCenter", "trying to view product's information");
                    checkIfStoreExistsAndActive();
					MarketLog.Log("StoreCenter", " store exists");
					MarketLog.Log("StoreCenter", " check if has premmision to edit products");
					_storeManager.CanManageProducts();
					MarketLog.Log("StoreCenter", " has premmission");
					MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
					Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
					checkifProductExists(product);
					string productInfo = product.ToString();
					string[] result = {productInfo};
					MarketLog.Log("StoreCenter", "info gained");
					Answer = new StoreAnswer(ViewProductInfoStatus.Success, "Product info has been successfully granted!", result);
				}
				catch (StoreException e)
				{
					MarketLog.Log("StoreCenter", "");
					Answer = new StoreAnswer((ViewProductInfoStatus)e.Status, "Something is wrong with viewing " + productName +
					                                                    " info!");
				}
				catch (DataException e)
				{
					Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
				}
				catch (MarketException)
				{
					MarketLog.Log("StoreCenter", "no premission");
					Answer = new StoreAnswer(ViewProductInfoStatus.NoAuthority,
						"User validation as valid customer has been failed . only valid users can browse market!");
				}
			}
			
		}
	}
