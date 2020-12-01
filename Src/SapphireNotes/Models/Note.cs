namespace SapphireNotes.Models
{
    public class Note
    {
        public Note()
        {
            Name = string.Empty;
        }

        public Note(string name, string filePath, string fileContents, NoteMetadata metadata)
        {
            FilePath = filePath;
            Name = name;
            Text = fileContents;
            Metadata = metadata;
        }

        public string FilePath { get; set; }
        public bool IsDirty { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public NoteMetadata Metadata { get; set; }
    }
}
