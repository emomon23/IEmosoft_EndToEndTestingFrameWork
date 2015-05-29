using System;
using System.Runtime.InteropServices;
using iEmosoft.Automation.Model;
using Excel = Microsoft.Office.Interop.Excel;
using iEmosoft.Automation.BaseClasses;

namespace iEmosoft.Automation
{
	public class ExcelAuthor : BaseAuthor, IDisposable
	{
	    private int currentStepIndexWrittenToFile = 0;

        Excel.Application excelApp = new Excel.Application();
		Excel.Workbook workbook = null;
		Excel.Worksheet activateWorksheet;
        
		object MISSING = System.Reflection.Missing.Value;
        		
		int RED;
		int GREEN;
		int ORANGE;
		int SALMON;
		int DARK_RED;
		int DARK_GREEN;
        
        public ExcelAuthor(string rootTestCasesFolderOrAppSettingName)
		{
			RED = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
			GREEN = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LimeGreen);
			ORANGE = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Orange);
			SALMON = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Tan);
			DARK_RED = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Brown);
			DARK_GREEN = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);

            try
            {
                base.rootTestCasesFolder =
                    System.Configuration.ConfigurationManager.AppSettings[rootTestCasesFolder].ToString();
            }
            catch
            {
                base.rootTestCasesFolder = rootTestCasesFolderOrAppSettingName;
            }
            this.testCaseTemplatePath = string.Format("{0}\\Resources\\TestCaseTemplate.xlsx", AppDomain.CurrentDomain.BaseDirectory);
		}
       
       	public override bool StartNewTestCase(TestCaseHeaderData testCaseHeader)
       	{
            SaveReport();
            Dispose();

       	    bool result = base.InitialzieNewTestCase(testCaseHeader);

       	    if (result)
       	    {
                //newTestCasePath gets initialized based on the testCaseHeader parameter
       	        base.newTestCasePath += "\\" + testCaseHeader.TestCaseFileName.Replace(".xlsx", "").Replace(".", "") + ".xlsx";
       	    }

            currentStepIndexWrittenToFile = 0;
            return result;
       	}
        
        public override void SaveReport()
        {
            if (base.fileIsDirty)
            {
                if (this.currentTestCaseStep != null)
                {
                    base.CommitCurrentTestStep();
                }
                
                WriteTestCaseHeaderToExcelDocument();
                WriteStepsToExcel();
                UpdatePassFailStatusForWholeTest();
                SaveExcelFileToDisk();
                base.fileIsDirty = false;
            }
        }

	    public void Dispose()
	    {
	        try
	        {
	            if (activateWorksheet != null)
	            {
	                Marshal.FinalReleaseComObject(activateWorksheet);
	                activateWorksheet = null;
	            }
	        }
	        catch
	        {
	        }

	        try
	        {
	            if (workbook != null)
	            {
	                Marshal.FinalReleaseComObject(workbook);
	                workbook = null;
	            }
	        }
	        catch
	        {
	        }

	        try
	        {
	            if (excelApp != null)
	            {
	                Marshal.FinalReleaseComObject(excelApp);
	                excelApp = null;
	            }
	        }catch {}
	    }
	
	    private void WriteTestCaseHeaderToExcelDocument()
	    {
            //If the workbook is not null, then we've already written the header
	        if (workbook == null)
	        {
	            this.workbook = excelApp.Workbooks.Open(testCaseTemplatePath, MISSING, MISSING, MISSING, MISSING,
	                MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING, MISSING);
	            this.activateWorksheet = workbook.ActiveSheet;


	            if (!string.IsNullOrEmpty(testCaseHeader.TestNumber))
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
	        }
	    }

	    private void WriteStepsToExcel()
	    {
	        for (int i=currentStepIndexWrittenToFile; i< recordedSteps.Count; i++)
	        {
	            this.WriteStepToExcel(recordedSteps[i]);
	        }    
	    }

        private void WriteStepToExcel(TestCaseStep step)
		{
           	if (!this.templateWasFound)
			{
				return;
			}

			//The steps will be number by 10's, this will allow a person to manually insert line items between numbers.
			string stepNumber = ((this.currentStepIndexWrittenToFile + 1) * 10).ToString();

			//steps being on row #14 in the test case template, as the currentStepIndex increases, so should the row we write too.
			string stepRow = (14 + this.currentStepIndexWrittenToFile).ToString();
			this.currentStepIndexWrittenToFile += 1;

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

	    private void SaveExcelFileToDisk()
	    {
            string newFileName = GetNextFileName();
            this.workbook.SaveAs(newFileName, MISSING, MISSING, MISSING, MISSING, MISSING, Excel.XlSaveAsAccessMode.xlExclusive, 2, MISSING, MISSING, MISSING, MISSING);
        }

        private void UpdatePassFailStatusForWholeTest(){
	        if (fileIsDirty && templateWasFound)
            {
                int darkColor = this.TestCaseFailed ? DARK_RED : DARK_GREEN;
                int lightColor = this.TestCaseFailed ? RED : GREEN;
                string passFailText = this.TestCaseFailed ? "FAIL" : "PASSED";

                this.WriteToExcelFile("B1", passFailText);
                this.SetCellsBackColor("B1", lightColor);
                this.SetCellsBackColor("A1", darkColor);
           }

            fileIsDirty = false;
	    }
       
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
    }
}
