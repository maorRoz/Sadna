using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using WebSocketManager;


namespace MarketWeb
{
    public class MarketServer : WebSocketHandler,IListener
    {
        public static readonly Dictionary<int,IUserService> Users = new Dictionary<int, IUserService>();
        private readonly MarketYard marketSession;
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            marketSession = MarketYard.Instance;
            MarketDB.Instance.InsertByForce();
        }

        public async Task EnterSystem(string socketId)
        {
            var userService = marketSession.GetUserService();
            var id = Convert.ToInt32(userService.EnterSystem().ReportList[0]);
            Users.Add(id, userService);
            await InvokeClientMethodAsync(socketId, "IdentifyClient", new object[]{id});
        }

        //Todo: fix this, not working on disconnects
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
            await InvokeClientMethodAsync(socketId, "NotifyFeed", new object[]{message});
        }
    }
}
