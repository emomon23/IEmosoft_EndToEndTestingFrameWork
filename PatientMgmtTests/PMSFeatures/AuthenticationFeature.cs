using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation;

namespace PatientMgmtTests.PMSFeatures
{
    public class AuthenticationFeature
    {
        TestExecutioner executioner;

        public AuthenticationFeature(TestExecutioner testExectioner)
        {
            this.executioner = testExectioner;
        }

        public bool LoginToPMS(string username, string password)
        {
            executioner.SetTextOnElement("username", username);
            executioner.SetTextOnElement("password", password);
            
            executioner.Pause(1000);
            
            executioner.ClickElement("loginBtn", "", "", "Enter username and password, click the Login button", "", true, 10);

            return executioner.DoesElementExist("ng-show", "invalidCredentials", "span", 3) == false;
        }
    }
}
