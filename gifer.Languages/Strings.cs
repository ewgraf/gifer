using System.Collections.Generic;
using System.Linq;

namespace gifer.Languages {
	public class Strings {
		public static Dictionary<string, string> StringsByName = new string[][] {
			new[] { "DefaultMessage_EN", "[Drag GIF/Image Here]" },
			new[] { "DefaultMessage_RU", "[Перетащите GIF/Картинку Сюда]" },
			new[] { "Settings_LanguageLabel_EN", "Language:" },
			new[] { "Settings_LanguageLabel_RU", "Язык:" },
			new[] { "Settings_RenderingModeLabel_EN", "Rendering:" },
			new[] { "Settings_RenderingModeLabel_RU", "Сглаживание:" },
			new[] { "Settings_RenderingModeFant_EN", "Fand (Quality)" }, // Use very high quality Fant bitmap scaling, which is slower than all other bitmap scaling modes, but produces higher quality output
			new[] { "Settings_RenderingModeFant_RU", "Fant (Качественное)" },
			new[] { "Settings_RenderingModeLinear_EN", "Linear (Fast)" }, // Use linear bitmap scaling, which is faster than HighQuality mode, but produces lower quality output
			new[] { "Settings_RenderingModeLinear_RU", "Линейное (Быстрое)" },
			new[] { "Settings_RenderingModeNearestNeighbor_EN", "Nearest neighbot (Raw pixels)" }, // Use nearest-neighbor bitmap scaling, which provides performance benefits over LowQuality mode when the software rasterizer is used. This mode is often used to magnify a bitmap
			new[] { "Settings_RenderingModeNearestNeighbor_RU", "Ближайший сосед (Оригинальные пиксели)" },
			new[] { "Settings_Title_EN", "[S]ettings" },
			new[] { "Settings_Title_RU", "[S] Настройки" },
			new[] { "Help_Title_EN", "[H]elp" },
			new[] { "Help_Title_RU", "[H] Помощь" },
			new[] { "Settings_Language_EN", "English" },
			new[] { "Settings_Language_RU", "Русский" },
			new[] { "Settings_CheckForUpdate_EN", "Check for update" },
			new[] { "Settings_CheckForUpdate_RU", "Проверять наличие обновления" },
			new[] { "Settings_CenterOpenedImage_EN", "Center opened image" },
			new[] { "Settings_CenterOpenedImage_RU", "Центрировать открываемое изображение" }
		}
		.ToDictionary(i => i[0], i => i[1]);
	}
}
