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
    [TestClass]
    public class OpenStoreTests
    {
        private MarketYard market;
        public Store toDeleteStore;
        private IStoreDL handler;
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
        public void CheckName()
        {
            Store store = new Store("SX", "name1test", "here");
            Store find = handler.GetStorebyName("name1test");
            Assert.IsNull(find);
            Assert.IsFalse(handler.IsStoreExistAndActive("name1test"));
            toDeleteStore = store;
            handler.AddStore(store);
            Assert.IsTrue(handler.IsStoreExistAndActive("name1test"));
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelSameName()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            toDeleteStore = handler.GetStorebyName("newStoreName");
            ans = liorSession.OpenStore("newStoreName", "adress");
            Assert.AreEqual((int)OpenStoreStatus.AlreadyExist, ans.Status);
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelPass()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            Store find = handler.GetStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            find = handler.GetStorebyName("newStoreName");
            Assert.IsNotNull(find);
            toDeleteStore = handler.GetStorebyName("newStoreName");
            Assert.IsNotNull(toDeleteStore);
            Assert.AreEqual((int)OpenStoreStatus.Success, ans.Status);
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelFail()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.GetStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            Assert.AreEqual((int)OpenStoreStatus.InvalidUser, ans.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
