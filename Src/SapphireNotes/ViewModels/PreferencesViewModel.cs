using Avalonia.Media;
using SapphireNotes.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.ViewModels
{
    public class PreferencesViewModel
    {
        public PreferencesViewModel(Preferences preferences)
        {
            FontFamily = FontUtil.FontFamilyFromFont(preferences.FontFamily);
            FontSize = preferences.FontSize;
            AutoSaveInterval = preferences.AutoSaveInterval;
        }

        public FontFamily FontFamily { get; set; }
        public short FontSize { get; set; }
        public short AutoSaveInterval { get; set; }
    }
}
