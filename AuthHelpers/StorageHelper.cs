// -----------------------------------------------------------------------
// <copyright file="StorageHelper.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
#if !(NETFX_CORE || WINDOWS_PHONE)
using System.IO.IsolatedStorage;
#endif
using System.Threading.Tasks;
#if (NETFX_CORE || WINDOWS_PHONE)
using Windows.Storage;
#endif

namespace MixRadio.AuthHelpers
{
    /// <summary>
    /// StorageHelper is a high-level isolated storage abstraction
    /// </summary>
    internal static class StorageHelper
    {
#pragma warning disable 1998  // Disable async warnings for test code
        /// <summary>
        /// Checks if a file exists
        /// </summary>
        /// <param name="fileName">The file to check</param>
        /// <returns>Whether the file exists</returns>
        public static async Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
#if (NETFX_CORE || WINDOWS_PHONE)
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return file != null;
#else
                return IsolatedStorageFile.GetUserStoreForAssembly().FileExists(fileName);
#endif
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Returns the text from a file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The string contents or null</returns>
        public static async Task<string> ReadTextAsync(string fileName)
        {
            if (await FileExistsAsync(fileName))
            {
#if (NETFX_CORE || WINDOWS_PHONE)
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                if (file != null)
                {
                    using (StreamReader sr = new StreamReader(await file.OpenStreamForReadAsync()))
                    {
                        return sr.ReadToEnd();
                    }
                }
#else
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (StreamReader sr = new StreamReader(store.OpenFile(fileName, FileMode.Open)))
                    {
                        return sr.ReadToEnd();
                    }
                }
#endif
            }

            return null;
        }

        /// <summary>
        /// Writes text to a file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">The content.</param>
        /// <returns>An async task</returns>
        public static async Task WriteTextAsync(string fileName, string content)
        {
#if (NETFX_CORE || WINDOWS_PHONE)
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (StreamWriter sw = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                sw.Write(content);
            }
#else
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (StreamWriter sw = new StreamWriter(store.OpenFile(fileName, FileMode.Create, FileAccess.Write)))
                {
                    sw.Write(content);
                }
            }
#endif
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="fileName">The file to delete</param>
        /// <returns>An async task</returns>
        public static async Task DeleteFileAsync(string fileName)
        {
#if (NETFX_CORE || WINDOWS_PHONE)
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            if (file != null)
            {
                await file.DeleteAsync();
            }
#else
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (store.FileExists(fileName))
                {
                    store.DeleteFile(fileName);
                }
            }
#endif
        }
#pragma warning restore 1998
    }
}
