using System;
using System.Collections.Generic;
using System.IO;
using SapphireNotes.Contracts;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Exceptions;
using SapphireNotes.Utils;

namespace SapphireNotes.Repositories
{
    public interface IFileSystemRepository : INotesRepository
    {
        void MoveAll(string oldDirectory, string newDirectory);
    }

    public class FileSystemRepository : IFileSystemRepository
    {
        private const string Extension = ".txt";
        private string _storageDirectory;
        private string _archiveDirectory;

        public FileSystemRepository(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
            _archiveDirectory = Path.Combine(_storageDirectory, Globals.ArchivePrefix);
        }

        public void Create(string name)
        {
            var path = Path.Combine(_storageDirectory, name + Extension);
            File.Create(path);
        }

        public string Create(string name, string content)
        {
            var path = Path.Combine(_storageDirectory, name + Extension);
            path = FileUtil.NextAvailableFileName(path);

            using StreamWriter sw = File.CreateText(path);
            sw.Write(content);

            return Path.GetFileNameWithoutExtension(path);
        }

        public void Update(string name, string newName)
        {
            var path = Path.Combine(_storageDirectory, name + Extension);
            var newPath = Path.Combine(_storageDirectory, newName + Extension);
            File.Move(path, newPath);
        }

        public void Delete(string name)
        {
            var path = Path.Combine(_storageDirectory, name + Extension);
            File.Delete(path);
        }

        public void DeleteArchived(string name)
        {
            var path = Path.Combine(_archiveDirectory, name + Extension);
            File.Delete(path);
        }

        public void Save(string name, string content)
        {
            var path = Path.Combine(_storageDirectory, name + Extension);
            File.WriteAllText(path, content);
        }

        public bool Exists(string name)
        {
            var path = Path.Combine(_storageDirectory, name + Extension);
            return File.Exists(path);
        }

        public string Archive(string name)
        {
            if (!Directory.Exists(_archiveDirectory))
            {
                Directory.CreateDirectory(_archiveDirectory);
            }

            var path = Path.Combine(_storageDirectory, name + Extension);
            var fileName = Path.GetFileName(path);

            var archivePath = Path.Combine(_archiveDirectory, fileName);
            archivePath = FileUtil.NextAvailableFileName(archivePath);

            File.Move(path, archivePath);

            return Path.GetFileNameWithoutExtension(archivePath);
        }

        public string Restore(string name)
        {
            var archivePath = Path.Combine(_archiveDirectory, name + Extension);
            var fileName = Path.GetFileName(archivePath);
            var path = Path.Combine(_storageDirectory, fileName);

            path = FileUtil.NextAvailableFileName(path);
            File.Move(archivePath, path);

            return Path.GetFileNameWithoutExtension(path);
        }

        public IEnumerable<Note> GetAll()
        {
            if (!Directory.Exists(_storageDirectory))
            {
                return new Note[0];
            }

            string[] textFiles = Directory.GetFiles(_storageDirectory, "*" + Extension);
            var notes = new List<Note>(textFiles.Length);
            foreach (string filePath in textFiles)
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                string contents = File.ReadAllText(filePath);
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);

                notes.Add(new Note(name, contents, lastWriteTime));
            }

            if (Directory.Exists(_archiveDirectory))
            {
                string[] archivedTextFiles = Directory.GetFiles(_archiveDirectory, "*" + Extension);
                foreach (string filePath in archivedTextFiles)
                {
                    string name = Path.GetFileNameWithoutExtension(filePath);
                    notes.Add(new Note(Globals.ArchivePrefix + "/" + name));
                }
            }

            return notes.ToArray();
        }

        public IEnumerable<Note> GetAllArchived()
        {
            if (!Directory.Exists(_archiveDirectory))
            {
                return new Note[0];
            }

            string[] textFiles = Directory.GetFiles(_archiveDirectory, "*" + Extension);
            if (textFiles.Length == 0)
            {
                return new Note[0];
            }

            var notes = new List<Note>(textFiles.Length);
            foreach (string filePath in textFiles)
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                string contents = File.ReadAllText(filePath);

                notes.Add(new Note(name, contents));
            }

            return notes.ToArray();
        }

        public void MoveAll(string oldDirectory, string newDirectory)
        {
            string[] textFiles = Directory.GetFiles(oldDirectory, "*" + Extension);

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

            bool oldArchiveExists = Directory.Exists(_archiveDirectory);
            if (oldArchiveExists)
            {
                string[] archivedTextFiles = Directory.GetFiles(_archiveDirectory, "*" + Extension);
                if (archivedTextFiles.Length > 0)
                {
                    var newArchivePath = Path.Combine(newDirectory, Globals.ArchivePrefix);
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
                            throw new MoveNotesException($"Couldn't move the archived notes. Make sure there aren't any existing notes with identical names in the chosen directory's '{Globals.ArchivePrefix}' folder.");
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
                Directory.Delete(_archiveDirectory);
            }

            _storageDirectory = newDirectory;
            _archiveDirectory = Path.Combine(newDirectory, Globals.ArchivePrefix);
        }
    }
}
