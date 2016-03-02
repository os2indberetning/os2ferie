namespace Core.DmzModel
{
    public class DriveReport
    {
        public int Id { get; set; }

        public string Date { get; set; }

        public string ManualEntryRemark { get; set; }

        public string Purpose { get; set; }

        public bool StartsAtHome { get; set; }

        public bool EndsAtHome { get; set; }

        public int EmploymentId { get; set; }

        public int ProfileId { get; set; }

        public int RateId { get; set; }

        public long? SyncedAt { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Rate Rate { get; set; }

        public virtual Route Route { get; set; }
    }
}
