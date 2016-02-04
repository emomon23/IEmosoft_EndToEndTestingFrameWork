using System;
using System.Collections.Generic;
using System.IO;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.BaseClasses
{
    public abstract class BaseAuthor : IDisposable
    {
        private int nextStepNumber = 0;

        protected List<TestCaseStep> recordedSteps = new List<TestCaseStep>();
        protected TestCaseHeaderData testCaseHeader;

        protected string testCaseTemplatePath;
        protected string newTestCasePath = "";
        protected string newTestCaseName = "";
        protected string rootTestCasesFolder = "";

        protected TestCaseStep currentTestCaseStep = null;

        protected bool fileIsDirty = false;
        protected bool templateWasFound = true;

        public bool TestCaseFailed
        {
            get
            {
                bool result = false;

                for (var i = 0; i < recordedSteps.Count; i++)
                {
                    if (recordedSteps[i].StepPassed == false)
                    {
                        result = true;
                        break;
                    }
                }

                return result;
            }
        }

        //** ABSTRACT METHODS **
        public abstract string SaveReport();
        public abstract bool StartNewTestCase(TestCaseHeaderData headerData);
        
        public List<TestCaseStep> RecordedSteps
        {
            get { return this.recordedSteps; }
        }

        public TestCaseHeaderData TestCaseHeader
        {
            get { return this.testCaseHeader; }
        }

        public bool AddTestStep(string stepDescription, string expectedResult = "", string suppliedData = "", bool wasSuccessful = true, string actualResult = "", string imageFile = "")
        {
            return false;
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult = "", string suppliedData = "")
        {
            this.fileIsDirty = true;

            this.currentTestCaseStep = new TestCaseStep()
            {
                StepDescription = stepDescription,
                ExpectedResult = expectedResult,
                SuppliedData = suppliedData,
                StepPassed = true
            };

            this.recordedSteps.Add(currentTestCaseStep);
        }
   
        public TestCaseStep CurrentStep
        {
            get { return this.currentTestCaseStep; }
        }

        protected bool InitialzieNewTestCase(TestCaseHeaderData testCaseHeader)
        {
            this.nextStepNumber = 0;
            this.testCaseHeader = testCaseHeader;
            this.newTestCaseName = testCaseHeader.TestName;

            string subFolder = string.IsNullOrEmpty(testCaseHeader.SubFolder) ? "" : "\\" + testCaseHeader.SubFolder;
            this.newTestCasePath = string.Format("{0}{1}", this.rootTestCasesFolder, subFolder);
      
            this.templateWasFound = File.Exists(testCaseTemplatePath);
            if (!templateWasFound)
            {
                return false;
            }

            if (!Directory.Exists(newTestCasePath))
            {
                Directory.CreateDirectory(newTestCasePath);
            }
            
            this.testCaseHeader = testCaseHeader;
            this.recordedSteps = new List<TestCaseStep>();

            fileIsDirty = true;
            return true;
        }
        
        protected string GetNextFileName()
        {
            string result = this.newTestCasePath;
            int ctr = 0;

            while (File.Exists(result))
            {
                ctr += 1;
                result = this.newTestCasePath.Replace(".", ctr.ToString() + ".");
            }

            if (this.TestCaseFailed)
            {
                string fileName = Path.GetFileName(result);

                result = result.Replace(fileName, "Failed - " + fileName);
            }

            return result;
        }

        protected string GetNextStepSequenceNumberString()
        {
            nextStepNumber += 1;
            return (nextStepNumber*10).ToString();
        }
               
        public void Dispose()
        {
           //Subclasses can hide this and implement their own dispose if they wish
        }
    }
}
