using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseProject
{
    public class Fetcher
    {
        public static void AddUser(int systemId)
        {
            using (var db = new MarketData())
            {
                   db.Users.Add(new UserData {SystemId = systemId });
                   db.SaveChanges();
            }

        }
    }
}
