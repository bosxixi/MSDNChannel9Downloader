using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public class ProgressBar : IProgressBar
    {
        public readonly int _row;
        public ProgressBar(int row, long maxTicks)
        {
            _row = row;
            MaxTicks = maxTicks;
        }
        public long CurrentTick { get; private set; }
        public long MaxTicks { get; private set; }
        public decimal Percentage
        {
            get
            {
                return ((decimal)CurrentTick / (decimal)MaxTicks) * 100m;
            }
        }

        public void Tick(string message = "")
        {
            CurrentTick++;
            DisplayProgressBar(message);
        }

        public void Tick(long currentTick, string message = "")
        {
            if (currentTick > MaxTicks)
            {
                throw new ArgumentOutOfRangeException(nameof(currentTick));
            }
            CurrentTick = currentTick;
            DisplayProgressBar(message);
        }

        private void DisplayProgressBar(string message)
        {
            Console.CursorVisible = false;
            ClearProgressBarRow();
            Console.WriteLine($"{Percentage.ToString("0.00")}% | {message}");
            Console.CursorVisible = true;
        }

        private void ClearProgressBarRow()
        {
            Console.SetCursorPosition(0, _row);
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write(' ');
            }
            Console.SetCursorPosition(0, _row);
        }
    }
}
