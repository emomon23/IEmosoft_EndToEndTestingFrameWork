using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Limilabs.FTP.Client;
using iEmosoft.Automation.HelperObjects;

namespace iEmosoft.Automation.FTP
{
    public class ReportUploader : IReportUploader
    {
        IAutomationConfiguration configuration;
        private List<string> localReportFiles =null;
        private List<string> localReportImageFiles = null;
        private string sessionKey = null;
        private Ftp ftpRequest;
        private string applicationId;
        private string testName;
        private string locationOfReport;
        private string currentRemoteDirectory = "";

        public ReportUploader(IAutomationConfiguration configuration)
        {
            this.configuration = configuration;
            sessionKey = Guid.NewGuid().ToString();
        }


        public void UploadReport(string locationOfReport, string testName, bool ? deleteLocalFilesAfterUpload, string applicationId = "")
        {
            if (!deleteLocalFilesAfterUpload.HasValue)
            {
                deleteLocalFilesAfterUpload = configuration.FTPUpload_DeleteLocalFilesAfterUploadComplete;
            }

            this.applicationId = applicationId.isNull() ? configuration.ApplicationUnderTest : applicationId;
            this.testName = testName;
            this.locationOfReport = locationOfReport;

            if (GetFileNamesToBeUplaoded())
            {
                UploadReportFiles();
                UploadImageFiles();
                CallPostUploadWebservice();
                DisposeFTPConnection();

                if (deleteLocalFilesAfterUpload.Value)
                {
                    DeleteLocalFilesIfApplicable();
                }
            }
        }

        private void UploadReportFiles()
        {
            ConnectToServer();
            foreach (string rptFile in this.localReportFiles)
            {
                UploadFile(rptFile);
            }
        }

        private void UploadImageFiles()
        {
            ConnectToServer(true);
            foreach (string imgFile in localReportImageFiles)
            {
                UploadFile(imgFile);
            }
        }

        private void CallPostUploadWebservice()
        {
          /*  string url = configuration.PostUploadWebAPIServiceURL;

            if (!url.isNull())
            {
                url = string.Format("{0}/{1}/{2}/{3}", url, applicationId, testName, sessionKey);
                var client = WebRequest.Create(url);
                client.GetResponse();
            }
           */
        }

        private void DeleteLocalFilesIfApplicable() 
        {
            try
            {
                Directory.Delete(locationOfReport);
            }
            catch { }
        }

        private bool GetFileNamesToBeUplaoded()
        {
            localReportFiles = new List<string>();
            localReportImageFiles = new List<string>();

            if (Directory.Exists(locationOfReport))
            {
                foreach (string fileName in Directory.GetFiles(locationOfReport))
                {
                    if (fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase) || fileName.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
                    {
                        localReportFiles.Add(fileName);
                    }
                }

                foreach (string imageName in Directory.GetFiles(locationOfReport + "\\ScreenCaptures"))
                {
                    localReportImageFiles.Add(imageName);
                }
            }

            return localReportImageFiles.Count > 0 || localReportFiles.Count > 0;
        }

        private void ConnectToServer(bool navToScreenCapture = false)
        {
            currentRemoteDirectory = string.Format("{0}/{1}/{2}", applicationId, testName, sessionKey);
            if (navToScreenCapture)
            {
                currentRemoteDirectory += "/ScreenCaptures";
            }

            DisposeFTPConnection();
            ftpRequest = new Ftp();
            ftpRequest.Connect(configuration.FTPUploadURL);
            ftpRequest.Login(configuration.FTPUploadUserName, configuration.FTPUploadPassword);
            ftpRequest.CreateFolder(currentRemoteDirectory);
        }

        private void DisposeFTPConnection()
        {
            if (ftpRequest != null)
            {
                try
                {
                    ftpRequest.Close();
                }catch {}

                try {
                    ftpRequest.Dispose();
                }
                catch { }
            }
        }
        private void UploadFile(string file)
        {
            string repotpath = currentRemoteDirectory + "/" + Path.GetFileName(file);
            ftpRequest.Upload(repotpath, file);
        }
    }

    public interface IReportUploader
    {
        void UploadReport(string locationOfReport, string testName, bool? deleteLocalFilesAfterUpload, string applicationId = "");
    }
}
