using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;
using PatientMgmtTests.TestData;

namespace PatientMgmtTests.HospitalTests
{
    [TestClass]
    public class VinDecodingBaselineTests
    {
        [TestMethod]
        public void PerformVinDecodingBaselineTests()
        {
            VinDataState stateMachine = new VinDataState();

            var availableVins = stateMachine.GetTotalNumberOfAvailableVins();

            using (var baselineTester = stateMachine.CreateBaselineTester())
            {
                foreach (string vin in availableVins)
                {
                    DecodedVin decodedVin = DecodeTheVinInTheUIJonny(vin);
                    baselineTester.HandleResult(decodedVin);
                }
            }
        }

        private DecodedVin DecodeTheVinInTheUIJonny(string vin)
        {
            //AA1995ChevroletImpalaBlu

            var result = new DecodedVin()
            {
                Make = vin.Substring(6,9),
                Model = vin.Substring(15,6),
                Vin = vin.Substring(0,2),
                Year = vin.Substring(2, 4)
            };

            result.Options.Add("Color", vin.Substring(21, 3));
          
            return result;
        }
    }
}
