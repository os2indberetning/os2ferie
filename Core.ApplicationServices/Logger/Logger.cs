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

        public void Log(string msg, string fileName, Exception ex)
        {
            using (var file = new StreamWriter("c://logs//os2eindberetning//" + fileName + ".log", true))
            {
                var time = DateTime.Now.ToString();
                file.WriteLine(time + " : " + msg);
                if (ex != null) file.WriteLine(ex);
                file.Close();
            }
        }

        public void Log(string msg, string fileName, Exception ex, int level)
        {
            using (var file = new StreamWriter("c://logs//os2eindberetning//" + fileName + ".log", true))
            {
                var time = DateTime.Now.ToString();
                file.WriteLine(time + " : " + "[Niveau " + level + "] - " + msg);
                if (ex != null) file.WriteLine(ex);
                file.Close();
            }
        }

        public void Log(string msg, string fileName, int level)
        {
            Log(msg, fileName, null, level);
        }
    }
}