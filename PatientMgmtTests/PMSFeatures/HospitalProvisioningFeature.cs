using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation;
using PatientMgmtTests.TestData;

namespace PatientMgmtTests.PMSFeatures
{
    public class HospitalProvisioningFeature :BaseFeature
    {
        private TestExecutioner testExecutioner;

        public HospitalProvisioningFeature(TestExecutioner executioner)
        {
            this.testExecutioner = executioner;
            base.navigationFeature = new NavigationFeature(executioner);
        }

        public void ProvisionANewHospital(HospitalModel hospitalData)
        {
            if (this.navigationFeature.NavigateToNewPatientPage())
            {
                testExecutioner.SetTextOnElement("hospitalName", hospitalData.HospitalName);
                testExecutioner.SetTextOnElement("street", hospitalData.Address);
                testExecutioner.SetTextOnElement("city", hospitalData.City);
                testExecutioner.SetTextOnElement("state", hospitalData.State);
                testExecutioner.ClickElement("saveBtn", "", "", "Enter hospital data, and click 'Save'", "Hospital data should be saved", true, true);
            }
        }
    }
}
