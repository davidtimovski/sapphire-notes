using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml.Styling;

namespace SapphireNotes.Utils
{
    public static class ThemeManager
    {
        private static readonly Dictionary<string, ThemeResources> _themeResources = new Dictionary<string, ThemeResources>
        {
            { "Dark", new ThemeResources(new string[] { "Accents", "Button", "MainWindow" }, 0) },
            { "Cosmos", new ThemeResources(new string[] { "Accents", "Button", "MainWindow" }, 5) }
        };
        private static readonly Random _random = new Random();

        public static string[] Themes => _themeResources.Keys.ToArray();

        public static IEnumerable<StyleInclude> GetThemeStyles(string theme)
        {
            var styles = _themeResources[theme].Styles;
            int backgrounds = _themeResources[theme].Backgrounds;
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
                int backgroundIndex = _random.Next(0, backgrounds);
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
        public ThemeResources(string[] styles, int backgrounds)
        {
            Styles = styles;
            Backgrounds = backgrounds;
        }

        public string[] Styles { get; private set; }
        public int Backgrounds { get; private set; }
    }
}
