using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ControlModifiedFiles
{
    public class ListVersion : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _checked;
        private string _path;
        private string _fileName;
        private DateTime _dateModified;

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

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public DateTime DateModified
        {
            get { return _dateModified; }
            set
            {
                if (_dateModified != value)
                {
                    _dateModified = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ListVersion(string path, string fileName, DateTime dateModified)
        {
            _path = path;
            _fileName = fileName;
            _dateModified = dateModified;
        }

    }
}
