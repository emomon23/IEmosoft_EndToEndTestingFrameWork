using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEmosoft.Automation
{
    public class JQuerySelector
    {
        public JQuerySelector() { }
        public JQuerySelector(string script)
        {
            this.jQuerySelectorScript = script;
        }
        public string jQuerySelectorScript { get; set; }
    }

    public static class StringJQuerySelectorExtensionClass
    {
        public static JQuerySelector ToJQuerySelector(this string str)
        {
            return new JQuerySelector(str);
        }
    }
}
