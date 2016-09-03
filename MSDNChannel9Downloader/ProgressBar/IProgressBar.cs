using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    interface IProgressBar
    {
        void Tick(long currentTick, string message = "");
        void Tick(string message = "");
        decimal Percentage { get; }
        long CurrentTick { get; }
        long MaxTicks { get; }
    }
}
