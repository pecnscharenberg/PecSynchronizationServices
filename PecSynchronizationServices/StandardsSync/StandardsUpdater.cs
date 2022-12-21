/*
 * Copyright (C) 2020 Pheinex LLC
 */

using PecForgeApi;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PecSynchronizationServices.StandardsSync
{
    public delegate void SynchronizationEventHandler(object sender, SynchronizationEventArgs e);

    public class SynchronizationEventArgs : EventArgs
    {
        public string Message { get; }

        public SynchronizationEventArgs(string message)
        {
            Message = message;
        }
    }

    public class StandardsUpdater
    {
        public static StandardsUpdater SharedInstance { get; } = new StandardsUpdater();

        public event SynchronizationEventHandler SynchronizationEvent;

        public bool IsSynchronizing { get; private set; }

        private StandardsUpdater() { }

        public async Task<SyncResult> SynchronizeFilesAsync()
        {
            return await Task.Run(() =>
            {
                return SyncronizeFiles();
            });
        }

        public SyncResult SyncronizeFiles()
        {
            lock (this)
            {
                if (IsSynchronizing)
                {
                    OnSynchronization("Synchronization already in progress.");
                    return new SyncResult();
                }
                else
                {
                    IsSynchronizing = true;
                }
            }

            try
            {
                OnSynchronization("Starting Syncronization...");
                var syncItems = Defaults.SharedInstance.SynchronizationSettings.GetSyncItems();
                OnSynchronization($"Syncronizing {syncItems.Length} locations.");
                var syncGroup = new Bim360SynchronizationGroup(syncItems);
                var synchronizer = new Bim360Synchronizer(syncGroup);

                try
                {
                    synchronizer.SynchronizationEvent += Synchronizer_SynchronizationEvent;
                    return synchronizer.Synchronize();
                }
                finally
                {
                    OnSynchronization("Done.");
                    synchronizer.SynchronizationEvent -= Synchronizer_SynchronizationEvent;
                }
            }
            finally
            {
                IsSynchronizing = false;
            }
        }

        private void Synchronizer_SynchronizationEvent(object sender, PecSynchronizationServices.SynchronizationEventArgs e)
        {
            Debug.Print(e.Message);
            OnSynchronization(e.Message);
        }

        private void OnSynchronization(string message)
        {
            SynchronizationEvent?.Invoke(this, new SynchronizationEventArgs(message));
        }
    }
}
