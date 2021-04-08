using System;

namespace aUI.Automation.ModelObjects
{
    public class TestCaseHeaderData
    {
        private string FileName = "";
        private string testName;

        public TestCaseHeaderData()
        {
            ExecutedOnDate = DateTime.Now.ToString();
            Priority = "HIGH";
            TestWriter = "Automated Test Writer";
            ExecutedByName = "Automated Tester";
        }

        public string TestNumber { get; set; }
        public string Prereqs { get; set; }

        public string TestFamily { get; set; }
        public string TestName
        {
            get
            {
                string result = testName;

                if (string.IsNullOrEmpty(result))
                {
                    int index = TestDescription.ToUpper().IndexOf("THAN");
                    if (index > 0)
                    {
                        result = result[(index + 5)..];
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
                if (!string.IsNullOrEmpty(FileName))
                    return FileName;

                //If the filename has not specifically been set, derive it from the testname and test number
                if (!string.IsNullOrEmpty(TestNumber))
                {
                    return string.Format("{0}_{1}", TestNumber, TestName.Replace(" ", "_").Replace(".", ""));
                }

                return TestName.Replace(" ", "_").Replace(".", "");
            }
            set
            {
                FileName = value;
            }
        }

    }
}
