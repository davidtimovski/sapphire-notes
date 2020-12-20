using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public MainWindowViewModel(IPreferencesService preferencesService, INotesService notesService)
        {
            _preferencesService = preferencesService;
            _notesService = notesService;

            LoadNotes();

            autoSaveTimer.Tick += new EventHandler(SaveDirtyNotes);
            SetAutoSaveTimer();
        }

        public event EventHandler<EventArgs> NoteEditClicked;
        public event EventHandler<EventArgs> NoteDeleteClicked;

        public NoteViewModel AddNote(Note note)
        {
            var noteVm = new NoteViewModel(note);

            noteVm.EditClicked += Note_EditClicked;
            noteVm.ArchiveClicked += Note_ArchiveClicked;
            noteVm.DeleteClicked += Note_DeleteClicked;
            noteVm.MiddleMouseClicked += Note_MiddleMouseClicked;

            Notes.Insert(0, noteVm);

            return noteVm;
        }

        public void UpdateNote(UpdatedNoteEventArgs e)
        {
            NoteViewModel noteVm = Notes.FirstOrDefault(x => x.Name == e.OriginalName);
            if (noteVm != null)
            {
                noteVm.Name = e.UpdatedNote.Name;
                noteVm.FontFamily = FontFamilyUtil.FontFamilyFromFont(e.UpdatedNote.Metadata.FontFamily);

                // Hack to invoke update of FontFamily if FontSize wasn't changed
                // https://github.com/AvaloniaUI/Avalonia/issues/5127
                noteVm.FontSize = e.UpdatedNote.Metadata.FontSize + 1;

                noteVm.FontSize = e.UpdatedNote.Metadata.FontSize;
            }
        }

        public void DeleteNote(DeletedNoteEventArgs e)
        {
            NoteViewModel noteVm = Notes.FirstOrDefault(x => x.Name == e.DeletedNote.Name);
            if (noteVm != null)
            {
                Notes.Remove(noteVm);
            }
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

            IEnumerable<Note> notes = Notes.Select(x => x.ToNote());
            _notesService.SaveDirtyWithMetadata(notes);

            _preferencesService.UpdateWindowSizePreferenceIfChanged(windowWidth, windowHeight, windowPositionX, windowPositionY);
        }

        private void LoadNotes()
        {
            Note[] notes = _notesService.Load();
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

        private void Note_EditClicked(object sender, EventArgs e)
        {
            NoteEditClicked.Invoke(sender, e);
        }

        private void Note_ArchiveClicked(object sender, EventArgs e)
        {
            ArchiveNote(sender as NoteViewModel);
        }

        private void Note_DeleteClicked(object sender, EventArgs e)
        {
            NoteDeleteClicked.Invoke(sender, e);
        }

        private void Note_MiddleMouseClicked(object sender, EventArgs e)
        {
            ArchiveNote(sender as NoteViewModel);
        }

        private void ArchiveNote(NoteViewModel noteVm)
        {
            _notesService.Archive(noteVm.Note);
            Notes.Remove(noteVm);
        }

        private void SaveDirtyNotes(object sender, EventArgs e)
        {
            IEnumerable<NoteViewModel> dirtyNotesVMs = Notes.Where(x => x.Note.IsDirty);
            if (dirtyNotesVMs.Any())
            {
                _notesService.SaveAll(dirtyNotesVMs.Select(x => x.ToNote()));

                foreach (var note in dirtyNotesVMs)
                {
                    note.Note.IsDirty = false;
                }
            }
        }

        private ObservableCollection<NoteViewModel> Notes { get; set; } = new ObservableCollection<NoteViewModel>();
    }
}
