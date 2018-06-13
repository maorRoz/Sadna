using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class ViewPromotionHistoryTest
    {
        private IUserService userService;
        private MarketYard marketSession;
        private List<string> expectedT;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userService = marketSession.GetUserService();
            userService.EnterSystem();
            expectedT = new List<string>
            {
                "Store: T Promoter: Arik1 Promoted: Arik1 Permissions: StoreOwner Date: 01/01/2018 Description: T has been opened",
                "Store: T Promoter: Arik1 Promoted: CJ Permissions: StoreOwner Date: 01/01/2018 Description: Regular promotion"
            };
        }
        [TestMethod]
        public void GetAllHistoryOfStoreTest()
        {
            userService.SignIn("Arik1", "123");
            var storeManagementSession = marketSession.GetStoreManagementService(userService, "T");
            var answer = storeManagementSession.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success,answer.Status);
            ComparePromotionHistory(expectedT.ToArray(),answer.ReportList);
      
        }

        [TestMethod]
        public void GetHistoryAfterSingleStoreManagerPromotionTest()
        {
            userService.SignIn("Arik1", "123");
            var storeManagementSession = marketSession.GetStoreManagementService(userService, "T");
            var answer = storeManagementSession.PromoteToStoreManager("Big Smoke", "ManageProducts");
            Assert.AreEqual((int)StoreEnum.Success,answer.Status);
            answer = storeManagementSession.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success,answer.Status);
            expectedT.Add("Store: T Promoter: Arik1 Promoted: Big Smoke Permissions: ManageProducts Date: "
                          + DateTime.Now.ToString("dd/MM/yyyy") + " Description: Regular promotion");
            ComparePromotionHistory(expectedT.ToArray(),answer.ReportList);
        }

        [TestMethod]
        public void GetHistoryAfterMultiPromotionTest()
        {
            userService.SignIn("Arik1", "123");
            var storeManagementSession = marketSession.GetStoreManagementService(userService, "T");
            var answer = storeManagementSession.PromoteToStoreManager("Big Smoke", "ManageProducts,DeclareDiscountPolicy");
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            answer = storeManagementSession.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            expectedT.Add("Store: T Promoter: Arik1 Promoted: Big Smoke Permissions: ManageProducts,DeclareDiscountPolicy Date: "
                          + DateTime.Now.ToString("dd/MM/yyyy") + " Description: Regular promotion");
            ComparePromotionHistory(expectedT.ToArray(), answer.ReportList);
        }

        [TestMethod]
        public void GetHistroyAfterMultiPromotionWithStoreOwnerTest()
        {
            userService.SignIn("Arik1", "123");
            var storeManagementSession = marketSession.GetStoreManagementService(userService, "T");
            var answer = storeManagementSession.PromoteToStoreManager("Big Smoke", "ManageProducts,DeclareDiscountPolicy,StoreOwner");
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            answer = storeManagementSession.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            expectedT.Add("Store: T Promoter: Arik1 Promoted: Big Smoke Permissions: StoreOwner Date: "
                          + DateTime.Now.ToString("dd/MM/yyyy") + " Description: Regular promotion");
            ComparePromotionHistory(expectedT.ToArray(), answer.ReportList);
        }

        [TestMethod]

        public void GetHistoryAfterOpeningStoreTest()
        {
            userService.SignIn("Arik1", "123");
            var shoppingSession = marketSession.GetStoreShoppingService(ref userService);
            var answer =shoppingSession.OpenStore("HistoryShop","blah");
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            var storeManagementSession = marketSession.GetStoreManagementService(userService, "HistoryShop");
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            answer = storeManagementSession.PromoteToStoreManager("Big Smoke", "ManageProducts,DeclareDiscountPolicy");
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            answer = storeManagementSession.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success, answer.Status);
            var expected = new[]
            {
                "Store: HistoryShop Promoter: Arik1 Promoted: Arik1 Permissions: StoreOwner Date: "
                +DateTime.Now.ToString("dd/MM/yyyy")+" Description: HistoryShop has been opened",
                "Store: HistoryShop Promoter: Arik1 Promoted: Big Smoke " +
                "Permissions: ManageProducts,DeclareDiscountPolicy Date: "+DateTime.Now.ToString("dd/MM/yyyy")+
                " Description: Regular promotion"
            };

            ComparePromotionHistory(expected,answer.ReportList);

        }

        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void ComparePromotionHistory(string[] expected, string[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
