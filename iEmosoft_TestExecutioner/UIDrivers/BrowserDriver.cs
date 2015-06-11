using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using iEmosoft;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Model;
using iEmosoft.Automation.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace iEmosoft.Automation.UIDrivers
{
    public class BrowserDriver : IUIDriver
    {
        public enum BrowserDriverEnumeration
        {
            Chome,
            Firefox,
            IE
        }

        private IWebDriver browser = null;

        public string DriverType { get; private set; }

        public List<string> FailedBrowsers { get { return new List<string> { this.DriverType }; } }

        public BrowserDriver(BrowserDriverEnumeration browserVendor = BrowserDriverEnumeration.Firefox)
        {
            switch (browserVendor)
            {
                case BrowserDriverEnumeration.Firefox:
                    browser = new FirefoxDriver();
                    this.DriverType = "FireFox";
                    break;
                case BrowserDriverEnumeration.Chome:
                    //Tony: browser = new Chrome() (eg google Selenium with Chome)
                    this.DriverType = "Chrome";
                    break;
                case BrowserDriverEnumeration.IE:
                    //Tony: browser = new IEDriver() (eg google Selenium with IE)
                    this.DriverType = "IE";
                    break;
            }
        }

        public bool ScreenContains(string lookFor)
        {
            return browser.PageSource.Contains(lookFor);
        }

        public void SetTextOnControl(string controlIdOrCssSelector, string textToSet)
        {
            IWebElement element = browser.MineForElement(controlIdOrCssSelector);

            if (! textToSet.StartsWith("+="))
            {
                element.Clear();
            }

            if (! textToSet.isNull())
            {
                element.SendKeys(textToSet);
            }
        }

        public void SetTextOnControl(string attributeName, string attributeValue, string textToSet,
            string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = browser.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);

            if (element == null)
            {
                throw new Exception(string.Format("Unable to find {0}[{1}='{2}']", element, attributeName, attributeValue));
            }

            if (textToSet.isNull() || textToSet.StartsWith("+=") == false){
                try
                {
                    element.Clear();
                }
                catch { }
            }

            if (!textToSet.isNull())
            {
                if (element.Displayed)
                {
                    element.SendKeys(textToSet);
                }
            }
        }

        public void ClickControl(string controlIdOrCssSelector)
        {
            IWebElement element = browser.MineForElement(controlIdOrCssSelector);
            element.Click();
        }

        public void ClickControl(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = browser.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            element.Click();
        }

        public string GetTextOnControl(string controlIdOrCssSelector)
        {
            IWebElement element = browser.MineForElement(controlIdOrCssSelector);
            return element.MineForTextValue(browser as IJavaScriptExecutor);
        }

        public string GetTextOnControl(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = browser.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            return element.MineForTextValue(browser as IJavaScriptExecutor);
        }

        public bool AmOnSceen(string snippetToLookFor)
        {
            return browser.Url.Contains(snippetToLookFor);
        }

        public void SetValueOnDropDown(string controlIdOrCssSelector, string valueToSet)
        {
            var dropdown = (SelectElement) browser.MineForElement(controlIdOrCssSelector);
            var originalValue = dropdown.SelectedOption.Text;

            dropdown.SelectByText(valueToSet);

            if (originalValue == dropdown.SelectedOption.Text)
            {
                dropdown.SelectByValue(valueToSet);
            }
        }

        public void SetValueOnDropDown(string attributeName, string attributeValue, string valueToSet,
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {

            var selectElement = browser.MineForElement(attributeName, attributeValue, "select", true);
            
            try
            {
                var dropdown = (SelectElement)selectElement;
                var originalValue = dropdown.SelectedOption.Text;

                dropdown.SelectByText(valueToSet);

                if (originalValue == dropdown.SelectedOption.Text)
                {
                    dropdown.SelectByValue(valueToSet);
                }
            }
            catch 
            {
                selectElement.SendKeys(valueToSet + Keys.Enter);
               
            }
        }

        public bool IsCheckBoxChecked(string controlIdOrCssSelector)
        {
            IWebElement element = browser.MineForElement(controlIdOrCssSelector);
            bool isCurrentlyChecked = element.Selected;

            return isCurrentlyChecked;

        }

        public bool IsCheckBoxChecked(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = browser.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            bool isCurrentlyChecked = element.Selected;

            return isCurrentlyChecked;

        }

        public void SetCheckBoxValueTo(string controlIdOrCssSelector, bool valueItShouldBeSetTo)
        {
            IWebElement element = browser.MineForElement(controlIdOrCssSelector);

            bool isCurrentlyChecked = element.Selected;

            if (isCurrentlyChecked != valueItShouldBeSetTo)
            {
                element.Click();
            }
        }

        public void SetCheckBoxValueTo(string attributeName, string attributeValue, bool valueItShouldBeSetTo,
            string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = browser.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            bool isCurrentlyChecked = element.Selected;

            if (isCurrentlyChecked != valueItShouldBeSetTo)
            {
                element.Click();
            }
        }

        public string GetTextOnDropDown(string controlIdOrCssSelector)
        {
            var element = (SelectElement) browser.MineForElement(controlIdOrCssSelector);
            try
            {
                return element.SelectedOption.Text;
            }
            catch
            {
            }

            return "";
        }

        public string GetTextOnDropDown(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            try
            {
                var element =
                    (SelectElement)
                        browser.MineForElement(attributeName, attributeValue, controlType, useWildCardSearch,
                            retryForSeconds);
                return element.SelectedOption.Text;
            }
            catch
            {
            }

            return "";
        }

        public string GetValueOnDropDown(string controlIdOrCssSelector)
        {
            try
            {
                var element = (SelectElement) browser.MineForElement(controlIdOrCssSelector);
                return element.SelectedOption.GetAttribute("value");
            }
            catch
            {
            }

            return "";

        }

        public string GetValueOnDropDown(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            try
            {
                var element =
                    (SelectElement)
                        browser.MineForElement(attributeName, attributeValue, controlType, useWildCardSearch,
                            retryForSeconds);
                return element.SelectedOption.GetAttribute("value");
            }
            catch
            {
            }

            return "";
        }

        public void NavigateTo(string windowNameOrUri)
        {
            browser.NavigateTo(windowNameOrUri);
        }

        public void Launch(string appNameOrUri)
        {
            NavigateTo(appNameOrUri);
        }

        public void Pause(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public IWebDriver RawWebDriver
        {
            get { return browser; }
        }


        public string CurrentFormName_OrPageURL
        {
            get { return browser.Url; }
        }

        public void ShowWindow()
        {
            browser.Manage().Window.Maximize();
        }

        public void MaximizeWindow()
        {
            browser.Manage().Window.Maximize();
        }

        public void Dispose()
        {
            browser.Quit();
        }
    }
}
