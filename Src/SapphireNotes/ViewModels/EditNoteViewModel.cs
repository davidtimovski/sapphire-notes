using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class EditNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;
        private readonly Note EditNote;

        public EditNoteViewModel(INotesService notesService)
        {
            _notesService = notesService;

            title = "New note";
            saveButtonLabel = "Create";
            isNew = true;
            name = string.Empty;
        }

        public EditNoteViewModel(INotesService notesService, Note note)
        {
            _notesService = notesService;

            title = "Edit note";
            saveButtonLabel = "Save";
            name = note.Name;
            EditNote = note;
        }

        public Note Create()
        {
            return _notesService.Create(name);
        }

        public (string originalName, Note updatedNote) Update()
        {
            string originalName = EditNote.Name;
            Note updatedNote = _notesService.Update(name, EditNote);

            return (originalName, updatedNote);
        }

        private string title;
        private string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        private string saveButtonLabel;
        private string SaveButtonLabel
        {
            get => saveButtonLabel;
            set => this.RaiseAndSetIfChanged(ref saveButtonLabel, value);
        }

        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set => this.RaiseAndSetIfChanged(ref isNew, value);
        }

        private string name;
        private string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
    }
}
