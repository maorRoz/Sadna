using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;


namespace SystemViewTests.UseCaseUnitTest
{
    [TestClass]
    public class AdminViewCategoryTestsDL
    {
        private IAdminDL handler;

        [TestInitialize]
        public void BuildStore()
        {
            handler = AdminDL.Instance;
        }


        [TestMethod]
        public void AddCategory()
        {

            var expected = new Category("C2", "Items");
            var find = handler.GetCategoryByName("Items");
            Assert.IsNull(find);
            handler.AddCategory(expected);
            find = handler.GetCategoryByName("Items");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveCategory()
        {

            var expected = new Category("C2", "Items");
            var find = handler.GetCategoryByName("Items");
            Assert.IsNull(find);
            handler.AddCategory(expected);
            find = handler.GetCategoryByName("Items");
            Assert.AreEqual(expected, find);
            handler.RemoveCategory(expected);
            find = handler.GetCategoryByName("Items");
            Assert.IsNull(find);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

    }
}
