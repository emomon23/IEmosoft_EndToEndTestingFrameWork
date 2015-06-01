using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iEmosoft.Automation.HelperObjects;
using iEmosoft.Automation.Interfaces;

namespace iEmosoft.Automation.ScreenCaptures
{
    public class LocalScreenCapture : IScreenCapture
    {
        private ScreenPhotographer photographer = null;
       
        public LocalScreenCapture(string rootPath)
        {
            photographer = new ScreenPhotographer(rootPath);
        }

        public void CaptureDesktop(string fileName, System.Drawing.Imaging.ImageFormat format = null,
            string textToOverlay = "")
        {
            photographer.CaptureScreenToFile(fileName, format, textToOverlay);
        }
        
        public byte[] LastImageCapturedAsByteArray
        {
            get { return photographer.LastImageCapturedAsByteArray; }
        }

        public void Dispose()
        {}


        public string NewFileName
        {
            get
            {
                return System.IO.Path.Combine(photographer.RootPath, string.Format("TestImage_{0}_{1}.jpg", new RandomTestData().GetRandomDigits(5),
                DateTime.Now.Minute));
            }
        }
    }
}
