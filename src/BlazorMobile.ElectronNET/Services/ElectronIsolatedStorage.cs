using BlazorMobile.Services;
using ElectronNET.API;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace BlazorMobile.ElectronNET.Services
{
    public class ElectronIsolatedStorage : IIsolatedStorageFile
    {
        private static string _cachedUserDataDir = null;

        internal static string GetUserDataDirectory()
        {
            //We only need to compute this once
            if (_cachedUserDataDir == null)
            {
                _cachedUserDataDir = ElectronMetadata.UserDataPath;
            }

            return _cachedUserDataDir;
        }

        private string GetAbsolutePath(string path)
        {
            return Path.Combine(GetUserDataDirectory(), path);
        }

        public async Task CreateDirectoryAsync(string path)
        {
            string absolutePath = GetAbsolutePath(path);

            try
            {
                if (!Directory.Exists(absolutePath))
                {
                    Directory.CreateDirectory(absolutePath);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> GetDirectoryExistsAsync(string path)
        {
            string absolutePath = GetAbsolutePath(path);

            try
            {
                return Directory.Exists(absolutePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> GetFileExistsAsync(string path)
        {
            string absolutePath = GetAbsolutePath(path);

            try
            {
                return File.Exists(absolutePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
        {
            string absolutePath = GetAbsolutePath(path);

            try
            {
                return File.GetLastWriteTime(absolutePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
        {
            string absolutePath = GetAbsolutePath(path);

            try
            {
                return File.Open(absolutePath, mode, access);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
        {
            string absolutePath = GetAbsolutePath(path);

            try
            {
                return File.Open(absolutePath, mode, access, share);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
