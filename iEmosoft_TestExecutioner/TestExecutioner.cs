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
        private bool reportingEnabled = true;

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

        public TestExecutioner()
        {
            reportingEnabled = false;
            Initialize(null, null, null, null);
        }

        private void Initialize(TestCaseHeaderData testCaseHeader, IUIDriver injectedDriver, BaseAuthor author, IScreenCapture capture)
        {
            AutomationFactory factory = new AutomationFactory();

            if (testCaseHeader != null)
            {
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
            }
             
            this.uiDriver = injectedDriver == null ? factory.CreateUIDriver() : injectedDriver;
        }

        public object ExecuteJavaScript(string script)
        {
            var fireFoxDriver = uiDriver as iEmosoft.Automation.UIDrivers.Firefox;
            return fireFoxDriver.RawWebDriver.ExecuteScript(script);
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

        public bool ClickElement(string attributeName, string attributeValue, string elementName = "", string stepDescription = "", string expectedResult = "", bool snapScreenBeforeClick = true)
        {
            uiDriver.ClickControl(attributeName, attributeValue, elementName);

            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription, expectedResult);
            }

            if (snapScreenBeforeClick && this.reportingEnabled)
            {
                this.CurrentStep.ImageFilePath = this.CaptureScreen();
            }


            return true;
        }

        public bool ClickElement(UIQuery query, string stepDescription = "", string expectedResult = "", bool snapScreenBeforeClick = true)
        {
            return this.ClickElement(query.AttributeName, query.AttributeValue, query.ControlTypeName, stepDescription, expectedResult, snapScreenBeforeClick);
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

        public void SetTextOnElement(UIQuery query, string valueToSet, string stepDescription = "")
        {
           this.SetTextOnElement(query.AttributeName, query.AttributeValue, valueToSet, query.ControlTypeName, stepDescription);
        }

        public void SetTextOnElement(string attributeName, string attributeValue, string textToSet,
            string elementName = "", string stepDescription = "", bool useWildCard = true)
        {
            uiDriver.SetTextOnControl(attributeName, attributeValue, textToSet, elementName, useWildCard);

            if (!stepDescription.isNull() && this.reportingEnabled)
            {
                this.BeginTestCaseStep(stepDescription);
            }
        }

        public string GetTextOnElement(string idOrCss)
        {
            return uiDriver.GetTextOnControl(idOrCss);
        }

        public string GetTextOnElement(string attributeName,string attributeValue,string controlType, bool useWildCardSearch = true)
        {
            return uiDriver.GetTextOnControl(attributeName, attributeValue, controlType, useWildCardSearch);
        }

        public string GetTextOnElement(UIQuery query)
        {
            return uiDriver.GetTextOnControl(query.AttributeName, query.AttributeValue, query.ControlTypeName);
        }

        public void SetValueOnDropdown(string attributeName, string attributeValue, string valueToSet, string stepDescription = "")
        {
            if (!string.IsNullOrEmpty(stepDescription) && this.reportingEnabled)
            {
                this.BeginTestCaseStep(stepDescription);
            }

            uiDriver.SetValueOnDropDown(attributeName, attributeValue, valueToSet);
        }

        public IWebDriver RawSeleniumWebDriver_AvoidCallingDirectly
        {
            get
            {
                var fireFox = uiDriver as iEmosoft.Automation.UIDrivers.Firefox;
                return fireFox.RawWebDriver;
            }
        }

        public void SetValueOnDropdown(UIQuery query, string valuleToSet, string stepDescription = "")
        {
            SetValueOnDropdown(query.AttributeName, query.AttributeValue, valuleToSet, stepDescription);
        }

        public void SetValueOnDropdown(string idOrCSS, string valueToSet, string stepDescription = "")
        {
            if (!string.IsNullOrEmpty(stepDescription) && this.reportingEnabled)
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

        public string GetSelectedTextOnDropdown(UIQuery query)
        {
            return GetSelectedTextOnDropdown(query.AttributeName, query.AttributeValue);
        }


        public string GetSelectedValueOnDropdown(string idOrCSS)
        {
            return uiDriver.GetValueOnDropDown(idOrCSS);
        }

        public string GetSelectedValueOnDropdown(string attributeName, string attributeValue)
        {
            return uiDriver.GetValueOnDropDown(attributeName, attributeValue, "select");
        }

        public string GetSelectedValueOnDropdown(UIQuery query)
        {
            return GetSelectedValueOnDropdown(query.AttributeName, query.AttributeValue);
        }

        public void NavigateTo(string url, string expectedResult = "")
        {
            if (!string.IsNullOrEmpty(expectedResult) && this.reportingEnabled)
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
            if (this.screenCapture == null || reportingEnabled == false)
            {
                return null;
            }

            string fileName = screenCapture.NewFileName;

            
            screenCapture.CaptureDesktop(fileName, null, textToWriteOnScreenCapture);
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

        public string CaptureScreen()
        {
            System.Threading.Thread.Sleep(500);
            return this.CaptureScreen(string.Empty);
        }
             
     
        public bool TestCaseFailed
        {
            get { return this.testAuthor != null ? testAuthor.TestCaseFailed : false; }
        }

        public void FailCurrentStep(string expectedResult, string actualResult)
        {
            var currentStep = this.CurrentStep;
            currentStep.StepPassed = false;

            if (!expectedResult.isNull())
            {
                currentStep.ExpectedResult = expectedResult;
            }

            if (!actualResult.isNull())
            {
                currentStep.ActualResult = actualResult;
            }

            this.CaptureScreen(actualResult);
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult = "", string suppliedData = "", bool captureImage = true)
        {
            if (this.testAuthor != null)
            {
                testAuthor.BeginTestCaseStep(stepDescription, expectedResult, suppliedData);
            }

            if (captureImage)
            {
                this.CaptureScreen();
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

            if (testAuthor != null && reportingEnabled)
            {
                testAuthor.SaveReport();
            }

            if (testAuthor != null)
            {
                this.testAuthor.Dispose();
            }

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
        }
               
    }
}
