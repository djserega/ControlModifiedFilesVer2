using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
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
using System.Windows.Interop;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CallUpdateVersionEvents _callUpdate = new CallUpdateVersionEvents();
        private Subscriber _subscriber;
        private UseContextMenuEvents _useContextMenu = new UseContextMenuEvents();

        private NotifyIcon _notifyIcon;

        private bool _handlerLoadForm;

        #region Properties

        public static ObservableCollection<FileSubscriber> listFiles = new ObservableCollection<FileSubscriber>();
        private bool _modifiedSettings = false;
        private Dictionary<string, string> _dictionaryNameColumn = new Dictionary<string, string>();
        private bool AwaitResultStatusAutoresult = false;

        #endregion

        #region Window

        public MainWindow()
        {
            InitializeComponent();

            _subscriber = new Subscriber(_callUpdate);
            _notifyIcon = new NotifyIcon(_useContextMenu);

            ApplicationRunWithAdministration();
        }

        private void MainWindowControlModifiedFiles_Loaded(object sender, RoutedEventArgs e)
        {
            _handlerLoadForm = true;

            SetItemSouce();

            ChangeVisiblePanelSettigs();
            LoadUserSettings();
            ChangeVisibleModifiedSettings();

            _callUpdate.CallUpdateVersion += UpdateVersion;
            _useContextMenu.CallUseContextMenu += EvokedContextMenu;

            SetIconShieldUAC();

            _handlerLoadForm = false;
        }

        private void EvokedContextMenu()
        {
            if (_useContextMenu.AddFiles)
                ButtonAddFiles_Click(null, null);
            else if (_useContextMenu.WindowsStateNormal)
            {
                MainMenuWindowShow();
                WindowState = WindowState.Normal;
            }
            else if (_useContextMenu.ExitApp)
                Application.Current.Shutdown();
        }

        private void MainWindowControlModifiedFiles_StateChanged(object sender, EventArgs e)
        {
            ChangeStateWindow();
        }

        #endregion

        #region Menu button

        private void ButtonAddFiles_Click(object sender, RoutedEventArgs e)
        {
            AddFilesInDataGridList(DirFile.SelectNewFiles(this), sender == null);
        }

        private void ButtonRemoveFiles_Click(object sender, RoutedEventArgs e)
        {
            List<FileSubscriber> listSelectedFiles = GetCurrentRows();
            if (listSelectedFiles.Count == 0)
                return;


            bool? result = Dialog.ShowQuesttion($"Будет отменен контроль файлов - {listSelectedFiles.Count}.\n" +
                $"Продолжить?");

            if (result == null || !(bool)result)
                return;

            foreach (FileSubscriber item in listSelectedFiles)
            {
                _subscriber.Unsubscribe(item);
                listFiles.Remove(item);
            }

            SetItemSouce();
        }

        #endregion

        #region Settings

        private void LoadUserSettings()
        {
            CheckBoxDirectoryVersion.IsChecked = UserSettings.GetUserSettings("HiddenColumnDirectoryVersion");
            CheckBoxPath.IsChecked = UserSettings.GetUserSettings("HiddenColumnPath");
            CheckBoxSize.IsChecked = UserSettings.GetUserSettings("HiddenColumnSize");
            CheckBoxUsePrefixUserName.IsChecked = UserSettings.GetUserSettings("UsePrefixUserName");
            GetStatusAutostart();
            CheckBoxHideToTray.IsChecked = UserSettings.GetUserSettings("HideToTray");
            TextBoxNotifyVersionCreation.Text = Convert.ToString(UserSettings.GetUserSettings("NotifyVersionCreation"));
        }

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

        private void CheckBoxUsePrefixUserName_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("UsePrefixUserName", CheckBoxUsePrefixUserName.IsChecked.Value);
            IsChangedSettings();
        }

        private void Autostart_Click(object sender, RoutedEventArgs e)
        {
            if (!AwaitResultStatusAutoresult)
                new Permission().SetRemoveAutostart(CheckBoxAutostart.IsChecked.Value);

            AwaitResultStatusAutoresult = true;
            GetStatusAutostart();
            AwaitResultStatusAutoresult = false;
        }

        private void IsChangedSettings()
        {
            if (!_handlerLoadForm)
                _modifiedSettings = true;

            ChangeVisibleModifiedSettings();
            ChangeVisibleColumnDataGrid();
        }

        private void CheckBoxHideToTray_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("HideToTray", CheckBoxHideToTray.IsChecked.Value);
            IsChangedSettings();
        }

        private void TextBoxNotifyVersionCreation_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textNotifyVersiob = TextBoxNotifyVersionCreation.Text.ToString();

            int notifyVersion;
            try
            {
                checked
                {
                    notifyVersion = Convert.ToInt32(textNotifyVersiob);
                }
            }
            catch (Exception)
            {
                notifyVersion = 0;
            }

            UserSettings.SetUserSettings("NotifyVersionCreation", notifyVersion);
            IsChangedSettings();
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

                        column.IsReadOnly = attribute.IsOnlyRead;
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

        private void SetItemSouce()
        {
            DataGridList.ItemsSource = listFiles;
        }

        private void DataGridList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column is DataGridBoundColumn currentColumn)
                {
                    string bindingPath = ((Binding)currentColumn.Binding).Path.Path;

                    if (bindingPath == "Checked")
                        if (e.Row.Item is FileSubscriber subscriber)
                        {
                            subscriber.Checked = ((System.Windows.Controls.Primitives.ToggleButton)e.EditingElement).IsChecked.Value;

                            if (subscriber.Checked)
                                _subscriber.Subscribe(subscriber);
                            else
                                _subscriber.Unsubscribe(subscriber);
                        }
                }
            }
        }

        private void DataGridList_Drop(object sender, DragEventArgs e)
        {
            List<string> listFilter = UserSettings.GetFormatFiles();

            string[] selectedFiles = (string[])e.Data.GetData("FileDrop");

            string[] addedFiles = new string[selectedFiles.Count()];

            int i = 0;
            foreach (string item in selectedFiles)
            {
                FileInfo fileInfo = new FileInfo(item);
                if (!String.IsNullOrWhiteSpace(listFilter.FirstOrDefault(f => f == fileInfo.Extension)))
                {
                    addedFiles[i++] = item;
                }
            }
            AddFilesInDataGridList(addedFiles);
        }

        #endregion

        #region Row datagrid (files)

        private FileSubscriber GetCurrentRow()
        {
            return (FileSubscriber)DataGridList.SelectedItem;
        }

        private List<FileSubscriber> GetCurrentRows()
        {
            List<FileSubscriber> list = new List<FileSubscriber>();
            foreach (FileSubscriber item in DataGridList.SelectedItems)
            {
                list.Add(item);
            }

            return list;
        }

        private void AddFilesInDataGridList(string[] arrayFilesName, bool notified = false)
        {
            if (arrayFilesName != null)
            {
                string[] addedFiles = new string[arrayFilesName.Count()];
                int i = 0;
                foreach (string file in arrayFilesName)
                {
                    if (!string.IsNullOrWhiteSpace(file))
                    {
                        FileSubscriber finded = listFiles.FirstOrDefault(f => f.Path == file);
                        if (finded != null)
                        {
                            Dialog.ShowMessage($"Выбранный файл уже контролируется:\n" +
                                $"{file}");
                            continue;
                        }

                        FileSubscriber subscriber = AddFileInDataGrid(file, notified);
                        addedFiles[i++] = subscriber.FileName;
                    }
                }
                if (notified)
                    _notifyIcon.AddFile(i, addedFiles);
            }
        }

        private FileSubscriber AddFileInDataGrid(string file, bool notified = false)
        {
            FileSubscriber subscriber = new FileSubscriber()
            {
                Checked = true,
                Path = file,
                FileName = DirFile.GetFileName(file)
            };
            subscriber.SetCurrentSize();

            _subscriber.Subscribe(subscriber, notified);

            listFiles.Add(subscriber);

            return subscriber;
        }

        #endregion

        #region Permission

        private void GetStatusAutostart()
        {
            CheckBoxAutostart.IsChecked = new Permission().GetStatusAutostart();
        }

        private void SetIconShieldUAC()
        {
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                      SystemIcons.Shield.ToBitmap().GetHbitmap(),
                      IntPtr.Zero,
                      Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
            ImageShield.Source = wpfBitmap;
        }

        private void ApplicationRunWithAdministration()
        {
            bool RunWithAdmin = false;
            string[] commandLine = Environment.GetCommandLineArgs();
            if (commandLine.Count() >= 2)
                RunWithAdmin = commandLine[1] == "/run from administrator";

            if (RunWithAdmin)
                GridProperties.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Private methods

        private void UpdateVersion(FileSubscriber subscriber, bool NeedNotified)
        {
            subscriber.SetCurrentVersion();
            subscriber.SetCurrentSize();

            if (NeedNotified)
            {
                if (subscriber.Version != subscriber.PreviousVersion)
                    subscriber.CountVersionWithoutNotify++;

                int userSettingsNotifyCount = UserSettings.GetUserSettings("NotifyVersionCreation");
                if (userSettingsNotifyCount > 0
                    && subscriber.CountVersionWithoutNotify >= userSettingsNotifyCount)
                {
                    _notifyIcon.CreateVersion(subscriber.Version);
                    subscriber.CountVersionWithoutNotify = 0;
                }
            }
        }

        private void MainMenuWindowShow()
        {
            ShowInTaskbar = true;
            Show();
        }

        private void MainMenuWindowHide()
        {
            Hide();
            ShowInTaskbar = false;
        }

        private void ChangeStateWindow()
        {
            if (WindowState == WindowState.Minimized
                && UserSettings.GetUserSettings("HideToTray"))
            {
                _notifyIcon.ChangeStateWindow();
                MainMenuWindowHide();
            }
            else
            {
                MainMenuWindowShow();

                _notifyIcon.HideIcon();
            }
        }

        #endregion

    }
}
