using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Threading;
using SapphireNotes.Models;
using SapphireNotes.Services;
using SapphireNotes.Views;
using ReactiveUI;

namespace SapphireNotes.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IPreferencesService _preferencesService;
        private readonly INotesService _notesService;
        private readonly DispatcherTimer autoSaveTimer = new DispatcherTimer();
        private readonly List<Window> _windows = new List<Window>();

        public MainWindowViewModel(IPreferencesService preferencesService, INotesService notesService)
        {
            _preferencesService = preferencesService;
            _notesService = notesService;

            preferences = _preferencesService.Preferences;

            var notes = _notesService.GetAll();
            foreach (var note in notes)
            {
                AddNote(note);
            }

            if (preferences.AutoSaveInterval != 0)
            {
                autoSaveTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                autoSaveTimer.Interval = TimeSpan.FromSeconds(preferences.AutoSaveInterval);
                autoSaveTimer.Start();
            }
        }

        public void AddNote(NoteViewModel note)
        {
            note.Edited += Note_Edited;
            note.Archived += Note_Archived;
            note.Deleted += Note_Deleted;
            Notes.Add(note);
        }

        public void OnClosing(int windowWidth, int windowHeight, int windowPositionX, int windowPositionY)
        {
            autoSaveTimer.Stop();

            // TODO: Not good for MVVM
            foreach (var window in _windows)
            {
                window.Close();
            }

            _notesService.SaveAll(Notes);

            _preferencesService.UpdateWindowSizePreferenceIfChanged(windowWidth, windowHeight, windowPositionX, windowPositionY);
        }

        private void Note_Edited(object sender, EventArgs e)
        {
            // TODO: Not good for MVVM
            var window = new EditNoteWindow
            {
                DataContext = new EditNoteViewModel(_notesService, sender as NoteViewModel),
                Width = 300,
                Height = 98,
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void Note_Archived(object sender, EventArgs e)
        {
            // TODO: Not good for MVVM
            var window = new ArchiveNoteWindow
            {
                DataContext = new ArchiveNoteViewModel(_notesService, sender as NoteViewModel),
                Topmost = true,
                CanResize = false
            };
            window.Archived += Note_Archive_Confirmed;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void Note_Archive_Confirmed(object sender, ArchivedNoteEventArgs e)
        {
            Notes.Remove(e.Note);
        }

        private void Note_Deleted(object sender, EventArgs e)
        {
            // TODO: Not good for MVVM
            var window = new DeleteNoteWindow
            {
                DataContext = new DeleteNoteViewModel(_notesService, sender as NoteViewModel),
                Topmost = true,
                CanResize = false
            };
            window.Deleted += Note_Delete_Confirmed;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void Note_Delete_Confirmed(object sender, DeletedNoteEventArgs e)
        {
            Notes.Remove(e.Note);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            _notesService.SaveAll(Notes);
        }

        private Preferences preferences;
        private Preferences Preferences
        {
            get => preferences;
            set => this.RaiseAndSetIfChanged(ref preferences, value);
        }

        private ObservableCollection<NoteViewModel> Notes { get; set; } = new ObservableCollection<NoteViewModel>();
    }
}
