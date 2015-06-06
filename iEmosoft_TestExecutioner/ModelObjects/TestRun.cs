using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iEmosoft.Automation.Model
{
    public class TestRun
    {
        List<TestCaseStep> steps = new List<TestCaseStep>();
        TestCaseHeaderData headerData;

        public TestRun()
        {
            this.RunDate = DateTime.Now;
        }

        public string ApplicationUnderTest { get; set; }
        public DateTime RunDate { get; set; }
        public double RuntimeInMinutes { get; set; }

        public List<TestCaseStep> Steps
        {
            get { return steps; }
            set { steps = value; }
        }

        public TestCaseHeaderData HeaderData
        {
            get
            {
                return headerData;
            }
            set
            {
                headerData = value;
            }
        }
    }
}
