using aUI.Automation.ModelObjects;
using aUI.Automation.Test.IEmosoft.com;
using NUnit.Framework;
using System;

namespace aUI.Automation.BaseClasses
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
            var restClient = new RestClient();

            DateTime? eta = null;
            if (!string.IsNullOrEmpty(etaDate))
            {
                if (!DateTime.TryParse(etaDate, out DateTime temp))
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
