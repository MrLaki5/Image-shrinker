using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImageShrink
{
    public partial class Form1 : Form
    {

        private int ShrinkDivider = 2;

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ComboBox1_SelectedIndexChanged(object sender,
        System.EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedDividerStr = (string)comboBox1.SelectedItem;
            ShrinkDivider = int.Parse(selectedDividerStr);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.label3.Text = "";
            String currPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo currDir = new DirectoryInfo(currPath);
            Directory.CreateDirectory(currPath+"\\Shrink");
            FileInfo[] rgFiles = currDir.GetFiles("*.jpg").Union(currDir.GetFiles("*.jpeg")).ToArray();
            for (int i=0; i<rgFiles.Length; i++)
            {
                Image img = Image.FromFile(rgFiles[i].FullName);
                int oldH = img.Height;
                int oldW = img.Width;
                int newH = 0;
                int newW = 0;
                newH = oldH/ ShrinkDivider;
                newW = oldW / ShrinkDivider;            
                if (newH == 0 || newW == 0) {
                    continue;
                }
                img = ResizeImage(img, newW, newH);
                img.Save(currPath+"\\Shrink\\"+rgFiles[i].Name, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            this.label3.Text = "Shrinking done!";
        }
    }
}
