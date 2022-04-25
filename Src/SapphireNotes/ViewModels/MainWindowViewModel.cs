using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Threading;
using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Services;
using SapphireNotes.Utils;
using SapphireNotes.ViewModels.UserControls;

namespace SapphireNotes.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IPreferencesService _preferencesService;
    private readonly INotesService _notesService;
    private readonly DispatcherTimer _autoSaveTimer = new();

    public MainWindowViewModel() {}
    public MainWindowViewModel(IPreferencesService preferencesService, INotesService notesService)
    {
        _preferencesService = preferencesService;
        _notesService = notesService;

        _preferencesService.Updated += PreferencesUpdated;

        _notesService.Created += NoteCreated;
        _notesService.Updated += NoteUpdated;
        _notesService.Deleted += NoteDeleted;
        _notesService.Restored += NoteRestored;

        LoadNotes();

        _autoSaveTimer.Tick += SaveDirtyNotes;
        SetAutoSaveTimer();

        CtrlW = ReactiveCommand.Create(SelectPreviousNote);
        CtrlE = ReactiveCommand.Create(SelectNextNote);
    }

    public event EventHandler<EventArgs> NoteEditClicked;
    public event EventHandler<EventArgs> NoteDeleteClicked;

    public void SaveDirty()
    {
        SaveDirtyNotes(null, null);
    }

    public void OnClosing(int windowWidth, int windowHeight, int windowPositionX, int windowPositionY)
    {
        _autoSaveTimer.Stop();

        _notesService.SaveAllWithMetadata(Notes.Select(x => x.ToNote()));

        _preferencesService.SaveWindowPreferences(windowWidth, windowHeight, windowPositionX, windowPositionY);
    }

    private ReactiveCommand<Unit, Unit> CtrlW { get; }
    private ReactiveCommand<Unit, Unit> CtrlE { get; }
    private void SelectPreviousNote()
    {
        if (Notes.Count > 1 && SelectedIndex > 0)
        {
            SelectedIndex--;
        }
    }
    private void SelectNextNote()
    {
        if (Notes.Count > 1 && SelectedIndex < Notes.Count - 1)
        {
            SelectedIndex++;
        }
    }

    private void AddNote(Note note, bool select = false)
    {
        var noteVm = new NoteViewModel(note);

        noteVm.EditClicked += Note_EditClicked;
        noteVm.ArchiveClicked += Note_ArchiveClicked;
        noteVm.DeleteClicked += Note_DeleteClicked;
        noteVm.MiddleMouseClicked += Note_MiddleMouseClicked;

        Notes.Insert(0, noteVm);

        if (select)
        {
            Selected = noteVm;
        }

        ShowIntroMessage = false;
    }

    private void PreferencesUpdated(object sender, UpdatedPreferencesEventArgs e)
    {
        SetAutoSaveTimer();

        if (e.NotesDirectoryChanged)
        {
            Notes.Clear();
            LoadNotes();
        }
        else
        {
            if (e.NewFontFamily != null)
            {
                foreach (NoteViewModel noteVm in Notes)
                {
                    noteVm.FontFamily = FontFamilyUtil.FontFamilyFromFont(e.NewFontFamily);
                }
            }

            if (e.NewFontSize.HasValue)
            {
                foreach (var noteVm in Notes)
                {
                    noteVm.FontSize = e.NewFontSize.Value;
                }
            }
        }
    }

    private void NoteCreated(object sender, CreatedNoteEventArgs e)
    {
        AddNote(e.CreatedNote, true);
    }

    private void NoteUpdated(object sender, UpdatedNoteEventArgs e)
    {
        var noteVm = Notes.FirstOrDefault(x => x.Name == e.OriginalName);
        if (noteVm != null)
        {
            noteVm.Name = e.UpdatedNote.Name;
            noteVm.FontFamily = FontFamilyUtil.FontFamilyFromFont(e.UpdatedNote.Metadata.FontFamily);
            noteVm.FontSize = e.UpdatedNote.Metadata.FontSize;
        }
    }

    private void NoteDeleted(object sender, DeletedNoteEventArgs e)
    {
        var noteVm = Notes.FirstOrDefault(x => x.Name == e.DeletedNote.Name);
        if (noteVm != null)
        {
            Notes.Remove(noteVm);
        }
        
        ShowIntroMessage = !Notes.Any();
    }

    private void NoteRestored(object sender, RestoredNoteEventArgs e)
    {
        AddNote(e.RestoredNote, true);
    }

    private void LoadNotes()
    {
        var notes = _notesService.Load();
        foreach (var note in notes)
        {
            AddNote(note);
        }

        ShowIntroMessage = !Notes.Any();
    }

    private void SetAutoSaveTimer()
    {
        _autoSaveTimer.Stop();

        if (_preferencesService.Preferences.AutoSaveInterval != 0)
        {
            _autoSaveTimer.Interval = TimeSpan.FromSeconds(_preferencesService.Preferences.AutoSaveInterval);
            _autoSaveTimer.Start();
        }
    }

    private void Note_EditClicked(object sender, EventArgs e)
    {
        NoteEditClicked?.Invoke(sender, e);
    }

    private void Note_ArchiveClicked(object sender, EventArgs e)
    {
        ArchiveNote(sender as NoteViewModel);
    }

    private void Note_DeleteClicked(object sender, EventArgs e)
    {
        NoteDeleteClicked?.Invoke(sender, e);
    }

    private void Note_MiddleMouseClicked(object sender, EventArgs e)
    {
        ArchiveNote(sender as NoteViewModel);
    }

    private void ArchiveNote(NoteViewModel noteVm)
    {
        _notesService.Archive(noteVm.ToNote());
        Notes.Remove(noteVm);

        ShowIntroMessage = !Notes.Any();
    }

    private void SaveDirtyNotes(object sender, EventArgs e)
    {
        var dirtyNotesVMs = Notes.Where(x => x.IsDirty).ToList();

        _notesService.SaveAll(dirtyNotesVMs.Select(x => x.ToNote()));

        dirtyNotesVMs.ForEach(x => x.SetPristine());
    }

    private ObservableCollection<NoteViewModel> Notes { get; } = new();

    private bool _showIntroMessage;
    public bool ShowIntroMessage
    {
        get => _showIntroMessage;
        set => this.RaiseAndSetIfChanged(ref _showIntroMessage, value);
    }
    
    private NoteViewModel _selected;
    public NoteViewModel Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }

    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }
}
