using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaFileIntegrityChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                string inputDirectoryString;
                if (options.InputDirectory?.Length > 0)
                {
                    inputDirectoryString = options.InputDirectory;
                    if (!System.IO.Directory.Exists(inputDirectoryString))
                    { 
                        throw new Exception("Input Directory does not exist");
                    }
                }
                else
                {
                    inputDirectoryString = AppDomain.CurrentDomain.BaseDirectory;
                }

                string outputDirectoryString;
                if (options.OutputDirectory?.Length > 0)
                {
                    outputDirectoryString = options.OutputDirectory;
                    if (!System.IO.Directory.Exists(inputDirectoryString))
                    {
                        throw new Exception("Output Directory does not exist");
                    }
                }
                else
                {
                    outputDirectoryString = AppDomain.CurrentDomain.BaseDirectory;
                }
                Console.WriteLine("Media File Integrity Checker");
                Console.WriteLine("Processing Started");
                MediaFileIntegrityChecker.RecurseDirectory(inputDirectoryString, outputDirectoryString, "m4a", inputDirectoryString, options.Overwrite.ToUpper() == "Y" );
                Console.WriteLine("Processing Finished");
                Console.ReadKey();
            }

        }

    }
}
