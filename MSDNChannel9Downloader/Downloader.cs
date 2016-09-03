using Konsole;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class Downloader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static VideoPage _videoPage;
        private ProgressBar bar;
        public Downloader(VideoPage videoPage)
        {

            logger.Debug(videoPage.Title + "Create");
            _videoPage = videoPage;
        }

        public async Task StartDownloadAsync()
        {
            foreach (var item in _videoPage.MediaFileInfos)
            {
                if (_videoPage.BestQualityType != null && item.Type == _videoPage.BestQualityType)
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
                    if (bar == null)
                    {
                        if (int.MaxValue < e.TotalBytesToReceive)
                        {
                            throw new IndexOutOfRangeException();
                        }
                        logger.Debug("ProgressBar create");
                        bar = new ProgressBar((int)e.TotalBytesToReceive);
                    }
                    bar.Refresh((int)e.BytesReceived, _videoPage.Title);
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
