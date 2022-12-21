/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System.Collections.Generic;

namespace PecSynchronizationServices
{
    public interface ISynchronizationGroup
    {
        IList<ISynchronizationItem> SynchronizationItems { get; }
    }
}
