using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels;

public class QuickNoteViewModel : ViewModelBase
{
    private readonly INotesService _notesService;
    private readonly IPreferencesService _preferencesService;

    public QuickNoteViewModel() {}
    public QuickNoteViewModel(INotesService notesService, IPreferencesService preferencesService)
    {
        _notesService = notesService;
        _preferencesService = preferencesService;

        _fontFamily = _preferencesService.Preferences.NotesFontFamily;
        _fontSize = _preferencesService.Preferences.NotesFontSize;
    }

    public void Create()
    {
        var noteFontFamily = _preferencesService.Preferences.NotesFontFamily;
        var noteFontSize = _preferencesService.Preferences.NotesFontSize;

        _notesService.CreateQuick(content, noteFontFamily, noteFontSize);
    }

    private string content = string.Empty;
    private string Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    private readonly FontFamily _fontFamily;
    public FontFamily FontFamily => _fontFamily;

    private readonly int _fontSize;
    public int FontSize => _fontSize;
}
