using System;
using System.Collections.Generic;
using Moq;
using SapphireNotes;
using SapphireNotes.Contracts;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Exceptions;
using SapphireNotes.Services;
using Xunit;

namespace UnitTests.Services
{
    public class NotesServiceTests
    {
        private readonly Mock<INotesMetadataService> _notesMetadataServiceMock = new();
        private readonly Mock<INotesRepository> _notesRepositoryMock = new();
        private readonly INotesService _sut;

        public NotesServiceTests()
        {
            _sut = new NotesService(_notesMetadataServiceMock.Object, _notesRepositoryMock.Object);
        }

        [Fact]
        public void CreateTrimsName()
        {
            string createdNoteName = null;
            _sut.Created += (object sender, CreatedNoteEventArgs e)
                => createdNoteName = e.CreatedNote.Name;

            _sut.Create(" name ", It.IsAny<string>(), It.IsAny<int>());

            Assert.Equal("name", createdNoteName);
        }

        [Fact]
        public void CreateThrowsIfNameMissing()
        {
            Assert.Throws<ValidationException>(() => _sut.Create("", It.IsAny<string>(), It.IsAny<int>()));
        }

        [Fact]
        public void CreateThrowsIfNoteWithSameNameExists()
        {
            _notesRepositoryMock
                .Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);

            Assert.Throws<ValidationException>(() => _sut.Create("dummy name", It.IsAny<string>(), It.IsAny<int>()));
        }

        [Fact]
        public void CreateInvokesCreatedEvent()
        {
            bool eventInvoked = false;
            _sut.Created += (object sender, CreatedNoteEventArgs e)
                => eventInvoked = true;

            _sut.Create("dummy name", It.IsAny<string>(), It.IsAny<int>());

            Assert.True(eventInvoked);
        }

        [Fact]
        public void CreateQuickInvokesCreatedEvent()
        {
            bool eventInvoked = false;
            _sut.Created += (object sender, CreatedNoteEventArgs e)
                => eventInvoked = true;

            _sut.CreateQuick("dummy content");

            Assert.True(eventInvoked);
        }

        [Fact]
        public void UpdateTrimsName()
        {
            string updatedNoteName = null;
            _sut.Updated += (object sender, UpdatedNoteEventArgs e)
                => updatedNoteName = e.UpdatedNote.Name;

            _sut.Update(" name ", new Note("dummy name"));

            Assert.Equal("name", updatedNoteName);
        }

        [Fact]
        public void UpdateThrowsIfNameMissing()
        {
            Assert.Throws<ValidationException>(() => _sut.Update("", new Note("dummy name")));
        }

        [Fact]
        public void UpdateThrowsIfNoteWithSameNameExists()
        {
            _notesRepositoryMock
                .Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);

            Assert.Throws<ValidationException>(() => _sut.Update("", new Note("dummy name")));
        }

        [Fact]
        public void UpdateInvokesUpdatedEvent()
        {
            bool eventInvoked = false;
            _sut.Updated += (object sender, UpdatedNoteEventArgs e)
                => eventInvoked = true;

            _sut.Update("dummy name", new Note("dummy name"));

            Assert.True(eventInvoked);
        }

        [Fact]
        public void ArchiveInvokesArchivedEvent()
        {
            _notesMetadataServiceMock.Setup(x => x.Get(It.IsAny<string>())).Returns(new NoteMetadata());

            bool eventInvoked = false;
            _sut.Archived += (object sender, ArchivedNoteEventArgs e)
                => eventInvoked = true;

            _sut.Archive(new Note("dummy name"));

            Assert.True(eventInvoked);
        }

        [Fact]
        public void RestoreInvokesRestoredEvent()
        {
            _notesMetadataServiceMock.Setup(x => x.Get(It.IsAny<string>())).Returns(new NoteMetadata());

            bool eventInvoked = false;
            _sut.Restored += (object sender, RestoredNoteEventArgs e)
                => eventInvoked = true;

            _sut.Restore(new Note("dummy name"));

            Assert.True(eventInvoked);
        }

        [Fact]
        public void DeleteInvokesDeletedEvent()
        {
            bool eventInvoked = false;
            _sut.Deleted += (object sender, DeletedNoteEventArgs e)
                => eventInvoked = true;

            _sut.Delete(new Note("dummy name")
            {
                Metadata = new NoteMetadata()
            });

            Assert.True(eventInvoked);
        }

        [Fact]
        public void LoadReturnsOnlyNonArchivedNotes()
        {
            const string nonArchivedName = "non archived name";

            _notesRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(new List<Note> {
                    new Note(nonArchivedName),
                    new Note(Globals.ArchivePrefix + "/" + "archived name")
                });

            Note[] notes = _sut.Load();

            Assert.Single(notes);
            Assert.Equal(nonArchivedName, notes[0].Name);
        }

        [Fact]
        public void LoadReturnsNotesWithMetadata()
        {
            _notesMetadataServiceMock.Setup(x => x.Get(It.IsAny<string>())).Returns(new NoteMetadata());

            _notesRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(new List<Note> {
                    new Note("dummy name"),
                    new Note("dummy name 2")
                });

            Note[] notes = _sut.Load();

            foreach (Note note in notes)
            {
                Assert.NotNull(note.Metadata);
            }
        }

        [Fact]
        public void LoadArchivedReturnsOnlyArchivedNotes()
        {
            Note[] notes = _sut.LoadArchived();

            _notesRepositoryMock.Verify(x => x.GetAllArchived());
        }

        [Fact]
        public void LoadArchivedReturnsNotesWithMetadata()
        {
            _notesMetadataServiceMock.Setup(x => x.Get(It.IsAny<string>())).Returns(new NoteMetadata());

            _notesRepositoryMock
                .Setup(x => x.GetAllArchived())
                .Returns(new List<Note> {
                    new Note(Globals.ArchivePrefix + "/dummy name"),
                    new Note(Globals.ArchivePrefix + "/dummy name 2")
                });

            Note[] notes = _sut.LoadArchived();

            foreach (Note note in notes)
            {
                Assert.NotNull(note.Metadata);
            }
        }

        [Fact]
        public void GetFontThatAllNotesUseReturnsDefaultIfNoNotes()
        {
            _notesMetadataServiceMock.Setup(x => x.GetDistinctFonts()).Returns(Array.Empty<string>());

            string font = _sut.GetFontThatAllNotesUse();

            Assert.Equal(Globals.DefaultFontFamily, font);
        }

        [Fact]
        public void GetFontThatAllNotesUseReturnsFirstIfOneNote()
        {
            const string singleFontName = "single font";
            _notesMetadataServiceMock.Setup(x => x.GetDistinctFonts()).Returns(new string[] { singleFontName });

            string font = _sut.GetFontThatAllNotesUse();

            Assert.Equal(singleFontName, font);
        }

        [Fact]
        public void GetFontThatAllNotesUseReturnsNullIfMultipleNotes()
        {
            _notesMetadataServiceMock.Setup(x => x.GetDistinctFonts()).Returns(new string[] { "font one", "font two" });

            string font = _sut.GetFontThatAllNotesUse();

            Assert.Null(font);
        }

        [Fact]
        public void GetFontSizeThatAllNotesUseReturnsDefaultIfNoNotes()
        {
            _notesMetadataServiceMock.Setup(x => x.GetDistinctFontSizes()).Returns(Array.Empty<int>());

            int? fontSize = _sut.GetFontSizeThatAllNotesUse();

            Assert.Equal(Globals.DefaultFontSize, fontSize);
        }

        [Fact]
        public void GetFontSizeThatAllNotesUseReturnsFirstIfOneNote()
        {
            const int singleFontSize = 16;
            _notesMetadataServiceMock.Setup(x => x.GetDistinctFontSizes()).Returns(new int[] { singleFontSize });

            int? fontSize = _sut.GetFontSizeThatAllNotesUse();

            Assert.Equal(singleFontSize, fontSize);
        }

        [Fact]
        public void GetFontSizeThatAllNotesUseReturnsNullIfMultipleNotes()
        {
            _notesMetadataServiceMock.Setup(x => x.GetDistinctFontSizes()).Returns(new int[] { 16, 22 });

            int? fontSize = _sut.GetFontSizeThatAllNotesUse();

            Assert.Null(fontSize);
        }
    }
}
