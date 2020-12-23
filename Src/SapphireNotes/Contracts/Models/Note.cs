using System;

namespace SapphireNotes.Contracts.Models
{
    public class Note
    {
        public Note(string name) : this(name, null, null, null)
        {
        }

        public Note(string name, string content) : this(name, content, null, null)
        {
        }

        public Note(string name, string content, DateTime? lastWriteTime) : this(name, content, lastWriteTime, null)
        {
        }

        public Note(string name, string content, DateTime? lastWriteTime, NoteMetadata metadata)
        {
            Name = name;
            Content = content;
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
