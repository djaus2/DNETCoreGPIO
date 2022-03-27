using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleDNETCoreGPIO
{
    class Program
    {
        static void Main(string[] args)
        {
            int delay = 10;
            string deviceConnectionStrimg = "";
            List<string> argsList = args.ToList<string>();
            if (args.Length == 0)
            {
                argsList.Add("0");
            }
            if (args.Length < 2)
            {
                argsList.Add(".");
            }
            if (args.Length < 3)
            {
                argsList.Add(delay.ToString());
            }
            if (args.Length < 4)
            {
                argsList.Add(deviceConnectionStrimg);
            }

            var Args = argsList.ToArray<string>();
            DotNetCoreCoreGPIO.Program.Main(Args);
        }
    }
}
