using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace ImageGallery.Client.Test.UI.Pages
{
    public class BaseSecurePage : BasePage
    {
        public BaseSecurePage(IWebDriver driver)
            : base(driver)
        {
        }

        public BaseSecurePage(IWebDriver driver, string url)
            : base(driver, url)
        {
        }

        [FindsBy(How = How.Id, Using = "input_login")]
        protected IWebElement UserName { get; set; }

        [FindsBy(How = How.Id, Using = "input_password")]
        protected IWebElement Password { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".btn.btn-primary")]
        protected IWebElement LoginButton { get; set; }

        [FindsBy(How = How.Id, Using = "input_logout")]
        protected IWebElement LogoutButton { get; set; }

        [FindsBy(How = How.LinkText, Using = "Google")]
        protected IWebElement LoginGoogleButton { get; set; }

        public void Login(string userName, string password)
        {
            UserName = LoadElement(nameof(UserName));
            UserName.SendKeys(userName);
            Password = LoadElement(nameof(Password));
            Password.SendKeys(password);
            LoginButton = LoadElement(nameof(LoginButton));
            LoginButton.Click();
        }

        public void LogoutAndWait()
        {
            LogoutButton = LoadClickableElement(nameof(LogoutButton));
            LogoutButton.Click();
            WaitForElementToBePresented(nameof(LoginButton));
        }

    }
}
