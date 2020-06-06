using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    public static class Logger
    {
        public static void Loggear(String stringALoggear)
        {
            using (StreamWriter w = File.AppendText("logger.txt"))
            {
                w.WriteLine(stringALoggear);
            }
        }
    }
}
