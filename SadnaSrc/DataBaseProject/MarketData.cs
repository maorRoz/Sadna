using System.ComponentModel.DataAnnotations;

namespace DataBaseProject
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class MarketData : DbContext
    {
      /*  public MarketData() 
            : base("name=MarketData")
        {

        }*/


        public virtual DbSet<UserData> Users { get; set; }
    }

    public class UserData
    {
        [Key]
        public int SystemId { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string CreditCard { get; set; }

    }

}