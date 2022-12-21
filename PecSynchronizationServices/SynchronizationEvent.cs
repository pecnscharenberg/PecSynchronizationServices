using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PecSynchronizationServices
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
}
