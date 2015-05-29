using System;
using System.IO;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.Authors
{
    public class HTMLAuthor : BaseAuthor, IDisposable
    {
        private string rawHTMLTemplate = "";
        private string rawUnattachedStepRowHTML;

        private string unattachedRowCommentFrontDelimiter = "<!--STEP ROW";
        private string unattachedRowCommentBackDelimiter = "-->";

        public HTMLAuthor(string rootTestCasesFolderOrAppSettingName)
        {
            try
            {
                base.rootTestCasesFolder =
                    System.Configuration.ConfigurationManager.AppSettings[rootTestCasesFolder].ToString();
            }
            catch
            {
                base.rootTestCasesFolder = rootTestCasesFolderOrAppSettingName;
            }

            this.testCaseTemplatePath = string.Format("{0}\\Resources\\TestReportTemplate.html", AppDomain.CurrentDomain.BaseDirectory);
        }

        public override void SaveReport()
        {
            rawHTMLTemplate = File.ReadAllText(base.testCaseTemplatePath);
            rawUnattachedStepRowHTML = GetUnattachedHTMLStepRow();

            if (base.fileIsDirty)
            {
                if (this.currentTestCaseStep != null)
                {
                    base.CommitCurrentTestStep();
                }

                WriteTestCaseHeaderToHTMLDocument();
                WriteStepsToHTMLDocument();
                UpdatePassFailStatusForWholeTest();
                SaveNewHTMLFileToDisk();
                base.fileIsDirty = false;
            }
        }

        public override bool StartNewTestCase(TestCaseHeaderData headerData)
        {
            SaveReport();
            Dispose();

            bool result = base.InitialzieNewTestCase(testCaseHeader);

            if (result)
            {
                //newTestCasePath gets initialized based on the testCaseHeader parameter
                base.newTestCasePath += "\\" + testCaseHeader.TestCaseFileName.Replace(".html", "").Replace(".", "") + ".html";
            }
            
            return result;
        }
        
        private void WriteTestCaseHeaderToHTMLDocument()
        {
            rawHTMLTemplate = rawHTMLTemplate.Replace("[PRE_REQS]", base.testCaseHeader.Prereqs);
            rawHTMLTemplate = rawHTMLTemplate.Replace("[TEST_TITLE]", base.testCaseHeader.TestName);
            rawHTMLTemplate = rawHTMLTemplate.Replace("[PRIORITY]", base.testCaseHeader.Priority);
            rawHTMLTemplate = rawHTMLTemplate.Replace("[AUTHOR]", base.testCaseHeader.TestWriter);
            rawHTMLTemplate = rawHTMLTemplate.Replace("[EXECUTED_BY]", base.testCaseHeader.ExecutedByName);
            rawHTMLTemplate = rawHTMLTemplate.Replace("[EXECUTED_DATE]", base.testCaseHeader.ExecutedOnDate);
        }

        private void WriteStepsToHTMLDocument()
        {
            bool isEven = false;
            
            foreach (var step in recordedSteps)
            {
                isEven = isEven ? false : true;
                WriteTestStepToHTMLDocument(step, isEven);
            }
            
        }

        private void WriteTestStepToHTMLDocument(TestCaseStep step, bool isEvenRow)
        {
            string evenOddRowText = isEvenRow ? "evenRow" : "oddRow";
            string newStepRowHTML = rawUnattachedStepRowHTML.Replace("[ODD_EVEN_ROW]", evenOddRowText);

            newStepRowHTML = newStepRowHTML.Replace("[STEP_NUMBER]", base.GetNextStepSequenceNumberString());
            newStepRowHTML = newStepRowHTML.Replace("[STEP_DESCRIPTION]", step.StepDescription);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_SUPPLIED_DATE]", step.SuppliedData);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_EXPECTED_RESULT]", step.ExpectedResult);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_ACTUAL_RESULT]", step.ActualResult);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_PASS_FAIL]", step.StepPassed? "PASS" : "FAIL");
            newStepRowHTML = newStepRowHTML.Replace("[IMAGE_PATH]", step.ImageFilePath);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_NOTES]", step.Notes);

            string replaceText = string.Format("{0}{1}{2}", newStepRowHTML, Environment.NewLine, unattachedRowCommentFrontDelimiter);
            rawHTMLTemplate = rawHTMLTemplate.Replace(unattachedRowCommentFrontDelimiter, replaceText);
        }

        private string GetUnattachedHTMLStepRow()
        {
            int startingIndex = rawHTMLTemplate.IndexOf(unattachedRowCommentFrontDelimiter);

            if (startingIndex == -1)
            {
                throw new Exception(string.Format("Unable to find {0} in HTML template, unable to create report", unattachedRowCommentFrontDelimiter));
            }

            int endingIndex = rawHTMLTemplate.IndexOf(unattachedRowCommentBackDelimiter, startingIndex);

            startingIndex += unattachedRowCommentFrontDelimiter.Length;
            return rawHTMLTemplate.Substring(startingIndex, endingIndex - startingIndex);
        }

        private void UpdatePassFailStatusForWholeTest()
        {
            rawHTMLTemplate = rawHTMLTemplate.Replace("[TEST_NUMBER]", base.testCaseHeader.TestNumber);
            rawHTMLTemplate = rawHTMLTemplate.Replace("[TEST_NUMBER_CLASS]", base.TestCaseFailed ? "failTestNumber" : "passTestNumber");
            rawHTMLTemplate = rawHTMLTemplate.Replace("[PASS_FAIL]", base.TestCaseFailed ? "FAIL" : "PASS");
            rawHTMLTemplate = rawHTMLTemplate.Replace("[PASS_FAIL_CLASS]", base.TestCaseFailed ? "failColor" : "passColor");
            
        }

        private void SaveNewHTMLFileToDisk()
        {
           string newFileName = GetNextFileName();
           File.WriteAllText(newFileName, rawHTMLTemplate);
        }
    }
    
}