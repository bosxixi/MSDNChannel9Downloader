using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using HapCss;

namespace MSDNChannel9Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            //MainAsync().Wait();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument dom = web.Load("https://channel9.msdn.com/Series/aspnetmonsters");
            var videos = dom.DocumentNode.ChildNodes.QuerySelectorAll("ul.entries li a.title").Select(c => 
            new Video
            {
                Title = c.InnerText.Replace("&#160;", " "),
                Url = c.Attributes.First(a => a.Name == "href").Value
            });

            foreach (var item in videos)
            {
                item.ToString();
            }
        }

        static async Task MainAsync()
        {
            //HttpClient client = new HttpClient();
            //string result = await client.GetStringAsync("https://channel9.msdn.com/Series/aspnetmonsters");

            //Console.WriteLine(result);
            //HtmlAgilityPack.HtmlDocument dom = new HtmlDocument();

        }

        class Video
        {
            public string Url { get; set; }

            public string Title { get; set; }

            public override string ToString()
            {
                Console.WriteLine($"Title: {Title}");
                Console.WriteLine($"Url: {Url}");
                Console.WriteLine();
                return string.Empty;
            }
        }
    }
}
