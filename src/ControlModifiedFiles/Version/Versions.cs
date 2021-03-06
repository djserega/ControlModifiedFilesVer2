﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class Versions : IDisposable
    {

        #region Fields

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
            int? maxVersion = GetMaxVersionFile();

            if (maxVersion == null)
                return;

            maxVersion++;

            string fileNameWithoutExtension = FileName.Remove(FileName.Length - Extension.Length);

            string newVersion = $"{fileNameWithoutExtension} {Constants.prefixVersion} {maxVersion}{Constants.postfixVersion}{Extension}";
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

        internal List<ListVersion> GetListVersion()
        {
            List<CommentsVersion> commentsVersions = new Comments(Subscriber, SubscriberInfo).GetListComments();

            List<ListVersion> list = new List<ListVersion>();

            foreach (FileInfo item in GetFilesInDirectoryVersion())
            {
                ListVersion listVersion = new ListVersion(
                    item.FullName,
                    item.Name,
                    item.LastWriteTime,
                    commentsVersions.Find(f => f.FileName == item.Name));

                list.Add(listVersion);
            }

            list.Sort((el1, el2) => el2.DateModified.CompareTo(el1.DateModified));

            return list;
        }

        internal void SetCommentFile(int version, string textComment)
        {
            if (Subscriber == null)
                return;

            FileInfo[] filesVersion = GetFilesInDirectoryVersion(version);

            if (filesVersion.Length > 0)
            {
                new Comments(Subscriber, filesVersion[0]).UpdateCommentFile(textComment);
            }
        }

        internal void RestoreVersion(ListVersion version)
        {
            if (Subscriber == null)
                return;

            FileInfo fileInfoVersion = new FileInfo(version.Path);
            if (!DirFile.FileExists(fileInfoVersion))
                return;

            FileInfo tempFileVersion = new FileInfo(DirFile.GetTempFile());

            fileInfoVersion.CopyTo(tempFileVersion.FullName);
            tempFileVersion.LastWriteTime = DateTime.Now;

            FileInfo fileInfoRestore = new FileInfo(Subscriber.Path);

            tempFileVersion.CopyTo(fileInfoRestore.FullName, true);

            DirFile.DeleteFile(tempFileVersion);
        }

        #endregion

        #region Private methods

        private int? GetMaxVersionFile(bool getVersion = false)
        {
            string currentHash = GetMD5(SubscriberInfo.FullName);

            FileInfo fileInfoMaxEdited = null;
            DateTime dateTimeMaxEdited = DateTime.MinValue;
            foreach (FileInfo file in GetFilesInDirectoryVersion())
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

            return DirFile.GetNumberVersion(fileInfoMaxEdited.Name);
        }

        private string GetMD5(string path)
        {
            string hash = String.Empty;

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

            return hash;
        }

        private DirectoryInfo GetDirectoryInfoSubscriber()
            => new DirectoryInfo(Subscriber.DirectoryVersion);

        private FileInfo[] GetFilesInDirectoryVersion()
            => GetDirectoryInfoSubscriber().GetFiles($"*{Constants.prefixVersion}*{Constants.postfixVersion}.*");

        private FileInfo[] GetFilesInDirectoryVersion(int version)
            => GetDirectoryInfoSubscriber().GetFiles($"*{Constants.prefixVersion} {version}*{Constants.postfixVersion}.*");

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
