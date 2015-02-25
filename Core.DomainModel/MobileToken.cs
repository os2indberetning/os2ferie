using System;

namespace Core.DomainModel
{
    public enum MobileTokenStatus
    {
        Deleted,
        Activated,
        Created
    }

    public class MobileToken
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Guid Guid { get; set; }
        public MobileTokenStatus Status { get; set; }
        public String Token { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public string StatusToPresent { get; set; }
    }
}
