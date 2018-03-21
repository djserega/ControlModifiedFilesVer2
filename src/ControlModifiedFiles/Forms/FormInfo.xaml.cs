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

    }
}
