/// <summary>
/// This simple program will count the lines of code contained within a directory
/// </summary>


using System;
using System.Collections.Generic;
using System.IO;

namespace Code_Lines
{
    public class Program
    {
        static Dictionary<string, int> linesCount = new Dictionary<string, int>();
        static List<string> extensions = new List<string> { ".cs", ".java", ".py", ".c", ".cpp", ".js",".cfm",".cfc",".html" };
        static Dictionary<string, string> languages = new Dictionary<string, string>();
        static Dictionary<string, string> directories = new Dictionary<string, string>();
        static Dictionary<string, int> fileCount = new Dictionary<string, int>();
        private static int directoryCount = 0;
        private static int totalFileCount = 0;

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a directory's full path as a command-line argument.");
                Console.WriteLine("Example:\n" + @"Code-Line C:\Users\<Your username>\Desktop");
                Console.WriteLine("If your directory has spaces in the path, enclose entire path in quotation marks.");
                Console.WriteLine("If you want to use the console's current directory, use:");
                Console.WriteLine("Code-Lines -c\nPress any key to close...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                // Directory should be arg[0] or if -c is passed, get directory that called the application
                string rootDir = (args[0] == "-c" ? Directory.GetCurrentDirectory() : args[0]);

                PopulateDictionaries();
                rootDir = rootDir.Replace("\"", "");

                Console.WriteLine(rootDir);

                VerifyDirExists(rootDir);
                ExpandDirectories(rootDir);
                CaptureLineCountDetailsByDirectory(directories);
                PrintResults();
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void CaptureLineCountDetailsByDirectory(Dictionary<string, string> directories)
        {
            foreach (var dir in directories)
            {
                FindFilesInDirectory(dir.Value);
            }
        }

        /// <summary>
        /// Verify that directory is a real directory
        /// </summary>
        /// <param name="directory">Full path to directory being checked</param>
        static void VerifyDirExists(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            try
            {
                if (!dir.Exists)
                {
                    Console.WriteLine("Error:\n" + dir.ToString() + "\nwas not found.");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                else
                {
                    Console.WriteLine("Directory exists");
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Recursively gets all sub-directories of root and performs line counting of files with correct extensions.
        /// </summary>
        /// <param name="directory">Full path to directory being counted</param>
        static void ExpandDirectories(string directory)
        {
            try
            {
                try
                {
                    directories.Add(directory, directory);
                        }
                catch(Exception e)
                {

                    //ignore if already added.
                }
              //  directoryCount++;
                // directories.Add
                //  FindFilesInDirectory(directory);
                foreach (string dir in Directory.GetDirectories(directory))
                {
                    directoryCount++;
                    try
                    {
                        directories.Add(dir, dir);
                    }
                    catch (Exception e)
                    {
                            //getting duplicate dictonary item exception. ignoring for now.
                    }



                    ExpandDirectories(dir);
                }
            }
            //catch (Exception e) when ( e is PathTooLongException
            //    || e is UnauthorizedAccessException )
            //{
            //    // Do nothing and skip
            //}
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void FindFilesInDirectory(string directory)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                Console.Write("\rDirectories checked: \t{0}\n", directoryCount);
                Console.Write("\rFiles checked: \t\t{0}", totalFileCount++);
                Console.CursorTop--;
                FileInfo fileInfo = new FileInfo(file);
                if (extensions.Contains(fileInfo.Extension))
                {
                    linesCount[fileInfo.Extension] += GetLineCount(file);
                    fileCount[fileInfo.Extension]++;
                }
            }
        }


        /// <summary>
        /// Initialise dictionaries with default values
        /// </summary>
        static void PopulateDictionaries()
        {
            foreach (string ext in extensions)
            {
                linesCount.Add(ext, 0);
                languages.Add(ext, null);
                fileCount.Add(ext, 0);
            }
            languages[".cs"] = "C#";
            languages[".java"] = "Java";
            languages[".py"] = "Python";
            languages[".c"] = "C";
            languages[".cpp"] = "C++";
            languages[".js"] = "JavaScript";
            languages[".cfm"] = "Cold Fusion(.cfm)";
            languages[".html"] = "HTML(.html)";
            languages[".cfc"] = "Cold Fusion Component(.cfc)";
            languages[".bat"] = "Batch file(.bat)";
            languages[".js"] = "Javascript";
        }

        /// <summary>
        /// Reads a file line by line
        /// </summary>
        /// <param name="file">Name of file to be analysed</param>
        /// <returns>Number of lines in the file</returns>
        static int GetLineCount(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            int count = 0;
            try
            {
                while (!sr.EndOfStream)
                {
                    sr.ReadLine();
                    count++;
                }
            }
            catch (IOException e)
            {
                System.Console.WriteLine(e.Message);
            }
            finally
            {
                sr.Close();
            }
            return count;
        }

        /// <summary>
        /// Prints a formatted summary of results to the console
        /// </summary>
        static void PrintResults()
        {
            Console.CursorTop += 2;    // new line
            Console.WriteLine("\r\nAnalysis:");

            foreach ( var dir in directories)
            {
                Console.WriteLine(dir.Value.ToString());

            }
            foreach (KeyValuePair<string, int> entry in linesCount)
            {
                if (entry.Value != 0)
                {
                    System.Console.WriteLine("Total lines of {0, -20} {1, -7} {2}.",
                     languages[entry.Key] + ":",
                     entry.Value,
                     "in " + fileCount[entry.Key] + " files");
                }
            }
        }
    }
}
