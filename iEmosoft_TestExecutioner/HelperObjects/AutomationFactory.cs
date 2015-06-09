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
            string []browserNames = configuration.TestExecutionerUIDriverType;
            IUIDriver driver = null;

            if (browserNames.Length > 1)
            {
                if (browserNames[0] == "WPF")
                {
                    return new WindowsWhite();
                }

                return new MultiBrowser(configuration);
            }

            switch (browserNames[0])
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
                throw new Exception("Unable to create UI driver, configuration setting should be WPF or WEB, actual value: " + uiDriverType);
            }

            return driver;
        }

        public IScreenCapture CreateScreenCapturer(string rootPathOrURL = null)
        {
            IScreenCapture result = null;

            switch (configuration.TestExecutionerScreenCapturer)
            {
                case "LOCAL":
                    result = new LocalScreenCapture(rootPathOrURL.isNull()? configuration.ScreenCaptureLocalPath : rootPathOrURL);
                    break;
                case "REMOTE":
                    result = new RemoteScreenCapture(rootPathOrURL.isNull()? configuration.ScreenCaptureRemoteServerURL : rootPathOrURL);
                    break;
                    
            }

            if (result == null)
            {
                throw new Exception(string.Format("expected configuration setting for screen capture to be 'REMOTE' or 'LOCAL'. Actual value '{0}'.  Unable to create screen capture object", configuration.TestExecutionerScreenCapturer));
            }

            return result;
            
        }

        public MultipleDestinationsAuthor CreateMultiAuthor(string reportPath)
        {
            MultipleDestinationsAuthor result = new MultipleDestinationsAuthor();

            string[] authorNames = configuration.MultiAuthor_Authors;
            
            foreach (string authorName in authorNames)
            {
                if (!string.IsNullOrEmpty(authorName) && !authorName.Contains("MULTIPLE"))
                {
                    result.Authors.Add(NewUpAuthor(authorName));        
                }
            }

            if (result.Authors.Count == 0)
            {
                throw new Exception("Unable to create MutipleDestinationAuthor, config setting 'MultiAuthor_Authors', contains no valid values");
            }

            return result;
        }

        private BaseAuthor NewUpAuthor(string authorTypeName, string rootPath = null)
        {
            BaseAuthor result = null;
            var reportPath = rootPath.isNull() ? configuration.TestReportFilePath : rootPath;
            
            switch (authorTypeName)
            {
                case "MULTIPLE":
                    result = CreateMultiAuthor(reportPath);
                    break;
                case "EXCEL":
                    result = new ExcelAuthor(reportPath);
                    break;
                case "REMOTE":
                    result = new RemoteAuthor("");
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
