using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.RecordableBrowser.Interfaces;
using iEmosoft.TestRecorderModel;

namespace iEmosoft.FogBugzBugEntry
{
    public class FogBugsLogger : BugCreator
    {
        private FogBugzServer fServer;
        private string baseURL = "";
        private string project = "";

        public FogBugsLogger(string project)
        {
            FogBugzInstance instanceCredentials = new FogBugzInstance();
            fServer = new FogBugzServer(instanceCredentials.Url, instanceCredentials.Username, instanceCredentials.Password);
            this.baseURL = instanceCredentials.Url;
            this.project = project;
        }

        public override string CreateBug(TestCaseData header, List<TestCaseStep> steps)
        {
            string bugURL = string.Format("{0}/f/cases/", this.baseURL);

            base.InitializeBugCreator(header, steps);

            var cases = fServer.Search("Active", BugTitle);
            if (cases.Count > 0)
            {
                return bugURL + cases[0].Id.ToString();
            }

            var newBug = fServer.CreateBug(new FBug(){ Title = BugTitle, Description = BugDescription, Status = "Active", Category = "Bug", Project = this.project });
            return bugURL + newBug.Id.ToString();
        }

        public override void Dispose()
        {
            fServer.Dispose();
        }
    }
}
