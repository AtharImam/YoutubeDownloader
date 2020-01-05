using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Models;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnDownload_ClickAsync(object sender, EventArgs e)
        {
            await this.DownloadVideo();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private async Task DownloadVideo()
        {
            YoutubeClient client = new YoutubeClient();

            if (txtUrl.Text.IndexOf("&list=") > 0)
            {
                string id = txtUrl.Text.Split(new string[] { "&list=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                var playlist = await client.GetPlaylistAsync(id);
                int count = playlist.Videos.Count();
                if (count > 0)
                {
                    int index = 1;
                    foreach (var vid in playlist.Videos)
                    {
                        await this.DownloadVideo(vid.Id, $"{index} / {count}");
                        index++;
                    }
                }
            }
            else
            {
                var id = YoutubeClient.ParseVideoId(txtUrl.Text);
                await this.DownloadVideo(id);
            }

            MessageBox.Show("Succeeded");
        }

        private async Task DownloadVideo(string id, string info = "")
        {
            YoutubeClient client = new YoutubeClient();

            Video video = await client.GetVideoAsync(id);
            this.Text = $"{info} Downloading Video : {video.Title}";
            // Get metadata for all streams in this video
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);

            // Select one of the streams, e.g. highest quality muxed stream
            var streamInfo = streamInfoSet.Muxed.First(item => item.VideoQuality == streamInfoSet.Muxed.Max(itm => itm.VideoQuality));

            // Get file extension based on stream's container
            var ext = streamInfo.Container.ToString();

            string filePath = video.Title + "." + ext;

            filePath = filePath.Replace("/", "");

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                filePath = filePath.Replace(c.ToString(), "");
            }

            filePath = Path.Combine(txtPath.Text, filePath);

            // Download stream to file
            await client.DownloadMediaStreamAsync(streamInfo, filePath);
        }
    }
}
