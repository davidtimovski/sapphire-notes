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
        private readonly INotesService _notesService;
        private readonly List<Window> _windows = new List<Window>();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            _notesService = Locator.Current.GetService<INotesService>();
            _notesService.Created += NoteCreated;
            _notesService.Restored += NoteRestored;

            var newNoteButton = this.FindControl<Button>("newNoteButton");
            newNoteButton.Command = ReactiveCommand.Create(NewNoteButtonClicked);

            var quickNoteButton = this.FindControl<Button>("quickNoteButton");
            quickNoteButton.Command = ReactiveCommand.Create(QuickNoteButtonClicked);

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
            vm.NoteDeleteClicked += Note_Delete;
        }

        private void Note_Edit(object sender, EventArgs e)
        {
            var window = new EditNoteWindow
            {
                DataContext = new EditNoteViewModel(_notesService, (sender as NoteViewModel).ToNote()),
                Width = 300,
                Height = 98,
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void Note_Delete(object sender, EventArgs e)
        {
            var window = new DeleteNoteWindow
            {
                DataContext = new DeleteNoteViewModel(_notesService, (sender as NoteViewModel).ToNote()),
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
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
                DataContext = new EditNoteViewModel(_notesService),
                Owner = this,
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void QuickNoteButtonClicked()
        {
            var window = new QuickNoteWindow
            {
                DataContext = new QuickNoteViewModel(_notesService),
                Owner = this,
                Topmost = true,
                CanResize = false
            };
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
            var window = new ArchivedNotesWindow
            {
                DataContext = new ArchivedNotesViewModel(_notesService),
                Owner = this,
                Topmost = true
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void NoteRestored(object sender, RestoredNoteEventArgs e)
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
                DataContext = new PreferencesViewModel(Locator.Current.GetService<IPreferencesService>(), _notesService),
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
