using System;

namespace SapphireNotes.Contracts.Models;

public class NoteMetadata
{
    public NoteMetadata() : this(Globals.DefaultNotesFontFamily, Globals.DefaultNotesFontSize, 0, null)
    {
    }

    public NoteMetadata(DateTime archived) : this(Globals.DefaultNotesFontFamily, Globals.DefaultNotesFontSize, 0, archived)
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

    private NoteMetadata(string fontFamily, int fontSize, int caretPosition, DateTime? archived)
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
