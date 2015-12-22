using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation;

namespace PatientMgmtTests.Pages
{
    public class LoginPage
    {
        TestExecutioner executioner;

        public LoginPage(TestExecutioner exectioner)
        {
            this.executioner = executioner;
        }

        public bool LoginToPMS(string username, string password)
        {
            return false;
        }
    }
}
