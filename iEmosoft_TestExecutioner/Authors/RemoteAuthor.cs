using System;
using System.IO;
using System.Net;
using System.Web;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.Authors
{
    public class RemoteAuthor : BaseAuthor
    {
        HelperObjects.IAutomationConfiguration config = new HelperObjects.AutomationConfiguration();
        string appId = "";

        public RemoteAuthor(string applicationIdentifier, HelperObjects.IAutomationConfiguration config =null)
        {
            if (config != null)
            {
                this.config = config;
            }

            appId = applicationIdentifier;
        }

        public override void SaveReport()
        {
            string url = config.RemoteAuthorURL;

            TestRun run = new TestRun()
            {
                ApplicationUnderTest = appId,
                HeaderData = base.testCaseHeader,
                Steps = base.recordedSteps
            };

          /*  HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
           */
        }

        public override bool StartNewTestCase(TestCaseHeaderData headerData)
        {
            return base.InitialzieNewTestCase(headerData);
        }

    }
}
