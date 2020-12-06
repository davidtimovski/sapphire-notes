namespace SapphireNotes.Models
{
    public class NoteMetadata
    {
        public NoteMetadata() : this(Constants.DefaultFontFamily, 15)
        {
        }

        public NoteMetadata(string fontFamily, int fontSize)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
        }

        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public int CursorPosition { get; set; }
    }
}
