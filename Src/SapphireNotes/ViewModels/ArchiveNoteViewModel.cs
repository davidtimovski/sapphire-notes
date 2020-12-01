using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class ArchiveNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;
        private readonly Note Note;

        public ArchiveNoteViewModel(INotesService notesService, Note note)
        {
            _notesService = notesService;

            text = $"Are you sure you wish to archive \"{note.Name}\"?";
            Note = note;
        }

        public Note Archive()
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
