
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace desktopFalling
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Hide();
        }
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);

            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            System.Threading.Thread.Sleep(100);

            this.ShowInTaskbar = false;
            for(int i=0; i<Screen.AllScreens.Length; i++)
            {
                var screen = Screen.AllScreens[i];
                Bitmap result = ScreenCapture.CaptureScreen(i);
                decimal screenScale = ScreenCapture.GetScreenScale(i);
                var window = new FallingWindow(new Size(screen.Bounds.Width, screen.WorkingArea.Height),result, screen.WorkingArea.Location, screenScale);
                
                
                
                
                
                window.Show();
                
            }

            //
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
        }
        
        
        
        private void tick(object sender, EventArgs e)
        {
            
            
        }
    }
}
