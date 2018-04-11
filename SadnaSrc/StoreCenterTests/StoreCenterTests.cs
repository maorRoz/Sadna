using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests
{
    [TestClass]
    class OpenStoreTests
    {
        private MarketYard market;
        private ModuleGlobalHandler handler;
        [TestInitialize]
        public void BuildStore()
        {
            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
        }
        [TestMethod]
        public void CheckName()
        {
            Store A = new Store("SX", "name1test", "here");
            Store find = handler.DataLayer.GetStoreByName("SX");
            Assert.IsNull(find);
            Assert.IsTrue(handler.CheckStoreNameUnique("SX"));
            handler.DataLayer.AddStore(A);
            find = handler.DataLayer.GetStoreByName("SX");
            Assert.IsFalse(handler.CheckStoreNameUnique("SX"));
            handler.DataLayer.RemoveStore(A);
        }
    }
}
