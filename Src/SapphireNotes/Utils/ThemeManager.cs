using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml.Styling;

namespace SapphireNotes.Utils
{
    public static class ThemeManager
    {
        private static readonly Dictionary<string, ThemeResources> ThemeResources = new()
        {
            { "Dark", new ThemeResources(0) },
            { "Light", new ThemeResources(0) },
            { "Cosmos", new ThemeResources(4) }
        };
        private static readonly Random Random = new();

        public static string[] Themes => ThemeResources.Keys.ToArray();

        public static IEnumerable<StyleInclude> GetThemeStyles(string theme)
        {
            var styles = ThemeResources[theme].Styles;
            int backgrounds = ThemeResources[theme].Backgrounds;
            var result = new List<StyleInclude>(styles.Length + (backgrounds > 0 ? 1 : 0));

            foreach (var style in styles)
            {
                result.Add(new StyleInclude(new Uri("resm:Styles?assembly=SapphireNotes"))
                {
                    Source = new Uri($"avares://SapphireNotes/Styles/Themes/{theme}/{style}.axaml")
                });
            }

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
    }

    public class ThemeResources
    {
        public ThemeResources(int backgrounds)
        {
            Backgrounds = backgrounds;
        }

        public string[] Styles { get; } = { "Accents", "Button", "MainWindow", "ScrollBar" };
        public int Backgrounds { get; }
    }
}
