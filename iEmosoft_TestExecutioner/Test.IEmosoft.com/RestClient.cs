using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace aUI.Automation.Test.IEmosoft.com
{
    public class RestClient
    {
        private string BaseURL = "";
        private string ApplicationUnderTest = "";

        public RestClient()
        {
            BaseURL = System.Configuration.ConfigurationManager.AppSettings["Test.IEmosoft.com_BaseURL"];
            ApplicationUnderTest = System.Configuration.ConfigurationManager.AppSettings["ApplicationIdUnderTest"];

        }
        public void RegisterTest(string testNumber, string testFamily, string testName, string testDescription, DateTime? eta)
        {

            var dto = new RegistationTestDTO() { ApplicationId = ApplicationUnderTest, TestDescription = testDescription, TestFamily = testFamily, TestName = testName, TestNumber = testNumber, ETA = eta };

            using var client = CreateHttpClient();
            var response = client.PostAsJsonAsync("api/Tests/RegisterTestUnderDevelopment", dto).Result;
            if (response.IsSuccessStatusCode)
            {

            }
        }

        public void RecordTestRun(TestRunDTO dto)
        {
            string url = "api/Tests/RecordTestRun";
            dto.StripOfImagePathString = System.Configuration.ConfigurationManager.AppSettings["TestReportFilePath"];

            using var client = CreateHttpClient();
            var response = client.PostAsJsonAsync(url, dto).Result;
            if (response.IsSuccessStatusCode)
            {
                //removed as this utilizes a retured nuget reference. Update for ftp to work again
                //                    var t = response.Content.ReadAsAsync(typeof(PostResultDTO)).Result;
                //                    PostResultDTO result = (PostResultDTO)t;

                //                    if (!result.Successful)
                //                    {
                //                        throw new Exception(result.Payload + ".  " + result.ErrorMessage);
                //                    }
            }

        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseURL)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }

    public class RegistationTestDTO
    {
        public string ApplicationId { get; set; }

        public string TestNumber { get; set; }

        public string TestFamily { get; set; }

        public string TestName { get; set; }

        public string TestDescription { get; set; }

        public DateTime? ETA { get; set; }
    }

    public class TestRunDTO
    {
        public TestRunDTO()
        {
            RunId = RunIdGenerator.RunId;
        }

        public enum TestRunStatusEnumeration
        {
            Passed = 0,
            Failed = 1,
            FailedPrereqs = 2,
        }


        public string ApplicationId { get; set; }
        public string TestNumber { get; set; }
        public int Status { get; set; }
        public string FTPPath { get; set; }
        public DateTime RunDate { get; set; }
        public TimeSpan TestTime { get; set; }
        public string TestResultString { get; set; }
        public string RunId { get; set; }

        public string StripOfImagePathString { get; set; }
    }

    public class PostResultDTO
    {
        public bool Successful { get; set; }
        public object Payload { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
    }

    public static class RunIdGenerator
    {
        private static string runId;

        public static string RunId
        {
            get
            {
                if (string.IsNullOrEmpty(runId))
                {
                    runId = Guid.NewGuid().ToString();
                }

                return runId;
            }
        }
    }
}
