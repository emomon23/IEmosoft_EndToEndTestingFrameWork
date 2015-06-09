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
    public class Firefox : IUIDriver
    {
        private IWebDriver firefoxDriver = new FirefoxDriver();

        public bool ScreenContains(string lookFor)
        {
            return firefoxDriver.PageSource.Contains(lookFor);
        }

        public void SetTextOnControl(string controlIdOrCssSelector, string textToSet)
        {
            IWebElement element = firefoxDriver.MineForElement(controlIdOrCssSelector);

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
            IWebElement element = firefoxDriver.MineForElement(attributeName, attributeValue, controlType,
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
            IWebElement element = firefoxDriver.MineForElement(controlIdOrCssSelector);
            element.Click();
        }

        public void ClickControl(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = firefoxDriver.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            element.Click();
        }

        public string GetTextOnControl(string controlIdOrCssSelector)
        {
            IWebElement element = firefoxDriver.MineForElement(controlIdOrCssSelector);
            return element.MineForTextValue(firefoxDriver as IJavaScriptExecutor);
        }

        public string GetTextOnControl(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = firefoxDriver.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            return element.MineForTextValue(firefoxDriver as IJavaScriptExecutor);
        }

        public bool AmOnSceen(string snippetToLookFor)
        {
            return firefoxDriver.Url.Contains(snippetToLookFor);
        }

        public void SetValueOnDropDown(string controlIdOrCssSelector, string valueToSet)
        {
            var dropdown = (SelectElement) firefoxDriver.MineForElement(controlIdOrCssSelector);
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
           
            var selectElement = firefoxDriver.MineForElement(attributeName, attributeValue, "select", useWildCardSearch,
                        retryForSeconds);

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
            IWebElement element = firefoxDriver.MineForElement(controlIdOrCssSelector);
            bool isCurrentlyChecked = element.Selected;

            return isCurrentlyChecked;

        }

        public bool IsCheckBoxChecked(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = firefoxDriver.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            bool isCurrentlyChecked = element.Selected;

            return isCurrentlyChecked;

        }

        public void SetCheckBoxValueTo(string controlIdOrCssSelector, bool valueItShouldBeSetTo)
        {
            IWebElement element = firefoxDriver.MineForElement(controlIdOrCssSelector);

            bool isCurrentlyChecked = element.Selected;

            if (isCurrentlyChecked != valueItShouldBeSetTo)
            {
                element.Click();
            }
        }

        public void SetCheckBoxValueTo(string attributeName, string attributeValue, bool valueItShouldBeSetTo,
            string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            IWebElement element = firefoxDriver.MineForElement(attributeName, attributeValue, controlType,
                useWildCardSearch, retryForSeconds);
            bool isCurrentlyChecked = element.Selected;

            if (isCurrentlyChecked != valueItShouldBeSetTo)
            {
                element.Click();
            }
        }

        public string GetTextOnDropDown(string controlIdOrCssSelector)
        {
            var element = (SelectElement) firefoxDriver.MineForElement(controlIdOrCssSelector);
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
                        firefoxDriver.MineForElement(attributeName, attributeValue, controlType, useWildCardSearch,
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
                var element = (SelectElement) firefoxDriver.MineForElement(controlIdOrCssSelector);
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
                        firefoxDriver.MineForElement(attributeName, attributeValue, controlType, useWildCardSearch,
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
            firefoxDriver.NavigateTo(windowNameOrUri);
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
            get { return firefoxDriver; }
        }


        public string CurrentFormName_OrPageURL
        {
            get { return firefoxDriver.Url; }
        }

        public void ShowWindow()
        {
            firefoxDriver.Manage().Window.Maximize();
        }

        public void MaximizeWindow()
        {
            firefoxDriver.Manage().Window.Maximize();
        }

        public void Dispose()
        {
            firefoxDriver.Quit();
        }
    }
}
