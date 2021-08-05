using aUI.Automation.Enums;
using aUI.Automation.HelperObjects;
using aUI.Automation.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aUI.Automation.Authors
{
    public class XRay
    {
        enum Endpts
        {
            [Api("/api/v2/authenticate")] Authenticate,
            [Api("/api/v2/graphql")] Graph,
            [Api("/api/v2/import/execution")] ImportResults,
        }

        /* Config settings needed:
         * XRayToken
         * XRayBase
         * XRayProject
         * 
         * XRayRetiredFolder
         * XRayTestFolder
         */

        private int EditTestThreshold = 25;
        string Project = Config.GetConfigSetting("XRayProject");
        string TestFolder = Config.GetConfigSetting("XRayTestFolder");
        string RetiredFolder = Config.GetConfigSetting("XRayRetiredFolder");
        string Env = Config.GetEnvironment();
        string ExecutionName = Config.GetConfigSetting("TestRunName", "Automation Test Execution");
        string ExecutionId = "";
        string ExecutionKey = "";

        List<string> TestEnvs = new List<string>();
        List<string> Folders = new List<string>();
        string ProjectId = "";
        Api ApiObj;

        List<XRayTest> Tests = new List<XRayTest>();


        public XRay(string testFolder = "")
        {
            ApiObj = new Api(null, Config.GetConfigSetting("XRayBase"));
            ApiObj.SetAuthentication(GetAuthentication());
            ProjectId = GetProjectSettings();

            if (!string.IsNullOrEmpty(testFolder))
            {
                TestFolder = testFolder;
            }

            GetFolders(ProjectId);
            GetAllTestCases();
        }

        #region Public methods
        public void CreateTestRun(List<string> testCases = null)
        {
            if(testCases == null)
            {
                testCases = new List<string>();
                Tests.ForEach(x => testCases.Add(x.IssueId));
            }

            ExecutionName += $" {DateTime.Now:MM/dd/yy H:mm:ss}";

            var cases = string.Join(", ", testCases.Select(x => string.Format("\"{0}\"", x)));
            var query = "mutation {createTestExecution(testIssueIds: [" + cases + "] testEnvironments: [\"" + Env + "\"] jira: {fields: { summary: \"" + ExecutionName + "\", project: {key: \"" + Project + "\"} }}) {testExecution {issueId jira(fields: [\"key\"])} warnings createdTestEnvironments}}";

            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");

            ExecutionKey = (string)rsp.data.createTestExecution.testExecution.jira.key;
            ExecutionId = (string)rsp.data.createTestExecution.testExecution.issueId;
        }

        public void CloseTestRun()
        {
            //looks like 1-2 jira calls
        }

        public void AddTestResult(TestExecutioner te, string testName)
        {
            var testCase = FindTestCase(testName, te);

            AddTestToTestRun(testCase.IssueId);
            AddTestResults(te, testCase.IssueKey);
        }
        #endregion

        private string GetProjectSettings()
        {
            var query = "query {getProjectSettings (projectIdOrKey: \"XT\") {projectId, testEnvironments}}";
            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");

            foreach(var env in ApiHelper.GetRspList(rsp.data.getProjectSettingstestEnvironments))
            {
                TestEnvs.Add((string)env);
            }

            return (string)rsp.data.getProjectSettings.projectId;
        }

        private void GetFolders(string projectId)
        {
            var query = "query {getFolder(projectId: \"10022\", path: \"/\") {path folders}}";
            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");

            //deal with nested folders later
            foreach(var folder in ApiHelper.GetRspList(rsp.data.getFolder.folders))
            {
                var path = (string)folder.path;
                if (path.Contains(TestFolder))
                {
                    TestFolder = path;
                }
                else if(path.Contains(RetiredFolder))
                {
                    RetiredFolder = path;
                }

                Folders.Add(path);
            }
        }

        private void GetAllTestCases()
        {
            var testReturnLimit = 100;
            var start = -testReturnLimit;

            int totalCount;
            do
            {
                start += testReturnLimit;
                //TODO Update query to get the test name back

                var folder = TestFolder.Contains("/") ? " folder: {path: \"" + TestFolder +"\"}": "";

                var query = "query { getTests(projectId: \"" + ProjectId + "\" limit: "+ testReturnLimit + " start: " + start + folder + ") {total results { issueId projectId testType {name} steps {id action data result} jira(fields: [\"summary\", \"key\"])}}}";

                var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");

                totalCount = (int)rsp.data.getTests.total;

                var tests = ApiHelper.GetRspList(rsp.data.getTests.results);

                foreach(var test in tests)
                {
                    //add test to full list
                    Tests.Add(new XRayTest(test));
                }
            } while (start < totalCount && totalCount < 5000);

            var a = "";
        }

        private void UpdateTestCase(string testName, XRayTest test, List<TestCaseStep> testSteps)
        {
            //check diff count
            var diff = test.StepDiff(testSteps);
            
            if(diff > EditTestThreshold)
            {
                RemoveTestFromTestRun(test.IssueId);
                MoveTestCase(test.IssueId, RetiredFolder);
                CreateTestCase(testName, testSteps);
            }
            else
            {
                var mutations = new List<string>();
                var removeItems = true;
                int index;
                for (index = 0; index < testSteps.Count; index++)
                {
                    //if testSteps is out of range, break
                    if (index >= test.Steps.Count)
                    {
                        removeItems = false;
                        break;
                    }
                    else if (!((string)test.Steps[index].action).Equals(testSteps[index].StepDescription))
                    {
                        break;
                        //if old and new steps don't match, break
                        //make sure prior steps are removed from that index on
                    }
                }
                
                if (removeItems || testSteps.Count < test.Steps.Count)
                {
                    //TODO figure out how to remove steps if any remain
                    for(int i = index; i < test.Steps.Count; i++)
                    {
                        mutations.Add($"removeTestStep(stepId: \"{(string)test.Steps[i].id}\")");
                    }
                }

                //update test steps that already exist
                for (int i = index; i < test.Steps.Count && i < testSteps.Count; i++)
                {
                    GenerateTestSteps(new List<TestCaseStep>() { testSteps[i] }, out var str);
                    mutations.Add($"updateTestStep(stepId: \"{(string)test.Steps[i].id}\" step: {str[1..^1]})");
                }

                //add new test steps
                for(int i = test.Steps.Count; i < testSteps.Count; i++)
                {
                    GenerateTestSteps(new List<TestCaseStep>() { testSteps[i] }, out var str);
                    mutations.Add($"addTestStep(issueId: \"{(string)test.IssueId}\" step: {str[1..^1]}){{id}}");
                }

                for (int i = 0; i < mutations.Count; i+= 10)
                {
                    var query = $"mutation {{ {string.Join(" ", GetMutationSubset(mutations, i))} }}";
                    var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");
                }

                //TODO build call to add new test steps

                //somehow figure out how to update the steps
                //start with finding where steps differ
                //from that point, update existing steps
                //if now fewer steps, then remove excess
                //if now more steps, then add
                //if steps are the same, then make no change
            }
        }

        private List<string> GetMutationSubset(List<string> list, int start, int max = 10)
        {
            var rtn = new List<string>();
            for(int i = start; i < (start+max) && i < list.Count; i++)
            {
                rtn.Add($"val{i}: {list[i]}");
            }

            return rtn;
        }

        private XRayTest FindTestCase(string testName, TestExecutioner te)
        {
            var testSteps = te.RecordedSteps;
            bool hasTest = Tests.Any(x => x.Name.ToLower().Trim().Equals(testName.ToLower().Trim()));

            if (hasTest)
            {
                var test = Tests.First(x => x.Name.Trim().Equals(testName.Trim()));
                
                if (!te.TestCaseFailed)
                {
                    UpdateTestCase(testName, test, testSteps);
                }
                
                return test;
            }
            else
            {
                return CreateTestCase(testName, testSteps);
            }
        }

        private XRayTest CreateTestCase(string testName, List<TestCaseStep> testSteps)
        {
            //Potentially use the 'import' instead of this as it may be much quicker
            GenerateTestSteps(testSteps, out var steps);

            var folder = string.IsNullOrEmpty(TestFolder) ? "" : "folderPath: \"" + TestFolder + "\"";

            var query = "mutation {createTest(testType: { name: \"Automated\" }steps: "
                //+"[{action: \"Create first example step\", result: \"First step was created\"},{action: \"Create second example step with data\", data: \"Data for the step\", result: \"Second step was created with data\" }]"
                + steps
                +" jira: {fields: { summary:\""+ 
                testName.Trim() + "\", project: {key: \""+Project+ "\"} }}"+folder+") {test {issueId testType {name} steps {id action data result} jira(fields: [\"key\", \"summary\"])} warnings}}";

            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");

            var test = new XRayTest(rsp.data.createTest.test);

            Tests.Add(test);
            return test;
        }

        private void MoveTestCase(string testId, string folder)
        {
            var query = "mutation {addTestsToFolder (projectId: \""+ProjectId+"\", path: \""+folder+"\", testIssueIds:[\""+testId+"\"]) {folder {name path}}}";
            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");
        }

        private void AddTestToTestRun(string testId)
        {
            var query = "mutation {addTestsToTestExecution(issueId: \"" + ExecutionId + "\" testIssueIds: [\""+testId+"\"]) {addedTests warning}}";

            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");
        }

        private void RemoveTestFromTestRun(string testId)
        {
            var query = "mutation {removeTestsFromTestExecution(issueId: \""+ ExecutionId + "\" testIssueIds: [\"" + testId + "\"])}";

            var rsp = ApiObj.PostCall(Endpts.Graph, new { query }, "");
        }


        private void AddTestResults(TestExecutioner te, string testKey)
        {
            var start = te.StartTime.ToString("yyyy-MM-dd'T'HH:mm:ssK");
            var end = te.DisposeTime == null ? DateTime.Now : (DateTime)te.DisposeTime;
            var finish = end.ToString("yyyy-MM-dd'T'HH:mm:ssK");

            var info = new
            {
                finishDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssK"),
            };

            //generate this from each test object we get

            var test = new
            {
                testKey,
                start,
                finish,
                comment = "",
                status = te.TestCaseFailed ? "FAILED" : "PASSED",
                steps = GenerateTestSteps(te.RecordedSteps, out _, te.TestCaseFailed)
            };

            var body = new
            {
                testExecutionKey = ExecutionKey,
                info,
                tests = new object[] {test}
            };

            var rsp = ApiObj.PostCall(Endpts.ImportResults, body, "");
            //check upload????????
        }

        private object[] GenerateTestSteps(List<TestCaseStep> steps, out string xrayStr, bool testFailed = false)
        {
            xrayStr = "[";
            List<object> xraySteps = new List<object>();
            for(int i = 0; i < steps.Count; i++)
            //foreach(var step in steps)
            {
                var step = steps[i];
                var status = step.StepPassed ? "PASSED" : "FAILED";
                if (step.ImageData != null && (!step.StepPassed || (testFailed && i == steps.Count-1)))
                {
                    status = "FAILED";
                    var rand = new RandomTestData();

                    var evidences = new object[]
                    {
                new {
                        data = Convert.ToBase64String(step.ImageData),
                        filename = $"{rand.GetRandomAlphaNumericString(50)}.png",
                        contentType = "image/png"
                    }
                    };

                    xraySteps.Add(new
                    {
                        status,
                        comment = step.Notes,
                        actualResult = step.ActualResult,
                        evidences
                    });
                }
                else
                {
                    xraySteps.Add(new
                    {
                        status,
                        comment = step.Notes,
                        actualResult = step.ActualResult
                    });
                }

                xrayStr += xraySteps.Count > 1 ? ", " : "";

                xrayStr += "{action: \"" + step.StepDescription + "\", result: \"" + step.ExpectedResult + "\"}";//, actualResult: \""+step.ActualResult+"\"}";
            }
            xrayStr += "]";
            return xraySteps.ToArray();
        }

        private string GetAuthentication()
        {
            var token = Config.GetConfigSetting("XRayToken");
            var split = Encoding.Default.GetString(Convert.FromBase64String(token)).Split(':');

            var body = new
            {
                client_id = split[0],
                client_secret = split[1]
            };

            var rsp = ApiObj.PostCall(Endpts.Authenticate, body, "");

            return (string)rsp;
        }
    }

    class XRayTest
    {
        public dynamic RawData;
        public string IssueId { get { return (string)RawData.issueId; } }
        public string IssueKey { get { return (string)RawData.jira.key; } }
        public List<dynamic> Steps;
        public string Name { get { return (string)RawData.jira.summary; } }
        public XRayTest(dynamic data)
        {
            RawData = data;
            Steps = ApiHelper.GetRspList(RawData.steps);
        }

        public int StepDiff(List<TestCaseStep> currSteps)
        {
            var lengthDiff = Steps.Count - currSteps.Count;
            var diffCount = Math.Abs(lengthDiff);

            var maxCount = lengthDiff >= 0 ? currSteps.Count : Steps.Count;

            for(int i = 0; i < maxCount; i++)
            {
                if (!currSteps[i].StepDescription.Equals((string)Steps[i].action))
                {
                    diffCount++;
                }
            }

            return diffCount;
        }
    }
}
