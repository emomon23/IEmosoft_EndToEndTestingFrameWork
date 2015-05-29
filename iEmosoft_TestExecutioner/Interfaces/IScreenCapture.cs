using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace iEmosoft.Automation.Interfaces
{
    public interface IScreenCapture : IDisposable
    {
        void CaptureDesktop(string fileName, ImageFormat format = null, string textToOverlay = "");
        byte[] LastImageCapturedAsByteArray { get; }
    }
}