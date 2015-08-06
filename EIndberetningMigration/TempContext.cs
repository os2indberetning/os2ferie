using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DataAccess;

namespace EIndberetningMigration
{
    public class TempContext : DataContext
    {
        public TempContext() : base()
        {
            Configuration.AutoDetectChangesEnabled = false;
        }
    }
}
