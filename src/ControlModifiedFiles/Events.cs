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


    internal delegate void UseContextMenu();

    internal class UseContextMenuEvents : EventArgs
    {
        internal event UseContextMenu CallUseContextMenu;

        internal bool AddFiles { get; private set; }
        internal bool WindowsStateNormal { get; private set; }
        internal bool ExitApp { get; private set; }

        internal void CallAddFiles()
        {
            if (CallUseContextMenu == null)
                return;

            AddFiles = true;

            CallUseContextMenu();

            AddFiles = false;
        }
        
        internal void CallWindowsStateNormal()
        {
            if (CallUseContextMenu == null)
                return;

            WindowsStateNormal = true;

            CallUseContextMenu();

            WindowsStateNormal = false;
        }

        internal void CallExitApp()
        {
            if (CallUseContextMenu == null)
                return;

            ExitApp = true;

            CallUseContextMenu();

            ExitApp = false;
        }
    }
}
