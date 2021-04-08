namespace aUI.Automation
{
    public class JQuerySelector
    {
        public JQuerySelector() { }
        public JQuerySelector(string script)
        {
            JQuerySelectorScript = script;
        }
        public string JQuerySelectorScript { get; set; }
    }

    public static class StringJQuerySelectorExtensionClass
    {
        public static JQuerySelector ToJQuerySelector(this string str)
        {
            return new JQuerySelector(str);
        }
    }
}
