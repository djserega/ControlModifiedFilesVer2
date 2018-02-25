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
        private string _prefixVersion = "{version";
        private string _postfixVersion = "}";

        internal FileInfo SubscriberInfo { get; set; }
        internal FileSubscriber Subscriber { get; set; }

        internal DateTime DateVersion { get; private set; }

        List<FileInfo> _tempFiles = new List<FileInfo>();

        private static readonly object _locker = new object();

        internal void CreateNewVersionFile()
        {
            string md5CurrentFile = GetMD5(SubscriberInfo.FullName);

            int? maxVersion = GetMaxVersionFile();

            if (maxVersion == null)
                return;

            maxVersion++;

            string extension = SubscriberInfo.Extension;
            string fileNameWithoutExtension = SubscriberInfo.Name.Remove(SubscriberInfo.Name.Length - extension.Length);

            string newVersion = $"{fileNameWithoutExtension} {_prefixVersion} {maxVersion}{_postfixVersion}.{extension}";

            SubscriberInfo.CopyTo(Path.Combine(
                Subscriber.DirectoryVersion,
                newVersion));
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

        private int? GetMaxVersionFile(bool getVersion = false)
        {
            string currentHash = GetMD5(SubscriberInfo.FullName);

            List<string> listMD5Version = new List<string>();

            DirectoryInfo directoryVersion = new DirectoryInfo(Subscriber.DirectoryVersion);

            FileInfo fileInfoMaxEdited = null;
            DateTime dateTimeMaxEdited = DateTime.MinValue;
            foreach (FileInfo file in directoryVersion.GetFiles())
            {
                string md5VersionFile = GetMD5(file.FullName);
                listMD5Version.Add(md5VersionFile);
                if (!String.IsNullOrWhiteSpace(md5VersionFile))
                {
                    if (dateTimeMaxEdited <= file.LastWriteTime)
                    {
                        fileInfoMaxEdited = file;
                        dateTimeMaxEdited = file.LastWriteTime;
                    };
                }
            }

            DateVersion = dateTimeMaxEdited;

            if (fileInfoMaxEdited == null)
                return null;

            if (!getVersion)
                if (listMD5Version.FirstOrDefault(f => f == currentHash) != null)
                    return null;

            return int.Parse(fileInfoMaxEdited.Name.Substring($"{_prefixVersion} ", "}"));
        }

        private string GetMD5(string path)
        {
            string hash = "";

            lock (_locker)

            {
                string fileNameTemp;
                try
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        fileNameTemp = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Temp",
                            $"controlmodifiedfiles_{Guid.NewGuid().ToString()}.tmp");

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
                            stream.Dispose();
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
                        item.Refresh();
                        if (item.Exists)
                            item.Delete();
                    }
                    catch (Exception ex)
                    {
                        Errors.Save($"Не удалось удалить временный файл: {item.FullName}");
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
