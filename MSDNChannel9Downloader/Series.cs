using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace MSDNChannel9Downloader
{

    class Series
    {
        public Series(string seriesLink, HttpClient httpClient)
        {
            if (String.IsNullOrEmpty(seriesLink))
            {
                throw new ArgumentNullException(nameof(seriesLink));
            }
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(seriesLink));
            }
            SeriesLink = seriesLink;
            _httpClient = httpClient;
            PageCount = getMaxPageNumber();
        }
        public string SeriesLink { get; set; }

        public readonly HttpClient _httpClient;

        public async Task<IEnumerable<VideoLink>> GetVideoLinksAsync()
        {
            List<VideoLink> videoLinks = new List<VideoLink>();
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

        private async Task<string> GetHtmlByPageNumerAsync(int page) =>
            await _httpClient.GetStringAsync($"https://channel9.msdn.com/Series/aspnetmonsters?page={page}");

        private async Task LoadHtmlDocumentsAsync()
        {
            foreach (var html in await GetHtmlsAsync())
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                HtmlDocuments.Add(htmlDoc);
            }
        }

        public IEnumerable<VideoLink> getPageVideos(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.entries li a.title").Select(c =>
            new VideoLink
            {
                Title = c.InnerText.Replace("&#160;", " "),
                Url = c.Attributes["href"].Value
            });
        }

        static int getMaxPageNumber()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDocument = web.Load("https://channel9.msdn.com/Series/aspnetmonsters");
            var pager = htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.paging li a");
            if (!pager.Any())
            {
                return 0;
            }

            return int.Parse(pager.Last(c => c.InnerText.IsNumeric()).InnerText);
        }

        private async Task<IEnumerable<string>> GetHtmlsAsync()
        {
            List<string> htmls = new List<string>();
            for (int i = 1; i <= PageCount; i++)
            {
                htmls.Add(await GetHtmlByPageNumerAsync(i));
                Console.WriteLine($"Page {i} downloaded");
            }
            return htmls;
        }
    }
}
