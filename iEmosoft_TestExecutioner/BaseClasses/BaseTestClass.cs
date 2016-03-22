using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation.Test.IEmosoft.com;
using iEmosoft.Automation.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iEmosoft.Automation.BaseClasses
{
    public abstract class BaseTestClass
    {
        public enum RegisterTestUnderDevelopmentResultEnumeration
        {
            AssertInconclusive,
            AssertFailure,
            DoNothing
        }

        protected void RegisterTestUnderDevelopment(string testNumber, string testName, string testDescription, string testFamily, string etaDate, RegisterTestUnderDevelopmentResultEnumeration assertAction = RegisterTestUnderDevelopmentResultEnumeration.AssertInconclusive)
        {
            RestClient restClient = new RestClient();

            DateTime? eta = null;
            if (!string.IsNullOrEmpty(etaDate))
            {
                DateTime temp;
                if (!DateTime.TryParse(etaDate, out temp))
                {
                    throw new Exception("etaDate must be a valid date (eg. '1/1/2020')");
                }

                eta = temp;
            }
            restClient.RegisterTest(testNumber, testFamily, testName, testDescription, eta);

            string assertMsg = "This test is has yet to be implemented.";
            if (assertAction == RegisterTestUnderDevelopmentResultEnumeration.AssertInconclusive)
            {
                Assert.Inconclusive(assertMsg);
            }
            else if (assertAction == RegisterTestUnderDevelopmentResultEnumeration.AssertFailure)
            {
                Assert.IsTrue(false, assertMsg);
            }
        }

        protected void RegisterTestUnderDevelopment(TestCaseHeaderData headerData, string etaDate)
        {
            RegisterTestUnderDevelopment(headerData.TestNumber, headerData.TestName, headerData.TestDescription, headerData.TestFamily, etaDate);
        }
    }
}
