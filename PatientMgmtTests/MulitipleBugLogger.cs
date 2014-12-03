using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.RecordableBrowser.Interfaces;
using iEmosoft.RecordableBrowser;
using iEmosoft.TestRecorderModel;
using iEmosoft.JiraBugEntry;
using iEmosoft.FogBugzBugEntry;

//For this demo, I wanted to save faled bugs to both FogBugz and Jira 
//so I'm creating a BugCreator that will pass through to both implementations

namespace iEmosoft.PatientMgmtTests
{
    public class MulitipleBugLogger : BugCreator
    {
        BugCreator fogBugz = null;
        BugCreator jira = null;

        public MulitipleBugLogger()
        {
            try
            {
                fogBugz = new FogBugsLogger("Sample Project");
            }
            catch { }

            try
            {
                jira = new iEmosoft.JiraBugEntry.BugLogger("TP");
            }
            catch { }
        }

        public override string CreateBug(TestCaseData header, List<TestCaseStep> steps)
        {
            string url = null;
            try
            {
               url = fogBugz.CreateBug(header, steps);
            }
            catch { }

            try
            {
              //We'll return the jira url, unless it fails
              url = jira.CreateBug(header, steps);
            }
            catch { }

            return url;
        }

        public override void Dispose()
        {
            try
            {
                fogBugz.Dispose();
            }
            catch { }

            try
            {
                jira.Dispose();
            }
            catch { }
        }
    }
}
