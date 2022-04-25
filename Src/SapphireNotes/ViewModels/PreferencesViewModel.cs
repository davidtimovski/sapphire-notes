using System;
using System.Linq;
using ReactiveUI;
using SapphireNotes.Exceptions;
using SapphireNotes.Services;
using SapphireNotes.Utils;
using SapphireNotes.ViewModels.UserControls;

namespace SapphireNotes.ViewModels;

public class PreferencesViewModel : ViewModelBase
{
    private readonly IPreferencesService _preferencesService;
    private readonly INotesService _notesService;
    private readonly int _initialThemeIndex;
    private const string CustomLabel = "[Custom]";
    private readonly int _initialFontIndex;
    private readonly int _initialFontSizeIndex;
    private readonly short[] _autoSaveIntervalValues = {
        0,
        1,
        5,
        10,
        30,
        60
    };

    public PreferencesViewModel() {}
    public PreferencesViewModel(IPreferencesService preferencesService, INotesService notesService)
    {
        _preferencesService = preferencesService;
        _notesService = notesService;

        _notesDirectory = preferencesService.Preferences.NotesDirectory;

        _selectedAutoSaveIntervalIndex = Array.IndexOf(_autoSaveIntervalValues, preferencesService.Preferences.AutoSaveInterval);

        _availableThemes = ThemeManager.Themes;
        _initialThemeIndex = _selectedThemeIndex = Array.IndexOf(_availableThemes, preferencesService.Preferences.Theme);

        var globalFont = _notesService.GetFontThatAllNotesUse();
        if (globalFont != null)
        {
            _availableFonts = Globals.AvailableFonts;
            _initialFontIndex = _selectedFontIndex = Array.IndexOf(_availableFonts, globalFont);
        }
        else
        {
            _availableFonts = DropdownUtil.GetOptionsWithFirst(Globals.AvailableFonts, CustomLabel);
        }

        var globalFontSize = _notesService.GetFontSizeThatAllNotesUse();
        if (globalFontSize.HasValue)
        {
            _availableFontSizes = Globals.AvailableFontSizes.Select(x => x.ToString()).ToArray();
            _initialFontSizeIndex = _selectedFontSizeIndex = Array.IndexOf(_availableFontSizes, globalFontSize.Value.ToString());
        }
        else
        {
            _availableFontSizes = DropdownUtil.GetOptionsWithFirst(Globals.AvailableFontSizes, CustomLabel);
        }
    }

    public void Save()
    {
        try
        {
            var preferences =
                new UpdatedPreferencesEventArgs(_notesDirectory != _preferencesService.Preferences.NotesDirectory);

            if (preferences.NotesDirectoryChanged && _moveNotes)
            {
                _notesService.MoveAll(_notesDirectory);
            }

            _preferencesService.Preferences.NotesDirectory = _notesDirectory;
            _preferencesService.Preferences.AutoSaveInterval = _autoSaveIntervalValues[_selectedAutoSaveIntervalIndex];
            _preferencesService.Preferences.Theme = _availableThemes[_selectedThemeIndex];

            if (_selectedThemeIndex != _initialThemeIndex)
            {
                preferences.NewTheme = _preferencesService.Preferences.Theme;
            }

            if (_selectedFontIndex != _initialFontIndex)
            {
                var fontFamily = _availableFonts[_selectedFontIndex];

                _preferencesService.Preferences.NotesFontFamily = fontFamily;
                preferences.NewFontFamily = fontFamily;
                _notesService.SetFontForAll(fontFamily);
            }

            if (_selectedFontSizeIndex != _initialFontSizeIndex)
            {
                var fontSize = int.Parse(_availableFontSizes[_selectedFontSizeIndex]);

                _preferencesService.Preferences.NotesFontSize = fontSize;
                preferences.NewFontSize = fontSize;
                _notesService.SetFontSizeForAll(fontSize);
            }

            _preferencesService.UpdatePreferences(preferences);
        }
        catch (MoveNotesException ex)
        {
            _alert.Show(ex.Message);
        }
    }

    private AlertViewModel _alert = new(450);
    private AlertViewModel Alert
    {
        get => _alert;
        set => this.RaiseAndSetIfChanged(ref _alert, value);
    }

    private string _notesDirectory;
    public string NotesDirectory
    {
        get => _notesDirectory;
        set
        {
            this.RaiseAndSetIfChanged(ref _notesDirectory, value);

            if (_notesDirectory == _preferencesService.Preferences.NotesDirectory)
            {
                MoveNotes = false;
                MoveNotesCheckBoxVisible = false;
            }
            else
            {
                MoveNotes = true;
                MoveNotesCheckBoxVisible = true;
                ApplyEnabled = true;
            }
        }
    }

    private bool _moveNotesCheckBoxVisible;
    private bool MoveNotesCheckBoxVisible
    {
        get => _moveNotesCheckBoxVisible;
        set => this.RaiseAndSetIfChanged(ref _moveNotesCheckBoxVisible, value);
    }

    private bool _moveNotes;
    private bool MoveNotes
    {
        get => _moveNotes;
        set => this.RaiseAndSetIfChanged(ref _moveNotes, value);
    }

    private string[] _autoSaveIntervalLabels = {
        "Never",
        "Every second",
        "Every 5 seconds",
        "Every 10 seconds",
        "Every 30 seconds",
        "Every minute"
    };
    private string[] AutoSaveIntervalLabels
    {
        get => _autoSaveIntervalLabels;
        set => this.RaiseAndSetIfChanged(ref _autoSaveIntervalLabels, value);
    }

    private int _selectedAutoSaveIntervalIndex;
    private int SelectedAutoSaveIntervalIndex
    {
        get => _selectedAutoSaveIntervalIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAutoSaveIntervalIndex, value);
            ApplyEnabled = true;
        }
    }

    private string[] _availableThemes;
    private string[] AvailableThemes
    {
        get => _availableThemes;
        set => this.RaiseAndSetIfChanged(ref _availableThemes, value);
    }

    private int _selectedThemeIndex;
    private int SelectedThemeIndex
    {
        get => _selectedThemeIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedThemeIndex, value);
            ApplyEnabled = true;
        }
    }

    private string[] _availableFonts;
    private string[] AvailableFonts
    {
        get => _availableFonts;
        set => this.RaiseAndSetIfChanged(ref _availableFonts, value);
    }

    private int _selectedFontIndex;
    private int SelectedFontIndex
    {
        get => _selectedFontIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFontIndex, value);
            ApplyEnabled = true;
        }
    }

    private string[] _availableFontSizes;
    private string[] AvailableFontSizes
    {
        get => _availableFontSizes;
        set => this.RaiseAndSetIfChanged(ref _availableFontSizes, value);
    }

    private int _selectedFontSizeIndex;
    private int SelectedFontSizeIndex
    {
        get => _selectedFontSizeIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFontSizeIndex, value);
            ApplyEnabled = true;
        }
    }

    private bool _applyEnabled;
    private bool ApplyEnabled
    {
        get => _applyEnabled;
        set => this.RaiseAndSetIfChanged(ref _applyEnabled, value);
    }
}
