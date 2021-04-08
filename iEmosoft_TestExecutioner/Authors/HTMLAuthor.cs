using aUI.Automation.BaseClasses;
using aUI.Automation.ModelObjects;
using System;
using System.IO;

namespace aUI.Automation.Authors
{
    public class HTMLAuthor : BaseAuthor, IDisposable
    {
        private string RawHTMLTemplate = "";
        private string RawUnattachedStepRowHTML;

        private string UnattachedRowCommentFrontDelimiter = "<!--STEP ROW";
        private string UnattachedRowCommentBackDelimiter = "-->";

        public HTMLAuthor(string rootTestCasesFolderOrAppSettingName)
        {
            try
            {
                RootTestCasesFolder =
                    System.Configuration.ConfigurationManager.AppSettings[RootTestCasesFolder].ToString();
            }
            catch
            {
                RootTestCasesFolder = rootTestCasesFolderOrAppSettingName;
            }
        }

        public override string SaveReport()
        {
            string result = "";

            RawHTMLTemplate = HtmlTemplate;// File.ReadAllText(base.testCaseTemplatePath);
            RawUnattachedStepRowHTML = GetUnattachedHTMLStepRow();

            if (FileIsDirty)
            {
                WriteTestCaseHeaderToHTMLDocument();
                WriteStepsToHTMLDocument();
                UpdatePassFailStatusForWholeTest();
                result = SaveNewHTMLFileToDisk();
                FileIsDirty = false;
            }

            return result;
        }

        public override bool StartNewTestCase(TestCaseHeaderData headerData)
        {
            SaveReport();
            Dispose();

            bool result = InitialzieNewTestCase(headerData);

            if (result)
            {
                //newTestCasePath gets initialized based on the testCaseHeader parameter
                NewTestCasePath += "\\" + TestCaseHeader.TestCaseFileName.Replace(".html", "").Replace(".", "") + ".html";
            }

            return result;
        }

        private void WriteTestCaseHeaderToHTMLDocument()
        {
            RawHTMLTemplate = RawHTMLTemplate.Replace("[PRE_REQS]", TestCaseHeader.Prereqs);
            RawHTMLTemplate = RawHTMLTemplate.Replace("[TEST_TITLE]", TestCaseHeader.TestName);
            RawHTMLTemplate = RawHTMLTemplate.Replace("[PRIORITY]", TestCaseHeader.Priority);
            RawHTMLTemplate = RawHTMLTemplate.Replace("[AUTHOR]", TestCaseHeader.TestWriter);
            RawHTMLTemplate = RawHTMLTemplate.Replace("[EXECUTED_BY]", TestCaseHeader.ExecutedByName);
            RawHTMLTemplate = RawHTMLTemplate.Replace("[EXECUTED_DATE]", TestCaseHeader.ExecutedOnDate);

            string description = TestCaseHeader.TestDescription.Replace("When", "<br/>When").Replace("WHEN", "<br/>WHEN").Replace("Then", "<br/>Then").Replace("THEN", "<br/>THEN").Replace("And", "<br/>&nbsp;&nbsp;&nbsp;And");
            RawHTMLTemplate = RawHTMLTemplate.Replace("[GIVEN_WHEN_THEN]", description);
        }

        private void WriteStepsToHTMLDocument()
        {
            bool isEven = false;

            foreach (var step in RecordedSteps)
            {
                isEven = !isEven;
                WriteTestStepToHTMLDocument(step, isEven);
            }
        }

        private void WriteTestStepToHTMLDocument(TestCaseStep step, bool isEvenRow)
        {
            string evenOddRowText = isEvenRow ? "evenRow" : "oddRow";
            string newStepRowHTML = RawUnattachedStepRowHTML.Replace("[ODD_EVEN_ROW]", evenOddRowText);

            newStepRowHTML = newStepRowHTML.Replace("[STEP_NUMBER]", GetNextStepSequenceNumberString());
            newStepRowHTML = newStepRowHTML.Replace("[STEP_DESCRIPTION]", step.StepDescription);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_SUPPLIED_DATE]", step.SuppliedData);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_EXPECTED_RESULT]", step.ExpectedResult);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_ACTUAL_RESULT]", step.ActualResult);
            newStepRowHTML = newStepRowHTML.Replace("[STEP_PASS_FAIL]", step.StepPassed ? "PASS" : "FAIL");
            newStepRowHTML = newStepRowHTML.Replace("[IMAGE_PATH]", step.ImageFilePath);

            if (string.IsNullOrEmpty(step.ImageFilePath))
            {
                newStepRowHTML = newStepRowHTML.Replace(">Image<", "><");
            }

            newStepRowHTML = newStepRowHTML.Replace("[STEP_NOTES]", step.Notes);

            string replaceText = string.Format("{0}{1}{2}", newStepRowHTML, Environment.NewLine, UnattachedRowCommentFrontDelimiter);
            RawHTMLTemplate = RawHTMLTemplate.Replace(UnattachedRowCommentFrontDelimiter, replaceText);
        }

        private string GetUnattachedHTMLStepRow()
        {
            int startingIndex = RawHTMLTemplate.IndexOf(UnattachedRowCommentFrontDelimiter);

            if (startingIndex == -1)
            {
                throw new Exception(string.Format("Unable to find {0} in HTML template, unable to create report", UnattachedRowCommentFrontDelimiter));
            }

            int endingIndex = RawHTMLTemplate.IndexOf(UnattachedRowCommentBackDelimiter, startingIndex);

            startingIndex += UnattachedRowCommentFrontDelimiter.Length;
            return RawHTMLTemplate[startingIndex..endingIndex];
        }

        private void UpdatePassFailStatusForWholeTest()
        {
            bool testcaseFailed = TestCaseFailed;

            RawHTMLTemplate = RawHTMLTemplate.Replace("[TEST_NUMBER]", TestCaseHeader.TestNumber);
            RawHTMLTemplate = RawHTMLTemplate.Replace("[TEST_NUMBER_CLASS]", testcaseFailed ? "failTestNumber" : "passTestNumber");
            RawHTMLTemplate = RawHTMLTemplate.Replace("[PASS_FAIL]", testcaseFailed ? "FAIL" : "PASS");
            RawHTMLTemplate = RawHTMLTemplate.Replace("[PASS_FAIL_CLASS]", testcaseFailed ? "failColor" : "passColor");

        }

        private string SaveNewHTMLFileToDisk()
        {
            var newFileName = GetNextFileName();
            File.WriteAllText(newFileName, RawHTMLTemplate);
            return newFileName;
        }

        private readonly string HtmlTemplate = "<style type=\"text/css\">\r\n     .testNumber {\r\n        width: 25%;\r\n        height: 50px\r\n    }\r\n\r\n     " +
            "th {\r\n         text-align: left;    \r\n     }\r\n\r\n     .failTestNumber {\r\n          background-color: darkred;\r\n     }\r\n\r\n    " +
            ".failColor {\r\n        background-color: red;\r\n    }\r\n\r\n    .passTestNumber {\r\n        background-color: darkgreen\r\n    }\r\n\r\n    " +
            ".passColor {\r\n        background-color: green\r\n    }\r\n\r\n    .bold {\r\n        font-weight:  bold\r\n    }\r\n\r\n    th{\r\n        " +
            "font-weight:  bold\r\n    }\r\n\r\n    .spaceDown {\r\n        margin-top:15px\r\n    }\r\n\r\n    .spaceDownBig {\r\n        margin-top:50px\r\n    " +
            "}\r\n\r\n    .centerText {\r\n        text-align: center;\r\n    }\r\n\r\n    .evenRow {\r\n        background-color: khaki\r\n    }\r\n\r\n    " +
            ".oddRow {\r\n        background-color: bisque\r\n    }\r\n</style>\r\n\r\n<html>\r\n    <body>\r\n\r\n    <table width=\"95%\">\r\n    <tr><td>\r\n    " +
            "<table border=\"1px\" width=\"100%\">\r\n\t   <tr>\r\n\t       <td class='bold centerText testNumber [TEST_NUMBER_CLASS]'>[TEST_NUMBER]</td>\r\n           " +
            "<td class='bold centerText [PASS_FAIL_CLASS]'>[PASS_FAIL]</td>\r\n\t   </tr>\r\n\t</table>\r\n\r\n    <table border=\"1\" width=\"100%\" class=\"spaceDown\">\r\n        " +
            "<tr>\r\n            <td width=\"150px\">Prereqs: </td>\r\n            <td colspan=\"3\">[PRE_REQS]</td>\r\n        </tr>\r\n\r\n        <tr>\r\n            " +
            "<td>Test Title: </td>\r\n            <td colspan=\"3\">[TEST_TITLE]</td>\r\n        </tr>\r\n\r\n        <tr>\r\n            <td>\r\n                " +
            "Priority\r\n            </td>\r\n            <td>\r\n                [PRIORITY]\r\n            </td>\r\n            <td width=\"150px\">\r\n                " +
            "Author\r\n            </td>\r\n            <td>\r\n                [AUTHOR]\r\n            </td>\r\n        </tr>\r\n\r\n        <tr>\r\n            " +
            "<td>\r\n                Executed By\r\n            </td>\r\n            <td>\r\n                [EXECUTED_BY]\r\n            </td>\r\n            " +
            "<td>\r\n                Executed Date\r\n            </td>\r\n            <td>\r\n                [EXECUTED_DATE]\r\n            </td>\r\n        " +
            "</tr>\r\n    </table>\r\n\r\n    <hr class=\"spaceDown\"/>\r\n\r\n    <div class='centerText bold evenRow spaceDown'>Description</div>\r\n        " +
            "<div class='descriptionText'>[GIVEN_WHEN_THEN]</div>\r\n\r\n    <table id='testStepsTabel' class=\"spaceDownBig\" width=\"100%\">\r\n        " +
            "<tr class=\"oddRow\">\r\n            <th class='numberColumn'>Step</th>\r\n            <th class='stepColumn'>Test Step</th>\r\n            " +
            "<th class='suppliedDataColumn'>Supplied Data</th>\r\n            <th class='expectedResultColumn'>Expected Result</th>\r\n            " +
            "<th class='actualResultColumn'>Actual Result</th>\r\n            <th class='statusColumn'>Status</th>\r\n            <th class='imageColumn'>Image</th>\r\n            " +
            "<th class='notesColumn'>Notes</th>\r\n        </tr>\r\n\r\n        <!--STEP ROW\r\n        <tr class=\"[ODD_EVEN_ROW]\">\r\n            " +
            "<td class='numberColumn'>[STEP_NUMBER]</td>\r\n            <td class='stepColumn'>[STEP_DESCRIPTION]</td>\r\n            <td class='suppliedDataColumn'>[STEP_SUPPLIED_DATE]</td>\r\n            " +
            "<td class='expectedResultColumn'>[STEP_EXPECTED_RESULT]</td>\r\n            <td class='actualResultColumn'>[STEP_ACTUAL_RESULT]</td>\r\n            " +
            "<td class='statusColumn'>[STEP_PASS_FAIL]</td>\r\n            <td class='imageColumn'><a href=\"[IMAGE_PATH]\">Image</a></td>\r\n            " +
            "<td class='notesColumn'>[STEP_NOTES]</td>\r\n        </tr>\r\n        -->\r\n\r\n    </table>\r\n\r\n        </td></tr></table>\r\n    </body>\r\n</html>";
    }

}