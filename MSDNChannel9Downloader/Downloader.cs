using NLog;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class Downloader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private VideoPage _videoPage;
        private ProgressBar _bar;
        public int CurrentPercent { get; set; }
        private string folder { get; set; }

        public MediaFileType? TypeToDownload { get; }

        public Downloader(VideoPage videoPage, string folder, MediaFileType? typeToDownload = null)
        {
            if (String.IsNullOrEmpty(folder))
            {
                throw new ArgumentNullException(nameof(folder));
            }
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            this.TypeToDownload = typeToDownload;
            this.folder = folder;
            //logger.Debug(videoPage.Title + "Create");
            _videoPage = videoPage;
        }

        public async Task StartDownloadAsync()
        {
            foreach (var item in _videoPage.MediaFileInfos)
            {
                if (TypeToDownload != null)
                {
                    if (item.Type == TypeToDownload)
                    {
                        await DownloadFileAsync(item);
                    }
                }
                else if (_videoPage.BestQuality != null && _videoPage.BestQuality == item)
                {
                    await DownloadFileAsync(item);
                }

            }
        }

        private async Task DownloadFileAsync(MediaFileInfo mediaFileInfo)
        {
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, e) =>
                {

                    if (_bar == null)
                    {
                        var options = new ProgressBarOptions()
                        {
                            DisplayTimeInRealTime = false,
                            CollapseWhenFinished = true,
                            ProgressBarOnBottom = false,
                            ProgressCharacter = '*',

                        };
                        _bar = new ProgressBar(100, _videoPage.Title, options: options);
                    }

                    if (CurrentPercent < e.ProgressPercentage)
                    {
                        CurrentPercent = e.ProgressPercentage;
                        _bar.Tick($"{_videoPage.Title} {e.BytesReceived / 1024 / 1024 }MB /{e.TotalBytesToReceive / 1024 / 1024}MB");
                    }

                    //Console.WriteLine($"{_videoPage.Title} {e.ProgressPercentage}% {e.BytesReceived} {e.TotalBytesToReceive}");
                };
                string enumName = Enum.GetName(typeof(MediaFileType), mediaFileInfo.Type);

                await client.DownloadFileTaskAsync(new Uri(mediaFileInfo.FileUri), $@"{folder}{_videoPage.FileName}_{enumName}.{mediaFileInfo.Suffix}");
            }
        }

    }
}
