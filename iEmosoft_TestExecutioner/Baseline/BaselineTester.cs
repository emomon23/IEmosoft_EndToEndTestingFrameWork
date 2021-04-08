using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace aUI.Automation.Baseline
{
    public class BaselineTester : IDisposable
    {
        private string BaselinePath = "";
        private List<BaselineDescrepencyCheck> Descrepencies = new();
        private Type ConcreateTestResultType;

        public BaselineTester(string baselineFileName, Type testResultImplementationType)
        {
            BaselinePath = Path.GetDirectoryName(baselineFileName) + "\\";
            ConcreateTestResultType = testResultImplementationType;

            if (!Directory.Exists(BaselinePath))
            {
                Directory.CreateDirectory(BaselinePath);
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
                    Descrepencies.Add(descrepency);
                }
            }
        }

        public void WriteBaseline(BaseTestResult testResult)
        {
            string path = BaselinePath + testResult.TestResultKey + ".json";
            File.WriteAllText(path, testResult.ToJSON());
        }

        private BaseTestResult GetBaselineTestResult(string key)
        {
            string path = BaselinePath + key + ".json";

            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            BaseTestResult result = (BaseTestResult)Activator.CreateInstance(ConcreateTestResultType);
            result.ReadFromJSONString(json);

            return result;
        }

        public void Dispose()
        {
            string message = "";

            if (Descrepencies.Count > 0)
            {
                string descrpencyFilePath = string.Format("{0}\\Descrepencies_{1}_{2}.{3}", BaselinePath, ConcreateTestResultType.ToString(),
                    Guid.NewGuid().ToString().Substring(0, 5), "json");

                WriteDescrepencies(descrpencyFilePath);

                message +=
                    string.Format(
                        "There are {0} descrepencies between baseline and this test run.  View {1} to see the descrepencies",
                        Descrepencies.Count, descrpencyFilePath);

                Assert.IsTrue(false, message);
            }

            if (message != "")
            {
                Assert.Inconclusive(message);
            }
            GC.SuppressFinalize(this);
        }


        private void WriteDescrepencies(string fileName)
        {
            var jsonString = JsonConvert.SerializeObject(Descrepencies);
            File.WriteAllText(fileName, jsonString);
        }
    }
}
