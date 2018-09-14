﻿using System;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace ImageGallery.Client.Test.UI.Pages
{
    public class BasePage : IDisposable
    {
        /// <summary>
        /// Default load timeout for page controls is 10 seconds.
        /// </summary>
        protected const int DefaultTimeout = 10;

        protected readonly IWebDriver _driver;

        public BasePage(IWebDriver driver)
        {
            _driver = driver;
        }

        public BasePage(IWebDriver driver, string url)
            : this(driver)
        {
            LoadPage(url);
        }

        public void LoadPage(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        public void SaveScreenshotAs(string filePath, ScreenshotImageFormat format)
        {
            ((ITakesScreenshot)_driver).
                GetScreenshot().
                SaveAsFile(filePath, format);
        }

        public void Dispose()
        {
            if (_driver != null)
            {
                _driver.Dispose();
            }
        }

        protected IWebElement LoadElement(string propertyName)
        {
            var propertyInfo = GetType().GetProperty(
                propertyName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var findsByArray = propertyInfo.GetCustomAttributes(typeof(FindsByAttribute), true);
            if (findsByArray.Length == 0)
            {
                return null;
            }

            var findsBy = findsByArray[0] as FindsByAttribute;
            var findsByLocator = MapFindBy(findsBy);
            IWebElement element = null;
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeout));
            wait.IgnoreExceptionTypes(new Type[] { typeof(NoSuchElementException) });
            element = wait.Until(ExpectedConditions.ElementIsVisible(findsByLocator));
            return element;
        }

        protected IWebElement LoadClickableElement(string propertyName)
        {
            var propertyInfo = GetType().GetProperty(
                propertyName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var findsByArray = propertyInfo.GetCustomAttributes(typeof(FindsByAttribute), true);
            if (findsByArray.Length == 0)
            {
                return null;
            }

            var findsBy = findsByArray[0] as FindsByAttribute;
            var findsByLocator = MapFindBy(findsBy);
            IWebElement element = null;
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeout));
            wait.IgnoreExceptionTypes(new Type[] { typeof(NoSuchElementException) });
            element = wait.Until(ExpectedConditions.ElementToBeClickable(findsByLocator));
            return element;
        }

        protected By MapFindBy(FindsByAttribute findsBy)
        {
            switch (findsBy.How)
            {
                case How.Id:
                    return By.Id(findsBy.Using);
                case How.Name:
                    return By.Name(findsBy.Using);
                case How.TagName:
                    return By.TagName(findsBy.Using);
                case How.ClassName:
                    return By.ClassName(findsBy.Using);
                case How.CssSelector:
                    return By.CssSelector(findsBy.Using);
                case How.LinkText:
                    return By.LinkText(findsBy.Using);
                case How.PartialLinkText:
                    return By.PartialLinkText(findsBy.Using);
                case How.XPath:
                    return By.XPath(findsBy.Using);
                default:
                    throw new ArgumentException("Invalid find criteria");
            }
        }
    }
}
