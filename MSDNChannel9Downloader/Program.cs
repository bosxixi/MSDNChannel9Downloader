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
using Konsole;

namespace MSDNChannel9Downloader
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            #region ProgressBar
            //var dirs = Directory.GetDirectories(@"F:\Learning").Where(d => Directory.GetFiles(d).Count() > 0).Take(10);

            //var tasks = new List<Task>();
            //var bars = new List<ProgressBar>();
            //foreach (var d in dirs)
            //{
            //    var dir = new DirectoryInfo(d);
            //    var files = dir.GetFiles().Take(100).Select(f => f.FullName).ToArray();
            //    if (files.Count() == 0) continue;
            //    var bar = new ProgressBar(files.Count());
            //    bars.Add(bar);
            //    bar.Refresh(0, d);
            //    tasks.Add(new Task(() => ProcessFiles(d, files, bar)));
            //}
            //Console.WriteLine("ready press enter.");
            //Console.ReadLine();

            //foreach (var t in tasks) t.Start();
            //Task.WaitAll(tasks.ToArray());
            //Console.WriteLine("done.");
            //Console.ReadLine();

            #endregion

            DownloadVideoToDiskAsync().Wait();


            //LoadVideoPagesToDiskAsync("https://channel9.msdn.com/Series/Using-Git-with-Visual-Studio-2013").Wait();
            //http://video.ch9.ms/ch9/d284/76aa3635-9cbe-4737-93c2-09aff655d284/Config_mid.mp4

            //DownloadFile("sdfsf.mp4", @"z:/", "http://video.ch9.ms/ch9/d284/76aa3635-9cbe-4737-93c2-09aff655d284/Config_mid.mp4");
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
        public static void ProcessFiles(string directory, string[] files, ProgressBar bar)
        {
            var cnt = files.Count();
            foreach (var file in files)
            {
                bar.Next(new FileInfo(file).Name);
                Thread.Sleep(133);
            }
        }



        static IEnumerable<VideoPage> GetVideoPagesFromDisk(string path)
        {
            string enPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            //string path = Path.Combine(enPath, "fsharp.json");
            string file = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<IEnumerable<VideoPage>>(file);
        }

        static async Task DownloadVideoToDiskAsync()
        {
            ServicePointManager.DefaultConnectionLimit = 64;
            List<Task> TaskList = new List<Task>();
            foreach (var item in GetVideoPagesFromDisk(@"C:\Users\bosxi\Videos\Using-Git-with-Visual-Studio-2013.json"))
            {
                if (item.BestQuality != null)
                {
                    var LastTask = new Task(() => { new Downloader(item).StartDownloadAsync(); });
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


    }
}
