/*
 * Copyright (C) 2020 Pheinex LLC
 */

using PecForgeApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PecSynchronizationServices
{
    class Defaults
    {
        private SynchronizationSettings synchronizationSettings;

        private static PecApi _apiClient;
        public static PecApi ApiClient => _apiClient ?? (_apiClient = new PecApi(@"FFBD9514-794E-4318-B690-E1200CA344C6"));

        public static Defaults SharedInstance { get; } = new Defaults();

        private Defaults()
        {
            SynchronizationSettings.OnSettingsChanged += SynchronizationSettings_OnSettingsChanged;
        }

        private void SynchronizationSettings_OnSettingsChanged(SettingsChangedEventArgs e)
        {
            Properties.Settings.Default.Bim360SynchronizationSettings = e.NewSettings.ToJson();
            Properties.Settings.Default.Save();
        }

        public SynchronizationSettings SynchronizationSettings
        {
            get
            {
                if (synchronizationSettings == null)
                {
                    if (Properties.Settings.Default.Bim360SynchronizationSettings is string existingSettingsJson &&
                        SynchronizationSettings.FromJson(existingSettingsJson) is SynchronizationSettings existingSettings)
                    {
                        Debug.Print("Found existing settings");
                        synchronizationSettings = existingSettings;
                    }
                    else
                    {
                        Debug.Print("Creating new settings");
                        var newSettings = new SynchronizationSettings();
                        Properties.Settings.Default.Bim360SynchronizationSettings = newSettings.ToJson();
                        Properties.Settings.Default.Save();
                        synchronizationSettings = newSettings;
                    }
                }
                return synchronizationSettings;
            }
        }
    }
}
