using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iEmosoft.Automation;
using iEmosoft.Automation.Model;

namespace PatientMgmtTests.PMSFeatures
{
    public class PMSApplication : IDisposable
    {
        private TestExecutioner executioner;
        private string loginURL = "http://localhost/PMS/default.html";

        public PMSApplication(string testNumber, string testName = "", string testDescription = "", string testFamily = "", string userName = "", string password = "")
        {
            TestCaseHeaderData tcHeader = new TestCaseHeaderData()
            {
                TestName = string.Format("{0}_{1}", testNumber, testName.Replace(" ", "_")),
                TestNumber = testNumber,
                TestDescription = testDescription
            };

            executioner = new TestExecutioner(tcHeader);
            executioner.NavigateTo(loginURL);

            if (!(string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password)))
            {
                this.IsAuthenticated = AuthFeature.LoginToPMS(userName, password);
            }

        }

        public PMSApplication(TestCaseHeaderData headerData, string userName = "", string password = "")
        {
            executioner = new TestExecutioner(headerData);
            executioner.NavigateTo(loginURL, "Navigate to Patitnet management system's login page");

            if (!(string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password)))
            {
                this.IsAuthenticated = AuthFeature.LoginToPMS(userName, password);
            }
        }


        public void Assertion(bool shouldBeTrue, string description, bool isHardFailure = true)
        {
            if (!shouldBeTrue)
            {
                executioner.FailCurrentStep(description, "");
                if (isHardFailure)
                {
                    Assert.IsTrue(shouldBeTrue, description);
                }
            }
        }

        public NavigationFeature NavigationFeature
        {
            get
            {
                return new NavigationFeature(this.executioner);
            }
        }

        public HospitalListScreenScrapper HospitalListScreenScraper
        {
            get
            {
                return new HospitalListScreenScrapper(this.executioner);
            }
        }

        public HospitalProvisioningFeature HospitalProvisioningFeature
        {
            get
            {
                return new HospitalProvisioningFeature(this.executioner);
            }
        }

        public AuthenticationFeature AuthFeature
        {
            get
            {
                return new AuthenticationFeature(executioner);
            }
        }

        public void Dispose()
        {
            executioner.Dispose();
        }

        public bool IsAuthenticated { get; set; }
    }
}
