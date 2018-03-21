using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ControlModifiedFiles
{
    internal static class UserSettings
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

        internal static List<string> GetFormatFiles()
        {
            List<string> list = new List<string>();
            foreach (String item in GetDefaultProperties().FormatFiles)
                list.Add(item);

            return list;
        }

        internal static List<string> GetExtension(ExtensionVersion version)
        {
            StringCollection collection;
            if (version == ExtensionVersion.v8)
                collection = GetDefaultProperties().ExtensionV8;
            else if (version == ExtensionVersion.v7)
                collection = GetDefaultProperties().ExtensionV7;
            else
                return null;

            List<string> list = new List<string>();
            foreach (String item in collection)
                list.Add(item);

            return list;
        }

        internal static dynamic GetUserSettings(string param)
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
                case "HideToTray":
                    return GetDefaultProperties().HideToTray;
                case "NotifyVersionCreation":
                    return GetDefaultProperties().NotifyVersionCreation;
                case "UseV8Viewer":
                    return GetDefaultProperties().UseV8Viewer;
                case "UseDefy":
                    return GetDefaultProperties().UseDefy;
                case "PathDefy":
                    return GetDefaultProperties().PathDefy;
                default:
                    Errors.Save($"Не найден параметр {param}.");
                    return false;
            }
        }

        internal static void SetUserSettings(string param, dynamic newValue)
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
                case "HideToTray":
                    GetDefaultProperties().HideToTray = newValue;
                    break;
                case "NotifyVersionCreation":
                    GetDefaultProperties().NotifyVersionCreation = newValue;
                    break;
                case "UseV8Viewer":
                    GetDefaultProperties().UseV8Viewer = newValue;
                    break;
                case "UseDefy":
                    GetDefaultProperties().UseDefy = newValue;
                    break;
                case "PathDefy":
                    GetDefaultProperties().PathDefy = newValue;
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
