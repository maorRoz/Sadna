using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace SystemViewTests.UseCaseUnitTest
{
	[TestClass]
	public class GetEntranceDetailsIntegrationTests
	{
		private SystemAdminService adminServiceSession;
		private IUserService userServiceSession;
		private MarketYard marketSession;
		[TestInitialize]

		public void MarketBuilder()
		{
			MarketLog.SetDB(MarketBackUpDB.Instance);
			MarketBackUpDB.Instance.CleanByForce();
			MarketDB.Instance.InsertByForce();
			marketSession = MarketYard.Instance;
			userServiceSession = marketSession.GetUserService();
			userServiceSession.EnterSystem();
			userServiceSession.SignIn("Arik1", "123");
		}

		[TestMethod]
		public void ViewEntranceReportTest()
		{
			var adminService = marketSession.GetSystemAdminService(userServiceSession);
			var answer = adminService.GetEntranceDetails();
			Assert.AreEqual((int)GetEntranceDetailsEnum.Success, answer.Status);
			Assert.AreEqual(1, answer.ReportList.Length);
			Assert.AreEqual("Number: 1 Date: "+DateTime.Now.Date, answer.ReportList[0]);

		}

		[TestCleanup]
		public void AdminTestCleanUp()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}
