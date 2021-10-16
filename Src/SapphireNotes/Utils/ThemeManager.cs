using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml.Styling;

namespace SapphireNotes.Utils
{
    public static class ThemeManager
    {
        private static readonly string[] GlobalStyles = { "Button", "CheckBox", "ComboBox", "ContextMenu", "Global", "ListBox", "Menu", "ScrollBar", "TextBlock", "TextBox" };
        private static readonly string[] ThemeOverrides = { "Accents", "Resources", "MainWindow" };
        private static readonly Dictionary<string, int> ThemeBackgroundCount = new()
        {
            { "Dark", 0 },
            { "Light", 0 },
            { "Cosmos", 4 }
        };
        private static readonly Random Random = new();

        public static string[] Themes => ThemeBackgroundCount.Keys.ToArray();

        public static IEnumerable<StyleInclude> GetThemeOverrides(string theme)
        {
            var result = new List<StyleInclude>();

            var themeIncludes = ThemeOverrides.Select(x => new StyleInclude(new Uri("resm:Styles?assembly=SapphireNotes"))
            {
                Source = new Uri($"avares://SapphireNotes/Styles/Themes/{theme}/{x}.axaml")
            });
            result.AddRange(themeIncludes);

            int backgrounds = ThemeBackgroundCount[theme];
            if (backgrounds > 0)
            {
                int backgroundIndex = Random.Next(0, backgrounds);
                result.Add(new StyleInclude(new Uri("resm:Styles?assembly=SapphireNotes"))
                {
                    Source = new Uri($"avares://SapphireNotes/Styles/Themes/{theme}/Backgrounds/{backgroundIndex}.axaml")
                });
            }

            return result;
        }

        public static IEnumerable<StyleInclude> GetGlobalStyles()
        {
            return GlobalStyles.Select(x => new StyleInclude(new Uri("resm:Styles?assembly=SapphireNotes"))
            {
                Source = new Uri($"avares://SapphireNotes/Styles/{x}.axaml")
            });
        }
    }
}
