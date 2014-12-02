using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using RecordableBrowser;
using RecordableBrowser.Interfaces;
using AnotherJiraRestClient;
using AnotherJiraRestClient.JiraModel;

namespace JiraBugEntry
{
    public class BugLogger : BugCreator
    {
        JiraClient client = null;

        public override void CreateBug(TestRecorderModel.TestCaseData header, List<TestRecorderModel.TestCaseStep> steps)
        {
            base.InitializeBugCreator(header, steps);
            this.InitializeJiraClient();

            if (this.BugPreviouslyEntered())
                return;

            this.CreateNewBug();
       }

        private void InitializeJiraClient()
        {
            if (client == null)
            {
                //Replace these 4 lines of code with your Jira creds and url
                JiraInstance credentialsReader = new JiraInstance();
                string userName = credentialsReader.Username;
                string password = credentialsReader.Password;
                string url = credentialsReader.Url;

                JiraAccount account = new JiraAccount()
                                        {
                                            Password = password,
                                            ServerUrl = url ,
                                            User = userName
                                        };

                client = new JiraClient(account);
            }
        }

        private void CreateNewBug()
        {
           
        }
        
        private bool BugPreviouslyEntered()
        {
            return false;
        }
        
        public override void Dispose()
        {
          
        }
    }
}
