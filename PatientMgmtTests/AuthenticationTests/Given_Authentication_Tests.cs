using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientMgmtTests.Pages;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.BaseClasses;

namespace PatientMgmtTests.AuthenticationTests
{
    [TestClass]
    public class Given_Authentication_Tests : iEmosoft.Automation.BaseClasses.BaseTestClass
    {
        [TestMethod]
        public void UI_Auth10_When_ValidUserName_InvalidPassword_Then_Invalid_UserName_Password_Should_Display()
        {
            RegisterTestUnderDevelopment("Auth10", "Valid UserName / InvalidPassword", "Given a valid username and an invalid password, when user tries to log in, they should not be able too", "Authentication Tests");
        }

        [TestMethod]
        public void UI_Auth20_When_CredentialsAreCorrectValues_IncorrectCase()
        {
            RegisterTestUnderDevelopment("Auth20", "Valid Username and Password", "Given valid username and password, When user tries to log in, they they should see the landing page", "Authentication Tests");            
        } 
    }
}
