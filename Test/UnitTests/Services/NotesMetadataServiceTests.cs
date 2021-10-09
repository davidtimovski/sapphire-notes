using System;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Services;
using Xunit;

namespace UnitTests.Services
{
    public class NotesMetadataServiceTests
    {
        private readonly INotesMetadataService _sut;

        public NotesMetadataServiceTests()
        {
            _sut = new NotesMetadataService();

            _sut.Initialize(Array.Empty<string>());
        }

        [Fact]
        public void Add()
        {
            const string noteName = "dummy name";
            var metadata = new NoteMetadata();
            _sut.Add(noteName, metadata);

            NoteMetadata retrieved = _sut.Get(noteName);
            Assert.Equal(metadata, retrieved);
        }

        [Fact]
        public void AddOrUpdate()
        {
            const string noteName = "dummy name";
            _sut.Add(noteName, new NoteMetadata { FontFamily = "Arial" });

            const string newFontFamily = "Open Sans";
            _sut.AddOrUpdate(noteName, new NoteMetadata { FontFamily = newFontFamily });

            NoteMetadata retrieved = _sut.Get(noteName);
            Assert.Equal(newFontFamily, retrieved.FontFamily);
        }

        [Fact]
        public void Contains()
        {
            const string noteName = "dummy name";
            var metadata = new NoteMetadata();
            _sut.Add(noteName, metadata);

            bool contains = _sut.Contains(noteName);
            Assert.True(contains);
        }

        [Fact]
        public void Remove()
        {
            const string noteName = "dummy name";
            _sut.Add(noteName, new NoteMetadata());

            _sut.Remove(noteName);

            bool contains = _sut.Contains(noteName);
            Assert.False(contains);
        }

        [Fact]
        public void Clear()
        {
            const string noteName = "dummy name";
            _sut.Add(noteName, new NoteMetadata());

            _sut.Clear();

            bool contains = _sut.Contains(noteName);
            Assert.False(contains);
        }

        [Theory]
        [InlineData("Arial", "Arial", "Open Sans", 2)]
        [InlineData("Arial", "Arial", "Arial", 1)]
        public void GetDistinctFonts(string font1, string font2, string font3, int distinct)
        {
            var fonts = new[] { font1, font2, font3 };
            for (var i = 0; i < fonts.Length; i++)
            {
                _sut.Add($"dummy name {i}", new NoteMetadata { FontFamily = fonts[i] });
            }

            string[] distinctFonts = _sut.GetDistinctFonts();

            Assert.Equal(distinct, distinctFonts.Length);

            foreach (var font in fonts)
            {
                Assert.Contains(font, distinctFonts);
            }
        }

        [Theory]
        [InlineData(15, 15, 23, 2)]
        [InlineData(15, 15, 15, 1)]
        public void GetDistinctFontSizes(int fontSize1, int fontSize2, int fontSize3, int distinct)
        {
            var fontSizes = new[] { fontSize1, fontSize2, fontSize3 };
            for (var i = 0; i < fontSizes.Length; i++)
            {
                _sut.Add($"dummy name {i}", new NoteMetadata { FontSize = fontSizes[i] });
            }

            int[] distinctFontSizes = _sut.GetDistinctFontSizes();

            Assert.Equal(distinct, distinctFontSizes.Length);

            foreach (var fontSize in fontSizes)
            {
                Assert.Contains(fontSize, distinctFontSizes);
            }
        }

        [Fact]
        public void SetFontForAll()
        {
            _sut.Add("dummy name 1", new NoteMetadata { FontFamily = "Arial" });
            _sut.Add("dummy name 2", new NoteMetadata { FontFamily = "Open Sans" });

            const string newFont = "Roboto";
            _sut.SetFontForAll(newFont);

            NoteMetadata dummy1Metadata = _sut.Get("dummy name 1");
            Assert.Equal(newFont, dummy1Metadata.FontFamily);

            NoteMetadata dummy2Metadata = _sut.Get("dummy name 2");
            Assert.Equal(newFont, dummy2Metadata.FontFamily);
        }

        [Fact]
        public void SetFontSizeForAll()
        {
            _sut.Add("dummy name 1", new NoteMetadata { FontSize = 15 });
            _sut.Add("dummy name 2", new NoteMetadata { FontSize = 23 });

            const int newFontSize = 18;
            _sut.SetFontSizeForAll(newFontSize);

            NoteMetadata dummy1Metadata = _sut.Get("dummy name 1");
            Assert.Equal(newFontSize, dummy1Metadata.FontSize);

            NoteMetadata dummy2Metadata = _sut.Get("dummy name 2");
            Assert.Equal(newFontSize, dummy2Metadata.FontSize);
        }
    }
}
