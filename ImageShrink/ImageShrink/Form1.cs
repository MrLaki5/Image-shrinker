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

        private String currPath;
        private int ShrinkDivider = 2;
        private int WorkFlag = 1;
        private int ImageNumberToShrink = -1;
        private int LeftImageNumberToShrink = -1;

        protected static Bitmap ResizeImage(Image image, int width, int height)
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

        protected void printMessage(String message, int errFlag)
        {
            if (errFlag == 1)
            {
                this.label3.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            }
            else
            {
                this.label3.ForeColor = System.Drawing.Color.FromArgb(0, 128, 0);
            }
            this.label3.Text = message;
        }


        public Form1()
        {
            InitializeComponent();
            this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.currPath = AppDomain.CurrentDomain.BaseDirectory;
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

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DirectoryInfo currDir = new DirectoryInfo(this.currPath);
            Directory.CreateDirectory(currPath + "\\Shrink");
            Directory.CreateDirectory(currPath + "\\Original");
            ImageNumberToShrink = -1;
            while (WorkFlag == 1)
            {
                FileInfo[] rgFiles = currDir.GetFiles("*.jpg").Union(currDir.GetFiles("*.jpeg")).ToArray();
                LeftImageNumberToShrink = rgFiles.Length;
                if (ImageNumberToShrink == -1)
                {
                    ImageNumberToShrink = rgFiles.Length;
                    if (ImageNumberToShrink == 0)
                    {
                        WorkFlag = 0;
                        continue;
                    }
                }
                if (rgFiles.Length > 0)
                {
                    String fullFileName = rgFiles[0].FullName;
                    String fileName = rgFiles[0].Name;
                    rgFiles[0].MoveTo(currPath + "\\Original\\" + fileName);
                    Image img = Image.FromFile(currPath + "\\Original\\" + fileName);
                    rgFiles = null;
                    int oldH = img.Height;
                    int oldW = img.Width;
                    int newH = 0;
                    int newW = 0;
                    newH = oldH / ShrinkDivider;
                    newW = oldW / ShrinkDivider;
                    if (newH != 0 && newW != 0)
                    {
                        img = ResizeImage(img, newW, newH);
                        img.Save(currPath + "\\Shrink\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    img.Dispose();
                    img = null;
                    int temp1 = ImageNumberToShrink - LeftImageNumberToShrink;
                    backgroundWorker1.ReportProgress((temp1 * 100) / ImageNumberToShrink);
                }
                else
                {
                    WorkFlag = 0;
                }      
            }
            backgroundWorker1.ReportProgress(100);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                this.progressBar1.Visible = true;
                this.progressBar1.Value = 0;
                if (Directory.Exists(this.currPath + "\\Original"))
                {
                    if (Directory.GetFiles(this.currPath + "\\Original").Length > 0)
                    {
                        this.printMessage("Original folder must\n be empty", 1);
                        return;
                    }
                }
                this.printMessage("", 0);
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
           progressBar1.Value = e.ProgressPercentage;
            if (e.ProgressPercentage == 100)
            {
                this.printMessage("Shrinking done!", 0);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                WorkFlag = 0;
            }
            base.OnFormClosing(e);
        }
    }
}
