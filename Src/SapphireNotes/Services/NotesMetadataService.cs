using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SapphireNotes.Models;

namespace SapphireNotes.Services
{
    public interface INotesMetadataService
    {
        int Count { get; }
        void Add(string name, NoteMetadata metadata);
        bool Contains(string name);
        NoteMetadata Get(string name);
        void Remove(string name);
        void Clear();
        void RemoveMissing(IEnumerable<string> noteNames);
        string[] GetDistinctFonts();
        int[] GetDistinctFontSizes();
        void SetFontForAll(string font);
        void SetFontSizeForAll(int fontSize);
        void LoadOrCreate();
        void Save();
    }

    public class NotesMetadataService : INotesMetadataService
    {
        private const string MetadataFileName = "metadata.bin";
        private readonly string _metadataFilePath;
        private Dictionary<string, NoteMetadata> _notesMetadata;

        public NotesMetadataService()
        {
#if DEBUG
            string appDataDirectory = string.Empty;
#else
            string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Globals.ApplicationName);
            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }
#endif

            _metadataFilePath = Path.Combine(appDataDirectory, MetadataFileName);
        }

        public int Count { get { return _notesMetadata.Count; } }

        public void Add(string name, NoteMetadata metadata)
        {
            _notesMetadata.Add(name, metadata);
        }

        public bool Contains(string name)
        {
            return _notesMetadata.ContainsKey(name);
        }

        public NoteMetadata Get(string name)
        {
            return _notesMetadata[name];
        }

        public void Remove(string name)
        {
            _notesMetadata.Remove(name);
        }

        public void Clear()
        {
            _notesMetadata.Clear();
        }

        public void RemoveMissing(IEnumerable<string> noteNames)
        {
            var missingNoteNames = _notesMetadata.Keys.Where(k => !noteNames.Contains(k));

            foreach (string noteName in missingNoteNames)
            {
                _notesMetadata.Remove(noteName);
            }
        }

        public string[] GetDistinctFonts()
        {
            return _notesMetadata.Values.Select(x => x.FontFamily).Distinct().ToArray();
        }

        public int[] GetDistinctFontSizes()
        {
            return _notesMetadata.Values.Select(x => x.FontSize).Distinct().ToArray();
        }

        public void SetFontForAll(string font)
        {
            foreach (var kvp in _notesMetadata)
            {
                _notesMetadata[kvp.Key].FontFamily = font;
            }
            Save();
        }

        public void SetFontSizeForAll(int fontSize)
        {
            foreach (var kvp in _notesMetadata)
            {
                _notesMetadata[kvp.Key].FontSize = fontSize;
            }
            Save();
        }

        public void LoadOrCreate()
        {
            if (File.Exists(_metadataFilePath))
            {
                using var reader = new BinaryReader(File.Open(_metadataFilePath, FileMode.Open));
                int notesCount = reader.ReadInt32();
                _notesMetadata = new Dictionary<string, NoteMetadata>(notesCount);

                for (var i = 0; i < notesCount; i++)
                {
                    var noteName = reader.ReadString();
                    var metadata = new NoteMetadata
                    {
                        FontSize = reader.ReadInt32(),
                        FontFamily = reader.ReadString(),
                        CaretPosition = reader.ReadInt32()
                    };

                    _notesMetadata.Add(noteName, metadata);
                }
            }
            else
            {
                _notesMetadata = new Dictionary<string, NoteMetadata>();
                Save();
            }
        }

        public void Save()
        {
            using var writer = new BinaryWriter(File.Open(_metadataFilePath, FileMode.OpenOrCreate));

            writer.Write(_notesMetadata.Count);

            foreach (var kvp in _notesMetadata)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.FontSize);
                writer.Write(kvp.Value.FontFamily);
                writer.Write(kvp.Value.CaretPosition);
            }
        }
    }
}
