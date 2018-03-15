using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ControlModifiedFiles
{
    public class FileSubscriber : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private fields

        private bool _checked;
        private string _path;
        private ulong _size;
        private string _sizeString;
        private int _version;
        private DateTime _dateMaxVersion;

        #endregion

        #region Properties

        [Column("Выбрано", IsOnlyRead = false)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Column("Путь к файлу", Visible = false)]
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Column("Имя файла", Visible = false)]
        public string FileName { get; set; }

        [Column("Размер (byte)", Visible = false)]
        public ulong Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Column("Размер", SortMemberPath = "Size")]
        public string SizeString
        {
            get { return _sizeString; }
            set
            {
                if (_sizeString != value)
                {
                    _sizeString = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Column("№ предыдущей версии", Visible = false)]
        public int PreviousVersion { get; set; }

        [Column("№ версии")]
        public int Version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Column("Дата изменения")]
        public DateTime DateMaxVersion
        {
            get { return _dateMaxVersion; }
            set
            {
                if (_dateMaxVersion != value)
                {
                    _dateMaxVersion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Column("Каталог версий", Visible = false)]
        public string DirectoryVersion { get; set; }

        [Column("Версии без уведомлений", Visible = false)]
        public int CountVersionWithoutNotify { get; set; }

        #endregion

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void SetCurrentVersion()
        {
            using (Versions versions = new Versions()
            {
                SubscriberInfo = new FileInfo(Path),
                Subscriber = this
            })
            {
                PreviousVersion = _version;
                Version = versions.GetVersion();
                DateMaxVersion = versions.DateVersion;
            }
        }

        internal void SetCurrentSize()
        {
            Size = DirFile.GetFileSize(_path);
            SizeString = DirFile.GetSizeFormat(_size);
        }
    }
}
