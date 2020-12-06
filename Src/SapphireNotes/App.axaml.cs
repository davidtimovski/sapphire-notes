using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SapphireNotes.DependencyInjection;
using SapphireNotes.Models;
using SapphireNotes.Services;
using SapphireNotes.ViewModels;
using SapphireNotes.Views;
using Splat;

namespace SapphireNotes
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private IClassicDesktopStyleApplicationLifetime _desktop;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _desktop = desktop;

                var preferencesService = Locator.Current.GetRequiredService<IPreferencesService>();

                bool notesDirectorySet = preferencesService.Load();
                if (notesDirectorySet)
                {
                    OpenMainWindow(preferencesService.Preferences);
                }
                else
                {
                    var window = new InitialSetupWindow
                    {
                        DataContext = new InitialSetupViewModel(preferencesService),
                        Topmost = true,
                        CanResize = false
                    };
                    window.Saved += InitialSetup_Saved;
                    window.Show();
                    window.Activate();
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitialSetup_Saved(object sender, EventArgs e)
        {
            var preferencesService = Locator.Current.GetRequiredService<IPreferencesService>();
            preferencesService.SavePreferences();

            OpenMainWindow(preferencesService.Preferences);
        }

        private void OpenMainWindow(Preferences preferences)
        {
            _desktop.MainWindow = new MainWindow
            {
                Width = preferences.Window.Width,
                Height = preferences.Window.Height,
                MinWidth = Preferences.WindowMinWidth,
                MinHeight = Preferences.WindowMinHeight,
                Position = new PixelPoint(preferences.Window.PositionX, preferences.Window.PositionY),
                DataContext = Locator.Current.GetRequiredService<MainWindowViewModel>()
            };
            _desktop.MainWindow.Show();
        }
    }
}
