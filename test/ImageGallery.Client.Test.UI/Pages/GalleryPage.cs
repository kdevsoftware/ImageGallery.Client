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

        [FindsBy(How = How.Id, Using = "val_alert_message")]
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

        [FindsBy(How = How.Id, Using = "lbl_totalRecords")]
        protected IWebElement TotalRecords { get; set; }

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

        public string GetValidationLoginErrorText()
        {
            ValidationErrorText = LoadElement(nameof(ValidationLoginErrorText));
            return ValidationErrorText.Text;
        }

        public string GetValidationPasswordErrorText()
        {
            ValidationErrorText = LoadElement(nameof(ValidationPasswordErrorText));
            return ValidationErrorText.Text;
        }

        public string GetValidationErrorText(string text)
        {
            ValidationErrorText = LoadElement(nameof(ValidationErrorText));
            return ValidationErrorText.Text;
        }

        public void AddImageToGallery(string imageTitle, string imageType, string imageFilePath)
        {
            // TODO : Check Image File Path is Valid
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

        public void DeleteImageByTitle(string imageTitle, string imageId)
        {

            var deleteButton = _driver.FindElement(
                By.XPath($"//div[div[text()= '{imageTitle}']]/div/a[text() = 'Delete']"));

            // Find Link Delete - lnk_delete_{ { image.id} }

            deleteButton.Click();
        }

        public string GetSuccessMessage()
        {
            SuccessMessageSpan = LoadClickableElement(nameof(SuccessMessageSpan));
            return SuccessMessageSpan.Text;
        }

        public int GetTotalRecords()
        {
            TotalRecords = LoadElement(nameof(TotalRecords));
            return int.Parse(TotalRecords.Text.Trim());
        }

        public string GetTotalRecordsMessage()
        {
            TotalRecordNumberLabel = LoadElement(nameof(TotalRecordNumberLabel));
            return TotalRecordNumberLabel.Text.Trim();
        }
    }
}
