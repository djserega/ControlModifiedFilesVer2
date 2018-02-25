using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class UserSettings
    {

        internal static string GetFilterFiles()
        {
            var filter = GetDefaultProperties().ListFilterFilesPredefined;
            StringBuilder sb = new StringBuilder();

            foreach (string itemFilter in filter)
            {
                if (sb.Length > 0)
                    sb.Append("|");
                sb.Append(itemFilter);
            }
            return sb.ToString();
        }

        internal static bool GetUserSettings(string param)
        {
            switch (param)
            {
                case "HiddenColumnDirectoryVersion":
                    return GetDefaultProperties().HiddenColumnDirectoryVersion;
                case "HiddenColumnPath":
                    return GetDefaultProperties().HiddenColumnPath;
                case "HiddenColumnSize":
                    return GetDefaultProperties().HiddenColumnSize;
                case "UsePrefixUserName":
                    return GetDefaultProperties().UsePrefixUserName;
                default:
                    Errors.Save($"Не найден параметр {param}.");
                    return false;
            }
        }

        internal static void SetUserSettings(string param, bool newValue)
        {
            switch (param)
            {
                case "HiddenColumnDirectoryVersion":
                    GetDefaultProperties().HiddenColumnDirectoryVersion = newValue;
                    break;
                case "HiddenColumnPath":
                    GetDefaultProperties().HiddenColumnPath = newValue;
                    break;
                case "HiddenColumnSize":
                    GetDefaultProperties().HiddenColumnSize = newValue;
                    break;
                case "UsePrefixUserName":
                    GetDefaultProperties().UsePrefixUserName = newValue;
                    break;
                default:
                    Errors.Save($"Не найден параметр {param}.");
                    break;
            }
        }

        internal static void SaveUserSettings()
        {
            GetDefaultProperties().Save();
        }

        private static Properties.Settings GetDefaultProperties()
        {
            return Properties.Settings.Default;
        }

    }
}
