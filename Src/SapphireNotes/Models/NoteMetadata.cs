using System;

namespace SapphireNotes.Models
{
    public class NoteMetadata
    {
        public NoteMetadata() : this(Globals.DefaultFontFamily, Globals.DefaultFontSize, 0, null)
        {
        }

        public NoteMetadata(int caretPosition) : this(Globals.DefaultFontFamily, Globals.DefaultFontSize, caretPosition, null)
        {
        }

        public NoteMetadata(DateTime archived) : this(Globals.DefaultFontFamily, Globals.DefaultFontSize, 0, archived)
        {
        }

        public NoteMetadata(string fontFamily, int fontSize)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
        }

        public NoteMetadata(string fontFamily, int fontSize, int caretPosition)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
            CaretPosition = caretPosition;
        }

        public NoteMetadata(string fontFamily, int fontSize, int caretPosition, DateTime? archived)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
            CaretPosition = caretPosition;
            Archived = archived;
        }

        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public int CaretPosition { get; set; }
        public DateTime? Archived { get; set; }
    }
}
