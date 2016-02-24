using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation;
using iEmosoft.Automation.Model;
using iEmosoft.Automation.HelperObjects;
using PatientMgmtTests.TestData;
using PatientMgmtTests.PMSFeatures;

namespace PatientMgmtTests.HospitalTests
{
    public class HospitalProvisioningSteps : IDisposable
    {
        RandomTestData randomRawData = new RandomTestData();
        
        //This is the test input data
        HospitalModel hospitalTestData;

        //We'll screen scrap the data on the screen and populte this udpatedHospitalModel
        HospitalModel postTestScreenScrappedHospital;

        TestCaseHeaderData testCaseHeader;

        //We need the PMSApplication instance in the assertion (Then methods).
        PMSApplication pmsApplication = null;

        PMSDataState pmsDataState = new PMSDataState();

        public HospitalProvisioningSteps(TestCaseHeaderData testcaseHeader)
        {
            this.testCaseHeader = testcaseHeader;
        }

        //Arrange -> Act -> Assert
        //Given -> When -> Then
        public void GivenAnExistingRandomlySelectedHospital()
        {
            hospitalTestData = pmsDataState.FetchARandomHospitalFromDatabase();
        }

        public void GivenTheLoggedInUserIsAPhysician()
        {
             pmsDataState.FetchUser("Physician");
        }

        public void GivenTheLoggedInUserIsASystemAdministrator()
        {
            pmsDataState.FetchUser("SysAdmin");
        }

        public void WhenTheExistingHospitalIsUpdated()
        {
            //Specify the altered values we will use for this test
            hospitalTestData.State = randomRawData.GetRandomState();
            hospitalTestData.Address = randomRawData.GetRandomAddress().Street1;
            hospitalTestData.City = randomRawData.GetRandomCity();

            this.UpdateTheHosptialRecord();
        }

        public void WhenTheHospitalNameIsUpdated(){
            hospitalTestData.OrigialValues.HospitalName = "Amplats";
            hospitalTestData.HospitalName = randomRawData.GetRandomCompanyName();

            this.UpdateTheHosptialRecord();
        }

        private void UpdateTheHosptialRecord()
        {
            //New up the PMSApplication and log into the system
            pmsApplication = new PMSApplication(this.testCaseHeader, pmsDataState.TestUser.UserName, pmsDataState.TestUser.Password);

            pmsApplication.NavigationFeature.NavigateToExistingHosptialEdit(hospitalTestData.OrigialValues.HospitalName);
            pmsApplication.HospitalProvisioningFeature.EditCurrentlySelectedHospital(hospitalTestData);
            pmsApplication.NavigationFeature.NavigateToHospitalList();

            this.postTestScreenScrappedHospital = pmsApplication.HospitalListScreenScraper.GetHospitalAddressFromHospitalList(hospitalTestData.HospitalName);
     
        }

        public void ThenTheHospitalShouldBeUpdatedAccordingly()
        {
            string assertionMessage = "Assert that the data displayed on the hospital list has been udpated";

            pmsApplication.Assertion(postTestScreenScrappedHospital.CombinedAddressString == hospitalTestData.CombinedAddressString, assertionMessage);
            pmsApplication.Assertion(postTestScreenScrappedHospital.HospitalName == hospitalTestData.HospitalName, assertionMessage);
        }


        public void Dispose()
        {
            pmsDataState.RollbackHospital(hospitalTestData);
            pmsApplication.Dispose();
        }
    }
}
