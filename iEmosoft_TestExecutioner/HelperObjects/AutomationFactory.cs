using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iEmosoft.Automation.Authors;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation.UIDrivers;

namespace iEmosoft.Automation
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

        public IUIDriver CreateDriver()
        {
            string uiDriverType = configuration.TestExecutionerUIDriverType;
            IUIDriver driver = null;

            switch (uiDriverType)
            {
                case "WEB":
                    driver = new Firefox();
                    break;
                case "WPF":
                    driver = new WindowsWhite();
                    break;
            }

            if (driver == null)
            {
                throw new Exception("Unable to create UI driver, configuration setting should be WPF or WEB, actual value: " + uiDriverType);
            }

            return driver;
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
                    result = new RemoteAuthor();
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
