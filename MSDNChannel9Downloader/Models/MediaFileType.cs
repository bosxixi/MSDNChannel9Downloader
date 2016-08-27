using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public enum MediaFileType
    {
        Unknow = 0,
        /// <summary>
        /// Audio only
        /// </summary>
        MP3 = 1,
        /// <summary>
        /// approx. 500-800kbps
        /// </summary>
        LowQualityMP4 = 2,
        /// <summary>
        /// approx. 2-2.5Mbps
        /// </summary>
        MidQualityMP4 = 3,
        /// <summary>
        /// best available
        /// </summary>
        HighQualityMP4 = 4,
        /// <summary>
        /// best available
        /// </summary>
        WMV = 5
    }
}
