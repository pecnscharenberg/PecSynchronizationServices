/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System.Collections.Generic;

namespace PecSynchronizationServices
{
    public struct SyncResult
    {
        public IList<string> Successes { get; }

        public IList<string> Failures { get; }

        public SyncResult(IList<string> successes, IList<string> failures)
        {
            Successes = successes;
            Failures = failures;
        }
    }
}
