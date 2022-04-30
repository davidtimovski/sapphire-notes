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

        notesDirectory = preferencesService.Preferences.NotesDirectory;

        selectedAutoSaveIntervalIndex = Array.IndexOf(_autoSaveIntervalValues, preferencesService.Preferences.AutoSaveInterval);

        availableThemes = ThemeManager.Themes;
        _initialThemeIndex = selectedThemeIndex = Array.IndexOf(availableThemes, preferencesService.Preferences.Theme);

        string globalFont = _notesService.GetFontThatAllNotesUse();
        if (globalFont != null)
        {
            availableFonts = Globals.AvailableFonts;
            _initialFontIndex = selectedFontIndex = Array.IndexOf(availableFonts, globalFont);
        }
        else
        {
            availableFonts = DropdownUtil.GetOptionsWithFirst(Globals.AvailableFonts, CustomLabel);
        }

        int? globalFontSize = _notesService.GetFontSizeThatAllNotesUse();
        if (globalFontSize.HasValue)
        {
            availableFontSizes = Globals.AvailableFontSizes.Select(x => x.ToString()).ToArray();
            _initialFontSizeIndex = selectedFontSizeIndex = Array.IndexOf(availableFontSizes, globalFontSize.Value.ToString());
        }
        else
        {
            availableFontSizes = DropdownUtil.GetOptionsWithFirst(Globals.AvailableFontSizes, CustomLabel);
        }
    }

    public void Save()
    {
        try
        {
            var preferences =
                new UpdatedPreferencesEventArgs(notesDirectory != _preferencesService.Preferences.NotesDirectory);

            if (preferences.NotesDirectoryChanged && moveNotes)
            {
                _notesService.MoveAll(notesDirectory);
            }

            _preferencesService.Preferences.NotesDirectory = notesDirectory;
            _preferencesService.Preferences.AutoSaveInterval = _autoSaveIntervalValues[selectedAutoSaveIntervalIndex];
            _preferencesService.Preferences.Theme = availableThemes[selectedThemeIndex];

            if (selectedThemeIndex != _initialThemeIndex)
            {
                preferences.NewTheme = _preferencesService.Preferences.Theme;
            }

            if (selectedFontIndex != _initialFontIndex)
            {
                var fontFamily = availableFonts[selectedFontIndex];

                _preferencesService.Preferences.NotesFontFamily = fontFamily;
                preferences.NewFontFamily = fontFamily;
                _notesService.SetFontForAll(fontFamily);
            }

            if (selectedFontSizeIndex != _initialFontSizeIndex)
            {
                var fontSize = int.Parse(availableFontSizes[selectedFontSizeIndex]);

                _preferencesService.Preferences.NotesFontSize = fontSize;
                preferences.NewFontSize = fontSize;
                _notesService.SetFontSizeForAll(fontSize);
            }

            _preferencesService.UpdatePreferences(preferences);
        }
        catch (MoveNotesException ex)
        {
            alert.Show(ex.Message);
        }
    }

    private AlertViewModel alert = new(450);
    private AlertViewModel Alert
    {
        get => alert;
        set => this.RaiseAndSetIfChanged(ref alert, value);
    }

    private string notesDirectory;
    public string NotesDirectory
    {
        get => notesDirectory;
        set
        {
            this.RaiseAndSetIfChanged(ref notesDirectory, value);

            if (notesDirectory == _preferencesService.Preferences.NotesDirectory)
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

    private bool moveNotesCheckBoxVisible;
    private bool MoveNotesCheckBoxVisible
    {
        get => moveNotesCheckBoxVisible;
        set => this.RaiseAndSetIfChanged(ref moveNotesCheckBoxVisible, value);
    }

    private bool moveNotes;
    private bool MoveNotes
    {
        get => moveNotes;
        set => this.RaiseAndSetIfChanged(ref moveNotes, value);
    }

    private string[] autoSaveIntervalLabels = {
        "Never",
        "Every second",
        "Every 5 seconds",
        "Every 10 seconds",
        "Every 30 seconds",
        "Every minute"
    };
    private string[] AutoSaveIntervalLabels
    {
        get => autoSaveIntervalLabels;
        set => this.RaiseAndSetIfChanged(ref autoSaveIntervalLabels, value);
    }

    private int selectedAutoSaveIntervalIndex;
    private int SelectedAutoSaveIntervalIndex
    {
        get => selectedAutoSaveIntervalIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedAutoSaveIntervalIndex, value);
            ApplyEnabled = true;
        }
    }

    private string[] availableThemes;
    private string[] AvailableThemes
    {
        get => availableThemes;
        set => this.RaiseAndSetIfChanged(ref availableThemes, value);
    }

    private int selectedThemeIndex;
    private int SelectedThemeIndex
    {
        get => selectedThemeIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedThemeIndex, value);
            ApplyEnabled = true;
        }
    }

    private string[] availableFonts;
    private string[] AvailableFonts
    {
        get => availableFonts;
        set => this.RaiseAndSetIfChanged(ref availableFonts, value);
    }

    private int selectedFontIndex;
    private int SelectedFontIndex
    {
        get => selectedFontIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedFontIndex, value);
            ApplyEnabled = true;
        }
    }

    private string[] availableFontSizes;
    private string[] AvailableFontSizes
    {
        get => availableFontSizes;
        set => this.RaiseAndSetIfChanged(ref availableFontSizes, value);
    }

    private int selectedFontSizeIndex;
    private int SelectedFontSizeIndex
    {
        get => selectedFontSizeIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedFontSizeIndex, value);
            ApplyEnabled = true;
        }
    }

    private bool applyEnabled;
    private bool ApplyEnabled
    {
        get => applyEnabled;
        set => this.RaiseAndSetIfChanged(ref applyEnabled, value);
    }
}
