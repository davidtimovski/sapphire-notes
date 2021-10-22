using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using SapphireNotes.Contracts.Models;
using SapphireNotes.DependencyInjection;
using SapphireNotes.Services;
using SapphireNotes.Utils;
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
                preferencesService.Updated += PreferencesUpdated;

                bool notesDirectorySet = preferencesService.Load();
                Styles.AddRange(ThemeManager.GetThemeStyles(preferencesService.Preferences.Theme));
                Styles.AddRange(ThemeManager.ComponentStyles);

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
                    window.Started += InitialSetup_Started;
                    window.Show();
                    window.Activate();
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitialSetup_Started(object sender, EventArgs e)
        {
            var preferencesService = Locator.Current.GetRequiredService<IPreferencesService>();
            preferencesService.SavePreferences();

            OpenMainWindow(preferencesService.Preferences);
        }

        private void OpenMainWindow(Preferences preferences)
        {
            _desktop.MainWindow = new MainWindow
            {
                Width = 960,
                Height = 540,
                MinWidth = Preferences.WindowMinWidth,
                MinHeight = Preferences.WindowMinHeight,
                Position = new PixelPoint(preferences.Window.PositionX, preferences.Window.PositionY),
                DataContext = Locator.Current.GetRequiredService<MainWindowViewModel>()
            };
            _desktop.MainWindow.Show();
        }

        private void PreferencesUpdated(object sender, UpdatedPreferencesEventArgs e)
        {
            if (e.NewTheme != null)
            {
                var currentThemeStyles = Styles.Where(x => (x as StyleInclude).Source.AbsolutePath.StartsWith("/Styles/Themes")).ToArray();
                Styles.RemoveAll(currentThemeStyles);

                Styles.AddRange(ThemeManager.GetThemeStyles(e.NewTheme));
            }
        }
    }
}
