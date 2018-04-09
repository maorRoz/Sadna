using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox
{
	
	[TestClass]
	public class UseCase2_1
	{
		private IUserBridge _bridgeSignUp;
		private IUserBridge _bridgeSignIn;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridgeSignIn = Driver.getBridge();
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

		public void FindUserWithErrorUserName2Letters1()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("sarl", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserName2Letters2()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("rtal", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserName2Letters3()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("shor", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserName1Letter1()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("ahal", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserName1Letter2()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("saal", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserName1Letter3()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("shol", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserName1Letter4()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.MistakeTipGiven, _bridgeSignIn.SignIn("shar", "852").Status);

		}

		[TestMethod]

		public void FindUserWithErrorUserNameMoreThan2Letters1()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NoUserFound, _bridgeSignIn.SignIn("saor", "852").Status);

		}


		[TestMethod]

		public void FindUserWithErrorUserNameMoreThan2Letters2()
		{
			SignUp("shal", "hatavor", "852");
			_bridgeSignIn.EnterSystem();
			Assert.AreEqual((int)SignInStatus.NoUserFound, _bridgeSignIn.SignIn("horl", "852").Status);

		}


		private void SignUp(string name,string address, string password)
		{
			_bridgeSignUp = Driver.getBridge();
			_bridgeSignUp.EnterSystem();
			_bridgeSignUp.SignUp(name,address,password);
		}

		

		 

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridgeSignUp?.CleanSession();
			_bridgeSignIn.CleanSession();
			_bridgeSignIn.CleanMarket();
			_bridgeSignUp?.CleanMarket();
		}

	}
}
