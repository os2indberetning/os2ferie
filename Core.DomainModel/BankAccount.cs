using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public enum AccountType
    {
        Omkostningssted,
        PSPElement
    }

    public class BankAccount
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public AccountType Type { get; set; }
        public string Description { get; set; }
    }
}
