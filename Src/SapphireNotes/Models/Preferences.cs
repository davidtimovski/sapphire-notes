namespace SapphireNotes.Models
{
    public class Preferences
    {
        public Preferences(string notesDirectory)
        {
            NotesDirectory = notesDirectory;
        }

        public const int WindowMinWidth = 320;
        public const int WindowMinHeight = 180;

        public string NotesDirectory { get; set; }
        public string FontFamily { get; set; } = Constants.DefaultFontFamily;
        public short FontSize { get; set; } = 15;
        public short AutoSaveInterval { get; set; } = 10;
        public WindowPreferences Window { get; set; } = new WindowPreferences();
    }

    public class WindowPreferences
    {
        public int Width { get; set; } = 960;
        public int Height { get; set; } = 540;
        public int PositionX { get; set; } = 640;
        public int PositionY { get; set; } = 360;
    }
}
