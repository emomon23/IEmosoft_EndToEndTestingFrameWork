using OpenQA.Selenium;

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


        public ElementResult(TestExecutioner tE)
        {
            TE = tE;
        }
    }
}
