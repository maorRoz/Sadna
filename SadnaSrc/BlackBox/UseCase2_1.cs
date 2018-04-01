using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox
{
	
	[TestClass]
	public class UseCase2_1
	{
		private UserBridge _bridgeSignUp;
		private UserBridge _bridgeSignIn;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridgeSignIn = new RealBridge();
		}

		[TestMethod]

		public void SignInSucceeded()
		{
			SignUp("Shalom", "hatavor","852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.Success , _bridgeSignIn.SignIn("Shalom", "852").Status);
		}

		[TestMethod]

		public void DidntEnterSystem()
		{
			SignUp("Shalom", "hatavor", "852");
			Assert.AreEqual((int)SignInStatus.DidntEnterSystem, _bridgeSignIn.SignIn("Shalom", "852").Status);
		}

		[TestMethod]

		public void SignInAlready()
		{
			_bridgeSignIn.EnterSystem();
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.SignIn("Shalom", "852");
			Assert.AreEqual((int)SignInStatus.SignedInAlready, _bridgeSignIn.SignIn("Shalom", "852").Status);
		}

		[TestMethod]

		public void NoUserFound()
		{
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NoUserFound, _bridgeSignIn.SignIn("Shalom", "852").Status);
		}

		[TestMethod]

		public void UserNameGivenIsNull()
		{
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NullEmptyDataGiven, _bridgeSignIn.SignIn(null, "852").Status);
		}

		[TestMethod]

		public void UserNameGivenIsEmpty()
		{
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NullEmptyDataGiven, _bridgeSignIn.SignIn("", "852").Status);
		}

		[TestMethod]

		public void PasswordGivenIsNull()
		{
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NullEmptyDataGiven, _bridgeSignIn.SignIn("Shalom", null).Status);
		}

		[TestMethod]

		public void PasswordGivenIsEmpty()
		{
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NullEmptyDataGiven, _bridgeSignIn.SignIn("Shalom", "").Status);
		}

		[TestMethod]

		public void UserNamePasswordGivenAreNull()
		{
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NullEmptyDataGiven, _bridgeSignIn.SignIn(null, null).Status);
		}

		[TestMethod]

		public void UserNamePasswordGivenAreEmpty()
		{
			SignUp("Shalom", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NullEmptyDataGiven, _bridgeSignIn.SignIn("", "").Status);
		}

		[TestMethod]

		private void SignUp(string name,string address, string password)
		{
			_bridgeSignUp = new RealBridge();
			_bridgeSignUp.EnterSystem();
			_bridgeSignUp.SignUp(name,address,password);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridgeSignUp?.CleanSession();
			_bridgeSignIn.CleanSession();
			MarketLog.RemoveLogs();
			MarketException.RemoveErrors();
			_bridgeSignIn.ExitMarket();
			_bridgeSignUp?.ExitMarket();
		}

	}
}
