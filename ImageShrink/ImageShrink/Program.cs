using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageShrink
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 frm = new Form1();
            frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.MinimumSize = new Size(300, 300);
            frm.MaximumSize = new Size(300, 300);
            Application.Run(frm);
        }
    }
}
