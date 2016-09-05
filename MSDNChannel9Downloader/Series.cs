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
using bosxixi.Extensions;
using System.Text;
using bosxixi.ProgressBar;

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

            VideoPages = new List<VideoInfo>();
            _cache = new Dictionary<string, Task<string>>();
            _uris = new Dictionary<int, string>();
            Console.WriteLine("Starting...");
        }
        public string Name { get; private set; }
        public string SeriesLink { get; private set; }
        public string pageParmName { get; private set; } = "page";
        public List<VideoInfo> VideoPages { get; private set; }
        private readonly Dictionary<string, Task<string>> _cache;
        private Dictionary<int, string> _uris { get; set; }

        public static async Task<Series> GetAsync(string seriesLink)
        {
            Series series = new Series(seriesLink);
            var pageCount = GetMaxPageNumber(series.SeriesLink);
            series.LoadUris(pageCount);
            foreach (var item in series._uris)
            {
                series.GetWebPageAsync(item.Key, item.Value);
            }

            var tasks = series._cache.Select(c => c.Value);
            string[] htmls = await Task.WhenAll(tasks);

            htmls = htmls.Select(c =>
            {
                byte[] bytes = Encoding.Default.GetBytes(c);
                return Encoding.UTF8.GetString(bytes);
            }).ToArray();

            Console.WriteLine("All page downloaded");

            var htmlDocs = htmls.Select(h => series.Parse(h));
            foreach (var htmlDoc in htmlDocs)
            {
                series.VideoPages.AddRange(series.GetPageVideos(htmlDoc));
            }
            return series;
        }

        private Task<string> GetWebPageAsync(int page, string uri)
        {
            Task<string> downloadTask;
            if (_cache.TryGetValue(uri, out downloadTask)) return downloadTask;
            var client = new WebClient();
            var bar = new ProgressBar(100);
            int currentPercent = 0;
            downloadTask = client.DownloadStringTaskAsync(uri);
            client.DownloadProgressChanged += (sender, args) => {
                if (currentPercent < args.ProgressPercentage)
                {
                    currentPercent = args.ProgressPercentage;
                    bar.Tick(args.ProgressPercentage, $"Page {page} downloading");
                }
            };
            client.DownloadStringCompleted += (sender, args) => {
                Console.WriteLine($"Page {page} download");
                client.Dispose();
            };
            //client.Disposed += (sender, args) => { Console.WriteLine($"Page {page} Disposed !!"); };
            return _cache[uri] = downloadTask;
        }

        private HtmlDocument Parse(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        private IEnumerable<VideoInfo> GetPageVideos(HtmlDocument htmlDocument)
        {
            Uri uri = new Uri(SeriesLink);
            return htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.entries li a.title").Select(c =>
            new VideoInfo
            {
                Title = c.InnerText.Replace("&#160;", " "),
                Uri = $"{uri.Scheme}://{uri.Host}{c.Attributes["href"].Value}"
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

        private void LoadUris(int pageCount)
        {
            Console.WriteLine($"Total pages {pageCount}");
            for (int i = 1; i <= pageCount; i++)
            {
                _uris.Add(i, $"{SeriesLink}?{pageParmName}={i}");
            }
        }

        public void SaveTo(string path = null)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = $@"Z:\download\{Name}.json";
                //path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\\{Name}.json";
            }

            string json = JsonConvert.SerializeObject(this.VideoPages, Formatting.Indented);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json, System.Text.Encoding.UTF8);
            Console.WriteLine("File Saved");
            SaveBestQualityUri(path);
        }

        public void SaveBestQualityUri(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var uris = VideoPages.Where(c => c.BestQuality != null).Select(c => c.BestQuality.FileUri);
            File.WriteAllLines(path, uris, System.Text.Encoding.UTF8);
            Console.WriteLine("File-Uri Saved");
        }
    }
}
