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
        string[] MultiAuthor_Authors { get; }
        string TestReportFilePath { get; }
        string TestExecutionerUIDriverType { get; }
        string TestExecutionerScreenCapturer { get; }
        string ScreenCaptureLocalPath { get; }
        string ScreenCaptureRemoteServerURL { get; }
        string RemoteAuthorURL { get; }
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

        public string ScreenCaptureRemoteServerURL
        {
            get
            {
                return GetConfigSetting("ScreenCaptureRemoteServerUri");
            }
        }

        public string TestExecutionerScreenCapturer
        {
            get { return GetConfigSetting("TestExecutionerScreenCaptureType").ToUpper(); }
        }

        public string RemoteAuthorURL
        {
            get
            {
                return GetConfigSetting("RemoteAuthorURL");
            }
        }

        public string TestExecutionerUIDriverType
        {
            get
            {
                return GetConfigSetting("TestExecutionerUIDriver").ToUpper();
            }
        }

        public string [] MultiAuthor_Authors
        {
            get { return GetConfigSetting("MultiAuthor_Authors").Replace(" ", "").ToUpper().Split(','); }
        }

        public string TestReportFilePath
        {
            get { return GetConfigSetting("TestReportFilePath"); }
        }

        private string GetConfigSetting(string settingName, string resultIfNotFound = null)
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings[settingName].ToString();
            }
            catch
            {
                throw new Exception(string.Format("Unable to find '{0}' in the config file, this is required", settingName));
            }
        }

    }
}
