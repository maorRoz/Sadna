using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace DbRobustnessTests
{
    [TestClass]
    public class StoreCenterManagementApiNoDb
    {
        private IStoreManagementService storeManagingService;
        private MarketAnswer answer;
        [TestInitialize]
        public void BuildNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService = marketSession.GetUserService();
            answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
            answer = userService.SignIn("Arik1", "123");
            Assert.AreEqual((int)SignInStatus.Success, answer.Status);
            storeManagingService = marketSession.GetStoreManagementService(userService,"T");
            MarketDB.ToDisable = true;
        }

        [TestMethod]
        public void PromoteToStoreManagerNoDBTest()
        {
            answer = storeManagingService.PromoteToStoreManager("Arik2","StoreOwner");
            Assert.AreEqual((int)PromoteStoreStatus.NoDB, answer.Status);
        }
        [TestMethod]
        public void AddNewProductNoDBTest()
        {
            answer = storeManagingService.AddNewProduct("NoDB product", 10.0, "there is no db!", 3);
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void RemoveProductNoDBTest()
        {
            answer = storeManagingService.RemoveProduct("DeleteMy BOX");
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void EditProductNoDBTest()
        {
            answer = storeManagingService.EditProduct("DeleteMy BOX", "Name", "blah","NoDB DeleteMy BOX");
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void AddQuantityToProductNoDBTest()
        {
            answer = storeManagingService.AddQuanitityToProduct("DeleteMy BOX", 5);
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void AdNewLotteryNoDBTest()
        {
            answer = storeManagingService.AddNewLottery("NoDB lottery", 10,"no db lottery ok?",new DateTime(2018,1,1),new DateTime(2019,1,1));
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void AddProductToCategoryNoDBTest()
        {
            answer = storeManagingService.AddProductToCategory("DeleteMy BOX", "WanderlandItems");
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void RemoveProductFromCategoryNoDBTest()
        {
            answer = storeManagingService.RemoveProductFromCategory("Fraid Egg", "WanderlandItems");
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void AddDiscountToProductNoDBTest()
        {
            answer = storeManagingService.AddDiscountToProduct("Fraid Egg",new DateTime(2018,1,1),new DateTime(2019,1,1),5,"VISIBLE",true);
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void EditDiscountNoDBTest()
        {
            answer = storeManagingService.EditDiscount("DeleteMy BOX",null,false,null,new DateTime(2018,1,2).ToString("yyyy-MM-dd"),null, false);
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void RemoveDiscountFromProductNoDBTest()
        {
            answer = storeManagingService.RemoveDiscountFromProduct("DeleteMy BOX");
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewStoreHistoryNoDBTest()
        {
            answer = storeManagingService.ViewStoreHistory();
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestMethod]
        public void CloseStoreNoDBTest()
        {
            answer = storeManagingService.CloseStore();
            Assert.AreEqual((int)StoreEnum.NoDB, answer.Status);
        }

        [TestCleanup]
        public void CleanUpNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
