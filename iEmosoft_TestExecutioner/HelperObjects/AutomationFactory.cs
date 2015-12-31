using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iEmosoft.Automation.Authors;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation.ScreenCaptures;
using iEmosoft.Automation.UIDrivers;
using iEmosoft.Automation.FTP;

namespace iEmosoft.Automation.HelperObjects
{
    public class AutomationFactory
    {
        private IAutomationConfiguration configuration;

        public AutomationFactory(IAutomationConfiguration config = null)
        {
            if (config == null)
            {
                configuration = new AutomationConfiguration();
            }
            else
            {
                configuration = config;
            }
        }
        
        public BaseAuthor CreateAuthor(string rootPath = null)
        {
            string authorTypeName = configuration.TestExecutionerAuthorTypeName;
            return NewUpAuthor(authorTypeName, rootPath);
        }

        public IUIDriver CreateUIDriver()
        {
            string browserName = configuration.TestExecutionerUIDriverType;
            IUIDriver driver = null;

            switch (browserName)
            {
                case "FIREFOX":
                    driver = new BrowserDriver();
                    break;
                case "IE":
                    driver = new BrowserDriver(BrowserDriver.BrowserDriverEnumeration.IE);
                    break;
                case "CHROME":
                    driver = new BrowserDriver(BrowserDriver.BrowserDriverEnumeration.Chome);
                    break;
                case "WPF":
                    driver = new WindowsWhite();
                    break;
                default:
                    throw new Exception("Unknown UI driver type in config, expected 'WPF', 'CHOME', 'IE','FIREFOX'");
            }

            if (driver == null)
            {
              //  throw new Exception("Unable to create UI driver, configuration setting should be WPF or WEB, actual value: " + uiDriverType);
            }

            return driver;
        }

        public IScreenCapture CreateScreenCapturer(string rootPathOrURL = null)
        {
            return new LocalScreenCapture(rootPathOrURL.isNull() ? configuration.ScreenCaptureLocalPath : rootPathOrURL);
        }

        public IReportUploader CreateReportFTPUploader()
        {
            return new ReportUploader(this.configuration);
        }

    
        private BaseAuthor NewUpAuthor(string authorTypeName, string rootPath = null)
        {
            BaseAuthor result = null;
            var reportPath = rootPath.isNull() ? configuration.TestReportFilePath : rootPath;
            
            switch (authorTypeName)
            {
                case "EXCEL":
                    result = new ExcelAuthor(reportPath);
                    break;
               
                case "HTML":
                    result = new HTMLAuthor(reportPath);
                    break;
            }

            if (result == null)
            {
                throw new Exception(string.Format("Config setting (for 'TestExecutionerAuthor / MultiAuthor_Authors' should be / contain 'MULTIPLE', 'EXCEL', 'REMOTE', or 'HTML', actual value '{0}'. Unable to create author object", authorTypeName));
            }

            return result;
        }

    }
}
