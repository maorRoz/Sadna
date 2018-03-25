using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class RegisteredUser : User
    {
        private string name;
        private string address;
        private string password;
        public RegisteredUser(int systemID,string name,string address,string password) : base(systemID)
        {
            this.name = name;
            this.address = address;
            this.password = password;
            PolicyService.AddStatePolicy(UserPolicy.State.RegisteredUser);
        }

        public override object[] ToData()
        {
            object[] ret = { systemID, name, address, password };
            return ret;
        }

        public void PromoteToAdmin()
        {
            PolicyService.AddStatePolicy(UserPolicy.State.SystemAdmin);
        }
        public void AddUserPolicy(string store, List<StoreAdminPolicy.StoreAction> actionsToAdd)
        {
            PolicyService.UpdateStorePolicies(store,actionsToAdd);
        }
    }
}
