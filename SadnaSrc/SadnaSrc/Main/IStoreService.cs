using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    public interface IStoreService
    {
        MarketAnswer OpenStore(User owner);

        MarketAnswer PromoteToOwner(Store store, User CurrentUser, User someoneToPromote);
        MarketAnswer PromoteToManager(Store store, User CurrentUser, User someoneToPromote);
        MarketAnswer CloseStore(Store store);
        MarketAnswer ChangeProductPurchesWayToImmidiate(Store store, Product product);
        MarketAnswer ChangeProductPurchesWayToLottery(Store store, Product product);
        MarketAnswer addDiscountToProduct_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer removeProduct(Store store, Product product);
    }
    public enum StoreEnum
    {
        Success,
        UpdateStockFail,
        OpenStoreFail,
        AddStoreOwnerFail,
        AddStoreManagerFail,
        CloseStoreFail
    }
}
