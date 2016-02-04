using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientMgmtTests.PMSFeatures;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;

namespace PatientMgmtTests.AuthenticationTests
{
    [TestClass]
    public class Given_Authentication_Tests : iEmosoft.Automation.BaseClasses.BaseTestClass
    {
        [TestMethod]
        public void UI_Auth10_When_ValidUserName_InvalidPassword_Then_Invalid_UserName_Password_Should_Display()
        {
            TestCaseHeaderData testCaseHeader = new TestCaseHeaderData()
            {
                TestNumber = "Auth10",
                TestFamily = "Valid UserName / InvalidPassword",
                TestDescription = "Given a valid username and an invalid password, when user tries to log in, they should not be able too",
                TestName = "Authentication Tests",
            };

            RegisterTestUnderDevelopment(testCaseHeader);
        }

        [TestMethod]
        public void UI_Auth20_When_CredentialsAreCorrectValues()
        {
            TestCaseHeaderData tchd = new TestCaseHeaderData(){
                 TestNumber = "Auth20",
                 TestName = "Valid Username and Password",
                 TestDescription = "Given valid username and password, When user tries to log in, they they should see the landing page",
                 TestFamily = "Authentication Tests"
            };

           using (PMSApplication application = new PMSApplication(tchd))
            {
                bool logIn = application.AuthFeature.LoginToPMS("memo", "Password");
                application.Assertion(logIn, "Should be logged into the system");
                application.NavigationFeature.AssertURLIsWhereExpected("HospitalList", "Assert you are on the hospital landing page");
            }
            
        } 
    }
}
