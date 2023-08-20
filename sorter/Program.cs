using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection.PortableExecutable;

namespace sorter
{
    internal class Program
    {
        private static string src = "D:\\Consoles\\ROMs\\c64\\";

        static void CreateDir(string dest, bool verbose = false)
        {
            if (verbose == true)
            {
                Console.WriteLine("Creating DIR: " + dest);
            }
            Directory.CreateDirectory(src + dest);
        }

        static void Move()
        {
            foreach (string file in Directory.GetFiles(src + "\\EXAMPLE\\"))
            {
                File.Move(file, src + Path.GetFileName(file));
            }
        }

        static void Main(string[] args)
        {
            int i = 1;
            List<string> files = Directory.GetFiles(src).ToList();

            // First deal with non-ZIP files, and move them to miscellaneous directory
            foreach (string s in files.ToArray())
            {
                if (!s.EndsWith("zip"))
                {
                    CreateDir("\\MISC\\");
                    File.Move(s, src + "\\MISC\\" + Path.GetFileName(s), true);
                    files.Remove(s);
                }
            }

            CreateDir("\\ZIPs\\", true);
            // Next, extract the contents of all .ZIP archives, and move each .ZIP archive to ZIP directory
            foreach (string s in files)
            {
                //Console.Write("File op " + i + " out of " + files.Length);
                string file = Path.GetFileName(s);

                Console.WriteLine(i + " out of " + files.Count + ":\t\tExtracting " + file + " and moving to " + src + "\\ZIPs\\" + file);
                ZipFile.ExtractToDirectory(s, src, true);
                File.Move(s, src + "\\ZIPs\\" + file);
                i++;
            }




            /*int */
            i = 0;
            /*List<string> */
            files = Directory.GetFiles(src).ToList();

            // Them we move the directories that may have been created into their alphabetic counterparts.
            // These are taken from first letter of the directory name.
            List<string> dirs = Directory.GetDirectories(src).ToList();
            foreach (string dir in dirs)
            {
                string dirname = new DirectoryInfo(dir).Name;
                if (dirname == "ZIPs")
                {
                    continue;
                }
                if (dirname == "MISC")
                {
                    continue;
                }
                string CHAR_DIR = dirname[0].ToString().ToUpper();
                Directory.CreateDirectory(src + "\\" + CHAR_DIR);
                Directory.Move(dir, src + "\\" + CHAR_DIR + "\\" + dirname);
            }

            // Lastly, we sort each ROM image into alphabetical subdirectories.
            // For ROM images not starting with a letter, we sort them into the subdirectories 'NUM' and 'SYM', for numbers and symbols respectively.
            // These are taken from first letter of the filename.
            foreach (string file in files)
            {
                i++;
                string fileName = Path.GetFileName(file);
                string CHAR_DIR = "\\" + fileName.ToUpper()[0] + "\\";
                string dest;
                if (char.IsDigit(fileName[0]))
                {
                    CreateDir("NUM");
                    dest = src + "\\NUM\\" + fileName;
                }
                else if (char.IsSymbol(fileName[0]))
                {
                    CreateDir("SYM");
                    dest = src + "\\SYM\\" + fileName;
                }
                else
                {
                    CreateDir(CHAR_DIR);
                    dest = src + CHAR_DIR + fileName;
                }
                Console.WriteLine("Moving file: " + fileName + " to " + dest + "\n" + i + " out of " + files.Count);
                File.Move(file, dest, true);
            }

            Console.WriteLine("Done");
        }
    }
}