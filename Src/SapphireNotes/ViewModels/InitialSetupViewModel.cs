using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels;

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

    private bool _directoryTextBoxVisible;
    private bool DirectoryTextBoxVisible
    {
        get => _directoryTextBoxVisible;
        set => this.RaiseAndSetIfChanged(ref _directoryTextBoxVisible, value);
    }

    private bool _startButtonVisible;
    private bool StartButtonVisible
    {
        get => _startButtonVisible;
        set => this.RaiseAndSetIfChanged(ref _startButtonVisible, value);
    }

    private string _notesDirectory;
    private string NotesDirectory
    {
        get => _notesDirectory;
        set => this.RaiseAndSetIfChanged(ref _notesDirectory, value);
    }

    private string _selectButtonLabel = "Select storage folder";
    private string SelectButtonLabel
    {
        get => _selectButtonLabel;
        set => this.RaiseAndSetIfChanged(ref _selectButtonLabel, value);
    }
}
