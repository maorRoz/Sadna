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
        private const int Success = 0;
        public static Dictionary<int,IUserService> users = new Dictionary<int, IUserService>();
        private MarketYard marketSession;
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            marketSession = MarketYard.Instance;
            MarketDB.Instance.InsertByForce();
        }

        public async Task EnterSystem(string socketId)
        {
            var userService = marketSession.GetUserService();
            var id = Convert.ToInt32(userService.EnterSystem().ReportList[0]);
            users.Add(id, userService);
            SubscribeSocket(id,socketId);
            await InvokeClientMethodAsync(socketId, "IdentifyClient", new object[]{id});
        }

        public void SubscribeSocket(int userId, string socketId)
        {
           // var observer = new FeedObserver(this, userId, socketId);
           // observer.Subscribe();
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
