using Nito.AsyncEx;
using System;
using System.Threading.Tasks;

namespace MediaFileIntegrityCheckerConsole
{
    class Program
    {
        public static async Task MainAsync(string[] args)
        {
            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (Delimon.Win32.IO.Directory.Exists(options.InputDirectory))
                {
                    var inputDirectory = new Delimon.Win32.IO.DirectoryInfo(options.InputDirectory);

                    var outputErrorDirectory = new DelimonExtended.DirectoryInfo(options.OutputErrorDirectory);
                    var outputCheckedDirectory = new DelimonExtended.DirectoryInfo(options.OutputCheckedDirectory);

                    var mfic = new MediaFileIntegrityChecker.MediaFileIntegrityChecker(inputDirectory, outputCheckedDirectory,
                        outputErrorDirectory, options.MediaDirectoryIdentifier, options.MediaFileExtension,
                        options.Overwrite.ToUpper() == "Y");
                    mfic.LogInfoEvent += Mfic_LogInfoEvent;
                    await mfic.Process();
                    mfic.LogInfoEvent -= Mfic_LogInfoEvent;
                }
                else
                {
                    Console.Write("Input Directory does not exist");
                }
            }

            else
            {
                Console.Write("Could Not Parse Command Line Arguments");
            }
        }


        public static void Main(string[] args)
        {

            AsyncContext.Run(() => MainAsync(args));
            Console.WriteLine("Processing Finished");
            Console.ReadKey();

        }

        private static void Mfic_LogInfoEvent(object sender, string e)
        {
            Console.WriteLine(e);
        }
    }
}
