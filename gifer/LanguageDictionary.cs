using System.Globalization;
using System.Reflection;
using System.Resources;
using gifer.Utils;

namespace gifer {
    public class LanguageDictionary {
        private static readonly ResourceManager _resources = new ResourceManager("gifer.Languages", Assembly.GetExecutingAssembly());

        public static string GetString(Language lang, string key) {
            return GetString($"{lang}_{key}");
        }

        public static string GetString(string key) {
            return _resources.GetString(key, CultureInfo.InvariantCulture);
        }
    }
}
