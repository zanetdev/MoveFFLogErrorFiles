
namespace DelimonExtended
{
    public class DirectoryInfo : Delimon.Win32.IO.DirectoryInfo
    {
        public DirectoryInfo(string path) : base(path)
        {
            if (!Delimon.Win32.IO.Directory.Exists(path))
                ForceCreateDirectory(path);

        }

        /// <summary>
        /// Delimon's utility doesnt create a full directory structure if it doesnt exist.
        /// It only creates the final directory in the sequence, end generates an exception
        /// if the rest of the sequence doesnt yet exist.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public Delimon.Win32.IO.DirectoryInfo ForceCreateDirectory(string directory)
        {
            string[] directoryArr = directory.Split('\\');
            string currentdir = directoryArr[0];

            for (int i = 0; i < directoryArr.Length; i++)
            {
                var s = directoryArr[i];
                if (i == 0)
                {
                    currentdir = s;
                }
                else
                {
                    currentdir += "\\" + s;
                }

                if (!Delimon.Win32.IO.Directory.Exists(currentdir))
                    Delimon.Win32.IO.Directory.CreateDirectory(currentdir);
            }

            return new Delimon.Win32.IO.DirectoryInfo(currentdir);
        }
    }
}
