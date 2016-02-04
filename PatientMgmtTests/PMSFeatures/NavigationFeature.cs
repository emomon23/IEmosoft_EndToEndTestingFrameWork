using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iEmosoft.Automation;

namespace PatientMgmtTests.PMSFeatures
{
    public class NavigationFeature
    {
        TestExecutioner executioner;

        public NavigationFeature(TestExecutioner executioner)
        {
            this.executioner = executioner;
        }

        public bool NavigateToNewPatientPage()
        {
            executioner.ClickElement("lnk_createNew", "", "", "Click the 'Create Hospital' button", "Should be taken to the new hospital page", true, true);
            string url = executioner.CurrentFormName_OrURL.ToString();

            return url.EndsWith("hospital");
        }

        public void AssertURLIsWhereExpected(string url, string description)
        {
            executioner.BeginTestCaseStep(description);
            if (!executioner.AmOnScreen(url))
            {
                executioner.FailCurrentStep(description, "Current url does not contain " + url);
                Assert.IsTrue(false, description);
            } 
        }
        
    }
}
