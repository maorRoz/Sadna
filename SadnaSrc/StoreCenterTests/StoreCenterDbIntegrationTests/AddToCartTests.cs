using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    //TODO: maybe remove these tests

    [TestClass]
    public class AddToCartTests
    {
        private MarketYard market;
        private StoreDL handler;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StoreDL.Instance;
            userService = market.GetUserService();
        }

        [TestMethod]
        public void AddToCartWhenStoreNotExists()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.GetStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.AddProductToCart("newStoreName", "ppp", 6);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenHasNoPermission()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            Store find = handler.GetStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 6);
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenProductIsNotExistsInStore()
        {

            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.GetStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOBO", 6);
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void AddToCartWhenQuantityisTooBig()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.GetStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 999999);
            Assert.AreEqual((int)StoreEnum.QuantityIsTooBig, ans.Status);
        }
        
        [TestMethod]
        public void AddToCartSuccess()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.GetStorebyName("X");
            MarketAnswer ans = liorSession.AddProductToCart("X", "BOX", 1);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
