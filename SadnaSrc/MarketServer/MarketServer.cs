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

        public async Task SignInUser(string socketId, string userId, string userName,string password)
        {
            int userIdNumber = Convert.ToInt32(userId);
            var userService = users[userIdNumber];
            var answer = userService.SignIn(userName, password);
            if (answer.Status == Success)
            {
                users.Remove(userIdNumber);
                userIdNumber = Convert.ToInt32(answer.ReportList[0]);
                if (!users.ContainsKey(userIdNumber))
                {
                    users.Add(Convert.ToInt32(userIdNumber), userService);
                }

                string state = answer.ReportList[1];
                await InvokeClientMethodAsync(socketId, "LoggedMarket",
                    new object[] {answer.Answer, userIdNumber, state});
            }
            else
            {
                await InvokeClientMethodAsync(socketId, "ErrorApi",
                    new object[] {answer.Answer});
            }
        }

        public async Task SendFeed()
        {
            await InvokeClientMethodAsync("?", "NotifyFeed", new object[]{"some stupid notification"});
        }
    }
}
