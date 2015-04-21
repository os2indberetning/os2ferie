using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ImportDBContext())
            {
                var firstOrg = context.Organisation.First();
                var orgs = context.Organisation.Where(x => x.LOSOrgId > 880000);
                int xy = 1;
            }
        }
    }
}
