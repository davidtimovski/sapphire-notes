using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class InitialSetupViewModel : ViewModelBase
    {
        private readonly IPreferencesService _preferencesService;

        public InitialSetupViewModel(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        public void SetNotesDirectory(string directory)
        {
            if (directory == string.Empty) return;
            
            DirectoryTextBoxVisible = true;

            _preferencesService.Preferences.NotesDirectory = directory;
            _preferencesService.SavePreferences();

            NotesDirectory = _preferencesService.Preferences.NotesDirectory;
            SelectButtonLabel = "Change storage folder";
            StartButtonVisible = true;
        }

        private bool directoryTextBoxVisible;
        private bool DirectoryTextBoxVisible
        {
            get => directoryTextBoxVisible;
            set => this.RaiseAndSetIfChanged(ref directoryTextBoxVisible, value);
        }

        private bool startButtonVisible;
        private bool StartButtonVisible
        {
            get => startButtonVisible;
            set => this.RaiseAndSetIfChanged(ref startButtonVisible, value);
        }

        private string notesDirectory;
        private string NotesDirectory
        {
            get => notesDirectory;
            set => this.RaiseAndSetIfChanged(ref notesDirectory, value);
        }

        private string selectButtonLabel = "Choose storage folder";
        private string SelectButtonLabel
        {
            get => selectButtonLabel;
            set => this.RaiseAndSetIfChanged(ref selectButtonLabel, value);
        }
    }
}
