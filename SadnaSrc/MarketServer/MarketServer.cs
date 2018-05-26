﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketFeed;
using WebSocketManager;


namespace MarketWeb
{
    public class MarketServer : WebSocketHandler,IListener
    {
        public static readonly Dictionary<int,IUserService> Users = new Dictionary<int, IUserService>();
        private readonly MarketYard marketSession;
        private const int Success = 0;
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            marketSession = MarketYard.Instance;
            MarketDB.Instance.InsertByForceClient();
        }

        public async Task EnterSystem(string socketId)
        {
            var userService = marketSession.GetUserService();
            var answer = userService.EnterSystem();

            var id = 0;
            if (answer.Status == Success)
            {
                id = Convert.ToInt32(answer.ReportList[0]);
                Users.Add(id, userService);
            }

            await InvokeClientMethodAsync(socketId, "IdentifyClient", new object[]{id});
        }

        public void UnSubscribeSocket(string socketId)
        {
            FeedSubscriber.UnSubscribeSocket(socketId);
        }

        public void SubscribeSocket(string userId, string socketId)
        {
            FeedSubscriber.SubscribeSocket(this,Convert.ToInt32(userId),socketId);
        }

        public async void GetMessage(string socketId, string message)
        {
            await SendFeed(socketId, message);
        }

        private async Task SendFeed(string socketId, string message)
        {
            try
            {
                await InvokeClientMethodAsync(socketId, "NotifyFeed", new object[] {message});
            }
            catch (Exception)
            {
              //  dont care
            }
        }
    }
}
