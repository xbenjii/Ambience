using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CUE.NET;
using CUE.NET.Devices.Keyboard;

namespace Corsair
{
    public partial class Main : Form
    {
        ScreenManager screen = new ScreenManager(0, 0);
        Bitmap preview = new Bitmap(500, 500);
        CUE.NET.Devices.Keyboard.CorsairKeyboard keyboard;
        public Main()
        {
            InitializeComponent();
            try
            {
                CueSDK.Initialize();
            }
            catch (CUE.NET.Exceptions.CUEException ex)
            {
                MessageBox.Show(ex.Message);
            }
            keyboard = CueSDK.KeyboardSDK;
            timer1.Interval = 150;
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            var image = screen.GetFrame(1000);
            var dominance = CalculateColor(image.NewPixels);
            using(Graphics gfx = Graphics.FromImage(preview))
            {
                using (SolidBrush brush = new SolidBrush(dominance)){
                    gfx.FillRectangle(brush, 0, 0, 500, 500);
                }
            }
            pictureBox1.Image = preview;
            CUE.NET.Devices.Keyboard.Keys.RectangleKeyGroup keyGroup = new CUE.NET.Devices.Keyboard.Keys.RectangleKeyGroup(keyboard,
                CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.Escape, CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.KeypadEnter)
            {
                Brush = new CUE.NET.Devices.Keyboard.Brushes.SolidColorBrush(dominance)
            };
            keyboard.UpdateLeds();
        }
        private Color CalculateColor(byte[] buffer)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int i = 0;
            while(i < buffer.Length)
            {
                b += buffer[i];
                g += buffer[i + 1];
                r += buffer[i + 2];
                i += 4;
            }
            int nPixels = i / 4;
            return Color.FromArgb(r / nPixels, g / nPixels, b / nPixels);
        }
    }
}
