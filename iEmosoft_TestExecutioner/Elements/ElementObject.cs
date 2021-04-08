using aUI.Automation.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace aUI.Automation.Elements
{
    public class ElementObject
    {
        public ElementType EleType = ElementType.Id;
        public string EleRef = "";
        public int MaxWait = 10;
        //build a few helper methods to create 'CustomCondition'
        public Func<IWebDriver, By, (IWebElement, bool)> CustomCondition = null;
        public Func<IWebDriver, By, (List<IWebElement>, bool)> CustomConditionMulti = null;

        public Wait WaitType = Wait.Visible;
        public string Text = "";
        public ElementAction Action = ElementAction.GetText;

        public bool Scroll = true;
        public bool Random = false;
        public bool Clear = true;
        public bool ProtectedValue = false;

        public string ElementName = "";

        /// <summary>
        /// Option to replace parts of a preset enum with runtime values.
        /// Key - what to replace
        /// Value - what to replace it with
        /// </summary>
        public Dictionary<string, string> RuntimeRefUpdate = new();


        public ElementObject()
        {
        }

        public ElementObject(Enum eleRef)
        {
            //TODO Expand constructors
            EleType = eleRef.Type();
            EleRef = eleRef.Ref();
            ElementName = eleRef.ToString();
        }

        /* CustomCondition Example:
         *
        private void help()
        {
            Func<IWebDriver, By, (IWebElement, bool)> test = (driver, by) => 
            {
                var element = driver.FindElement(by);
                if (element.GetAttribute("id").Contains("bob"))
                {
                    return (element, true);
                }
                return (null, false);
            };
        }
        */
    }
}
