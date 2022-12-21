/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PecSynchronizationServices
{
    public class Bim360Synchronizer : ISynchronizer
    {
        public ISynchronizationGroup SynchronizationGroup { get; }

        public event SynchronizationEventHandler SynchronizationEvent;

        public Bim360Synchronizer(ISynchronizationGroup synchronizationGroup)
        {
            SynchronizationGroup = synchronizationGroup;
        }

        public SyncResult Synchronize()
        {
            var successes = new List<string>();
            var failures = new List<string>();
            var count = 0;
            foreach (var item in SynchronizationGroup.SynchronizationItems)
            {
                try
                {
                    OnSynchronization($"Item {++count} of {SynchronizationGroup.SynchronizationItems.Count}");
                    var result = Synchronize(item);
                    successes.AddRange(result.Successes);
                    failures.AddRange(result.Failures);
                }
                catch (Exception ex)
                {
                    failures.Add($"Sync error: {ex.Message}");
                    OnSynchronization(ex.Message);
                }
            }

            return new SyncResult(successes.ToArray(), failures.ToArray());
        }

        private SyncResult Synchronize(ISynchronizationItem item)
        {
            var localFolder = item.LocalFolder;
            var remoteFolder = item.RemoteFolder;

            var remoteFolderName = remoteFolder.GetName();
            OnSynchronization($"Synchronizing {remoteFolderName} -> {localFolder.GetName()}...");

            var successes = new List<string>();
            var failures = new List<string>();

            var remoteFiles = remoteFolder.GetFiles();
            var remoteFolders = remoteFolder.GetFolders();
            var nothingToDo = remoteFiles.Count == 0 && remoteFolders.Count == 0;

            Debug.Assert(string.Compare(remoteFolderName, "new folder", true) != 0);

            if (nothingToDo)
            {
                OnSynchronization("Nothing to synchronize.");
                return new SyncResult(successes, failures);
            }
            var localPath = localFolder.GetName();
            Directory.CreateDirectory(localPath);
            var localFiles = localFolder.GetFiles();
            var localFileDictionary = new Dictionary<string, IFile>();
            foreach(var localFile in localFiles)
            {
                localFileDictionary[localFile.GetName()] = localFile;
            }
            foreach (var remoteFile in remoteFiles)
            {
                try
                {
                    var remoteName = remoteFile.GetName();
                    if (NeedsLocalUpdate(localFolder, remoteFile, localFileDictionary))
                    {
                        var message = $"Updating file {remoteName}.";
                        OnSynchronization(message);
                        var remoteFileContents = remoteFile.GetFile();
                        var localFilePath = Path.Combine(localPath, remoteName);
                        File.WriteAllBytes(localFilePath, remoteFileContents);
                        File.SetLastWriteTimeUtc(localFilePath, remoteFile.GetLastModifiedUtc());
                        successes.Add(message);
                    }
                    else
                    {
                        var message = $"File {remoteName} is already up to date.";
                        successes.Add(message);
                        OnSynchronization(message);
                    }
                }
                catch (Exception ex)
                {
                    var message = $"Sync error: {ex.Message}";
                    failures.Add(message);
                    OnSynchronization(message);
                }
            }

            if (item.DeleteRemovedItems)
            {
                OnSynchronization("Checking for deleted/renamed items.");
                var remoteFileNames = new HashSet<string>(remoteFiles.Select(f => f.GetName().ToLowerInvariant()));
                var excludeItems = new HashSet<string>(item.ExcludeItems.Select(ei => ei.ToLowerInvariant()));
                foreach (var localFile in localFolder.GetFiles())
                {
                    try
                    {
                        var localFileName = localFile.GetName().ToLowerInvariant();
                        if (!excludeItems.Contains(localFileName) &&
                            !remoteFileNames.Contains(localFileName))
                        {
                            var localFilePath = Path.Combine(localPath, localFile.GetName());
                            var message = $"Deleting local file {localFilePath}.";
                            OnSynchronization(message);
                            File.Delete(localFilePath);
                            successes.Add(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = $"Delete error: {ex.Message}";
                        failures.Add(message);
                        OnSynchronization(message);
                    }
                }

                OnSynchronization("Checking for deleted/renamed folders.");
                var remoteDirectoryNames = new HashSet<string>(remoteFolders.Select(f => f.GetName().ToLowerInvariant()));
                foreach (var localSubFolder in localFolder.GetFolders().Where(f => f is LocalFolder).Cast<LocalFolder>())
                {
                    try
                    {
                        var subFolderName = Path.GetFileName(localSubFolder.GetName()).ToLowerInvariant();
                        if (!excludeItems.Contains(subFolderName) &&
                            !remoteDirectoryNames.Contains(subFolderName) )
                        {
                            var localFolderPath = localSubFolder.FolderPath;
                            var message = $"Deleting local folder {localFolderPath} including subfolders.";
                            OnSynchronization(message);
                            Directory.Delete(localFolderPath, true);
                            successes.Add(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = $"Remove folder error: {ex.Message}";
                        failures.Add(message);
                        OnSynchronization(message);
                    }
                }
            }

            OnSynchronization($"{remoteFolder.GetName()} Complete.");

            if (item.SynchronizeSubFolders)
            {
                foreach (var remoteSubFolder in remoteFolders)
                {
                    var localSubFolderPath = Path.Combine(localPath, remoteSubFolder.GetName());
                    var localSubFolder = new LocalFolder(localSubFolderPath);
                    var result = Synchronize(new Bim360SynchronizationItem(
                        remoteSubFolder,
                        localSubFolder,
                        new Bim360SynchronizationItem.SyncOptions(item.SynchronizeSubFolders, item.DeleteRemovedItems, item.ExcludeItems)));
                    successes.AddRange(result.Successes);
                    failures.AddRange(result.Failures);
                }
            }

            return new SyncResult(successes.ToArray(), failures.ToArray());
        }

        private bool NeedsLocalUpdate(IFolder localFolder, IFile remoteFile, Dictionary<string, IFile> localFileNames)
        {
            var remoteName = remoteFile.GetName();

            if (!localFolder.FileExists(remoteName)) { return true; }

            if (localFileNames.TryGetValue(remoteName, out IFile localFile))
            {
                if (localFile.GetLastModifiedUtc() != remoteFile.GetLastModifiedUtc()) { return true; }
                else { return false; }
            }
            else
            {
                return true;
            }
        }

        private void OnSynchronization(string message)
        {
            SynchronizationEvent?.Invoke(this, new SynchronizationEventArgs(message));
        }
    }
}
