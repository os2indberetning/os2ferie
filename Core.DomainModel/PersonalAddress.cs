
namespace Core.DomainModel
{
    public enum PersonalAddressType
    {
        Standard,
        Home,
        Work,
        AlternativeHome,
        AlternativeWork,
        OldHome
    }

    public class PersonalAddress : Address
    {
        public PersonalAddressType Type { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
