using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class MediaFileInfo
    {
        public string FileUri { get; set; }
        public float FileSize { get; set; }
        public string FileSizeUnit { get; set; }
        public MediaFileType Type { get; set; }
        public string FileName
        {
            get
            {
                Regex r = new Regex(@"[\w]+.[\w]+", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                return r.Match(FileUri).ToString();
            }
        }
      
        public string Suffix
        {
            get
            {
                Regex r = new Regex(@"(\w+).(\w)", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                return r.Match(FileUri).ToString();
            }
        }
        public void Print()
        {
            Console.WriteLine($"FileUri: {FileUri}");
            Console.WriteLine($"FileSize: {FileSize}");
            Console.WriteLine($"FileSizeUnit: {FileSizeUnit}");
            Console.WriteLine($"MediaFileType: {Enum.GetName(typeof(MediaFileType), Type)}");
            Console.WriteLine();
        }
    }
}
