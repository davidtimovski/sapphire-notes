using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class DeleteNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;
        private readonly NoteViewModel Note;

        public DeleteNoteViewModel(INotesService notesService, NoteViewModel note)
        {
            _notesService = notesService;

            text = $"Are you sure you wish to delete \"{note.Name}\"?";
            Note = note;
        }

        public NoteViewModel Delete()
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
