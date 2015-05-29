using System;
using System.Collections.Generic;
using iEmosoft.Automation.Authors;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;


namespace iEmosoft.Automation
{
    public class TestExecutioner : IDisposable
    {
        private IUIDriver uiDriver = null;
        private IScreenCapture screenCapture = null;
        private BaseAuthor testAuthor = null;
      
        public TestExecutioner(string testCaseNumber, string testCaseName="", IUIDriver uiDriver = null, BaseAuthor author = null, IScreenCapture capture = null)
        {
             var testCaseHeader = new TestCaseHeaderData()
            {
                ExecutedByName = "Mike Emo Automation Test Executioner",
                ExecutedOnDate = DateTime.Now.ToShortDateString(),
                TestName = testCaseName.Replace("_", " "),
                TestNumber = testCaseNumber,
                TestWriter = "Mike Emo Automation Test Executioner"
            };
            
            this.Initialize(testCaseHeader, uiDriver, author, capture);
        }

        public TestExecutioner(TestCaseHeaderData testCaseHeader, IUIDriver uiDriver = null, BaseAuthor author = null, IScreenCapture capture = null)
        {
            this.Initialize(testCaseHeader, uiDriver, author, capture);
        }

        private void Initialize(TestCaseHeaderData testCaseHeader, IUIDriver injectedDriver, BaseAuthor author, IScreenCapture capture)
        {
            AutomationFactory factory = new AutomationFactory();

            if (capture != null)
            {
                this.screenCapture = capture;
            }
            else
            {
                this.screenCapture = factory.CreateScreenCapturer();
            }

            this.testAuthor = author == null ? factory.CreateAuthor() : author;
            this.testAuthor.StartNewTestCase(testCaseHeader);

            this.uiDriver = injectedDriver == null ? factory.CreateUIDriver() : injectedDriver;
        }

        public BugCreator BugCreator { get; set; }

        public bool ClickElement(string idOrCss, string stepDescription, string expectedResult, bool snapScreenBeforeClick)
        {
            uiDriver.ClickControl(idOrCss);
           
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription, expectedResult);
            }

            if (snapScreenBeforeClick)
            {
              this.CaptureScreen();
            }
            

            return true;
        }
        
        public bool ClickElement(string elementSearch)
        {
            return this.ClickElement(elementSearch, null, null, false);
        }

        public string CurrentFormName_OrURL { get { return uiDriver.CurrentFormName_OrPageURL; } }
        
        public void SetTextOnElement(string idOrCSSSelector, string text)
        {
            SetTextOnElement(idOrCSSSelector, text, null);
        }

        public void SetTextOnElement(string idOrCSSSelector, string text, string stepDescription)
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            uiDriver.SetTextOnControl(idOrCSSSelector, text);
        }

        public void SetTextOnElement(string attributeName, string attributeValue, string textToSet,
            string elementName = "", bool useWildCard = true)
        {
            uiDriver.SetTextOnControl(attributeValue, attributeValue, textToSet, elementName, useWildCard);
        }

        public string GetTextOnElement(string idOrCss)
        {
            return uiDriver.GetTextOnControl(idOrCss);
        }

        public string GetTextOnElement(string attributeName,string attributeValue,string controlType, bool useWildCardSearch = true)
        {
            return uiDriver.GetTextOnControl(attributeName, attributeValue, controlType, useWildCardSearch);
        }

        public void SetValueOnDropdown(string attributeName, string attributeValue, string valueToSet, string stepDescription = "")
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            uiDriver.SetValueOnDropDown(attributeName, attributeValue, valueToSet);
        }

        public void SetValueOnDropdown(string idOrCSS, string valueToSet, string stepDescription = "")
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            uiDriver.SetValueOnDropDown(idOrCSS, valueToSet);
        }

        public string GetSelectedTextOnDropdown(string idOrCSS)
        {
            return uiDriver.GetTextOnDropDown(idOrCSS);
        }

        public string GetSelectedTextOnDropdown(string attributeName, string attributeValue)
        {
            return uiDriver.GetTextOnDropDown(attributeName, attributeValue, "select");
        }


        public string GetSelectedValueOnDropdown(string idOrCSS)
        {
            return uiDriver.GetValueOnDropDown(idOrCSS);
        }

        public string GetSelectedValueOnDropdown(string attributeName, string attributeValue)
        {
            return uiDriver.GetValueOnDropDown(attributeName, attributeValue, "select");
        }

        public void NavigateTo(string url, string expectedResult = "")
        {
            if (!string.IsNullOrEmpty(expectedResult))
            {
                this.BeginTestCaseStep("Navigate to " + url, expectedResult);
            }

            uiDriver.MaximizeWindow();
            uiDriver.NavigateTo(url);
            System.Threading.Thread.Sleep(3000);
        }

        public void Quit()
        {
            uiDriver.Dispose();
        }
        
        public string CaptureScreen(string textToWriteOnScreenCapture)
        {
            if (this.screenCapture == null)
            {
                return null;
            }

            string fileName = string.Format("TestImage_{0}_{1}.jpg", new RandomTestData().GetRandomDigits(5),
                DateTime.Now.Minute);

            
            screenCapture.CaptureDesktop(textToWriteOnScreenCapture);
            if (this.testAuthor != null && this.testAuthor.CurrentStep != null)
            {
                this.testAuthor.CurrentStep.ImageFilePath = fileName;
                this.testAuthor.CurrentStep.ImageData = screenCapture.LastImageCapturedAsByteArray;
            }

            if (this.testAuthor.CurrentStep != null)
            {
                this.testAuthor.CurrentStep.ImageFilePath = fileName;
            }
            return fileName;
        }

        public void CaptureScreen()
        {
            System.Threading.Thread.Sleep(3000);
            this.CaptureScreen(string.Empty);
        }
             
     
        public bool TestCaseFailed
        {
            get { return this.testAuthor != null ? testAuthor.TestCaseFailed : false; }
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult, string suppliedData)
        {
            if (this.testAuthor != null)
            {
                testAuthor.BeginTestCaseStep(stepDescription, expectedResult, suppliedData);
            }
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult)
        {
            if (this.testAuthor != null)
            {
                testAuthor.BeginTestCaseStep(stepDescription, expectedResult);
            }
        }

        public void BeginTestCaseStep(string stepDescription)
        {
            if (this.testAuthor != null)
            {
                testAuthor.BeginTestCaseStep(stepDescription);
            }
        }

        public void BeginTestCaseStep(string stepDescription, bool captureImage)
        {
            this.BeginTestCaseStep(stepDescription);
            if (captureImage)
            {
                this.CaptureScreen();
            }
        }

        public void CommitTestStep()
        {
            if (this.testAuthor != null)
            {
                testAuthor.CommitCurrentTestStep();
            }
        }

       
        public void CommitTestStep(string actualResult, bool wasSuccessful = true, string imageFile = "")
        {
            if (this.testAuthor != null)
            {
                testAuthor.CommitCurrentTestStep(wasSuccessful, actualResult, imageFile);
            }
        }
      
        public bool StartNewTestCase(TestCaseHeaderData testCaseHeader)
        {
            if (this.testAuthor != null)
            {
                return testAuthor.StartNewTestCase(testCaseHeader);
            }

            return false;
        }
        

        public TestCaseStep CurrentStep
        {
            get 
            {
                return this.testAuthor == null ? null : testAuthor.CurrentStep;
            }
        }

        public void Dispose()
        {
            this.Quit();
            testAuthor.SaveReport();
            this.testAuthor.Dispose();

            if (this.BugCreator != null)
                this.BugCreator.Dispose();
        }


        public bool PageContains(string lookFor)
        {
            return uiDriver.ScreenContains(lookFor);
        }
        
        public void AssertPageContains(string lookFor, bool continueIfFails = false)
        {
            testAuthor.BeginTestCaseStep(string.Format("Verify page contains string '{0}'", lookFor));

            if (!this.PageContains(lookFor))
            {
                string msg = string.Format("Unable to find '{0}' on current page, see image for details.", lookFor);

                if (this.testAuthor.CurrentStep.ActualResult.IsNull() == false && this.testAuthor.CurrentStep.ActualResult.Length > 0)
                    this.testAuthor.CurrentStep.ActualResult += "  ";

                this.testAuthor.CurrentStep.ActualResult = msg;
                this.testAuthor.CurrentStep.StepPassed = false;

                //captuer the screen with an error message
                this.CaptureScreen(msg);
                this.FailTest();

                if (! continueIfFails)
                {
                    throw new Exception(msg);
                }
            }
            else
            {
                //No error message, capture the screen (with no error message);
                this.CaptureScreen();
            }
        }


        public bool AmOnScreen(string urlSnippet)
        {
            return uiDriver.AmOnSceen(urlSnippet);
        }

        public void AssertAmOnScreen(string urlSnippet)
        {
            if (!this.AmOnScreen(urlSnippet))
            {
                if (this.testAuthor.CurrentStep != null)
                {
                    if (this.testAuthor.CurrentStep.ActualResult.Length > 0)
                        this.testAuthor.CurrentStep.ActualResult += "  ";

                    this.testAuthor.CurrentStep.ActualResult = string.Format("Am not on the expected page, url does not contain '{0}'", urlSnippet);
                    this.testAuthor.CurrentStep.StepPassed = false;     
                }

                this.FailTest();
                throw new Exception("Am not on the expected page.  Url does not contain '" + urlSnippet + "'");
            }
        }

        public void AssertPageNotContain(string lookFor)
        {
            if (this.PageContains(lookFor))
            {
                string msg = string.Format("Page contains text ('{0}'), that should not exist, we may not be on the page expected.", lookFor);

                if (this.testAuthor.CurrentStep != null)
                {
                    if (this.testAuthor.CurrentStep.ActualResult.Length > 0)
                        this.testAuthor.CurrentStep.ActualResult += "  ";

                    this.testAuthor.CurrentStep.ActualResult = msg;
                    this.testAuthor.CurrentStep.StepPassed = false;
                }

                this.FailTest();
                throw new Exception(msg);
            }
        }


        public List<TestCaseStep> RecordedSteps
        {
            get { return testAuthor.RecordedSteps; }
        }

        public TestCaseHeaderData TestCaseHeader
        {
            get { return testAuthor.TestCaseHeader;  }
        }

        private void FailTest(Exception exp)
        {
            this.BeginTestCaseStep("Un expected error occurred", "", "");
            this.CurrentStep.ActualResult = exp.Message;
            this.CurrentStep.StepPassed = false;

            this.FailTest();
        }

        private void FailTest()
        {
            if (this.BugCreator != null)
            {
                try
                {
                    this.BugCreator.CreateBug(testAuthor.TestCaseHeader, testAuthor.RecordedSteps);
                }
                catch { }
            }
        }
    }
}
