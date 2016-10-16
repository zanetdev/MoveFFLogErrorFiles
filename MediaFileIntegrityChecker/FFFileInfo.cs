using System;
using System.Diagnostics;

namespace MediaFileIntegrityChecker
{
    public class FfFileInfo
    {
        public event EventHandler<string> LogInfoEvent;

        public enum LogErrorLevel
        {

            /// <summary>
            /// Show nothing at all; be silent.
            /// </summary>
            Quiet,
            /// <summary>
            /// Only show fatal errors which could lead the process to crash, such as an assertion failure. This is not currently used for anything.
            /// </summary>
            Panic,
            /// <summary>
            /// Only show fatal errors. These are errors after which the process absolutely cannot continue.
            /// </summary>
            Fatal,
            /// <summary>
            /// Show all errors, including ones which can be recovered from.
            /// </summary>
            Error,
            /// <summary>
            /// Show all warnings and errors. Any message related to possibly incorrect or unexpected events will be shown.
            /// </summary>
            Warning,
            /// <summary>
            /// Show informative messages during processing. This is in addition to warnings and errors. This is the default value.
            /// </summary>
            Info,
            /// <summary>
            /// Same as info, except more verbose.
            /// </summary>
            Verbose,
            /// <summary>
            /// Show everything, including debugging information.
            /// </summary>
            Debug,

            Trace

        }

        private readonly Delimon.Win32.IO.FileInfo _ffLogFile;
        private readonly Delimon.Win32.IO.FileInfo _mediaFile;
        private readonly bool _overwrite;
        private readonly LogErrorLevel _logErrorLevel;


        public FfFileInfo(string mediaFileName, bool overwrite, LogErrorLevel logErrorLevel)
        {
            _logErrorLevel = logErrorLevel;
            _overwrite = overwrite;
            _mediaFile = new Delimon.Win32.IO.FileInfo(mediaFileName);
            _ffLogFile = GetFfLogFile();
        }

        public bool HasErrors
        {
            get
            {
                return Delimon.Win32.IO.File.ReadAllText(_ffLogFile.FullName).Length > 0;
            }
        }

        public Delimon.Win32.IO.FileInfo FfLogFile
        {
            get { return _ffLogFile; }
        }

        public Delimon.Win32.IO.FileInfo MediaFile
        {
            get { return _mediaFile; }
        }

        public void MoveFiles(Delimon.Win32.IO.DirectoryInfo destinationDirectory)

        {
            if (LogInfoEvent != null)
            {
                LogInfoEvent(this, "MOVING FILE ::: " + _mediaFile.FullName);
                try
                {
                    _mediaFile.MoveTo(destinationDirectory.FullName + "\\" + _mediaFile.Name);
                    _ffLogFile.MoveTo(destinationDirectory.FullName + "\\" + FfLogFile.Name);
                    LogInfoEvent(this, "SUCCESS MOVING ::: " + _mediaFile.FullName);
                }
                catch (Exception ex)
                {
                    LogInfoEvent(this, "FAILURE MOVING ::: " + _mediaFile.FullName);
                    LogInfoEvent(this, "                   " + ex.Message);
                }
            }
        }


        private Delimon.Win32.IO.FileInfo GetFfLogFile()
        {
            var fflogfilename = _mediaFile.FullName + ".fflog";

            if (Delimon.Win32.IO.File.Exists(fflogfilename))
            {
                if (!_overwrite)
                {
                    return new Delimon.Win32.IO.FileInfo(fflogfilename);
                }
                else
                {
                    Delimon.Win32.IO.File.Delete(fflogfilename);
                }
            }

            //Tests a media file with the FFMpeg utility. Results are stored in an "fflog" file corresponding to the Media File's Name
            var p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"AdditionalFiles\FFMpeg\ffmpeg.exe";

            //ffmpeg seems to be case sensitive for its arguments
            p.StartInfo.Arguments = $"-v {_logErrorLevel.ToString().ToLower()} -i \"{_mediaFile.FullName}\" -f null - ";

            p.Start();
            string outputError = p.StandardError.ReadToEnd();

            p.WaitForExit();

            using (var fflogfile = Delimon.Win32.IO.File.CreateText(fflogfilename))
            {
                fflogfile.Write(outputError);
                fflogfile.Flush();
                fflogfile.Close();
            }
            return new Delimon.Win32.IO.FileInfo(fflogfilename);

        }

    }
}
