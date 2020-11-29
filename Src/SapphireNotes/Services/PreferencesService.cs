using System;
using System.IO;
using SapphireNotes.Models;

namespace SapphireNotes.Services
{
    public interface IPreferencesService
    {
        Preferences Preferences { get; }
        void UpdateWindowSizePreferenceIfChanged(int width, int height, int positionX, int positionY);
    }

    public class PreferencesService : IPreferencesService
    {
        private const string PreferencesFileName = "preferences.bin";
        private readonly string _preferencesFilePath;
        private readonly string _notesDirectory;

        public PreferencesService()
        {
#if DEBUG
            string appDataDirectory = string.Empty;
#else
            string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sapphire Notes");
            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }
#endif

            _preferencesFilePath = Path.Combine(appDataDirectory, PreferencesFileName);
            _notesDirectory = Path.Combine(appDataDirectory, "notes");

            if (File.Exists(_preferencesFilePath))
            {
                LoadPreferences();
            }
            else
            {
                Preferences = new Preferences(_notesDirectory);
                SavePreferences();
            }
        }

        public Preferences Preferences { get; private set; }

        public void UpdateWindowSizePreferenceIfChanged(int width, int height, int positionX, int positionY)
        {
            if (width != Preferences.Window.Width || height != Preferences.Window.Height)
            {
                Preferences.Window.Width = width;
                Preferences.Window.Height = height;
            }

            Preferences.Window.PositionX = positionX;
            Preferences.Window.PositionY = positionY;

            SavePreferences();
        }

        private void LoadPreferences()
        {
            var existingPreferences = new Preferences(_notesDirectory);
            using (var reader = new BinaryReader(File.Open(_preferencesFilePath, FileMode.Open)))
            {
                existingPreferences.NotesDirectory = reader.ReadString();
                existingPreferences.FontFamily = reader.ReadString();
                existingPreferences.FontSize = reader.ReadInt16();
                existingPreferences.AutoSaveInterval = reader.ReadInt16();
                existingPreferences.Window = new WindowPreferences
                {
                    Width = reader.ReadInt32(),
                    Height = reader.ReadInt32(),
                    PositionX = reader.ReadInt32(),
                    PositionY = reader.ReadInt32()
                };
            }
            Preferences = existingPreferences;
        }

        private void SavePreferences()
        {
            using var writer = new BinaryWriter(File.Open(_preferencesFilePath, FileMode.OpenOrCreate));
            writer.Write(Preferences.NotesDirectory);
            writer.Write(Preferences.FontFamily);
            writer.Write(Preferences.FontSize);
            writer.Write(Preferences.AutoSaveInterval);
            writer.Write(Preferences.Window.Width);
            writer.Write(Preferences.Window.Height);
            writer.Write(Preferences.Window.PositionX);
            writer.Write(Preferences.Window.PositionY);
        }
    }
}
