﻿using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework.IO;
using WhatThe.Mods.CitiesSkylines.NetworkTextures.Pieces;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.Util
{
    /// <summary>
    /// File system helper.
    /// </summary>
    internal static class FileSystem
    {
        /// <summary>
        /// Check if file exists, with file name automatic.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// True if file exists.
        /// </returns>
        public static bool Exists(string subDirectory, string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = FilePathName(subDirectory, ".tmp");
            }
            else if (fileName[0] == '.')
            {
                fileName = FilePathName(subDirectory, fileName);
            }

            return File.Exists(fileName);
        }

        /// <summary>
        /// Check if file exists, with file name automatic.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if file exists.</returns>
        public static bool Exists(string fileName = null)
        {
            return Exists(null, fileName);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The name of the file.</returns>
        public static string FileName(string extension = "")
        {
            return Library.Name + extension;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <returns>The file path.</returns>
        public static string FilePath(string subDirectory = null)
        {
            string path = Path.Combine(DataLocation.localApplicationData, "ModConfig");

            if (!String.IsNullOrEmpty(subDirectory))
            {
                if (subDirectory[0] == '.')
                {
                    subDirectory = Library.Name + subDirectory;
                }

                path = Path.Combine(path, subDirectory);
            }

            return path;
        }

        /// <summary>
        /// Gets the complete path.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The complete path.
        /// </returns>
        public static string FilePathName(string subDirectory, string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = FileName(".tmp");
            }
            else if (fileName[0] == '.')
            {
                fileName = FileName(fileName);
            }

            return Path.GetFullPath(Path.Combine(FilePath(subDirectory), fileName));
        }

        /// <summary>
        /// Gets the complete path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The complete path.</returns>
        public static string FilePathName(string fileName = null)
        {
            return FilePathName(null, fileName);
        }

        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="append">If set to <c>true</c> append to file.</param>
        /// <returns>
        /// The open file writer.
        /// </returns>
        public static StreamWriter OpenStreamWriter(string subDirectory, string fileName, bool append = false)
        {
            string filePathName = FileSystem.FilePathName(subDirectory, fileName);
            string filePath = Path.GetDirectoryName(filePathName);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            return new StreamWriter(filePathName, append);
        }

        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="append">If set to <c>true</c> append to file.</param>
        /// <returns>
        /// The open file writer.
        /// </returns>
        public static StreamWriter OpenStreamWriter(string fileName = null, bool append = false)
        {
            return OpenStreamWriter(null, fileName, append);
        }

        /// <summary>
        /// Saves the binary data.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="bytes">The bytes.</param>
        /// <returns>
        /// True on success.
        /// </returns>
        public static bool SaveBytes(string subDirectory, string fileName, byte[] bytes)
        {
            Log.Debug(typeof(FileSystem), "SaveBytes", subDirectory, fileName, bytes.LongLength);

            if (bytes.LongLength == 0)
            {
                return false;
            }

            try
            {
                string filePathName = FileSystem.FilePathName(subDirectory, fileName);
                string filePath = Path.GetDirectoryName(filePathName);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                File.WriteAllBytes(filePathName, bytes);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(typeof(FileSystem), "SaveList", ex, subDirectory, fileName, bytes.LongLength);
                return false;
            }
        }

        /// <summary>
        /// Saves the binary data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="bytes">The bytes.</param>
        /// <returns>
        /// True on success.
        /// </returns>
        public static bool SaveBytes(string fileName, byte[] bytes)
        {
            return SaveBytes(null, fileName, bytes);
        }

        /// <summary>
        /// Saves the binary data.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>
        /// True on success.
        /// </returns>
        public static bool SaveBytes(byte[] bytes)
        {
            return SaveBytes(null, null, bytes);
        }

        /// <summary>
        /// Saves the list.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="list">The list.</param>
        /// <param name="append">If set to <c>true</c> append to file.</param>
        /// <returns>True on success.</returns>
        public static bool SaveList(string fileName, List<string> list, bool append = false)
        {
            return SaveList(null, fileName, list, append);
        }

        /// <summary>
        /// Saves the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="append">If set to <c>true</c> append to file.</param>
        /// <returns>
        /// True on success.
        /// </returns>
        public static bool SaveList(List<string> list, bool append = false)
        {
            return SaveList(null, null, list, append);
        }

        /// <summary>
        /// Saves the list.
        /// </summary>
        /// <param name="subDirectory">The sub directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="list">The list.</param>
        /// <param name="append">If set to <c>true</c> append to file.</param>
        /// <returns>True on success.</returns>
        public static bool SaveList(string subDirectory, string fileName, List<string> list, bool append = false)
        {
            Log.Debug(typeof(FileSystem), "SaveList", subDirectory, fileName, list.Count, append);

            if (list.Count == 0)
            {
                return false;
            }

            try
            {
                using (StreamWriter listFile = FileSystem.OpenStreamWriter(subDirectory, fileName, append))
                {
                    listFile.Write(String.Join("", list.ToArray()));
                    listFile.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(typeof(FileSystem), "SaveList", ex, subDirectory, fileName, list.Count, append);
                return false;
            }
        }
    }
}