using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using DynamicData;
using ReactiveUI;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels;

public sealed class ArchivedNotesViewModel : ViewModelBase, IDisposable
{
    private readonly INotesService _notesService;
    private readonly List<ArchivedNoteViewModel> _shadowNotes;
    private string _previousSearchText = string.Empty;

    public  ArchivedNotesViewModel() {}

    public ArchivedNotesViewModel(INotesService notesService)
    {
        _notesService = notesService;
        _notesService.Archived += NoteArchived;
        _notesService.Restored += NoteRestored;
        _notesService.Deleted += NoteDeleted;

        var archived = _notesService.LoadArchived();
        _archivedNotesExist = archived.Any();
        _searchFieldEnabled = archived.Length > 1;

        var viewModels = archived.Select(x => new ArchivedNoteViewModel(x)).ToList();
        ArchivedNotes.AddRange(viewModels);

        _shadowNotes = viewModels;

        OnRestoreCommand = ReactiveCommand.Create(RestoreSelectedNote);
        OnDeleteCommand = ReactiveCommand.Create(DeleteSelectedNote);
        OnConfirmCommand = ReactiveCommand.Create(ConfirmDelete);
        OnCancelCommand = ReactiveCommand.Create(CancelDelete);
        OnClearSearchCommand = ReactiveCommand.Create(ClearSearch);
    }

    public void SearchTextChanged()
    {
        var searchedText = SearchText.Trim().ToLowerInvariant();

        if (searchedText != _previousSearchText)
        {
            FilterNotes(searchedText);
        }

        _previousSearchText = searchedText;
    }

    private void FilterNotes(string searchText)
    {
        ArchivedNotes.Clear();

        if (searchText == string.Empty)
        {
            ArchivedNotes.AddRange(_shadowNotes.OrderByDescending(x => x.ArchivedDate));
        }
        else
        {
            var matches = _shadowNotes.Select(x =>
            {
                short score = 0;
                if (x.Name.ToLowerInvariant().Contains(searchText))
                {
                    score += 2;
                }
                if (x.Content.ToLowerInvariant().Contains(searchText))
                {
                    score += 1;
                }

                return new { Score = score, Note = x };
            })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Note.ArchivedDate)
                .Select(x => x.Note);

            ArchivedNotes.AddRange(matches);
        }
    }

    private ReactiveCommand<Unit, Unit> OnRestoreCommand { get; }
    private ReactiveCommand<Unit, Unit> OnDeleteCommand { get; }
    private ReactiveCommand<Unit, Unit> OnConfirmCommand { get; }
    private ReactiveCommand<Unit, Unit> OnCancelCommand { get; }
    private ReactiveCommand<Unit, Unit> OnClearSearchCommand { get; }

    private void NoteArchived(object sender, ArchivedNoteEventArgs e)
    {
        var archivedNoteVm = new ArchivedNoteViewModel(e.ArchivedNote);

        _shadowNotes.Add(archivedNoteVm);

        FilterNotes(SearchText.Trim().ToLowerInvariant());

        ArchivedNotesExist = true;
        SearchFieldEnabled = _shadowNotes.Count > 1;
    }

    private void NoteRestored(object sender, RestoredNoteEventArgs e)
    {
        var restoredVm = ArchivedNotes.First(x => x.Name == e.RestoredNote.Name);
        RemoveNote(restoredVm);
    }

    private void NoteDeleted(object sender, DeletedNoteEventArgs e)
    {
        var deletedVm = ArchivedNotes.FirstOrDefault(x => x.Name == e.DeletedNote.Name);
        if (deletedVm != null)
        {
            RemoveNote(deletedVm);
        }
    }

    private void RestoreSelectedNote()
    {
        _notesService.Restore(_selected.Note);
        Selected = null;
    }

    private void DeleteSelectedNote()
    {
        if (_selected == null)
        {
            return;
        }

        SearchFieldEnabled = false;
        ConfirmPromptVisible = true;
    }

    private void ConfirmDelete()
    {
        _notesService.Delete(_selected.Note);

        HideConfirmDeletePrompt();
        Selected = null;
    }

    private void CancelDelete()
    {
        HideConfirmDeletePrompt();
        ActionButtonsEnabled = true;
    }

    private void ClearSearch()
    {
        SearchText = string.Empty;

        ArchivedNotes.Clear();
        ArchivedNotes.AddRange(_shadowNotes.OrderByDescending(x => x.ArchivedDate));

        _previousSearchText = string.Empty;
    }

    private void RemoveNote(ArchivedNoteViewModel archivedNoteVm)
    {
        _shadowNotes.RemoveAll(x => x.Name == archivedNoteVm.Name);
        ArchivedNotes.Remove(archivedNoteVm);
        ArchivedNotesExist = _shadowNotes.Any();
        SearchFieldEnabled = _shadowNotes.Count > 1;
    }

    private bool _archivedNotesExist;
    private bool ArchivedNotesExist
    {
        get => _archivedNotesExist;
        set => this.RaiseAndSetIfChanged(ref _archivedNotesExist, value);
    }

    private bool _actionButtonsEnabled;
    private bool ActionButtonsEnabled
    {
        get => _actionButtonsEnabled;
        set => this.RaiseAndSetIfChanged(ref _actionButtonsEnabled, value);
    }

    private string _searchText = string.Empty;
    private string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            ClearSearchEnabled = _searchText.Trim().Length > 0;
        }
    }

    private bool _searchFieldEnabled;
    private bool SearchFieldEnabled
    {
        get => _searchFieldEnabled;
        set => this.RaiseAndSetIfChanged(ref _searchFieldEnabled, value);
    }

    private bool _clearSearchEnabled;
    private bool ClearSearchEnabled
    {
        get => _clearSearchEnabled;
        set => this.RaiseAndSetIfChanged(ref _clearSearchEnabled, value);
    }

    private ObservableCollection<ArchivedNoteViewModel> ArchivedNotes { get; } = new ObservableCollection<ArchivedNoteViewModel>();

    private ArchivedNoteViewModel _selected;
    private ArchivedNoteViewModel Selected
    {
        get => _selected;
        set
        {
            this.RaiseAndSetIfChanged(ref _selected, value);
            ActionButtonsEnabled = value != null;
            HideConfirmDeletePrompt();
        }
    }

    private bool _confirmPromptVisible;
    private bool ConfirmPromptVisible
    {
        get => _confirmPromptVisible;
        set => this.RaiseAndSetIfChanged(ref _confirmPromptVisible, value);
    }

    private string _confirmPromptText;
    private string ConfirmPromptText
    {
        get => _confirmPromptText;
        set => this.RaiseAndSetIfChanged(ref _confirmPromptText, value);
    }

    private void HideConfirmDeletePrompt()
    {
        ConfirmPromptVisible = false;
        ConfirmPromptText = null;
        SearchFieldEnabled = _shadowNotes.Count > 1;
    }

    public void Dispose()
    {
        _notesService.Archived -= NoteArchived;
        _notesService.Restored -= NoteRestored;
        _notesService.Deleted -= NoteDeleted;
    }
}
