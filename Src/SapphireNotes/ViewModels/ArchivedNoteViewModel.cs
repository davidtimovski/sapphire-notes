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

        _name = note.Name;
        _content = note.Content;
        _fontFamily = FontFamilyUtil.FontFamilyFromFont(note.Metadata.FontFamily);
        _fontSize = note.Metadata.FontSize;
        if (note.Metadata.Archived != null) _archivedDate = DateTimeUtil.GetRelativeDate(note.Metadata.Archived.Value);
    }

    public Note Note { get; }

    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _content;
    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    private FontFamily _fontFamily;
    public FontFamily FontFamily
    {
        get => _fontFamily;
        set => this.RaiseAndSetIfChanged(ref _fontFamily, value);
    }

    private int _fontSize;
    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    private string _archivedDate;
    public string ArchivedDate
    {
        get => _archivedDate;
        set => this.RaiseAndSetIfChanged(ref _archivedDate, value);
    }
}
