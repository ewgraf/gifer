using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace gifer.Utils {
    public static class ConfigHelper {
        private readonly static BitmapScalingMode[] Modes = (BitmapScalingMode[])Enum.GetValues(typeof(BitmapScalingMode));
        private readonly static Language[] Languages = (Language[])Enum.GetValues(typeof(Language));
        private readonly static string ExePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\gifer.exe";

        private static Configuration GetConfiguration() {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appdata, "gifer");
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            string configPath = Path.Combine(folder, "gifer.config");
            if (!File.Exists(configPath)) {
                File.WriteAllText(configPath,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
    <startup>
        <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.5.2"" />
    </startup>
</configuration>");
            }
            var fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configPath;
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
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
            var config = GetConfiguration();
            return bool.Parse(config.AppSettings.Settings["showHelpAtStartup"].Value);
        }

        public static void SetShowHelpAtStartup(bool showHelpAtStartup) {
            var config = GetConfiguration();
            if (config.AppSettings.Settings["showHelpAtStartup"].Value != showHelpAtStartup.ToString()) {
                config.AppSettings.Settings.Remove("showHelpAtStartup");
                config.AppSettings.Settings.Add("showHelpAtStartup", showHelpAtStartup.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public static Language? FindLanguage() {
            var config = GetConfiguration();
            Language language;
            string value = config.AppSettings.Settings[nameof(language)]?.Value;
            if (value != null && Enum.TryParse(value, out language) && Languages.Contains(language)) {
                return language;
            } else {
                return null;
            }
        }

        public static void SetLanguage(Language language) {
            var config = GetConfiguration();
            if (config.AppSettings.Settings[nameof(language)].Value != language.ToString()) {
                config.AppSettings.Settings.Remove(nameof(language));
                config.AppSettings.Settings.Add(nameof(language), language.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public static BitmapScalingMode? FindScalingMode() {
            var config = GetConfiguration();
            BitmapScalingMode scalingMode;
            string value = config.AppSettings.Settings[nameof(scalingMode)]?.Value;
            if (value != null && Enum.TryParse(value, out scalingMode) && Modes.Contains(scalingMode)) {
                return scalingMode;
            } else {
                return null;
            }
        }

        public static void SetScalingMode(BitmapScalingMode scalingMode) {
            var config = GetConfiguration();
            if (config.AppSettings.Settings[nameof(scalingMode)].Value != scalingMode.ToString()) {
                config.AppSettings.Settings.Remove(nameof(scalingMode));
                config.AppSettings.Settings.Add(nameof(scalingMode), scalingMode.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }
    }
}
