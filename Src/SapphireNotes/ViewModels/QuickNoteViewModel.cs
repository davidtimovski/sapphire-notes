using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class QuickNoteViewModel : ViewModelBase
    {
        private readonly INotesService _notesService;
        private readonly IPreferencesService _preferencesService;

        public QuickNoteViewModel() {}
        public QuickNoteViewModel(INotesService notesService, IPreferencesService preferencesService)
        {
            _notesService = notesService;
            _preferencesService = preferencesService;

            fontFamily = _preferencesService.Preferences.NotesFontFamily;
            fontSize = _preferencesService.Preferences.NotesFontSize;
        }

        public void Create()
        {
            var fontFamily = _preferencesService.Preferences.NotesFontFamily;
            var fontSize = _preferencesService.Preferences.NotesFontSize;

            _notesService.CreateQuick(content, fontFamily, fontSize);
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

        private readonly FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get => fontFamily;
        }

        private readonly int fontSize;
        public int FontSize
        {
            get => fontSize;
        }
    }
}
