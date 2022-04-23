using System;
using Avalonia.Media;

namespace SapphireNotes.Utils;

public static class FontFamilyUtil
{
    public static FontFamily FontFamilyFromFont(string font)
    {
        return FontFamily.Parse(font, new Uri("resm:SapphireNotes.Assets.Fonts?assembly=SapphireNotes"));
    }
}
