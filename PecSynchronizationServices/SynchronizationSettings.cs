/*
 * Copyright (C) 2021 Pheinex LLC
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PecForgeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PecSynchronizationServices
{
    public class SettingsChangedEventArgs : EventArgs
    {
        public object Sender { get; }
        public SynchronizationSettings NewSettings { get; }

        public SettingsChangedEventArgs(object sender, SynchronizationSettings newSettings)
        {
            Sender = sender;
            NewSettings = newSettings;
        }
    }

    public delegate void SettingsChanged(SettingsChangedEventArgs e);

    public class SynchronizationSettings
    {
        public Dictionary<string, bool> FoldersToSync { get; set; } = new Dictionary<string, bool>();

        public event SettingsChanged OnSettingsChanged;

        public SynchronizationSettings() { }

        public bool ShouldSynchronize(Bim360SyncItem syncItem)
        {
            if (syncItem?.LocalFolder?.FolderPath is string localPath)
            {
                if (FoldersToSync.TryGetValue(localPath, out bool existingSyncSetting))
                {
                    return existingSyncSetting;
                }
                else
                {
                    var newSyncSetting = Directory.Exists(syncItem.LocalFolder.FolderPath);
                    FoldersToSync[localPath] = newSyncSetting;
                    NotifySettingsChanged();
                    return newSyncSetting;
                }
            }
            else
            {
                return false;
            }
        }

        public void SetShouldSynchronize(Bim360SyncItem syncItem, bool doSync)
        {
            FoldersToSync[syncItem.LocalFolder.FolderPath] = doSync;
            NotifySettingsChanged();
        }

        public Bim360SyncItem[] GetSyncItems()
        {
            var allItems = Defaults.ApiClient.GetBim360SyncItems();

            return allItems.Where(item => ShouldSynchronize(item)).ToArray();
        }

        private void NotifySettingsChanged()
        {
            OnSettingsChanged?.Invoke(new SettingsChangedEventArgs(this, this));
        }

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

        public static SynchronizationSettings FromJson(string json) => JsonConvert.DeserializeObject<SynchronizationSettings>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SynchronizationSettings self) => JsonConvert.SerializeObject(self, SynchronizationSettings.Converter.Settings);
    }
}
