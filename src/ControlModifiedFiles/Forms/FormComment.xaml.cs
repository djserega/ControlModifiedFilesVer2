using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для FormComment.xaml
    /// </summary>
    public partial class FormComment : Window
    {
        private MainWindow _owner;
        private double _minLeft;
        private double _maxLeft;
        private double _minTop;
        private double _maxTop;

        public bool ClickOK { get; private set; }

        private FileSubscriber _subscriber;
        public int Version { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
            TextBoxComment.Focus();

            _owner = (MainWindow)Owner;
            _minLeft = _owner.Left + 10;
            _maxLeft = _owner.Left + _owner.Width - 10 - Width;
            _minTop = _owner.Top + 10;
            _maxTop = _owner.Top + _owner.Height - 10 - Height;

            SetPositionWindow();
        }

        private void SetPositionWindow()
        {
            if (Left < _minLeft)
                Left = _minLeft;
            else if (Left > _maxLeft)
                Left = _maxLeft;

            if (Top < _minTop)
                Top = _minTop;
            else if (Top > _maxTop)
                Top = _maxTop;
        }

        public FormComment(FileSubscriber subscriber)
        {
            InitializeComponent();

            _subscriber = subscriber;
            Version = _subscriber.Version;

            TextBlockHeader.Text = TextBlockHeader.Text.Replace("%1", Version.ToString());
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            ClickOK = true;
            Close();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            SetPositionWindow();
        }
    }
}
