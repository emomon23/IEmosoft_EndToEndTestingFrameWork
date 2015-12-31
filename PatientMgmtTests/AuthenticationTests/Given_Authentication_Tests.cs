using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientMgmtTests.Pages;
using iEmosoft.Automation.HelperObjects;

namespace PatientMgmtTests.AuthenticationTests
{
    [TestClass]
    public class Given_Authentication_Tests
    {
        [TestMethod]
        public void UI_Auth10_When_ValidUserName_InvalidPassword_Then_Invalid_UserName_Password_Should_Display()
        {
            AutomationFactory f = new AutomationFactory();

            iEmosoft.Automation.FTP.IReportUploader loader = f.CreateReportFTPUploader();
            loader.UploadReport(@"C:\PatientMgmtSystemTestsResults", "FTPUploadTEst", false, "Powerlink2_0");

           // PMSSession session = new PMSSession("UI_Auth10", "Given a valid username and an invalid password.  When the user attempts to login.  Then an invalid username or password should display");

        }
    }
}
