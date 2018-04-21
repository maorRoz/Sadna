using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests
{
    [TestClass]
    public class AddToCartTests
    {
        private MarketYard market;
        private ModuleGlobalHandler handler;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
            userService = market.GetUserService();
        }

        [TestMethod]
        public void AddToCartWhenStoreNotExists()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.AddProductToCart("newStoreName", "ppp", 6);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenHasNoPremmision()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            Store find = handler.DataLayer.getStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 6);
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenProductIsNotExistsInStore()
        {

            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOBO", 6);
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenQuantityisTooBig()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 999999);
            Assert.AreEqual((int)StoreEnum.QuantityIsTooBig, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenQuantityisZero()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 0);
            Assert.AreEqual((int)StoreEnum.quantityIsNegatie, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenQuantityisNegative()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", -1);
            Assert.AreEqual((int)StoreEnum.quantityIsNegatie, ans.Status);
        }
        [TestMethod]
        public void AddToCartSuccess()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 1);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
