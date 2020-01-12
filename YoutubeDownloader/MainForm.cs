using System;
using System.Threading;
using System.Windows.Forms;

namespace YoutubeDownloader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnInstance_ClickAsync(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ProgressControl prgCtrl = new ProgressControl();
                prgCtrl.Dock = DockStyle.Top;
                this.panel1.Controls.Add(prgCtrl);
                string url = frm.DownloadedUrl;
                string path = frm.DownloadPath;
                await prgCtrl.StartDownloadAsync(url, path);
            }
        }
    }
}
