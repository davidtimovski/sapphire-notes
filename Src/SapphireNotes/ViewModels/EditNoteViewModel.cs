using System;
using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Exceptions;
using SapphireNotes.Services;
using SapphireNotes.ViewModels.UserControls;

namespace SapphireNotes.ViewModels;

public class EditNoteViewModel : ViewModelBase
{
    private readonly INotesService _notesService;
    private readonly IPreferencesService _preferencesService;
    private readonly Note _editNote;

    public EditNoteViewModel() {}
    public EditNoteViewModel(INotesService notesService, IPreferencesService preferencesService)
    {
        _notesService = notesService;
        _preferencesService = preferencesService;

        _title = "New note";
        _saveButtonLabel = "Create";
        _isNew = true;
        _name = string.Empty;

        _selectedFontIndex = Array.IndexOf(_availableFonts, _preferencesService.Preferences.NotesFontFamily);
        _availableFontSizes = Globals.AvailableFontSizes;
        _selectedFontSizeIndex = Array.IndexOf(_availableFontSizes, _preferencesService.Preferences.NotesFontSize);
    }

    public EditNoteViewModel(INotesService notesService, IPreferencesService preferencesService, Note note)
    {
        _notesService = notesService;
        _preferencesService = preferencesService;

        _title = "Edit note";
        _saveButtonLabel = "Save";
        _name = note.Name;
        _editNote = note;

        _selectedFontIndex = Array.IndexOf(_availableFonts, note.Metadata.FontFamily);
        _availableFontSizes = Globals.AvailableFontSizes;
        _selectedFontSizeIndex = Array.IndexOf(_availableFontSizes, note.Metadata.FontSize);
    }

    public bool Create()
    {
        var fontFamily = _availableFonts[_selectedFontIndex];
        var fontSize = _availableFontSizes[_selectedFontSizeIndex];

        try
        {
            UpdatePreferences(fontFamily, fontSize);

            _notesService.Create(_name, fontFamily, fontSize);
        }
        catch (ValidationException ex)
        {
            _alert.Show(ex.Message);
            return false;
        }

        return true;
    }

    public bool Update()
    {
        _editNote.Metadata.FontFamily = _availableFonts[_selectedFontIndex];
        _editNote.Metadata.FontSize = _availableFontSizes[_selectedFontSizeIndex];

        try
        {
            UpdatePreferences(_editNote.Metadata.FontFamily, _editNote.Metadata.FontSize);

            _notesService.Update(_name, _editNote);
        }
        catch (ValidationException ex)
        {
            _alert.Show(ex.Message);
            return false;
        }

        return true;
    }

    private void UpdatePreferences(string fontFamily, int fontSize)
    {
        _preferencesService.Preferences.NotesFontFamily = fontFamily;
        _preferencesService.Preferences.NotesFontSize = fontSize;
        _preferencesService.SavePreferences();
    }

    private string _title;
    private string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private AlertViewModel _alert = new(300);
    private AlertViewModel Alert
    {
        get => _alert;
        set => this.RaiseAndSetIfChanged(ref _alert, value);
    }

    private string _saveButtonLabel;
    private string SaveButtonLabel
    {
        get => _saveButtonLabel;
        set => this.RaiseAndSetIfChanged(ref _saveButtonLabel, value);
    }

    private bool _isNew;
    public bool IsNew
    {
        get => _isNew;
        set => this.RaiseAndSetIfChanged(ref _isNew, value);
    }

    private string _name;
    private string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            _alert.Hide();
        }
    }

    private string[] _availableFonts = Globals.AvailableFonts;
    private string[] AvailableFonts
    {
        get => _availableFonts;
        set => this.RaiseAndSetIfChanged(ref _availableFonts, value);
    }

    private int _selectedFontIndex;
    private int SelectedFontIndex
    {
        get => _selectedFontIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedFontIndex, value);
    }

    private int[] _availableFontSizes;
    private int[] AvailableFontSizes
    {
        get => _availableFontSizes;
        set => this.RaiseAndSetIfChanged(ref _availableFontSizes, value);
    }

    private int _selectedFontSizeIndex;
    private int SelectedFontSizeIndex
    {
        get => _selectedFontSizeIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedFontSizeIndex, value);
    }
}
