using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation.Test.IEmosoft.com;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.BaseClasses
{
    public abstract class BaseTestClass
    {
        protected void RegisterTestUnderDevelopment(string testNumber, string testName, string testDescription, string testFamily)
        {
            RestClient restClient = new RestClient();
            restClient.RegisterTest(testNumber, testFamily, testName, testDescription);
        }

        protected void RegisterTestUnderDevelopment(TestCaseHeaderData headerData)
        {
            RegisterTestUnderDevelopment(headerData.TestNumber, headerData.TestName, headerData.TestDescription, headerData.TestFamily);
        }
    }
}
