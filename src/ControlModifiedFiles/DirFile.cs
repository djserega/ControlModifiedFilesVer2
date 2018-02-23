using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal static ulong GetFileSize(string path) => CalculateFileSize(path);

        internal static Tuple<DateTime, DateTime> GetDateCreateEdited(string path) => DateCreateEdited(path);

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

        internal static DateTime CompareDatePlus(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) == 1)
                date1 = date2;

            return date1;
        }

        internal static DateTime CompareDateMinus(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) == -1)
                date1 = date2;
            return date1;
        }

        internal static string LoadFile(string path)
        {
            string data;
            using (StreamReader streamReader = new StreamReader(path))
            {
                data = streamReader.ReadToEnd();
            }
            return data;
        }

        internal static string GetFileName(string fileName)
        {
            return new FileInfo(fileName).Name;
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

        private static Tuple<DateTime, DateTime> DateCreateEdited(string path)
        {

            DateTime dateCreate = DateTime.MaxValue;
            DateTime dateEdit = DateTime.MinValue;

            foreach (string files in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new FileInfo(files);
                dateCreate = CompareDatePlus(dateCreate, fileInfo.CreationTime);
                dateEdit = CompareDateMinus(dateEdit, fileInfo.LastWriteTime);
            }

            return new Tuple<DateTime, DateTime>(dateCreate, dateEdit);
        }

        #endregion

    }
}
