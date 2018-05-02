using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.FullCycle_Integration
{
    [TestClass]
    public class LotteryEndToEnd
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private StoreDL handler;
        IUserService userService;
        IOrderService orderService;
        IUserService otherUser;
        IStoreManagementService managementService;
        public LotterySaleManagmentTicket LotteryToDelete;
        public LinkedList<LotteryTicket> tickets;

        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StoreDL.GetInstance();
            userService = market.GetUserService();
            otherUser = market.GetUserService();
            userService.EnterSystem();
            otherUser.EnterSystem();
            userService.SignIn("Arik1", "123");
            managementService = (StoreManagementService)market.GetStoreManagementService(userService, "T");
            Product P = new Product("P10000", "name", 100, "ds");
            ProductToDelete = new StockListItem(1, P, null, PurchaseEnum.Lottery, "S7");
            LotteryToDelete = new LotterySaleManagmentTicket("L100", "T", P, DateTime.Parse("31/12/2017"), DateTime.Parse("31/12/2020"));
            handler.AddStockListItemToDataBase(ProductToDelete);
            handler.AddLottery(LotteryToDelete);
            tickets = new LinkedList<LotteryTicket>();
        }
        [TestMethod]
        public void LotteryEndToEndNoLotto()
        {
            orderService = market.GetOrderService(ref otherUser);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            orderService.BuyLotteryTicket("name", "T", 1, 50);
            tickets = handler.GetAllTickets("L100");
            Assert.AreEqual(1, tickets.Count);
            LotteryTicket ticket = tickets.First();
            Assert.AreEqual(LotteryTicketStatus.Waiting, ticket.myStatus);
        }
        [TestMethod]
        public void LotteryEndToEndOneWinner()
        {
            orderService = market.GetOrderService(ref otherUser);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            orderService.BuyLotteryTicket("name", "T", 1, 100);
            tickets = handler.GetAllTickets("L100");
            Assert.AreEqual(1, tickets.Count);
            LotteryTicket ticket = tickets.First();
            Assert.AreEqual(LotteryTicketStatus.Winning, ticket.myStatus);
        }
        [TestMethod]
        public void LotteryEndToEndCancelLotto()
        {
            
            orderService = market.GetOrderService(ref otherUser);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            orderService.BuyLotteryTicket("name", "T", 1, 50);
            tickets = handler.GetAllTickets("L100");
            Assert.AreEqual(1, tickets.Count);
            LotteryTicket ticket = tickets.First();
            Assert.AreEqual(LotteryTicketStatus.Waiting, ticket.myStatus);
            managementService.ChangeProductPurchaseWayToImmediate("name");
            LinkedList<LotteryTicket> noLotterys = handler.GetAllTickets("L100");
            Assert.AreEqual(0, noLotterys.Count);
            
        }
        [TestMethod]
        public void LotteryEndToEndPurchesIlligalValueZeroMouney()
        {
            orderService = market.GetOrderService(ref otherUser);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            MarketAnswer ans = orderService.BuyLotteryTicket("name", "T", 1, 0);
            Assert.AreEqual(ans.Status, (int)OrderStatus.InvalidCoupon);

        }
        [TestMethod]
        public void LotteryEndToEndPurchesIlligalValueNegativeMouney()
        {
            orderService = market.GetOrderService(ref otherUser);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            MarketAnswer ans = orderService.BuyLotteryTicket("name", "T", 1, -5);
            Assert.AreEqual(ans.Status, (int)OrderStatus.InvalidCoupon);
        }

        [TestMethod]
        public void LotteryEndToEndPurchesIlligalValueOverMouney()
        {
            orderService = market.GetOrderService(ref otherUser);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            MarketAnswer ans = orderService.BuyLotteryTicket("name", "T", 1, 900000);
            Assert.AreEqual(ans.Status, (int)OrderStatus.InvalidCoupon);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            orderService.CleanSession();
            userService.CleanSession();
            managementService.CleanSession();
            otherUser.CleanSession();
            MarketYard.CleanSession();
        }
    }
}