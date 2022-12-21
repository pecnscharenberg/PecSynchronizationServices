/*
 * Copyright (C) 2020 Pheinex LLC
 */

namespace PecSynchronizationServices
{
    public interface ISynchronizer
    {
        event SynchronizationEventHandler SynchronizationEvent;

        SyncResult Synchronize();
    }
}
