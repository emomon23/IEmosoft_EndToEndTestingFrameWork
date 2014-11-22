using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        public static IntPtr GetFirefoxWindowsHandle(this IWebDriver driver)
        {
            IntPtr result = IntPtr.Zero;

            var title = driver.Title;
            title = title.IsNull() ? "Something we don't want to seach for ASFSFSA!!" : title;

            var firefoxBrowsers = Process.GetProcessesByName("firefox");
            if (firefoxBrowsers.Count() == 1)
                return firefoxBrowsers.ElementAt(0).MainWindowHandle;

            IntPtr activeWindowHandle = GetActiveWindow();

            foreach (var browser in firefoxBrowsers)
            {
                if (browser.MainWindowTitle == title || browser.MainWindowHandle == activeWindowHandle)
                {
                    result = browser.MainWindowHandle;
                }
            }

            return result;
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

        public static bool PageContains(this IWebDriver driver, string lookFor)
        {
            return driver.PageSource.ToLower().Contains(lookFor.ToLower());
        }

        public static string GetSelectedValueOnDropdown(this IWebDriver driver, By by){
            var dropdown = (SelectElement)driver.FindElement(by);

            return dropdown.SelectedOption.GetAttribute("value");
        }

        public static void NavigateTo(this IWebDriver driver, string url){
            driver.Navigate().GoToUrl(url);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();
        

        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
