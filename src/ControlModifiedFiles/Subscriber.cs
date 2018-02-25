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
        private List<FileInfo> listAction = new List<FileInfo>();

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
                FileInfo fileInfo = new FileInfo(e.FullPath);

                var keyAction = listAction.FirstOrDefault(f => f == fileInfo);

                if (keyAction != null)
                    return;

                lock (_locker)
                {
                    listAction.Add(fileInfo);

                    int ind = 0;

                    if (!WaitTryCopyVersion(fileInfo, ref ind))
                    {
                        Errors.Save(new Exception($"Не удалось получить доступ к файлу {fileInfo.FullName}"));
                        listAction.Remove(fileInfo);
                        return;
                    }

                    FileSubscriber subscriber = _listSubscriber.First(f => f.Key.FullName == fileInfo.FullName).Value;

                    using (Versions versions = new Versions()
                    {
                        SubscriberInfo = fileInfo,
                        Subscriber = subscriber
                    })
                    {
                        versions.CreateNewVersionFile();
                    }

                    _callUpdate.Call(subscriber);

                    listAction.Remove(fileInfo);

                    fileInfo = null; 
                }

            }
            catch (Exception ex)
            {
                Errors.Save(ex);
            }
        }

        private bool WaitTryCopyVersion(FileInfo fileInfo, ref int ind)
        {
            if (ind > 3)
                return false;
            try
            {
                fileInfo.OpenRead();
                return true;
            }
            catch (Exception)
            {
                //Thread.Sleep(1 * 1000);
                ind++;
                return WaitTryCopyVersion(fileInfo, ref ind);
            }
        }

        #endregion

    }
}
