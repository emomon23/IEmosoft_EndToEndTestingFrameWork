using System.Collections.Generic;

namespace aUI.Automation.Baseline
{

    public class BaselineDescrepencyCheck
    {
        public string Key { get; set; }
        public BaselineDescrepencyCheck()
        {
            Mismatches = new List<Mismatch>();
        }

        public List<Mismatch> Mismatches { get; set; }

        public void InsertMismatch(string fieldName, string expectedValue, string actualValule, bool append = true)
        {
            var mismatch = new Mismatch()
            {
                ActualValue = actualValule,
                ExpectedValue = expectedValue,
                PropertyName = fieldName
            };

            if (append)
            {
                Mismatches.Add(mismatch);
            }
            else
            {
                Mismatches.Insert(0, mismatch);
            }
        }
    }

    public class Mismatch
    {
        public string PropertyName { get; set; }
        public string ExpectedValue { get; set; }
        public string ActualValue { get; set; }
    }
}
