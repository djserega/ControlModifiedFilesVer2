using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class V8Viewer
    {

        internal bool V8VieverInstalled { get; }
        internal string Path { get; }

        internal V8Viewer()
        {
            try
            {
                RegistryKey keyV8Viewer = GetRegistryKey();

                if (keyV8Viewer.GetValue("0") is string[] stringValue
                    && stringValue.Count() > 1)
                {
                    Path = stringValue[1];
                    V8VieverInstalled = true;
                }
                else
                    V8VieverInstalled = false;
            }
            catch (Exception)
            {
                V8VieverInstalled = false;
            }
        }

        private RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\UFH\SHC\", false);
        }

        internal void CompareVersion(ICollection<ListVersion> listVersion)
        {
            string path1 = string.Empty;
            string path2 = string.Empty;

            foreach (ListVersion item in listVersion)
            {
                if (!string.IsNullOrWhiteSpace(path1)
                    && !string.IsNullOrWhiteSpace(path2))
                    break;

                if (item.Checked)
                {
                    if (string.IsNullOrWhiteSpace(path1))
                        path1 = item.Path;
                    else
                        path2 = item.Path;
                }
            }

            if (!string.IsNullOrWhiteSpace(path1)
                && !string.IsNullOrWhiteSpace(path2))
                Process.Start("C:\\Program Files (x86)\\V8 Viewer\\v8viewer.exe", $" -diff \"{path1}\" \"{path2}\"");
        }

    }
}
