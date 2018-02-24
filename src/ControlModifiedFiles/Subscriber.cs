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
        internal void Subscribe(FileSubscriber subscriber)
        {
            if (subscriber.Checked
                && !String.IsNullOrWhiteSpace(subscriber.Path))
            {
                FileInfo fileInfo = new FileInfo(subscriber.Path);

                FileSystemWatcher watcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name)
                {
                    NotifyFilter = NotifyFilters.LastWrite
                };

                watcher.Changed += Subscribe_Changed;
                watcher.EnableRaisingEvents = true;
            }
        }

        private void Subscribe_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                //Thread.Sleep(2 * 1000);

                //var keyWatcher = DictionaryWatcher.First(f => f.Key.Path == e.FullPath);
                //FileSubscriber file = keyWatcher.Key;

                //FileInfo fileInfo = new FileInfo(file.Path);
                //CreateNewVersionFile(fileInfo, file);

                //var args = new ChangedFileEvent();
                //if (args != null)
                //    foreach (EventHandler<ChangedFileEvent> deleg in ChangeFileEvent.GetInvocationList())
                //        deleg.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Errors.Save(ex);
            }
        }
    }
}
