//Developed by www.codestack.net
//License: https://github.com/codestack-net-dev/pdm-copy-task-script/blob/master/LICENSE
 
using System;
using System.IO;
using System.Linq;

namespace CodeStack.Dev.PdmCopyTaskScript
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemWatcher watcher = null;

            if (args.Any())
            {
                var outPath = args.First();

                if (Directory.Exists(outPath))
                {
                    var path = Path.GetTempPath();

                    try
                    {
                        Console.WriteLine("Waiting for the debug macro. Press any key to exit...");
                        watcher = WaitForDebugMacro(path, outPath);
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        WriteError($"Failed to extract debug macro. {ex.Message}");
                    }
                    finally
                    {
                        if (watcher != null)
                        {
                            watcher.Dispose();
                        }
                    }
                }
                else
                {
                    WriteError($"Directory {outPath} doesn't exist");
                }
            }
            else
            {
                WriteError("Output directory is not specified. Specify the directory to copy debug macro as the first parameter");
            }
        }

        private static FileSystemWatcher WaitForDebugMacro(string path, string outPath)
        {
            var watcher = new FileSystemWatcher(path, "*.*");

            watcher.Renamed += (sender, e) =>
            {
                if (Path.GetExtension(e.FullPath).Equals(".swb",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    var destFile = Path.Combine(outPath, Path.GetFileName(e.FullPath));
                    File.Copy(e.FullPath, destFile);
                    Console.WriteLine($"Debug macro is copied to {destFile}");
                }
            };

            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private static void WriteError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }
    }
}
