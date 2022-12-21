/*
 * Copyright (C) 2020 Pheinex LLC
 */

using PecForgeApi;
using System;
using System.Linq;

namespace PecSynchronizationServices
{
    public class Bim360Item : IFile
    {
        private string _name;
        private PecForgeApi.Items.ItemData _itemData;
        private PecForgeApi.Items.Included _latestVersion;

        private PecForgeApi.Items.ItemData ItemData => _itemData ?? (_itemData = ApiClient.GetItem(ProjectId, ItemId));

        private PecForgeApi.Items.Included LatestVersion => _latestVersion ?? (_latestVersion = ItemData.Included.First(item => item.Type == ContentTypes.Versions));

        private PecApi ApiClient { get; set; } = Defaults.ApiClient;

        public string ProjectId { get; }

        public string ItemId { get; }

        public Bim360Item(string projectId, string itemId)
        {
            ProjectId = projectId;
            ItemId = itemId;
        }

        public string GetName() => _name ?? (_name = ItemData.Data.Attributes.DisplayName);

        public long GetSize()
        {
            return LatestVersion.Attributes.StorageSize;
        }

        public DateTime GetLastModifiedUtc()
        {
            return LatestVersion.Attributes.LastModifiedTime.DateTime;
        }

        public byte[] GetFile()
        {
            var latestVersionId = LatestVersion.Relationships.Storage.Data.Id;
            var components = latestVersionId.Split(':')[3].Split('/');
            var bucketKey = components[0];
            var objectName = components[1];
            var data = ApiClient.DownloadObject(bucketKey, objectName);
            return data;
        }

        public bool Equals(IFile other)
        {
            if (other is Bim360Item otherBim360Item)
            {
                return otherBim360Item.ItemId == ItemId;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IFile);
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode();
        }
    }
}
