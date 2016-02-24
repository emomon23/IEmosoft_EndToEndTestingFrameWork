using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Model;
using PatientMgmtTests;
using PatientMgmtTests.TestData;

namespace PatientMgmtTests.PMSFeatures
{
    public class HospitalListScreenScrapper
    {
        TestExecutioner executioner;

        public HospitalListScreenScrapper(TestExecutioner executioner)
        {
            this.executioner = executioner;
        }

        public HospitalModel GetHospitalAddressFromHospitalList(string hospitalName)
        {           
            JQuerySelector selector = new JQuerySelector("$('h3:contains(\"" + hospitalName + "\")').next()");
            string text = executioner.GetTextOnElement(selector).Trim();

            return new HospitalModel() { HospitalName = hospitalName, CombinedAddressString = text };
        }
    }
}
