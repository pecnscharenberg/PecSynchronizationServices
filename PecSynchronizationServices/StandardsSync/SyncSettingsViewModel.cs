using PecForgeApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PecSynchronizationServices.StandardsSync
{
    class SyncSettingsViewModel : ObservableObject
    {
        private bool userCanSelectSyncItems = true;
        private ObservableCollection<ISyncronizationListItem> synchronizationItems;

        private PecApi Api => Defaults.ApiClient; 

        public bool UserCanSelectSyncItems
        {
            get => userCanSelectSyncItems;
            set
            {
                userCanSelectSyncItems = value;
                OnPropertyChanged(nameof(UserCanSelectSyncItems));
            }
        }

        public ObservableCollection<ISyncronizationListItem> SynchronizationItems
        {
            get => synchronizationItems;
            set
            {
                synchronizationItems = value;
                OnPropertyChanged(nameof(SynchronizationItems));
            }
        }

        public SyncSettingsViewModel()
        {
            LoadSyncItems();
        }

        public void SelectAllSynchronizationItems()
        {
            foreach(var item in SynchronizationItems)
            {
                item.SynchronizeItem = true;
            }
        }

        public void DeselectAllSynchronizationItems()
        {
            foreach(var item in SynchronizationItems)
            {
                item.SynchronizeItem = false;
            }
        }

        private async void LoadSyncItems()
        {
            try
            {
                UserCanSelectSyncItems = false;
                SynchronizationItems = new ObservableCollection<ISyncronizationListItem>(new[] { new LoadingSyncItem("Loading Sync Items...") });
                var syncItems = await Task.Run(() =>
                {
                    var bim360SyncItems = Api.GetBim360SyncItems();
                    return bim360SyncItems.Select(si => new SynchronizationListItem(si));
                });
                SynchronizationItems = new ObservableCollection<ISyncronizationListItem>(syncItems);
                UserCanSelectSyncItems = true;
            }
            catch (Exception ex)
            {
                SynchronizationItems = new ObservableCollection<ISyncronizationListItem>(new[] { new LoadingSyncItem(ex.Message) });
                UserCanSelectSyncItems = false;
            }
        }

        public interface ISyncronizationListItem
        {
            string ProjectName { get; }
            string LocalPath { get; }
            string RemotePath { get; }
            bool SynchronizeItem { get; set; }
        }

        private class LoadingSyncItem : ISyncronizationListItem
        {
            public string ProjectName { get; }

            public string LocalPath => "";

            public string RemotePath => "";

            public bool SynchronizeItem { get; set; }

            public LoadingSyncItem (string message)
            {
                ProjectName = message;
            }
        }

        public class SynchronizationListItem : ObservableObject, ISyncronizationListItem
        {
            private static Dictionary<string, string> ProjectNames { get; } = new Dictionary<string, string>();

            private string projectName;
            private string remoteName;

            private PecApi Api { get; } = new PecForgeApi.PecApi(@"FFBD9514-794E-4318-B690-E1200CA344C6");
            public Bim360SyncItem SynchronizationItem { get; }

            public string ProjectName
            {
                get => projectName;
                set
                {
                    projectName = value;
                    OnPropertyChanged(nameof(ProjectName));
                }
            }

            public string RemotePath
            {
                get => remoteName;
                set
                {
                    remoteName = value;
                    OnPropertyChanged(nameof(RemotePath));
                }
            }

            public string LocalPath => SynchronizationItem?.LocalFolder?.FolderPath ?? "**ERROR**";

            public bool SynchronizeItem
            {
                get => Defaults.SharedInstance.SynchronizationSettings.ShouldSynchronize(SynchronizationItem);
                set
                {
                    Defaults.SharedInstance.SynchronizationSettings.SetShouldSynchronize(SynchronizationItem, value);
                    OnPropertyChanged(nameof(SynchronizeItem));
                }
            }

            public SynchronizationListItem(Bim360SyncItem syncItem)
            {
                SynchronizationItem = syncItem;
                FetchAndSetProjectName();
                FetchAndSetFolderPath();
            }

            private async void FetchAndSetProjectName()
            {
                if (SynchronizationItem?.RemoteFolder is PecForgeApi.Bim360Folder remoteFolder &&
                    remoteFolder.HubId is string hubId &&
                    remoteFolder.ProjectId is string projectId)
                {
                    ProjectName = "Loading Project...";

                    string existingProjectName;
                    lock (ProjectNames)
                    {
                        ProjectNames.TryGetValue(projectId, out existingProjectName);
                    }

                    if (existingProjectName != null)
                    {
                        ProjectNames[projectId] = existingProjectName;
                        ProjectName = existingProjectName;
                    }
                    else
                    {
                        var projectData = await Task.Run(() =>
                        {
                            try
                            {
                                return Api.GetProject(hubId, projectId);
                            }
                            catch
                            {
                                return null;
                            }
                        });

                        if (projectData?.Data?.Attributes?.Name is string projectName)
                        {
                            lock (ProjectNames)
                            {
                                ProjectNames[projectData.Data.Id] = projectName;
                            }
                            ProjectName = projectName;
                        }
                        else
                        {
                            ProjectName = "**ERROR**";
                        }
                    }
                }
                else
                {
                    ProjectName = "**ERROR**";
                }
            }

            private async void FetchAndSetFolderPath()
            {
                if (SynchronizationItem?.RemoteFolder is PecForgeApi.Bim360Folder remoteFolder &&
                    remoteFolder.ProjectId is string projectId &&
                    remoteFolder.FolderId is string folderId)
                {
                    RemotePath = "Loading Folder...";

                    var remotePath = await Task.Run(() =>
                    {
                        try
                        {
                            var syncFolder = Api.GetFolder(projectId, folderId);
                            var folderStack = new Stack<string>();
                            folderStack.Push(syncFolder.Data.Attributes.Name);
                            var parentFolder = Api.GetFolderParent(projectId, folderId);
                            while (parentFolder != null)
                            {
                                folderStack.Push(parentFolder.Data.Attributes.Name);
                                parentFolder = Api.GetFolderParent(projectId, parentFolder.Data.Id);
                            }
                            var sb = new StringBuilder($"\\{folderStack.Pop()}");
                            while (folderStack.Count > 0)
                            {
                                sb.Append($"\\{folderStack.Pop()}");
                            }
                            return sb.ToString();
                        }
                        catch
                        {
                            return null;
                        }
                    });

                    RemotePath = remotePath ?? "**ERROR**";
                }
                else
                {
                    RemotePath = "**ERROR**";
                }
            }
        }
    }
}
