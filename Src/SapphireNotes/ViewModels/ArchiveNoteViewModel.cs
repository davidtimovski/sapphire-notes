using System.ComponentModel;
using SapphireNotes.Models;
using SapphireNotes.Services;
using ReactiveUI;

namespace SapphireNotes.ViewModels
{
    public class ArchiveNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;
        private readonly NoteViewModel Note;

        public ArchiveNoteViewModel(INotesService notesService, NoteViewModel note)
        {
            _notesService = notesService;

            text = $"Are you sure you wish to archive \"{note.Name}\"?";
            Note = note;
        }

        public NoteViewModel Archive()
        {
            _notesService.Archive(Note);
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
