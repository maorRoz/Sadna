using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterUnitTests
    {
        [TestClass]

        public class RemoveCategoryDiscountTests
        {
            private MarketYard market;
            IUserService userService;
            [TestInitialize]
            public void BuildStore()
            {

                MarketDB.Instance.InsertByForce();
                market = MarketYard.Instance;
                userService = market.GetUserService();
                MarketYard.SetDateTime(new DateTime(2018, 4, 14));

            }
            [TestMethod]
            public void NoStore()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
                MarketAnswer ans = liorSession.RemoveCategoryDiscount("MTG_Cards");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
            }
            [TestMethod]
            public void NoPermission()
            {

                userService.EnterSystem();
                userService.SignIn("Big Smoke", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.RemoveCategoryDiscount("MTG_Cards");
                Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
            }

            [TestMethod]
            public void NoCategory()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.RemoveCategoryDiscount("NoCategory");
                Assert.AreEqual((int)StoreEnum.CategoryNotExistsInSystem, ans.Status);
            }
        
            [TestMethod]
            public void NoCategoryDiscount()
            {

                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.RemoveCategoryDiscount("WanderlandItems");
                Assert.AreEqual((int)StoreEnum.CategoryDiscountNotExistsInStore, ans.Status);
            }

            [TestMethod]
            public void RemoveDiscountSuccess()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.RemoveCategoryDiscount("MTG_Cards");
                Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            }

            [TestMethod]
            public void RemoveDiscountBadInputFail()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.RemoveCategoryDiscount("MT'G_Cards");
                Assert.AreEqual((int)StoreEnum.BadInput, ans.Status);
            }

        [TestCleanup]
            public void CleanUpOpenStoreTest()
            {
                MarketDB.Instance.CleanByForce();
                MarketYard.CleanSession();
            }


        }
    }
