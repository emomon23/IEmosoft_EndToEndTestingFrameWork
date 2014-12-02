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
            client.CreateIssue(newIssue);
            return this.GetPreviouslyEnteredBugURL();
        }
        
        private string GetPreviouslyEnteredBugURL()
        {
            string filter=string.Format("project={0} AND status=Open", this.projectName, base.BugTitle);
            string result = null;

            try
            {
                var existingIssues = client.GetIssuesByJql(filter, 0, 500);
                if (existingIssues != null && existingIssues.issues != null && existingIssues.issues.Count > 0)
                {
                    var duplicate = existingIssues.issues.FirstOrDefault(i => i.fields.summary == base.BugTitle);

                    if (duplicate != null)
                    {
                        result = string.Format("{0}/browse/{1}", this.jiraURL, duplicate.key);
                    }
                }

                return result;
            }
            catch (Exception exp)
            {
                return string.Format("Exception in BugLogger.GetPreviouslyEnteredBugURL(), unable to automate bug creation in Jira: {0}", exp.ToString());
            }
        }
        
        public override void Dispose()
        {
            //Nothing to dispose of here
        }
    }
}
