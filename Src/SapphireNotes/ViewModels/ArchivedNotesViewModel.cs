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
    private string previousSearchText = string.Empty;

    public  ArchivedNotesViewModel() {}

    public ArchivedNotesViewModel(INotesService notesService)
    {
        _notesService = notesService;
        _notesService.Archived += NoteArchived;
        _notesService.Restored += NoteRestored;
        _notesService.Deleted += NoteDeleted;

        var archived = _notesService.LoadArchived();
        archivedNotesExist = archived.Any();
        searchFieldEnabled = archived.Length > 1;

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

        if (searchedText != previousSearchText)
        {
            FilterNotes(searchedText);
        }

        previousSearchText = searchedText;
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
        ArchivedNoteViewModel deletedVm = ArchivedNotes.FirstOrDefault(x => x.Name == e.DeletedNote.Name);
        if (deletedVm != null)
        {
            RemoveNote(deletedVm);
        }
    }

    private void RestoreSelectedNote()
    {
        _notesService.Restore(selected.Note);
        Selected = null;
    }

    private void DeleteSelectedNote()
    {
        if (selected == null)
        {
            return;
        }

        SearchFieldEnabled = false;
        ConfirmPromptVisible = true;
    }

    private void ConfirmDelete()
    {
        _notesService.Delete(selected.Note);

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

        previousSearchText = string.Empty;
    }

    private void RemoveNote(ArchivedNoteViewModel archivedNoteVm)
    {
        _shadowNotes.RemoveAll(x => x.Name == archivedNoteVm.Name);
        ArchivedNotes.Remove(archivedNoteVm);
        ArchivedNotesExist = _shadowNotes.Any();
        SearchFieldEnabled = _shadowNotes.Count > 1;
    }

    private bool archivedNotesExist;
    private bool ArchivedNotesExist
    {
        get => archivedNotesExist;
        set => this.RaiseAndSetIfChanged(ref archivedNotesExist, value);
    }

    private bool actionButtonsEnabled;
    private bool ActionButtonsEnabled
    {
        get => actionButtonsEnabled;
        set => this.RaiseAndSetIfChanged(ref actionButtonsEnabled, value);
    }

    private string searchText = string.Empty;
    private string SearchText
    {
        get => searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref searchText, value);
            ClearSearchEnabled = searchText.Trim().Length > 0;
        }
    }

    private bool searchFieldEnabled;
    private bool SearchFieldEnabled
    {
        get => searchFieldEnabled;
        set => this.RaiseAndSetIfChanged(ref searchFieldEnabled, value);
    }

    private bool clearSearchEnabled;
    private bool ClearSearchEnabled
    {
        get => clearSearchEnabled;
        set => this.RaiseAndSetIfChanged(ref clearSearchEnabled, value);
    }

    private ObservableCollection<ArchivedNoteViewModel> ArchivedNotes { get; } = new ObservableCollection<ArchivedNoteViewModel>();

    private ArchivedNoteViewModel selected;
    private ArchivedNoteViewModel Selected
    {
        get => selected;
        set
        {
            this.RaiseAndSetIfChanged(ref selected, value);
            ActionButtonsEnabled = value != null;
            HideConfirmDeletePrompt();
        }
    }

    private bool confirmPromptVisible;
    private bool ConfirmPromptVisible
    {
        get => confirmPromptVisible;
        set => this.RaiseAndSetIfChanged(ref confirmPromptVisible, value);
    }

    private string confirmPromptText;
    private string ConfirmPromptText
    {
        get => confirmPromptText;
        set => this.RaiseAndSetIfChanged(ref confirmPromptText, value);
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
