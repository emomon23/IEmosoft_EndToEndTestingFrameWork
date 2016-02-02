using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft.Automation.Test.IEmosoft.com;
using iEmosoft.Automation.HelperObjects;

namespace TestUploaderConsoleApp
{
    public class UploadFilesProcess
    {
        IReportUploader uploader;

        public UploadFilesProcess(){
            AutomationFactory factory = new AutomationFactory();
            uploader = factory.CreateReportFTPUploader();
        }
        
        public void UploadReport(string locationOfReport, string testName, string applicationId)
        {
            uploader.UploadReport(locationOfReport, testName, this.DeleteFilesAfterUpload, applicationId);
        }

        private bool DeleteFilesAfterUpload
        {
            get
            {
                var setting = System.Configuration.ConfigurationManager.AppSettings["DeleteAfterUpload"];
                bool result = false;

                bool.TryParse(setting, out result);
                return result;
            }
        }
    }
}
