using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using RecordableBrowser;
using RecordableBrowser.Interfaces;
using OpenQA;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;

namespace JiraBugEntry
{
    public class BugLogger : BugCreator
    {
        private string loginURL = "https://www.lokitali.com/loki_jira/login.jsp";
        private IWebDriver browser;
       
        public override void CreateBug(TestRecorderModel.TestCaseData header, List<TestRecorderModel.TestCaseStep> steps)
        {
            browser = new FirefoxDriver();
            base.InitializeBugCreator(header, steps);

            this.Login();

            if (this.BugPreviouslyEntered())
                return;

            this.CreateNewBug();
       }

        private void CreateNewBug()
        {
            browser.ClickElement(By.Id(LokiControls.DashboardPage.CreateBugLinkId));

            browser.SetTextOnElement(LokiControls.CreateIssuePage.SummaryId, BugTitle);
            browser.SetTextOnElement(LokiControls.CreateIssuePage.DueDateId, DateTime.Now.AddDays(7).ToShortDateString());
            browser.SetTextOnElement(LokiControls.CreateIssuePage.DescriptionId, BugDescription);

            if (base.ImageFileExists)
            {
                browser.ClickElement(By.Id(LokiControls.CreateIssuePage.AttachementChooseFileButtonId));
                System.Threading.Thread.Sleep(3000);
                SendKeys.Send(base.PathToFailedImage + "{ENTER}");
                System.Threading.Thread.Sleep(3000);
            }

            browser.ClickElement(By.Id(LokiControls.CreateIssuePage.SubmitButtonId));
        }

        private void Login()
        {
            browser.Navigate().GoToUrl(loginURL);
            browser.SetTextOnElement(LokiControls.LoginPage.UserNameId, "memo");
            browser.SetTextOnElement(LokiControls.LoginPage.PasswordId, "NotReallyMyPassword");
            browser.ClickElement(By.Id(LokiControls.LoginPage.LoginSubmitButtonId));
        }

        private bool BugPreviouslyEntered()
        {
            if (!browser.PageContains("System Dashboard"))
                return true;  //We're not on the dashboard, don't try to proceed, just say thie bug already exists and end it there

            string subsetDescription = this.BugDescription.Substring(0, 25);

            return browser.PageContains(BugTitle) && browser.PageContains(subsetDescription);
        }
        
        public override void Dispose()
        {
            browser.Quit();
        }
    }
}
