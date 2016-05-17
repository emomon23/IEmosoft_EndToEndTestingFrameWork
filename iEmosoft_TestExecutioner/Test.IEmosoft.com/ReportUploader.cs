using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Limilabs.FTP.Client;
using iEmosoft.Automation.HelperObjects;

namespace iEmosoft.Automation.Test.IEmosoft.com
{
    public class ReportUploader : IReportUploader
    {
        IAutomationConfiguration configuration;
        ReportFilesManager fileManager;
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
            this.applicationId = applicationId.isNull() ? configuration.ApplicationUnderTest : applicationId;
        }

        public string UploadReport(string testName, List<string> filesToUpload, bool? deleteFilesAfterUpload = null)
        {
            if (configuration.FTPUploadURL.isNull())
            {
                return string.Empty;
            }

            if (!deleteFilesAfterUpload.HasValue)
            {
                deleteFilesAfterUpload = configuration.FTPUpload_DeleteLocalFilesAfterUploadComplete;
            }
            
            this.testName = testName;
            if (configuration.TestReportFilePath.IsNotNull())
            {
                this.fileManager = new ReportFilesManager(filesToUpload, configuration.TestReportFilePath);
                return UploadLocalFilesFromALists(deleteFilesAfterUpload.Value);
            }

            return string.Empty;
        }

        public string UploadReport(string locationOfReport, string testName, bool ? deleteLocalFilesAfterUpload, string applicationId = "")
        {
            string result = "";

            if (!deleteLocalFilesAfterUpload.HasValue)
            {
                deleteLocalFilesAfterUpload = configuration.FTPUpload_DeleteLocalFilesAfterUploadComplete;
            }
                      
            this.testName = testName;
            this.locationOfReport = locationOfReport;

            if (GetFileNamesToBeUplaoded())
            {
                result = UploadLocalFilesFromALists(deleteLocalFilesAfterUpload.Value);
            }

            return result;
        }

        private string UploadLocalFilesFromALists(bool delete)
        {
            string result = UploadReportFiles();
            UploadImageFiles();
            DisposeFTPConnection();

            if (delete)
            {
                DeleteLocalFilesIfApplicable();
            }
            else
            {
                fileManager.RestoreReportFileCotents();
            }

            return result;
        }

        private string UploadReportFiles()
        {
            try
            {
                string result = "";

                ConnectToServer();
                foreach (string rptFile in this.fileManager.ReportFiles)
                {
                    result = UploadFile(rptFile);
                }

                return result;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void UploadImageFiles()
        {
            ConnectToServer(true);
            foreach (string imgFile in this.fileManager.ImageFiles)
            {
                UploadFile(imgFile);
            }
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
            var localReportFiles = new List<string>();
            if (Directory.Exists(locationOfReport))
            {
                foreach (string fileName in Directory.GetFiles(locationOfReport))
                {
                    if (fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase) || fileName.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
                    {
                        localReportFiles.Add(fileName);
                    }
                }

                foreach (string imageName in Directory.GetFiles(locationOfReport + "\\ScreenCapture"))
                {
                    localReportFiles.Add(imageName);
                }
            }

            if (localReportFiles.Count > 0)
            {
                fileManager = new ReportFilesManager(localReportFiles, configuration.TestReportFilePath);
            }
            return localReportFiles.Count > 0;
        }

        private string ConnectToServer(bool navToScreenCapture = false)
        {
            currentRemoteDirectory = string.Format("{0}/{1}/{2}", applicationId, testName, sessionKey);
            if (navToScreenCapture)
            {
                currentRemoteDirectory += "/ScreenCapture";
            }

            DisposeFTPConnection();
            ftpRequest = new Ftp();
            ftpRequest.Connect(configuration.FTPUploadURL);
            ftpRequest.Login(configuration.FTPUploadUserName, configuration.FTPUploadPassword);
            ftpRequest.CreateFolder(currentRemoteDirectory);

            return currentRemoteDirectory;
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
        private string UploadFile(string file)
        {
            string remotePath = currentRemoteDirectory + "/" + Path.GetFileName(file);
            ftpRequest.Upload(remotePath, file);

            return remotePath;
        }
    }

    public interface IReportUploader
    {
        string UploadReport(string testName, List<string> filesToUpload, bool? deleteFilesAfterUpload = null);
        string UploadReport(string locationOfReport, string testName, bool? deleteLocalFilesAfterUpload, string applicationId = "");
    }

    public class ReportFilesManager
    {
        List<string> imageFilesToUpload = new List<string>();
        List<string> reportFileNames = new List<string>();
        List<TestReportContents> reportFiles = new List<TestReportContents>();
        public ReportFilesManager(List<string> files, string replacementText)
        {
            foreach (string file in files)
            {
                bool isReportFile = file.EndsWith("html") || file.Contains(".xls");

                if (isReportFile){
                    reportFileNames.Add(file);
                    var c = new TestReportContents(file, replacementText);
                    reportFiles.Add(c);
                }
                else
                {
                    imageFilesToUpload.Add(file);
                }     
            
            }
        }

        public List<string> ImageFiles
        {
            get
            {
                return this.imageFilesToUpload;
            }
        }

        public List<string> ReportFiles
        {
            get
            {
                return this.reportFileNames;
            }
        }

        public void RestoreReportFileCotents()
        {
            foreach (var r in this.reportFiles)
            {
                r.RestoreReportFileContents();
            }
        }
        private class TestReportContents
        {
            private bool fileContentWasChanged = false;

            public TestReportContents(string filePath, string replaceText)
            {
                if (filePath.EndsWith(".html"))
                {
                    this.IsHTMLFile = true;
                    this.path = filePath;
                    this.OriginalContents = File.ReadAllText(filePath);

                    //This .html file is being uploaded to a server, it's current href="C:\\bla\bla\bla won't work
                    //need to strip the text off before uploading it, and restore the text when done uplaoding
                    if (!replaceText.isNull())
                    {
                        string newContents = this.OriginalContents.Replace(replaceText, "");
                        File.WriteAllText(filePath, newContents);
                        fileContentWasChanged = true;
                    }
                }
            }
            public string OriginalContents { get; set; }
            public string path { get; set; }

            public bool IsHTMLFile {get;set;}

            public void RestoreReportFileContents()
            {
                if (fileContentWasChanged)
                {
                    File.WriteAllText(this.path, this.OriginalContents);
                }
            }
        }
    }
}
