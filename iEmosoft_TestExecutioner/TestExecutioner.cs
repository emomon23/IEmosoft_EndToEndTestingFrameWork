using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using TestRecorderModel;

namespace RecordableBrowser
{
    public class TestExecutioner : ITestRecorder , IDisposable
    {
        private IWebDriver firefoxDriver = new FirefoxDriver();
        private IScreenCapture screenCapture = null;
        private ITestRecorder recorder = null;
              
        public TestExecutioner(string testCaseNumber, string testCaseName, string rootPath)
        {
             var testCaseHeader = new TestCaseData()
            {
                ExecutedByName = "Mike Emo Automation Test Executioner",
                ExecutedOnDate = DateTime.Now.ToShortDateString(),
                TestName = testCaseName.Replace("_", " "),
                TestNumber = testCaseNumber,
                TestWriter = "Mike Emo Automation Test Executioner"
            };

            if (! string.IsNullOrEmpty(testCaseNumber))
            {
                testCaseHeader.TestCaseFileName = string.Format("{0}_{1}.xlsx", testCaseNumber, testCaseName);
            }
            else 
            {
                testCaseHeader.TestCaseFileName = string.Format("{0}.xlsx", testCaseName);
            }

            this.Initialize(testCaseHeader, rootPath);
        }

        public TestExecutioner(string testCaseName, string rootPath)
        {
            var testCaseHeader = new TestCaseData()
            {
                ExecutedByName = "Mike Emo Automation Test Executioner",
                ExecutedOnDate = DateTime.Now.ToShortDateString(),
                TestName = testCaseName.Replace("_", " "),
                TestCaseFileName = testCaseName.Replace(" ", "_") + ".xlsx",
                TestWriter = "Mike Emo Automation Test Executioner"
            };

            this.Initialize(testCaseHeader, rootPath);
        }

        public TestExecutioner(TestCaseData testCaseHeader, string rootPath)
        {
            this.Initialize(testCaseHeader, rootPath);
        }

        private void Initialize(TestCaseData testCaseHeader, string rootPath)
        {
            this.screenCapture = new ScreenCapture(rootPath + "\\ScreenCaptures");
             
            this.recorder = new TestCaseRecorder(rootPath);
            this.recorder.StartNewTestCase(testCaseHeader);
        }

        public bool ClickElement(By by, string stepDescription, string expectedResult, bool snapScreenBeforeClick)
        {
            try
            {
                var element = firefoxDriver.FindElement(by);
                if (element == null)
                    return false;
            }
            catch
            {
                return false;
            }

            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription, expectedResult);
            }

            if (snapScreenBeforeClick)
            {
              this.CaptureScreen();
            }

            this.firefoxDriver.ClickElement(by);
            return true;
        }

        public bool ClickElement(string elementSearch, string stepDescription, string expectedResult, bool snapScreenBeforeClick)
        {
            bool result = this.ClickElement(By.Id(elementSearch), stepDescription, expectedResult, snapScreenBeforeClick);
            if (!result)
            {
                result = this.ClickElement(By.XPath(elementSearch), stepDescription, expectedResult, snapScreenBeforeClick);
            }

            if (!result)
            {
                result = this.ClickElement(By.PartialLinkText(elementSearch), stepDescription, expectedResult, snapScreenBeforeClick);
            }

            return result;
        }

        public bool ClickElement(string elementSearch)
        {
            return this.ClickElement(elementSearch, null, null, false);
        }

        public string CurrentURL { get { return firefoxDriver.Url; } }

        public void ClickElement(By by)
        {
            this.firefoxDriver.FindElement(by).Click();
            System.Threading.Thread.Sleep(2000);
        }

        public void SetTextOnElement(By by, string text, string stepDescription)
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            firefoxDriver.FindElement(by).SendKeys(text);
        }

        public void SetTextOnElement(string elementToFind, string text)
        {
            SetTextOnElement(elementToFind, text, null);
        }

        public void SetTextOnElement(string elementToFind, string text, string stepDescription)
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            IWebElement element = null;

            try
            {
                element = firefoxDriver.FindElement(By.Id(elementToFind));
            }
            catch { }

            if (element == null)
            {
                element = firefoxDriver.FindElement(By.XPath(elementToFind));
            }

            if (elementToFind != null)
                element.SendKeys(text);

        }

        public string GetTextOnElement(By by)
        {
            var element = firefoxDriver.FindElement(by);
            string rtnVal = element.Text;

            if (string.IsNullOrEmpty(rtnVal))
                rtnVal = element.GetAttribute("value");

            return rtnVal;
        }

        public void SetValueOnDropdown(By by, string valueToSet)
        {
            SetValueOnDropdown(by, valueToSet, null);
        }

        public void SetValueOnDropdown(By by, string valueToSet, string stepDescription)
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            var dropdown = (SelectElement)firefoxDriver.FindElement(by);
            var originalValue = dropdown.SelectedOption.Text;

            dropdown.SelectByText(valueToSet);

            if (originalValue == dropdown.SelectedOption.Text)
            {
                dropdown.SelectByValue(valueToSet);
            }
        }

        public string GetSelectedTextOnDropdown(By by)
        {
            var dropdown = (SelectElement)firefoxDriver.FindElement(by);

            return dropdown.SelectedOption.Text;
        }

        public string GetSelectedValueOnDropdown(By by)
        {
            var dropdown = (SelectElement)this.firefoxDriver.FindElement(by);

            return dropdown.SelectedOption.GetAttribute("value");
        }

        public void NavigateTo(string url)
        {
            this.firefoxDriver.Navigate().GoToUrl(url);
        }

        public void NavigateTo(string url, string expectedResult)
        {
            if (!string.IsNullOrEmpty(expectedResult))
            {
                this.BeginTestCaseStep("Navigate to " + url, expectedResult);
            }

            firefoxDriver.Manage().Window.Maximize();
            this.firefoxDriver.Navigate().GoToUrl(url);
            System.Threading.Thread.Sleep(3000);
        }

        public void Quit()
        {
            firefoxDriver.Quit();
        }

        public IWebDriver WebDriver
        {
            get
            {
                return firefoxDriver;
            }
        }

        public string CaptureScreen(string textToWriteOnScreenCapture)
        {
            if (this.screenCapture == null)
            {
                return null;
            }

            string fileName = screenCapture.CaptureScreenToFile(textToWriteOnScreenCapture);
            if (this.recorder != null && this.recorder.CurrentStep != null)
            {
                this.recorder.CurrentStep.ImageFilePath = fileName;
                this.recorder.CurrentStep.ImageData = screenCapture.LastImageCapturedAsByteArray;
            }

            if (this.recorder.CurrentStep != null)
            {
                this.recorder.CurrentStep.ImageFilePath = fileName;
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
            get { return this.recorder != null ? recorder.TestCaseFailed : false; }
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult, string suppliedData)
        {
            if (this.recorder != null)
            {
                recorder.BeginTestCaseStep(stepDescription, expectedResult, suppliedData);
            }
        }

        public void BeginTestCaseStep(string stepDescription, string expectedResult)
        {
            if (this.recorder != null)
            {
                recorder.BeginTestCaseStep(stepDescription, expectedResult);
            }
        }

        public void BeginTestCaseStep(string stepDescription)
        {
            if (this.recorder != null)
            {
                recorder.BeginTestCaseStep(stepDescription);
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
            if (this.recorder != null)
            {
                recorder.CommitTestStep();
            }
        }

        public void CommitTestStep(string actualResult)
        {
            if (this.recorder != null)
            {
                recorder.CommitTestStep(actualResult);
            }
        }

        public void CommitTestStep(bool wasSuccessful, string actualResult)
        {
            if (this.recorder != null)
            {
                recorder.CommitTestStep(wasSuccessful, actualResult);
            }
        }

        public void CommitTestStep(string actualResult, string imageFile)
        {
            if (this.recorder != null)
            {
                recorder.CommitTestStep(actualResult, imageFile);
            }
        }

        public void CommitTestStep(bool wasSuccessful, string actualResult, string imageFile)
        {
            if (this.recorder != null)
            {
                recorder.CommitTestStep(wasSuccessful, actualResult, imageFile);
            }
        }

        public bool StartNewTestCase(TestRecorderModel.TestCaseData testCaseHeader)
        {
            if (this.recorder != null)
            {
                return recorder.StartNewTestCase(testCaseHeader);
            }

            return false;
        }

        public void RecordStep(TestRecorderModel.TestCaseStep step)
        {
            if (this.recorder != null)
            {
                recorder.RecordStep(step);
            }
        }

        public void SaveRecordedTest()
        {
            if (this.recorder != null)
            {
                recorder.SaveRecordedTest();
            }
        }

        public TestRecorderModel.TestCaseStep CurrentStep
        {
            get 
            {
                return this.recorder == null ? null : recorder.CurrentStep;
            }
        }

        public void Dispose()
        {
            this.Quit();
            this.SaveRecordedTest();
            this.recorder.Dispose();
        }


        public bool PageContains(string lookFor)
        {
            return this.firefoxDriver.PageSource.Contains(lookFor);
        }
               

        public void AssertPageContains(string lookFor)
        {
            recorder.BeginTestCaseStep(string.Format("Verify page contains string '{0}'", lookFor));

            if (!this.PageContains(lookFor))
            {
                string msg = string.Format("Unable to find '{0}' on current page, see image for details.", lookFor);

                if (this.recorder.CurrentStep.ActualResult.IsNull() == false && this.recorder.CurrentStep.ActualResult.Length > 0)
                    this.recorder.CurrentStep.ActualResult += "  ";

                this.recorder.CurrentStep.ActualResult = msg;
                this.recorder.CurrentStep.StepPassed = false;

                //captuer the screen with an error message
                this.CaptureScreen(msg);

                throw new Exception(msg);
            }
            else
            {
                //No error message, capture the screen (with no error message);
                this.CaptureScreen();
            }
        }


        public bool AmOnPage(string urlSnippet)
        {
            return this.firefoxDriver.Url.ToLower().Contains(urlSnippet.ToLower());
        }

        public void AssertAmOnPage(string urlSnippet)
        {
            if (!this.AmOnPage(urlSnippet))
            {
                if (this.recorder.CurrentStep != null)
                {
                    if (this.recorder.CurrentStep.ActualResult.Length > 0)
                        this.recorder.CurrentStep.ActualResult += "  ";

                    this.recorder.CurrentStep.ActualResult = string.Format("Am not on the expected page, url does not contain '{0}'", urlSnippet);
                    
                }

                throw new Exception("Am not on the expected page.  Url does not contain '" + urlSnippet + "'");
            }
        }

        public void AssertPageNotContain(string lookFor)
        {
            if (this.PageContains(lookFor))
            {
                string msg = string.Format("Page contains text ('{0}'), that should not exist, we may not be on the page expected.", lookFor);

                if (this.recorder.CurrentStep != null)
                {
                    if (this.recorder.CurrentStep.ActualResult.Length > 0)
                        this.recorder.CurrentStep.ActualResult += "  ";

                    this.recorder.CurrentStep.ActualResult = msg;
                }

                throw new Exception(msg);
            }
        }
    }
}
