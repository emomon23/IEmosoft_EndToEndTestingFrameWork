using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEmosoft.Automation.Model
{
    public class TestCaseHeaderData
    {
        private string fileName="";
        private string testName;

        public TestCaseHeaderData()
        {
            this.ExecutedOnDate = DateTime.Now.ToString();
            this.Priority = "HIGH";
            this.TestWriter = "Automated Test Writer";
            this.ExecutedByName = "Automated Tester";
        }

        public string TestNumber { get; set; }
        public string Prereqs { get; set; }
        public string TestName
        {
            get
            {
                string result = testName;

                if (string.IsNullOrEmpty(result))
                {
                    int index = this.TestDescription.ToUpper().IndexOf("THAN");
                    if (index > 0)
                    {
                        result = result.Substring(index + 5);
                    }
                }

                return result;
            }
            set { testName = value; }
        }

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
