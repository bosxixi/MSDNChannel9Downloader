using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class VideoPage
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public ICollection<MediaFileInfo> MediaFileInfos { get; set; } = new List<MediaFileInfo>();

        public MediaFileInfo BestQuality
        {
            get
            {
                int max = MediaFileInfos.Select(c => (int)c.Type).Max();
                return MediaFileInfos.FirstOrDefault(c => c.Type == (MediaFileType)max);
            }
        }

        public void LoadMediaFileInfos()
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDocument = web.Load(Url);
                var lis = htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.download li");

                foreach (var li in lis)
                {
                    var file = new MediaFileInfo()
                    {
                        FileUri = GetMediaFileUri(li),
                        FileSize = GetMediaFileSize(li),
                        Type = GetMediaFileType(li)
                    };

                    MediaFileInfos.Add(file);
                    file.Print();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("//-------------------------Exception");
                Console.WriteLine($"Title: {Title}");
                Console.WriteLine($"Url: {Url}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("//-------------------------Exception End");
            }
           
        }
        private string GetMediaFileUri(HtmlNode node)
        {
            return node.QuerySelectorAll("a").Single().Attributes["href"].Value;
        }
        private string GetMediaFileSize(HtmlNode node)
        {
            string text = node.QuerySelectorAll("div.popup").Single().InnerText;
            return text.Substring(text.Length - 15, 15).Trim();
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
            if (node.QuerySelectorAll("a").Single().InnerText.Contains("WMV"))
            {
                return MediaFileType.WMV;
            }
            return MediaFileType.Unknow;
        }

        public void Print()
        {
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Url: {Url}");
            Console.WriteLine();
        }


    }


}
