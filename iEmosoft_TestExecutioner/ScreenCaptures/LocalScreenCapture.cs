using aUI.Automation.HelperObjects;
using aUI.Automation.Interfaces;
using System;

namespace aUI.Automation.ScreenCaptures
{
    public class LocalScreenCapture : IScreenCapture
    {
        private ScreenPhotographer Photographer = null;

        public LocalScreenCapture(string rootPath)
        {
            Photographer = new ScreenPhotographer(rootPath);
        }

        public void CaptureDesktop(string fileName, string textToOverlay, bool deleteDup = true)
        //string fileName, System.Drawing.Imaging.ImageFormat format = null, string textToOverlay = "")
        {
            Photographer.CaptureScreenToFile(fileName, System.Drawing.Imaging.ImageFormat.Png, textToOverlay);
        }

        public byte[] LastImageCapturedAsByteArray
        {
            get { return Photographer.LastImageCapturedAsByteArray; }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public string NewFileName
        {
            get
            {
                return System.IO.Path.Combine(Photographer.RootPath, string.Format("TestImage_{0}_{1}.png", new RandomTestData().GetRandomDigits(5),
                DateTime.Now.Minute));
            }
        }
    }
}
