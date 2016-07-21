using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFileIntegrityChecker
{
    public class MediaFileIntegrityChecker
    {
         
        public static void RecurseDirectory(string InputRoot, string OutputRoot, string FileExtension, string CurrentDirectory, bool OverwriteFFLog)
        {
            var InputDirectory = new System.IO.DirectoryInfo(CurrentDirectory);

            foreach (System.IO.FileInfo mediaFile in InputDirectory.GetFiles("*." + FileExtension))
            {
                var fflogFile = getFFLogFile(mediaFile.FullName, OverwriteFFLog);

                if (fflogFile.Length > 0)   //Check for Errors in fflog file
                {
                    Console.WriteLine(mediaFile.FullName);
                    try
                    {
                        //Move Audio File
                        var mediaFileNewName = mediaFile.FullName.Replace(InputRoot, OutputRoot);
                        System.IO.Directory.CreateDirectory(mediaFileNewName.Replace(mediaFile.Name, ""));
                        mediaFile.MoveTo(mediaFileNewName);
                        Console.WriteLine("Moved:::" + mediaFileNewName);

                        //Move fflog file
                        var fflogNewName = mediaFile.FullName.Replace(InputRoot, OutputRoot) + ".fflog";
                        fflogFile.MoveTo(fflogNewName);
                        Console.WriteLine("Moved:::" + fflogNewName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing:" + mediaFile.FullName);
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            foreach (System.IO.DirectoryInfo dir in InputDirectory.GetDirectories())
            {
                RecurseDirectory(InputRoot, OutputRoot, FileExtension, dir.FullName, OverwriteFFLog);
            }
        }

        public static System.IO.FileInfo getFFLogFile(string MediaFileName, bool OverwriteFFLog)
        {
            var fflogfilename = MediaFileName + ".fflog";

            if (!OverwriteFFLog)
            {
                if (System.IO.File.Exists(fflogfilename))
                {
                    Console.WriteLine("Already Processed:::" + MediaFileName);
                    return new System.IO.FileInfo(fflogfilename);
                }
            }
            //Tests a media file with the FFMpeg utility. Results are stored in an "fflog" file corresponding to the Media File's Name
            var p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"AdditionalFiles\FFMpeg\ffmpeg.exe";
            p.StartInfo.Arguments = @"-v error -i """ + MediaFileName + @""" -f null - ";

            p.Start();
            string outputError = p.StandardError.ReadToEnd();
            
            p.WaitForExit();

            using (var fflogfile = System.IO.File.CreateText(fflogfilename))
            {
                fflogfile.Write(outputError);
                fflogfile.Flush();
                fflogfile.Close();
            }
            return new System.IO.FileInfo(fflogfilename);

        }
    }
}
