namespace SapphireNotes.Contracts.Models
{
    public class Preferences
    {
        public const int WindowMinWidth = 380;
        public const int WindowMinHeight = 180;

        public string NotesDirectory { get; set; } = string.Empty;
        public short AutoSaveInterval { get; set; } = 10;
        public string Theme { get; set; } = "Dark";
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
