using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SapphireNotes.Models;

namespace SapphireNotes.Services
{
    public interface INotesMetadataService
    {
        void Add(string name, NoteMetadata metadata);
        void AddOrUpdate(string name, NoteMetadata metadata);
        bool Contains(string name);
        NoteMetadata Get(string name);
        void Remove(string name);
        void Clear();
        string[] GetDistinctFonts();
        int[] GetDistinctFontSizes();
        void SetFontForAll(string font);
        void SetFontSizeForAll(int fontSize);
        void Initialize(IEnumerable<string> notesOnFilesystem);
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

        public void Add(string name, NoteMetadata metadata)
        {
            _notesMetadata.Add(name, metadata);
        }

        public void AddOrUpdate(string name, NoteMetadata metadata)
        {
            if (_notesMetadata.ContainsKey(name))
            {
                _notesMetadata[name] = metadata;
            }
            else
            {
                _notesMetadata.Add(name, metadata);
            }
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

        public void Initialize(IEnumerable<string> notesOnFilesystem)
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
                    (
                        fontFamily: reader.ReadString(),
                        fontSize: reader.ReadInt32(),
                        caretPosition: reader.ReadInt32()
                    );

                    long archivedTicks = reader.ReadInt64();
                    if (archivedTicks != 0)
                    {
                        metadata.Archived = new DateTime(archivedTicks);
                    }

                    _notesMetadata.Add(noteName, metadata);
                }

                SynchronizeWithFileSystem(notesOnFilesystem);
            }
            else
            {
                _notesMetadata = new Dictionary<string, NoteMetadata>(notesOnFilesystem.Count());

                foreach (string noteName in notesOnFilesystem)
                {
                    if (noteName.Contains(Globals.ArchiveDirectoryName + "/"))
                    {
                        _notesMetadata.Add(noteName, new NoteMetadata(DateTime.Now));
                    }
                    else
                    {
                        _notesMetadata.Add(noteName, new NoteMetadata());
                    }
                }

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
                writer.Write(kvp.Value.FontFamily);
                writer.Write(kvp.Value.FontSize);
                writer.Write(kvp.Value.CaretPosition);
                writer.Write(kvp.Value.Archived.HasValue ? kvp.Value.Archived.Value.Ticks : 0);
            }
        }

        private void SynchronizeWithFileSystem(IEnumerable<string> notesOnFilesystem)
        {
            IEnumerable<string> deletedNotes = _notesMetadata.Keys.Where(k => !notesOnFilesystem.Contains(k));
            foreach (string noteName in deletedNotes)
            {
                _notesMetadata.Remove(noteName);
            }

            IEnumerable<string> addedNotes = notesOnFilesystem.Where(k => !_notesMetadata.Keys.Contains(k));
            foreach (string noteName in addedNotes)
            {
                if (noteName.Contains(Globals.ArchiveDirectoryName + "/"))
                {
                    _notesMetadata.Add(noteName, new NoteMetadata(DateTime.Now));
                }
                else
                {
                    _notesMetadata.Add(noteName, new NoteMetadata());
                }
            }
        }
    }
}
