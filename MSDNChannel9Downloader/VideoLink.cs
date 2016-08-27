using System;

namespace MSDNChannel9Downloader
{
    public class VideoPageLink
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public void Print()
        {
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Url: {Url}");
            Console.WriteLine();
        }
    }
}
