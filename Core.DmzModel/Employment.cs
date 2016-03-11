namespace Core.DmzModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Employment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string EmploymentPosition { get; set; }

        public long StartDateTimestamp { get; set; }

        public long EndDateTimestamp { get; set; }

        public string ManNr { get; set; }

        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
