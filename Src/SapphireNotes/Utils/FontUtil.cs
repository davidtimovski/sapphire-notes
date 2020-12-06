using Avalonia.Media;

namespace SapphireNotes.Utils
{
    public static class FontUtil
    {
        public static FontFamily FontFamilyFromFont(string font)
        {
            return new FontFamily("resm:SapphireNotes.Assets.Fonts?assembly=SapphireNotes#" + font);
        }
    }
}
