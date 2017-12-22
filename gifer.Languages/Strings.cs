using System.Collections.Generic;
using System.Linq;

namespace gifer.Languages {
    public class Strings {
        public static Dictionary<string, string> StringsByName = new string[][] {
            new[] { "EN_DefaultMessage", "[Drag GIF/Image Here]" },
            new[] { "RU_DefaultMessage", "[Перетащите GIF/Картинку Сюда]" },
            new[] { "EN_Settings_LanguageLabel", "Language:" },
            new[] { "RU_Settings_LanguageLabel", "Язык:" },
            new[] { "EN_Settings_RenderingModeLabel", "Rendering:" },
            new[] { "RU_Settings_RenderingModeLabel", "Сглаживание:" },
            new[] { "RU_Settings_RenderingModeFant", "Fant (Качественное)" },
            new[] { "RU_Settings_RenderingModeLinear", "Линейное (Быстрое)" },
            new[] { "RU_Settings_RenderingModeNearestNeighbor", "Ближайший сосед (Оригинальные пиксели)" },
            new[] { "EN_Settings_Title", "[S]ettings" },
            new[] { "RU_Settings_Title", "[S] Настройки" },
            new[] { "Settings_Language_EN", "English" },
            new[] { "Settings_Language_RU", "Русский" }
        }
        .ToDictionary(i => i[0], i => i[1]);
    }
}
