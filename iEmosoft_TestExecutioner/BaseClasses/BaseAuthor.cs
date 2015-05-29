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
        
        public bool TestCaseFailed { get; protected set; }

        //** ABSTRACT METHODS **
        public abstract void SaveReport();
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
            if (this.currentTestCaseStep != null)
            {
                this.RecordStep(this.currentTestCaseStep);
            }

            this.currentTestCaseStep = new TestCaseStep()
            {
                StepDescription = stepDescription,
                ExpectedResult = expectedResult,
                SuppliedData = suppliedData,
                StepPassed = true
            };

            this.recordedSteps.Add(currentTestCaseStep);
        }
        
        public void CommitCurrentTestStep(bool wasSuccessful = true, string actualResult = "", string imageFile = "")
        {
            currentTestCaseStep.StepPassed = wasSuccessful;
            currentTestCaseStep.ActualResult = actualResult;
            currentTestCaseStep.ImageFilePath = imageFile;

            if (!wasSuccessful)
            {
                this.TestCaseFailed = true;
            }

            this.RecordStep(currentTestCaseStep);
        }

        public void FailCurrentTestStep(string actualResult = "", string imageFile = "")
        {
            CommitCurrentTestStep(false, actualResult,imageFile);
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
            this.TestCaseFailed = false;

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

        private void RecordStep(TestCaseStep step)
        {
            this.recordedSteps.Add(step);
            this.currentTestCaseStep = null;
            this.fileIsDirty = true;
        }


        public void Dispose()
        {
           //Subclasses can hide this and implement their own dispose if they wish
        }
    }
}
