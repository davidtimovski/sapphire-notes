using System;

namespace SapphireNotes.Exceptions
{
    public class MoveNotesException : Exception
    {
        public MoveNotesException(string message) : base(message) { }
    }
}
