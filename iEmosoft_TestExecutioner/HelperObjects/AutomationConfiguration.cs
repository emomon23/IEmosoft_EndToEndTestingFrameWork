using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace iEmosoft.Automation.HelperObjects
{
    public interface IAutomationConfiguration
    {
        string TestExecutionerAuthorTypeName { get; }
        string TestReportFilePath { get; }
        string ScreenCaptureLocalPath { get; }
        string FTPUploadURL { get; }
        string FTPUploadUserName { get; }
        string FTPUploadPassword { get; }
        string PostUploadWebAPIServiceURL { get; }
        string ApplicationUnderTest { get; }
        bool FTPFilesInTestProcess { get; }
        bool FTPUpload_DeleteLocalFilesAfterUploadComplete { get; }
        string SauceLabsKey { get; }
        string SauceLabsBrowser { get; }
        string SauceLabsPlatform { get; }
        string TestExecutionerUIDriverType { get; }
    }

    public class AutomationConfiguration : IAutomationConfiguration
    {
        public string TestExecutionerAuthorTypeName
        {
            get
            {
                return GetConfigSetting("TestExecutionerAuthor").ToUpper(); 
            }
        }

        public string ScreenCaptureLocalPath
        {
            get { return System.IO.Path.Combine(GetConfigSetting("TestReportFilePath"), "ScreenCapture"); }
        }

        public string SauceLabsKey
        {
            get
            {
                return GetConfigSetting("SauceLabsKey");
            }
        }

        public string SauceLabsBrowser
        {
            get
            {
                return GetConfigSetting("SauceLabsBrowser", "Firefox");
            }
        }
        public string SauceLabsPlatform
        {
            get
            {
                return GetConfigSetting("SauceLabsPlatform", "Windows 7");
            }
        }
        public bool FTPFilesInTestProcess
        {
            get
            {
                bool result = false;
                bool.TryParse(GetConfigSetting("FTPFilesInTestProcess", "false"), out result);
                return result;
            }
        }
        public string ApplicationUnderTest
        {
            get
            {
                return GetConfigSetting("ApplicationIdUnderTest");
            }
        }

        public string FTPUploadURL
        {
            get
            {
                //ftp://www.serverURL.com|userName|password|true
                return GetConfigSetting("FTPUploadURLAndCredentials").Split('|')[0].Trim(); ;
            }
        }

      
        public string FTPUploadUserName
        {
            get
            {
                //ftp://www.serverURL.com|userName|password|true
                return GetConfigSetting("FTPUploadURLAndCredentials").Split('|')[1].Trim();
            }
        }

        public bool FTPUpload_DeleteLocalFilesAfterUploadComplete
        {
            get
            {
                bool result = false;

                //ftp://www.serverURL.com|userName|password|true
                string [] temp = GetConfigSetting("FTPUploadURLAndCredentials").Split('|');
                if (temp.Length > 3)
                {
                    bool.TryParse(temp[3], out result);
                }

                return result;
            }
        }
        public string FTPUploadPassword
        {
            get
            {
                //ftp://www.serverURL.com|userName|password|true
                return GetConfigSetting("FTPUploadURLAndCredentials").Split('|')[2].Trim(); ;
            }
        }

        public string TestExecutionerUIDriverType
        {
            get
            {
                //Browse or WindowsWhite
                return GetConfigSetting("TestExecutionerUIDriver").ToUpper();
            }
        }

        public string PostUploadWebAPIServiceURL
        {
            get
            {
                return GetConfigSetting("PostUploadWebAPIServiceURL");
            }
        }
         
               
        public string TestReportFilePath
        {
            get { return GetConfigSetting("TestReportFilePath"); }
        }

        private string GetConfigSetting(string settingName, string resultIfNotFound = null)
        {
            try
            {
                var result = System.Configuration.ConfigurationManager.AppSettings[settingName];
                return result.ToString();
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Unable to find '{0}' in the config file, this is required", settingName));
            }
        }

    }
}
