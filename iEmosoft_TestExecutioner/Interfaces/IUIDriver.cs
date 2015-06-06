using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iEmosoft.Automation.Interfaces
{
    public interface IUIDriver : IDisposable
    {
        bool ScreenContains(string lookFor);

        string DriverType { get;  }
        List<string> FailedBrowsers { get; }

        void SetTextOnControl(string controlIdOrCssSelector, string textToSet);
        void SetTextOnControl(string attributeName, string attributeValue, string textToSet, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        
        void ClickControl(string controlIdOrCssSelector);
        void ClickControl(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        string GetTextOnControl(string controlIdOrCssSelector);
        string GetTextOnControl(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        bool AmOnSceen(string snippetToLookFor);

        void SetValueOnDropDown(string controlIdOrCssSelector, string valueToSet);
        void SetValueOnDropDown(string attributeName, string attributeValue, string valueToSet = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        bool IsCheckBoxChecked(string controlIdOrCssSelector);
        bool IsCheckBoxChecked(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        void SetCheckBoxValueTo(string controlIdOrCssSelector, bool valueItShouldBeSetTo);
        void SetCheckBoxValueTo(string attributeName, string attributeValue, bool valueItShouldBeSetTo, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        string GetTextOnDropDown(string controlIdOrCssSelector);
        string GetTextOnDropDown(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        string GetValueOnDropDown(string controlIdOrCssSelector);
        string GetValueOnDropDown(string attributeName, string attributeValue, string controlType = "",
            bool useWildCardSearch = true, int retryForSeconds = 10);

        string CurrentFormName_OrPageURL { get; }

        void NavigateTo(string windowNameOrUri);
        void Launch(string appNameOrUri);

        void Pause(int milliseconds);

        void MaximizeWindow();
    }
}
