using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;
using SadnaSrc.PolicyComponent;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace SystemViewTests.AdminViewApiTest
{
	[TestClass]
	public class GetEntranceDetailsMockTest
	{
		private Mock<IMarketBackUpDB> marketDbMocker;
		private Mock<IUserAdmin> admin;
		private Mock<IAdminDL> adminDbMocker;

		[TestInitialize]
		public void MarketBuilder()
		{
			marketDbMocker = new Mock<IMarketBackUpDB>();
			MarketException.SetDB(marketDbMocker.Object);
			MarketLog.SetDB(marketDbMocker.Object);
			admin = new Mock<IUserAdmin>();
			adminDbMocker = new Mock<IAdminDL>();
			Pair<int, DateTime> p1 = new Pair<int, DateTime>(5555, new DateTime(2018,06,13,0,0,0,0,0));
			Pair<int, DateTime> p2 = new Pair<int, DateTime>(5566, new DateTime(2018, 06, 12, 0, 0, 0, 0, 0));
			Pair<int, DateTime> p3 = new Pair<int, DateTime>(5666, new DateTime(2018, 06, 12, 0, 0, 0, 0, 0));
			Pair<int, DateTime>[] report = {p1,p2,p3};
			adminDbMocker.Setup(x => x.GetEntranceReport()).Returns(report);
		}

		[TestMethod]
		public void NoAuthority()
		{
			admin.Setup(x => x.ValidateSystemAdmin())
				.Throws(new MarketException((int)GetEntranceDetailsEnum.NoAuthority, ""));
			GetEntranceDetailsSlave slave = new GetEntranceDetailsSlave(adminDbMocker.Object, admin.Object);
			slave.GetEntranceDetails();
			Assert.AreEqual((int)GetEntranceDetailsEnum.NoAuthority, slave.Answer.Status);
		}


		[TestMethod]
		public void ViewPoliciesSuccess()
		{
			GetEntranceDetailsSlave slave = new GetEntranceDetailsSlave(adminDbMocker.Object, admin.Object);
			slave.GetEntranceDetails();
			Assert.AreEqual((int)ViewPolicyStatus.Success, slave.Answer.Status);
			Assert.AreEqual(2, slave.Answer.ReportList.Length);
			Assert.AreEqual("Number: 2 Date: 12/06/2018 00:00:00", slave.Answer.ReportList[0]);
			Assert.AreEqual("Number: 1 Date: 13/06/2018 00:00:00", slave.Answer.ReportList[1]);
			
		}
	}
}
