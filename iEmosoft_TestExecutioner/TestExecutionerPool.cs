using aUI.Automation.ModelObjects;
using System.Collections.Generic;

namespace aUI.Automation
{
    //This class allows you to pool and reuse TestExecutioners (and Browser instances).
    //For example, you might have 300 tests, but 10 browsers are able to be reused for each of those tests
    //more efficient
    public static class TestExecutionerPool
    {
        private static Dictionary<string, TestExecutioner> ListOfExecutioners = new();

        public static TestExecutioner GetTestExecutioner(TestCaseHeaderData header, string userName, string password, string defaultLandingPage = "")
        {
            var result = GetAvailableExecutioner(userName + password + defaultLandingPage);

            if (result == null)
            {
                result = CreateNewExecutioner(header, userName + password, defaultLandingPage);
            }
            else
            {
                result.StartNewTestCase(header);
            }

            return result;
        }

        public static void ClosePool()
        {
            foreach (var item in ListOfExecutioners)
            {
                try
                {
                    item.Value.Dispose();
                }
                catch { }
            }
        }

        private static TestExecutioner GetAvailableExecutioner(string key)
        {
            if (ListOfExecutioners.ContainsKey(key))
            {
                var result = ListOfExecutioners[key];
                if (result.PoolState.IsAvailable)
                {
                    result.PoolState.IsAvailable = false;
                    result.PoolState.IsPartOfTestExecutionerPool = true;
                    return result;
                }
            }

            return null;
        }

        private static TestExecutioner CreateNewExecutioner(TestCaseHeaderData header, string key, string defaultLandingPage)
        {

            var result = new TestExecutioner(header);
            result.PoolState.IsPartOfTestExecutionerPool = true;
            result.PoolState.IsAvailable = false;
            result.PoolState.LangingPageURL = defaultLandingPage;

            ListOfExecutioners.Add(key + defaultLandingPage, result);
            return result;
        }
    }
}
