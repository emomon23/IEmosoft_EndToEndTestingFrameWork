using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.BaseClasses
{
    public abstract class BugCreator : IDisposable
    {
        protected TestCaseData header = null;
        protected List<TestCaseStep> steps = null;

        protected string BugTitle = "";
        protected string BugDescription = "";
     

        //Make sure to call base.InitializeBugCreator when you override this method!
        public abstract string CreateBug(TestCaseData header, List<TestCaseStep> steps);

        protected void InitializeBugCreator(TestCaseData header, List<TestCaseStep> steps)
        {
            this.header = header;
            this.steps = steps;
            this.InitializeBugTitleAndSummary();
        }

        protected string PathToFailedImage
        {
            get
            {
                var failedStep = this.GetFailedStep();
                if (failedStep != null && failedStep.ImageFilePath.IsNull() == false)
                    return failedStep.ImageFilePath;

                return "";
            }
        }

        protected bool ImageFileExists
        {
            get
            {
                var failedStep = this.GetFailedStep();
                return failedStep != null || failedStep.ImageFilePath.IsNull() == false;
            }
        }

        protected string GetTestStepsParagraph()
        {
            return this.GetTestStepsParagraph("\n\n");
        }

        protected string GetTestStepsParagraph(string stepSeperator)
        {
            string result = stepSeperator;

            for (int i=0; i<steps.Count(); i++)
            {
                var step = steps[i];

                result += string.Format("Step {0}: {1}{2}", (i+1) * 10, step.StepDescription, stepSeperator);
            }

            return result;
        }

        protected TestCaseStep GetFailedStep()
        {
            return steps.FirstOrDefault(s => s.StepPassed == false);
        }

        private void InitializeBugTitleAndSummary()
        {
            string testNumber = header.TestNumber.IsNull() ? "" : header.TestNumber + " - ";
            var badStep = this.GetFailedStep();

            this.BugTitle = string.Format("{0}{1}", testNumber, badStep.ActualResult.Replace(", see image for details", ""));
            this.BugDescription = string.Format("- {1}{0} - Prereqs: {2}{0}{0}Steps to reproduce:{3}", "\n",
                header.TestDescription,
                header.Prereqs,
                GetTestStepsParagraph());
        }

        public abstract void Dispose();
    }
}
