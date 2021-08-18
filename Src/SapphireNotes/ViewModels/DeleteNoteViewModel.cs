using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class DeleteNoteViewModel : ViewModelBase
    {
        private readonly INotesService _notesService;
        private readonly Note Note;

        public DeleteNoteViewModel(INotesService notesService, Note note)
        {
            _notesService = notesService;

            text = $"Are you sure you wish to delete \"{note.Name}\"?";
            Note = note;
        }

        public void Delete()
        {
            _notesService.Delete(Note);
        }

        private string text;
        private string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }
    }
}
