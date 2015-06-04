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
    public class WindowsWhite : IUIDriver
    {
        public bool ScreenContains(string lookFor)
        {
            throw new NotImplementedException();
        }

        public void SetTextOnControl(string controlIdOrCssSelector, string textToSet)
        {
            throw new NotImplementedException();
        }

        public void SetTextOnControl(string attributeName, string attributeValue, string textToSet, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public void ClickControl(string controlIdOrCssSelector)
        {
            throw new NotImplementedException();
        }

        public void ClickControl(string attributeName, string attributeValue, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public string GetTextOnControl(string controlIdOrCssSelector)
        {
            throw new NotImplementedException();
        }

        public string GetTextOnControl(string attributeName, string attributeValue, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public bool AmOnSceen(string snippetToLookFor)
        {
            throw new NotImplementedException();
        }

        public void SetValueOnDropDown(string controlIdOrCssSelector, string valueToSet)
        {
            throw new NotImplementedException();
        }

        public void SetValueOnDropDown(string attributeName, string attributeValue, string valueToSet = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public bool IsCheckBoxChecked(string controlIdOrCssSelector)
        {
            throw new NotImplementedException();
        }

        public bool IsCheckBoxChecked(string attributeName, string attributeValue, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public void SetCheckBoxValueTo(string controlIdOrCssSelector, bool valueItShouldBeSetTo)
        {
            throw new NotImplementedException();
        }

        public void SetCheckBoxValueTo(string attributeName, string attributeValue, bool valueItShouldBeSetTo, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public string GetTextOnDropDown(string controlIdOrCssSelector)
        {
            throw new NotImplementedException();
        }

        public string GetTextOnDropDown(string attributeName, string attributeValue, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public string GetValueOnDropDown(string controlIdOrCssSelector)
        {
            throw new NotImplementedException();
        }

        public string GetValueOnDropDown(string attributeName, string attributeValue, string controlType = "", bool useWildCardSearch = true, int retryForSeconds = 10)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string windowNameOrUri)
        {
            throw new NotImplementedException();
        }

        public void Launch(string appNameOrUri)
        {
            throw new NotImplementedException();
        }

        public void Pause(int milliseconds)
        {
            throw new NotImplementedException();
        }


        public string CurrentFormName_OrPageURL
        {
            get { throw new NotImplementedException(); }
        }


        public void MaximizeWindow()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
               
    }
}
