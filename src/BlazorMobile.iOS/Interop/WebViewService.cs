using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using Foundation;
using UIKit;
using WebKit;

namespace BlazorMobile.iOS.Interop
{
    public class WebViewService : IWebViewService
    {
        private void DeleteCachedFiles()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                {
                    WKWebsiteDataStore.DefaultDataStore.FetchDataRecordsOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, (NSArray records) =>
                    {
                        for (nuint i = 0; i < records.Count; i++)
                        {
                            var record = records.GetItem<WKWebsiteDataRecord>(i);

                            WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(record.DataTypes,
                                new[] { record }, () => { /* Nothing to do after completion */  });
                        }
                    });
                }

                // Remove the basic cache.
                NSUrlCache.SharedCache.RemoveAllCachedResponses();

                // Clear web cache
                DeleteLibraryFolderContents("Caches");

                // Removes all app cache storage.
                DeleteLibraryFolder("WebKit");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error deleting cache {ex.Message}");
            }

        }

        private void DeleteLibraryFolder(string folderName)
        {
            var manager = NSFileManager.DefaultManager;
            var library = manager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User).First();
            var dir = Path.Combine(library.Path, folderName);

            manager.Remove(dir, out NSError error);
            if (error != null)
            {
                ConsoleHelper.WriteError(error.Description);
            }
        }

        private void DeleteLibraryFolderContents(string folderName)
        {
            var manager = NSFileManager.DefaultManager;
            var library = manager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User).First();
            var dir = Path.Combine(library.Path, folderName);
            var contents = manager.GetDirectoryContent(dir, out NSError error);
            if (error != null)
            {
                ConsoleHelper.WriteError(error.Description);
            }

            foreach (var c in contents)
            {
                try
                {
                    manager.Remove(Path.Combine(dir, c), out NSError errorRemove);
                    if (errorRemove != null)
                    {
                        ConsoleHelper.WriteError(error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteError($"Error deleting folder contents: {folderName}{Environment.NewLine}{ex.Message}");
                }
            }
        }

        public void ClearWebViewData()
        {
            try
            {
                DeleteCachedFiles();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
        }

        public void ClearCookies()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                {
                    NSHttpCookieStorage.SharedStorage.RemoveCookiesSinceDate(NSDate.DistantPast);
                }
                else
                {
                    var cookies = NSHttpCookieStorage.SharedStorage.Cookies;

                    foreach (var c in cookies)
                    {
                        NSHttpCookieStorage.SharedStorage.DeleteCookie(c);
                    }
                }

                // Remove all cookies stored by the site. This includes localStorage, sessionStorage, and WebSQL/IndexedDB.
                DeleteLibraryFolderContents("Cookies");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error deleting cookies {ex.Message}");
            }
        }
    }
}