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

namespace StoreCenterTests
{
    [TestClass]
    public class LotteryEndToEnd
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private ModuleGlobalHandler handler;
        IUserService userService;
        IOrderService orderService;
        IUserService otherUser;
        IStoreManagementService managementService;
        public LotterySaleManagmentTicket LotteryToDelete;
        public LinkedList<LotteryTicket> tickets;

        [TestInitialize]
        public void BuildStore()
        {

            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
            userService = market.GetUserService();
            otherUser = market.GetUserService();
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            managementService = (StoreManagementService)market.GetStoreManagementService(userService, "T");
            Product P = new Product("P10000", "name", 100, "ds");
            ProductToDelete = new StockListItem(1, P, null, PurchaseEnum.Lottery, "S7");
            LotteryToDelete = new LotterySaleManagmentTicket("L100", "T", P, DateTime.Parse("31/12/2017"), DateTime.Parse("31/12/2020"));
            handler.DataLayer.AddStockListItemToDataBase(ProductToDelete);
            handler.DataLayer.AddLottery(LotteryToDelete);
            tickets = new LinkedList<LotteryTicket>();
        }
        [TestMethod]
        public void LotteryEndToEndNoLotto()
        {
            orderService = market.GetOrderService(ref userService);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            orderService.BuyLotteryTicket("name", "T", 1, 50);
            tickets = handler.DataLayer.getAllTickets("L100");
            Assert.AreEqual(1, tickets.Count);
            LotteryTicket ticket = tickets.First();
            Assert.AreEqual(LotteryTicketStatus.Waiting, ticket.myStatus);
        }
        [TestMethod]
        public void LotteryEndToEndOneWinner()
        {
            orderService = market.GetOrderService(ref userService);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            orderService.BuyLotteryTicket("name", "T", 1, 100);
            tickets = handler.DataLayer.getAllTickets("L100");
            Assert.AreEqual(1, tickets.Count);
            LotteryTicket ticket = tickets.First();
            Assert.AreEqual(LotteryTicketStatus.Winning, ticket.myStatus);
        }
        [TestMethod]
        public void LotteryEndToEndCancelLotto()
        {
            orderService = market.GetOrderService(ref userService);
            ((OrderService)orderService).LoginBuyer("Arik3", "123");
            orderService.BuyLotteryTicket("name", "T", 1, 50);
            tickets = handler.DataLayer.getAllTickets("L100");
            Assert.AreEqual(1, tickets.Count);
            LotteryTicket ticket = tickets.First();
            Assert.AreEqual(LotteryTicketStatus.Waiting, ticket.myStatus);
            managementService.ChangeProductPurchaseWayToImmediate("name");
            LinkedList<LotteryTicket> noLotterys = handler.DataLayer.getAllTickets("L100");
            Assert.AreEqual(0, noLotterys.Count);
            
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            if (ProductToDelete != null)
            {
                handler.DataLayer.RemoveStockListItem(ProductToDelete);
            }
            if (LotteryToDelete != null)
            {
                handler.DataLayer.RemoveLottery(LotteryToDelete);
            }
            orderService.CleanSession();
            userService.CleanSession();
            managementService.CleanSession();
            otherUser.CleanSession();
            MarketYard.CleanSession();
        }
    }
}