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
                SetThemeOverrides(preferencesService.Preferences.Theme);
                SetGlobalStyles();

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
                Width = preferences.Window.Width,
                Height = preferences.Window.Height,
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
                var themeOverrides = Styles.Where(x => (x as StyleInclude).Source.AbsolutePath.StartsWith("/Styles/Themes")).ToArray();
                Styles.RemoveAll(themeOverrides);

                SetThemeOverrides(e.NewTheme);
            }
        }

        private void SetGlobalStyles()
        {
            var globalStyles = ThemeManager.GetGlobalStyles();
            Styles.AddRange(globalStyles);
        }

        private void SetThemeOverrides(string theme)
        {
            var themeOverrides = ThemeManager.GetThemeOverrides(theme);
            Styles.AddRange(themeOverrides);
        }
    }
}
