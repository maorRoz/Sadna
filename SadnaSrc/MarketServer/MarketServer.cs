using System;
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
        private static readonly Dictionary<int,IUserService> Users = new Dictionary<int, IUserService>();
        private static readonly MarketYard marketSession = MarketYard.Instance;
        private const int Success = 0;
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            try
            {
                MarketDB.Instance.InsertByForceClient();
            }
            catch (Exception)
            {
                //dont care
            }
        }

        public static IUserService GetUserSession(int userId)
        {
            return userId == 0 ? marketSession.GetUserService() : Users[userId];
        }

        public static void ReplaceSystemIds(int newId, int oldId)
        {
            var userService = Users[oldId];
            Users.Remove(oldId);
            if (!Users.ContainsKey(newId))
            {
                Users.Add(Convert.ToInt32(newId), userService);
            }
        }

        public async Task MassSyncUsersWithNoId()
        {
            await InvokeClientMethodToAllAsync("EnterSystemAgain", null);
        }
        private int EnterSystem()
        {
            var userService = marketSession.GetUserService();
            var answer = userService.EnterSystem();

            if (answer.Status != Success) return 0;
            var id = Convert.ToInt32(answer.ReportList[0]);
            Users.Add(id, userService);

            return id;
        }
        public async Task EnterSystem(string socketId)
        {
            var generatedId = EnterSystem();
            await InvokeClientMethodAsync(socketId, "IdentifyClient", new object[]{ generatedId});
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
