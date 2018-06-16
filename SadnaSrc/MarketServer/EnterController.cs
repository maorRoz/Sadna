using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;

namespace MarketWeb
{
    [Route("api/enter")]
    public class EnterController : Controller
    {
        private static readonly Dictionary<int, IUserService> users = new Dictionary<int, IUserService>();
        private static readonly MarketYard marketSession = MarketYard.Instance;
        private const int Success = 0;
        [HttpGet]
        public int Get()
        {
            var userService = marketSession.GetUserService();
            var answer = userService.EnterSystem();

            if (answer.Status != Success) return 0;
            var id = Convert.ToInt32(answer.ReportList[0]);
            users.Add(id,userService);
            return id;
        }

        public static IUserService GetUserSession(int userId)
        {
            return userId == 0 ? marketSession.GetUserService() : users[userId];
        }

        public static void ReplaceSystemIds(int newId, int oldId)
        {
            var userService = users[oldId];
            users.Remove(oldId);
            if (!users.ContainsKey(newId))
            {
                users.Add(Convert.ToInt32(newId), userService);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string data)
        {
            int i = 5;
        }
    }
}
