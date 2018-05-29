using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace BlackBox.StoreBlackBoxTests
{
    [TestClass]
    public class UseCase3_8
    {
        private IUserBridge _userBridge;
        private IStoreManagementBridge _ownerStoreBridge;
        private IStoreShoppingBridge _storeShopping;

        [TestInitialize]

        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            _ownerStoreBridge = StoreManagementDriver.getBridge();
            _userBridge = UserDriver.getBridge();
            _userBridge.EnterSystem();
            _userBridge.SignUp("Pnina", "misholSusia", "852852", "77777777");
            _storeShopping = StoreShoppingDriver.getBridge();
            _storeShopping.GetStoreShoppingService(_userBridge.GetUserSession());
            _storeShopping.OpenStore("HistoryShop", "");
            _ownerStoreBridge.GetStoreManagementService(_userBridge.GetUserSession(),"HistoryShop");
        }


        [TestMethod]
        public void GetHistoryRecordsTest()
        {
            _ownerStoreBridge.PromoteToStoreManager("Big Smoke", "ManageProducts");
            var answer = _ownerStoreBridge.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success,answer.Status);
            var expected = new[]
            {
                "Store: HistoryShop Promoter: Pnina Promoted: Pnina Permissions: StoreOwner Date: "
                +DateTime.Now.ToString("dd/MM/yyyy")+" Description: HistoryShop has been opened",
                "Store: HistoryShop Promoter: Pnina Promoted: Big Smoke " +
                "Permissions: ManageProducts Date: "+DateTime.Now.ToString("dd/MM/yyyy")+
                " Description: Regular promotion"
            };
            var actual = answer.ReportList;
            Assert.AreEqual(expected.Length,actual.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestCleanup]

        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }
    }
}
