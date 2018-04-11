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
    public class OpenStoreTests
    {
        private MarketYard market;
        public Store toDeleteStore;
        private ModuleGlobalHandler handler;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            
            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
            userService = market.GetUserService();
        }
        [TestMethod]
        public void CheckName()
        {
            Store store = new Store("SX", "name1test", "here");
            Store find = handler.DataLayer.getStorebyName("name1test");
            Assert.IsNull(find);
            Assert.IsFalse(handler.DataLayer.IsStoreExist("name1test"));
            toDeleteStore = store;
            handler.DataLayer.AddStore(store);
            Assert.IsTrue(handler.DataLayer.IsStoreExist("name1test"));
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelSameName()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            toDeleteStore = handler.DataLayer.getStorebyName("newStoreName");
            ans = liorSession.OpenStore("newStoreName", "adress");
            Assert.AreEqual((int)OpenStoreStatus.AlreadyExist, ans.Status);
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelPass()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            Store find = handler.DataLayer.getStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            find = handler.DataLayer.getStorebyName("newStoreName");
            Assert.IsNotNull(find);
            toDeleteStore = handler.DataLayer.getStorebyName("newStoreName");
            Assert.IsNotNull(toDeleteStore);
            Assert.AreEqual((int)OpenStoreStatus.Success, ans.Status);
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelFail()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.DataLayer.getStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            Assert.AreEqual((int)OpenStoreStatus.InvalidUser, ans.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            if (toDeleteStore != null)
            {
                handler.DataLayer.RemoveStore(toDeleteStore);
            }
            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
