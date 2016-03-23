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
            executioner.ClickElement("lnk_createNew", "", "", "Click the 'Create Hospital' button", "Should be taken to the new hospital page", true, 10);
            string url = executioner.CurrentFormName_OrURL.ToString();

            return url.EndsWith("hospital");
        }

        public void NavigateToHospitalList()
        {
            this.executioner.ClickElement("lnk_Hospitals", "", "", "Click on the Hospital menu item", "Should be taken to the list of hospitals",true, 10);
        }

        public void NavigateToExistingHosptialEdit(string hosptialName)
        {
            //This is a good example of where there isn't a good clean way to access an html element, and 
            //you, as the test automator can't go to the devs and say 'add an id here'
            //In this example I'm trying to get the anchor tag for the Edit button for a specified hosptial

            //For example: $('div[ng-repeat*="hospital" h3:contains("Childrens") a')
            JQuerySelector script = new JQuerySelector("$('div[ng-repeat*=\"hospital\"] h3:contains(\"" + hosptialName + "\") a')");
            executioner.ClickElement(script, "Click the Edit button for " + hosptialName + " hospital", "Should be taken to the hospital edit screen");
            executioner.Pause(500);
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
