﻿using System;
using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Exceptions;
using SapphireNotes.Services;
using SapphireNotes.ViewModels.UserControls;

namespace SapphireNotes.ViewModels
{
    public class EditNoteViewModel : ViewModelBase
    {
        private readonly INotesService _notesService;
        private readonly Note EditNote;

        public EditNoteViewModel(INotesService notesService)
        {
            _notesService = notesService;

            title = "New note";
            saveButtonLabel = "Create";
            isNew = true;
            name = string.Empty;

            selectedFontIndex = Array.IndexOf(availableFonts, Globals.DefaultFontFamily);
            availableFontSizes = Globals.AvailableFontSizes;
            selectedFontSizeIndex = Array.IndexOf(availableFontSizes, Globals.DefaultFontSize);
        }

        public EditNoteViewModel(INotesService notesService, Note note)
        {
            _notesService = notesService;

            title = "Edit note";
            saveButtonLabel = "Save";
            name = note.Name;
            EditNote = note;

            selectedFontIndex = Array.IndexOf(availableFonts, note.Metadata.FontFamily);
            availableFontSizes = Globals.AvailableFontSizes;
            selectedFontSizeIndex = Array.IndexOf(availableFontSizes, note.Metadata.FontSize);
        }

        public bool Create()
        {
            string fontFamily = availableFonts[selectedFontIndex];
            int fontSize = availableFontSizes[selectedFontSizeIndex];

            try
            {
                _notesService.Create(name, fontFamily, fontSize);
            }
            catch (ValidationException ex)
            {
                alert.Show(ex.Message);
                return false;
            }

            return true;
        }

        public bool Update()
        {
            EditNote.Metadata.FontFamily = availableFonts[selectedFontIndex];
            EditNote.Metadata.FontSize = availableFontSizes[selectedFontSizeIndex];

            try
            {
                _notesService.Update(name, EditNote);
            }
            catch (ValidationException ex)
            {
                alert.Show(ex.Message);
                return false;
            }

            return true;
        }

        private string title;
        private string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        private AlertViewModel alert = new(250);
        private AlertViewModel Alert
        {
            get => alert;
            set => this.RaiseAndSetIfChanged(ref alert, value);
        }

        private string saveButtonLabel;
        private string SaveButtonLabel
        {
            get => saveButtonLabel;
            set => this.RaiseAndSetIfChanged(ref saveButtonLabel, value);
        }

        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set => this.RaiseAndSetIfChanged(ref isNew, value);
        }

        private string name;
        private string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
                alert.Hide();
            }
        }

        private string[] availableFonts = Globals.AvailableFonts;
        private string[] AvailableFonts
        {
            get => availableFonts;
            set => this.RaiseAndSetIfChanged(ref availableFonts, value);
        }

        private int selectedFontIndex;
        private int SelectedFontIndex
        {
            get => selectedFontIndex;
            set => this.RaiseAndSetIfChanged(ref selectedFontIndex, value);
        }

        private int[] availableFontSizes;
        private int[] AvailableFontSizes
        {
            get => availableFontSizes;
            set => this.RaiseAndSetIfChanged(ref availableFontSizes, value);
        }

        private int selectedFontSizeIndex;
        private int SelectedFontSizeIndex
        {
            get => selectedFontSizeIndex;
            set => this.RaiseAndSetIfChanged(ref selectedFontSizeIndex, value);
        }
    }
}
