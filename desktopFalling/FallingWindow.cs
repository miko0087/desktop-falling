using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace desktopFalling
{
    public partial class FallingWindow : Form
    {
        Image image;
        Size size;
        Point location;
        PointF[] DestinationPoins = {
            new PointF(0, 0),   // destination for upper-left point of
                                // original
            new PointF(200, 0),  // destination for upper-right point of
                                    // original
            new PointF(0, 200)    // destination for lower-left point of
            
        };

        decimal screenScale;
        
        List<Bitmap> RenderedImages = new List<Bitmap>();
        public FallingWindow(Size size, Bitmap image, Point location, decimal screenScale)
        {
            this.screenScale = screenScale;
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.location = location;
            this.Location = Location;
            this.Size = size;
            this.image = image;
            this.size = size;
            DestinationPoins[1] = new Point(size.Width, 0);
            DestinationPoins[2] = new Point(0, size.Height);
            this.DoubleBuffered = true;
            
            float gravity = 0;
            float smooth = 5f;
            for (int i = 0; i < 50; i++)
            {
                
                gravity += 8f / smooth;
                Bitmap RenderImage = new Bitmap(size.Width, size.Height);

                

                

                using (var graphics = Graphics.FromImage(RenderImage))
                {
                    
                    graphics.DrawImage(image, DestinationPoins);
                }
                
                RenderedImages.Add(RenderImage);
                DestinationPoins[0] = new PointF(DestinationPoins[0].X + (30f + gravity) / smooth, DestinationPoins[0].Y + gravity);
                DestinationPoins[1] = new PointF(DestinationPoins[1].X + (10f + gravity) / smooth, DestinationPoins[1].Y + gravity);
            }
        }
        bool doneDrawing = true;
        int currentImage = 0;
        private void onPaint(object sender, PaintEventArgs e)
        {
            
            doneDrawing = false;
            this.Location = location;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            /*e.Graphics.DrawImage(
                image,
                new Rectangle(0, 0, (int)this.size.Width, (int)this.size.Height),
                                    // destination rectangle
                0,
                0,                  // upper-left corner of source rectangle
                image.Width,        // width of source rectangle
                image.Height,       // height of source rectangle
                GraphicsUnit.Pixel
            );
            */
            
            e.Graphics.DrawImage(RenderedImages[currentImage], 0, 0);
            

            
            doneDrawing = true;
        }

        private void onLoaded(object sender, EventArgs e)
        {
            MoveWindowToBack();
        }

        private void keydeown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F7)
            {
                Application.Exit();
            }
        }
        bool hasStarted = false;
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        private void MoveWindowToBack()
        {
            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }
        private const int WM_ACTIVATE = 0x0006;
        private const int WA_ACTIVE = 1;
        private const int WA_CLICKACTIVE = 2;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            const int GWL_EXSTYLE = -20;
            const int WS_EX_NOACTIVATE = 0x08000000;

            // Get the current window style
            int currentStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);

            // Add the WS_EX_NOACTIVATE style to the window style
            SetWindowLong(this.Handle, GWL_EXSTYLE, currentStyle | WS_EX_NOACTIVATE);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Intercept the activation message and prevent focus
            if (m.Msg == WM_ACTIVATE)
            {
                int activationType = (int)m.WParam & 0xFFFF;
                if (activationType == WA_ACTIVE || activationType == WA_CLICKACTIVE)
                {
                    m.Result = IntPtr.Zero;
                }
            }
        }
        private void tick(object sender, EventArgs e)
        {
            
            if (hasStarted)
            {
                
                currentImage++;
                if (currentImage > 50)
                    currentImage = 50;
                if (currentImage > 49)
                {

                    currentImage = 49;
                }
                if (currentImage > 2)
                {
                    RenderedImages[currentImage - 2].Dispose();
                }
                if (doneDrawing)
                {
                    this.Refresh();
                }
            }
        }

        private void onClick(object sender, EventArgs e)
        {
            
            this.hasStarted = true;
            
        }
    }
}
