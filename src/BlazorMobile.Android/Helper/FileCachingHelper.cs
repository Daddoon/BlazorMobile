
using BlazorMobile.Common.Helpers;
using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlazorMobile.Droid.Helper
{
    internal static class FileCachingHelper
    {
        public static string GetFileCacheFolder()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "blazormobile_temp");
        }

        private static object taskLock = new object();

        /// <summary>
        /// Clear cached file (mainly used for GeckoView FileInput workaround) if time elapsed condition is met
        /// NOTE: This operation will be done in an other thread in order to not block the UI during the process
        /// </summary>
        /// <param name="condition"></param>
        public static void ClearCache(TimeSpan condition)
        {
            Task.Run(() =>
            {
                lock (taskLock)
                {
                    bool deleteAll = false;
                    if (condition == TimeSpan.Zero)
                    {
                        deleteAll = true;
                    }

                    string cacheFolder = GetFileCacheFolder();

                    try
                    {
                        if (!Directory.Exists(cacheFolder))
                        {
                            return;
                        }

                        DirectoryInfo cacheFolderDir = new DirectoryInfo(cacheFolder);

                        FileInfo[] allFiles = cacheFolderDir.GetFiles();
                        foreach (FileInfo file in allFiles)
                        {
                            if (deleteAll || (DateTime.UtcNow - file.CreationTimeUtc) > condition)
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception ex)
                                {
                                    //We should still continue
                                    ConsoleHelper.WriteException(ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        ConsoleHelper.WriteException(ex1);
                    }
                }
            });
        }

        /// <summary>
        /// Clear cached file (mainly used for GeckoView FileInput workaround)
        /// NOTE: This operation will be done in an other thread in order to not block the UI during the process
        /// </summary>
        public static void ClearCache()
        {
            ClearCache(TimeSpan.Zero);
        }
    }
}