using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail.LogMailer
{
    public interface ILogReader
    {
        List<string> Read(string file);
    }
}
