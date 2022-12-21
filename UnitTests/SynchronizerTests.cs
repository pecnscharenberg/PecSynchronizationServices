using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PecSynchronizationServices;

namespace UnitTests
{
    [TestClass]
    public class SynchronizerTests
    {
        [TestMethod]
        public void TestSynchronization()
        {
            var synchronizationGroup = new TestConfig();
            ISynchronizer synchronizer = new Bim360Synchronizer(synchronizationGroup);

            synchronizer.SynchronizationEvent += Synchronizer_SynchronizationEvent;

            var result = synchronizer.Synchronize();

            Assert.AreEqual(0, result.Failures.Count);
            Assert.IsTrue(result.Successes.Count > 0);
        }

        private void Synchronizer_SynchronizationEvent(object sender, SynchronizationEventArgs e)
        {
            Debug.Print(e.Message);
        }

        private class TestConfig : ISynchronizationGroup
        {
            public IList<ISynchronizationItem> SynchronizationItems { get; } = new ISynchronizationItem[]
            {
                new Bim360SynchronizationItem(
                    new Bim360Folder(@"b.63d04401-ee68-4768-a325-150769bc8bb9", @"urn:adsk.wipprod:fs.folder:co.67QxCJPuT1CEvHN6A7FqmA"),
                    new LocalFolder(@"C:\tmp\Bim360SyncTestRoot"))
            };
        }
    }
}
