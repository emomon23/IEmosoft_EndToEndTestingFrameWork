using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iEmosoft.Automation
{
    public class BaselineTester : IDisposable
    {
        private string baselinePath = "";
        private bool baselineFileExists = false;
                
        private List<BaselineDescrepencyCheck> descrepencies = new List<BaselineDescrepencyCheck>();
        private Type concreateTestResultType;

        public BaselineTester(string baselineFileName, Type testResultImplementationType)
        {
            this.baselinePath = Path.GetDirectoryName(baselineFileName) + "\\";
            this.concreateTestResultType = testResultImplementationType;

            if (!Directory.Exists(baselinePath))
            {
                Directory.CreateDirectory(baselinePath);
            }
        }

        public void HandleResult(BaseTestResult testResult)
        {
            BaseTestResult baselineTestResult = GetBaselineTestResult(testResult.TestResultKey);
            if (baselineTestResult == null)
            {
                WriteBaseline(testResult);
            }
            else
            {
                var descrepency = baselineTestResult.CompareResults(testResult);
                if (descrepency != null && descrepency.Mismatches != null && descrepency.Mismatches.Count > 0)
                {
                    this.descrepencies.Add(descrepency);
                }
            }
        }

        public void WriteBaseline(BaseTestResult testResult)
        {
            string path = baselinePath + testResult.TestResultKey + ".json";
            File.WriteAllText(path, testResult.ToJSON());
        }

        private BaseTestResult GetBaselineTestResult(string key)
        {
            string path = baselinePath + key + ".json";

            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            BaseTestResult result = (BaseTestResult)Activator.CreateInstance(this.concreateTestResultType);
            result.ReadFromJSONString(json);

            return result;
        }

        public void Dispose()
        {
            string message = "";

            if (descrepencies.Count > 0)
            {
                string descrpencyFilePath = string.Format("{0}\\Descrepencies_{1}_{2}.{3}", baselinePath, this.concreateTestResultType.ToString(),
                    Guid.NewGuid().ToString().Substring(0, 5), "json");

                WriteDescrepencies(descrpencyFilePath);

                message +=
                    string.Format(
                        "There are {0} descrepencies between baseline and this test run.  View {1} to see the descrepencies",
                        descrepencies.Count, descrpencyFilePath);

                Assert.IsTrue(false, message);
            }

            if (message != "")
            {
                Assert.Inconclusive(message);
            }
        }

  
        private void WriteDescrepencies(string fileName)
        {
            var jsonString = new JavaScriptSerializer().Serialize(descrepencies);
            File.WriteAllText(fileName, jsonString);
        }
    }
}
