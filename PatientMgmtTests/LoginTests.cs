using System;
using RecordableBrowser;
using RecordableBrowser.TestData;
using TestRecorderModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PatientMgmtTests
{
    [TestClass]
    public class LoginTests
    {
        TestCaseData testCaseHeader = null;

        [TestInitialize]
        public void InitializerTestCase()
        {
            testCaseHeader = new TestCaseData()
            {
                ExecutedByName = "PatientMgmtTests.LoginTests",
                ExecutedOnDate = DateTime.Now.ToShortDateString(),
                Priority = "HIGH",
                TestWriter = "iEmoSoft Test Executioner"
            };
        }

        [TestMethod]
        public void T1000_ValidUserLogsIntoPatientManagementSystem()
        {
            testCaseHeader.Prereqs = "Request a new user account be created for the Patient Mgmt System";
            testCaseHeader.TestDescription = "Given a valid username and password, when a user logs in, then they should see the list of hospital in the system.";
            testCaseHeader.TestName = "Patient Management System Valid User Login Test";
            testCaseHeader.TestNumber = "T1000";

            using (PMSTestContext tester = new PMSTestContext(testCaseHeader))
            {
                tester.LoginToPMSSite("mike.emo@iEmosoft.com", "P@ssword");
                tester.AssertAmOnHospitalListPage();
            }
        }

        [TestMethod]
        public void T2000_InvalidUserTriesToLogIntoPatientManagementSystem()
        {
            testCaseHeader.TestDescription = "Given an invalid username and password, when a user logs in, then they should see an error message 'Invalid username or password'";
            testCaseHeader.TestName = "Patient Management System Invalid User Tries To Login Test";
            testCaseHeader.TestNumber = "T2000";

            using (PMSTestContext tester = new PMSTestContext(testCaseHeader))
            {
                tester.LoginToPMSSite("mike.emo@iEmosoft.com", "invalidPassword");
                tester.AssertPageContains("Invalid username or password");
            }
        }
              
        [TestMethod]
        public void T4000_Create_A_New_Hospital_Record_Verify_It_Displays_On_List()
        {
            testCaseHeader.TestDescription = "Given a new hospital record is created, then it should display on the Hospital List";
            testCaseHeader.TestName = "Newly created hospital displays on hospital list";
            testCaseHeader.TestNumber = "T4000";

            RandomTestData testDataSource = new RandomTestData();
            var hospitalData = testDataSource.GetRandomCompany();

            using (PMSTestContext tester = new PMSTestContext(testCaseHeader))
            {
                tester.LoginToPMSSite("mike.emo@iEmosoft.com", "P@ssw0rd");
                tester.CreateNewHospital(hospitalData.CompanyName, hospitalData.HQAddress.Street1, hospitalData.HQAddress.City, hospitalData.HQAddress.State);
                tester.AssertPageContains("Your data has been saved");

                tester.NavigateToHospitalList();
                tester.AssertHospitalExistsOnList(hospitalData.CompanyName);
            }
        }
    }
}
