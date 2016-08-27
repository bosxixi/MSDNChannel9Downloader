﻿using System;
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
            Series series = new Series("https://channel9.msdn.com/Series/aspnetmonsters?page=1", client);
            var videoLinks = await series.GetVideoLinksAsync();
            foreach (var item in videoLinks)
            {
                item.Print();
                item.LoadMediaFileInfos();
            }
            series.SaveTo();
        }
      

      
    }
}
