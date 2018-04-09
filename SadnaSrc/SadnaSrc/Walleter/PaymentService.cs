﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyExternalSystems;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

namespace SadnaSrc.Walleter
{
    public class PaymentService : IPaymentService
    {
        private PaymentSystem sock;


        //TODO: change this once info about external systems is available.
        public MarketAnswer AttachExternalSystem()
        {
            sock = new PaymentSystem();
            MarketLog.Log("SupplyPoint", "Connection to external payment system established successfully ! ");
            return new WalleterAnswer(WalleterStatus.Success, "Connection to external payment system established successfully !");

        }

        public void ProccesPayment(Order order, string creditCardetails)
        {
            if (sock == null)
            {
                throw new WalleterException(WalleterStatus.NoPaymentSystem, "Failed, an error in the payment system occured.");
            }
            MarketLog.Log("Walleter", "Attempting to proccess payment for order ID: " + order.GetOrderID());
            CheckCreditCard(creditCardetails);
            if (sock.ProccessPayment(creditCardetails, order.GetPrice()))
                {
                    MarketLog.Log("Walleter", "Payment for order ID: " + order.GetOrderID() + " was completed.");
                    return;
                }
                throw new WalleterException(WalleterStatus.PaymentSystemError, "Failed, an error in the payment system occured.");
           

        }

        public void Refund(double sum, string creditCardetails,string username)
        {
            if (sock == null)
            {
                throw new WalleterException(WalleterStatus.NoPaymentSystem, "Failed, an error in the payment system occured.");
            }
            MarketLog.Log("Walleter", "Attempting to make a refund for user: " + username );
            CheckCreditCard(creditCardetails); 
            CheckRefundDetails(sum,username);
            if (sock.ProccessPayment(creditCardetails,  -1 * sum))
            {
                MarketLog.Log("Walleter", "Refund for user: "+ username + " was completed !");
                return;
            }
            throw new WalleterException(WalleterStatus.PaymentSystemError, "Failed, an error in the payment system occured.");
            

        }

        public void CheckCreditCard(string details)
        {
            int x;
            if (details.Length != 8 || !Int32.TryParse(details, out x))
            {
                throw new WalleterException(WalleterStatus.InvalidCreditCardSyntax, "Failed, Invalid credit card details..");
            }
        }

        public void CheckOrderDetails(Order order)
        {
            if (order.GetOrderID() == 0 || order.GetPrice() == 0.0)
            {
                throw new WalleterException(WalleterStatus.InvalidOrder, "Failed, Invalid order details");
            }
        }

        public void CheckRefundDetails(double sum, string username)
        {
            if (sum == 0 || username == null)
            {
                throw new WalleterException(WalleterStatus.InvalidData, "Failed, Invalid details");
            }
        }

        public void BreakExternal()
        {
            sock.fuckUp();
        }
    }
}
