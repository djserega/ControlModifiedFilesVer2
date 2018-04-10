using System;
using System.Reflection;
using System.Text;
using WF = System.Windows.Forms;
          
namespace ControlModifiedFiles
{
    internal class NotifyIcon
    {
        private WF.NotifyIcon _notifyIcon;
        private string _balloonTipTextByDefault;
        private UseContextMenuEvents _useContextMenu;
        public FileSubscriber Subscriber { get; set; }

        internal NotifyIcon(UseContextMenuEvents useContextMenu)
        {
            _useContextMenu = useContextMenu;
            _balloonTipTextByDefault = "Уведомление по умолчанию.";

            _notifyIcon = new WF.NotifyIcon()
            {
                BalloonTipIcon = WF.ToolTipIcon.Info,
                ContextMenu = CreateContextMenu(),
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location)
            };
            _notifyIcon.DoubleClick += ContextMenuItemStateNormal;
            _notifyIcon.BalloonTipClicked += _notifyIcon_BalloonTipClicked;

            SetBalloonTipTextDefault();
        }

        private void _notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (Subscriber != null)
            {
                var mainWindow = App.Current.MainWindow;
                mainWindow.Activate();

                FormComment form = new FormComment(Subscriber) { Owner = mainWindow };
                form.ShowDialog();
                if (form.ClickOK)
                {
                    string textComment = form.TextBoxComment.Text;
                    if (!string.IsNullOrWhiteSpace(textComment))
                    {
                        new Versions() { Subscriber = Subscriber }.SetCommentFile(form.Version, textComment);
                        Subscriber = null;
                    }
                }
            }
        }

        #region CreateContextMenu

        private WF.ContextMenu CreateContextMenu()
        {
            var contextMenu = new WF.ContextMenu();

            var menuItemAdd = AddMenuItemContextMenu(ref contextMenu, "Добавить файлы", "ContextMenuAddFiles", ContextMenuItemAdd_Click);
            menuItemAdd.DefaultItem = true;
            menuItemAdd.Shortcut = WF.Shortcut.Ins;

            AddMenuItemContextMenu(ref contextMenu, "Развернуть", "ContextMenuStateNormal", ContextMenuItemStateNormal);
            AddMenuItemContextMenu(ref contextMenu, "-", "br");
            AddMenuItemContextMenu(ref contextMenu, "Закрыть", "ContextMenuExit", ContextMenuItemExit);

            return contextMenu;
        }

        private WF.MenuItem AddMenuItemContextMenu(ref WF.ContextMenu contextMenu, string caption, string name, EventHandler eventHandlerClick = null)
        {
            var menuItem = contextMenu.MenuItems.Add(caption);
            menuItem.Name = name;

            if (eventHandlerClick != null)
                menuItem.Click += eventHandlerClick;

            return menuItem;
        }

        #endregion

        #region Events

        private void ContextMenuItemAdd_Click(object sender, EventArgs e)
        {
            _useContextMenu.CallAddFiles();
        }

        private void ContextMenuItemStateNormal(object sender, EventArgs e)
        {
            _useContextMenu.CallWindowsStateNormal();
        }

        private void ContextMenuItemExit(object sender, EventArgs e)
        {
            _useContextMenu.CallExitApp();
        }

        #endregion

        #region Methods

        internal void HideIcon()
        {
            _notifyIcon.Visible = false;
        }

        private void ShowIcon(bool showBalloonTip = true)
        {
            _notifyIcon.Visible = true;

            if (showBalloonTip)
                _notifyIcon.ShowBalloonTip(3 * 1000);
        }

        internal void StartProgram()
        {
            ShowIcon(false);
        }

        internal void ChangeStateWindow()
        {
            SetBalloonTipTextHideToTray();
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        internal void CreateVersion()
        {
            SetBalloonTipTextNewVersionFile();
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        internal void CreateVersion(int version)
        {
            SetBalloonTipTextNewVersionFile(version);
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        internal void AddFile(int i, string[] addedFiles)
        {
            if (i > 0)
            {
                string text;
                if (i == 1)
                {
                    text = $"Добавлен файл {addedFiles[--i]}";
                }
                else
                {
                    text = $"Добавлено файлов: {i}";
                    if (i < 4)
                    {
                        StringBuilder stringBuilder = new StringBuilder(text);
                        foreach (string fileName in addedFiles)
                        {
                            stringBuilder.AppendLine(fileName);
                        }
                        text = stringBuilder.ToString();
                    }
                }
                AddFile(text);
            }
        }

        internal void AddFile(string text)
        {
            SetBalloonTipTextAddFiles(text);
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        #endregion

        #region Private methods

        private void SetBalloonTipTextDefault()
        {
            SetBalloonTipText(_balloonTipTextByDefault);
        }

        private void SetBalloonTipTextAddFiles(string text)
        {
            SetBalloonTipText(text);
        }

        private void SetBalloonTipTextNewVersionFile(int version)
        {
            SetBalloonTipText($"Актуальная версия {version}.");
        }

        private void SetBalloonTipTextNewVersionFile()
        {
            SetBalloonTipText($"Файл: {Subscriber.FileName}\nАктуальная версия {Subscriber.Version}.");
        }

        private void SetBalloonTipTextHideToTray()
        {
            SetBalloonTipText("Приложение продолжает 'следить' за выбранными файлами :-)).");
        }

        private void SetBalloonTipText(string text)
        {
            _notifyIcon.BalloonTipText = text;
        }

        #endregion

    }
}
