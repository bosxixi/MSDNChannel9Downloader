using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader
{
    public static class Extension
    {
        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static bool IsValidFileName(this string fileName, string sourceFolder = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return false;
            }
            if (sourceFolder != null && File.Exists(Path.Combine(sourceFolder, fileName)))
            {
                return false;
            }
            return  true;
        }
        public static string GetValidFileName(this string fileName, string sourceFolder = null)
        {
            if (!fileName.IsValidFileName())
            {
                fileName = fileName.CleanFileName();
            }

            if (sourceFolder != null && Exists(fileName, sourceFolder))
            {
                int startNumber = 1;
                fileName = GetUnicFileName(fileName, ref startNumber, sourceFolder);
            }

            return fileName;
        }
        private static string CleanFileName(this string fileName) => new string(fileName.Where(m => !Path.GetInvalidFileNameChars().Contains(m)).ToArray<char>());
        private static bool Exists(this string currentFileName, string sourceFolder) => File.Exists(Path.Combine(sourceFolder, currentFileName));

        private static string GetUnicFileName(this string fileName, ref int currentNumber, string sourceFolder)
        {
            if (!Exists($"fileName_{currentNumber}", sourceFolder))
            {
                return $"fileName_{currentNumber}";
            }
            currentNumber++;
            return GetUnicFileName(fileName, ref currentNumber, sourceFolder);
        }
    }
}
