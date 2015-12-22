using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation;
using iEmosoft.Automation.Model;

namespace PatientMgmtTests.Pages
{
    public class PMSSession
    {
        private TestExecutioner executioner;
        private string loginURL = "http://localhost/PMS/default.html";

        public PMSSession(string testNumber = "", string testDescription = "")
        {
            TestCaseHeaderData tcHeader = new TestCaseHeaderData()
            {
                TestName = string.Format("{0}_{1}", testNumber, testDescription.Replace(" ", "_")),
                TestNumber = testNumber,
                TestDescription = testDescription
            };

            executioner = new TestExecutioner(tcHeader);
            executioner.NavigateTo(loginURL);
        }

        public LoginPage LoginPage
        {
            get
            {
                return new LoginPage(executioner);
            }
        }
    }
}
