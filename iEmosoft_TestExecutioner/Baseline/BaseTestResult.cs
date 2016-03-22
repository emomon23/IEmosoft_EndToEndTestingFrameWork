using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace iEmosoft.Automation
{
    public abstract class BaseTestResult
    {
        abstract public string TestResultKey { get; }
        abstract public BaselineDescrepencyCheck CompareResults(BaseTestResult testResult);
        abstract public void ReadFromJSONString(string json);

        public string ToJSON()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}
