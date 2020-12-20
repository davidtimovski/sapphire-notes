using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class QuickNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INotesService _notesService;

        public QuickNoteViewModel(INotesService notesService)
        {
            _notesService = notesService;
        }

        public void Create()
        {
            _notesService.CreateQuick(text);
        }

        private string text = string.Empty;
        private string Text
        {
            get
            {
                return text;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref text, value);
            }
        }
    }
}
