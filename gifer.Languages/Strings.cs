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
            new[] { "EN_Settings_RenderingModeFant", "Fand (Quality)" }, // Use very high quality Fant bitmap scaling, which is slower than all other bitmap scaling modes, but produces higher quality output
            new[] { "RU_Settings_RenderingModeFant", "Fant (Качественное)" },
            new[] { "EN_Settings_RenderingModeLinear", "Linear (Fast)" }, // Use linear bitmap scaling, which is faster than HighQuality mode, but produces lower quality output
            new[] { "RU_Settings_RenderingModeLinear", "Линейное (Быстрое)" },
            new[] { "EN_Settings_RenderingModeNearestNeighbor", "Nearest neighbot (Raw pixels)" }, // Use nearest-neighbor bitmap scaling, which provides performance benefits over LowQuality mode when the software rasterizer is used. This mode is often used to magnify a bitmap
            new[] { "RU_Settings_RenderingModeNearestNeighbor", "Ближайший сосед (Оригинальные пиксели)" },
            new[] { "EN_Settings_Title", "[S]ettings" },
            new[] { "RU_Settings_Title", "[S] Настройки" },
            new[] { "EN_Help_Title", "[H]elp" },
            new[] { "RU_Help_Title", "[H] Помощь" },
            new[] { "Settings_Language_EN", "English" },
            new[] { "Settings_Language_RU", "Русский" }
        }
        .ToDictionary(i => i[0], i => i[1]);
    }
}
