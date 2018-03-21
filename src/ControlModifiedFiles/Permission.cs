using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace ControlModifiedFiles
{
    internal class Permission
    {
        private string _fullPathApplication;
        private string _nameApplication;

        public Permission()
        {
            _fullPathApplication = Assembly.GetExecutingAssembly().Location;
            _nameApplication = Path.GetFileNameWithoutExtension(new FileInfo(_fullPathApplication).Name);
        }

        internal bool GetStatusAutostart()
        {
            try
            {
                return StatusAutostart();
            }
            catch (Exception ex)
            {
                Dialog.ShowMessage($"Не удалось получить статус автозапуска.\nПричина: {ex.Message}");
                return false;
            }
        }

        internal bool SetRemoveAutostart(bool status)
        {
            try
            {
                if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    RunWithAdministrator();
                    return false;
                };

                if (status)
                    SetAutostart();
                else
                    RemoveAutostart();

                return true;
            }
            catch (Exception ex)
            {
                if (status)
                    Dialog.ShowMessage($"Не удалось подключить автозапуск.\nПричина: {ex.Message}");
                else
                    Dialog.ShowMessage($"Не удалось отключить автозапуск.\nПричина: {ex.Message}");

                return false;
            }
        }

        private bool StatusAutostart()
        {
            using (RegistryKey key = GetRegistryKey())
            {
                object status = key.GetValue(_nameApplication);
                return status != null;
            };
        }

        private void SetAutostart()
        {
            using (RegistryKey key = GetRegistryKey(true))
            {
                key.SetValue(_nameApplication, _fullPathApplication);
            };
        }

        private void RemoveAutostart()
        {
            using (RegistryKey key = GetRegistryKey(true))
            {
                key.DeleteValue(_nameApplication);
            };
        }

        private RegistryKey GetRegistryKey(bool writable = false)
        {
            return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", writable);
        }

        private void RunWithAdministrator()
        {
            try
            {
                Process.Start(new ProcessStartInfo(_fullPathApplication, "/\"run from administrator\"")
                {
                    Verb = "runas"
                });
                Application.Current.Shutdown();
            }
            catch (Win32Exception) { }
            catch (Exception ex)
            {
                Dialog.ShowMessage("Ошибка повышения прав.");
                Errors.Save(ex);
            }
        }
    }
}
