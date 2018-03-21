using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
