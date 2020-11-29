using System.ComponentModel;
using SapphireNotes.Models;
using SapphireNotes.Services;
using ReactiveUI;

namespace SapphireNotes.ViewModels
{
    public class EditNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;
        private readonly NoteViewModel EditNote;

        public EditNoteViewModel(INotesService notesService)
        {
            _notesService = notesService;

            title = "New note";
            saveButtonLabel = "Create";
            isNew = true;
            name = string.Empty;
        }

        public EditNoteViewModel(INotesService notesService, NoteViewModel note)
        {
            _notesService = notesService;

            title = "Edit note";
            saveButtonLabel = "Save";
            name = note.Name;
            EditNote = note;
        }

        public NoteViewModel Create()
        {
            return _notesService.Create(name);
        }

        public void Update()
        {
            _notesService.Update(name, EditNote);
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
