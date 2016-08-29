﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class VideoPage
    {
        public string Url { get; set; }
        public string Title { get; set; }

        private static object syncRoot = new Object();

        public List<MediaFileInfo> MediaFileInfos { get; set; }

        public MediaFileInfo BestQuality
        {
            get
            {
                if (!MediaFileInfos?.Any() ?? true)
                {
                    return null;
                }
                int max = MediaFileInfos.Select(c => (int)c.Type).Max();
                return MediaFileInfos.FirstOrDefault(c => c.Type == (MediaFileType)max);
            }
        }
        public void LoadMediaFileInfos()
        {
            try
            {
                MediaFileInfos = MediaFileInfos ?? new List<MediaFileInfo>();
                WebClient client = new WebClient();
                string htmlString = client.DownloadString(Url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlString);
                var lis = htmlDocument.DocumentNode.ChildNodes.QuerySelectorAll("ul.download li");

                foreach (var li in lis)
                {
                    var file = new MediaFileInfo()
                    {
                        FileUri = GetMediaFileUri(li),
                        FileSize = GetMediaFileSize(li),
                        Type = GetMediaFileType(li)
                    };
                    lock (syncRoot)
                    {
                        MediaFileInfos.Add(file);
                    }
            
                }
                BestQuality?.Print();
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
