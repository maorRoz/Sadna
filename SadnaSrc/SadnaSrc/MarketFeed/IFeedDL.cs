using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public interface IFeedDL
    {
        Dictionary<int, IFeedQueue> GetReaders();
        void SaveNotification(Notification notification);


    }
}
