using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SapphireNotes.Exceptions;
using SapphireNotes.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.Services
{
    public interface INotesService
    {
        Note Create(string name, string fontFamily, int fontSize);
        Note Update(string newName, Note note);
        void Archive(Note note);
        void Restore(Note note);
        void Delete(Note note);
        void SaveAll(IEnumerable<Note> notes);
        void SaveDirtyWithMetadata(IEnumerable<Note> notes);
        Note[] Load();
        Note[] LoadArchived();
        void MoveAll(string oldDirectory, string newDirectory);
        string GetFontThatAllNotesUse();
        int? GetFontSizeThatAllNotesUse();
        void SetFontForAll(string font);
        void SetFontSizeForAll(int fontSize);
    }

    public class NotesService : INotesService
    {
        private readonly INotesMetadataService _notesMetadataService;
        private readonly Preferences _preferences;

        public NotesService(INotesMetadataService notesMetadataService, Preferences preferences)
        {
            _notesMetadataService = notesMetadataService;
            _preferences = preferences;
        }

        public Note Create(string name, string fontFamily, int fontSize)
        {
            name = name.Trim();

            if (name.Length == 0)
            {
                throw new ValidationException("Name is required.");
            }

            var fileName = name + ".txt";
            if (Exists(fileName))
            {
                throw new ValidationException("A note with the same name already exists.");
            }

            var path = Path.Combine(_preferences.NotesDirectory, fileName);
            File.Create(path);

            var note = new Note(name, path, string.Empty, new NoteMetadata(fontFamily, fontSize));

            _notesMetadataService.Add(note.Name, note.Metadata);
            _notesMetadataService.Save();

            return note;
        }

        public Note Update(string newName, Note note)
        {
            newName = newName.Trim();

            if (newName.Length == 0)
            {
                throw new ValidationException("Name is required.");
            }

            var fileName = newName + ".txt";
            if (note.Name.ToLowerInvariant() != newName.ToLowerInvariant() && Exists(fileName))
            {
                throw new ValidationException("A note with the same name already exists.");
            }

            var path = Path.Combine(_preferences.NotesDirectory, fileName);
            File.Move(note.FilePath, path);

            _notesMetadataService.Remove(note.Name);
            _notesMetadataService.Add(newName, note.Metadata);
            _notesMetadataService.Save();

            note.Name = newName;
            note.FilePath = path;

            return note;
        }

        public void Archive(Note note)
        {
            Save(note);

            var archiveDirectory = Path.Combine(_preferences.NotesDirectory, Globals.ArchiveDirectoryName);
            if (!Directory.Exists(archiveDirectory))
            {
                Directory.CreateDirectory(archiveDirectory);
            }

            var fileName = Path.GetFileName(note.FilePath);
            var archivePath = Path.Combine(_preferences.NotesDirectory, Globals.ArchiveDirectoryName, fileName);

            archivePath = FileUtil.NextAvailableFileName(archivePath);
            File.Move(note.FilePath, archivePath);

            var newName = Path.GetFileNameWithoutExtension(archivePath);

            NoteMetadata metadata = _notesMetadataService.Get(note.Name);
            metadata.Archived = DateTime.Now;

            _notesMetadataService.Remove(note.Name);
            _notesMetadataService.Add(Globals.ArchiveDirectoryName + "/" + newName, metadata);

            _notesMetadataService.Save();

            note.FilePath = archivePath;
        }

        public void Restore(Note note)
        {
            var fileName = Path.GetFileName(note.FilePath);
            var notesPath = Path.Combine(_preferences.NotesDirectory, fileName);

            notesPath = FileUtil.NextAvailableFileName(notesPath);
            File.Move(note.FilePath, notesPath);

            var newName = Path.GetFileNameWithoutExtension(notesPath);

            NoteMetadata metadata = _notesMetadataService.Get(Globals.ArchiveDirectoryName + "/" + note.Name);
            metadata.Archived = null;

            _notesMetadataService.Remove(Globals.ArchiveDirectoryName + "/" + note.Name);
            _notesMetadataService.Add(newName, metadata);

            _notesMetadataService.Save();

            note.FilePath = notesPath;
            note.Name = newName;
        }

        public void Delete(Note note)
        {
            File.Delete(note.FilePath);

            _notesMetadataService.Remove(note.Name);
            _notesMetadataService.Save();
        }

        public void SaveAll(IEnumerable<Note> notes)
        {
            foreach (Note note in notes)
            {
                Save(note);
            }
        }

        public void SaveDirtyWithMetadata(IEnumerable<Note> notes)
        {
            foreach (Note note in notes)
            {
                if (note.IsDirty)
                {
                    Save(note);
                }

                _notesMetadataService.AddOrUpdate(note.Name, note.Metadata);
            }

            _notesMetadataService.Save();
        }

        public Note[] Load()
        {
            if (!Directory.Exists(_preferences.NotesDirectory))
            {
                return new Note[0];
            }

            string[] textFiles = Directory.GetFiles(_preferences.NotesDirectory, "*.txt");
            var notes = new List<Note>(textFiles.Length);
            foreach (string filePath in textFiles)
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                string contents = File.ReadAllText(filePath);

                notes.Add(new Note(name, filePath, contents));
            }

            var notesOnFileSystem = notes.Select(x => x.Name).ToList();

            var archivePath = Path.Combine(_preferences.NotesDirectory, Globals.ArchiveDirectoryName);
            if (Directory.Exists(archivePath))
            {
                string[] archivedTextFiles = Directory.GetFiles(archivePath, "*.txt");
                if (archivedTextFiles.Length > 0)
                {
                    notesOnFileSystem.AddRange(archivedTextFiles.Select(x => Globals.ArchiveDirectoryName + "/" + Path.GetFileNameWithoutExtension(x)));
                }
            }

            _notesMetadataService.Initialize(notesOnFileSystem);

            foreach (Note note in notes)
            {
                note.Metadata = _notesMetadataService.Get(note.Name);
            }

            var orderedByLastWrite = notes.OrderByDescending(x => File.GetLastWriteTime(x.FilePath)).ToArray();
            return orderedByLastWrite;
        }

        public Note[] LoadArchived()
        {
            var archivePath = Path.Combine(_preferences.NotesDirectory, Globals.ArchiveDirectoryName);
            if (!Directory.Exists(archivePath))
            {
                return new Note[0];
            }

            string[] textFiles = Directory.GetFiles(archivePath, "*.txt");
            if (textFiles.Length == 0)
            {
                return new Note[0];
            }

            var notes = new List<Note>(textFiles.Length);
            foreach (string filePath in textFiles)
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                string contents = File.ReadAllText(filePath);

                NoteMetadata metadata = _notesMetadataService.Get(Globals.ArchiveDirectoryName + "/" + name);

                notes.Add(new Note(name, filePath, contents, metadata));
            }

            var orderedByLastWrite = notes.OrderByDescending(x => x.Metadata.Archived).ToArray();
            return orderedByLastWrite;
        }

        public void MoveAll(string oldDirectory, string newDirectory)
        {
            string[] textFiles = Directory.GetFiles(oldDirectory, "*.txt");

            var fromTo = new Dictionary<string, string>();
            foreach (string filePath in textFiles)
            {
                var newPath = Path.Combine(newDirectory, Path.GetFileName(filePath));
                if (!File.Exists(newPath))
                {
                    fromTo.Add(filePath, newPath);
                }
                else
                {
                    throw new MoveNotesException("Couldn't move the notes. Make sure there aren't any existing notes with identical names in the chosen directory.");
                }
            }
            
            var oldArchivePath = Path.Combine(oldDirectory, Globals.ArchiveDirectoryName);
            bool oldArchiveExists = Directory.Exists(oldArchivePath);
            if (oldArchiveExists)
            {
                string[] archivedTextFiles = Directory.GetFiles(oldArchivePath, "*.txt");
                if (archivedTextFiles.Length > 0)
                {
                    var newArchivePath = Path.Combine(newDirectory, Globals.ArchiveDirectoryName);
                    if (!Directory.Exists(newArchivePath))
                    {
                        Directory.CreateDirectory(newArchivePath);
                    }

                    foreach (string filePath in archivedTextFiles)
                    {
                        var newPath = Path.Combine(newArchivePath, Path.GetFileName(filePath));
                        if (!File.Exists(newPath))
                        {
                            fromTo.Add(filePath, newPath);
                        }
                        else
                        {
                            throw new MoveNotesException($"Couldn't move the archived notes. Make sure there aren't any existing notes with identical names in the chosen directory's '{Globals.ArchiveDirectoryName}' folder.");
                        }
                    }
                }
            }

            foreach (var kvp in fromTo)
            {
                File.Move(kvp.Key, kvp.Value);
            }

            if (oldArchiveExists)
            {
                Directory.Delete(oldArchivePath);
            }
        }

        public string GetFontThatAllNotesUse()
        {
            var fonts = _notesMetadataService.GetDistinctFonts();
            if (fonts.Length == 0)
            {
                return Globals.DefaultFontFamily;
            }

            if (fonts.Length == 1)
            {
                return fonts[0];
            }

            return null;
        }

        public int? GetFontSizeThatAllNotesUse()
        {
            var fontSizes = _notesMetadataService.GetDistinctFontSizes();
            if (fontSizes.Length == 0)
            {
                return Globals.DefaultFontSize;
            }

            if (fontSizes.Length == 1)
            {
                return fontSizes[0];
            }

            return null;
        }

        public void SetFontForAll(string font)
        {
            _notesMetadataService.SetFontForAll(font);
        }

        public void SetFontSizeForAll(int fontSize)
        {
            _notesMetadataService.SetFontSizeForAll(fontSize);
        }

        private void Save(Note note)
        {
            File.WriteAllText(note.FilePath, note.Text);
        }

        private bool Exists(string fileName)
        {
            var path = Path.Combine(_preferences.NotesDirectory, fileName);
            return File.Exists(path);
        }
    }
}
