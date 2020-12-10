namespace SapphireNotes.Models
{
    public class NoteMetadata
    {
        public NoteMetadata() : this(Globals.DefaultFontFamily, Globals.DefaultFontSize)
        {
        }

        public NoteMetadata(string fontFamily, int fontSize)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
        }

        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public int CaretPosition { get; set; }
    }
}
