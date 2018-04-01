using System;

namespace ControlModifiedFiles
{
    internal delegate void UpdateVersion(FileSubscriber subscriber, bool NeedNotified);

    internal class CallUpdateVersionEvents : EventArgs
    {
        internal event UpdateVersion CallUpdateVersion;

        internal bool NeedNotified { get; set; }
        
        internal void Call(FileSubscriber subscriber)
        {
            if (CallUpdateVersion == null)
                return;

            CallUpdateVersion(subscriber, NeedNotified);
        }

    }


    internal delegate void UseContextMenu();

    internal class UseContextMenuEvents : EventArgs
    {
        internal event UseContextMenu CallUseContextMenu;

        internal bool AddFiles { get; private set; }

        internal void CallAddFiles()
        {
            if (CallUseContextMenu == null)
                return;

            AddFiles = true;

            CallUseContextMenu();

            AddFiles = false;
        }


        internal bool WindowsStateNormal { get; private set; }

        internal void CallWindowsStateNormal()
        {
            if (CallUseContextMenu == null)
                return;

            WindowsStateNormal = true;

            CallUseContextMenu();

            WindowsStateNormal = false;
        }


        internal bool ExitApp { get; private set; }

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
