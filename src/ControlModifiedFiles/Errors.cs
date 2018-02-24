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

        private static string _fileName = GetFileNameFileError();
      
        internal static void Save(string message)
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

                writer.WriteLine(GetTextMessageError(message));
                writer.Flush();
            }
        }

        internal static void Save(Exception ex)
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

                writer.WriteLine(GetTextMessageError(ex));
                writer.Flush();
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

    }
}
