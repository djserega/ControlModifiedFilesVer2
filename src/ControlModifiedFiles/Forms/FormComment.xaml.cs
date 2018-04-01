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
        public bool ClickOK { get; private set; }

        private FileSubscriber _subscriber;
        public int Version { get; private set; }

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
    }
}
