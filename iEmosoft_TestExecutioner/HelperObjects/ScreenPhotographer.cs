using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace iEmosoft.Automation.HelperObjects
{
    public class ScreenPhotographer 
    {
        private string baseRootFolder = "";
        private Image lastImageCaptured = null;
        private IntPtr screenToCapture = IntPtr.Zero;

        public IntPtr ScreenToCaptureWindowsHandle
        {
            get
            {
                return this.screenToCapture;
            }
            set
            {
                this.screenToCapture = value;
            }
        }

        public string RootPath
        {
            get
            {
                return baseRootFolder;
            }
        }
        
        public ScreenPhotographer() { }
             
        public ScreenPhotographer(string rootFolder)
        {
            this.baseRootFolder = rootFolder;
            if (!Directory.Exists(rootFolder))
            {
                try
                {
                    Directory.CreateDirectory(rootFolder);
                }
                catch
                {
                    throw new Exception(string.Format("Unable to create directory '{0}'", rootFolder));
                }
            }
        }
       
        public Image CaptureScreen(string textToWriteOnImage = "")
        {
            if (this.screenToCapture == IntPtr.Zero)
                this.screenToCapture = User32.GetDesktopWindow();
          
            Image img = CaptureWindow(this.screenToCapture);

            if (!textToWriteOnImage.isNull())
            {
                WriteTextOnImage(img, textToWriteOnImage);
            }

            return img;
        }
        
        public void CaptureScreenToFile(string filename, ImageFormat format = null, string textToWriteOnImage = "")
        {
            Image img = CaptureScreen();
            
            if (! textToWriteOnImage.IsNull())
            {
                this.WriteTextOnImage(img, textToWriteOnImage);
            }

            if (format == null)
            {
                format = ImageFormat.Jpeg;
            }

            img.Save(filename, format);

            this.lastImageCaptured = img;
        }
        
        public Image LastImageCaptured { get { return this.lastImageCaptured; } }

        public byte[] LastImageCapturedAsByteArray
        {
            get
            {
                byte [] result;
                using (MemoryStream ms = new MemoryStream())
                {
                    this.lastImageCaptured.Save(ms, ImageFormat.Png);
                    result = ms.ToArray();
                }

                return result;
            }
        }

        private void WriteTextOnImage(Image image, string textToWrite)
        {
            if (string.IsNullOrEmpty(textToWrite))
                return;

            PointF textLocation = new PointF(200, 200);

            using (Graphics graphics = Graphics.FromImage(image))
            {
                using (var arialFont = new Font("Arial", 14))
                {
                    graphics.DrawString(textToWrite, arialFont, Brushes.Red, textLocation);
                }
            }
        }

        public Image CaptureWindow(IntPtr windowHandle, string textToWriteOnImage = "")
        {
            IntPtr handle = windowHandle;

            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);


            this.lastImageCaptured = img;

            if (!textToWriteOnImage.isNull())
            {
                this.WriteTextOnImage(img, textToWriteOnImage);
            }

            return img;
        }

        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }

        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format, string textToWriteOnImage)
        {
            Image img = CaptureWindow(handle);
            this.WriteTextOnImage(img, textToWriteOnImage);
            img.Save(filename, format);
        }
        
        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}
