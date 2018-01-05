namespace gifer.Languages {
    public class LanguageDictionary {

        public static string GetString(string key, Language lang) {
            return GetString($"{key}_{lang}");
        }

        public static string GetString(string key) => Strings.StringsByName[key];
    }
}
