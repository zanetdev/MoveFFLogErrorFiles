using Delimon.Win32.IO;
using System;
using System.Threading.Tasks;

namespace MediaFileIntegrityChecker
{
    public class MediaFileIntegrityChecker
    {
        public event EventHandler<string> LogInfoEvent;
        //private DirectoryInfo _currentDirectory;


        private readonly DirectoryInfo _inputRoot;
        private readonly DirectoryInfo _outputRootError;
        private readonly string _fileExtension;
        private readonly bool _overwriteFfLog;
        private readonly string _mediaDirectoryIdentifier;
        private readonly DirectoryInfo _outputRootChecked;


        public MediaFileIntegrityChecker(DirectoryInfo inputRoot, DirectoryInfo outputRootChecked, DirectoryInfo outputRootError, string mediaDirectoryIdentifier, string fileExtension, bool overwriteFfLog)
        {
            _inputRoot = inputRoot;
            _outputRootError = outputRootError;
            _outputRootChecked = outputRootChecked;
            _fileExtension = fileExtension;
            _overwriteFfLog = overwriteFfLog;
            _mediaDirectoryIdentifier = mediaDirectoryIdentifier;
        }

        public async Task<bool> Process()
        {
            await RecurseDirectory(_inputRoot);
            return true;
        }

        private async Task<bool> RecurseDirectory(DirectoryInfo currentDirectory)
        {
            //Only process the files in a directory if that directory is a media directory
            if (currentDirectory.FullName.ToUpper().Contains(_mediaDirectoryIdentifier.ToUpper()))
            {
                if (LogInfoEvent != null) LogInfoEvent(this, "PROCESSING DIRECTORY ::: " + currentDirectory.FullName);



                foreach (FileInfo mediaFile in currentDirectory.GetFiles())
                {
                    if (mediaFile.FullName.ToUpper().EndsWith("." + _fileExtension.ToUpper()))
                    {


                        var ffFileInfo = new FfFileInfo(mediaFile.FullName, _overwriteFfLog,
                            FfFileInfo.LogErrorLevel.Error);
                        ffFileInfo.LogInfoEvent += LogInfoEvent;

                        if (ffFileInfo.HasErrors)
                        {
                            if (LogInfoEvent != null) LogInfoEvent(this, "FILE CHECK FAILED");
                            var currentDestinationErrDir = new DelimonExtended.DirectoryInfo(currentDirectory.FullName.Replace(_inputRoot.FullName, _outputRootError.FullName));
                            ffFileInfo.MoveFiles(destinationDirectory: currentDestinationErrDir);
                        }
                        else
                        {
                            if (LogInfoEvent != null) LogInfoEvent(this, "FILE CHECK OK");
                            var currentDestinationChkDir = new DelimonExtended.DirectoryInfo(currentDirectory.FullName.Replace(_inputRoot.FullName, _outputRootChecked.FullName));
                            ffFileInfo.MoveFiles(destinationDirectory: currentDestinationChkDir);
                        }
                    }
                    else
                    {
                        if (!mediaFile.Name.ToUpper().EndsWith(".FFLOG"))
                        {
                            if (LogInfoEvent != null) LogInfoEvent(this, "ACCOMPANYING FILE - NO CHECK REQUIRED");
                            var currentDestinationChkDir = new DelimonExtended.DirectoryInfo(currentDirectory.FullName.Replace(_inputRoot.FullName, _outputRootChecked.FullName));
                            mediaFile.MoveTo(currentDestinationChkDir.FullName + "\\" + mediaFile.Name);
                        }

                    }
                }
            }

            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                await RecurseDirectory(dir);
            }
            return true;
        }
    }
}
