using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Models;
using System.IO;

namespace YoutubeDownloader
{
    public partial class ProgressControl : UserControl
    {
        string downloadedPath;
      
        string downloadedUrl;

        bool downloadingVideo = true;
        public ProgressControl()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.downloadingVideo = false;
            this.Dispose();
        }

        public async Task StartDownloadAsync(string videoUrl, string videoPath)
        {
            this.downloadedPath = videoPath;
            this.lblPath.Text = videoPath;
            this.downloadedUrl = videoUrl;
            await this.DownloadVideo();
        }

        private async Task DownloadVideo()
        {
            YoutubeClient client = new YoutubeClient();

            if (downloadedUrl.IndexOf("&list=") > 0)
            {
                string id = downloadedUrl.Split(new string[] { "&list=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                var playlist = await client.GetPlaylistAsync(id);
                int count = playlist.Videos.Count();
                this.progressBar1.Step = 1;
                this.progressBar1.Maximum = count;
                Func<int, int, string> appendZero = (fileIndex, totalVideos) =>
                {
                    string append = string.Empty;
                    if (fileIndex < 10)
                    {
                        if (totalVideos > 100)
                        {
                            append = "00";
                        }
                        else
                        {
                            append = "0";
                        }
                    }
                    else if (fileIndex < 100)
                    {
                        if (totalVideos > 100)
                        {
                            append = "0";
                        }
                    }

                    return append;
                };

                if (count > 0)
                {
                    int index = 0;
                    lblVideoCount.Text = index + "/" + count;
                    foreach (var vid in playlist.Videos)
                    {
                        string zero = appendZero(index + 1, count) + (index+1);
                        await this.DownloadVideo(vid.Id, "Part" + zero);
                        index++;
                        if (!downloadingVideo)
                        {
                            break;
                        }

                        this.progressBar1.PerformStep();
                        lblProgress.Text = Convert.ToInt32(this.progressBar1.Value * 100 / count) + "%";
                        lblVideoCount.Text = index + "/" + count;
                    }

                }
            }
            else
            {
                var id = YoutubeClient.ParseVideoId(downloadedUrl);
                await this.DownloadVideo(id);
            }

            if (downloadingVideo)
            {
                MessageBox.Show("Succeeded");
            }
        }

        private async Task DownloadVideo(string id, string info = "")
        {
            try
            {
                YoutubeClient client = new YoutubeClient();

                Video video = await client.GetVideoAsync(id);
                this.lblVideo.Text = video.Title;
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

                string oldFileName = Path.Combine(downloadedPath, filePath);
                string indexFileName = Path.Combine(downloadedPath, info + "_" + filePath);
                if(File.Exists(oldFileName))
                {
                    File.Move(oldFileName, indexFileName);
                    return;
                }

                picBox.Load(video.Thumbnails.LowResUrl);
                // Download stream to file
                await client.DownloadMediaStreamAsync(streamInfo, indexFileName);
            }
            catch
            {

            }
        }
    }
}
