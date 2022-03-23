using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml.Styling;

namespace SapphireNotes.Utils
{
    public static class ThemeManager
    {
        private static readonly Uri StyleIncludeBaseUri = new("resm:Styles?assembly=SapphireNotes");
        private static readonly Dictionary<string, int> ThemeBackgroundCount = new()
        {
            { "Dark", 0 },
            { "Light", 0 },
            { "Cosmos", 4 }
        };
        private static readonly Random Random = new();

        public static string[] Themes => ThemeBackgroundCount.Keys.ToArray();
        public static readonly StyleInclude[] ComponentStyles = 
        {
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/Accents.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/Button.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/CheckBox.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/ComboBox.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/ContextMenu.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/Global.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/ListBox.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/Menu.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/ScrollBar.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/TextBlock.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/TextBox.axaml") },
            new(StyleIncludeBaseUri) { Source = new Uri("avares://SapphireNotes/Styles/Window.axaml") }
        };

        public static IEnumerable<StyleInclude> GetThemeStyles(string theme)
        {
            var result = new List<StyleInclude>
            {
                new(StyleIncludeBaseUri) { Source = new Uri($"avares://SapphireNotes/Styles/Themes/{theme}/Resources.axaml") }
            };

            int backgrounds = ThemeBackgroundCount[theme];
            if (backgrounds > 0)
            {
                int backgroundIndex = Random.Next(0, backgrounds);
                result.Add(new StyleInclude(StyleIncludeBaseUri)
                {
                    Source = new Uri($"avares://SapphireNotes/Styles/Themes/{theme}/Backgrounds/{backgroundIndex}.axaml")
                });
            }

            return result;
        }
    }
}
