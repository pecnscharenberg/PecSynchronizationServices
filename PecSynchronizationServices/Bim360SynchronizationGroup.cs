using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PecSynchronizationServices
{
    public class Bim360SynchronizationGroup : ISynchronizationGroup
    {
        public IList<ISynchronizationItem> SynchronizationItems { get; }

        public Bim360SynchronizationGroup(PecForgeApi.Bim360SyncItem[] syncItems)
        {
            SynchronizationItems = syncItems
                .Select(item => new Bim360SynchronizationItem(
                    new Bim360Folder(item.RemoteFolder.ProjectId, item.RemoteFolder.FolderId),
                    new LocalFolder(item.LocalFolder.FolderPath),
                    new Bim360SynchronizationItem.SyncOptions(item.SyncronizeSubFolders, item.RemoveDeletedItems, item.ExcludeItems))
                )
                .ToArray();
        }
    }
}
