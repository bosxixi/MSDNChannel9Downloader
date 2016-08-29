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
            var client = new HttpClient();
            Series series = new Series("https://channel9.msdn.com/Tags/fsharp", client);
            Task task = LoadVideoPagesToDiskAsync(series);
            task.Wait();
            series.SaveTo();
            Console.WriteLine("File Saved!");

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

        static IEnumerable<VideoPage> GetVideoPagesFromDisk()
        {
            string enPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string path = Path.Combine(enPath, "fsharp.json");
            string file = File.ReadAllText(path);
        
            return JsonConvert.DeserializeObject<IEnumerable<VideoPage>>(file);
        }

        static async Task LoadVideoPagesToDiskAsync(Series series)
        {
            List<Task> TaskList = new List<Task>();

            await series.LoadVideoPages();

            foreach (var item in series.VideoPages)
            {
                item.Print();
                var LastTask = new Task(item.LoadMediaFileInfos);
                LastTask.Start();
                TaskList.Add(LastTask);;
            }
            await Task.WhenAll(TaskList.ToArray());
            
        }



    }
}
