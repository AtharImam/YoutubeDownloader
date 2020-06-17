using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using System.IO;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using System.Diagnostics;

namespace YoutubeDownloader
{
    public partial class ProgressControl : UserControl, IProgress<double>
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

        bool isList = false;

        private async Task DownloadVideo()
        {
            YoutubeClient client = new YoutubeClient();

            if (downloadedUrl.IndexOf("&list=") > 0)
            {
                isList = true;
                string id = downloadedUrl.Split(new string[] { "&list=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                var playlist = await client.Playlists.GetVideosAsync(id);
                int count = playlist.Count();
                this.progressBar1.Maximum = count;
                this.progressBar2.Visible = true;
                
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
                    foreach (var vid in playlist)
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
                        this.progressBar2.Value = 0;
                    }

                }
            }
            else
            {
                this.progressBar2.Visible = false;
                this.progressBar1.Maximum = 100;
                var id = new VideoId(downloadedUrl);
                downloadingVideo = await this.DownloadVideo(id);
            }

            if (downloadingVideo)
            {
                MessageBox.Show("Succeeded");
            }
        }

        private async Task<bool> DownloadVideo(VideoId videoId, string info = "")
        {
            try
            {
                YoutubeClient client = new YoutubeClient();

                Video video = await client.Videos.GetAsync(videoId);
                this.lblVideo.Text = video.Title;

                var streams = await client.Videos.Streams.GetManifestAsync(videoId);
                var streamInfoSet = streams.GetMuxed().WithHighestVideoQuality();

                string filePath = video.Title + "." + streamInfoSet.Container.Name;

                filePath = filePath.Replace("/", "");

                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                foreach (char c in invalid)
                {
                    filePath = filePath.Replace(c.ToString(), "");
                }

                string oldFileName = Path.Combine(downloadedPath, filePath);
                string indexFileName = Path.Combine(downloadedPath, (string.IsNullOrEmpty(info) ? "" : (info + "_")) + filePath);
                if (File.Exists(oldFileName))
                {
                    File.Move(oldFileName, indexFileName);
                    return true;
                }

                picBox.Load(video.Thumbnails.LowResUrl);

                await client.Videos.Streams.DownloadAsync(streamInfoSet, indexFileName, this);
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void Report(double value)
        {
            var progressBar = this.isList ? this.progressBar2 : this.progressBar1;

            progressBar.Value = Convert.ToInt32(value * 100);

            if (!this.isList)
            {
                lblProgress.Text = Convert.ToInt32(this.progressBar1.Value) + "%";
            }
        }
    }
}
