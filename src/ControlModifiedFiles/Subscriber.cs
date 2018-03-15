using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class Subscriber
    {

        #region Fields

        private CallUpdateVersionEvents _callUpdate;

        private Dictionary<FileInfo, FileSubscriber> _listSubscriber = new Dictionary<FileInfo, FileSubscriber>();
        private Dictionary<FileSubscriber, FileSystemWatcher> _listWather = new Dictionary<FileSubscriber, FileSystemWatcher>();

        private static readonly object _locker = new object();

        #endregion

        #region Internal methods

        internal Subscriber(CallUpdateVersionEvents callUpdate)
        {
            _callUpdate = callUpdate;
        }

        internal void Subscribe(FileSubscriber subscriber)
        {
            if (subscriber.Checked
                && !String.IsNullOrWhiteSpace(subscriber.Path))
            {
                if (_listWather.FirstOrDefault(f => f.Key == subscriber).Key != null)
                    return;

                FileInfo fileInfo = new FileInfo(subscriber.Path);

                FileSystemWatcher watcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name)
                {
                    NotifyFilter = NotifyFilters.LastWrite
                };

                _listSubscriber.Add(fileInfo, subscriber);

                watcher.Changed += Subscribe_Changed;
                watcher.EnableRaisingEvents = true;

                _listWather.Add(subscriber, watcher);

                using (Versions versions = new Versions()
                {
                    SubscriberInfo = fileInfo,
                    Subscriber = subscriber
                })
                {
                    versions.CreateDirectoryVersion();
                }
                _callUpdate.Call(subscriber);
            }
        }

        internal void Unsubscribe(FileSubscriber subscriber)
        {
            var keyWather = _listWather.FirstOrDefault(f => f.Key == subscriber);
            if (keyWather.Key == null)
                return;

            FileSystemWatcher watcher = keyWather.Value;

            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            _listWather.Remove(subscriber);
        }

        #endregion

        #region Private methods

        private void Subscribe_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                Thread.Sleep(1000);

                FileInfo fileInfo = new FileInfo(e.FullPath);

                FileSubscriber subscriber = _listSubscriber.First(f => f.Key.FullName == fileInfo.FullName).Value;

                string tempFileName = DirFile.GetTempFile();
                FileInfo fileInfoTemp = fileInfo.CopyTo(tempFileName);
                string fileName = fileInfo.Name;
                string extension = fileInfo.Extension;
                fileInfo = null;

                Task.Run(() =>
                {
                    lock (_locker)
                    {
                        using (Versions versions = new Versions()
                        {
                            SubscriberInfo = fileInfoTemp,
                            Subscriber = subscriber,
                            FileName = fileName,
                            Extension = extension
                        })
                        {
                            versions.CreateNewVersionFile();
                        }
                        
                        _callUpdate.Call(subscriber);

                        DirFile.DeleteFile(fileInfoTemp);

                        fileInfoTemp = null;
                    }
                });

            }
            catch (Exception ex)
            {
                Errors.Save(ex);
            }
        }

        #endregion

    }
}
