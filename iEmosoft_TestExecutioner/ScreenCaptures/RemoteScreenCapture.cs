using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Interfaces;

namespace iEmosoft.Automation.ScreenCaptures
{
    public class RemoteScreenCapture : IScreenCapture
    {
        private string remoteURL = "";
        private string tempRootPath = "";
        private ScreenPhotographer photographer = null;
        private List<string> filesToUpload = new List<string>();

        public RemoteScreenCapture(string remoteURL)
        {
            this.remoteURL = remoteURL;

            RandomTestData data = new RandomTestData();
            this.tempRootPath = string.Format(@"C:\EmoAutomationTempDirectory_{0}_{1}_{2}\", data.GetRandomDigits(5),
                data.GetRandomFirstName(), data.GetRandomState());

            photographer = new ScreenPhotographer(tempRootPath);

        }

        public void CaptureDesktop(string fileName, System.Drawing.Imaging.ImageFormat format = null,
            string textToOverlay = "")
        {
            photographer.CaptureScreenToFile(fileName, format, textToOverlay);
            this.filesToUpload.Add(tempRootPath + fileName);
        }

        public byte[] LastImageCapturedAsByteArray
        {
            get { return photographer.LastImageCapturedAsByteArray; }
        }

        public void Dispose()
        {
            foreach (string file in this.filesToUpload)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.UploadFile(remoteURL, file);
                    }
                }
                catch
                {
                }
            }

            try
            {
                Directory.Delete(tempRootPath);
            }
            catch
            {
            }
        }


        public string NewFileName
        {
            get { return "Dosnt really matter"; }
        }

        void IScreenCapture.CaptureDesktop(string fileName, System.Drawing.Imaging.ImageFormat format, string textToOverlay)
        {
            throw new NotImplementedException();
        }

        byte[] IScreenCapture.LastImageCapturedAsByteArray
        {
            get { throw new NotImplementedException(); }
        }

        string IScreenCapture.NewFileName
        {
            get { throw new NotImplementedException(); }
        }
               
        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
