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
using System.Text.RegularExpressions;
using NLog;
using ShellProgressBar;

namespace MSDNChannel9Downloader
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Console.WriteLine("please input Series or Tags Uri link? --example https://channel9.msdn.com/Tags/fsharp");
            string uri = Console.ReadLine();
            Console.WriteLine("Store to where path?     --example d:/foldername");
            string path = Console.ReadLine();

            DownloadAsync(uri, path).Wait();
            Console.ReadLine();
        }

        static IEnumerable<VideoPage> GetVideoPagesFromDisk(string path)
        {
            string enPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string file = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<IEnumerable<VideoPage>>(file);
        }

        static async Task DownloadVideoToDiskAsync()
        {
            ServicePointManager.DefaultConnectionLimit = 64;
            List<Task> TaskList = new List<Task>();
            var vs = GetVideoPagesFromDisk(@"C:\Users\bosxi\Videos\fsharp.json");
            foreach (var item in vs)
            {
                if (item.BestQuality != null)
                {
                    var LastTask = new Task(() => { new Downloader(item, @"F:\d\fsharp\").StartDownloadAsync(); });
                    LastTask.Start();
                    TaskList.Add(LastTask);
                }
            }

            await Task.WhenAll(TaskList.ToArray());
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
                TaskList.Add(LastTask); ;
            }

            await Task.WhenAll(TaskList.ToArray());

            series.SaveTo();
        }
        static async Task DownloadAsync(string uri, string path)
        {
            Regex r = new Regex(@"[^/]+", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
            var title = r.Match(uri).ToString();
            Console.Title = title;

            var series = await Series.GetAsync(uri);
            List<Task> TaskList = new List<Task>();
            foreach (var item in series.VideoPages)
            {
                item.Print();
                var LastTask = new Task(item.LoadMediaFileInfos);
                LastTask.Start();
                TaskList.Add(LastTask); ;
            }
            await Task.WhenAll(TaskList.ToArray());

            Console.Clear();

            List<Task> tasks = new List<Task>();
            foreach (var item in series.VideoPages)
            {
                if (item.BestQuality != null)
                {
                    Console.WriteLine($"0.00% | {item.Title}");
                    var LastTask = new Task(() => { new Downloader(item, $@"{path}/{title}/").StartDownloadAsync(); });
                    LastTask.Start();
                    tasks.Add(LastTask);
                }
            }

            await Task.WhenAll(tasks.ToArray());
        }

    }
}
