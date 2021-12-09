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

        public void SelectNotesDirectory(string directory)
        {
            if (directory == string.Empty) return;

            NotesDirectory = directory;
            DirectoryTextBoxVisible = true;

            SelectButtonLabel = "Change storage folder";
            StartButtonVisible = true;
        }

        public void SaveNotesDirectory()
        {
            _preferencesService.Preferences.NotesDirectory = NotesDirectory;
            _preferencesService.SavePreferences();
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

        private string selectButtonLabel = "Select storage folder";
        private string SelectButtonLabel
        {
            get => selectButtonLabel;
            set => this.RaiseAndSetIfChanged(ref selectButtonLabel, value);
        }
    }
}
