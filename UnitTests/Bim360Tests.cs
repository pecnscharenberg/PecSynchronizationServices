using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PecSynchronizationServices;

namespace UnitTests
{
    [TestClass]
    public class Bim360Tests
    {
        private readonly string testProjectId = @"b.63d04401-ee68-4768-a325-150769bc8bb9";
        private readonly string testFolderId = @"urn:adsk.wipprod:fs.folder:co.67QxCJPuT1CEvHN6A7FqmA";

        [TestMethod]
        public void TestFolderName()
        {
            var bim360Folder = new Bim360Folder(testProjectId, testFolderId);

            Assert.AreEqual(@"Standards Folder for Unit Tests", bim360Folder.GetName());
        }

        [TestMethod]
        public void TestSubFolders()
        {
            var bim360Folder = new Bim360Folder(testProjectId, testFolderId);

            var subFolders = bim360Folder.GetFolders();

            Assert.AreEqual(3, subFolders.Count);
        }

        [TestMethod]
        public void TestItems()
        {
            var bim360Folder = new Bim360Folder(testProjectId, testFolderId);

            var items = bim360Folder.GetFiles();

            Assert.IsTrue(items.Count > 0);

            var smallRevitModel = items.FirstOrDefault(item => item.GetName() == "Small File.rvt");
            Assert.IsNotNull(smallRevitModel);
            Assert.AreEqual(8736768, smallRevitModel.GetSize());
            
            var expectedDate = DateTimeOffset.Parse("2020-04-06T15:21:06.0000000Z");
            Assert.AreEqual(expectedDate.DateTime, smallRevitModel.GetLastModifiedUtc());
        }
    }
}
