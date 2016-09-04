using bosxixi.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class VideoPage
    {
        public string Uri { get; set; }
        public string Title { get; set; }

        private static object syncRoot = new Object();

        public List<MediaFileInfo> MediaFileInfos { get; set; }

        public string FileName
        {
            get
            {
                return Title.GetValidFileName();
            }
        }

        public MediaFileInfo BestQuality
        {
            get
            {
                if (!MediaFileInfos?.Any() ?? true)
                {
                    return null;
                }
                float max = MediaFileInfos.Select(c => c.FileSize).Max();
                return MediaFileInfos.FirstOrDefault(c => c.FileSize == max);
            }
        }



        public void LoadMediaFileInfos()
        {
            try
            {
                using (var client = new WebClient())
                {
                    MediaFileInfos = MediaFileInfos ?? new List<MediaFileInfo>();
                    string htmlString = client.DownloadString(Uri);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlString);
                    var lis = htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.download li");

                    foreach (var li in lis)
                    {
                        var file = new MediaFileInfo()
                        {
                            FileUri = GetMediaFileUri(li),
                            FileSize = GetMediaFileSize(li),
                            Type = GetMediaFileType(li),
                            FileSizeUnit = GetMediaFileSizeUnit(li)
                        };
                        lock (syncRoot)
                        {
                            MediaFileInfos.Add(file);
                        }
                    }
                }
                BestQuality?.Print();
            }
            catch (Exception ex)
            {

                Console.WriteLine("//-------------------------Exception");
                Console.WriteLine($"Title: {Title}");
                Console.WriteLine($"Url: {Uri}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("//-------------------------Exception End");
            }
        }

        private string GetMediaFileUri(HtmlNode node)
        {
            return node.QuerySelectorAll("a").Single().Attributes["href"].Value;
        }
        private float GetMediaFileSize(HtmlNode node)
        {
            string text = node.QuerySelectorAll("div.popup").Single().InnerText;
            text = text.Substring(text.Length - 15, 15).Trim();
            Regex r = new Regex(@"\d+.?\d", RegexOptions.IgnoreCase);
            return float.Parse(r.Match(text).ToString());
          
        }
        private string GetMediaFileSizeUnit(HtmlNode node)
        {
            string text = node.QuerySelectorAll("div.popup").Single().InnerText;
            text = text.Substring(text.Length - 15, 15).Trim();
            Regex r = new Regex(@"[^ ?\d+.?\d ?]+", RegexOptions.IgnoreCase);
            return r.Match(text).ToString();
        }
        private MediaFileType GetMediaFileType(HtmlNode node)
        {
            if (node.QuerySelectorAll("a").Single().InnerText.Contains("MP3"))
            {
                return MediaFileType.MP3;
            }
            if (node.QuerySelectorAll("span").Single().InnerText.Contains("500-800"))
            {
                return MediaFileType.LowQualityMP4;
            }
            if (node.QuerySelectorAll("span").Single().InnerText.Contains("2-2.5"))
            {
                return MediaFileType.MidQualityMP4;
            }
            if (node.QuerySelectorAll("a").Single().InnerText.Contains("MP4"))
            {
                return MediaFileType.HighQualityMP4;
            }
            if (!node.QuerySelectorAll("a").Single().InnerText.Contains(",") && !node.QuerySelectorAll("a").Single().InnerText.Contains("，"))
            {
                return MediaFileType.HighQualityWMV;
            }
            return MediaFileType.MidQualityWMV;
        }

        public void Print()
        {
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Url: {Uri}");
            Console.WriteLine();
        }
    }


}
