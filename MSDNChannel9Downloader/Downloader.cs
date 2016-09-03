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
        private static List<ProgressBar> _bars;
        private ProgressBar bar;
        private static object syncRoot = new Object();
        public int CurrentPercent { get; set; }

        public Downloader(VideoPage videoPage)
        {

            //logger.Debug(videoPage.Title + "Create");
            _videoPage = videoPage;
        }

        public async Task StartDownloadAsync()
        {
            foreach (var item in _videoPage.MediaFileInfos)
            {
                if (_videoPage.BestQuality != null && item == _videoPage.BestQuality)
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
                    if (_bars == null)
                    {
                        _bars = new List<ProgressBar>();
                    }
                    if (bar == null)
                    {
                        var options = new ProgressBarOptions()
                        {
                            DisplayTimeInRealTime = false,
                            CollapseWhenFinished = true,
                            ProgressBarOnBottom = false,
                            ProgressCharacter = '*',
                            
                        };
                        bar = new ProgressBar(100, _videoPage.Title, options: options);
                        lock (syncRoot)
                        {
                            _bars.Add(bar);
                        }
                    }

                    if (CurrentPercent < e.ProgressPercentage)
                    {
                        CurrentPercent = e.ProgressPercentage;
                        bar.Tick($"{_videoPage.Title} {e.BytesReceived / 1024 / 1024 }MB /{e.TotalBytesToReceive / 1024 / 1024}MB");
                    }

                    //Thread.Sleep(3000);
                    //Console.WriteLine($"{_videoPage.Title} {e.ProgressPercentage}% {e.BytesReceived} {e.TotalBytesToReceive}");
                };
                string folder = $@"z:/Using-Git-with-Visual-Studio-2013/";
                string enumName = Enum.GetName(typeof(MediaFileType), mediaFileInfo.Type);
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                await client.DownloadFileTaskAsync(new Uri(mediaFileInfo.FileUri), $@"{folder}{_videoPage.FileName}_{enumName}.{mediaFileInfo.Suffix}");
            }
        }

    }
}
