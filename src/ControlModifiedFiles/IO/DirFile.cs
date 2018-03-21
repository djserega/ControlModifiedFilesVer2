using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace ControlModifiedFiles
{
    internal static class DirFile
    {

        #region internal methods

        internal static string[] SelectNewFiles(Window owner)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = UserSettings.GetFilterFiles(),
                FilterIndex = 0,
                Multiselect = true,
                Title = "Выбор файла(ов) контроля",
                CheckFileExists = true,
                CheckPathExists = true
            };

            bool? result = openFile.ShowDialog(owner);

            if (result.HasValue && result.Value)
                return openFile.FileNames;
            else
                return null;
        }

        internal static string SelectFile(string title, string filter)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = filter,
                FilterIndex = 0,
                Multiselect = false,
                Title = title,
                CheckFileExists = true,
                CheckPathExists = true
            };

            bool? result = openFile.ShowDialog();

            if (result.HasValue && result.Value)
                return openFile.FileName;
            else
                return null;
        }

        internal static ulong GetFileSize(string path) => CalculateFileSize(path);

        internal static string GetSizeFormat(ulong size)
        {

            if (size < 1024)
                return (size).ToString("F0") + " bytes";

            else if ((size >> 10) < 1024)
                return (size / (float)1024).ToString("F1") + " KB";

            else if ((size >> 20) < 1024)
                return ((size >> 10) / (float)1024).ToString("F1") + " MB";

            else if ((size >> 30) < 1024)
                return ((size >> 20) / (float)1024).ToString("F1") + " GB";

            else if ((size >> 40) < 1024)
                return ((size >> 30) / (float)1024).ToString("F1") + " TB";

            else if ((size >> 50) < 1024)
                return ((size >> 40) / (float)1024).ToString("F1") + " PB";

            else
                return ((size >> 50) / (float)1024).ToString("F0") + " EB";

        }

        internal static string GetFileName(string fileName)
            => new FileInfo(fileName).Name;

        internal static string GetExtension(string fileName)
            => new FileInfo(fileName).Extension;

        internal static string GetTempFile()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Temp",
                $"controlmodifiedfiles_{Guid.NewGuid().ToString()}.tmp");
        }

        internal static void DeleteFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                Errors.Save("В метод удаления передан пустой объект.");
                return;
            };

            fileInfo.Refresh();
            if (fileInfo.Exists)
                fileInfo.Delete();
        }

        internal static bool FileExists(string fileName)
        {
            return new FileInfo(fileName).Exists;
        }

        #endregion

        #region private methods

        private static ulong CalculateFileSize(string path)
        {
            return (ulong)new FileInfo(path).Length;
        }

        private static ulong CalculateSize(string path)
        {
            ulong size = 0;

            foreach (string files in Directory.GetFiles(path))
                size += (ulong)new FileInfo(files).Length;

            foreach (string dir in Directory.GetDirectories(path))
                size += CalculateSize(dir);

            return size;
        }

        #endregion

    }
}
