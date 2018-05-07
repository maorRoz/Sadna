using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;


namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class RemoveProductFromCategoryTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;



        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
        }

        [TestMethod]
        public void RemoveProductFromCategoryWhenStoreNotExists()
        {
            RemoveProductFromCategorySlave slave = new RemoveProductFromCategorySlave("noStore", userService.Object, handler.Object);
            slave.RemoveProductFromCategory("WanderlandItems", "OnePunchManPoster");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
        }

        [TestMethod]
        public void RemoveProductFromCategoryWhenHasNoPremmision()
        {
            RemoveProductFromCategorySlave slave = new RemoveProductFromCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(9, "bla"));
            slave.RemoveProductFromCategory("WanderlandItems", "OnePunchManPoster");
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveProductFromCategoryWhenCategoryNotExists()
        {
            RemoveProductFromCategorySlave slave = new RemoveProductFromCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            slave.RemoveProductFromCategory("BB", "OnePunchManPoster");
            Assert.AreEqual((int)StoreEnum.CategoryNotExistsInStore, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveProductFromWhenProductNotExists()
        {
            RemoveProductFromCategorySlave slave = new RemoveProductFromCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            handler.Setup(x => x.GetCategoryByName("WanderlandItems"))
                .Returns(new Category("C1", "WanderlandItems"));
            slave.RemoveProductFromCategory("WanderlandItems", "OnePunchManPoster");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveProductFromWhenProductNotInCategory()
        {
            RemoveProductFromCategorySlave slave = new RemoveProductFromCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            handler.Setup(x => x.GetCategoryByName("WanderlandItems"))
                .Returns(new Category("C1", "WanderlandItems"));
            handler.Setup(x => x.GetProductByNameFromStore("T", "OnePunchManPoster"))
                .Returns(new Product("S21", "OnePunchManPoster", 10, "bla"));
            LinkedList<Product> expected = new LinkedList<Product>();
            handler.Setup(x => x.GetAllCategoryProducts("C1")).Returns(expected);
            slave.RemoveProductFromCategory("WanderlandItems", "OnePunchManPoster");
            Assert.AreEqual((int)StoreEnum.ProductNotInCategory, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveProductFromCategorySuccess()
        {
            RemoveProductFromCategorySlave slave = new RemoveProductFromCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            handler.Setup(x => x.GetCategoryByName("WanderlandItems"))
                .Returns(new Category("C1", "WanderlandItems"));
            handler.Setup(x => x.GetProductByNameFromStore("T", "Fraid Egg")).Returns(new Product("S21", "Fraid Egg", 10, "bla"));
            LinkedList<Product> expected = new LinkedList<Product>();
            expected.AddLast(new Product("S21", "Fraid Egg", 10, "bla"));
            handler.Setup(x => x.GetAllCategoryProducts("C1")).Returns(expected);
            slave.RemoveProductFromCategory("WanderlandItems", "Fraid Egg");
            Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);
        }
        [TestCleanup]
        public void CleanUpTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
