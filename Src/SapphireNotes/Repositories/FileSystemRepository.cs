using System;
using System.Collections.Generic;
using System.IO;
using SapphireNotes.Contracts;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Exceptions;
using SapphireNotes.Services;
using SapphireNotes.Utils;

namespace SapphireNotes.Repositories;

public interface IFileSystemRepository : INotesRepository
{
    void MoveAll(string oldDirectory);
}

public class FileSystemRepository : IFileSystemRepository
{
    private const string Extension = ".txt";
    private readonly IPreferencesService _preferencesService;

    public FileSystemRepository(IPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
    }

    public void Create(string name)
    {
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        File.Create(path).Dispose();
    }

    public string Create(string name, string content)
    {
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        path = FileUtil.NextAvailableFileName(path);

        using var sw = File.CreateText(path);
        sw.Write(content);

        return Path.GetFileNameWithoutExtension(path);
    }

    public void Update(string name, string newName)
    {
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        var newPath = Path.Combine(_preferencesService.Preferences.NotesDirectory, newName + Extension);
        File.Move(path, newPath);
    }

    public void Delete(string name)
    {
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        File.Delete(path);
    }

    public void DeleteArchived(string name)
    {
        var path = Path.Combine(GetArchiveDirectory(), name + Extension);
        File.Delete(path);
    }

    public void Save(string name, string content)
    {
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        File.WriteAllText(path, content);
    }

    public bool Exists(string name)
    {
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        return File.Exists(path);
    }

    public string Archive(string name)
    {
        var archiveDirectory = GetArchiveDirectory();

        if (!Directory.Exists(archiveDirectory))
        {
            Directory.CreateDirectory(archiveDirectory);
        }

        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, name + Extension);
        var fileName = Path.GetFileName(path);

        var archivePath = Path.Combine(archiveDirectory, fileName);
        archivePath = FileUtil.NextAvailableFileName(archivePath);

        File.Move(path, archivePath);

        return Path.GetFileNameWithoutExtension(archivePath);
    }

    public string Restore(string name)
    {
        var archivePath = Path.Combine(GetArchiveDirectory(), name + Extension);
        var fileName = Path.GetFileName(archivePath);
        var path = Path.Combine(_preferencesService.Preferences.NotesDirectory, fileName);

        path = FileUtil.NextAvailableFileName(path);
        File.Move(archivePath, path);

        return Path.GetFileNameWithoutExtension(path);
    }

    public IEnumerable<Note> GetAll()
    {
        if (!Directory.Exists(_preferencesService.Preferences.NotesDirectory))
        {
            return Array.Empty<Note>();
        }

        var textFiles = Directory.GetFiles(_preferencesService.Preferences.NotesDirectory, "*" + Extension);
        var notes = new List<Note>(textFiles.Length);
        foreach (var filePath in textFiles)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            var contents = File.ReadAllText(filePath);
            var lastWriteTime = File.GetLastWriteTime(filePath);

            notes.Add(new Note(name, contents, lastWriteTime));
        }

        var archiveDirectory = GetArchiveDirectory();
        if (Directory.Exists(archiveDirectory))
        {
            var archivedTextFiles = Directory.GetFiles(archiveDirectory, "*" + Extension);
            foreach (var filePath in archivedTextFiles)
            {
                var name = Path.GetFileNameWithoutExtension(filePath);
                notes.Add(new Note(Globals.ArchivePrefix + "/" + name));
            }
        }

        return notes.ToArray();
    }

    public IEnumerable<Note> GetAllArchived()
    {
        var archiveDirectory = GetArchiveDirectory();
        if (!Directory.Exists(archiveDirectory))
        {
            return Array.Empty<Note>();
        }

        var textFiles = Directory.GetFiles(archiveDirectory, "*" + Extension);
        if (textFiles.Length == 0)
        {
            return Array.Empty<Note>();
        }

        var notes = new List<Note>(textFiles.Length);
        foreach (var filePath in textFiles)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            var contents = File.ReadAllText(filePath);

            notes.Add(new Note(name, contents));
        }

        return notes.ToArray();
    }

    public void MoveAll(string newDirectory)
    {
        var textFiles = Directory.GetFiles(_preferencesService.Preferences.NotesDirectory, "*" + Extension);

        var fromTo = new Dictionary<string, string>();
        foreach (var filePath in textFiles)
        {
            var newPath = Path.Combine(newDirectory, Path.GetFileName(filePath));
            if (!File.Exists(newPath))
            {
                fromTo.Add(filePath, newPath);
            }
            else
            {
                throw new MoveNotesException("Couldn't move the notes. " +
                                             "Make sure there aren't any existing notes with identical names in the chosen directory.");
            }
        }

        var archiveDirectory = GetArchiveDirectory();
        var oldArchiveExists = Directory.Exists(archiveDirectory);
        if (oldArchiveExists)
        {
            var archivedTextFiles = Directory.GetFiles(archiveDirectory, "*" + Extension);
            if (archivedTextFiles.Length > 0)
            {
                var newArchivePath = Path.Combine(newDirectory, Globals.ArchivePrefix);
                if (!Directory.Exists(newArchivePath))
                {
                    Directory.CreateDirectory(newArchivePath);
                }

                foreach (var filePath in archivedTextFiles)
                {
                    var newPath = Path.Combine(newArchivePath, Path.GetFileName(filePath));
                    if (!File.Exists(newPath))
                    {
                        fromTo.Add(filePath, newPath);
                    }
                    else
                    {
                        throw new MoveNotesException("Couldn't move the archived notes. " +
                                                     "Make sure there aren't any existing notes with identical names in the chosen directory's " +
                                                     $"'{Globals.ArchivePrefix}' folder.");
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
            Directory.Delete(GetArchiveDirectory());
        }
    }

    private string GetArchiveDirectory()
    {
        return Path.Combine(_preferencesService.Preferences.NotesDirectory, Globals.ArchivePrefix);
    }
}
