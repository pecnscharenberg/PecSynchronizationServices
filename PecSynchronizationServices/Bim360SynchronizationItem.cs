/*
 * Copyright (C) 2020 Pheinex LLC
 */

namespace PecSynchronizationServices
{
    public class Bim360SynchronizationItem : ISynchronizationItem
    {
        private SyncOptions Options { get; }

        public IFolder RemoteFolder { get; }

        public IFolder LocalFolder { get; }

        public bool SynchronizeSubFolders => Options.SynchronizeSubFolders;

        public bool DeleteRemovedItems => Options.DeleteRemovedItems;

        public string[] ExcludeItems => Options.ExcludeItems;

        public Bim360SynchronizationItem(IFolder remoteFolder, IFolder localFolder, SyncOptions options)
        {
            RemoteFolder = remoteFolder;
            LocalFolder = localFolder;
            Options = options;
        }

        public override string ToString()
        {
            return $"Sync Item: {LocalFolder.GetName()} | {RemoteFolder.GetName()}";
        }

        public class SyncOptions
        {
            public bool SynchronizeSubFolders { get; }

            public bool DeleteRemovedItems { get; }

            public string[] ExcludeItems { get; }

            public SyncOptions(bool synchronizeSubFolders, bool deleteRemovedItems, string[] exlcudeItems)
            {
                SynchronizeSubFolders = synchronizeSubFolders;
                DeleteRemovedItems = deleteRemovedItems;
                ExcludeItems = exlcudeItems;
            }

        }
    }
}
