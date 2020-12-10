using System;
using System.ComponentModel;
using System.Linq;
using ReactiveUI;
using SapphireNotes.Services;
using SapphireNotes.Utils;

namespace SapphireNotes.ViewModels
{
    public class PreferencesViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IPreferencesService _preferencesService;
        private readonly INotesService _notesService;
        private readonly string _initialNotesDirectory;
        private const string CustomLabel = "[Custom]";
        private readonly int _initialFontIndex;
        private readonly int _initialFontSizeIndex;
        private readonly short[] _autoSaveIntervalValues = new short[]
        {
            0,
            1,
            5,
            10,
            30,
            60
        };

        public PreferencesViewModel(IPreferencesService preferencesService, INotesService notesService)
        {
            _preferencesService = preferencesService;
            _notesService = notesService;

            _initialNotesDirectory = preferencesService.Preferences.NotesDirectory;
            notesDirectory = preferencesService.Preferences.NotesDirectory;

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
                availableFontSizes = Globals.GetAvailableFontSizes().Select(x => x.ToString()).ToArray();
                _initialFontSizeIndex = selectedFontSizeIndex = Array.IndexOf(availableFontSizes, globalFontSize.Value.ToString());
            }
            else
            {
                var fontSizes = Globals.GetAvailableFontSizes();
                availableFontSizes = DropdownUtil.GetOptionsWithFirst(fontSizes, CustomLabel);
            }

            selectedAutoSaveIntervalIndex = Array.IndexOf(_autoSaveIntervalValues, preferencesService.Preferences.AutoSaveInterval);
        }

        public void SetNotesDirectory(string directory)
        {
            if (directory != string.Empty)
            {
                NotesDirectory = directory;
            }
        }

        public bool Save()
        {
            _preferencesService.Preferences.NotesDirectory = notesDirectory;
            _preferencesService.Preferences.AutoSaveInterval = _autoSaveIntervalValues[selectedAutoSaveIntervalIndex];

            _preferencesService.SavePreferences();

            if (moveNotes)
            {
                _notesService.MoveAll(_initialNotesDirectory);
            }

            bool notesAreDirty = _preferencesService.Preferences.NotesDirectory != _initialNotesDirectory;

            if (selectedFontIndex != _initialFontIndex)
            {
                _notesService.SetFontForAll(availableFonts[selectedFontIndex]);
                notesAreDirty = true;
            }

            if (selectedFontSizeIndex != _initialFontSizeIndex)
            {
                _notesService.SetFontSizeForAll(int.Parse(availableFontSizes[selectedFontSizeIndex]));
                notesAreDirty = true;
            }

            return notesAreDirty;
        }

        private string notesDirectory;
        private string NotesDirectory
        {
            get
            {
                return notesDirectory;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref notesDirectory, value);

                if (notesDirectory == _initialNotesDirectory)
                {
                    MoveNotes = false;
                    MoveNotesCheckBoxVisible = false;
                }
                else
                {
                    MoveNotes = true;
                    MoveNotesCheckBoxVisible = true;
                    SaveEnabled = true;
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

        private string[] availableFonts;
        private string[] AvailableFonts
        {
            get => availableFonts;
            set => this.RaiseAndSetIfChanged(ref availableFonts, value);
        }

        private int selectedFontIndex;
        private int SelectedFontIndex
        {
            get
            {
                return selectedFontIndex;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedFontIndex, value);
                SaveEnabled = true;
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
            get
            {
                return selectedFontSizeIndex;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedFontSizeIndex, value);
                SaveEnabled = true;
            }
        }

        private string[] autoSaveIntervalLabels = new string[]
        {
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
            get
            {
                return selectedAutoSaveIntervalIndex;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedAutoSaveIntervalIndex, value);
                SaveEnabled = true;
            }
        }

        private bool saveEnabled;
        private bool SaveEnabled
        {
            get => saveEnabled;
            set => this.RaiseAndSetIfChanged(ref saveEnabled, value);
        }
    }
}
