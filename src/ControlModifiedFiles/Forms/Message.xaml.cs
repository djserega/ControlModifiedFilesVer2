using System.Windows;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : Window
    {

        #region Fields

        private string _textMessage;
        private bool _question;
        private int _timer;
        private string _textButtonOK;
        private string _textButtonCancel;

        #endregion

        #region Properties

        internal bool PressButtonOK { get; private set; }
        internal bool PressButtonCancel { get; private set; }
        internal bool ClosedByTimeout { get; private set; }

        #endregion

        #region Constructors

        private Message()
        {
            InitializeComponent();
        }

        public Message(string textMessage, bool question = false)
        {
            InitializeComponent();

            _textMessage = textMessage;

            _question = question;
            PrepareForm();
        }

        public Message(string textMessage, int timer, bool question = false)
        {
            InitializeComponent();

            _textMessage = textMessage;
            _timer = timer;

            _question = question;
            PrepareForm();
        }

        #endregion

        #region Window

        private void WindowMessage_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockMessage.Text = _textMessage;

            _textButtonOK = ButtonOK.Content.ToString();
            _textButtonCancel = ButtonCancel.Content.ToString();


            if (_timer > 0)
                StartTimerAsync();
        }

        #endregion

        #region Button

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            PressButtonOK = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            PressButtonCancel = true;
            Close();
        }

        #endregion

        private async void StartTimerAsync()
        {
            if (_question)
                ButtonCancel.Content = _textButtonCancel + $" ({_timer} с.)";
            else
                ButtonOK.Content = _textButtonOK + $" ({_timer} с.)";

            _timer--;

            if (_timer >= 0)
            {
                await OtherMethods.StartTimerPause();
                StartTimerAsync();
            }
            else if (_timer < 0)
            {
                ClosedByTimeout = true;
                Close();
            }
        }

        private void PrepareForm()
        {
            if (_question)
                Title = "Вопрос";
            else
                ButtonCancel.Visibility = Visibility.Collapsed;
        }
    }
}
