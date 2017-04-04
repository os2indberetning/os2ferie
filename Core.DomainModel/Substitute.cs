namespace Core.DomainModel
{
    public class Substitute
    {
        public int Id { get; set; }
        public long StartDateTimestamp { get; set; }
        public long EndDateTimestamp { get; set; }

        public int LeaderId { get; set; }
        public virtual Person Leader { get; set; }
        public int SubId { get; set; }
        public virtual Person Sub { get; set; }
        public virtual Person Person { get; set; }
        public int PersonId { get; set; }
        public int OrgUnitId { get; set; }
        public virtual OrgUnit OrgUnit { get; set; }
        public int? CreatedById { get; set; }
        public virtual Person CreatedBy { get; set; }

        public ReportType Type { get; set; }
    }
}
