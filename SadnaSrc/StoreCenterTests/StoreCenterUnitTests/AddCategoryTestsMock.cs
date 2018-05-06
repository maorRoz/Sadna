﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;


namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class AddCategoryTestsMock
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
        public void AddCategoryWhenStoreNotExists()
        {
            AddCategorySlave slave = new AddCategorySlave("noStore", userService.Object, handler.Object);
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);

        }
        [TestMethod]
        public void AddCategoryWhenHasNoPremmision()
        {

            AddCategorySlave slave = new AddCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(9, "bla"));
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.NoPremmision, slave.Answer.Status);
        }
        [TestMethod]
        public void AddCategoryWhenCategoryAlreadyExists()
        {
            AddCategorySlave slave = new AddCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            handler.Setup(x => x.getCategoryByName("items")).Returns(new Category("items"));
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.CategoryExistsInStore, slave.Answer.Status);
        }
        [TestMethod]
        public void AddCategorySuccess()
        {
            AddCategorySlave slave = new AddCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}