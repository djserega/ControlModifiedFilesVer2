using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class Versions : IDisposable
    {

        #region Fields

        private string _prefixVersion = "{version";
        private string _postfixVersion = "}";

        private List<FileInfo> _tempFiles = new List<FileInfo>();
        private static readonly object _locker = new object();

        #endregion

        #region Properties

        internal FileInfo SubscriberInfo { get; set; }
        internal FileSubscriber Subscriber { get; set; }
        internal string FileName { get; set; }
        internal string Extension { get; set; }

        internal DateTime DateVersion { get; private set; }

        #endregion

        #region Internal methods

        internal void CreateNewVersionFile()
        {
            string md5CurrentFile = GetMD5(SubscriberInfo.FullName);

            int? maxVersion = GetMaxVersionFile();

            if (maxVersion == null)
                return;

            maxVersion++;

            string fileNameWithoutExtension = FileName.Remove(FileName.Length - Extension.Length);

            string newVersion = $"{fileNameWithoutExtension} {_prefixVersion} {maxVersion}{_postfixVersion}{Extension}";
            string newFileName = Path.Combine(
                Subscriber.DirectoryVersion,
                newVersion);

            if (!new FileInfo(newFileName).Exists)
                SubscriberInfo.CopyTo(newFileName);
        }

        internal void CreateDirectoryVersion()
        {
            DirectoryInfo globalDirectoryVersion = SubscriberInfo.Directory.CreateSubdirectory("_version");

            string nameDirectoryVersionFile = "";
            if (UserSettings.GetUserSettings("UsePrefixUserName"))
                nameDirectoryVersionFile += Environment.UserName + "_";

            nameDirectoryVersionFile += SubscriberInfo.Name;

            DirectoryInfo directoryVersion = globalDirectoryVersion.CreateSubdirectory(nameDirectoryVersionFile);

            Subscriber.DirectoryVersion = directoryVersion.FullName;
        }

        internal int GetVersion()
        {
            int? maxVersion = GetMaxVersionFile(true);
            if (maxVersion == null)
                return 0;
            return (int)maxVersion;
        }

        #endregion

        #region Private methods

        private int? GetMaxVersionFile(bool getVersion = false)
        {
            string currentHash = GetMD5(SubscriberInfo.FullName);

            DirectoryInfo directoryVersion = new DirectoryInfo(Subscriber.DirectoryVersion);

            FileInfo fileInfoMaxEdited = null;
            DateTime dateTimeMaxEdited = DateTime.MinValue;
            foreach (FileInfo file in directoryVersion.GetFiles())
            {

                if (dateTimeMaxEdited <= file.LastWriteTime)
                {
                    fileInfoMaxEdited = file;
                    dateTimeMaxEdited = file.LastWriteTime;
                };

            }

            DateVersion = dateTimeMaxEdited;

            if (fileInfoMaxEdited == null)
                return 0;

            if (!getVersion)
                if (GetMD5(fileInfoMaxEdited.FullName) == currentHash)
                    return null;

            return int.Parse(fileInfoMaxEdited.Name.Substring($"{_prefixVersion} ", "}"));
        }

        private string GetMD5(string path)
        {
            string hash = String.Empty;

            //lock (_locker)
            {
                try
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        string fileNameTemp = DirFile.GetTempFile();

                        FileInfo fileInfoTemp = new FileInfo(fileNameTemp);
                        if (!fileInfoTemp.Exists)
                        {
                            new FileInfo(path).CopyTo(fileNameTemp);
                            _tempFiles.Add(fileInfoTemp);
                        }

                        using (FileStream stream = new FileStream(fileNameTemp, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            byte[] hashByte = md5.ComputeHash(stream);
                            hash = BitConverter.ToString(hashByte).Replace("-", "").ToLowerInvariant();
                            stream.Close();
                        }

                        md5.Clear();
                    }
                }
                catch (FileNotFoundException)
                {
                    Errors.Save($"Файл '{path}' перемещен или удален.");
                }
                catch (IOException ex)
                {
                    Errors.Save(ex);
                }
            }

            return hash;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SubscriberInfo = null;
                    Subscriber = null;
                }

                foreach (FileInfo item in _tempFiles)
                {
                    try
                    {
                        DirFile.DeleteFile(item);
                    }
                    catch (Exception ex)
                    {
                        Errors.Save($"Не удалось удалить временный файл: {item.FullName}");
                        Errors.Save(ex);
                    }
                }
                _tempFiles.Clear();
                _tempFiles = null;

                disposedValue = true;
            }
        }

        ~Versions()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
