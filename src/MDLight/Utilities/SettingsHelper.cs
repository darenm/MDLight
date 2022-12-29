using System;

using Windows.Storage;

namespace MDLight.Utilities
{
    public enum AppSettings
    {
        AppTheme,
        Backdrop
    }
    
    internal static class SettingsHelper
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static string GetSetting(AppSettings setting, string defaultValue = null)
        {
            if (localSettings.Values[setting.ToString()] != null)
            {
                return localSettings.Values[setting.ToString()].ToString();
            }

            return defaultValue;
        }

        public static T GetSetting<T>(AppSettings setting, T defaultValue = default)
        {
            if (localSettings.Values[setting.ToString()] != null)
            {
                return (T)localSettings.Values[setting.ToString()];
            }

            return defaultValue;
        }

        public static void SetSetting(AppSettings setting, object value)
        {
            localSettings.Values[setting.ToString()] = value;
        }

        public static void ClearSetting(AppSettings setting)
        {
            localSettings.Values.Remove(setting.ToString());
        }
    }
}
