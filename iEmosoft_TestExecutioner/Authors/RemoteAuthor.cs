using System;
using System.IO;
using iEmosoft.Automation.BaseClasses;
using iEmosoft.Automation.Model;

namespace iEmosoft.Automation.Authors
{
    public class RemoteAuthor : BaseAuthor
    {
        public override void SaveReport()
        {
           
        }

        public override bool StartNewTestCase(TestCaseHeaderData headerData)
        {
            return base.InitialzieNewTestCase(headerData);
        }

    }
}
