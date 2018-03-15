using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WF = System.Windows.Forms;


namespace ControlModifiedFiles
{
    internal class NotifyIcon
    {
        private WF.NotifyIcon _notifyIcon;
        private string _balloonTipTextByDefault;
        private UseContextMenuEvents _useContextMenu;

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

            SetBalloonTipTextDefault();
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

        private void ShowIcon()
        {
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(3 * 1000);
        }

        internal void ChangeStateWindow()
        {
            SetBalloonTipTextHideToTray();
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        internal void CreateVersion()
        {
            SetBalloonTipTextAddFiles();
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        internal void CreateVersion(int version)
        {
            SetBalloonTipTextAddFiles(version);
            ShowIcon();
            SetBalloonTipTextDefault();
        }

        #endregion

        #region Private methods

        private void SetBalloonTipTextDefault()
        {
            SetBalloonTipText(_balloonTipTextByDefault);
        }

        private void SetBalloonTipTextAddFiles()
        {
            SetBalloonTipText("Добавлен новый файл.");
        }

        private void SetBalloonTipTextAddFiles(int version)
        {
            SetBalloonTipText($"Актуальная версия {version}.");
        }

        private void SetBalloonTipTextNewVersionFile()
        {
            SetBalloonTipText("Создана новая версия файла.");
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
