using System;

namespace aUI.Automation.Interfaces
{
    public interface IScreenCapture : IDisposable
    {
        void CaptureDesktop(string fileName, string textToOverlay, bool deleteDup = true);
        byte[] LastImageCapturedAsByteArray { get; }
        string NewFileName { get; }

    }
}