namespace Core.DmzModel
{
    public class UserAuth
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string GuId { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
