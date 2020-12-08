using System;
using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class PreferencesViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IPreferencesService _preferencesService;
        private readonly INotesService _notesService;
        private readonly string _initialNotesDirectory;
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

            selectedAutoSaveInteravalIndex = Array.IndexOf(_autoSaveIntervalValues, preferencesService.Preferences.AutoSaveInterval);
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
            _preferencesService.Preferences.AutoSaveInterval = _autoSaveIntervalValues[selectedAutoSaveInteravalIndex];

            _preferencesService.SavePreferences();

            if (moveNotes)
            {
                _notesService.MoveAll(_initialNotesDirectory);
            }

            return _preferencesService.Preferences.NotesDirectory != _initialNotesDirectory;
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

        private int selectedAutoSaveInteravalIndex;
        private int SelectedAutoSaveInteravalIndex
        {
            get
            {
                return selectedAutoSaveInteravalIndex;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedAutoSaveInteravalIndex, value);
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
