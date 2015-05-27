using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation
{
    public interface ITestAuthor : IDisposable
    {
		bool TestCaseFailed { get; }
	    void BeginTestCaseStep(string stepDescription, string expectedResult, string suppliedData);
        void BeginTestCaseStep(string stepDescription, string expectedResult);
        void BeginTestCaseStep(string stepDescription);
        void CommitTestStep();
        void CommitTestStep(string actualResult);
        void CommitTestStep(bool wasSuccessful, string actualResult);
        void CommitTestStep(string actualResult, string imageFile);
        void CommitTestStep(bool wasSuccessful, string actualResult, string imageFile);
       	bool StartNewTestCase(TestCaseData testCaseHeader);
		void RecordStep(TestCaseStep step);
        void SetBugRecord(string bugLink, string bugLinkText);
		void SaveRecordedTest();
        TestCaseStep CurrentStep { get; }
        List<TestCaseStep> RecordedSteps { get; }
        TestCaseData TestCaseHeader { get; }
    }
}
