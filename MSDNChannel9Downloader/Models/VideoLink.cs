using System;
using System.Collections.Generic;

namespace MSDNChannel9Downloader
{
    public class VideoPage
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public ICollection<MediaFileInfo> MediaFileInfos { get; set; }

        public void Print()
        {
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Url: {Url}");
            Console.WriteLine();
        }
    }

   
}
