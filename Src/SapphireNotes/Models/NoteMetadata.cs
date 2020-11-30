namespace SapphireNotes.Models
{
    public class NoteMetadata
    {
        public NoteMetadata()
        {
            FontSize = 15;
            FontFamily = Constants.DefaultFontFamily;
        }

        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public int CursorPosition { get; set; }
    }
}
