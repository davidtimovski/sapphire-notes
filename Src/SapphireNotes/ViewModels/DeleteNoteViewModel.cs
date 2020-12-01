﻿using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class DeleteNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;
        private readonly Note Note;

        public DeleteNoteViewModel(INotesService notesService, Note note)
        {
            _notesService = notesService;

            text = $"Are you sure you wish to delete \"{note.Name}\"?";
            Note = note;
        }

        public Note Delete()
        {
            _notesService.Delete(Note);
            return Note;
        }

        private string text;
        private string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }
    }
}
