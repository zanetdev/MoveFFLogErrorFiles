using CommandLine;
using CommandLine.Text;

namespace MediaFileIntegrityCheckerConsole
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Root Input directory to be processed.")]
        public string InputDirectory { get; set; }

        [Option('e', "erroroutput", Required = true, HelpText = "Root Output directory for files identified with errors")]
        public string OutputErrorDirectory { get; set; }

        [Option('o', "output", Required = true, HelpText = "Root Output directory for files identified with NO errors")]
        public string OutputCheckedDirectory { get; set; }

        [Option('w', "overwrite", Required = true, HelpText = "Overwrite fflog files")]
        public string Overwrite { get; set; }

        [Option('d', "mediadirectoryidentifier", Required = true, HelpText = "Identifer which must be present in order to process the directory as a media directory")]
        public string MediaDirectoryIdentifier { get; set; }

        [Option('x', "mediafileextension", Required = true, HelpText = "The extension of the media files we are processing. Eg MP4 or M4A")]
        public string MediaFileExtension { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
