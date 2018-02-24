using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Properties

        public static ICollection<FileSubscriber> listFiles = new List<FileSubscriber>();
        private bool _modifiedSettings = false;
        private Dictionary<string, string> _dictionaryNameColumn = new Dictionary<string, string>();

        #endregion

        #region Window

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindowControlModifiedFiles_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeVisiblePanelSettigs();
            CheckBoxDirectoryVersion.IsChecked = UserSettings.GetUserSettings("HiddenColumnDirectoryVersion");
            CheckBoxPath.IsChecked = UserSettings.GetUserSettings("HiddenColumnPath");
            CheckBoxSize.IsChecked = UserSettings.GetUserSettings("HiddenColumnSize");
            ChangeVisibleModifiedSettings();

            SetItemSouce();
        }

        #endregion

        #region Menu button

        private void ButtonAddFiles_Click(object sender, RoutedEventArgs e)
        {
            string[] arrayFiles = DirFile.SelectNewFiles(this);
            if (arrayFiles != null)
                AddFilesInDataGridList(arrayFiles);
        }

        private void ButtonRemoveFiles_Click(object sender, RoutedEventArgs e)
        {
            IList listSelectedFiles = GetCurrentRows();
            if (listSelectedFiles.Count == 0)
                return;


            bool? result = Dialog.ShowQuesttion($"Будет отменен контроль файлов - {listSelectedFiles.Count}.\n" +
                $"Продолжить?");

            if (result == null || !(bool)result)
                return;

            foreach (FileSubscriber item in listSelectedFiles)
            {
                listFiles.Remove(item);
            }

            SetItemSouce(listFiles.ToList());
        }

        #endregion

        #region Settings

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            ChangeVisiblePanelSettigs();
        }

        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SaveUserSettings();
            _modifiedSettings = false;
            ChangeVisiblePanelSettigs();
            ChangeVisibleModifiedSettings();
        }

        private void CheckBoxPath_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("HiddenColumnPath", CheckBoxPath.IsChecked.Value);
            IsChangedSettings();
        }

        private void CheckBoxSize_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("HiddenColumnSize", CheckBoxSize.IsChecked.Value);
            IsChangedSettings();
        }

        private void CheckBoxDirectoryVersion_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("HiddenColumnDirectoryVersion", CheckBoxDirectoryVersion.IsChecked.Value);
            IsChangedSettings();
        }

        private void IsChangedSettings()
        {
            _modifiedSettings = true;
            ChangeVisibleModifiedSettings();
            ChangeVisibleColumnDataGrid();
        }

        #region Visibility

        private void ChangeVisibleModifiedSettings()
        {
            ModifiedSettings.Visibility = _modifiedSettings ? Visibility.Visible : Visibility.Hidden;
        }

        private void ChangeVisibleColumnDataGrid()
        {
            ChangeVisibleColumnDataGridByName("DirectoryVersion", CheckBoxDirectoryVersion.IsChecked.Value);
            ChangeVisibleColumnDataGridByName("Path", CheckBoxPath.IsChecked.Value);
            ChangeVisibleColumnDataGridByName("Size", CheckBoxSize.IsChecked.Value);
        }

        private void ChangeVisibleColumnDataGridByName(string columnName, bool valueVisible)
        {
            DataGridList.Columns.First(
                f => (string)f.Header == _dictionaryNameColumn.First(
                    f2 => f2.Key == columnName).Value).Visibility = valueVisible == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ChangeVisiblePanelSettigs()
        {
            GridProperties.Visibility = GridProperties.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            ButtonSettings.FontWeight = GridProperties.Visibility == Visibility.Visible ? FontWeights.Heavy : FontWeights.Normal;
        }

        #endregion

        #endregion

        #region DataGridList

        private void DataGridList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            _dictionaryNameColumn.Clear();

            foreach (PropertyInfo propInfo in new FileSubscriber().GetType().GetProperties())
            {
                DataGridColumn column = DataGridList.Columns.FirstOrDefault(f => (string)f.Header == propInfo.Name);
                if (column != null)
                {
                    var propAttribute = propInfo.GetCustomAttributes<ColumnAttribute>();
                    if (propAttribute.Count() > 0)
                    {
                        var attribute = propAttribute.First();

                        _dictionaryNameColumn.Add(propInfo.Name, attribute.HeaderName);

                        column.Header = attribute.HeaderName;
                        column.Visibility = attribute.VisibleColumn ? Visibility.Visible : Visibility.Hidden;

                        if (!string.IsNullOrWhiteSpace(attribute.SortMemberPath))
                            column.SortMemberPath = attribute.SortMemberPath;

                        if (attribute.SortDirection != null)
                            column.SortDirection = attribute.SortDirection;
                    }
                }
            }
            ChangeVisibleColumnDataGrid();
        }

        private void SetItemSouce(List<FileSubscriber> list = null)
        {
            if (list != null)
                listFiles = list;
            DataGridList.ItemsSource = listFiles;
        }

        #endregion

        #region Row datagrid (files)

        private FileSubscriber GetCurrentRow()
        {
            return (FileSubscriber)DataGridList.SelectedItem;
        }

        private IList GetCurrentRows()
        {
            return DataGridList.SelectedItems;
        }

        private void AddFilesInDataGridList(string[] arrayFilesName)
        {
            List<FileSubscriber> list = listFiles.ToList();

            foreach (string file in arrayFilesName)
            {
                FileSubscriber finded = list.Find(f => f.Path == file);

                if (finded != null)
                {
                    Dialog.ShowMessage($"Выбранный файл уже контролируется:\n" +
                        $"{file}");
                    return;
                }

                ulong sizeFile = DirFile.GetFileSize(file);

                FileSubscriber fileChecked = new FileSubscriber()
                {
                    Checked = true,
                    Path = file,
                    FileName = DirFile.GetFileName(file),
                    Size = sizeFile,
                    SizeString = DirFile.GetSizeFormat(sizeFile)
                };

                //_subscriber.SubscribeChangeFile(fileChecked);

                list.Add(fileChecked);
            }

            SetItemSouce(list);
        }

        #endregion

    }
}
