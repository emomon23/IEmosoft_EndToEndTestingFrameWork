using aUI.Automation.Enums;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace aUI.Automation.HelperObjects
{
    public class Api : IDisposable
    {
        //constructor would create the primary object and any needed default headers + auth tokens
        HttpClient Client;
        TestExecutioner TE;
        string ApplicationType = "application/json";

        public Api(TestExecutioner te, string baseUrl = "", string appType = "application/json")
        {
            TE = te;
            ApplicationType = appType;
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = Config.GetConfigSetting("ApiUrl", "");
            }

            Client = new();
            Client.BaseAddress = new Uri(baseUrl);
            Client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(appType));

        }

        //get
        public dynamic GetCall(Enum endpt, string query = "", int expectedCode = 200)
        {
            var ept = $"{endpt.Api()}{query}";
            var rspMsg = Client.GetAsync(ept).Result;

            TE.Assert.AreEqual(expectedCode, (int)rspMsg.StatusCode, "Verify response code matches expected");

            return rspMsg.GetRsp();
        }

        //post
        public dynamic PostCall(Enum endpt, object body, int expectedCode = 200)
        {
            //create httpContent
            var data = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, ApplicationType);
            var rspMsg = Client.PostAsync(endpt.Api(), data).Result;

            TE.Assert.AreEqual(expectedCode, (int)rspMsg.StatusCode, "Verify response code matches expected");

            return rspMsg.GetRsp();
        }

        //put
        public dynamic PutCall(Enum endpt, object body, int expectedCode = 200)
        {
            //create httpContent
            var data = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, ApplicationType);
            var rspMsg = Client.PutAsync(endpt.Api(), data).Result;

            TE.Assert.AreEqual(expectedCode, (int)rspMsg.StatusCode, "Verify response code matches expected");

            return rspMsg.GetRsp();
        }

        //delete
        public dynamic DeleteCall(Enum endpt, string query = "", int expectedCode = 200)
        {
            var ept = $"{endpt.Api()}{query}";
            var rspMsg = Client.DeleteAsync(ept).Result;

            TE.Assert.AreEqual(expectedCode, (int)rspMsg.StatusCode, "Verify response code matches expected");

            return rspMsg.GetRsp();
        }

        public void Dispose()
        {
            if(Client != null)
            {
                Client.Dispose();
            }
        }
    }

    public static class ApiHelper
    {
        public static dynamic GetRsp(this HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
