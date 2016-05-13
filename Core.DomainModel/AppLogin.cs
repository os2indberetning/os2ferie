using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class AppLogin
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string GuId { get; set; }
    }
}
