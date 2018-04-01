using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
    public class StoreService : IStoreService
    {
        public StoreService(UserService userService)
        {
            
        }
        
        public MarketAnswer OpenStore()
        {
            throw new NotImplementedException();
        }

        public MarketAnswer PromoteToOwner()
        {
            throw new NotImplementedException();
        }
    }
}
