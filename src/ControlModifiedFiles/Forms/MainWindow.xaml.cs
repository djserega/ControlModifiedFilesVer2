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
using System.Diagnostics;

namespace ControlModifiedFiles
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Private fields

        #region Events

        private CallUpdateVersionEvents _callUpdate = new CallUpdateVersionEvents();
        private UseContextMenuEvents _useContextMenu = new UseContextMenuEvents();

        #endregion

        private List<string> _listFilter = UserSettings.GetFormatFiles();
        private List<string> _listExtensionV8 = UserSettings.GetExtension(ExtensionVersion.v8);
        private List<string> _listExtensionV7 = UserSettings.GetExtension(ExtensionVersion.v7);

        private Subscriber _subscriber;
        private NotifyIcon _notifyIcon;

        private FileSubscriber _fileSubscriberCurrentRow;

        private bool _handlerLoadForm;

        private bool _openedGridListVersion;
        private bool _openedGridSettings;

        private ICollection<ListVersion> _listVersion = new List<ListVersion>();
        private ComparsionV8Viewer _comparsionV8Viewer = new ComparsionV8Viewer();
        private ComparsionDefy _comparsionDefy = new ComparsionDefy();

        #endregion

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

            SetItemSouceDataGridList();
            SetItemSourceVersion();

            ChangeVisiblePanelSettings(true);
            ChangeVisiblePanelVersion(true);
            ChangeVisibleComparer(false);

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

            SetItemSouceDataGridList();
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
            CheckBoxUseV8Viewer.IsChecked = UserSettings.GetUserSettings("UseV8Viewer");
            CheckBoxUseDefy.IsChecked = UserSettings.GetUserSettings("UseDefy");
            TextBoxPathDefy.Text = UserSettings.GetUserSettings("PathDefy");
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_openedGridListVersion)
                ChangeVisiblePanelVersion();

            ChangeVisiblePanelSettings();
        }

        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SaveUserSettings();
            _modifiedSettings = false;
            ChangeVisiblePanelSettings();
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

        private void TextBoxPathDefy_TextChanged(object sender, TextChangedEventArgs e)
        {
            _comparsionDefy = new ComparsionDefy();
            IsChangedSettings();
        }

        private void CheckBoxUseV8Viewer_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("UseV8Viewer", CheckBoxUseV8Viewer.IsChecked.Value);
            IsChangedSettings();
        }

        private void CheckBoxUseDefy_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.SetUserSettings("UseDefy", CheckBoxUseDefy.IsChecked.Value);
            IsChangedSettings();
        }

        private void ButtonSelectDefy_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = _comparsionDefy.SelectFile();
            if (!string.IsNullOrWhiteSpace(selectedFile))
            {
                TextBoxPathDefy.Text = selectedFile;
                UserSettings.SetUserSettings("PathDefy", TextBoxPathDefy.Text);
                _comparsionDefy = new ComparsionDefy();
                IsChangedSettings();
            }
        }

        #endregion

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

        private void ChangeVisiblePanelSettings(bool onLoad = false)
        {
            bool fadingIn = false;

            if (!onLoad)
            {
                fadingIn = GridProperties.Opacity <= 0;

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

                timer.Tick += (s, e1) =>
                {
                    if (fadingIn)
                    {
                        GridProperties.Visibility = _openedGridSettings ? Visibility.Visible : Visibility.Hidden;
                        if ((GridProperties.Opacity += 0.1d) >= 1)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                    else
                    {
                        if ((GridProperties.Opacity -= 0.1d) <= 0)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                };
                timer.Disposed += (sender, e) => GridProperties.Visibility = _openedGridSettings ? Visibility.Visible : Visibility.Hidden;

                timer.Interval = 50;
                timer.Start();
            }
            else
                GridProperties.Opacity = 0;

            _openedGridSettings = fadingIn;

            ButtonSettings.FontWeight = _openedGridSettings ? FontWeights.Heavy : FontWeights.Normal;
        }

        private void ChangeVisiblePanelVersion(bool onLoad = false)
        {
            bool fadingIn = false;

            if (!onLoad)
            {
                fadingIn = GridListVersion.Opacity <= 0;

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

                timer.Tick += (sender, e) =>
                {
                    if (fadingIn)
                    {
                        GridListVersion.Visibility = _openedGridListVersion ? Visibility.Visible : Visibility.Hidden;
                        if ((GridListVersion.Opacity += 0.1d) >= 1)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                    else
                    {
                        if ((GridListVersion.Opacity -= 0.1d) <= 0)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                };
                timer.Disposed += (sender, e) => GridListVersion.Visibility = _openedGridListVersion ? Visibility.Visible : Visibility.Hidden;

                timer.Interval = 50;
                timer.Start();
            }
            else
                GridListVersion.Opacity = 0;

            _openedGridListVersion = fadingIn;

            ButtonVersions.FontWeight = _openedGridListVersion ? FontWeights.Heavy : FontWeights.Normal;
        }

        private void ChangeVisibleComparer(bool visible)
        {
            if (_fileSubscriberCurrentRow == null)
                visible = false;
            else
            {
                if (!string.IsNullOrWhiteSpace(_listExtensionV8.FirstOrDefault(f => f == _fileSubscriberCurrentRow.Extension)))
                {
                    if (!(UserSettings.GetUserSettings("UseV8Viewer") && _comparsionV8Viewer.ProgramInstalled))
                        visible = false;
                }
                else if (!string.IsNullOrWhiteSpace(_listExtensionV7.FirstOrDefault(f => f == _fileSubscriberCurrentRow.Extension)))
                {
                    if (!(UserSettings.GetUserSettings("UseDefy") && _comparsionDefy.ProgramInstalled))
                        visible = false;
                }
            }
            ButtonCompareVersion.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            CheckBoxSelectVersion.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }


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

                        if (propInfo.Name == "DateMaxVersion")
                            ((DataGridTextColumn)column).Binding.StringFormat = "dd.MM.yyyy HH:mm:ss";

                        if (!string.IsNullOrWhiteSpace(attribute.SortMemberPath))
                            column.SortMemberPath = attribute.SortMemberPath;

                        if (attribute.SortDirection != null)
                            column.SortDirection = attribute.SortDirection;
                    }
                }
            }
            ChangeVisibleColumnDataGrid();
        }

        private void SetItemSouceDataGridList()
        {
            DataGridList.ItemsSource = listFiles;
        }

        private void SetItemSourceVersion()
        {
            DataGridVersion.ItemsSource = _listVersion;
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
            string[] selectedFiles = (string[])e.Data.GetData("FileDrop");

            string[] addedFiles = new string[selectedFiles.Count()];

            int i = 0;
            foreach (string item in selectedFiles)
            {
                FileInfo fileInfo = new FileInfo(item);
                if (!String.IsNullOrWhiteSpace(_listFilter.FirstOrDefault(f => f == fileInfo.Extension)))
                {
                    addedFiles[i++] = item;
                }
            }
            AddFilesInDataGridList(addedFiles);
        }

        private void DataGridList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _fileSubscriberCurrentRow = null;
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is FileSubscriber subscriber)
                {
                    _fileSubscriberCurrentRow = subscriber;
                    LoadListVersion();
                }
                else
                    _listVersion.Clear();
            }
            else if (_listVersion.Count > 0)
                _listVersion.Clear();
        }

        #endregion

        #region DataGridVersion

        private void ButtonVersions_Click(object sender, RoutedEventArgs e)
        {
            if (_openedGridSettings)
                ChangeVisiblePanelSettings();

            ChangeVisiblePanelVersion();

            if (_openedGridListVersion)
                LoadListVersion();
        }

        private void DataGridVersion_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!e.Cancel)
                ((ListVersion)e.Row.DataContext).Checked = !((ListVersion)e.Row.DataContext).Checked;
        }

        private void ButtonCompareVersion_Click(object sender, RoutedEventArgs e)
        {
            string path1 = string.Empty;
            string path2 = string.Empty;

            foreach (ListVersion item in _listVersion)
            {
                if (!string.IsNullOrWhiteSpace(path1)
                    && !string.IsNullOrWhiteSpace(path2))
                    break;

                if (item.Checked)
                {
                    if (string.IsNullOrWhiteSpace(path1))
                        path1 = item.Path;
                    else
                        path2 = item.Path;
                }
            }

            if (!string.IsNullOrWhiteSpace(path1)
                && !string.IsNullOrWhiteSpace(path2))
            {
                if (!string.IsNullOrWhiteSpace(_listExtensionV8.FirstOrDefault(f => f == _fileSubscriberCurrentRow.Extension)))
                    _comparsionV8Viewer.CompareVersion(path1, path2);
                else if (!string.IsNullOrWhiteSpace(_listExtensionV7.FirstOrDefault(f => f == _fileSubscriberCurrentRow.Extension)))
                    _comparsionDefy.CompareVersion(path1, path2);
            }
        }

        private void DataGridVersion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((DataGrid)e.Source).SelectedItem is ListVersion selectedVersion)
            {
                selectedVersion.Checked = !selectedVersion.Checked;
            }
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
                list.Add(item);

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

        #region Versions

        private void LoadListVersion()
        {
            if (_fileSubscriberCurrentRow != null)
            {
                _listVersion = new Versions() { Subscriber = _fileSubscriberCurrentRow }.GetListVersion();
                SetItemSourceVersion();

                ChangeVisibleComparer(true);
            }
            else
                ChangeVisibleComparer(false);
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
                    _notifyIcon.CreateVersion(subscriber);
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
