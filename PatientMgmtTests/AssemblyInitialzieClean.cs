using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iEmosoft.Automation;

namespace PatientMgmtTests
{
    [TestClass]
    public class AssemblyInitialzieClean
    {
        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            TestExecutionerPool.ClosePool();
        }

    }
}
