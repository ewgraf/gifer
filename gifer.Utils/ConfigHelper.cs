using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifer.Utils {
    public static class ConfigHelper {
        public static Configuration Setup(this Configuration config) {
            if (!config.AppSettings.Settings.AllKeys.Contains("showHelpAtStartup")) {
                config.AppSettings.Settings.Add("showHelpAtStartup", "true");
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
    }
}
