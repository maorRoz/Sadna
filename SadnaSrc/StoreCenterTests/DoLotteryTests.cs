﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests
{
    [TestClass]
    public class DoLotteryTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private ModuleGlobalHandler handler;
        IUserService userService;
        public LotterySaleManagmentTicket LotteryToDelete;
        public LinkedList<LotteryTicket> tickets;

        [TestInitialize]
        public void BuildStore()
        {

            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
            userService = market.GetUserService();
            Product P = new Product("P10000", "name", 100, "ds");
            ProductToDelete = new StockListItem(1, P, null, PurchaseEnum.Lottery, "S7");
            LotteryToDelete = new LotterySaleManagmentTicket("L100", "T", P, DateTime.Parse("31/12/2019"), DateTime.Parse("31/12/2020"));
            handler.DataLayer.AddStockListItemToDataBase(ProductToDelete);
            handler.DataLayer.AddLottery(LotteryToDelete);
            tickets = new LinkedList<LotteryTicket>();
        }
        [TestMethod]
        public void DolotteryNoMoeny()
        {
            LotteryToDelete.TotalMoneyPayed = 40;
            LotteryTicket obj = LotteryToDelete.Dolottery(0);
            Assert.IsNull(obj);
        }
        [TestMethod]
        public void DolotteryOneUser()
        {
            LotteryTicket expected = new LotteryTicket("T100", "L100", 0, 100, 100, handler.DataLayer.getUserIDFromUserName("Arik1"));
            tickets.AddLast(expected);
            handler.DataLayer.AddLotteryTicket(expected);
            LotteryToDelete.TotalMoneyPayed = 100;
            LotteryTicket find = LotteryToDelete.Dolottery();
            expected.myStatus = LotteryTicketStatus.Winning;
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void DolotteryTweUsersFirstWIn()
        {
            LotteryTicket expectedWin = new LotteryTicket("T100", "L100", 0, 50, 50, handler.DataLayer.getUserIDFromUserName("Arik1"));
            LotteryTicket expectedLose = new LotteryTicket("T101", "L100", 50, 100, 50, handler.DataLayer.getUserIDFromUserName("Arik2"));
            tickets.AddLast(expectedWin);
            handler.DataLayer.AddLotteryTicket(expectedWin);
            tickets.AddLast(expectedLose);
            handler.DataLayer.AddLotteryTicket(expectedLose);
            LotteryToDelete.TotalMoneyPayed = 100;
            LotteryTicket find = LotteryToDelete.Dolottery(20);
            expectedWin.myStatus = LotteryTicketStatus.Winning;
            LotteryTicket findLose = handler.DataLayer.GetLotteryTicket("T101");
            Assert.AreEqual(LotteryTicketStatus.Losing, findLose.myStatus);
            Assert.AreEqual(expectedWin, find);
        }
        [TestMethod]
        public void DolotteryIlligalValue()
        {
            LotteryTicket expected = new LotteryTicket("T100", "L100", 0, 100, 100, handler.DataLayer.getUserIDFromUserName("Arik1"));
            tickets.AddLast(expected);
            handler.DataLayer.AddLotteryTicket(expected);
            LotteryToDelete.TotalMoneyPayed = 100;
            LotteryTicket find = LotteryToDelete.Dolottery(5000);
            Assert.IsNull(find);
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
            foreach(LotteryTicket tick in tickets)
            {
                handler.DataLayer.RemoveLotteryTicket(tick);
            }
            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
