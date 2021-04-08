using Newtonsoft.Json;

namespace aUI.Automation.Baseline
{
    public abstract class BaseTestResult
    {
        abstract public string TestResultKey { get; }
        abstract public BaselineDescrepencyCheck CompareResults(BaseTestResult testResult);
        abstract public void ReadFromJSONString(string json);

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
