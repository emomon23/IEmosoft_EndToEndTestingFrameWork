using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRecorderModel
{
    public class TestCaseData
    {
        private string fileName="";
        public TestCaseData()
        {
            this.ExecutedOnDate = DateTime.Now.ToShortDateString();
            this.Priority = "HIGH";
            this.TestWriter = "Automated Test Writer";
            this.ExecutedByName = "Automated Tester";
        }

        public string TestNumber { get; set; }
        public string Prereqs { get; set; }
        public string TestName { get; set; }
        public string Priority { get; set; }
        public string TestWriter { get; set; }
        public string ExecutedByName { get; set; }
        public string ExecutedOnDate { get; set; }
        public string TestDescription { get; set; }
        public string SubFolder { get; set; }

        public string TestCaseFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(fileName))
                    return fileName;

                //If the filename has not specifically been set, derive it from the testname and test number
                if (!string.IsNullOrEmpty(TestNumber))
                {
                    return string.Format("{0}_{1}", TestNumber, TestName.Replace(" ", "_").Replace(".", ""));
                }

                return TestName.Replace(" ", "_").Replace(".", "");
            }
            set
            {
                this.fileName = value;
            }
        }

    }
}
