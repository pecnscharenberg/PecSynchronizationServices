/*
 * Copyright (C) 2020 Pheinex LLC
 */

using PecForgeApi;
using PecSynchronizationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PecSynchronizationServices.StandardsSync
{
    public class UpdateStandardsViewModel : ObservableObject, IDisposable
    {
        private bool inUpdate = false;

        public bool EditSettingsAllowed => !InUpdate;

        public bool InUpdate
        {
            get { return inUpdate; }
            set
            {
                if (value == inUpdate) { return; }

                inUpdate = value;
                OnPropertyChanged(nameof(InUpdate));
                OnPropertyChanged(nameof(UpdateAllowed));
                OnPropertyChanged(nameof(EditSettingsAllowed));
            }
        }

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public bool UpdateAllowed => !InUpdate;

        private Dispatcher Dispatcher { get; }

        public UpdateStandardsViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            InUpdate = StandardsUpdater.SharedInstance.IsSynchronizing;
            if (InUpdate)
            {
                Messages.Add("Synchronization already in progress...");
            }
            StandardsUpdater.SharedInstance.SynchronizationEvent += Updater_SynchronizationEvent;
            InUpdate = StandardsUpdater.SharedInstance.IsSynchronizing;
        }

        public async Task StartUpdate()
        {
            if (InUpdate) { return; }

            try
            {
                InUpdate = true;
                Dispatcher.Invoke(delegate
                {
                    Messages.Clear();
                });

                var result = await StandardsUpdater.SharedInstance.SynchronizeFilesAsync();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(delegate
                {
                    Messages.Add(ex.Message);
                });
            }
            finally
            {
                InUpdate = false;
            }
        }

        private void Updater_SynchronizationEvent(object sender, StandardsSync.SynchronizationEventArgs e)
        {
            if (Dispatcher is Dispatcher dispatcher)
            {
                dispatcher.Invoke(delegate
                {
                    Messages.Add(e.Message);
                    InUpdate = StandardsUpdater.SharedInstance.IsSynchronizing;
                });
            }
            else
            {
                Messages.Add(e.Message);
                InUpdate = StandardsUpdater.SharedInstance.IsSynchronizing;
            }
        }

        public void Dispose()
        {
            StandardsUpdater.SharedInstance.SynchronizationEvent -= Updater_SynchronizationEvent;
        }
    }
}
