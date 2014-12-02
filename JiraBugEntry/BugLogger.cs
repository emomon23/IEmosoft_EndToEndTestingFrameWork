using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using iEmosoft.RecordableBrowser;
using iEmosoft.RecordableBrowser.Interfaces;
using AnotherJiraRestClient;
using AnotherJiraRestClient.JiraModel;

namespace iEmosoft.JiraBugEntry
{
    public class BugLogger : BugCreator
    {
        private string projectName = "";
        private string jiraURL = "";
        private JiraClient client = null;

        public BugLogger(string projectName)
        {
            this.projectName = projectName;
        }

        public override string CreateBug(TestRecorderModel.TestCaseData header, List<TestRecorderModel.TestCaseStep> steps)
        {
            base.InitializeBugCreator(header, steps);
            this.InitializeJiraClient();

            string result = this.GetPreviouslyEnteredBugURL();
            if (result.IsNull())
            {
                result = this.CreateNewBug();
            }

            return result;
       }

        private void InitializeJiraClient()
        {
            if (client == null)
            {
                //Replace these 4 lines of code with your Jira creds and url
                JiraInstance credentialsReader = new JiraInstance();
                string userName = credentialsReader.Username;
                string password = credentialsReader.Password;
                this.jiraURL = credentialsReader.Url;

                JiraAccount account = new JiraAccount()
                                        {
                                            Password = password,
                                            ServerUrl = jiraURL,
                                            User = userName
                                        };

                client = new JiraClient(account);
            }
        }

        private string CreateNewBug()
        {
            //Need to implement this
            CreateIssue newIssue = new CreateIssue(this.projectName, base.BugTitle, base.BugDescription, "1", "1", null);
            return this.GetPreviouslyEnteredBugURL();
        }
        
        private string GetPreviouslyEnteredBugURL()
        {
            string filter=string.Format("project={0} AND summary={1} AND status=Active", this.projectName, base.BugTitle);
            string result = null;

            var existingIssues = client.GetIssuesByJql(filter, 0, 50);
            if (existingIssues != null && existingIssues.issues != null && existingIssues.issues.Count > 0)
            {
                result = string.Format("{0}/browse/{1}", this.jiraURL, existingIssues.issues[0].key);
            }
            
            return result;
        }
        
        public override void Dispose()
        {
            //Nothing to dispose of here
        }
    }
}
