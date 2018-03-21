using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;

namespace ControlModifiedFiles
{
    internal class ComparsionV8Viewer
    {

        internal bool ProgramInstalled { get; }

        internal string Path { get; }

        internal ComparsionV8Viewer()
        {
            try
            {
                RegistryKey keyV8Viewer = GetRegistryKey();

                if (keyV8Viewer.GetValue("0") is string[] stringValue
                    && stringValue.Count() > 1)
                {
                    Path = stringValue[1];
                    ProgramInstalled = true;
                }
                else
                    ProgramInstalled = false;
            }
            catch (Exception)
            {
                ProgramInstalled = false;
            }
        }

        private RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\UFH\SHC\", false);
        }

        internal void CompareVersion(string path1, string path2)
        {
            if (!string.IsNullOrWhiteSpace(path1)
                && !string.IsNullOrWhiteSpace(path2))
                Process.Start("C:\\Program Files (x86)\\V8 Viewer\\v8viewer.exe", $" -diff \"{path1}\" \"{path2}\"");
        }

    }
}
