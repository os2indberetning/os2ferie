using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail.LogMailer
{
    public class LogReader : ILogReader
    {
        public List<string> Read(string file)
        {
            return System.IO.File.ReadAllLines(file).Reverse().ToList();
        }
    }
}
