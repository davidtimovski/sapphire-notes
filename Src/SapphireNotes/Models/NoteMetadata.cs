namespace SapphireNotes.Models
{
    public class NoteMetadata
    {
        public NoteMetadata()
        {
            FontSize = 15;
            FontFamily = "resm:SapphireNotes.Assets.Fonts?assembly=SapphireNotes#Open Sans";
        }

        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public int CursorPosition { get; set; }
    }
}
