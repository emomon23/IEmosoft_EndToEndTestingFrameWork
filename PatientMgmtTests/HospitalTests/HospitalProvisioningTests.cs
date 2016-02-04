using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.BaseClasses;

namespace PatientMgmtTests.HospitalTests
{
    [TestClass]
    public class HospitalProvisioningTests : BaseTestClass
    {
        [TestMethod]
        public void HOS10_Create_New_Hospital()
        {
            RegisterTestUnderDevelopment("HOS10", "Create A New Hospital", "Given unique random data, When a new hosptial is created, Then toast should appear and hospital should be added to the list of hospitals.", "Basic Provisioning");
        }

        [TestMethod]
        public void HOS20_HospitalValdiation()
        {
            RegisterTestUnderDevelopment("HOS20", "New Hospital Data Validation", "Given bad hospital data, When the user tries to create the hospital, Then error messages should appear", "Basic Provisioning");
        }
    }
}
