using System;
using System.Collections.Generic;
using System.ComponentModel;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Services;

namespace SapphireNotes.ViewModels
{
    public class EditNoteViewModel : ViewModelBase, INotifyPropertyChanged
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

            selectedFontIndex = 3;
            SetAvailableFontSizes();
            selectedFontSizeIndex = 5;
        }

        public EditNoteViewModel(INotesService notesService, Note note)
        {
            _notesService = notesService;

            title = "Edit note";
            saveButtonLabel = "Save";
            name = note.Name;
            EditNote = note;

            selectedFontIndex = Array.IndexOf(availableFonts, note.Metadata.FontFamily);
            SetAvailableFontSizes();
            selectedFontSizeIndex = Array.IndexOf(availableFontSizes, note.Metadata.FontSize);
        }

        public Note Create()
        {
            string fontFamily = availableFonts[selectedFontIndex];
            int fontSize = availableFontSizes[selectedFontSizeIndex];

            return _notesService.Create(name, fontFamily, fontSize);
        }

        public (string originalName, Note updatedNote) Update()
        {
            string originalName = EditNote.Name;

            EditNote.Metadata.FontFamily = availableFonts[selectedFontIndex];
            EditNote.Metadata.FontSize = availableFontSizes[selectedFontSizeIndex];

            Note updatedNote = _notesService.Update(name, EditNote);

            return (originalName, updatedNote);
        }

        private void SetAvailableFontSizes()
        {
            var availableFontSizes = new List<int>(37);

            for (var i = 10; i <= 40; i++)
            {
                availableFontSizes.Add(i);
            }

            for (var i = 50; i <= 100; i += 10)
            {
                availableFontSizes.Add(i);
            }

            this.availableFontSizes = availableFontSizes.ToArray();
        }

        private string title;
        private string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
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
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private string[] availableFonts = new string[] { "Arial", "Calibri", "Consolas", "Open Sans", "Roboto", "Verdana" };
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
