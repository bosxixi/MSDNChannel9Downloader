using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSDNChannel9Downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDNChannel9Downloader.Tests
{
    [TestClass()]
    public class ExtensionTests
    {
        [TestMethod()]
        public void IsValidFileNameTest()
        {
            var file1 = "file.pdf";
            var file2 = "file.gif";
            var file3 = "abc abc.docx";
            var file4 = "abc.v2.docx";
            var file5 = "fi<le.pdf";
            var file6 = "file.pdf";
            var file7 = ".gif";
            var file8 = ".docx";
            var file9 = "abc.docx";
            var file10 = "ab*c.v2.docx";
            var file11 = "abc.v2.docx";


            Assert.IsTrue(file1.IsValidFileName());
            Assert.IsTrue(file2.IsValidFileName());
            Assert.IsTrue(file3.IsValidFileName());
            Assert.IsTrue(file4.IsValidFileName());
            Assert.IsFalse(file5.IsValidFileName());
            Assert.IsTrue(file6.IsValidFileName());
            Assert.IsFalse(file7.IsValidFileName());
            Assert.IsFalse(file8.IsValidFileName());
            Assert.IsTrue(file9.IsValidFileName());
            Assert.IsFalse(file10.IsValidFileName());
            Assert.IsTrue(file11.IsValidFileName());
        }

        [TestMethod()]
        public void GetValidFileNameTest()
        {
            var file1 = "fi<le.pdf";
            var file2 = "ab*c.v2.docx";

            var validfile1 = file1.GetValidFileName();
            var validfile2 = file2.GetValidFileName();

            Assert.Fail();
        }
    }
}