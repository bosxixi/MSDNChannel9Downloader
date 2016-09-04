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
using bosxixi.Extensions;

namespace MSDNChannel9Downloader
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            //LoadVideoPagesToDiskAsync("https://channel9.msdn.com/Tags/xamarin").GetAwaiter().GetResult();
            //Console.WriteLine("complete");

            //foreach (var item in GetVideoPagesFromDisk(@"C:\Users\bosxi\Videos\xamarin.json"))
            //{
            //    if (item.BestQuality != null)
            //    {
            //        Console.WriteLine(item.BestQuality.FileUri);
            //    }

            //}
            FileNameChanger(GetVideoPagesFromDisk(@"C:\Users\bosxi\Videos\xamarin.json"), @"C:\Users\bosxi\Downloads\xamarin");



            //Console.WriteLine("please input Series or Tags Uri link? --example https://channel9.msdn.com/Tags/fsharp");
            //string uri = Console.ReadLine();
            //Console.WriteLine("Store to where path?     --example d:/foldername");
            //string path = Console.ReadLine();

            //DownloadAsync(uri, path).Wait();
            Console.ReadLine();
        }

        static void FileNameChanger(IEnumerable<VideoPage> videoPages, string path)
        {
            var files = videoPages.Where(c => c.BestQuality != null)
                .Select(b => new KeyValuePair<string, MediaFileInfo>(b.Title.GetValidFileName(), b.BestQuality));
            files.Foreach(c => {
                var oldFileName = Path.Combine(path, c.Value.FileName);
                var fileTypeName = Enum.GetName(typeof(MediaFileType), c.Value.Type);
                var newFileName = Path.Combine(path, $"{c.Key.GetValidFileName()}.{c.Value.Suffix}");
                //var newFileName = Path.Combine(path, $"{c.Key.GetValidFileName()}_{fileTypeName}.{c.Value.Suffix}");
                Console.WriteLine($"{new FileInfo(oldFileName).Name} -> {new FileInfo(newFileName).Name}");
            });

            prompt:
            Console.WriteLine("Are you sure to rename all files?[y/n]");
            string input = Console.ReadLine();
            if (input == "y")
            {
                files.Foreach(c => {
                    var oldFileName = Path.Combine(path, c.Value.FileName);
                    var fileTypeName = Enum.GetName(typeof(MediaFileType), c.Value.Type);
                    var newFileName = Path.Combine(path, $"{c.Key.GetValidFileName()}_{fileTypeName}.{c.Value.Suffix}");
                    File.Move(oldFileName, newFileName);
                });
            }
            else
            {
                goto prompt;
            }
           
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
