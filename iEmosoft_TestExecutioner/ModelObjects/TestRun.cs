using System;
using System.Collections.Generic;

namespace aUI.Automation.ModelObjects
{
    public class TestRun
    {
        List<TestCaseStep> Steps { get; set; } = new List<TestCaseStep>();
        public TestCaseHeaderData HeaderData { get; set; }

        public TestRun()
        {
            RunDate = DateTime.Now;
        }

        public string ApplicationUnderTest { get; set; }
        public DateTime RunDate { get; set; }
        public double RuntimeInMinutes { get; set; }
    }
}
