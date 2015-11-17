using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.Logger
{
    public class Logger : ILogger
    {
        public void Log(string msg, string fileName)
        {
            using (var file = new StreamWriter("c://logs//os2eindberetning//" + fileName + ".log", true))
            {
                var time = DateTime.Now.ToString();
                file.WriteLine(time + " : " + msg);
                file.Close();
            }
        }
    }
}
