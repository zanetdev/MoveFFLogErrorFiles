
# Media File Integrity Inspector

## Purpose

The purpose of this utility is to allow media files to be checked for 
integrity and then moved to directories identifying the status of the files.

The original reason that I wrote this was that I had ripped my collection of CDs
to lossless digital media files (ALAC). At some stage these rips became corrupted. I 
suspect this was due to a faulty memory module in the the PC which at one time 
handled moving of the files from one drive to another.

In any event, they were randomly corrupted and during playback occasionaly 
loud ticks, bursts of static, squeeks or pops could be heard making the experience
of listening to music very unpleasant and jarring.

Something to be aware of is that it is quite possible that you may not be 
able to access your files after running this utility as it uses the Delimon.win32
library. In order to access these files you will again need to restructure them
in some way in order to access them with the regular windows explorer. You can use
the Delimon proof of concept explorr to accomplish this 

## Rationale
In order to solve this problem I turned to the FFMpeg library which is able to assist
in identifying these files. The problem though is that processing and marking
files with suspected errors required manual intervention. 

This was the primary motivation for this project which manages the FFMpeg processor 
and moving the files about.

An additional problem was that the files were on a NAS system with an SMB share.
Due to the way the files were structured in the file system the length could
quite quickly exceed windows regular limit on the length of the file name. IE 260
characters. To solve this issue I incorporated the Delimon.Win32.IO library so that
processing and moving long file names would not be an issue.

## Usage
#### Command Line Switches


|Switch |Required| Description                          |
|------|--------| ------------------------------------ |
|-i     | Y      | Root Input directory to be processed. This contains the files that you want the utility to check 
|-e     | Y      | Root Output directory for files identified with errors. The directory where you want files suspected of errors to be moved
|-o     | Y      | Root Output directory for files identified with NO errors. The directory where you want files that have no errors suspected to be moved. This will include all other non processed files in those directories.
|-w     | Y      | Overwrite fflog files. If 'N' then the utility will not process those filaes again for error swith the ffmpeg utility
|-d     | Y      | An identifer which must be present in the path in order for the utility to process the directory and its sub directories as a media directory
|-x     | Y      | The extension of the media file to process.


#### Example:
We have the following directory structure:

c
>music
>>artist1
>>>album1
>>>>SomeFileWithAnError1.mp4
>>>>
>>>>SomeFileWithAnError2.mp4
>>>>
>>>>SomeFileWithoutAnError3.mp4
>>>>
>>>>SomeNonMediaFile.zip
>>>>
>>>album[XYZ]2
>>>>SomeFileWithAnError1.mp4
>>>>
>>>>SomeFileWithoutAnError2.mp4
>>>>
>>>>SomeNonMediaFile.zip
>>>>
>>artist[XYZ]2
>>>album1
>>>>SomeFileWithoutAnError1.mp4
>>>>
>>>>SomeFileWithAnError2.mp4
>>>>
>>>album2
>>>>
>>>>SomeFileWithoutAnError1.mp4
>>>>
>>>>SomeFileWithAnError2.mp4


xxx.exe -i "c:\music" - e "c:\musicWithErrors" -o "c:\musicWithoutErrors" -w N -d "[XYZ]"

Will Result in the following

c
>music
>>artist1
>>>album1
>>>>SomeFileWithAnError1.mp4
>>>>
>>>>SomeFileWithAnError2.mp4
>>>>
>>>>SomeFileWithoutAnError3.mp4
>>>>
>>>>SomeNonMediaFile.zip

As can be seen above the entire c:\music\artist1\album1 directory structure
was not processed. The reason for this is that it does not contain the input given with the [N] switch
and so was completely left alone.

c
>musicWithErrors
>>artist1
>>>album[XYZ]2
>>>>SomeFileWithAnError1.mp4
>>>>
>>artist[XYZ]2
>>>album1
>>>>SomeFileWithAnError2.mp4
>>>>
>>>album2
>>>>SomeFileWithAnError2.mp4

c
>musicWithoutErrors
>>artist1
>>>album[XYZ]2
>>>>SomeFileWithoutAnError2.mp4
>>>>
>>>>SomeNonMediaFile.zip
>>>>
>>artist[XYZ]2
>>>album1
>>>>SomeFileWithoutAnError1.mp4
>>>>
>>>album2
>>>>SomeFileWithoutAnError1.mp4

Above it can be seen that the Non Media File was moved to the same location 
as media files that were not suspected of having an error.

### Dependant Projects
[Command Line Parser](https://github.com/gsscoder/commandline)

[Delimon.Win32.IO](https://gallery.technet.microsoft.com/scriptcenter/DelimonWin32IO-Library-V40-7ff6b16c)

[FFMpeg](https://github.com/FFmpeg/FFmpeg)
