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
        private const int SuccessLogin = 0;
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

        public async Task SignInUser(string socketId, string userId, string userName,string password)
        {
            int userIdNumber = Convert.ToInt32(userId);
            var userService = users[userIdNumber];
            var answer = userService.SignIn(userName, password);
            if (answer.Status == SuccessLogin)
            {
                users.Remove(userIdNumber);
                userIdNumber = Convert.ToInt32(answer.ReportList[0]);
                if (!users.ContainsKey(userIdNumber))
                {
                    users.Add(Convert.ToInt32(userIdNumber), userService);
                }
            }
            await InvokeClientMethodAsync(socketId, "LoggingMarket",
                new object[] { answer.Status, answer.Answer, userIdNumber });
        }
    }
}
