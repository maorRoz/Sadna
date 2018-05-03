using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests
{
    [TestClass]
    public class OpenStoreTestsMock
    {
        private MarketYard market;
        public Store toDeleteStore;
        private Mock<I_StoreDL> handler;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = new Mock<I_StoreDL>();
            userService = market.GetUserService();
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelPass()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService, handler.Object);
            liorSession.LoginShoper("Arik3", "123");
            Store find = handler.Object.GetStorebyName("newStoreName");
            Assert.IsNull(find);
            handler.Setup(x => x.GetStorebyName("newStoreName")).Returns(new Store("newStoreName", ""));
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            find = handler.Object.GetStorebyName("newStoreName");
            Assert.IsNotNull(find);
            Assert.AreEqual((int)OpenStoreStatus.Success, ans.Status);
        }
    //    [TestMethod]
/*        public void OpenStoreInterfaceLevelFail()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            Store find = handler.GetStorebyName("newStoreName");
            Assert.IsNull(find);
            MarketAnswer ans = liorSession.OpenStore("newStoreName", "adress");
            Assert.AreEqual((int)OpenStoreStatus.InvalidUser, ans.Status);
        }*/


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
