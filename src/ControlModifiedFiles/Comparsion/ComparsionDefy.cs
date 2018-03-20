using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class ComparsionDefy
    {
        internal bool ProgramInstalled { get; }

        internal string Path { get; }

        internal ComparsionDefy()
        {
            Path = UserSettings.GetUserSettings("PathDefy");

            if (!string.IsNullOrWhiteSpace(Path))
            {
                if (DirFile.FileExists(Path))
                    ProgramInstalled = true;
                else
                    ProgramInstalled = false;
            }
            else
                ProgramInstalled = false;
        }

        internal string SelectFile()
            => DirFile.SelectFile("Файл программы Defy", "Defy.exe|Defy.exe");

        internal void CompareVersion(string path1, string path2)
        {
            if (!string.IsNullOrWhiteSpace(path1)
                && !string.IsNullOrWhiteSpace(path2))
                Process.Start(Path, $" compare \"{path1}\" \"{path2}\"");
        }

    }
}
