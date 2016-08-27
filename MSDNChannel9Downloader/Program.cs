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

namespace MSDNChannel9Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
            Console.ReadLine();
        }


        static async Task MainAsync()
        {
            var client = new HttpClient();
            Series series = new Series("https://channel9.msdn.com/Series/aspnetmonsters", client);
            await series.LoadVideoPages();

            foreach (var item in series.VideoPages)
            {
                item.Print();
                item.LoadMediaFileInfos();
            }

            series.SaveTo();
        }
      

      
    }
}
