using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class ArchivedNotesViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;

        public ArchivedNotesViewModel(INotesService notesService)
        {
            _notesService = notesService;

            var archived = _notesService.LoadArchived();
            archivedNotesExist = archived.Length > 0;

            foreach (Note note in archived)
            {
                ArchivedNotes.Add(new ArchivedNoteViewModel(note));
            }

            Restore = ReactiveCommand.Create(RestoreSelectedNote);
            Delete = ReactiveCommand.Create(DeleteSelectedNote);
            Confirm = ReactiveCommand.Create(ConfirmDelete);
            Cancel = ReactiveCommand.Create(CancelDelete);
        }

        public event EventHandler<ArchivedNoteRestoredEventArgs> NoteRestored;

        private ReactiveCommand<Unit, Unit> Restore { get; }
        private ReactiveCommand<Unit, Unit> Delete { get; }
        private ReactiveCommand<Unit, Unit> Confirm { get; }
        private ReactiveCommand<Unit, Unit> Cancel { get; }

        private void RestoreSelectedNote()
        {
            _notesService.Restore(selected.Note);

            NoteRestored.Invoke(this, new ArchivedNoteRestoredEventArgs
            {
                RestoredNote = selected.Note
            });

            ArchivedNotes.Remove(selected);
            ArchivedNotesExist = ArchivedNotes.Any();

            Selected = null;
        }

        private void DeleteSelectedNote()
        {
            if (selected == null)
            {
                return;
            }

            ShowConfirmPrompt("Are you sure you wish to delete the selected note?");
        }

        private void ConfirmDelete()
        {
            _notesService.Delete(selected.Note);

            ArchivedNotes.Remove(selected);
            ArchivedNotesExist = ArchivedNotes.Any();
            HideConfirmPrompt();
            Selected = null;
        }

        private void CancelDelete()
        {
            HideConfirmPrompt();
            ActionButtonsEnabled = true;
        }

        private void HideConfirmPrompt()
        {
            ConfirmPromptOpacity = 0;
            ConfirmPromptVisible = false;
            ConfirmPromptText = null;
        }

        private bool archivedNotesExist;
        private bool ArchivedNotesExist
        {
            get => archivedNotesExist;
            set => this.RaiseAndSetIfChanged(ref archivedNotesExist, value);
        }

        private bool actionButtonsEnabled;
        private bool ActionButtonsEnabled
        {
            get => actionButtonsEnabled;
            set => this.RaiseAndSetIfChanged(ref actionButtonsEnabled, value);
        }

        private ObservableCollection<ArchivedNoteViewModel> ArchivedNotes { get; set; } = new ObservableCollection<ArchivedNoteViewModel>();

        private ArchivedNoteViewModel selected;
        private ArchivedNoteViewModel Selected
        {
            get 
            { 
                return selected; 
            }
            set 
            {
                this.RaiseAndSetIfChanged(ref selected, value);
                ActionButtonsEnabled = value != null;
                HideConfirmPrompt();
            }
        }

        private bool confirmPromptVisible;
        private bool ConfirmPromptVisible
        {
            get => confirmPromptVisible;
            set => this.RaiseAndSetIfChanged(ref confirmPromptVisible, value);
        }

        private string confirmPromptText;
        private string ConfirmPromptText
        {
            get => confirmPromptText;
            set => this.RaiseAndSetIfChanged(ref confirmPromptText, value);
        }

        private double confirmPromptOpacity;
        private double ConfirmPromptOpacity
        {
            get => confirmPromptOpacity;
            set => this.RaiseAndSetIfChanged(ref confirmPromptOpacity, value);
        }

        private void ShowConfirmPrompt(string text)
        {
            ActionButtonsEnabled = false;
            ConfirmPromptText = text;
            ConfirmPromptVisible = true;
            ConfirmPromptOpacity = 1;
        }
    }

    public class ArchivedNoteRestoredEventArgs : EventArgs
    {
        public Note RestoredNote { get; set; }
    }
}
