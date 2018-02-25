using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal delegate void UpdateVersion(FileSubscriber subscriber);

    internal class CallUpdateVersionEvents : EventArgs
    {
        internal event UpdateVersion CallUpdateVersion;
        
        internal void Call(FileSubscriber subscriber)
        {
            if (CallUpdateVersion == null)
                return;

            CallUpdateVersion(subscriber);
        }
    }
}
