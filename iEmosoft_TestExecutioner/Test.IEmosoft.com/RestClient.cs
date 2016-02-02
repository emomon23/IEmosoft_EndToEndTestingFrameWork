using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace iEmosoft.Automation.Test.IEmosoft.com
{
    public class RestClient
    {
        public void RegisterTest(string testNumber, string testFamily, string testName, string testDescription)
        {
            var baseURL = System.Configuration.ConfigurationManager.AppSettings["Test.IEmosoft.com_BaseURL"];
            var applicationUnderTest = System.Configuration.ConfigurationManager.AppSettings["ApplicationIdUnderTest"];

            RegistationTestDTO dto = new RegistationTestDTO() { ApplicationId = Guid.Parse(applicationUnderTest), TestDescription = testDescription, TestFamily = testFamily, TestName = testName, TestNumber = testNumber };

            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.PostAsJsonAsync("api/Tests/RegisterTestUnderDevelopment", dto).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Get the URI of the created resource.
                    Uri gizmoUrl = response.Headers.Location;
                }
            }
        }
    }

    public class RegistationTestDTO
    {
        public Guid ApplicationId { get; set; }

        public string TestNumber { get; set; }

        public string TestFamily { get; set; }

        public string TestName { get; set; }

        public string TestDescription { get; set; }
    }
}
