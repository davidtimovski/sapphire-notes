using System.Collections.Generic;
using SapphireNotes.Contracts.Models;

namespace SapphireNotes.Contracts
{
    public interface INotesRepository
    {
        void Create(string name);
        string Create(string name, string content);
        void Update(string name, string newName);
        void Delete(string name);
        void DeleteArchived(string name);
        void Save(string name, string content);
        bool Exists(string name);
        string Archive(string name);
        string Restore(string name);
        IEnumerable<Note> GetAll();
        IEnumerable<Note> GetAllArchived();
    }
}
