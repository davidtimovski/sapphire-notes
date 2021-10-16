using System.Collections.Generic;

namespace SapphireNotes
{
    public static class Globals
    {
        static Globals()
        {
            var availableFontSizes = new List<int>(37);

            for (var i = 10; i <= 40; i++)
            {
                availableFontSizes.Add(i);
            }
            for (var i = 50; i <= 100; i += 10)
            {
                availableFontSizes.Add(i);
            }

            AvailableFontSizes = availableFontSizes.ToArray();
        }

        public const string ApplicationName = "Sapphire Notes";
        public const string ArchivePrefix = "archive";
        public const string DefaultFontFamily = "Open Sans";
        public const int DefaultFontSize = 15;
        public static readonly string[] AvailableFonts = { "Arial", "Calibri", "Consolas", "Open Sans", "Roboto", "Verdana" };
        public static readonly int[] AvailableFontSizes;
    }
}
