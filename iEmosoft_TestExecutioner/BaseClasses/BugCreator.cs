using aUI.Automation.HelperObjects;
using aUI.Automation.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aUI.Automation.BaseClasses
{
    public abstract class BugCreator : IDisposable
    {
        protected TestCaseHeaderData Header = null;
        protected List<TestCaseStep> Steps = null;

        protected string BugTitle = "";
        protected string BugDescription = "";


        //Make sure to call base.InitializeBugCreator when you override this method!
        public abstract string CreateBug(TestCaseHeaderData header, List<TestCaseStep> steps);

        protected void InitializeBugCreator(TestCaseHeaderData header, List<TestCaseStep> steps)
        {
            Header = header;
            Steps = steps;
            InitializeBugTitleAndSummary();
        }

        protected string PathToFailedImage
        {
            get
            {
                var failedStep = GetFailedStep();
                if (failedStep != null && string.IsNullOrEmpty(failedStep.ImageFilePath) == false)
                    return failedStep.ImageFilePath;

                return "";
            }
        }

        protected bool ImageFileExists
        {
            get
            {
                var failedStep = GetFailedStep();
                return failedStep != null || failedStep.ImageFilePath.IsNull() == false;
            }
        }

        protected string GetTestStepsParagraph()
        {
            return GetTestStepsParagraph("\n\n");
        }

        protected string GetTestStepsParagraph(string stepSeperator)
        {
            string result = stepSeperator;

            for (int i = 0; i < Steps.Count; i++)
            {
                var step = Steps[i];

                result += string.Format("Step {0}: {1}{2}", (i + 1) * 10, step.StepDescription, stepSeperator);
            }

            return result;
        }

        protected TestCaseStep GetFailedStep()
        {
            return Steps.FirstOrDefault(s => s.StepPassed == false);
        }

        private void InitializeBugTitleAndSummary()
        {
            string testNumber = Header.TestNumber.IsNull() ? "" : Header.TestNumber + " - ";
            var badStep = GetFailedStep();

            BugTitle = string.Format("{0}{1}", testNumber, badStep.ActualResult.Replace(", see image for details", ""));
            BugDescription = string.Format("- {1}{0} - Prereqs: {2}{0}{0}Steps to reproduce:{3}", "\n",
                Header.TestDescription,
                Header.Prereqs,
                GetTestStepsParagraph());
        }

        public abstract void Dispose();
    }
}
