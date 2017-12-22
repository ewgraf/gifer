using gifer.Utils;

namespace gifer.Languages {
    public class LanguageDictionary {

        public static string GetString(Language lang, string key) {
            return GetString($"{lang}_{key}");
        }

        public static string GetString(string key) => Strings.StringsByName[key];
    }
}
