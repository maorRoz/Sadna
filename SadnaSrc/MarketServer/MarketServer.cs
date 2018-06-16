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
