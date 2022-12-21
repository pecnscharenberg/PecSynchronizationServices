/*
 * Copyright (C) 2020 Pheinex LLC
 */

using System;
using System.IO;

namespace PecSynchronizationServices
{
    public class LocalFile : IFile
    {
        public string Path { get; }

        public LocalFile(string path)
        {
            Path = System.IO.Path.GetFullPath(path);
        }

        public string GetName()
        {
            return System.IO.Path.GetFileName(Path);
        }

        public long GetSize()
        {
            var fileInfo = new FileInfo(Path);
            return fileInfo.Length;
        }

        public DateTime GetLastModifiedUtc()
        {
            return File.GetLastWriteTimeUtc(Path);
        }

        public byte[] GetFile()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IFile other)
        {
            if (other is LocalFile otherLocalFile)
            {
                return otherLocalFile.Path.ToLowerInvariant() == Path.ToLowerInvariant();
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
            return Path.ToLowerInvariant().GetHashCode();
        }
    }
}
