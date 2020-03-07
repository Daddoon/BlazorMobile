using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlazorMobile.Build.Cli.Helper
{
    public static class DirectoryHelper
    {
        public static IEnumerable<string> GetFiles(string root, string searchPattern, List<string> filters = null)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                IEnumerable<string> next = null;
                try
                {
                    next = Directory.EnumerateFiles(path, searchPattern);
                }
                catch { }
                if (next != null && next.Count() != 0)
                    foreach (var file in next) yield return file;
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next)
                    {
                        if (filters == null)
                            pending.Push(subdir);
                        else
                        {
                            string lastDirName = Path.GetFileName(subdir);
                            bool filterFound = filters.Any(p => lastDirName.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);

                            //Filtered directories not found, so we can reference the current directory
                            if (!filterFound)
                                pending.Push(subdir);
                        }
                    }
                }
                catch { }
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
