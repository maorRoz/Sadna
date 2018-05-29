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

        public class AddCategoryDiscountTests
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
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 50);
                Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
            }
            [TestMethod]
            public void NoPermission()
            {

                userService.EnterSystem();
                userService.SignIn("Big Smoke", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 50);
                Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
            }

            [TestMethod]
            public void NoCategory()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("noCategory", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 50);
                Assert.AreEqual((int)StoreEnum.CategoryNotExistsInSystem, ans.Status);
            }
        [TestMethod]
        public void AlreadyHasDiscount()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
            MarketAnswer ans = liorSession.AddCategoryDiscount("MTG_Cards", DateTime.Parse("01/01/2019"),
                DateTime.Parse("31/01/2019"), 50);
            Assert.AreEqual((int)StoreEnum.CategoryDiscountAlreadyExistsInStore, ans.Status);
        }

        [TestMethod]
            public void BadDiscountDates1()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("31/01/2019"), DateTime.Parse("01/01/2019"),
                    50);
                Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
            }

            [TestMethod]
            public void DiscountAmountIs100()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 100);
                Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
            }

            [TestMethod]
            public void DisocuntAmountIsBigger100()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 105);
                Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
            }

            [TestMethod]
            public void DisocuntAmountIsNegative()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), -100);
                Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, ans.Status);
            }

            [TestMethod]
            public void DisocuntAmountIsZero()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 0);
                Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, ans.Status);
            }

            [TestMethod]
            public void AddCategoryDiscountSuccess()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.AddCategoryDiscount("WanderlandItems", DateTime.Parse("01/01/2019"),
                    DateTime.Parse("31/01/2019"), 20);
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
