using OpenQA.Selenium;
using System;

namespace aUI.Automation.Elements
{
    public class ElementResult
    {
        public TestExecutioner TE = null;


        public string Text = "";
        public string AttributeText = "";
        public string ElementName = "";
        public IWebElement RawEle = null;
        public bool Success = false;
        public Exception Exception = null;

        public ElementResult(TestExecutioner tE)
        {
            TE = tE;
        }
    }
}
