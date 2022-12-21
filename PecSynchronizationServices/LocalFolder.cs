/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PecSynchronizationServices
{

    public class LocalFolder : IFolder
    {
        public string FolderPath { get; }

        public LocalFolder(string path)
        {
            FolderPath = Path.GetFullPath(path);
        }

        public string GetName() => FolderPath;

        public IList<IFolder> GetFolders()
        {
            Logging.LogItem($"GetFolders {FolderPath}");
            var folders = Directory.GetDirectories(FolderPath).Select(item => new LocalFolder(item)).ToArray();
            Logging.LogItem($"GetFolders {FolderPath} done");

            return folders;
        }

        public IList<IFile> GetFiles()
        {
            Logging.LogItem($"GetFiles {FolderPath}");
            var contents = Directory.GetFiles(FolderPath);
            Logging.LogItem($"GetFiles {FolderPath} done");
            return contents.Select(item => new LocalFile(item)).ToArray();
        }

        public bool FileExists(string fileName) => File.Exists(Path.Combine(FolderPath, fileName));
    }
}
