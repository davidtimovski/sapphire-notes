using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SapphireNotes.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.Services
{
    public interface INotesService
    {
        Note Create(string name, string fontFamily, int fontSize);
        Note Update(string newName, Note note);
        void Archive(Note note);
        void Delete(Note note);
        void SaveAll(IEnumerable<Note> notes);
        void SaveAllWithMetadata(IEnumerable<Note> notes);
        Note[] GetAll();
        void MoveAll(string oldDirectory);
    }

    public class NotesService : INotesService
    {
        private const string MetadataFileName = "metadata.bin";
        private readonly string _metadataFilePath;
        private const string ArchiveDirectoryName = "archive";
        private readonly Preferences _preferences;
        private Dictionary<string, NoteMetadata> _notesMetadata;

        public NotesService(Preferences preferences)
        {
            _preferences = preferences;

#if DEBUG
            string appDataDirectory = string.Empty;
#else
            string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ApplicationName);
            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }
#endif

            _metadataFilePath = Path.Combine(appDataDirectory, MetadataFileName);

            if (File.Exists(_metadataFilePath))
            {
                LoadMetadata();
            }
            else
            {
                _notesMetadata = new Dictionary<string, NoteMetadata>();
                SaveMetadata();
            }
        }

        public Note Create(string name, string fontFamily, int fontSize)
        {
            name = name.Trim();

            if (name.Length == 0)
            {
                throw new InvalidNoteNameException("Name is required.");
            }

            var fileName = name + ".txt";
            if (Exists(fileName))
            {
                throw new InvalidNoteNameException("A note with the same name already exists.");
            }

            var path = Path.Combine(_preferences.NotesDirectory, fileName);
            File.Create(path);

            return new Note(name, path, string.Empty, new NoteMetadata(fontFamily, fontSize));
        }

        public Note Update(string newName, Note note)
        {
            newName = newName.Trim();

            if (newName.Length == 0)
            {
                throw new InvalidNoteNameException("Name is required.");
            }

            if (note.Name == newName)
            {
                return note;
            }

            var fileName = newName + ".txt";
            if (note.Name.ToLowerInvariant() != newName.ToLowerInvariant() && Exists(fileName))
            {
                throw new InvalidNoteNameException("A note with the same name already exists.");
            }

            var path = Path.Combine(_preferences.NotesDirectory, fileName);
            File.Move(note.FilePath, path);

            note.Name = newName;
            note.FilePath = path;

            return note;
        }

        public void Archive(Note note)
        {
            var archiveDirectory = Path.Combine(_preferences.NotesDirectory, ArchiveDirectoryName);
            if (!Directory.Exists(archiveDirectory))
            {
                Directory.CreateDirectory(archiveDirectory);
            }

            MoveToArchive(note.FilePath);
        }

        public void Delete(Note note)
        {
            File.Delete(note.FilePath);
        }

        public void SaveAll(IEnumerable<Note> notes)
        {
            foreach (var note in notes)
            {
                File.WriteAllText(note.FilePath, note.Text);
            }
        }

        public void SaveAllWithMetadata(IEnumerable<Note> notes)
        {
            _notesMetadata.Clear();

            foreach (var note in notes)
            {
                File.WriteAllText(note.FilePath, note.Text);
                _notesMetadata.Add(note.Name, note.Metadata);
            }

            SaveMetadata();
        }

        public Note[] GetAll()
        {
            if (Directory.Exists(_preferences.NotesDirectory))
            {
                string[] textFiles = Directory.GetFiles(_preferences.NotesDirectory, "*.txt");

                if (textFiles.Length == 0)
                {
                    var sampleNotes = CreateSampleNotes();
                    return sampleNotes;
                }
                else
                {
                    var notes = new List<Note>(textFiles.Length);
                    foreach (string filePath in textFiles)
                    {
                        var name = Path.GetFileNameWithoutExtension(filePath);
                        var contents = File.ReadAllText(filePath);

                        NoteMetadata metadata;
                        if (_notesMetadata.ContainsKey(name))
                        {
                            metadata = _notesMetadata[name];
                        }
                        else
                        {
                            metadata = new NoteMetadata();
                            _notesMetadata.Add(name, metadata);
                        }

                        notes.Add(new Note(name, filePath, contents, metadata));
                    }

                    var orderedByLastWrite = notes.OrderByDescending(x => File.GetLastWriteTime(x.FilePath)).ToArray();
                    return orderedByLastWrite;
                }
            }
            else
            {
                Directory.CreateDirectory(_preferences.NotesDirectory);

                var sampleNotes = CreateSampleNotes();
                return sampleNotes;
            }
        }

        public void MoveAll(string oldDirectory)
        {
            string[] textFiles = Directory.GetFiles(oldDirectory, "*.txt");

            foreach (string filePath in textFiles)
            {
                var newPath = Path.Combine(_preferences.NotesDirectory, Path.GetFileName(filePath));
                File.Move(filePath, newPath);
            }

            var oldArchivePath = Path.Combine(oldDirectory, ArchiveDirectoryName);
            if (!Directory.Exists(oldArchivePath))
            {
                return;
            }

            string[] archivedTextFiles = Directory.GetFiles(oldArchivePath, "*.txt");
            if (archivedTextFiles.Length == 0)
            {
                return;
            }

            var newArchivePath = Path.Combine(_preferences.NotesDirectory, ArchiveDirectoryName);
            if (!Directory.Exists(newArchivePath))
            {
                Directory.CreateDirectory(newArchivePath);
            }

            foreach (string filePath in archivedTextFiles)
            {
                var newPath = Path.Combine(newArchivePath, Path.GetFileName(filePath));
                File.Move(filePath, newPath);
            }

            Directory.Delete(oldArchivePath);
        }

        private bool Exists(string fileName)
        {
            var path = Path.Combine(_preferences.NotesDirectory, fileName);
            if (File.Exists(path))
            {
                return true;
            }

            return false;
        }

        private void MoveToArchive(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var archivePath = Path.Combine(_preferences.NotesDirectory, ArchiveDirectoryName, fileName);

            archivePath = FileUtil.NextAvailableFileName(archivePath);
            File.Move(filePath, archivePath);
        }

        private Note[] CreateSampleNotes()
        {
            var note1name = "note 1";
            var note1path = Path.Combine(_preferences.NotesDirectory, note1name + ".txt");
            var note1Text = "This is a sample note";
            using (var stream = File.CreateText(note1path))
            {
                stream.Write(note1Text);
            }

            var note2name = "note 2";
            var note2path = Path.Combine(_preferences.NotesDirectory, note2name + ".txt");
            var note2Text = "This is another sample note";
            using (var stream = File.CreateText(note2path))
            {
                stream.Write(note2Text);
            }

            return new Note[]
            {
                 new Note(note1name, note1path, note1Text, new NoteMetadata()),
                 new Note(note2name, note2path, note2Text, new NoteMetadata())
            };
        }

        private void LoadMetadata()
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
                    CursorPosition = reader.ReadInt32()
                };

                _notesMetadata.Add(noteName, metadata);
            }
        }

        private void SaveMetadata()
        {
            using var writer = new BinaryWriter(File.Open(_metadataFilePath, FileMode.OpenOrCreate));

            writer.Write(_notesMetadata.Count);

            foreach (var kvp in _notesMetadata)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.FontSize);
                writer.Write(kvp.Value.FontFamily);
                writer.Write(kvp.Value.CursorPosition);
            }
        }
    }

    public class InvalidNoteNameException : Exception
    {
        public InvalidNoteNameException(string message) : base(message) { }
    }
}
