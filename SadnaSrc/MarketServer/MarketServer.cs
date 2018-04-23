using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketManager;


namespace MarketServer
{
    public class MarketServer : WebSocketHandler
    {
        public MarketServer(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {

        }

        public async Task SendMessage(string socketId, string message)
        {
            await InvokeClientMethodToAllAsync("marketMessage", socketId, message);
        }
    }
}
