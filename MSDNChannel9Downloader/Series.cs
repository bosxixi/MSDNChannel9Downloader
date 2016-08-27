using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;

namespace MSDNChannel9Downloader
{

    public class Series
    {
        public Series(string seriesLink, HttpClient httpClient)
        {
            if (String.IsNullOrEmpty(seriesLink))
            {
                throw new ArgumentNullException(nameof(seriesLink));
            }
            if (seriesLink.Contains("?"))
            {
                throw new ArgumentException("can not contain paramater.");
            }
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(seriesLink));
            }
            Uri uri = new Uri(seriesLink);
            Name = uri.AbsolutePath.Split('/')[2];
            Console.WriteLine("Starting...");

            HtmlDocuments = new List<HtmlDocument>();
            SeriesLink = seriesLink;
         
            
            _httpClient = httpClient;
            PageCount = getMaxPageNumber(SeriesLink);
            Console.WriteLine($"This Series has {PageCount} pages.");
        }
        public string Name { get; set; }
        public string SeriesLink { get; set; }

        public readonly HttpClient _httpClient;
        public string pageParmName { get; set; } = "page";

        public async Task<IEnumerable<VideoPage>> GetVideoLinksAsync()
        {
            List<VideoPage> videoLinks = new List<VideoPage>();
            if (!HtmlDocuments?.Any() ?? true)
            {
                await LoadHtmlDocumentsAsync();
            }
            foreach (var item in HtmlDocuments)
            {
                var pageVideoLinks = getPageVideos(item);
                videoLinks.AddRange(pageVideoLinks);
            }
            return videoLinks;
        }

        public readonly ICollection<HtmlDocument> HtmlDocuments;

        public int PageCount { get; private set; }

        private async Task<string> GetHtmlByPageNumerAsync(string pageParmName, int page) =>
            await _httpClient.GetStringAsync($"{SeriesLink}?{pageParmName}={page}");

        private async Task LoadHtmlDocumentsAsync()
        {
            foreach (var html in await GetHtmlsAsync())
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                HtmlDocuments.Add(htmlDoc);
            }
        }

        public IEnumerable<VideoPage> getPageVideos(HtmlDocument htmlDocument)
        {
            Uri uri = new Uri(SeriesLink);
            return htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.entries li a.title").Select(c =>
            new VideoPage
            {
                Title = c.InnerText.Replace("&#160;", " "),
                Url = $"{uri.Scheme}://{uri.Host}{c.Attributes["href"].Value}"
            });
        }

        public static int getMaxPageNumber(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDocument = web.Load(url);
            var pager = htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.paging li a");
            if (!pager.Any())
            {
                return 1;
            }

            return int.Parse(pager.Last(c => c.InnerText.IsNumeric()).InnerText);
        }

        private async Task<IEnumerable<string>> GetHtmlsAsync()
        {
            List<string> htmls = new List<string>();
            for (int i = 1; i <= PageCount; i++)
            {
                htmls.Add(await GetHtmlByPageNumerAsync(pageParmName, i));
                Console.WriteLine($"Page {i} downloaded");
            }
            return htmls;
        }

        public void SaveTo(string path = null)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\\{Name}.json";
            }
         
            string json = JsonConvert.SerializeObject(this);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json);
        }
    }
}
