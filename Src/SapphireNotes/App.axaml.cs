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

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var preferencesService = Locator.Current.GetRequiredService<IPreferencesService>();

                desktop.MainWindow = new MainWindow
                {
                    Width = preferencesService.Preferences.Window.Width,
                    Height = preferencesService.Preferences.Window.Height,
                    MinWidth = Preferences.WindowMinWidth,
                    MinHeight = Preferences.WindowMinHeight,
                    Position = new PixelPoint(preferencesService.Preferences.Window.PositionX, preferencesService.Preferences.Window.PositionY),
                    DataContext = Locator.Current.GetRequiredService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
