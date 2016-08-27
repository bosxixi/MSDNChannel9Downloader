using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSDNChannel9Downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader.Tests
{
    [TestClass()]
    public class SeriesTests
    {
        [TestMethod()]
        public void getPageVideosTest()
        {
            int number = Series.getMaxPageNumber("https://channel9.msdn.com/Series/aspnetmonsters");
            Assert.IsTrue(number > 0);
        }
    }
}