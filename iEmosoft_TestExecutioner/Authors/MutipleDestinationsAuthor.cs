using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.Authors
{
    public class MultipleDestinationsAuthor : BaseAuthor
    {
        public MultipleDestinationsAuthor()
        {
            this.Authors = new List<BaseAuthor>();
        }

        public List<BaseAuthor> Authors { get; set; }
        
        public override void SaveReport()
        {
            foreach (var author in Authors)
            {
                author.SaveReport();
            }
        }

        public override bool StartNewTestCase(Model.TestCaseHeaderData headerData)
        {
            bool result = false;
            foreach (var author in Authors)
            {
               result = author.StartNewTestCase(headerData);
            }

            return result;
        }

        public List<TestCaseStep> RecordedSteps
        {
            get { return Authors[0].RecordedSteps; }
        }

        public TestCaseHeaderData TestCaseHeader
        {
            get { return Authors[0].TestCaseHeader; }
        }

        public bool AddTestStep(string stepDescription, string expectedResult = "", string suppliedData = "", bool wasSuccessful = true, string actualResult = "", string imageFile = "")
        {
            bool result = true;

            foreach (var author in Authors)
            {
                result = author.AddTestStep(stepDescription, expectedResult, suppliedData, wasSuccessful, actualResult,
                    imageFile);
            }

            return result;
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult = "", string suppliedData = "")
        {
            foreach (var author in Authors)
            {
                author.BeginTestCaseStep(stepDescription, expectedResult, suppliedData);
            }
        }
               
        public TestCaseStep CurrentStep
        {
            get
            {
                throw new Exception("Can't access the current step directly on a multiple distination author");
            }
        }
    }
}
