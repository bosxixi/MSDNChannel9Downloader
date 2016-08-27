using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class MediaFileInfo
    {
        public string Url { get; set; }
        public string FileSize { get; set; }
        public MediaFileType Type { get; set; }
    }
}
