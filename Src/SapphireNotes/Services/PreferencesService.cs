using System;
using System.IO;
using System.Runtime.InteropServices;
using SapphireNotes.Contracts.Models;

namespace SapphireNotes.Services
{
    public interface IPreferencesService
    {
        Preferences Preferences { get; }
        bool Load();
        void UpdatePreferences(UpdatedPreferencesEventArgs args);
        void SavePreferences();
        void SaveWindowPreferences(int width, int height, int positionX, int positionY);
        event EventHandler<UpdatedPreferencesEventArgs> Updated;
    }

    public class PreferencesService : IPreferencesService
    {
        private const string PreferencesFileName = "preferences.bin";
        private const int DefaultWindowPositionX = 640;
        private const int DefaultWindowPositionY = 360;
        private string _preferencesFilePath;

        public event EventHandler<UpdatedPreferencesEventArgs> Updated;

        public Preferences Preferences { get; private set; }

        public bool Load()
        {
            string appDataDirectory = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Globals.ApplicationName);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                appDataDirectory = $"/home/{Environment.UserName}/Documents/{Globals.ApplicationName}";
            }

            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }

#if DEBUG
            appDataDirectory = string.Empty;
#endif

            _preferencesFilePath = Path.Combine(appDataDirectory, PreferencesFileName);

            if (File.Exists(_preferencesFilePath))
            {
                ReadPreferences();
                return Preferences.NotesDirectory != string.Empty;
            }

            Preferences = new Preferences();
            SavePreferences();

            return false;
        }

        public void UpdatePreferences(UpdatedPreferencesEventArgs args)
        {
            SavePreferences();
            Updated.Invoke(this, args);
        }

        public void SavePreferences()
        {
            using var writer = new BinaryWriter(File.Open(_preferencesFilePath, FileMode.OpenOrCreate));
            writer.Write(Preferences.NotesDirectory);
            writer.Write(Preferences.AutoSaveInterval);
            writer.Write(Preferences.Theme);
            writer.Write(Preferences.Window.Width);
            writer.Write(Preferences.Window.Height);
            writer.Write(Preferences.Window.PositionX);
            writer.Write(Preferences.Window.PositionY);
        }

        public void SaveWindowPreferences(int width, int height, int positionX, int positionY)
        {
            if (width != Preferences.Window.Width || height != Preferences.Window.Height)
            {
                Preferences.Window.Width = width;
                Preferences.Window.Height = height;
            }

            Preferences.Window.PositionX = positionX > 0 ? positionX : DefaultWindowPositionX;
            Preferences.Window.PositionY = positionY > 0 ? positionY : DefaultWindowPositionY;

            SavePreferences();
        }

        private void ReadPreferences()
        {
            Preferences = new Preferences();

            using var reader = new BinaryReader(File.Open(_preferencesFilePath, FileMode.Open));
            Preferences.NotesDirectory = reader.ReadString();
            Preferences.AutoSaveInterval = reader.ReadInt16();
            Preferences.Theme = reader.ReadString();
            Preferences.Window = new WindowPreferences
            {
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                PositionX = reader.ReadInt32(),
                PositionY = reader.ReadInt32()
            };
        }
    }

    public class UpdatedPreferencesEventArgs : EventArgs
    {
        public bool NotesDirectoryChanged { get; set; }
        public string NewTheme { get; set; }
        public string NewFontFamily { get; set; }
        public int? NewFontSize { get; set; }
    }
}
