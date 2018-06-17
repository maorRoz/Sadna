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

        public class EditCategoryDisocuntTests
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
            public void noStore()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "End Date", "12/05/2019");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
            }
            [TestMethod]
            public void NoPermission()
            {

                userService.EnterSystem();
                userService.SignIn("Big Smoke", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "End Date", "12/05/2019");
                Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
            }

            [TestMethod]
            public void NoCategory()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("noCategory", "End Date", "12/05/2019");
                Assert.AreEqual((int)StoreEnum.CategoryNotExistsInSystem, ans.Status);
            }

            [TestMethod]
            public void BadDiscountDates1()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "Start Date", "12/05/2119");
                Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
            }

            [TestMethod]
            public void DiscountAmountIs100()
            {

                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "DiscountAmount", "100");
                Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
            }

            [TestMethod]
            public void DisocuntAmountIsBigger100()
            {

            userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "DiscountAmount", "140");
                Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
            }

            [TestMethod]
            public void DisocuntAmountIsNegative()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "DiscountAmount", "-100");
                Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, ans.Status);
            }

            [TestMethod]
            public void DisocuntAmountIsZero()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "DiscountAmount", "0");
                Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, ans.Status);
        }

            [TestMethod]
            public void AddCategoryDiscountSuccess()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Cards", "DiscountAmount", "20");
                Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }

            [TestMethod]
            public void AddCategoryDiscountBadInputFail()
            {
                userService.EnterSystem();
                userService.SignIn("Arik1", "123");
                StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "T");
                MarketAnswer ans = liorSession.EditCategoryDiscount("MTG_Ca'rds", "DiscountAmount", "20");
                Assert.AreEqual((int)DiscountStatus.BadInput, ans.Status);
            }


        [TestCleanup]
            public void CleanUpOpenStoreTest()
            {
                MarketDB.Instance.CleanByForce();
                MarketYard.CleanSession();
            }

}
}
