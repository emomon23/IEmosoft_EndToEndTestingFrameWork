using aUI.Automation.HelperObjects;
using aUI.Automation.Interfaces;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.IO;

namespace aUI.Automation.ScreenCaptures
{
    class HeadlessScreenCapture : IScreenCapture
    {
        ScreenPhotographer Photo = null;
        private Screenshot Sc = null;
        private IUIDriver Driver = null;
        private int ImageNum = 0;

        public HeadlessScreenCapture(string rootPath, IUIDriver driver)
        {
            Driver = driver;
            Photo = new ScreenPhotographer(rootPath);
        }

        public void CaptureDesktop(string fileName, string textToOverlay, bool deleteDup = true)
        {
            Sc = null;
            try
            {
                Sc = ((ITakesScreenshot)Driver.RawWebDriver).GetScreenshot();
                //File.WriteAllBytes(fileName, Sc.AsByteArray);
                //Sc.SaveAsFile(fileName, ScreenshotImageFormat.Png);
                Bitmap bmp;
                using (var ms = new MemoryStream(Sc.AsByteArray))
                {
                    bmp = new Bitmap(ms);
                }

                if (!string.IsNullOrEmpty(textToOverlay))
                {
                    AddOverlay(textToOverlay, bmp);
                }

                bmp.Save(fileName);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch(Exception e)
            {
                var a = "";
                //ignore it, do nothing
            }
        }

        private void AddOverlay(string text, Bitmap bitmap)
        {
            using Graphics graphics = Graphics.FromImage(bitmap);
            using Font arialFont = new("Arial", 16);
            try
            {
                graphics.DrawString(Driver.CurrentFormName_OrPageURL, arialFont, Brushes.Red, new PointF(25, 80));
            }
            catch { }
            
            graphics.DrawString(text, arialFont, Brushes.Red, new PointF(25, 108));
        }

        public byte[] LastImageCapturedAsByteArray
        {
            get { return Sc.AsByteArray; }
        }

        public void Dispose()
        {}

        public string NewFileName
        {
            get
            {
                return Path.Combine(Photo.RootPath, string.Format("TestImage_{0}_{1}.png", ImageNum++, DateTime.Today.ToString("MM-dd-yyyy")));
            }
        }
    }
}
