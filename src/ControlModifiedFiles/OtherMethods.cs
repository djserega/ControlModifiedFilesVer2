using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal static class OtherMethods
    {
        internal static Task StartTimerPause(int seconds = 1)
        {
            return Task.Run(() => Thread.Sleep(seconds * 1000));
        }

        internal static Task StartTimerPause(double seconds)
        {
            return Task.Run(() => Thread.Sleep((int)(seconds * 1000)));
        }

    }
}