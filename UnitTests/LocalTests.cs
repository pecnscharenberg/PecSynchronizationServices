using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PecSynchronizationServices;

namespace UnitTests
{
    [TestClass]
    public class LocalTests
    {
        private readonly string directoryName1 = @"../../../Test Files";

        private readonly string fileName1 = @"../../../Test Files/Small File.rvt";

        [TestMethod]
        public void TestGetFiles()
        {
            var folder = new LocalFolder(directoryName1);

            Assert.AreEqual(1, folder.GetFiles().Count);
        }

        [TestMethod]
        public void TestGetFolders()
        {
            var folder = new LocalFolder(directoryName1);

            var subfolders = folder.GetFolders();

            Assert.AreEqual(3, subfolders.Count);
            Assert.AreEqual(1, subfolders.Where(item => item.GetName().EndsWith("\\Mech")).Count());
        }

        [TestMethod]
        public void TestFileData()
        {
            var file = new LocalFile(fileName1);

            Assert.AreEqual(8736768, file.GetSize());

            var expectedDate = @"4/7/2020 1:24:23 PM";
            Assert.AreEqual(expectedDate, file.GetLastModifiedUtc().ToString());
        }
    }
}
