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
        private int _version;
        private DateTime _dateMaxVersion;


        [Column("Выбрано", IsOnlyRead = false)]
        public bool Checked { get; set; }

        [Column("Путь к файлу", Visible = false)]
        public string Path { get; set; }

        [Column("Имя файла", Visible = false)]
        public string FileName { get; set; }

        [Column("Размер (byte)", Visible = false)]
        public ulong Size { get; set; }

        [Column("Размер", SortMemberPath = "Size")]
        public string SizeString { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

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
                Version = versions.GetVersion();
                DateMaxVersion = versions.DateVersion;
            }
        }

    }
}
