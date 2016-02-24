using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;

namespace PatientMgmtTests.HospitalTests
{
    [TestClass]
    public class HospitalProvisioningTests : BaseTestClass
    {
        [TestMethod]
        public void HOS10_UpdateExistingHospitalValue()
        {
            TestCaseHeaderData testDescription = new TestCaseHeaderData()
            {
                TestNumber = "HOS10",
                TestDescription = "Given an existing hospital, When the hospital data is altered, Then the hospital record should be updated accordingly",
                TestFamily = "Hospital Tests",
                TestName = "UpdateAnExistingHospital",
                Prereqs = "None"
            };
                      
            using (var testSteps = new HospitalProvisioningSteps(testDescription))
            {
                testSteps.GivenAnExistingRandomlySelectedHospital();
                testSteps.WhenTheExistingHospitalIsUpdated();
                testSteps.ThenTheHospitalShouldBeUpdatedAccordingly();
            }
          
        }


        [TestMethod]
        public void HOS20_UpdateExistingHospitalName()
        {
            TestCaseHeaderData testDescription = new TestCaseHeaderData()
            {
                TestNumber = "HOS20",
                TestDescription = "Given an existing hospital, When the hospital name is altered, Then the hospital record should be updated accordingly",
                TestFamily = "Hospital Tests",
                TestName = "UpdateAnExistingHospital",
                Prereqs = "None"
            };

            using (var testSteps = new HospitalProvisioningSteps(testDescription))
            {
                testSteps.GivenAnExistingRandomlySelectedHospital();
                testSteps.WhenTheHospitalNameIsUpdated();
                testSteps.ThenTheHospitalShouldBeUpdatedAccordingly();
            }
        }
      
    }
}
