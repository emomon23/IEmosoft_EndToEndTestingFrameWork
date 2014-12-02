using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace iEmosoft.RecordableBrowser
{
    public interface IScreenCapture
    {
        IntPtr ScreenToCaptureWindowsHandle { get; set; }
        Image CaptureScreen();
        Image CaptureWindow(IntPtr windowHandle, string textToWriteOnImage);
        Image CaptureWindow(IntPtr windowHandle);
        void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format);
        void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format, string textToWriteOnImage);
        string CaptureScreenToFile(string textToWriteOnImage);
        string CaptureScreenToFile();
        void CaptureScreenToFile(string filename, ImageFormat format);
        void CaptureScreenToFile(string filename, ImageFormat format, string textToWriteOnImage);
        Image LastImageCaptured { get; }
        byte[] LastImageCapturedAsByteArray { get; }
    }
}
