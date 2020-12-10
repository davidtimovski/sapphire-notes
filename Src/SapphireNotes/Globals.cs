using System.Collections.Generic;

namespace SapphireNotes
{
    public static class Globals
    {
        public const string ApplicationName = "Sapphire Notes";
        public const string DefaultFontFamily = "Arial";
        public const int DefaultFontSize = 15;
        public static string[] AvailableFonts = new string[] { "Arial", "Calibri", "Consolas", "Open Sans", "Roboto", "Verdana" };

        public static int[] GetAvailableFontSizes()
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

            return availableFontSizes.ToArray();
        }
    }
}
