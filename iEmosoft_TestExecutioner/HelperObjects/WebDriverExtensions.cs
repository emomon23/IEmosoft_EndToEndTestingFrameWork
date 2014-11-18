using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace RecordableBrowser
{
    public static class WebDriverExtensions
    {
        public static void ClickElement(this IWebDriver driver, By by)
        {
            driver.FindElement(by).Click();
        }

        public static void SetTextOnElement(this IWebDriver driver, By by, string text){
            driver.FindElement(by).SendKeys(text);
        }

        public static bool ElementExists(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by) != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool ElementExists(this IWebDriver driver, string elementID)
        {
            return ElementExists(driver, By.Id(elementID));
        }

        public static void SetTextOnElement(this IWebDriver driver, string elementToFind, string text)
        {
            IWebElement element = null;

            try
            {
                element = driver.FindElement(By.Id(elementToFind));
            }
            catch { }

            if (element == null)
            {
                element = driver.FindElement(By.XPath(elementToFind));
            }

            if (elementToFind != null)
                element.SendKeys(text);

        }

        public static string GetTextOnElement(this IWebDriver driver, By by)
        {
            var element = driver.FindElement(by);
            string rtnVal = element.Text;

            if (string.IsNullOrEmpty(rtnVal))
                rtnVal = element.GetAttribute("value");

            return rtnVal;
        }

        public static void SetValueOnDropdown(this IWebDriver driver, By by, string valueToSet)
        {
            var dropdown = (SelectElement)driver.FindElement(by);
            var originalValue = dropdown.SelectedOption.Text;

            dropdown.SelectByText(valueToSet);

            if (originalValue == dropdown.SelectedOption.Text){
                dropdown.SelectByValue(valueToSet);
            }
        }

        public static string GetSelectedTextOnDropdown(this IWebDriver driver, By by){
            var dropdown = (SelectElement)driver.FindElement(by);

            return dropdown.SelectedOption.Text;
        }

        public static string GetSelectedValueOnDropdown(this IWebDriver driver, By by){
            var dropdown = (SelectElement)driver.FindElement(by);

            return dropdown.SelectedOption.GetAttribute("value");
        }

        public static void NavigateTo(this IWebDriver driver, string url){
            driver.Navigate().GoToUrl(url);
        }


        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
