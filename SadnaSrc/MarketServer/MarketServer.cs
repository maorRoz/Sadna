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
        private Dictionary<int,IUserService> users;
        private MarketYard marketSession;
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            marketSession = MarketYard.Instance;
            users = new Dictionary<int,IUserService>();
        }

        public async Task EnterSystem(string socketId)
        {
            var userService = marketSession.GetUserService();
            var id = Convert.ToInt32(userService.EnterSystem().ReportList[0]);
            users.Add(id, userService);
            await InvokeClientMethodAsync(socketId, "IdentifyClient", new object[]{id});
        }

        public async Task SignUpUser(string socketId,string userId, string userName, string address, string password,
            string creditCard)
        {
            var userService = users[Convert.ToInt32(userId)];
            var answer = userService.SignUp(userName, address, password, creditCard);
            await InvokeClientMethodAsync(socketId, "LoggingMarket", new object[]{answer.Status,answer.Answer,userId});
        }
    }
}
