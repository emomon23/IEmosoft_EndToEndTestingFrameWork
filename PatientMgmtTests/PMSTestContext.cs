using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordableBrowser;
using TestRecorderModel;
using OpenQA;
using OpenQA.Selenium;

namespace PatientMgmtTests
{
    public class PMSTestContext : IDisposable
    {
        private string url = "http://localhost/PMS/default.html";
        private string testReportFolder = @"C:\PatientMgmtSystemTestsResults";
        private TestExecutioner testRecorder;

        public PMSTestContext(string testCaseName)
        {
            testRecorder = new TestExecutioner(testCaseName, testReportFolder);
        }

        public PMSTestContext(TestCaseData testCaseHeader)
        {
            testRecorder = new TestExecutioner(testCaseHeader, testReportFolder);
        }

        public void LoginToPMSSite(string username, string password)
        {
            testRecorder.NavigateTo(url, "Should see the login page");
            testRecorder.AssertPageContains("Login");

            testRecorder.SetTextOnElement("username", username);
            testRecorder.SetTextOnElement("password", password);
            testRecorder.ClickElement("loginBtn", "Enter username and password and click 'Login'", "Should be taken to list of Hospitals", true);
        }

        public void CreateNewHospital(string hospitalName, string address, string city, string state)
        {
            this.NavigateToHospitalList();
            testRecorder.ClickElement("lnk_createNew");

            System.Threading.Thread.Sleep(3000);

            testRecorder.SetTextOnElement("hospitalName", hospitalName);
            testRecorder.SetTextOnElement("street", address);
            testRecorder.SetTextOnElement("city", city);
            testRecorder.SetTextOnElement("state", state);
            testRecorder.ClickElement("saveBtn", "Enter new Hospital information and click 'Save'", "A new hospital record should be created and a message displayed", true);
        }

        public void AssertAmOnHospitalListPage()
        {
            System.Threading.Thread.Sleep(3000);
            bool exists = testRecorder.WebDriver.ElementExists(By.Id("hospitalList"));
            
            testRecorder.BeginTestCaseStep("Verify the Hospital List page is displayed", true);
            if (!exists)
            {              
                testRecorder.CurrentStep.ActualResult = "Am not on hopsital list, see image";
                testRecorder.CurrentStep.StepPassed = false;
                throw new Exception("Am not on the hospital list page as was expected");
            }
        }

        public void AssertPageContains(string lookFor)
        {
            testRecorder.AssertPageContains(lookFor);
        }

        public void AssertPageNotContain(string lookFor)
        {
            testRecorder.AssertPageNotContain(lookFor);
        }

        public void AssertHospitalExistsOnList(string hospitalName)
        {
            testRecorder.AssertPageContains(hospitalName);
        }
        
        public void NavigateToHospitalList()
        {
            testRecorder.ClickElement("lnk_Hospitals");
        }


        public void Dispose()
        {
            testRecorder.Dispose();
        }
    }
}
