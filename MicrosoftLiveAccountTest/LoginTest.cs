using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace MicrosoftLiveAccountTest
{

    [TestClass]
    public class LoginTest
    {
        public static string userLogin = ConfigurationManager.AppSettings.Get("UserLogin");
        public static string userPasswd = ConfigurationManager.AppSettings.Get("UserPassword");

        public static IWebDriver driver;

        [TestInitialize]
        public void TestInitialize()
        {
            driver = new ChromeDriver();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //-driver.Dispose();
        }

        [TestMethod]
        public void VerifyLoginAsValidUser()
        {
            driver.Url = "https://outlook.live.com/owa/";
            driver.Manage().Window.Maximize();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            //wait for element 'Sign in' and click it
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@data-task='signin']")));
            driver.FindElement(By.XPath("//a[@data-task='signin']")).Click();

            //wait for user login input and send keys into it
            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("loginfmt")));
            driver.FindElement(By.Name("loginfmt")).SendKeys(userLogin);

            //wait for element 'Next' and click it
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
            driver.FindElement(By.Id("idSIButton9")).Click();

            //wait for user password input and send keys into it
            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("passwd")));
            driver.FindElement(By.Name("passwd")).SendKeys(userPasswd);

            //wait for element 'Sign in' and click it
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("idSIButton9")));
            driver.FindElement(By.Id("idSIButton9")).Click();

            WebDriverWait localWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            //if element 'O365_MainLink_Me' from logged in page does not exist check if 'Stay signed in?' screen appeared
            try
            {
                localWait.Until(ExpectedConditions.ElementExists(By.Id("O365_MainLink_Me")));
            }
            catch
            {
                //wait for element 'No' and click it
                localWait.Until(ExpectedConditions.ElementExists(By.Id("idBtn_Back")));
                driver.FindElement(By.Id("idBtn_Back")).Click();

                localWait.Until(ExpectedConditions.ElementExists(By.Id("O365_MainLink_Me")));
            }

            //click element 'Account manager'
            driver.FindElement(By.Id("O365_MainLink_Me")).Click();

            wait.Until(ExpectedConditions.ElementExists(By.Id("FlexPane_MeControl")));
            wait.Until(ExpectedConditions.ElementExists(By.XPath($"//span[contains(., '{userLogin}')]")));

            //assert that correct user is signed in (user login is displayed in Account manager tab)
            Assert.IsTrue(driver.FindElement(By.Id("FlexPane_MeControl")).FindElement(By.XPath($"//span[contains(., '{userLogin}')]")).Displayed);

            //assert that element 'Sign out' exists
            Assert.IsTrue(driver.FindElement(By.Id("FlexPane_MeControl")).FindElement(By.Id("meControlSignoutLink")).Displayed);
        }
    }
}
