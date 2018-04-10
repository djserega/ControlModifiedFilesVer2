using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для Info.xaml
    /// </summary>
    public partial class FormInfo : Window
    {
        private MainWindow _owner;
        private double _minLeft;
        private double _maxLeft;
        private double _minTop;
        private double _maxTop;

        public FormInfo()
        {
            InitializeComponent();
        }

        private void WindowInfo_Loaded(object sender, RoutedEventArgs e)
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

            TextBlockHeaderInfo.Text = TextBlockHeaderInfo.Text.Replace(
                "%1",
                assemblyName.Name);

            TextBlockVersion.Text = TextBlockVersion.Text.Replace(
                "%1",
                assemblyName.Version.ToString());


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

        private void HyperLinkInfo_Click(object sender, RoutedEventArgs e)
        {
            OpenHyperLink(sender as Hyperlink);
        }

        private void HyperLinkProfileInfostart_Click(object sender, RoutedEventArgs e)
        {
            OpenHyperLink(sender as Hyperlink);
        }

        private void HyperLinkProfileGitHub_Click(object sender, RoutedEventArgs e)
        {
            OpenHyperLink(sender as Hyperlink);
        }

        private void OpenHyperLink(Hyperlink hyperlink)
        {
            Process.Start(hyperlink.NavigateUri.ToString());
        }

        private void WindowInfo_LocationChanged(object sender, System.EventArgs e)
        {
            SetPositionWindow();
        }

        private void WindowInfo_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (_dragDrop)
            //    if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            //    {
            //        SetPositionWindow();
            //    }
        }

        private void WindowInfo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dragDrop = true;
        }

        private void WindowInfo_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dragDrop = false;
        }
    }
}
