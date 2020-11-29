using SapphireNotes.Services;
using SapphireNotes.ViewModels;
using Splat;

namespace SapphireNotes.DependencyInjection
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterServices(services, resolver);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IPreferencesService>(() => new PreferencesService());

            var preferencesService = resolver.GetService<IPreferencesService>();
            services.RegisterLazySingleton<INotesService>(() => new NotesService(preferencesService.Preferences));
        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.Register(() => new MainWindowViewModel(
               resolver.GetService<IPreferencesService>(),
               resolver.GetService<INotesService>()
           ));
        }
    }
}
