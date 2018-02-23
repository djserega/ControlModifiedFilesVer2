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
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        internal string TextMessage { get; set; }
        internal int Timer { get; set; }
        private string _TextButtonOK;

        public Message()
        {
            InitializeComponent();
        }
        public Message(string textMessage)
        {
            InitializeComponent();

            TextMessage = textMessage;
        }

        public Message(string textMessage, int timer)
        {
            InitializeComponent();

            TextMessage = textMessage;
            Timer = timer;
        }

        private void WindowMessage_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockMessage.Text = TextMessage;

            _TextButtonOK = ButtonOK.Content.ToString();

            if (Timer > 0)
                StartTimerAsync();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void StartTimerAsync()
        {
            ButtonOK.Content = _TextButtonOK + $" ({Timer} с.)";

            Timer--;

            if (Timer > 0)
            {
                await OtherMethods.StartTimerPause();
                StartTimerAsync();
            }
        }
    }
}
