using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.Services;
using SapphireNotes.ViewModels;
using Splat;

namespace SapphireNotes.Views
{
    public class MainWindow : Window
    {
        private readonly List<Window> _windows = new List<Window>();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var newNoteButton = this.FindControl<Button>("newNoteButton");
            newNoteButton.Command = ReactiveCommand.Create(NewNoteButtonClicked);

            var archivedButton = this.FindControl<Button>("archivedButton");
            archivedButton.Command = ReactiveCommand.Create(ArchivedButtonClicked);

            var preferencesButton = this.FindControl<Button>("preferencesButton");
            preferencesButton.Command = ReactiveCommand.Create(PreferencesButtonClicked);

            DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object sender, EventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.NoteEditClicked += Note_Edit;
            vm.NoteArchiveClicked += Note_Archive;
            vm.NoteDeleteClicked += Note_Delete;
        }

        private void Note_Edit(object sender, EventArgs e)
        {
            var window = new EditNoteWindow
            {
                DataContext = new EditNoteViewModel(Locator.Current.GetService<INotesService>(), (sender as NoteViewModel).ToNote()),
                Width = 300,
                Height = 98,
                Topmost = true,
                CanResize = false
            };
            window.Updated += Note_Updated;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }
        private void Note_Updated(object sender, UpdatedNoteEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.UpdateNote(e);
        }

        private void Note_Archive(object sender, EventArgs e)
        {
            var window = new ArchiveNoteWindow
            {
                DataContext = new ArchiveNoteViewModel(Locator.Current.GetService<INotesService>(), (sender as NoteViewModel).ToNote()),
                Topmost = true,
                CanResize = false
            };
            window.Archived += Note_ArchiveConfirmed;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }
        private void Note_ArchiveConfirmed(object sender, ArchivedNoteEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.ArchiveNote(e);
        }

        private void Note_Delete(object sender, EventArgs e)
        {
            var window = new DeleteNoteWindow
            {
                DataContext = new DeleteNoteViewModel(Locator.Current.GetService<INotesService>(), (sender as NoteViewModel).ToNote()),
                Topmost = true,
                CanResize = false
            };
            window.Deleted += Note_DeleteConfirmed;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }
        private void Note_DeleteConfirmed(object sender, DeletedNoteEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.DeleteNote(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (var window in _windows)
            {
                window.Close();
            }

            var vm = (MainWindowViewModel)DataContext;
            vm.OnClosing((int)Width, (int)Height, Position.X, Position.Y);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NewNoteButtonClicked()
        {
            var window = new EditNoteWindow
            {
                DataContext = new EditNoteViewModel(Locator.Current.GetService<INotesService>()),
                Owner = this,
                Topmost = true,
                CanResize = false
            };
            window.Created += NoteCreated;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void NoteCreated(object sender, CreatedNoteEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            NoteViewModel noteVm = vm.AddNote(e.CreatedNote);

            var noteTabControl = this.FindControl<TabControl>("noteTabs");
            noteTabControl.SelectedItem = noteVm;
        }

        private void ArchivedButtonClicked()
        {
            var vm = new ArchivedNotesViewModel(Locator.Current.GetService<INotesService>());
            vm.NoteRestored += NoteRestored;

            var window = new ArchivedNotesWindow
            {
                DataContext = vm,
                Owner = this,
                Topmost = true
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void NoteRestored(object sender, ArchivedNoteRestoredEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            NoteViewModel noteVm = vm.AddNote(e.RestoredNote);

            var noteTabControl = this.FindControl<TabControl>("noteTabs");
            noteTabControl.SelectedItem = noteVm;
        }

        private void PreferencesButtonClicked()
        {
            var window = new PreferencesWindow
            {
                DataContext = new PreferencesViewModel(Locator.Current.GetService<IPreferencesService>(), Locator.Current.GetService<INotesService>()),
                Owner = this,
                Topmost = true,
                CanResize = false
            };
            window.Saved += PreferencesSaved;
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void PreferencesSaved(object sender, PreferencesSavedEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.PreferencesSaved(e.NotesAreDirty);
        }
    }
}
