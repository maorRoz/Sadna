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
		private readonly string userNameNotSoleOwner = "Arik3";
		private string userNameSoleOwner = "Arik2";

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
			Assert.AreEqual((int)RemoveUserStatus.Success, _adminBridge.RemoveUser(userNameNotSoleOwner).Status);
		}

		[TestMethod]

		public void SuccessDeleteUserSoleOwner()
		{
			//TODO: check the store was closed
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.Success, _adminBridge.RemoveUser(userNameNotSoleOwner).Status);
		}

		[TestMethod]

		public void DeleteMySelf()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.SelfTermination, _adminBridge.RemoveUser(adminName).Status);
		}

		[TestMethod]

		public void UserToRemoveWasntFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NoUserFound, _adminBridge.RemoveUser("sdadasdasdasdasdasdas").Status);
		}

		[TestMethod]

		public void NoSystemAdmin1()
		{
			SignIn("hello", adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(userNameNotSoleOwner).Status);
		}

		[TestMethod]

		public void NoSystemAdmin2()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(userNameNotSoleOwner).Status);
		}

		[TestMethod]

		public void NoSystemAdmin3()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(userNameSoleOwner).Status);
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
