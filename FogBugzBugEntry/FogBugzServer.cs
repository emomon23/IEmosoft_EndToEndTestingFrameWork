using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Net;
using iEmosoft.RecordableBrowser;

namespace iEmosoft.FogBugzBugEntry
{
    public class FogBugzServer : IDisposable
    {
        private string token;
        private string baseURL;

        private string rawWebRequestContent;
        private XmlDocument webRequestDoc;
        
        public FogBugzServer(string baseURL, string emailAdress, string password)
        {
            this.baseURL = baseURL;

            string url = string.Format("{0}/api.asp?cmd=logon&email={1}&password={2}", baseURL, emailAdress, password);
            this.GetWebRequestContent(url);

            this.token = this.GetNodeValue("response/token");
        }

        public IList<FBug> Search(string  statusFilter, string titleFilter)
        {
            List<FBug> resultList = new List<FBug>();

            string url = string.Format("{0}/api.asp?cmd=search&token={1}&cols=sTitle,sStatus,sProject,sCategory", this.baseURL, this.token);
            if (! titleFilter.IsNull())
            {
                //Fogbugz doesn't like dashes in their query filters?
                url += "&q=" + titleFilter.Replace("-", "");
            }

            this.GetWebRequestContent(url);

            XmlNodeList casesList = webRequestDoc.SelectNodes("response/cases/case");
            foreach (XmlNode caseNode in casesList)
            {
                FBug bug = new FBug()
                {
                    Id =  int.Parse(caseNode.Attributes["ixBug"].Value),
                    Status = caseNode.SelectSingleNode("sStatus").InnerText,
                    Title = caseNode.SelectSingleNode("sTitle").InnerText,
                    Project = caseNode.SelectSingleNode("sProject").InnerText,
                    Category = caseNode.SelectSingleNode("sCategory").InnerText
                };

                if (statusFilter.IsNull() || statusFilter == bug.Status)
                {
                    resultList.Add(bug);
                }
            }

            return resultList;
        }

        public FBug CreateBug(FBug bugToSave)
        {
            string url = string.Format("{0}/api.asp?cmd=new&token={1}&sTitle={2}&sEvent={3}&sProject={4}&sCategory={5}", this.baseURL, this.token, bugToSave.Title, bugToSave.Description, bugToSave.Project, bugToSave.Category);

            this.GetWebRequestContent(url);
            bugToSave.Id = int.Parse(this.webRequestDoc.SelectSingleNode("response/case").Attributes["ixBug"].Value);
            return bugToSave;
        }

        public void LogOff()
        {
            string url = string.Format("{0}/api.asp?cmd=logoff&token={1}", this.baseURL, this.token);
            this.GetWebRequestContent(url);
        }

        private void GetWebRequestContent(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            rawWebRequestContent = null;

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                try
                {
                    this.rawWebRequestContent = reader.ReadToEnd();
                    webRequestDoc = new XmlDocument();
                    webRequestDoc.LoadXml(rawWebRequestContent);

                    
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error in GetWebRequestContent for url '{0}'.  {1}", url, ex.Message), ex);
                }
            }
        }

        private string GetNodeValue(string nodePath)
        {
            return this.webRequestDoc.SelectSingleNode(nodePath).InnerText;
        }

        private XmlNode GetNode(string nodePath)
        {
            return this.webRequestDoc.SelectSingleNode(nodePath);
        }

        public void Dispose()
        {
            try
            {
                this.LogOff();
            }
            catch { }
        }
    }
}
