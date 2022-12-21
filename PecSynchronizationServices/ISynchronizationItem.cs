/*
 * Copyright (C) 2020 Pheinex LLC
 */

namespace PecSynchronizationServices
{
    public interface ISynchronizationItem
    {
        IFolder RemoteFolder { get; }

        IFolder LocalFolder { get; }

        bool SynchronizeSubFolders { get; }

        bool DeleteRemovedItems { get; }

        string[] ExcludeItems { get; }
    }
}
