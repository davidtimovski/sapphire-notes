using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels;

public class DeleteNoteViewModel : ViewModelBase
{
    private readonly INotesService _notesService;
    private readonly Note _note;

    public DeleteNoteViewModel() {}
    public DeleteNoteViewModel(INotesService notesService, Note note)
    {
        _notesService = notesService;

        text = $"Are you sure you wish to delete \"{note.Name}\"?";
        _note = note;
    }

    public void Delete()
    {
        _notesService.Delete(_note);
    }

    private string text;
    private string Text
    {
        get => text;
        set => this.RaiseAndSetIfChanged(ref text, value);
    }
}
