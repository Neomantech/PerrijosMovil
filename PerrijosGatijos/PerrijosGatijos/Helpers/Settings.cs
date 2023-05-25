using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PerrijosGatijos.Helpers
{
	public class Settings
	{

		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;
        private const string IsLoggedInTokenKey = "isloggedid_key";
        private static readonly bool IsLoggedInTokenDefault = false;
        private const string IsLCodeInTokenKey = "iscode_key";
        private static readonly bool IsCodeInTokenDefault = false;

        #endregion

        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static bool IsLoggedIn
        {
            get { return AppSettings.GetValueOrDefault(IsLoggedInTokenKey, IsLoggedInTokenDefault); }
            set { AppSettings.AddOrUpdateValue(IsLoggedInTokenKey, value); }
        }

        public static bool IsCodeIn
        {
            get { return AppSettings.GetValueOrDefault(IsLCodeInTokenKey, IsCodeInTokenDefault); }
            set { AppSettings.AddOrUpdateValue(IsLCodeInTokenKey, value); }
        }
    }
}

