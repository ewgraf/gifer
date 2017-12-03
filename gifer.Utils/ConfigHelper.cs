using System;
using System.Configuration;
using System.Linq;

namespace gifer.Utils {
    public static class ConfigHelper {
        private readonly static Language[] Languages = (Language[])Enum.GetValues(typeof(Language));

        public static Configuration Setup(this Configuration config) {
            if (!config.AppSettings.Settings.AllKeys.Contains("showHelpAtStartup")) {
                config.AppSettings.Settings.Add("showHelpAtStartup", "true");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (!config.AppSettings.Settings.AllKeys.Contains("language")) {
                config.AppSettings.Settings.Add("language", "RU");
                config.Save(ConfigurationSaveMode.Minimal);
            }
            return config;
        }

        public static bool GetShowHelpAtStartup(this Configuration config) {
            return bool.Parse(config.AppSettings.Settings["showHelpAtStartup"].Value);
        }

        public static void SetShowHelpAtStartup(this Configuration config, bool showHelpAtStartup) {
            if (config.AppSettings.Settings["showHelpAtStartup"].Value != showHelpAtStartup.ToString()) {
                config.AppSettings.Settings.Remove("showHelpAtStartup");
                config.AppSettings.Settings.Add("showHelpAtStartup", showHelpAtStartup.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public static Language? FindLanguage(this Configuration config) {
            Language language;
            string value = config.AppSettings.Settings[nameof(language)]?.Value;
            if (value != null && Enum.TryParse(value, out language) && Languages.Contains(language)) {
                return language;
            } else {
                return null;
            }
        }

        public static void SetLanguage(this Configuration config, Language language) {
            if (config.AppSettings.Settings[nameof(language)].Value != language.ToString()) {
                config.AppSettings.Settings.Remove(nameof(language));
                config.AppSettings.Settings.Add(nameof(language), language.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }
    }
}
