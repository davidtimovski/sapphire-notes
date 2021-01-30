using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.Services;
using SapphireNotes.ViewModels;
using SapphireNotes.ViewModels.UserControls;
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

            _notesService = Locator.Current.GetService<INotesService>();

            var escapeButton = this.FindControl<Button>("escapeButton");
            escapeButton.Command = ReactiveCommand.Create(ExcapeButtonClicked);

            var newNoteMenuItem = this.FindControl<MenuItem>("newNoteMenuItem");
            newNoteMenuItem.Command = ReactiveCommand.Create(NewNoteMenuItemClicked);

            var quickNoteMenuItem = this.FindControl<MenuItem>("quickNoteMenuItem");
            quickNoteMenuItem.Command = ReactiveCommand.Create(QuickNoteMenuItemClicked);

            var archivedMenuItem = this.FindControl<MenuItem>("archivedMenuItem");
            archivedMenuItem.Command = ReactiveCommand.Create(ArchivedMenuItemClicked);

            var preferencesMenuItem = this.FindControl<MenuItem>("preferencesMenuItem");
            preferencesMenuItem.Command = ReactiveCommand.Create(PreferencesMenuItemClicked);

            var tipsMenuItem = this.FindControl<MenuItem>("tipsMenuItem");
            tipsMenuItem.Command = ReactiveCommand.Create(TipsMenuItemClicked);

            var aboutMenuItem = this.FindControl<MenuItem>("aboutMenuItem");
            aboutMenuItem.Command = ReactiveCommand.Create(AboutMenuItemClicked);

            DataContextChanged += MainWindow_DataContextChanged;
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

            var vm = DataContext as MainWindowViewModel;
            vm.OnClosing((int)Width, (int)Height, Position.X, Position.Y);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ExcapeButtonClicked()
        {
            Close();
        }

        private void NewNoteMenuItemClicked()
        {
            var window = new EditNoteWindow
            {
                DataContext = new EditNoteViewModel(_notesService),
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void QuickNoteMenuItemClicked()
        {
            var window = new QuickNoteWindow
            {
                DataContext = new QuickNoteViewModel(_notesService),
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void ArchivedMenuItemClicked()
        {
            var window = new ArchivedNotesWindow
            {
                DataContext = new ArchivedNotesViewModel(_notesService),
                Topmost = true
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void PreferencesMenuItemClicked()
        {
            var vm = DataContext as MainWindowViewModel;
            vm.SaveDirty();

            var window = new PreferencesWindow
            {
                DataContext = new PreferencesViewModel(Locator.Current.GetService<IPreferencesService>(), _notesService),
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void TipsMenuItemClicked()
        {
            var window = new TipsWindow
            {
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void AboutMenuItemClicked()
        {
            var window = new AboutWindow
            {
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();

            _windows.Add(window);
        }

        private void MainWindow_DataContextChanged(object sender, EventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            vm.NoteEditClicked += Note_Edit;
            vm.NoteDeleteClicked += Note_Delete;
        }
    }
}
