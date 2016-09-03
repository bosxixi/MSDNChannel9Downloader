using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public enum MediaFileType
    {
        /// <summary>
        /// Audio only
        /// </summary>
        MP3 = 1,
        /// <summary>
        /// approx. 500-800kbps
        /// </summary>
        LowQualityMP4 = 2,
        /// <summary>
        /// Lo-band, Mobile
        /// </summary>
        MidQualityWMV = 3,
        /// <summary>
        /// approx. 2-2.5Mbps
        /// </summary>
        MidQualityMP4 = 4,
        /// <summary>
        /// best available
        /// </summary>
        HighQualityMP4 = 5,
        /// <summary>
        /// best available
        /// </summary>
        HighQualityWMV = 6
    }
}
