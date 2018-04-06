using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UseCaseUnitTest
{
    [TestClass]
    public class UseCase1_6_1_Test
    {
        private UserService userServiceGuestSession;
        private UserService userServiceRegisteredSession;
        private MarketYard marketSession;
        private CartItem item1;
        private CartItem item2;
        private List<CartItem> expected;


        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceGuestSession = (UserService)marketSession.GetUserService();
            userServiceGuestSession.EnterSystem();
            userServiceRegisteredSession = null;
            expected = new List<CartItem>();
            item1 = new CartItem("Health Potion", "X", 1, 5.0, "Immediate");
            item2 = new CartItem("Health Potion", "Y", 2, 0.5, "Immediate");

        }

        [TestMethod]
        public void IncreaseCartItemGuestTest()
        {
            AddAllItems(userServiceGuestSession);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceGuestSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", 3).Status);
            item1.ChangeQuantity(3);
            Assert.AreEqual(item1, userServiceGuestSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
        }

        [TestMethod]
        public void DecreaseCartItemGuestTest()
        {
            AddAllItems(userServiceGuestSession);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceGuestSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -1).Status);
            item1.ChangeQuantity(-1);
            Assert.AreEqual(item1, userServiceGuestSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
        }

        [TestMethod]
        public void IncreaseCartItemToRegisteredTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddItem1(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", 3).Status);
            item1.ChangeQuantity(3);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            userServiceRegisteredSession.SignUp("MaorEditItem1", "no-where", "123");
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            AddItem2(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", 2).Status);
            item2.ChangeQuantity(2);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
        }

        [TestMethod]
        public void DecreaseCartItemToRegisteredTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddAllItems(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", 3).Status);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", 2).Status);
            item1.ChangeQuantity(3);
            item2.ChangeQuantity(2);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
            userServiceRegisteredSession.SignUp("MaorEditItem2", "no-where", "123");
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -1).Status);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -2).Status);
            item1.ChangeQuantity(-1);
            item2.ChangeQuantity(-2);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
        }

        [TestMethod]
        public void DecreaseCartItemToZeroTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddAllItems(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -1).Status);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -1).Status);
            item2.ChangeQuantity(-1);
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
            userServiceRegisteredSession.SignUp("MaorEditItem3", "no-where", "123");
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
            Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -1).Status);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
        }

        [TestMethod]
        public void DecreaseCartItemToNegativeTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddAllItems(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -2).Status);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -1).Status);
            item2.ChangeQuantity(-1);
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
            userServiceRegisteredSession.SignUp("MaorEditItem4", "no-where", "123");
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
            Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -2).Status);
            Assert.AreEqual(item1, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0, "Immediate"));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5, "Immediate"));
        }

        [TestMethod]
        public void DidntEnterTest()
        {
            userServiceRegisteredSession = (UserService)marketSession.GetUserService();
            Assert.AreEqual((int)EditCartItemStatus.DidntEnterSystem, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -2).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void NoItemToEditFoundTest1()
        {
            userServiceRegisteredSession = DoEnter();
            AddItem1(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -1).Status);
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -2).Status);
            userServiceRegisteredSession.SignUp("MaorEditItem5", "no-where", "123");
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -1).Status);
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -2).Status);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", 3).Status);

        }

        [TestMethod]
        public void NoItemToEditFoundTest2()
        {
            userServiceRegisteredSession = DoEnter();
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -1).Status);
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -2).Status);
            userServiceRegisteredSession.SignUp("MaorEditItem6", "no-where", "123");
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -1).Status);
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -2).Status);
            AddAllItems(userServiceRegisteredSession);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", 3).Status);
            Assert.AreEqual((int)EditCartItemStatus.Success, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", 2).Status);
            userServiceRegisteredSession.MarketUser.Cart.EmptyCart();
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("X", "Health Potion", 5.0, "Immediate", -4).Status);
            Assert.AreEqual((int)EditCartItemStatus.NoItemFound, userServiceRegisteredSession.EditCartItem("Y", "Health Potion", 0.5, "Immediate", -4).Status);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceGuestSession.CleanSession();
            userServiceRegisteredSession?.CleanSession();
            MarketYard.CleanSession();
        }

        private UserService DoEnter()
        {
            UserService userService = (UserService)marketSession.GetUserService();
            userService.EnterSystem();
            return userService;
        }

        private void AddItem1(UserService userService)
        {
            expected.Add(item1);
            userService.AddToCart("Health Potion", "X", 1, 5.0, "Immediate");
        }

        private void AddItem2(UserService userService)
        {
            expected.Add(item2);
            userService.AddToCart("Health Potion", "Y", 2, 0.5, "Immediate");
        }

        private void AddAllItems(UserService userService)
        {
            AddItem1(userService);
            AddItem2(userService);
        }


    }
}
