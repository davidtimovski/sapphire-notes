using System;

namespace SapphireNotes.Contracts.Models
{
    public class Note
    {
        public Note(string name) : this(name, null, null, null)
        {
        }

        public Note(string name, string fileContents) : this(name, fileContents, null, null)
        {
        }

        public Note(string name, string fileContents, DateTime? lastWriteTime) : this(name, fileContents, lastWriteTime, null)
        {
        }

        public Note(string name, string fileContents, DateTime? lastWriteTime, NoteMetadata metadata)
        {
            Name = name;
            Content = fileContents;
            LastWriteTime = lastWriteTime;
            Metadata = metadata;
        }

        public bool IsDirty { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public NoteMetadata Metadata { get; set; }
    }
}
