using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using SapphireNotes.Models;
using SapphireNotes.Services;
using SapphireNotes.Utils;
using SapphireNotes.Views;

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

            LoadNotes();

            autoSaveTimer.Tick += new EventHandler(SaveDirtyNotes);
            SetAutoSaveTimer();
        }

        public void AddNote(Note note)
        {
            var noteVm = new NoteViewModel(note);

            noteVm.Edited += Note_Edited;
            noteVm.Archived += Note_Archived;
            noteVm.Deleted += Note_Deleted;
            Notes.Add(noteVm);
        }

        public void PreferencesSaved(bool notesAreDirty)
        {
            SetAutoSaveTimer();

            if (notesAreDirty)
            {
                Notes.Clear();
                LoadNotes();
            }
        }

        public void OnClosing(int windowWidth, int windowHeight, int windowPositionX, int windowPositionY)
        {
            autoSaveTimer.Stop();

            // TODO: Not good for MVVM
            foreach (var window in _windows)
            {
                window.Close();
            }

            IEnumerable<Note> notes = Notes.Select(x => x.ToNote());
            _notesService.SaveDirtyWithMetadata(notes);

            _preferencesService.UpdateWindowSizePreferenceIfChanged(windowWidth, windowHeight, windowPositionX, windowPositionY);
        }

        private void LoadNotes()
        {
            Note[] notes = _notesService.LoadAll();
            foreach (var note in notes)
            {
                AddNote(note);
            }
        }

        private void SetAutoSaveTimer()
        {
            autoSaveTimer.Stop();

            if (_preferencesService.Preferences.AutoSaveInterval != 0)
            {
                autoSaveTimer.Interval = TimeSpan.FromSeconds(_preferencesService.Preferences.AutoSaveInterval);
                autoSaveTimer.Start();
            }
        }

        private void Note_Edited(object sender, EventArgs e)
        {
            // TODO: Not good for MVVM
            var window = new EditNoteWindow
            {
                DataContext = new EditNoteViewModel(_notesService, (sender as NoteViewModel).ToNote()),
                Width = 300,
                Height = 98,
                Topmost = true,
                CanResize = false
            };
            window.Updated += Note_Updated;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void Note_Updated(object sender, UpdatedNoteEventArgs e)
        {
            NoteViewModel noteVm = Notes.FirstOrDefault(x => x.Name == e.OriginalName);
            if (noteVm != null)
            {
                noteVm.FilePath = e.UpdatedNote.FilePath;
                noteVm.Name = e.UpdatedNote.Name;
                noteVm.FontFamily = FontFamilyUtil.FontFamilyFromFont(e.UpdatedNote.Metadata.FontFamily);

                // Hack to invoke update of FontFamily if FontSize wasn't changed
                // https://github.com/AvaloniaUI/Avalonia/issues/5127
                noteVm.FontSize = e.UpdatedNote.Metadata.FontSize + 1;

                noteVm.FontSize = e.UpdatedNote.Metadata.FontSize;
            }
        }

        private void Note_Archived(object sender, EventArgs e)
        {
            // TODO: Not good for MVVM
            var window = new ArchiveNoteWindow
            {
                DataContext = new ArchiveNoteViewModel(_notesService, (sender as NoteViewModel).ToNote()),
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
            var noteVm = Notes.FirstOrDefault(x => x.Name == e.Note.Name);
            if (noteVm != null)
            {
                Notes.Remove(noteVm);
            }
        }

        private void Note_Deleted(object sender, EventArgs e)
        {
            // TODO: Not good for MVVM
            var window = new DeleteNoteWindow
            {
                DataContext = new DeleteNoteViewModel(_notesService, (sender as NoteViewModel).ToNote()),
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
            var noteVm = Notes.FirstOrDefault(x => x.Name == e.DeletedNote.Name);
            if (noteVm != null)
            {
                Notes.Remove(noteVm);
            }
        }

        private void SaveDirtyNotes(object sender, EventArgs e)
        {
            IEnumerable<NoteViewModel> dirtyNotesVMs = Notes.Where(x => x.IsDirty);
            if (dirtyNotesVMs.Any())
            {
                _notesService.SaveAll(dirtyNotesVMs.Select(x => x.ToNote()));

                foreach (var note in dirtyNotesVMs)
                {
                    note.IsDirty = false;
                }
            }
        }

        private ObservableCollection<NoteViewModel> Notes { get; set; } = new ObservableCollection<NoteViewModel>();
    }
}
