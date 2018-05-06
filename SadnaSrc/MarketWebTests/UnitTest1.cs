using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SadnaSrc.Main;

namespace MarketWebTests
{
    [TestClass]
    public class UnitTest1
    {
       /* private IWebDriver driver;
        [TestInitialize]
        public void WebBuilder()
        {
            MarketDB.Instance.InsertByForceClient();
            var driverDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            driver = new ChromeDriver(driverDir);
        }
        [TestMethod]
        public void TestMethod1()
        {
            driver.Navigate().GoToUrl("http://localhost:3000");
            driver.FindElement(By.XPath("//a[contains(text(),'Sign In')]")).Click();
            driver.FindElement(By.XPath("//input[contains(@id,'user-name-entry')]")).Click();
            driver.FindElement(By.XPath("//input[contains(@id,'user-name-entry')]")).SendKeys("Avi");
            driver.FindElement(By.XPath("//input[contains(@id,'user-password-entry')]")).Click();
            driver.FindElement(By.XPath("//input[contains(@id,'user-password-entry')]")).SendKeys("123");
            driver.FindElement(By.XPath("//input[contains(@id,'sign-in-button')]")).Click();
        }

        [TestCleanup]
        public void WebCleanUp()
        {
            driver.Quit();
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }*/
    }
}
