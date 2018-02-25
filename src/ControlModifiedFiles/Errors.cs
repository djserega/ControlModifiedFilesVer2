using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal static class Errors
    {

        #region Fields

        private static string _fileName = GetFileNameFileError();
        private static readonly object _locker = new object();

        #endregion

        #region Internal methods

        internal static void Save(string message) => SaveError(GetTextMessageError(message));

        internal static void Save(Exception ex) => SaveError(GetTextMessageError(ex));

        #endregion

        #region Private methods

        private static void SaveError(string message)
        {
            lock (_locker)
            {
                bool writeHeader = false;

                FileInfo fileInfo = new FileInfo(_fileName);
                if (!fileInfo.Exists)
                {
                    fileInfo.Create();
                    writeHeader = true;
                }

                using (StreamWriter writer = fileInfo.AppendText())
                {
                    if (writeHeader)
                        writer.WriteLine(GetTextMessageError());

                    writer.WriteLine(message);
                    writer.Flush();
                }
            }
        }

        private static string GetFileNameFileError()
        {
            return Path.Combine(
                Environment.CurrentDirectory,
                $"errors{DateTime.Now.ToString("yyyyMMdd")}.txt");
        }

        private static string GetTextMessageError()
        {
            return "Time ; Message ; Source ; StackTrace";
        }

        private static string GetTextMessageError(string message)
        {
            char separator = ';';

            return $"{DateTime.Now.ToString("HH:mm:ss")}" +
                $" {separator} " +
                $"{message}" +
                $" {separator} " +
                $" - " +
                $" {separator} " +
                $" - ";
        }

        private static string GetTextMessageError(Exception ex)
        {
            char separator = ';';

            return $"{DateTime.Now.ToString("HH:mm:ss")}" +
                $" {separator} " +
                $"{ex.Message}" +
                $" {separator} " +
                $"{ex.Source}" +
                $" {separator} " +
                $"{ex.StackTrace}";
        }

        #endregion

    }
}
