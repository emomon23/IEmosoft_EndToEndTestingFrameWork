using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace iEmosoft.Automation
{
    public class BaselineTester : IDisposable
    {
        private string baselinePath = "";
        private string baselineFileName = "";
        private bool baselineFileExists = false;

        private List<BaseTestResult> baselineTestResults = new List<BaseTestResult>();
        private List<BaseTestResult> newTestResultsNotInBaseline = new List<BaseTestResult>(); 
        private List<BaselineDescrepencyCheck> descrepencies = new List<BaselineDescrepencyCheck>();
        private Type concreateTestResultType;

        public BaselineTester(string baselineFileName, Type testResultImplementationType)
        {
            this.baselinePath = Path.GetDirectoryName(baselineFileName);
            this.baselineFileName = baselineFileName;
            this.concreateTestResultType = testResultImplementationType;

            var x = Activator.CreateInstance(concreateTestResultType);
            if (!Directory.Exists(baselinePath))
            {
                Directory.CreateDirectory(baselinePath);
            }

            if (File.Exists(baselineFileName))
            {
                baselineFileExists = true;
                ReadBaselineFile();
            }
        }

        public void HandleResult(BaseTestResult testResult)
        {
            if (this.baselineFileExists)
            {
                //A baseline file exists, so we are new comparing the testResult parameter with the baseline file
                var baselineTestResult = baselineTestResults.FirstOrDefault(t => t.TestResultKey == testResult.TestResultKey);
                if (baselineTestResult != null)
                {
                    var compareResult = baselineTestResult.CompareResults(testResult);
                    if (compareResult != null && compareResult.Mismatches != null && compareResult.Mismatches.Count > 0)
                    {
                        compareResult.Key = testResult.TestResultKey;
                        this.descrepencies.Add(compareResult);
                    }
                }
                else
                {
                    newTestResultsNotInBaseline.Add(testResult);
                }
            }
            else
            {
                //A baseline file does not exist, so we are in the process of creating a baseline file
                baselineTestResults.Add(testResult);
            }
        }


        public void Dispose()
        {
            if (!baselineFileExists)
            {
                WriteListOfTestResults(baselineTestResults, baselineFileName);
            }
            else
            {
                string message = "";

                if (newTestResultsNotInBaseline.Count > 0)
                {
                    string newResultsFile = string.Format("{0}\\New_{1}_{2}.{3}",baselinePath, this.concreateTestResultType.ToString(),
                        Guid.NewGuid().ToString().Substring(0, 5), Path.GetExtension(baselineFileName));

                    WriteListOfTestResults(newTestResultsNotInBaseline, newResultsFile);
                    message =
                        string.Format(
                            "New test results exist in this run that do not exist in the baseline run.  See {0} for a list of test test results.  Delete the baseline file and run the tests again to generate a new baseline\n\n",
                            newResultsFile);
                }

                if (descrepencies.Count > 0)
                {
                    string descrpencyFilePath = string.Format("{0}\\Descrepencies_{1}_{2}.{3}",baselinePath, this.concreateTestResultType.ToString(),
                        Guid.NewGuid().ToString().Substring(0, 5), Path.GetExtension(baselineFileName));

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
        }

        private void ReadBaselineFile()
        {
            string [] jsonRecords = File.ReadAllText(baselineFileName).Split('\n');
            foreach (string jsonRecord in jsonRecords)
            {
                var objTestResult = (BaseTestResult)Activator.CreateInstance(concreateTestResultType);
                objTestResult.ReadFromJSONString(jsonRecord.Replace("\n", ""));

                this.baselineTestResults.Add(objTestResult);
            }

        }

        private void WriteListOfTestResults(List<BaseTestResult> resultsToWrite, string fileName )
        {
            string jsonSeperator = "\n";
            string jsonString = "";

            foreach (var testResult in resultsToWrite)
            {
                var tempString = testResult.ToJSON().Replace(jsonSeperator, "");
                jsonString += tempString + jsonSeperator;
            }

            File.WriteAllText(fileName, jsonString);
        }
        
        private void WriteDescrepencies(string fileName)
        {
            var jsonString = new JavaScriptSerializer().Serialize(descrepencies);
            File.WriteAllText(fileName, jsonString);
        }
    }

    public class BaselineDescrepencyCheck
    {
        public string Key { get; set; }
        public BaselineDescrepencyCheck()
        {
            this.Mismatches = new List<Mismatch>();
        }
        
        public List<Mismatch> Mismatches { get; set; }

        public void InsertMismatch(string fieldName, string expectedValue, string actualValule, bool append = true)
        {
            var mismatch = new Mismatch()
            {
                ActualValue = actualValule,
                ExpectedValue = expectedValue,
                PropertyName = fieldName
            };

            if (append)
            {
                this.Mismatches.Add(mismatch);
            }
            else
            {
                this.Mismatches.Insert(0, mismatch);
            }
        }
    }

    public class Mismatch
    {
        public string PropertyName { get; set; }
        public string ExpectedValue { get; set; }
        public string ActualValue { get; set; }
    }
}
