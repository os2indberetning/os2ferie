namespace Core.DmzModel
{
    public partial class Token
    {
        public string GuId { get; set; }

        public string TokenString { get; set; }

        public int Status { get; set; }

        public int ProfileId { get; set; }

        public int Id { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
