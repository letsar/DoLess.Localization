using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoLess.Localization.Core;

namespace DoLess.Localization.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ResxConverter resxConverter = new ResxConverter(args[0]);
                bool success = resxConverter.GenerateLocalization();
                if (success)
                {
                    Console.WriteLine("DoLess.Localization converted all the resx files");
                }
                else
                {
                    Console.Error.WriteLine("DoLess.Localization failed to convert all the resx files");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
