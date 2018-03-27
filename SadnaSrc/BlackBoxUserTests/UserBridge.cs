namespace BlackBoxUserTests
{
	public interface UserBridge
	{
		string EnterSystem();
		string SignUp(string name, string address, string password);
	}
}
