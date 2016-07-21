using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFileIntegrityChecker
{
    class Options
    {
        [Option('i', "input", Required = false, HelpText = "Root Input directory to be processed.")]
        public string InputDirectory { get; set; }

        [Option('o', "output", Required = false, HelpText = "Root Output directory for files identified with errors")]
        public string OutputDirectory { get; set; }

        [Option('w', "overwrite", Required = false, HelpText = "Overwrite fflog files")]
        public string Overwrite { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
