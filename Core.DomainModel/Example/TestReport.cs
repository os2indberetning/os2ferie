using System;

namespace Core.DomainModel.Example
{
    public class TestReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Type { get; set; }
        public long Timestamp { get; set; }
        //public DateTimeOffset ReportedDate { get; set; }
        //public DateTime DateTimeTest { get; set; }
    }
}