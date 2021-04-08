using OpenQA.Selenium;
using System;

namespace aUI.Automation.Elements
{
    public class ElementResult
    {
        public TestExecutioner TE = null;
        private ElementActions EA = null;


        public string Text = "";
        public string AttributeText = "";
        public IWebElement RawEle = null;
        public bool Success = false;
        public Exception Exception = null;

        public ElementResult(TestExecutioner tE)
        {
            TE = tE;
        }
    }
}
