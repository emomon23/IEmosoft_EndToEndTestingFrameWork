using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Office.Core;
using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;
using iEmosoft.TestRecorderModel;

namespace iEmosoft.RecordableBrowser
{
	public class TestCaseRecorder : ITestRecorder, IDisposable
	{
        List<TestCaseStep> recordedSteps = new List<TestCaseStep>();
        TestCaseData testCaseHeader;

		string testCaseTemplatePath;
		string newTestCasePath = "";
		string newTestCaseName = "";
		string rootTestCasesFolder = "";

        TestCaseStep currentTestCaseStep = null;

		bool fileIsDirty = false;
		bool templateWasFound = true;

		Excel.Application excelApp = new Excel.Application();
		Excel.Workbook workbook;
		Excel.Worksheet activateWorksheet;

		int currentStepIndex = 0;
		
		object MISSING = System.Reflection.Missing.Value;
        		
		int RED;
		int GREEN;
		int ORANGE;
		int SALMON;
		int DARK_RED;
		int DARK_GREEN;
        
		public bool TestCaseFailed { get; private set; }

        public List<TestCaseStep> RecordedSteps { get { return this.recordedSteps; } }

        public TestCaseData TestCaseHeader { get { return this.testCaseHeader; } }

        public TestCaseRecorder(string rootTestCasesFolder)
		{
			RED = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
			GREEN = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LimeGreen);
			ORANGE = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Orange);
			SALMON = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Tan);
			DARK_RED = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Brown);
			DARK_GREEN = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);

			this.rootTestCasesFolder = rootTestCasesFolder;
            this.testCaseTemplatePath = string.Format("{0}\\Resources\\TestCaseTemplate.xlsx", AppDomain.CurrentDomain.BaseDirectory);
		}

        public void BeginTestCaseStep(string stepDescription, string expectedResult, string suppliedData)
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

        public void BeginTestCaseStep(string stepDescription, string expectedResult)
        {
            this.BeginTestCaseStep(stepDescription, expectedResult, string.Empty);
        }

        public void BeginTestCaseStep(string stepDescription)
        {
            this.BeginTestCaseStep(stepDescription, string.Empty, string.Empty);
        }

        public void CommitTestStep()
        {
            CommitTestStep(true, string.Empty, string.Empty);
        }

        public void CommitTestStep(string actualResult)
        {
            CommitTestStep(true, actualResult, string.Empty);
        }

        public void CommitTestStep(bool wasSuccessful, string actualResult)
        {
            CommitTestStep(wasSuccessful, actualResult, string.Empty);
        }

        public void CommitTestStep(string actualResult, string imageFile)
        {
            CommitTestStep(true, actualResult, imageFile);
        }

        public void CommitTestStep(bool wasSuccessful, string actualResult, string imageFile)
        {
            currentTestCaseStep.StepPassed = wasSuccessful;
            currentTestCaseStep.ActualResult = actualResult;
            currentTestCaseStep.ImageFilePath = imageFile;

            this.RecordStep(currentTestCaseStep);
        }

       	public bool StartNewTestCase(TestCaseData testCaseHeader)
		{
           	if (this.newTestCasePath != "")
			{
				this.SaveRecordedTest();
			}

            this.testCaseHeader = testCaseHeader;

			this.newTestCaseName = testCaseHeader.TestName;

			string subFolder = string.IsNullOrEmpty(testCaseHeader.SubFolder) ? "" : "\\" + testCaseHeader.SubFolder;
			this.newTestCasePath = string.Format("{0}{1}", this.rootTestCasesFolder, subFolder);
			this.TestCaseFailed = false;

			this.templateWasFound = File.Exists(testCaseTemplatePath);
			if (! templateWasFound)
			{
				return false;
			}
			
			if (!Directory.Exists(newTestCasePath))
			{
				Directory.CreateDirectory(newTestCasePath);
			}
			newTestCasePath += "\\" + testCaseHeader.TestCaseFileName.Replace(".xlsx", "").Replace(".", "") + ".xlsx";

			this.workbook = excelApp.Workbooks.Open(testCaseTemplatePath, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING);
			this.activateWorksheet = workbook.ActiveSheet;

			if (! string.IsNullOrEmpty(testCaseHeader.TestNumber))
			{
				this.WriteToExcelFile("A1", testCaseHeader.TestNumber.ToString());
			}

			this.WriteToExcelFile("B2", testCaseHeader.Prereqs);
			this.WriteToExcelFile("B3", testCaseHeader.TestName);
			this.WriteToExcelFile("B4", testCaseHeader.Priority);
			this.WriteToExcelFile("B5", testCaseHeader.TestWriter);
			this.WriteToExcelFile("D4", testCaseHeader.ExecutedByName);
			this.WriteToExcelFile("D5", testCaseHeader.ExecutedOnDate);
			this.WriteToExcelFile("A8", testCaseHeader.TestDescription);

			this.currentStepIndex = 0;

			fileIsDirty = true;
			return true;
       	}

		public void RecordStep(TestCaseStep step)
		{
           	if (!this.templateWasFound)
			{
				return;
			}

			//The steps will be number by 10's, this will allow a person to manually insert line items between numbers.
			string stepNumber = ((this.currentStepIndex + 1) * 10).ToString();

			//steps being on row #14 in the test case template, as the currentStepIndex increases, so should the row we write too.
			string stepRow = (14 + this.currentStepIndex).ToString();
			this.currentStepIndex += 1;

			this.WriteToExcelFile("A" + stepRow, stepNumber);
			this.WriteToExcelFile("B" + stepRow, step.StepDescription);
			this.WriteToExcelFile("C" + stepRow, step.SuppliedData);
			this.WriteToExcelFile("D" + stepRow, step.ExpectedResult);
			this.WriteToExcelFile("E" + stepRow, step.ActualResult);
			this.WriteToExcelFile("F" + stepRow, step.StepPassed ? "True" : "FALSE!");
            this.WriteToExcelFile("H" + stepRow, step.Notes);

            if (!string.IsNullOrEmpty(step.ImageFilePath))
            {
                var range = activateWorksheet.Range["G" + stepRow];
                var hyperLink = activateWorksheet.Hyperlinks.Add(range, step.ImageFilePath, MISSING, MISSING, "Image");
            }

			string statusRow = "F" + stepRow;

			if (!step.StepPassed)
			{
				this.TestCaseFailed = true;
				this.SetCellsBackColor(statusRow, RED);
			}
			else
			{
				this.SetCellsBackColor(statusRow, GREEN);
			}

			fileIsDirty = true;
            this.currentTestCaseStep = null;
		}

		public void SaveRecordedTest()
		{
            if (this.currentTestCaseStep != null)
            {
                this.RecordStep(this.currentTestCaseStep);
            }

            if (fileIsDirty && templateWasFound)
			{
				int darkColor = this.TestCaseFailed ? DARK_RED : DARK_GREEN;
				int lightColor = this.TestCaseFailed ? RED : GREEN;
				string passFailText = this.TestCaseFailed ? "FAIL" : "PASSED";

				this.WriteToExcelFile("B1", passFailText);
				this.SetCellsBackColor("B1", lightColor);
				this.SetCellsBackColor("A1", darkColor);
				
				string newFileName = GetNextFileName();
				this.workbook.SaveAs(newFileName, MISSING, MISSING, MISSING, MISSING, MISSING, Excel.XlSaveAsAccessMode.xlExclusive, 2, MISSING, MISSING, MISSING, MISSING);
			}

			fileIsDirty = false;
		}

        public void SetBugRecord(string bugLink, string bugLinkText)
        {
            var range = activateWorksheet.Range["F4"];
            var hyperLink = activateWorksheet.Hyperlinks.Add(range, bugLink, MISSING, MISSING, bugLinkText);
        }

        public void Dispose()
        {
            if (this.newTestCasePath != "")
            {
                this.SaveRecordedTest();
            }

            /* can't get this code to work */
            workbook.Close(false, MISSING, MISSING);
            Marshal.FinalReleaseComObject(workbook);

            this.excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
        }

        private string GetNextFileName()
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

        public TestCaseStep CurrentStep { get { return this.currentTestCaseStep; } }

		private void WriteToExcelFile(string cell, string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				this.activateWorksheet.get_Range(cell, MISSING).Value = text;
			}
		}

		private void SetCellsBackColor(string cell, int color)
		{
			this.activateWorksheet.get_Range(cell).Interior.Color = color;
		}


        public bool PageContains(string lookFor)
        {
            throw new NotImplementedException();
        }

        public bool AmOnPage(string urlSnippet)
        {
            throw new NotImplementedException();
        }

        public void AssertAmOnPage(string urlSnippet)
        {
            throw new NotImplementedException();
        }

        public void AssertPageContains(string lookFor)
        {
            throw new NotImplementedException();
        }

        public void AssertPageNotContain(string lookFor)
        {
            throw new NotImplementedException();
        }
    }
}
