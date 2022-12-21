using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PecSynchronizationServices
{
    public interface IFile : IEquatable<IFile>
    {
        string GetName();

        long GetSize();

        DateTime GetLastModifiedUtc();

        byte[] GetFile();
    }
}
