using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.Logger
{
    public interface ILogger
    {
        void Log(string msg, string fileName);
        void Log(string msg, string fileName, Exception ex);

        void Log(string msg, string fileName, Exception ex, int level);
        void Log(string msg, string fileName, int level);
    }
}
