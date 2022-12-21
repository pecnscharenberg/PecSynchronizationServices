/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System.Collections.Generic;

namespace PecSynchronizationServices
{
    public interface IFolder
    {
        string GetName();

        IList<IFolder> GetFolders();

        IList<IFile> GetFiles();

        bool FileExists(string fileName);
    }
}
