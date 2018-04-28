using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using SadnaSrc.Main;
using WebSocketManager;


namespace MarketWeb
{
    public class MarketServer : WebSocketHandler
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
            await InvokeClientMethodAsync(socketId, "IdentifyClient", new object[]{id});
        }

        public async Task SendFeed()
        {
            await InvokeClientMethodAsync("?", "NotifyFeed", new object[]{"some stupid notification"});
        }
    }
}
