using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection.PortableExecutable;

namespace romsort
{
    internal class Program
    {
        private static string src = "U:\\Gaming\\Consoles\\Amstrad\\cpc6128\\dsk\\ZIPs\\";

        private static List<string>? files;

        static void Move()
        {
            foreach (string file in Directory.GetFiles(src + "\\EXAMPLE\\"))
            {
                File.Move(file, src + Path.GetFileName(file));
            }
        }

        static void CreateDir(string dest, bool verbose = false)
        {
            if (verbose == true)
            {
                Console.WriteLine("Creating DIR: " + dest);
            }
            Directory.CreateDirectory(src + dest);
        }

        static void ExistingFiles(List<string> files)
        {
            // First deal with non-ZIP files, and move them to miscellaneous directory
            foreach (string s in files.ToArray())
            {
                if (!s.EndsWith("zip"))
                {
                    if (!Directory.Exists(src + "\\MISC\\"))
                    {
                        CreateDir("\\MISC\\");
                    }
                    //File.Move(s, src + "\\MISC\\" + Path.GetFileName(s), true);
                    files.Remove(s);
                }
            }
        }

        static void ZIPExtractAndMove(List<string> files, int i)
        {
            // Next, extract the contents of all .ZIP archives, and move each .ZIP archive to ZIP directory
            foreach (string s in files)
            {
                //Console.Write("File op " + i + " out of " + files.Length);
                string file = Path.GetFileName(s);

                Console.WriteLine(i + " out of " + files.Count() + ":\t\tExtracting " + file + " and moving to " + src + "\\ZIPs\\" + file);
                ZipFile.ExtractToDirectory(s, src, true);
                File.Move(s, src + "\\ZIPs\\" + file);
                i++;
            }
        }

        static void SortDirectories()
        {
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
                if (dirname.Length == 1)
                {
                    continue;
                }
                string CHAR_DIR = dirname[0].ToString().ToUpper();
                if (!Directory.Exists(src + "\\" + CHAR_DIR + "\\"))
                {
                    CreateDir("\\" + CHAR_DIR, true);
                }
                
                Directory.Move(dir, src + "\\" + CHAR_DIR + "\\" + dirname);
            }
        }

        static void SortROMs(List<string> files, int i)
        {
            // Lastly, we sort each ROM image into alphabetical subdirectories.
            // For ROM images not starting with a letter, we sort them into the subdirectories 'NUM' and 'SYM', for numbers and symbols respectively.
            // These are taken from first letter of the filename.
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string CHAR_DIR = "\\" + fileName.ToUpper()[0] + "\\";
                string dest;
                if (char.IsDigit(fileName[0]))
                {
                    if (!Directory.Exists(src + "\\NUM\\"))
                    {
                        CreateDir("\\NUM\\", true);
                    }
                    dest = src + "\\NUM\\" + fileName;
                }
                else if (char.IsSymbol(fileName[0]))
                {
                    if (!Directory.Exists(src + "\\SYM\\"))
                    {
                        CreateDir("\\SYM\\", true);
                    }
                    dest = src + "\\SYM\\" + fileName;
                }
                else
                {
                    if (!Directory.Exists(src + "\\" + CHAR_DIR))
                    {
                        CreateDir(CHAR_DIR, true);
                    }
                    dest = src + CHAR_DIR + fileName;
                }
                Console.WriteLine("Moving file: " + fileName + " to " + dest + "\n" + i + " out of " + files.Count);
                File.Move(file, dest, true);
                i++;
            }
        }

        static void Main(string[] args)
        {
            // Set initial counter
            int i = 1;

            // Obtain initial layout
            files = Directory.GetFiles(src).ToList();

            // Step 1
            ExistingFiles(files);

            CreateDir("\\ZIPs\\", true);

            // Step 2
            ZIPExtractAndMove(files, i);

            // Rescan for changes
            files = Directory.GetFiles(src).ToList();

            // Reset counter
            i = 1;

            // Step 3
            SortDirectories();

            
            i = 1;
            // Step 4
            SortROMs(files, i);

            Console.WriteLine("Done");
        }
    }
}