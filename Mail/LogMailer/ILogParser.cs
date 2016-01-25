using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail.LogMailer
{
    public interface ILogParser
    {
        List<string> Messages(List<string> log, DateTime date);
    }
}
