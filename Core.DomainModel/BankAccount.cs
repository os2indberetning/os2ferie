using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
    }
}
