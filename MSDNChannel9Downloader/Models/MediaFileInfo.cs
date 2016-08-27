using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class MediaFileInfo
    {
        public string FileUri { get; set; }
        public string FileSize { get; set; }
        public MediaFileType Type { get; set; }

        public void Print()
        {
            Console.WriteLine($"FileUri: {FileUri}");
            Console.WriteLine($"FileSize: {FileSize}");
            Console.WriteLine($"MediaFileType: {Enum.GetName(typeof(MediaFileType), Type)}");
            Console.WriteLine();
        }
    }
}
