﻿using System;
using SadnaSrc.AdminView;
using SadnaSrc.MarketData;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;
using SadnaSrc.OrderPool;
using SadnaSrc.PolicyComponent;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;
using SadnaSrc.Walleter;

namespace SadnaSrc.Main
{
    public class MarketYard
    {
        private static MarketYard _instance;

        public static MarketYard Instance => _instance ?? (_instance = new MarketYard());

        private static StoreOrderTools refundLotteriesService;

        public static DateTime MarketDate { get; private set; }
        private MarketYard()
        {
            MarketDate = new DateTime(2018, 4, 14);
            refundLotteriesService = new StoreOrderTools();
        }

        public static void SetDateTime(DateTime marketDate)
        {
            if (_instance == null) {return;}
            MarketDate = marketDate;
            refundLotteriesService.RefundAllExpiredLotteries();
        }


        public IUserService GetUserService()
        {
            return new UserService();
        }

        public ISystemAdminService GetSystemAdminService(IUserService userService)
        {
            return new SystemAdminService(new UserAdminHarmony(userService));
        }

        public IStoreManagementService GetStoreManagementService(IUserService userService,string store)
        {
            return new StoreManagementService(new UserSellerHarmony(ref userService,store),store);
        }

        public IStoreShoppingService GetStoreShoppingService(ref IUserService userService)
        {
            return new StoreShoppingService(new UserShopperHarmony(ref userService));
        }

        public IOrderService GetOrderService(ref IUserService userService)
        {
            return new OrderService(new UserBuyerHarmony(ref userService), new StoresSyncherHarmony());
        }

        public IPaymentService GetPaymentService()
        {
            return PaymentService.Instance;
        }

        public ISupplyService GetSupplyService()
        {
            return SupplyService.Instance;
        }

        public IPublisher GetPublisher()
        {
            try
            {
                return Publisher.Instance;
            }
            catch (DataException)
            {
                return null;
            }
        }

        public IGlobalPolicyManager GetGlobalPolicyManager()
        {
            return PolicyHandler.Instance;
        }

        public IStorePolicyManager GetStorePolicyManager()
        {
            return PolicyHandler.Instance;
        }

        public IPolicyChecker GetPolicyChecker()
        {
            return PolicyHandler.Instance;
        }

        public static void CleanSession()
        {
            MarketException.RemoveErrors();
            MarketBackUpDB.Instance.CleanByForce();
            Publisher.CleanPublisher();
            Category.RestartCategoryID();
            Product.RestartProductID();
            Store.RestartStoreID();
            LotteryTicket.RestartLotteryTicketID();
            LotterySaleManagmentTicket.RestartLotteryID();
            Discount.RestartDiscountID();
        }
    }
}
