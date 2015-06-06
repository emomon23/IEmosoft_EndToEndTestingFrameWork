using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation.Model;
using iEmosoft.Automation.HelperObjects;

namespace iEmosoft.Automation.UIDrivers
{
    public class MultiBrowser : IUIDriver
    {
        List<IUIDriver> browsers;
        List<string> browsersThatFailed;

        public MultiBrowser(IAutomationConfiguration config)
        {
            string[] browsersToNewUp = config.TestExecutionerUIDriverType;

            foreach (string browserName in browsersToNewUp)
            {
                switch (browserName)
                {
                    case "FIREFOX":
                        this.browsers.Add(new BrowserDriver(BrowserDriver.BrowserDriverEnumeration.Firefox));
                        break;
                    case "IE":
                        this.browsers.Add(new BrowserDriver(BrowserDriver.BrowserDriverEnumeration.IE));
                        break;
                    case "CHORME":
                        this.browsers.Add(new BrowserDriver(BrowserDriver.BrowserDriverEnumeration.Chome));
                        break;
                    default:
                        throw new Exception(string.Format("Unable to determine browser to create, expected 'CHROME, IE, OR FIREFOX', actual value: '{0}'", browserName));
                }
            }
        }

        public string DriverType { get { return "MultiBrowser"; } }

        public List<string> FailedBrowsers { get { return this.browsersThatFailed;  } }

        private void IntializeFailedList()
        {
            browsersThatFailed = new List<string>();
        }

        private void AddBrowserToFailedList(BrowserDriver b, string msg = "")
        {
            browsersThatFailed.Add(b.DriverType + " " + msg);
        }

        public bool ScreenContains(string lookFor)
        {
            bool result = true;
            IntializeFailedList();

            foreach (BrowserDriver b in browsers)
            {
                if (!b.ScreenContains(lookFor))
                {
                    AddBrowserToFailedList(b, "does not contain " + lookFor);
                    result = false;
                }
            }

            return result;
        }

        public void SetTextOnControl(string controlIdOrCssSelector, string textToSet)
        {
            //Tony, see method above (in this file) for example
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

        public string CurrentFormName_OrPageURL
        {
            get { throw new NotImplementedException(); }
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
