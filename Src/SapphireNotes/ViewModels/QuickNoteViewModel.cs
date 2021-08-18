using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class QuickNoteViewModel : ViewModelBase
    {
        private readonly INotesService _notesService;

        public QuickNoteViewModel(INotesService notesService)
        {
            _notesService = notesService;
        }

        public void Create()
        {
            _notesService.CreateQuick(content);
        }

        private string content = string.Empty;
        private string Content
        {
            get
            {
                return content;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }
    }
}
