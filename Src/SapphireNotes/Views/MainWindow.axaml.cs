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

            var preferencesButton = this.FindControl<Button>("preferencesButton");
            preferencesButton.Command = ReactiveCommand.Create(PreferencesButtonClicked);
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
            vm.AddNote(e.CreatedNote);
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
