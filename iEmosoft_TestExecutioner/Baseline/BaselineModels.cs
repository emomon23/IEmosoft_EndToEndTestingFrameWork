using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using iEmosoft.Automation.Interfaces;
using iEmosoft.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace iEmosoft.Automation
{
    
    public class BaselineDescrepencyCheck
    {
        public string Key { get; set; }
        public BaselineDescrepencyCheck()
        {
            this.Mismatches = new List<Mismatch>();
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
                this.Mismatches.Add(mismatch);
            }
            else
            {
                this.Mismatches.Insert(0, mismatch);
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
