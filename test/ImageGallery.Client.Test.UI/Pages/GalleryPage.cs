using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace ImageGallery.Client.Test.UI.Pages
{
    public class GalleryPage : BaseSecurePage
    {
        public GalleryPage(IWebDriver driver)
            : base(driver)
        {
        }

        public GalleryPage(IWebDriver driver, string url)
            : base(driver, url)
        {
        }

        [FindsBy(How = How.LinkText, Using = "Add an image")]
        protected IWebElement AddImageLink { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".danger.validation-summary-errors")]
        protected IWebElement ValidationErrorText { get; set; }

        [FindsBy(How = How.Name, Using = "title")]
        protected IWebElement ImageTitleText { get; set; }

        [FindsBy(How = How.Name, Using = "category")]
        protected IWebElement ImageTypeSelect { get; set; }

        [FindsBy(How = How.XPath, Using = ".//input[@type='file']")]
        protected IWebElement ImageBrowseButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".btn.btn-primary")]
        protected IWebElement SubmitImageButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "toast-message")]
        protected IWebElement SuccessMessageSpan { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".col-md-3.col-sm-12.text-right")]
        protected IWebElement TotalRecordNumberLabel { get; set; }

        public bool IsAddImageButtonAvailable()
        {
            bool buttonIsAvailable;
            try
            {
                AddImageLink = LoadElement(nameof(AddImageLink));
                buttonIsAvailable = true;
            }
            catch (NoSuchElementException)
            {
                buttonIsAvailable = false;
            }
            catch (WebDriverTimeoutException)
            {
                buttonIsAvailable = false;
            }

            return buttonIsAvailable;
        }

        public string GetValidationErrorText()
        {
            ValidationErrorText = LoadElement(nameof(ValidationErrorText));
            return ValidationErrorText.Text;
        }

        public void AddImageToGallery(string imageTitle, string imageType, string imageFilePath)
        {
            AddImageLink = LoadClickableElement(nameof(AddImageLink));
            AddImageLink.Click();

            ImageTitleText = LoadElement(nameof(ImageTitleText));
            ImageTypeSelect = LoadElement(nameof(ImageTypeSelect));
            ImageBrowseButton = LoadElement(nameof(ImageBrowseButton));
            SubmitImageButton = LoadElement(nameof(SubmitImageButton));

            ImageTitleText.SendKeys(imageTitle);
            var imageTypeSelect = new SelectElement(ImageTypeSelect);
            imageTypeSelect.SelectByText(imageType);
            ImageBrowseButton.SendKeys(imageFilePath);
            SubmitImageButton.Click();
        }

        public void DeleteImageByTitle(string imageTitle)
        {
            var deleteButton = _driver.FindElement(
                By.XPath($"//div[div[text()= '{imageTitle}']]/div/a[text() = 'Delete']"));
            deleteButton.Click();
        }

        public string GetSuccessMessage()
        {
            SuccessMessageSpan = LoadClickableElement(nameof(SuccessMessageSpan));
            return SuccessMessageSpan.Text;
        }

        public string GetTotalRecordsMessage()
        {
            TotalRecordNumberLabel = LoadElement(nameof(TotalRecordNumberLabel));
            return TotalRecordNumberLabel.Text.Trim();
        }

    }
}
