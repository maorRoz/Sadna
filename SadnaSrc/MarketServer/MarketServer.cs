using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task SendMessage(string socketId, string message)
        {
            await InvokeClientMethodToAllAsync("marketMessage", socketId, message);
        }

        public async Task EnterSystem(object socketId)
        {
            var user = marketSession.GetUserService();
            var id = user.EnterSystem().ReportList[0];
            users.Add(user);
            await InvokeClientMethodAsync(null,"identifyClient", new object[]{id});
        }
    }
}
