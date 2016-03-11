using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.InteropServices;
using iEmosoft.Automation.Authors;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation.Test.IEmosoft.com;
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
        private bool testPassed = true;
        private DateTime startTime;
        private DateTime disposeTime;
        private List<string> allTestFiles = new List<string>();
        private PoolState poolState = new PoolState() { IsAvailable = true, IsPartOfTestExecutionerPool = false };

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

        public TestExecutioner(bool useConfigFile = true)
        {
            reportingEnabled = false;
            if (useConfigFile)
            {
                Initialize(null, null, null, null);
            }
            else
            {
                this.uiDriver = new iEmosoft.Automation.UIDrivers.BrowserDriver(new AutomationConfiguration(), UIDrivers.BrowserDriver.BrowserDriverEnumeration.Firefox);
            }
        }

        public PoolState PoolState
        {
            get
            {
                return poolState;
            }
            set
            {
                poolState = value;
            }
        }

        public bool LoadJQuery()
        {
            try
            {
                string script = "if (!window.jQuery) { var jq = document.createElement('script'); jq.type = 'text/javascript'; jq.src = 'https://code.jquery.com/jquery-2.2.1.min.js'; document.getElementsByTagName('head')[0].appendChild(jq);";
                this.ExecuteJavaScript(script);
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
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
            this.startTime = DateTime.Now;
        }

        public bool DoesElementExist(string attributeName, string attributeValue, string elementName = "", int mineForSeconds = 10)
        {
            try
            {
                return this.RawSeleniumWebDriver_AvoidCallingDirectly.MineForElement(attributeName, attributeValue, elementName, true, mineForSeconds) != null;
            }
            catch
            {
                return false;
            }
        }

        public bool WaitForElementToVanish(string idOrCSSSelector, int mineForSeconds = 10)
        {
            bool result = false;

            for (int i = 0; i < mineForSeconds; i++)
            {
                if (! DoesElementExist(idOrCSSSelector, 1)){
                    result = true;
                    break; 
                }
            }

            return result;
        }

        public bool DoesElementExist(string idOrCSSSelector, int mineForSeconds = 10)
        {
            try
            {
                return this.RawSeleniumWebDriver_AvoidCallingDirectly.MineForElement(idOrCSSSelector, mineForSeconds) != null;
            }
            catch
            {
                return false;
            }
        }

        public void RefreshWebPage()
        {
            this.RawSeleniumWebDriver_AvoidCallingDirectly.Navigate().Refresh();
        }
        public object ExecuteJavaScript(string script)
        {
            var fireFoxDriver = uiDriver as iEmosoft.Automation.UIDrivers.BrowserDriver;
            return fireFoxDriver.RawWebDriver.ExecuteScript(script);
        }

        public BugCreator BugCreator { get; set; }
           

        public bool ClickElement(string IdOrAttributeName, string attributeValue = "", string elementName = "", string stepDescription = "", string expectedResult = "", bool snapScreenBeforeClick = true, bool waitForURLChange = false)
        {
            string currentPageOrUrl = uiDriver.CurrentFormName_OrPageURL;

            if (string.IsNullOrEmpty(attributeValue))
            {
                attributeValue = IdOrAttributeName;
                IdOrAttributeName = "id";
                
            }
                     
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription, expectedResult);
            }

            if (snapScreenBeforeClick && this.reportingEnabled && this.CurrentStep != null)
            {
                this.CurrentStep.ImageFilePath = this.CaptureScreen();
            }

            uiDriver.ClickControl(IdOrAttributeName, attributeValue, elementName);

            if (waitForURLChange)
            {
                for (int i = 0; i < 50; i++)
                {
                    System.Threading.Thread.Sleep(200);
                    if (currentPageOrUrl != uiDriver.CurrentFormName_OrPageURL)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        public bool ClickElement(UIQuery query, string stepDescription = "", string expectedResult = "", bool snapScreenBeforeClick = true)
        {
            return this.ClickElement(query.AttributeName, query.AttributeValue, query.ControlTypeName, stepDescription, expectedResult, snapScreenBeforeClick);
        }

        public bool ClickElement(JQuerySelector script, string stepDescription = "", string expectedResult = "", bool snapScreenBeforeClick = true, bool waitForURLToChange = false)
        {
            string newId = this.CreateRandomIdAttributeOnSelector(script);
            return this.ClickElement(newId, "", "", stepDescription, expectedResult, snapScreenBeforeClick, waitForURLToChange);
        }
              

        public string CurrentFormName_OrURL { get { return uiDriver.CurrentFormName_OrPageURL; } }
        
        public void SetTextOnElement(string idOrCSSSelector, string text)
        {
            SetTextOnElement(idOrCSSSelector, text, null);
        }

        public void Pause(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void SetTextOnElement(string idOrCSSSelector, string text, string stepDescription)
        {
            if (!string.IsNullOrEmpty(stepDescription))
            {
                this.BeginTestCaseStep(stepDescription);
            }

            if (text == null)
            {
                text = "";
            }

            uiDriver.SetTextOnControl(idOrCSSSelector, text);
        }

        public void SetTextOnElement(UIQuery query, string valueToSet, string stepDescription = "")
        {
           this.SetTextOnElement(query.AttributeName, query.AttributeValue, valueToSet, query.ControlTypeName, stepDescription);
        }

        public void SetTextOnElement(JQuerySelector selector, string valueToSet, string stepDescription = "")
        {
            var newId = this.CreateRandomIdAttributeOnSelector(selector );
            this.SetTextOnElement(newId, valueToSet, stepDescription);
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

        public string GetTextOnElement(JQuerySelector selector)
        {
            var newId = this.CreateRandomIdAttributeOnSelector(selector);
            return uiDriver.GetTextOnControl(newId);
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

            if (DoesElementExist(attributeName, attributeValue ))
            {
                string script = string.Format("$('[{0}*='{1}']').val('{2}').change();", attributeName, attributeValue, valueToSet);
                ExecuteJavaScript(script);
            }

        }

        public IWebDriver RawSeleniumWebDriver_AvoidCallingDirectly
        {
            get
            {
                var fireFox = uiDriver as iEmosoft.Automation.UIDrivers.BrowserDriver;
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

            string script = "";

            if (this.DoesElementExist(idOrCSS))
            {
                if (this.IsAngularApp)
                {
                    script = string.Format("angular.element(\"#{0}\").val('{1}').change();", idOrCSS, valueToSet);
                }
                else
                {
                    script = string.Format("$(\"#{0}\").val('{1}').change();", idOrCSS, valueToSet);
                }

                this.ExecuteJavaScript(script);
            }
          
        }

        public string GetSelectedTextOnDropdown(string elementId)
        {
            object objResult = "";

            if (DoesElementExist(elementId))
            {
                string script = string.Format("return $('#{0} :selected').text()", elementId);

                objResult = ExecuteJavaScript(script);
            }

            return objResult.ToString();
        }

        public string GetSelectedTextOnDropdown(string attributeName, string attributeValue)
        {
            object objResult = "";

            if (DoesElementExist(attributeName, attributeValue))
            {
                string script = string.Format("return $('[{0}*=\"{1}\"] :selected').text()", attributeName,
                    attributeValue);

                objResult = ExecuteJavaScript(script);
            }

            return objResult.ToString();
        }

        public string GetSelectedTextOnDropdown(UIQuery query)
        {
            return this.GetSelectedTextOnDropdown(query.AttributeName, query.AttributeValue);
        }


        public string GetSelectedValueOnDropdown(string elementId)
        {
            object objResult = "";

            if (DoesElementExist(elementId))
            {
                string script = string.Format("return $('#{0}').val();", elementId);

                objResult = ExecuteJavaScript(script);
            }

            return objResult.ToString();
        }

        public string GetSelectedValueOnDropdown(string attributeName, string attributeValue)
        {
            object objResult = "";

            if (DoesElementExist(attributeName, attributeValue))
            {
                string script = string.Format("return $('[{0}*=\"{1}\"]').val()", attributeName,
                    attributeValue);

                objResult = ExecuteJavaScript(script);
            }

            return objResult.ToString();
        }

        public string GetSelectedValueOnDropdown(UIQuery query)
        {
            return GetSelectedValueOnDropdown(query.AttributeName, query.AttributeValue);
        }

        public void  NavigateTo(string url, string expectedResult = "")
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

            var process = Process.GetProcesses().FirstOrDefault(x => x.MainWindowTitle.Contains(this.RawSeleniumWebDriver_AvoidCallingDirectly.Title));

            if (process != null)
            {
                BringToFront(process.MainWindowHandle);
            }

            uiDriver.MaximizeWindow();

            screenCapture.CaptureDesktop(fileName, null, textToWriteOnScreenCapture);
            if (this.testAuthor != null && this.testAuthor.CurrentStep != null)
            {
                this.testAuthor.CurrentStep.ImageFilePath = fileName;
                this.testAuthor.CurrentStep.ImageData = screenCapture.LastImageCapturedAsByteArray;
            }

            allTestFiles.Add(fileName);
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
            this.testPassed = false;
            var currentStep = this.CurrentStep;
            if (currentStep != null)
            {
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
         

        public TestCaseStep CurrentStep
        {
            get 
            {
                return this.testAuthor == null ? null : testAuthor.CurrentStep;
            }
        }

        public void ReleaseFromPool()
        {
            if (this.poolState.IsPartOfTestExecutionerPool)
            {
                ProcessTestResults();
                poolState.WasAlreadyInPool = true;
              
                if (!poolState.LangingPageURL.isNull())
                {
                    this.NavigateTo(poolState.LangingPageURL);
                }
                poolState.IsAvailable = true;
            }
            else
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                ProcessTestResults();
            }
            catch { }

            //Close the browser
            this.Quit();
        }

        private void ProcessTestResults()
        {
            this.disposeTime = DateTime.Now;
           
            if (testAuthor != null && reportingEnabled)
            {
                string reportFile = testAuthor.SaveReport();
                allTestFiles.Add(reportFile);
            }

            if (testAuthor != null)
            {
                this.testAuthor.Dispose();
            }

            if (this.BugCreator != null)
                this.BugCreator.Dispose();

            string ftpReportPath = FTPReport();
            CallRestService(ftpReportPath);
        }

        private string FTPReport()
        {
            ReportUploader uploader = new ReportUploader(new AutomationConfiguration());
            var testName = this.TestCaseHeader.TestName;
            return uploader.UploadReport(testName, allTestFiles);

        }

        private void CallRestService(string ftpReportPath)
        {
            TestRunDTO dto = new TestRunDTO()
            {
                ApplicationId = new AutomationConfiguration().ApplicationUnderTest,
                FTPPath = ftpReportPath,
                Status = this.testPassed ? (int)TestRunDTO.TestRunStatusEnumeration.Passed : (int)TestRunDTO.TestRunStatusEnumeration.Failed,
                TestNumber = TestCaseHeader.TestNumber,
                TestTime = disposeTime - startTime,
                RunDate = startTime
            };

            RestClient restClient = new RestClient();
            restClient.RecordTestRun(dto);
        }

        public bool WaitForURLChange(string urlSnippet, int waitSeconds = 20)
        {
            bool result = false;

            for (int i = 0; i <= (waitSeconds *2); i++)
            {
                if (this.CurrentFormName_OrURL.Contains(urlSnippet))
                {
                    result = true;
                    break;
                }

                Pause(500);
            }

            return result;
        }

        public bool PageContains(string lookFor)
        {
            try
            {
                var element = uiDriver.RawWebDriver.FindElement(By.TagName("body"));
            }
            catch { };

            return uiDriver.ScreenContains(lookFor);
        }

        public bool IsCheckBoxChecked(string idOrCss)
        {
            return uiDriver.IsCheckBoxChecked(idOrCss);
        }

        public bool IsCheckBoxChecked(string idOrCss, bool value)
        {
            bool existingValue = uiDriver.IsCheckBoxChecked(idOrCss);

            if (existingValue != value)
            {
                uiDriver.ClickControl(idOrCss);
            }

            return uiDriver.IsCheckBoxChecked(idOrCss);
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
                this.testPassed = false;
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

                this.testPassed = false;
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

                this.testPassed = false;
               throw new Exception(msg);
            }
        }


        public List<TestCaseStep> RecordedSteps
        {
            get { return testAuthor.RecordedSteps; }
        }

        public void StartNewTestCase(TestCaseHeaderData header)
        {
            if (testAuthor != null)
            {
                testAuthor.StartNewTestCase(header);
                this.startTime = DateTime.Now;
            }
        }

        public TestCaseHeaderData TestCaseHeader
        {
            get { return testAuthor.TestCaseHeader;  }
        }

        private string CreateRandomIdAttributeOnSelector(JQuerySelector seletor)
        {
            string newId = Guid.NewGuid().ToString().Replace("-", "");
            string script = string.Format("$({0}).attr('id', '{1}')", seletor.jQuerySelectorScript, newId);
            
            this.ExecuteJavaScript(script);
            return newId;
        }

        private bool IsAngularApp
        {
            get
            {
                string script = "return angular == null ? 'FALSE' : 'TRUE'";
                object result = this.ExecuteJavaScript(script);

                return result.ToString() == "TRUE";
            }
        }

        private string GetNgModelAttribute(string idOrCss)
        {
            var element = this.RawSeleniumWebDriver_AvoidCallingDirectly.MineForElement(idOrCss);
            string ngModelValue = element.GetAttribute("ng-model");

            return ngModelValue;
        }
        private void FailTest(Exception exp)
        {
            this.BeginTestCaseStep("Un expected error occurred", "", "");
            this.CurrentStep.ActualResult = exp.Message;
            this.CurrentStep.StepPassed = false;
            this.testPassed = false;
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static void BringToFront(IntPtr handle)
        {
            // Make Calculator the foreground application
            SetForegroundWindow(handle);
        }
               
    }

    public class PoolState
    {
        public bool IsPartOfTestExecutionerPool { get; set; }
        public bool IsAvailable { get; set; }

        public bool WasAlreadyInPool { get; set; }

        public string LangingPageURL { get; set; }
    }
}
