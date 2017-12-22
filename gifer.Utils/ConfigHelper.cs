using System;
using System.Configuration;
using System.Linq;
using System.Windows.Media;

namespace gifer.Utils {
    public static class ConfigHelper {
        private readonly static BitmapScalingMode[] Modes = (BitmapScalingMode[])Enum.GetValues(typeof(BitmapScalingMode));
        private readonly static Language[] Languages = (Language[])Enum.GetValues(typeof(Language));
        private readonly static string ExePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\gifer.exe";

        public static Configuration Setup(this Configuration config) {
            if (!config.AppSettings.Settings.AllKeys.Contains("showHelpAtStartup")) {
                config.AppSettings.Settings.Add("showHelpAtStartup", "true");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (!config.AppSettings.Settings.AllKeys.Contains("language")) {
                config.AppSettings.Settings.Add("language", "EN");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (!config.AppSettings.Settings.AllKeys.Contains("scalingMode")) {
                config.AppSettings.Settings.Add("scalingMode", "NearestNeighbor");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            return config;
        }

        public static bool GetShowHelpAtStartup() {
            var config = ConfigurationManager.OpenExeConfiguration(ExePath).Setup();
            return bool.Parse(config.AppSettings.Settings["showHelpAtStartup"].Value);
        }

        public static void SetShowHelpAtStartup(bool showHelpAtStartup) {
            var config = ConfigurationManager.OpenExeConfiguration(ExePath).Setup();
            if (config.AppSettings.Settings["showHelpAtStartup"].Value != showHelpAtStartup.ToString()) {
                config.AppSettings.Settings.Remove("showHelpAtStartup");
                config.AppSettings.Settings.Add("showHelpAtStartup", showHelpAtStartup.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public static Language? FindLanguage() {
            var config = ConfigurationManager.OpenExeConfiguration(ExePath).Setup();
            Language language;
            string value = config.AppSettings.Settings[nameof(language)]?.Value;
            if (value != null && Enum.TryParse(value, out language) && Languages.Contains(language)) {
                return language;
            } else {
                return null;
            }
        }

        public static void SetLanguage(Language language) {
            var config = ConfigurationManager.OpenExeConfiguration(ExePath).Setup();
            if (config.AppSettings.Settings[nameof(language)].Value != language.ToString()) {
                config.AppSettings.Settings.Remove(nameof(language));
                config.AppSettings.Settings.Add(nameof(language), language.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public static BitmapScalingMode? FindScalingMode() {
            var config = ConfigurationManager.OpenExeConfiguration(ExePath).Setup();
            BitmapScalingMode scalingMode;
            string value = config.AppSettings.Settings[nameof(scalingMode)]?.Value;
            if (value != null && Enum.TryParse(value, out scalingMode) && Modes.Contains(scalingMode)) {
                return scalingMode;
            } else {
                return null;
            }
        }

        public static void SetScalingMode(BitmapScalingMode scalingMode) {
            var config = ConfigurationManager.OpenExeConfiguration(ExePath).Setup();
            if (config.AppSettings.Settings[nameof(scalingMode)].Value != scalingMode.ToString()) {
                config.AppSettings.Settings.Remove(nameof(scalingMode));
                config.AppSettings.Settings.Add(nameof(scalingMode), scalingMode.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }
    }
}
