using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using HapCss;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace MSDNChannel9Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadVideoPagesToDiskAsync("https://channel9.msdn.com/Series/aspnetmonsters").Wait();

            //var vps = GetVideoPagesFromDisk();
            //foreach (var item in vps)
            //{
            //    if (item.BestQuality != null)
            //    {
            //        Console.WriteLine(item.BestQuality.FileUri);
            //    }
            //}
            Console.ReadLine();
        }

        static async Task DownloadFile(string fileName, string path, string uri)
        {
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, e) =>
                {
                    Console.WriteLine($"{fileName} {e.ProgressPercentage}% {e.BytesReceived / 1024 / 1024}MB {e.TotalBytesToReceive / 1024 / 1024}MB");
                };
                client.DownloadFileCompleted += (sender, e) =>
                {
                    Console.WriteLine($"File Downloaded.");
                };
                await client.DownloadFileTaskAsync(uri, @"z:/video.mp4");
            }
        }

        static IEnumerable<VideoPage> GetVideoPagesFromDisk()
        {
            string enPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string path = Path.Combine(enPath, "Using-Git-with-Visual-Studio-2013.json");
            string file = File.ReadAllText(path);
        
            return JsonConvert.DeserializeObject<IEnumerable<VideoPage>>(file);
        }

        static async Task LoadVideoPagesToDiskAsync(string uri)
        {
            var series = await Series.GetAsync(uri);
            List<Task> TaskList = new List<Task>();
            foreach (var item in series.VideoPages)
            {
                item.Print();
                var LastTask = new Task(item.LoadMediaFileInfos);
                LastTask.Start();
                TaskList.Add(LastTask);;
            }

            await Task.WhenAll(TaskList.ToArray());

            series.SaveTo();
        }

    
    }
}
