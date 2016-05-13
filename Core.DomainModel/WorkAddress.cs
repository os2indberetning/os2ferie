using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class WorkAddress : Address
    {
        public int OrgUnitId { get; set; }
    }
}
