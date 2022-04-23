using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.ViewModels;

public class ArchivedNoteViewModel : ViewModelBase
{
    public ArchivedNoteViewModel(Note note)
    {
        Note = note;

        name = note.Name;
        content = note.Content;
        fontFamily = FontFamilyUtil.FontFamilyFromFont(note.Metadata.FontFamily);
        fontSize = note.Metadata.FontSize;
        archivedDate = DateTimeUtil.GetRelativeDate(note.Metadata.Archived.Value);
    }

    public Note Note { get; }

    private string name;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private string content;
    public string Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    private FontFamily fontFamily;
    public FontFamily FontFamily
    {
        get => fontFamily;
        set => this.RaiseAndSetIfChanged(ref fontFamily, value);
    }

    private int fontSize;
    public int FontSize
    {
        get => fontSize;
        set => this.RaiseAndSetIfChanged(ref fontSize, value);
    }

    private string archivedDate;
    public string ArchivedDate
    {
        get => archivedDate;
        set => this.RaiseAndSetIfChanged(ref archivedDate, value);
    }
}
