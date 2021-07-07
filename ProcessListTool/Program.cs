using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ProcessListTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var encoding = Encoding.UTF8;
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "current running processes.txt");
            File.WriteAllText(configPath, "", encoding);
            Process[] processList = Process.GetProcesses();
            foreach (Process p in processList)
            {
                if (string.IsNullOrEmpty(p.MainWindowTitle))
                {
                    continue;
                }
                Console.WriteLine(p.MainWindowTitle);
                File.AppendAllText(configPath, $"{p.MainWindowTitle}{Environment.NewLine}", encoding);
            }

            Console.ReadLine();
        }
    }
}
