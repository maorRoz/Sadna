using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox
{
	[TestClass]
	public class UseCase5_2
	{
		private IUserBridge _adminBridge;
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";
		private readonly int adminId = 1;
		private readonly int UserIdNotSoleOwner = 3;
		private int UserIdSoleOwner = 2;

		[TestInitialize]

		public void MarketBuilder()
		{
			_adminBridge = new RealBridge();
			
		}

		[TestMethod]

		public void SuccessDeleteUserNotSoleOwner()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.Success, _adminBridge.RemoveUser(UserIdNotSoleOwner).Status);
		}

		[TestMethod]

		public void SuccessDeleteUserSoleOwner()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.Success, _adminBridge.RemoveUser(UserIdNotSoleOwner).Status);
		}

		[TestMethod]

		public void DeleteMySelf()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.SelfTermination, _adminBridge.RemoveUser(adminId).Status);
		}

		[TestMethod]

		public void UserToRemoveWasntFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NoUserFound, _adminBridge.RemoveUser(-5).Status);
		}

		[TestMethod]

		public void NoSystemAdmin1()
		{
			SignIn("hello", adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(UserIdNotSoleOwner).Status);
		}

		[TestMethod]

		public void NoSystemAdmin2()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(UserIdNotSoleOwner).Status);
		}

		[TestMethod]

		public void NoSystemAdmin3()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(UserIdSoleOwner).Status);
		}

		private void SignIn(string userName, string password)
		{
			_adminBridge.EnterSystem();
			_adminBridge.SignIn(userName, password);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_adminBridge.CleanSession();
			_adminBridge.CleanMarket();
		}
	}
}
