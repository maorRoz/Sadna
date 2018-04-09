using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox
{
	[TestClass]
	public class UseCase5_2
	{
		private IUserBridge _adminBridge;
		private IUserBridge _signUpBridge1;
		private IUserBridge _signUpBridge2;
		private IUserBridge _signInBridge;
		private readonly string userSoleStoreOwner = "PninaSoleStoreOwner";
		private readonly string userNotSoleStoreOwner = "PninaNotSoleStoreOwner";
		private readonly string userSoleStoreOwnerPass = "789456";
		private readonly string userNotSoleStoreOwnerPass = "741852";
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";

		[TestInitialize]

		public void MarketBuilder()
		{
			_adminBridge = Driver.getBridge();
			_signUpBridge1 = Driver.getBridge();
			_signUpBridge1.EnterSystem();
			_signUpBridge1.SignUp(userSoleStoreOwner, "mishol", userSoleStoreOwnerPass,"12345678");
			_signUpBridge2 = Driver.getBridge();
			_signUpBridge2.EnterSystem();
			_signUpBridge2.SignUp(userNotSoleStoreOwner, "susia", userNotSoleStoreOwnerPass, "12345678");
			
		}

		[TestMethod]

		public void SuccessDeleteUserNotSoleOwner()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.Success, _adminBridge.RemoveUser(userNotSoleStoreOwner).Status);
			_signInBridge = Driver.getBridge();
			_signInBridge.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NoUserFound,_signInBridge.SignIn(userNotSoleStoreOwner, userNotSoleStoreOwnerPass).Status);
		}

		[TestMethod]

		public void SuccessDeleteUserSoleOwner()
		{
			//TODO: open a store.
			//_adminBridge.GetStoreService();
			//_adminBridge.OpenStore("newStore", "newPlace");
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			_signInBridge = Driver.getBridge();
			_signInBridge.EnterSystem();
			Assert.AreEqual((int)RemoveUserStatus.Success, _adminBridge.RemoveUser(userSoleStoreOwner).Status);
			//TODO: try to close a store, this should fail because the store is already closed.
			
			Assert.AreEqual((int)SignInStatus.NoUserFound, _signInBridge.SignIn(userSoleStoreOwner, userSoleStoreOwnerPass).Status);

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
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(userNotSoleStoreOwner).Status);
		}

		[TestMethod]

		public void NoSystemAdmin2()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(userNotSoleStoreOwnerPass).Status);
		}

		[TestMethod]

		public void NoSystemAdmin3()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, _adminBridge.RemoveUser(userSoleStoreOwnerPass).Status);
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
			_signUpBridge1.CleanSession();
			_signUpBridge2.CleanSession();
			_signInBridge?.CleanSession();
			_signUpBridge1.CleanMarket();

		}
	}
}
