
using System;

namespace Core.DomainModel
{
    public class FileGenerationSchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Generated { get; set; }
    }
}
