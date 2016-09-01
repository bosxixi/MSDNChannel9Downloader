using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading;

namespace MSDNChannel9Downloader
{

    public class Series
    {
        private Series(string seriesLink)
        {
            if (String.IsNullOrEmpty(seriesLink))
            {
                throw new ArgumentNullException(nameof(seriesLink));
            }
            if (seriesLink.Contains("?"))
            {
                throw new ArgumentException("can not contain paramater.");
            }
            Uri uri = new Uri(seriesLink);
            Name = uri.AbsolutePath.Split('/')[2];
            SeriesLink = seriesLink;
   
            VideoPages = new List<VideoPage>();
            _cache = new Dictionary<string, Task<string>>();
            Console.WriteLine("Starting...");
        }
        public string Name { get; private set; }
        public string SeriesLink { get; private set; }
        public string pageParmName { get; private set; } = "page";
        public List<VideoPage> VideoPages { get; private set; }
        private readonly Dictionary<string, Task<string>> _cache;

        public static async Task<Series> GetAsync(string seriesLink)
        {
            Series series = new Series(seriesLink);
            var pageCount = GetMaxPageNumber(series.SeriesLink);
            foreach (var uri in series.GetUris(pageCount))
            {
                series.GetWebPageAsync(uri);
            }

            var tasks = series._cache.Select(c => c.Value);
            string[] htmls = await Task.WhenAll(tasks);
            Console.WriteLine("All page downloaded");

            Thread.Sleep(3000);

            var htmlDocs = htmls.Select(h => series.Parse(h));
            foreach (var htmlDoc in htmlDocs)
            {
                series.VideoPages.AddRange(series.GetPageVideos(htmlDoc));
            }
            return series;
        }

        private Task<string> GetWebPageAsync(string uri)
        {
            Task<string> downloadTask;
            if (_cache.TryGetValue(uri, out downloadTask)) return downloadTask;
            return _cache[uri] = new WebClient().DownloadStringTaskAsync(uri);
        }

        private HtmlDocument Parse(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        private IEnumerable<VideoPage> GetPageVideos(HtmlDocument htmlDocument)
        {
            Uri uri = new Uri(SeriesLink);
            return htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.entries li a.title").Select(c =>
            new VideoPage
            {
                Title = c.InnerText.Replace("&#160;", " "),
                Url = $"{uri.Scheme}://{uri.Host}{c.Attributes["href"].Value}"
            });
        }
    
        public static int GetMaxPageNumber(string uri)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDocument = web.Load(uri);
            var pager = htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.paging li a");
            if (!pager.Any())
            {
                return 1;
            }
            return int.Parse(pager.Last(c => c.InnerText.IsNumeric()).InnerText);
        }

        private IEnumerable<string> GetUris(int pageCount)
        {
            Console.WriteLine($"Total pages {pageCount}");
            for (int i = 1; i <= pageCount; i++)
            {
                yield return $"{SeriesLink}?{pageParmName}={i}";
            }
        }

        public void SaveTo(string path = null)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\\{Name}.json";
            }

            string json = JsonConvert.SerializeObject(this.VideoPages, Formatting.Indented);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json);
            Console.WriteLine("File Saved");
        }
    }
}
