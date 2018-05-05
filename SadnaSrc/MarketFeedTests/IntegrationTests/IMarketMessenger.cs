using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketFeedTests.IntegrationTests
{
    public interface IMarketMessenger
    {
        void SendMessage(int receiver,string message);
    }
}
