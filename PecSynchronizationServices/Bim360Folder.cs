/*
 * Copyright (C) 2020 Pheinex LLC
 */

using PecForgeApi;
using System.Collections.Generic;
using System.Linq;

namespace PecSynchronizationServices
{
    public class Bim360Folder : IFolder
    {
        private string _name;
        private PecForgeApi.Folders.FolderData _folderData;

        private PecApi ApiClient { get; set; } = Defaults.ApiClient;

        private PecForgeApi.Folders.FolderData FolderData => _folderData ?? (_folderData = ApiClient.GetFolder(ProjectId, FolderId));

        public string ProjectId { get; }

        public string FolderId { get; }

        public Bim360Folder(string projectId, string folderId)
        {
            ProjectId = projectId;
            FolderId = folderId;
        }

        public string GetName()
        {
            return _name ?? (_name = FolderData.Data.Attributes.Name);
        }

        public IList<IFolder> GetFolders()
        {
            return ApiClient
                .GetFolderContents(ProjectId, FolderId)
                .Data
                .Where(item => item.Type == ContentTypes.Folders)
                .Select(item => new Bim360Folder(ProjectId, item.Id))
                .ToArray();
        }

        public IList<IFile> GetFiles()
        {
            return ApiClient
                .GetFolderContents(ProjectId, FolderId)
                .Data
                .Where(item => item.Type == ContentTypes.Items)
                .Select(item => new Bim360Item(ProjectId, item.Id))
                .ToArray();
        }

        public bool FileExists(string fileName) => GetFiles().FirstOrDefault(item => item.GetName() == fileName) != null;
    }
}
