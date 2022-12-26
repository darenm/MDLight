using System;

using Windows.Storage;

namespace MDLight.Utilities
{
    public enum AppSettings
    {
        AppTheme,
    }
    
    internal static class SettingsHelper
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static string GetSetting(AppSettings setting)
        {
            if (localSettings.Values[setting.ToString()] != null)
            {
                return localSettings.Values[setting.ToString()].ToString();
            }
            else
            {
                return null;
            }
        }

        public static T GetSetting<T>(AppSettings setting)
        {
            if (localSettings.Values[setting.ToString()] != null)
            {
                return (T)localSettings.Values[setting.ToString()];
            }
            else
            {
                return default(T);
            }
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
