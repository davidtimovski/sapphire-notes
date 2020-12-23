using System;
using System.Collections.Generic;
using System.Linq;
using SapphireNotes.Contracts;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Exceptions;
using SapphireNotes.Repositories;

namespace SapphireNotes.Services
{
    public interface INotesService
    {
        void Create(string name, string fontFamily, int fontSize);
        void CreateQuick(string content);
        void Update(string newName, Note note);
        void Archive(Note note);
        void Restore(Note note);
        void Delete(Note note);
        void SaveAll(IEnumerable<Note> notes);
        void SaveAllWithMetadata(IEnumerable<Note> notes);
        Note[] Load();
        Note[] LoadArchived();
        void MoveAll(string oldDirectory, string newDirectory);
        string GetFontThatAllNotesUse();
        int? GetFontSizeThatAllNotesUse();
        void SetFontForAll(string font);
        void SetFontSizeForAll(int fontSize);

        event EventHandler<CreatedNoteEventArgs> Created;
        event EventHandler<UpdatedNoteEventArgs> Updated;
        event EventHandler<ArchivedNoteEventArgs> Archived;
        event EventHandler<DeletedNoteEventArgs> Deleted;
        event EventHandler<RestoredNoteEventArgs> Restored;
    }

    public class NotesService : INotesService
    {
        private readonly INotesMetadataService _notesMetadataService;
        private readonly INotesRepository _notesRepository;

        public event EventHandler<CreatedNoteEventArgs> Created;
        public event EventHandler<UpdatedNoteEventArgs> Updated;
        public event EventHandler<ArchivedNoteEventArgs> Archived;
        public event EventHandler<DeletedNoteEventArgs> Deleted;
        public event EventHandler<RestoredNoteEventArgs> Restored;

        public NotesService(INotesMetadataService notesMetadataService, INotesRepository notesRepository)
        {
            _notesMetadataService = notesMetadataService;
            _notesRepository = notesRepository;
        }

        public void Create(string name, string fontFamily, int fontSize)
        {
            name = name.Trim();

            if (name.Length == 0)
            {
                throw new ValidationException("Name is required.");
            }

            if (_notesRepository.Exists(name))
            {
                throw new ValidationException("A note with the same name already exists.");
            }

            _notesRepository.Create(name);

            var note = new Note(name, string.Empty, DateTime.Now, new NoteMetadata(fontFamily, fontSize));

            _notesMetadataService.Add(note.Name, note.Metadata);
            _notesMetadataService.Save();

            Created.Invoke(this, new CreatedNoteEventArgs
            {
                CreatedNote = note
            });
        }

        public void CreateQuick(string content)
        {
            string name = _notesRepository.Create("Quick note", content);

            var note = new Note(name, content, DateTime.Now, new NoteMetadata(content.Length));

            _notesMetadataService.Add(note.Name, note.Metadata);
            _notesMetadataService.Save();

            Created.Invoke(this, new CreatedNoteEventArgs
            {
                CreatedNote = note
            });
        }

        public void Update(string newName, Note note)
        {
            string originalName = note.Name;
            newName = newName.Trim();

            if (newName.Length == 0)
            {
                throw new ValidationException("Name is required.");
            }

            var fileName = newName + ".txt";
            if (originalName.ToLowerInvariant() != newName.ToLowerInvariant() && _notesRepository.Exists(newName))
            {
                throw new ValidationException("A note with the same name already exists.");
            }

            _notesRepository.Update(originalName, newName);

            _notesMetadataService.Remove(originalName);
            _notesMetadataService.Add(newName, note.Metadata);
            _notesMetadataService.Save();

            note.Name = newName;

            Updated.Invoke(this, new UpdatedNoteEventArgs
            {
                OriginalName = originalName,
                UpdatedNote = note
            });
        }

        public void Archive(Note note)
        {
            _notesRepository.Save(note.Name, note.Content);

            var newName = _notesRepository.Archive(note.Name);

            NoteMetadata metadata = _notesMetadataService.Get(note.Name);
            metadata.Archived = DateTime.Now;

            _notesMetadataService.Remove(note.Name);
            _notesMetadataService.Add(Globals.ArchivePrefix + "/" + newName, metadata);

            _notesMetadataService.Save();

            Archived?.Invoke(this, new ArchivedNoteEventArgs
            {
                ArchivedNote = note
            });
        }

        public void Restore(Note note)
        {
            var newName = _notesRepository.Restore(note.Name);

            NoteMetadata metadata = _notesMetadataService.Get(Globals.ArchivePrefix + "/" + note.Name);
            metadata.Archived = null;

            _notesMetadataService.Remove(Globals.ArchivePrefix + "/" + note.Name);
            _notesMetadataService.Add(newName, metadata);

            _notesMetadataService.Save();

            note.Name = newName;

            Restored.Invoke(this, new RestoredNoteEventArgs
            {
                RestoredNote = note
            });
        }

        public void Delete(Note note)
        {
            if (note.Metadata.Archived.HasValue)
            {
                _notesRepository.DeleteArchived(note.Name);
            }
            else
            {
                _notesRepository.Delete(note.Name);
            }            

            _notesMetadataService.Remove(note.Name);
            _notesMetadataService.Save();

            Deleted.Invoke(this, new DeletedNoteEventArgs
            {
                DeletedNote = note
            });
        }

        public void SaveAll(IEnumerable<Note> notes)
        {
            foreach (Note note in notes)
            {
                _notesRepository.Save(note.Name, note.Content);
            }
        }

        public void SaveAllWithMetadata(IEnumerable<Note> notes)
        {
            foreach (Note note in notes)
            {
                _notesRepository.Save(note.Name, note.Content);
                _notesMetadataService.AddOrUpdate(note.Name, note.Metadata);
            }

            _notesMetadataService.Save();
        }

        public Note[] Load()
        {
            IEnumerable<Note> notes = _notesRepository.GetAll();

            _notesMetadataService.Initialize(notes.Select(x => x.Name));

            notes = notes.Where(x => !x.Name.StartsWith(Globals.ArchivePrefix + "/"));
            foreach (Note note in notes)
            {
                note.Metadata = _notesMetadataService.Get(note.Name);
            }

            return notes.OrderByDescending(x => x.LastWriteTime).ToArray();
        }

        public Note[] LoadArchived()
        {
            IEnumerable<Note> notes = _notesRepository.GetAllArchived();

            foreach (Note note in notes)
            {
                note.Metadata = _notesMetadataService.Get(Globals.ArchivePrefix + "/" + note.Name);
            }

            return notes.OrderByDescending(x => x.Metadata.Archived).ToArray();
        }

        public void MoveAll(string oldDirectory, string newDirectory)
        {
            var fileSystemRepository = (IFileSystemRepository)_notesRepository;
            fileSystemRepository.MoveAll(oldDirectory, newDirectory);
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
    }

    public class CreatedNoteEventArgs : EventArgs
    {
        public Note CreatedNote { get; set; }
    }

    public class UpdatedNoteEventArgs : EventArgs
    {
        public string OriginalName { get; set; }
        public Note UpdatedNote { get; set; }
    }

    public class DeletedNoteEventArgs : EventArgs
    {
        public Note DeletedNote { get; set; }
    }

    public class ArchivedNoteEventArgs : EventArgs
    {
        public Note ArchivedNote { get; set; }
    }

    public class RestoredNoteEventArgs : EventArgs
    {
        public Note RestoredNote { get; set; }
    }
}
