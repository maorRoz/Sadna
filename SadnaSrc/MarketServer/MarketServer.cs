using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using SadnaSrc.Main;
using WebSocketManager;


namespace MarketServer
{
    public class MarketServer : WebSocketHandler
    {
        private List<IUserService> users;
        private MarketYard marketSession;
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            marketSession = MarketYard.Instance;
            users = new List<IUserService>();
        }

        public async Task EnterSystem(string socketId)
        {
            var user = marketSession.GetUserService();
            var id = user.EnterSystem().ReportList[0];
            users.Add(user);
            await InvokeClientMethodAsync(socketId, "identifyClient", new object[]{id});
        }
    }
}
