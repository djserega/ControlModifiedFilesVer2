using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal delegate void UpdateVersion();

    internal class CallUpdateVersionEvents : EventArgs
    {
        internal event UpdateVersion CallUpdateVersion;
        
        internal void Call()
        {
            if (CallUpdateVersion == null)
                return;

            CallUpdateVersion();
        }
    }
}
